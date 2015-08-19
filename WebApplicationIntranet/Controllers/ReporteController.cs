using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entity.Reportes;

namespace WebApplication.Controllers
{
    public class ReporteController : BaseController<PorcentajeEncuestaEstadistica>
    {
        //
        // GET: /PorcentajeEncuestaEstadistica/
        public ActionResult PorcentajeEncuestaEstadistica()
        {
            return View("PorcentajeEncuestaEstadistica/Index");
        }
	}
}