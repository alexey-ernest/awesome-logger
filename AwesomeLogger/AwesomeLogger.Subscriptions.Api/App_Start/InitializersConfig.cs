using System;
using System.Collections.Generic;
using System.Diagnostics;
using AwesomeLogger.Subscriptions.Api.Initializers;
using Microsoft.Practices.Unity;

namespace AwesomeLogger.Subscriptions.Api
{
    public static class InitializersConfig
    {
        public static void Configure(IUnityContainer container)
        {
            try
            {
                var initializers = new List<IInitializable>
                {
                    container.Resolve<IServiceBusInitializer>()
                };

                foreach (var initializer in initializers)
                {
                    initializer.Initialize();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Failed to initialize: {0}", e);
                throw;
            }
        }
    }
}