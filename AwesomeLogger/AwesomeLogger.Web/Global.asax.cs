using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AwesomeLogger.Web
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            IoCConfig.GetConfiguredContainer();
            JsonConfig.Configure(GlobalConfiguration.Configuration);

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            MvcHandler.DisableMvcResponseHeader = true;
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            if (context == null)
            {
                return;
            }

            context.Response.Headers.Remove("Server");
        }
    }
}