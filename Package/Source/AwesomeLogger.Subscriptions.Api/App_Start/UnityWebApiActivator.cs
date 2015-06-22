using System.Web.Http;
using AwesomeLogger.Subscriptions.Api;
using Microsoft.Practices.Unity.WebApi;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof (UnityWebApiActivator), "Start")]
[assembly: ApplicationShutdownMethod(typeof (UnityWebApiActivator), "Shutdown")]

namespace AwesomeLogger.Subscriptions.Api
{
    /// <summary>Provides the bootstrapping for integrating Unity with WebApi when it is hosted in ASP.NET</summary>
    public static class UnityWebApiActivator
    {
        /// <summary>Integrates Unity when the application starts.</summary>
        public static void Start()
        {
            // Use UnityHierarchicalDependencyResolver if you want to use a new child container for each IHttpController resolution.
            // var resolver = new UnityHierarchicalDependencyResolver(UnityConfig.GetConfiguredContainer());
            var resolver = new UnityDependencyResolver(IoCConfig.GetConfiguredContainer());

            GlobalConfiguration.Configuration.DependencyResolver = resolver;
        }

        /// <summary>Disposes the Unity container when the application is shut down.</summary>
        public static void Shutdown()
        {
            var container = IoCConfig.GetConfiguredContainer();
            container.Dispose();
        }
    }
}