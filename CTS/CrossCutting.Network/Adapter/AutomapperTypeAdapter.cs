using AutoMapper;
using CrossCutting.Adapters;
using System;
using System.IO;
using System.Linq;

namespace CrossCutting.Network.Adapter
{
    public class AutomapperTypeAdapter : ITypeAdapter
    {
        public TTarget Adapt<TSource, TTarget>(TSource source)
            where TSource : class
            where TTarget : class, new()
        {
            try
            {
                var projection = Mapper.Map<TSource, TTarget>(source);
                return projection;
            }
            catch
            {
                ConfigureMappings(typeof(TTarget).FullName);
                return Mapper.Map<TSource, TTarget>(source);
            }
        }

        public TTarget Adapt<TTarget>(object source) where TTarget : class, new()
        {
            try
            {
                var projection = Mapper.Map<TTarget>(source);
                return projection;
            }
            catch
            {
                ConfigureMappings(typeof(TTarget).FullName);
                return Mapper.Map<TTarget>(source);
            }
        }

        private void ConfigureMappings(string type)
        {
            var logMessage = string.Format("{0}\t{1}\t{2}\t{3}", "Inicio Reconfiguracion de  AutoMapper", DateTime.Now,
                                           type, Environment.NewLine);

            Mapper.Reset();

            //scan all assemblies finding Automapper Profile
            var profiles = AppDomain.CurrentDomain
                                    .GetAssemblies()
                                    .SelectMany(a => a.GetTypes())
                                    .Where(t => t.BaseType == typeof(Profile));

            Mapper.Initialize(cfg =>
            {
                foreach (var item in profiles)
                {
                    if (item.FullName != "AutoMapper.SelfProfiler`2")
                        cfg.AddProfile(Activator.CreateInstance(item) as Profile);
                }
            });

            logMessage += string.Format("{0}\t{1}\t{2}", "Final Reconfiguracion de  AutoMapper", DateTime.Now, Environment.NewLine);
            LogError(logMessage);

        }

        private void LogError(string logMessage)
        {
            //Create a writer and open the file:
            //StreamWriter log = null;
            const string logFile = @"c:\temp\logfile.txt";

            try
            {
                using (var w = File.AppendText(logFile))
                {
                    Log(logMessage, w);
                    // Close the writer and underlying file.
                    w.Close();
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        private static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
            w.WriteLine("  :");
            w.WriteLine("  :{0}", logMessage);
            w.WriteLine("-------------------------------");
            // Update the underlying file.
            w.Flush();
        }
    }
}
