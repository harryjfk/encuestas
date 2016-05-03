using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Domain.Managers;
using Entity;
using Seguridad.PRODUCE;

namespace WebApplication.Controllers
{
    /*[Authorize]
    [Autorizacion]*/
    public class LineaProductoEstablecimientoController : BaseController<LineaProductoEstablecimiento>
    {
        public ActionResult GetDorpDownLineaProducto(string id, long idCiiu = 0, string nombre = "IdLineaProducto", string @default = null)
        {
            long IdEstablecimiento = ((LineaProductoEstablecimiento)Session[CriteriaSesion]).IdEstablecimiento;
            var notInclude = Manager.LineaProductoEstablecimiento.Get(h => h.IdEstablecimiento == IdEstablecimiento);
            var list = Manager.LineaProducto.Get(t => t.Activado
                && !notInclude.Any(p2 => p2.IdLineaProducto == t.Id)
                && t.Codigo.Length == 7
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
            long IdEstablecimiento = ((LineaProductoEstablecimiento)Session[CriteriaSesion]).IdEstablecimiento;
            var list = Manager.Ciiu.Get(t => t.Activado
                && t.Establecimientos.Any(h => h.IdEstablecimiento == IdEstablecimiento))
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
       
        public ActionResult Get(long id)
        {
            bool FirstLoad = false;

            if (Session[CriteriaSesion] != null)
            {
                if (Session[CriteriaSesion] is LineaProductoEstablecimiento == false)
                {
                    FirstLoad = true;
                }
            }
            else
            {
                FirstLoad = true;
            }

            if (FirstLoad)
            {
                LineaProductoEstablecimiento criteria = new LineaProductoEstablecimiento();
                criteria.IdEstablecimiento = id;
                Session[CriteriaSesion] = criteria;
            }

            return RedirectToAction("Index");
        }

        public override JsonResult CreatePost(LineaProductoEstablecimiento element, params string[] properties)
        {   
            element.IdEstablecimiento = ((LineaProductoEstablecimiento)Session[CriteriaSesion]).IdEstablecimiento;            
            return base.CreatePost(element);
        }

        public override ActionResult Buscar(LineaProductoEstablecimiento criteria)
        {
            criteria.IdEstablecimiento = ((LineaProductoEstablecimiento)Session[CriteriaSesion]).IdEstablecimiento;
            return base.Buscar(criteria);
        }
    }
}
