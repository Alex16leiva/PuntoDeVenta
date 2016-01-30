using CrossCutting.Identity;
using CrossCutting.Logging;
using Dominio.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Transaction = CrossCutting.Logging.Transaction;

namespace Infraestructura.Core
{
    public class BCUnitOfWork : DbContext
    {
        public BCUnitOfWork(string connectionString)
            :base(connectionString)
        {
            Database.SetInitializer<BCUnitOfWork>(null);
        }
        public BCUnitOfWork()
        {
            Database.SetInitializer<BCUnitOfWork>(null);
        }

        #region Implementacion IQuereyableUnitOfWork

        public DbSet<TEntity> CreateSet<TEntity>() where TEntity : class
        {
            return Set<TEntity>();
        }

        #endregion

        #region Implementacion IUnitOfWork

        public virtual void Commit(TransactionInfo transactionInfo)
        {
            #region Check Prerequisites

            if (transactionInfo == null) throw new ArgumentNullException("transactionInfo");
            if (string.IsNullOrWhiteSpace(transactionInfo.TipoTransaccion)) throw new ArgumentException("transactionInfo.TipoTransaccion");
            if (string.IsNullOrWhiteSpace(transactionInfo.ModificadoPor)) throw new ArgumentException("transactionInfo.ModificadoPor");

            #endregion

            var transaction = BuildTransactionInfo(transactionInfo);

            Commit(transaction);
        }

        private Transaction BuildTransactionInfo(TransactionInfo transactionInfo)
        {
            var transactionId = IdentityFactory.CreateIdentity().NewSequentialTransactionIdentity();
            return new Transaction
            {
                TransactionId = transactionId.TransactionId,
                TransactionDate = transactionId.TransactionDate,
                TransactionDateUtc = transactionId.TransactionUtcDate,
                ModifiedBy = transactionInfo.ModificadoPor,
                TransactionType = transactionInfo.TipoTransaccion
            };
        }

        public virtual void Commit(Transaction transaction)
        {
            #region Check Prerequisites

            if (transaction == null) throw new ArgumentNullException("transaction");
            if (string.IsNullOrWhiteSpace(transaction.TransactionType)) throw new ArgumentException("transactionInfo.TipoTransaccion");
            if (string.IsNullOrWhiteSpace(transaction.ModifiedBy)) throw new ArgumentException("transactionInfo.ModificadoPor");

            #endregion

            var objectContext = ((IObjectContextAdapter)this).ObjectContext;
            try
            {
                objectContext.Connection.Open();

                // Resetenado el detalla de las transacciones.
                transaction.TransactionDetail = new List<TransactionDetail>();

                using (var scope = TransactionScopeFactory.GetTransactionScope())
                {
                    var changedEntities = new List<ModifiedEntityEntry>();
                    var tableMapping = new List<EntityMapping>();
                    var sqlCommandInfos = new List<SqlCommandInfo>();

                    // Get ebtities with changes.
                    var changedDbEntityEntries = GetChangedDbEntityEntries();

                    // Apply transaction info.
                    foreach (var entry in changedDbEntityEntries)
                    {
                        ApplyTransactionInfo(transaction, entry);

                        // Get the deleted records info first
                        if (entry.State == EntityState.Deleted)
                        {
                            var entityMapping = GetEntityMappingConfiguration(tableMapping, entry);
                            var sqlCommandInfo = GetSqlCommandInfo(transaction, entry, entityMapping);
                            if (sqlCommandInfo != null) sqlCommandInfos.Add(sqlCommandInfo);

                            transaction.AddDetail(entityMapping.TableName, entry.State.ToString(), transaction.TransactionType);
                        }
                        else
                        {
                            changedEntities.Add(new ModifiedEntityEntry(entry, entry.State.ToString()));
                        }
                    }

                    base.SaveChanges();


                    // Get the Added and Mdified records after changes, that way we will be able to get the generated .
                    foreach (var entry in changedEntities)
                    {
                        var entityMapping = GetEntityMappingConfiguration(tableMapping, entry.EntityEntry);
                        var sqlCommandInfo = GetSqlCommandInfo(transaction, entry.EntityEntry, entityMapping);
                        if (sqlCommandInfo != null) sqlCommandInfos.Add(sqlCommandInfo);

                        transaction.AddDetail(entityMapping.TableName, entry.State, transaction.TransactionType);
                    }

                    // Adding Audit Detail Transaction CommandInfo.
                    sqlCommandInfos.AddRange(GetAuditRecords(transaction));


                    // Insert Transaction and audit records.
                    foreach (var sqlCommandInfo in sqlCommandInfos)
                    {
                        Database.ExecuteSqlCommand(sqlCommandInfo.Sql, sqlCommandInfo.Parameters);
                    }

                    scope.Complete();
                }
            }
            finally
            {
                objectContext.Connection.Close();
            }
        }

        private IEnumerable<DbEntityEntry> GetChangedDbEntityEntries()
        {
            return ChangeTracker.Entries().Where(
                e =>
                e.Entity is Entity &&
                (e.State == EntityState.Modified || e.State == EntityState.Added || e.State == EntityState.Deleted));
        }

        private void ApplyTransactionInfo(Transaction transaction, DbEntityEntry entry)
        {
            ((Entity)entry.Entity).FechaTransaccion = transaction.TransactionDate;
            ((Entity)entry.Entity).DescripcionTransaccion = entry.State.ToString();
            ((Entity)entry.Entity).ModificadoPor = transaction.ModifiedBy;

            AplicarInformacionTransaccion(entry, "TipoTransaccion", transaction.TransactionType);
            AplicarInformacionTransaccion(entry, "TransaccionUId", transaction.TransactionId);
            AplicarInformacionTransaccion(entry, "FechaTransaccionUtc", transaction.TransactionDateUtc);
        }

        private void AplicarInformacionTransaccion(DbEntityEntry item, string nombrePropiedad, object valorPropiedad)
        {
            if (item != null && item.Entity != null)
            {
                var propInfoEntity = item.Entity.GetType().GetProperty(nombrePropiedad);
                if (propInfoEntity != null)
                {
                    propInfoEntity.SetValue(item.Entity, valorPropiedad, null);
                }
            }
        }

        private EntityMapping GetEntityMappingConfiguration(List<EntityMapping> tableMapping, DbEntityEntry entry)
        {
            var type = GetDomainEntityType(entry);
            var entityMapping = tableMapping.FirstOrDefault(m => m.EntityType == type);
            if (entityMapping == null)
            {
                string sql = Set(type).ToString();
                var regex = new Regex("FROM (?<table>.*) AS");

                var match = regex.Match(sql);
                var tname = match.Groups["table"].Value;

                entityMapping = CreateTableMapping(type, tname);
                tableMapping.Add(entityMapping);
            }
            return entityMapping;
        }

        private Type GetDomainEntityType(DbEntityEntry entry)
        {
            var type = entry.Entity.GetType();
            if (type.FullName.Contains("Dominio") || type.FullName.Contains("T4"))
            {
                return type;
            }
            return type.BaseType;
        }

        private EntityMapping CreateTableMapping(Type type, string tname)
        {
            return new EntityMapping
            {
                EntityType = type,
                TableName = tname,
                TransactionTableName = GetTransactionTableName(tname)
            };
        }

        private string GetTransactionTableName(string tname)
        {
            if (tname.Contains("_Transacciones"))
            {
                return tname;
            }

            string[] nameArray = tname.Split('.');
            string name = nameArray[1].Replace("[", "").Replace("]", "");

            int place = tname.LastIndexOf(name, StringComparison.Ordinal);

            string result = tname.Remove(place, name.Length).Insert(place, string.Format("{0}_Transacciones", name));
            return result;
        }

        private SqlCommandInfo GetSqlCommandInfo(Transaction transaction, DbEntityEntry entry, EntityMapping entityMapping)
        {
            if (entityMapping.TableName.Contains("_Transacciones"))
            {
                return null;
            }

            string sqlInsert;
            object[] param;
            CreateTransactionInsertStatement(entityMapping, entry, transaction, out sqlInsert, out param);

            var sqlCommandInfo = new SqlCommandInfo(sqlInsert, param);
            return sqlCommandInfo;
        }

        private void CreateTransactionInsertStatement(EntityMapping entityMapping, DbEntityEntry entry,
                                    Transaction transaction, out string sqlInsert, out object[] objects)
        {
            var insert = new StringBuilder();
            var fields = new StringBuilder();
            var paramNames = new StringBuilder();
            var values = new List<Object>();

            insert.AppendLine(string.Format("Insert Into {0} ", entityMapping.TransactionTableName));

            int index = 0;
            IEnumerable<string> propertyNames = entry.State == EntityState.Deleted
                                                    ? entry.OriginalValues.PropertyNames
                                                    : entry.CurrentValues.PropertyNames;

            foreach (string property in propertyNames)
            {
                string prop = property;
                if (prop != "RowVersion")
                {
                    if (fields.Length == 0)
                    {
                        fields.Append(string.Format(" ({0}", prop));
                        paramNames.Append(string.Format(" values ({0}{1}{2}", "{", index, "}"));
                    }
                    else
                    {
                        fields.Append(string.Format(", {0}", prop));
                        paramNames.Append(string.Format(", {0}{1}{2}", "{", index, "}"));
                    }

                    values.Add(GetEntityPropertyValue(entry, prop, transaction));
                    index++;
                }
            }

            fields.Append(string.Format(") "));
            paramNames.Append(string.Format(") "));

            insert.AppendLine(fields.ToString());
            insert.AppendLine(paramNames.ToString());

            sqlInsert = insert.ToString();
            objects = values.ToArray();
        }

        private object GetEntityPropertyValue(DbEntityEntry entry, string prop, Transaction transaction)
        {
            object value;
            TryGeTransactionInfo(prop, transaction, out value);
            if (value != null)
            {
                return value;
            }

            if (entry.State == EntityState.Deleted || entry.State == EntityState.Detached)
            {
                return prop == "DescripcionTransaccion"
                           ? EntityState.Deleted.ToString()
                           : entry.Property(prop).OriginalValue;
            }
            return entry.Property(prop).CurrentValue;
        }

        private void TryGeTransactionInfo(string property, Transaction transaction, out object value)
        {
            switch (property)
            {
                case "TransaccionUId":
                    value = transaction.TransactionId;
                    break;
                case "TipoTransaccion":
                    value = transaction.TransactionType;
                    break;
                case "FechaTransaccion":
                    value = transaction.TransactionDate;
                    break;
                case "FechaTransaccionUtc":
                    value = transaction.TransactionDateUtc;
                    break;
                case "ModificadoPor":
                    value = transaction.ModifiedBy;
                    break;
                default:
                    value = null;
                    break;
            }
        }

        private IEnumerable<SqlCommandInfo> GetAuditRecords(Transaction transaction)
        {
            var auditCommands = new List<SqlCommandInfo> { GetAuditHeaderCommandInfo(transaction) };

            // Adding Audit Detail Transaction CommandInfo
            foreach (var transactionDetail in transaction.TransactionDetail)
            {
                auditCommands.Add(GetAuditDetailCommandInfo(transactionDetail));
            }

            return auditCommands;
        }

        private SqlCommandInfo GetAuditHeaderCommandInfo(Transaction transaction)
        {
            const string sqlInsert =
                "insert into  Comunes.LogTransacciones(TransaccionUId, TipoTransaccion, FechaTransaccion, FechaTransaccionUtc, " +
                "ModificadoPor) " +
                "values({0}, {1}, {2}, {3}, {4} )";

            var param = new object[]
                                 {
                                     transaction.TransactionId, transaction.TransactionType, transaction.TransactionDate,
                                     transaction.TransactionDateUtc, transaction.ModifiedBy
                                 };

            return new SqlCommandInfo(sqlInsert, param);
        }

        private SqlCommandInfo GetAuditDetailCommandInfo(TransactionDetail transactionDetail)
        {
            const string sqlInsert =
                "insert into  Comunes.LogTransaccionesDetalle(TransaccionUId,TipoTransaccion, EntidadDominio, DescripcionTransaccion) " +
                                       "values({0}, {1}, {2},{3})";

            var param = new object[]
                                 {
                                     transactionDetail.TransactionId,transactionDetail.TransactionType, transactionDetail.TableName, transactionDetail.CrudOperation
                                 };

            return new SqlCommandInfo(sqlInsert, param);
        }

        #endregion
    }
}
