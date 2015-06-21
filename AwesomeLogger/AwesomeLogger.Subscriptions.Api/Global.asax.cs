using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace AwesomeLogger.Subscriptions.Api
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            var container = IoCConfig.GetConfiguredContainer();
            InitializersConfig.Configure(container);
            JsonConfig.Configure(GlobalConfiguration.Configuration);

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

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