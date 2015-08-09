using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Domain.Managers;
using Entity;

namespace WebApplication.Controllers
{
    public class LineaProductoEstablecimientoController : BaseController<LineaProductoEstablecimiento>
    {
        private static long IdEstablecimiento { get; set; }

        public ActionResult GetDorpDownLineaProducto(string id, long idCiiu = 0, string nombre = "IdLineaProducto", string @default = null)
        {
            var list = Manager.LineaProducto.Get(t => t.Activado
                && t.LineasProductoEstablecimiento.All(h => h.IdEstablecimiento != IdEstablecimiento)
                /*&& t.Ciiu.Establecimientos.Any(h=>h.Id==IdEstablecimiento*/
                && t.IdCiiu == idCiiu)
                .Select(t => new SelectListItem()
            {
                Text = t.ToString(),
                Value = t.Id.ToString(),
                Selected = t.Id.ToString() == id
            }).ToList();
            if (@default != null)
                list.Insert(0, new SelectListItem()
                {
                    Selected = id == "0",
                    Value = "0",
                    Text = @default
                });
            return View("_DropDown", Tuple.Create<IEnumerable<SelectListItem>, string>(list, nombre));
        }
        public ActionResult GetDorpDownCiiu(string id="0", string nombre = "IdCiiu", string @default = null)
        {
            var list = Manager.Ciiu.Get(t => t.Activado
                && t.Establecimientos.Any(h => h.Id == IdEstablecimiento))
                .Select(t => new SelectListItem()
                {
                    Text = t.ToString(),
                    Value = t.Id.ToString(),
                    Selected = t.Id.ToString() == id
                }).ToList();
            if (@default != null)
                list.Insert(0, new SelectListItem()
                {
                    Selected = id == "0",
                    Value = "0",
                    Text = @default
                });
            return View("_DropDown", Tuple.Create<IEnumerable<SelectListItem>, string>(list, nombre));
        }
        public override ActionResult Index()
        {
            Query = Query ?? new Query<LineaProductoEstablecimiento>().Validate();
            Query.Criteria = Query.Criteria ?? new LineaProductoEstablecimiento();
            Query.Criteria.IdEstablecimiento = IdEstablecimiento;
            Query.BuildFilter();
            return base.Index();
        }
        public ActionResult Get(long id)
        {
            var establecimiento = Manager.Establecimiento.Find(id);
            if (establecimiento == null) return HttpNotFound("El establecimiento no existe");
            IdEstablecimiento = id;
            Query = Query ?? new Query<LineaProductoEstablecimiento>().Validate();
            Query.Criteria = Query.Criteria ?? new LineaProductoEstablecimiento();
            Query.Criteria.Establecimiento = establecimiento;
            return RedirectToAction("Index");
        }
        public override JsonResult CreatePost(LineaProductoEstablecimiento element, params string[] properties)
        {
            element.IdEstablecimiento = IdEstablecimiento;
            return base.CreatePost(element);
        }
        public override ActionResult Buscar(LineaProductoEstablecimiento criteria)
        {
            Query = Query ?? new Query<LineaProductoEstablecimiento>().Validate();
            Query.Criteria = Query.Criteria ?? new LineaProductoEstablecimiento();
            criteria.Establecimiento = Query.Criteria.Establecimiento;
            return base.Buscar(criteria);
        }
    }
}
