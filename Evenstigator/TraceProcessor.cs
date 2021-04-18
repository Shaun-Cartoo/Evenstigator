using Microsoft.Windows.EventTracing;
using Microsoft.Windows.EventTracing.Processes;
using log4net;
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Windows.EventTracing.Syscalls;

namespace Evenstigator
{
    static class MyTraceProcessor
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static FileSystemWatcher _watcher;
        private static readonly string _fileExt = ".etl";
        private static string _etlFileName;
        private static string _etlFilePath;
        static public void Init(string etlFilePath, string etlFileName) 
        {
            _etlFilePath = etlFilePath;
            //_etlFileName = etlFileName;
            _watcher = new FileSystemWatcher(_etlFilePath);
            _watcher.NotifyFilter = NotifyFilters.Attributes
                                | NotifyFilters.CreationTime
                                | NotifyFilters.DirectoryName
                                | NotifyFilters.FileName
                                | NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.Security
                                | NotifyFilters.Size;

            _watcher.Changed += OnChanged;
            _watcher.Created += OnCreated;
            _watcher.Filter = "*" + _fileExt;
            _watcher.IncludeSubdirectories = true;
            _watcher.EnableRaisingEvents = true;
        }
        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            //Console.WriteLine($"ETL file located at : {e.FullPath} has changed.");
            Process(e.FullPath);
        }
        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            //string value = $"Created: {e.FullPath}";
            //Console.WriteLine($"ETL file located at : {e.FullPath} has been created.");
            Process(e.FullPath);
        }

        private static void Process(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    Console.Error.WriteLine("ETL file name not provided...");
                    return;
                }

                using (ITraceProcessor trace = TraceProcessor.Create(path))
                {
                    IPendingResult<IProcessDataSource> pendingProcessData = trace.UseProcesses();

                    trace.Process();

                    IProcessDataSource processData = pendingProcessData.Result;

                    foreach (IProcess process in processData.Processes)
                    {
                        Console.WriteLine(process.CommandLine);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occured while processing. {ex}");
            }
        }
    }
}
