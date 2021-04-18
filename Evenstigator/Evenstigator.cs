using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Evenstigator
{
    public partial class Evenstigator : ServiceBase
    {
        private EventLog _eventLog;
        private string _src = ConfigurationManager.AppSettings["src"];
        private string _log = ConfigurationManager.AppSettings["log"];
        private string _etlName = ConfigurationManager.AppSettings["etlName"];
        private string _etlPath = ConfigurationManager.AppSettings["etlPath"];

        public Evenstigator()
        {
            InitializeComponent();
            _eventLog = new EventLog();

            if(!EventLog.SourceExists(_src))
            {
                EventLog.CreateEventSource(_src, _log);
            }

            _eventLog.Source = _src;
            _eventLog.Log = _log;
        }

        public void RunAsConsole(string[] args)
        {
            OnStart(args);
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
            OnStop();
        }
        protected override void OnStart(string[] args)
        {
            try
            {
                _eventLog.WriteEntry($"Evenstigator service has started...etl being read from location {_etlPath}");
                MyTraceProcessor.Init(_etlPath, _etlName);
                Dispose();
            }
            catch (Exception ex)
            {
                _eventLog.WriteEntry($"The following error occured: {ex}");
            }
        }
        protected override void OnPause()
        {
            base.OnPause();
            _eventLog.WriteEntry("Evenstigator service has been paused...");
        }
        protected override void OnStop()
        {
            _eventLog.WriteEntry("Evenstigator service has stopped...");
            Dispose();
        }
        public new void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
    }
}
