using System;
using System.Diagnostics;
using System.ServiceProcess;
using AwesomeLogger.Monitor.Startup;
using Microsoft.Practices.Unity;

namespace AwesomeLogger.Monitor
{
    internal static class Program
    {
        private const string ServicePrintName = "AwesomeLogger Monitor";
        private static IMonitorManager _monitorManager;

        public static void Start()
        {
            try
            {
                var container = IoCConfig.Configure();
                InitializersConfig.Configure(container);

                Trace.TraceInformation("{0} started.", ServicePrintName);

                // Start
                _monitorManager = container.Resolve<IMonitorManager>();
                _monitorManager.Start();
            }
            catch (Exception e)
            {
                Trace.TraceError("Could not start {0}: {1}", ServicePrintName, e);
            }
        }

        public static void Stop()
        {
            Trace.TraceInformation("{0} stopped.", ServicePrintName);
            _monitorManager.Dispose();
        }

        private static void Main()
        {
            if (!Environment.UserInteractive)
            {
                // Windows service
                using (var service = new MonitorService())
                {
                    ServiceBase.Run(service);
                }
            }
            else
            {
                // Console
                Start();

                Console.WriteLine("Press any key to stop...");
                Console.ReadKey(true);

                Stop();
            }
        }
    }
}