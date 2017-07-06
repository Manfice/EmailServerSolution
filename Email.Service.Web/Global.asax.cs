using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Email.Agent;

namespace Email.Service.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            StartEmailAgent();
        }

        protected void Application_End()
        {
            StopEmailAgent();
        }

        protected void Application_BeginRequest()
        {
        }

        private static void StartEmailAgent()
        {
            EmailAgentManager.StartAgent("localhost", 1);
        }

        private static void StopEmailAgent()
        {
            EmailAgentManager.StopAgent();
        }
    }
}
