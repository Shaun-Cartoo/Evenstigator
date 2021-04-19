using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Evenstigator
{
    static class Program
    {
        static void Main(string[] args)
        {
            Evenstigator service = new Evenstigator();
            if (Environment.UserInteractive)
            {
                service.RunAsConsole(args);
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new Evenstigator()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
