using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Managers;

namespace WebApplication.Controllers
{
    public class ProvinciaController : Controller
    {
        public Manager Manager {
            get { return Tools.GetManager(); }
        }
        public ActionResult GetDropDown(string id="",string nombre="IdProvincia",string @default=null,string idDepartamento=null)
        {
            var list = (idDepartamento == null ? Manager.Provincia.Get() : Manager.Departamento.GetProvincias(idDepartamento)).OrderBy(t => t.Nombre).Select(t => new SelectListItem()
            {
                Text = t.Nombre,
                Value = t.Codigo.ToString(),
                Selected = t.Codigo.ToString() == id
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

        
        public ActionResult GetNombreProvincia(string idProvincia)
        {
            var provincia = Manager.Provincia.Find(idProvincia);
            var name = "...";
            if (provincia != null)
                name = provincia.Nombre;
            return PartialView("_Text", name);
        }
    }
}