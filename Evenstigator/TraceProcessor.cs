using Microsoft.Windows.EventTracing;
using Microsoft.Windows.EventTracing.Processes;
using log4net;
using System;
using System.IO;

namespace Evenstigator
{
    static class MyTraceProcessor
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string _fileExt = ".etl";
        private static string _etlFileName;
        private static string _etlFilePath;
        static public void Init(string etlFilePath, string etlFileName) 
        {
            _etlFilePath = etlFilePath;
            _etlFileName = etlFileName;

            Process();

            using (var watcher = new FileSystemWatcher(_etlFilePath))
            {
                watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

                watcher.Changed += OnChanged;
                watcher.Created += OnCreated;
                watcher.Filter = "*"+_fileExt;
                watcher.IncludeSubdirectories = true;
                watcher.EnableRaisingEvents = true;
            }
        }
        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Console.WriteLine($"ETL file located at : {e.FullPath} has changed.");
            Process();
        }
        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);
            Process();
        }

        static public void Process()
        {
            if (string.IsNullOrEmpty(_etlFileName))
            {
                Console.Error.WriteLine("ETL file name not provided...");
                return;
            }

            using (ITraceProcessor trace = TraceProcessor.Create(_etlFilePath+""+_etlFileName+""+_fileExt))
            {
                IPendingResult<IProcessDataSource> pendingProcessData = trace.UseProcesses();

                trace.Process();

                IProcessDataSource processData = pendingProcessData.Result;

                foreach (IProcess process in processData.Processes)
                {
                    log.Info(process.CommandLine);
                }
            }
        }
    }
}
