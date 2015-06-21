using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;
using AwesomeLogger.NotificationService.Startup;
using Microsoft.Practices.Unity;
using Microsoft.ServiceBus.Messaging;

namespace AwesomeLogger.NotificationService
{
    internal class Program
    {
        private const string ServicePrintName = "AwesomeLogger Notification Service";
        private static INotificationManager _notificationManager;

        public static void Start()
        {
            try
            {
                var container = IoCConfig.Configure();
                InitializersConfig.Configure(container);

                Trace.TraceInformation("{0} started.", ServicePrintName);

                // Process messages
                _notificationManager = container.Resolve<INotificationManager>();
                _notificationManager.Start();
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
            _notificationManager.Dispose();
        }

        private static void Main()
        {
            if (!Environment.UserInteractive)
            {
                // Windows service
                using (var service = new NotificationService())
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