using Seguridad.PRODUCE;
using System.Web;
using System.Web.Mvc;

namespace WebApplication
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            // seguridad.PRODUCE
            filters.Add(new AutorizacionRol());
        }
    }
}
