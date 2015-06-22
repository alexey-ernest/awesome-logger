using System;
using AwesomeLogger.ErrorHandlingService.Configuration;
using AwesomeLogger.ErrorHandlingService.Initializers;
using AwesomeLogger.NotificationService.Configuration;
using Microsoft.Practices.Unity;

namespace AwesomeLogger.ErrorHandlingService.Startup
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

            container.RegisterType<IErrorHandlingManager, ErrorHandlingManager>();

            // Initializers
            container.RegisterType<IDiagnosticsInitializer, DiagnosticsInitializer>(
                new ContainerControlledLifetimeManager());
            container.RegisterType<IServiceBusInitializer, ServiceBusInitializer>(
                new ContainerControlledLifetimeManager());
        }
    }
}