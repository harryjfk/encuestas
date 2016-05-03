using Seguridad.PRODUCE;
using System.Web;
using System.Web.Mvc;

namespace WebApplication
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //brb
            //filters.Add(new AutorizacionRol());
            //endbrb

            filters.Add(new HandleErrorAttribute());
        }
    }
}
