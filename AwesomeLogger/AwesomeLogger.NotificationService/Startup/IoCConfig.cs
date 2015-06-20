using System;
using AwesomeLogger.NotificationService.Configuration;
using AwesomeLogger.NotificationService.Initializers;
using AwesomeLogger.NotificationService.Services;
using Microsoft.Practices.Unity;

namespace AwesomeLogger.NotificationService.Startup
{
    public static class IoCConfig
    {
        private static readonly Lazy<IUnityContainer> Container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        public static IUnityContainer Configure()
        {
            return Container.Value;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            // Common
            container.RegisterType<IConfigurationProvider, ConfigurationProvider>(
                new ContainerControlledLifetimeManager());

            container.RegisterType<INotificationManager, NotificationManager>();
            container.RegisterType<IEmailService, SendgridEmailService>();

            // Initializers
            container.RegisterType<IDiagnosticsInitializer, DiagnosticsInitializer>(
                new ContainerControlledLifetimeManager());
            container.RegisterType<IServiceBusInitializer, ServiceBusInitializer>(
                new ContainerControlledLifetimeManager());
        }
    }
}