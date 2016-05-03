using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class SeguridadController : Controller
    {
        //
        // GET: /Seguridad/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AccesoDenegado()
        {
            return View();
        }
	}
}