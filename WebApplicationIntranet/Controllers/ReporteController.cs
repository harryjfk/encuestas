using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entity;
using Entity.Reportes;

namespace WebApplication.Controllers
{
    public class ReporteController : BaseController<PorcentajeEncuestaEstadistica>
    {
        public ActionResult PorcentajeEncuestaEstadistica()
        {
            var filter = new PorcentajeEncuestaEstadisticaFilter()
            {
                IsAnnual = true,
                Year = DateTime.Now.Year,
                Estado = EstadoEncuesta.Todos,
            };
            var report = new PorcentajeEncuestaEstadistica()
            {
                Filter = filter
            };
            return View("PorcentajeEncuestaEstadistica/Index", report);
        }
        //
        // GET: /PorcentajeEncuestaEstadistica/
        [HttpPost]
        public ActionResult BuscarPorcentajeEncuestaEstadistica(PorcentajeEncuestaEstadisticaFilter filter)
        {
            filter = filter ?? new PorcentajeEncuestaEstadisticaFilter()
            {
                IsAnnual = true,
                Year = DateTime.Now.Year,
                Estado = EstadoEncuesta.Todos,
            };
            var report = Manager.EncuestaEstadistica.PorcentajeEncuestaEstadistica(filter);
            return View("PorcentajeEncuestaEstadistica/Index",report);
        }
	}
}