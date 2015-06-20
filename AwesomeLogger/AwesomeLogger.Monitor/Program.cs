using System;
using System.Diagnostics;
using AwesomeLogger.Monitor.Startup;
using Microsoft.Practices.Unity;

namespace AwesomeLogger.Monitor
{
    internal static class Program
    {
        private const string MonitorPrintName = "AwesomeLogger Monitor";

        public static void Start()
        {
            try
            {
                var container = IoCConfig.Configure();
                InitializersConfig.Configure(container);

                Trace.TraceInformation("{0} started.", MonitorPrintName);

                // Start
                var monitor = container.Resolve<IMonitorManager>();
                monitor.Start();
            }
            catch (Exception e)
            {
                Trace.TraceError("Could not start {0}: {1}", MonitorPrintName, e);
            }
        }

        public static void Stop()
        {
            Trace.TraceInformation("{0} stopped.", MonitorPrintName);
        }

        private static void Main()
        {
            Start();

            Console.WriteLine("Press any key to stop...");
            Console.ReadKey(true);

            Stop();
        }
    }
}