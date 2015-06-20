using System;
using System.Diagnostics;
using AwesomeLogger.NotificationService.Startup;
using Microsoft.Practices.Unity;

namespace AwesomeLogger.NotificationService
{
    internal class Program
    {
        private const string ServicePrintName = "AwesomeLogger Notification Service";

        public static void Start()
        {
            try
            {
                var container = IoCConfig.Configure();
                InitializersConfig.Configure(container);

                Trace.TraceInformation("{0} started.", ServicePrintName);

                // Process messages
                var notificationManager = container.Resolve<INotificationManager>();
                notificationManager.Start();
            }
            catch (Exception e)
            {
                Trace.TraceError("Could not start {0}: {1}", ServicePrintName, e);
            }
        }

        public static void Stop()
        {
            Trace.TraceInformation("{0} stopped.", ServicePrintName);
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