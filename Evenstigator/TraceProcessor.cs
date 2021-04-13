using Microsoft.Windows.EventTracing;
using Microsoft.Windows.EventTracing.Processes;
using log4net;
using System;

namespace Evenstigator
{
    static class MyTraceProcessor
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static public void Init(string [] args) 
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: <trace.etl>");
                return;
            }

            using (ITraceProcessor trace = TraceProcessor.Create(args[0]))
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
