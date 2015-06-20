using System;
using AwesomeLogger.Monitor.Configuration;
using AwesomeLogger.Monitor.Events;
using AwesomeLogger.Monitor.Initializers;
using Microsoft.Practices.Unity;

namespace AwesomeLogger.Monitor.Startup
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

            container.RegisterType<IMonitorManager, MonitorManager>(
                new ContainerControlledLifetimeManager());
            container.RegisterType<IErrorEventEmitter>(
                new InjectionFactory(
                    c =>
                        new ErrorEventEmitter(
                            container.Resolve<IConfigurationProvider>().Get(SettingNames.ServiceBusConnectionString),
                            container.Resolve<IConfigurationProvider>().Get(SettingNames.ServiceBusErrorTopic))));
            container.RegisterType<IMatchEventEmitter>(
                new InjectionFactory(
                    c =>
                        new ErrorEventEmitter(
                            container.Resolve<IConfigurationProvider>().Get(SettingNames.ServiceBusConnectionString),
                            container.Resolve<IConfigurationProvider>().Get(SettingNames.ServiceBusSubscriptionTopic))));

            // Initializers
            container.RegisterType<IDiagnosticsInitializer, DiagnosticsInitializer>(
                new ContainerControlledLifetimeManager());
            container.RegisterType<IServiceBusInitializer, ServiceBusInitializer>(
                new ContainerControlledLifetimeManager());
        }
    }
}