using System;
using AwesomeLogger.Subscriptions.Api.DAL;
using AwesomeLogger.Subscriptions.Api.Events;
using AwesomeLogger.Subscriptions.Api.Infrastructure.Configuration;
using AwesomeLogger.Subscriptions.Api.Initializers;
using AwesomeLogger.Subscriptions.Api.Interceptions;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace AwesomeLogger.Subscriptions.Api
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class IoCConfig
    {
        #region Unity Container

        private static readonly Lazy<IUnityContainer> Container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return Container.Value;
        }

        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            container.AddNewExtension<Interception>();

            // Common
            container.RegisterType<IConfigurationProvider, ConfigurationProvider>(
                new ContainerControlledLifetimeManager());

            // DAL
            container.RegisterType<SubscriptionDbContext, SubscriptionDbContext>(new PerRequestLifetimeManager());
            container.RegisterType<ISubscriptionRepository, SubscriptionRepository>(new PerRequestLifetimeManager(),
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<SubscriptionNotificationsInterception>()
                );

            container.RegisterType<IEventEmitter>(
                new InjectionFactory(
                    c =>
                        new ServiceBusEventEmitter(
                            container.Resolve<IConfigurationProvider>().Get(SettingNames.ServiceBusConnectionString),
                            container.Resolve<IConfigurationProvider>().Get(SettingNames.ServiceBusSubscriptionTopic))));

            // Initializers
            container.RegisterType<IServiceBusInitializer, ServiceBusInitializer>(
                new ContainerControlledLifetimeManager());
        }
    }
}
