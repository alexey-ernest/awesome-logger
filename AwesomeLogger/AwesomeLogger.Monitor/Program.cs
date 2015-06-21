using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;
using AwesomeLogger.Monitor.Startup;
using Microsoft.Practices.Unity;
using Microsoft.ServiceBus.Messaging;

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
            catch (MessagingCommunicationException e)
            {
                Trace.TraceWarning("Failed to connect to ServiceBus: {0}", e);

                // trying to connect again
                Task.Delay(60000).Wait();
                Start();
            }
            catch (UnauthorizedAccessException e)
            {
                Trace.TraceWarning("Failed to connect: {0}", e);

                // trying to connect again
                Task.Delay(60000).Wait();
                Start();
            }
            catch (Exception e)
            {
                Trace.TraceError("Could not start {0}: {1}", ServicePrintName, e);
                Stop();
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