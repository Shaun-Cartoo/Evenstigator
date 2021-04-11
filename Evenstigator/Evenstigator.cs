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
        public Evenstigator()
        {
            InitializeComponent();
            _eventLog = new EventLog();

            //if (args.Length > 0)
            //    _src = args[0];
            //if (args.Length > 1)
            //    _log = args[1];

            if(!EventLog.SourceExists(_src))
            {
                EventLog.CreateEventSource(_src, _log);
            }

            _eventLog.Source = _src;
            _eventLog.Log = _log;
        }

        protected override void OnStart(string[] args)
        {
            _eventLog.WriteEntry("Evenstigator service has started...");
        }
        protected override void OnPause()
        {
            base.OnPause();
            _eventLog.WriteEntry("Evenstigator service has been paused...");
        }
        protected override void OnStop()
        {
            _eventLog.WriteEntry("Evenstigator service has stopped...");
        }
    }
}
