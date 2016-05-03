using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Domain.Managers;
using WebApplication.Binders;
using Seguridad.PRODUCE;

namespace WebApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //Tools.GetManager().Seed();
            ModelBinders.Binders.Add(typeof(decimal?), new ModelBinder.DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal), new ModelBinder.DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(double?), new ModelBinder.DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(double), new ModelBinder.DecimalModelBinder());

            //brb
            //ConfigurationSecurity.Start();
            //endbrb
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            //brb
            //ConfigurationSecurity.EndRequest(this);
            //endbrb
        }

    }
}
