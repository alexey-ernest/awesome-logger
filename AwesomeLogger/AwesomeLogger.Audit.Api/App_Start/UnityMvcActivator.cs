using System.Linq;
using System.Web.Mvc;
using AwesomeLogger.Audit.Api;
using Microsoft.Practices.Unity.Mvc;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof (UnityWebActivator), "Start")]
[assembly: ApplicationShutdownMethod(typeof (UnityWebActivator), "Shutdown")]

namespace AwesomeLogger.Audit.Api
{
    /// <summary>Provides the bootstrapping for integrating Unity with ASP.NET MVC.</summary>
    public static class UnityWebActivator
    {
        /// <summary>Integrates Unity when the application starts.</summary>
        public static void Start()
        {
            var container = IoCConfig.GetConfiguredContainer();

            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(container));

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            DynamicModuleUtility.RegisterModule(typeof (UnityPerRequestHttpModule));
        }

        /// <summary>Disposes the Unity container when the application is shut down.</summary>
        public static void Shutdown()
        {
            var container = IoCConfig.GetConfiguredContainer();
            container.Dispose();
        }
    }
}