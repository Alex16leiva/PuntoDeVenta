using AutoMapper;
using CrossCutting.Logging;
using System;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Security;
using System.Text;

namespace CrossCutting.Network.Logging
{
    public sealed class TraceSourceLog : ILogger
    {
        readonly TraceSource _source;

        /// <summary>
        /// Create a new instance of this trace manager
        /// </summary>
        public TraceSourceLog()
        {
            // Create default source
            _source = new TraceSource("CTS.NET");
        }


        /// <summary>
        /// Trace internal message in configured listeners
        /// </summary>
        /// <param name="eventType">Event type to trace</param>
        /// <param name="message">Message of event</param>
        void TraceInternal(TraceEventType eventType, string message)
        {

            if (_source != null)
            {
                try
                {
                    var connectionString = ConfigurationManager.ConnectionStrings["connectionString"];
                    if (connectionString != null)
                    {
                        var moduleName = ExtractModuleNameFromMessage(message);

                        var connection = new SqlConnection(connectionString.ConnectionString);
                        const string sql = "insert into Comunes.LogEventosExcepciones(EventType, ModuleName, Message, Date) " +
                                           "values(@EventTYpe, @ModuleName, @Message, @Date)";

                        using (connection)
                        {
                            var command = new SqlCommand(sql, connection);
                            command.Parameters.AddWithValue("@EventTYpe", eventType);
                            command.Parameters.AddWithValue("@ModuleName", moduleName);
                            command.Parameters.AddWithValue("@Message", message);
                            command.Parameters.AddWithValue("@Date", DateTime.Now);
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                    }

                    _source.TraceEvent(eventType, (int)eventType, message);
                }
                catch (SecurityException)
                {
                    //Cannot access to file listener or cannot have
                    //privileges to write in event log etc...
                }

            }
        }

        private string ExtractModuleNameFromMessage(string errorMessage)
        {

            string[] lines = errorMessage.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (line.Contains("Application") || line.Contains("DistributedServices"))
                {
                    var words = line.Split('.');
                    if (words[0].Contains("CTS") && words[1].Contains("NET") && words.Length > 3)
                    {
                        return words[2];
                    }
                }
            }

            return "UnKnow Module";
        }

        public void LogInfo(string message, params object[] args)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                var messageToTrace = string.Format(CultureInfo.InvariantCulture, message, args);

                TraceInternal(TraceEventType.Information, messageToTrace);
            }
        }

        public void LogWarning(string message, params object[] args)
        {

            if (!string.IsNullOrWhiteSpace(message))
            {
                var messageToTrace = string.Format(CultureInfo.InvariantCulture, message, args);

                TraceInternal(TraceEventType.Warning, messageToTrace);
            }
        }

        public void LogError(string message, params object[] args)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                var messageToTrace = (args != null && args.Length > 0)
                    ? string.Format(CultureInfo.InvariantCulture, message, args)
                    : message;

                TraceInternal(TraceEventType.Error, messageToTrace);
            }
        }

        public void LogError(string message, Exception exception, params object[] args)
        {
            if (!string.IsNullOrWhiteSpace(message)
                &&
                exception != null)
            {
                var messageToTrace = (args != null && args.Length > 0)
                    ? string.Format(CultureInfo.InvariantCulture, message, args)
                    : message;

                var exceptionData = exception.ToString(); // The ToString() create a string representation of the current exception



                // find the first InnerException that's not wrapped
                var innerExceptionData = new StringBuilder();
                var exceptionEsAutoMapperMappingException = false;
                while (exception is AutoMapperMappingException)
                {
                    exceptionEsAutoMapperMappingException = true;
                    exception = exception.InnerException;
                }

                if (exception != null && exceptionEsAutoMapperMappingException)
                {
                    innerExceptionData.AppendLine(exception.ToString());

                }

                if (exception != null && exception.GetType() == typeof(DbEntityValidationException))
                {
                    innerExceptionData.AppendLine();
                    innerExceptionData.AppendLine();
                    innerExceptionData.AppendLine("Entity Validation Errors");

                    foreach (var error in ((DbEntityValidationException)exception).EntityValidationErrors)
                    {
                        //var entry = error.Entry;
                        foreach (var err in error.ValidationErrors)
                        {
                            innerExceptionData.AppendLine(string.Format("{0} -> {1} -> {2}", error.Entry.Entity, err.PropertyName, err.ErrorMessage));
                        }
                    }

                }

                var errorMess = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} Exception: \n{1}\n\n InnerException: \n{2}",
                        messageToTrace,
                        exceptionData,
                        innerExceptionData);

                TraceInternal(TraceEventType.Error, errorMess);

                var environmentName = ConfigurationManager.AppSettings["EnvironmentName"];
                if (environmentName == "HK-Production")
                {
                }
            }
        }

        public void LogTransaction(Transaction transaction)
        {
            SqlTransaction sqlTransaction = null;
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["CTS.NETDatabase"];
                if (connectionString != null)
                {
                    string sql = "insert into  Comunes.LogTransacciones(TransaccionUId, TipoTransaccion, FechaTransaccion, FechaTransaccionUtc, " +
                        "ModificadoPor, Aplicacion, Formulario, OrigenTransaccion, Computadora, UsuarioWindows) " +
                                       "values(@TransaccionUId, @TipoTransaccion, @FechaTransaccion, @FechaTransaccionUtc, " +
                        "@ModificadoPor, @Aplicacion, @Formulario, @OrigenTransaccion, @Computadora, @UsuarioWindows)";
                    using (var connection = new SqlConnection(connectionString.ConnectionString))
                    {
                        connection.Open();
                        sqlTransaction = connection.BeginTransaction();
                        using (var command = new SqlCommand(sql, connection))
                        {
                            command.Transaction = sqlTransaction;
                            // Inserting Transaction Header
                            command.Parameters.AddWithValue("@TransaccionUId", transaction.TransactionId);
                            command.Parameters.AddWithValue("@TipoTransaccion", transaction.TransactionType);
                            command.Parameters.AddWithValue("@FechaTransaccion", transaction.TransactionDate);
                            command.Parameters.AddWithValue("@FechaTransaccionUtc", transaction.TransactionDateUtc);
                            command.Parameters.AddWithValue("@ModificadoPor", transaction.ModifiedBy);

                            command.ExecuteNonQuery();
                            command.Parameters.Clear();
                            command.Parameters.Add("@TransaccionUId", SqlDbType.UniqueIdentifier);
                            command.Parameters.Add("@EntidadDominio", SqlDbType.VarChar);
                            command.Parameters.Add("@DescripcionTransaccion", SqlDbType.VarChar);
                            sql = "insert into  Comunes.LogTransaccionesDetalle(TransaccionUId, EntidadDominio, DescripcionTransaccion) " +
                                       "values(@TransaccionUId, @EntidadDominio, @DescripcionTransaccion)";
                            command.CommandText = sql;
                            foreach (var transactionDetail in transaction.TransactionDetail)
                            {
                                command.Parameters["@TransaccionUId"].Value = transactionDetail.TransactionId;
                                command.Parameters["@EntidadDominio"].Value = transactionDetail.TableName;
                                command.Parameters["@DescripcionTransaccion"].Value = transactionDetail.CrudOperation;
                                command.ExecuteNonQuery();
                            }
                        }
                        sqlTransaction.Commit();
                    }
                }
            }
            catch (SecurityException)
            {
                //Cannot access to file listener or cannot have
                //privileges to write in event log etc...
                if (sqlTransaction != null) sqlTransaction.Rollback();
                throw;
            }
        }

        public void Debug(string message, params object[] args)
        {
            if (!String.IsNullOrWhiteSpace(message))
            {
                var messageToTrace = string.Format(CultureInfo.InvariantCulture, message, args);

                if (_source != null)
                {
                    try
                    {
                        _source.TraceEvent(TraceEventType.Verbose, (int)TraceEventType.Verbose, messageToTrace);
                    }
                    catch (SecurityException)
                    {
                        //Cannot access to file listener or cannot have
                        //privileges to write in event log etc...
                    }
                }
            }
        }

        public void Debug(string message, Exception exception, params object[] args)
        {
            if (!String.IsNullOrWhiteSpace(message)
                &&
                exception != null)
            {
                var messageToTrace = string.Format(CultureInfo.InvariantCulture, message, args);

                var exceptionData = exception.ToString(); // The ToString() create a string representation of the current exception

                TraceInternal(TraceEventType.Error, string.Format(CultureInfo.InvariantCulture, "{0} Exception:{1}", messageToTrace, exceptionData));
            }
        }

        public void Debug(object item)
        {
            if (item != null)
            {
                TraceInternal(TraceEventType.Verbose, item.ToString());
            }
        }

        public void Fatal(string message, params object[] args)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                var messageToTrace = string.Format(CultureInfo.InvariantCulture, message, args);

                TraceInternal(TraceEventType.Critical, messageToTrace);
            }
        }

        public void Fatal(string message, Exception exception, params object[] args)
        {
            if (!string.IsNullOrWhiteSpace(message)
                &&
                exception != null)
            {
                var messageToTrace = string.Format(CultureInfo.InvariantCulture, message, args);

                var exceptionData = exception.ToString(); // The ToString() create a string representation of the current exception

                TraceInternal(TraceEventType.Critical, string.Format(CultureInfo.InvariantCulture, "{0} Exception:{1}", messageToTrace, exceptionData));
            }
        }

    }
}
