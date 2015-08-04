using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Managers;

namespace WebApplication.Controllers
{
    public class DepartamentoController : Controller
    {
        public Manager Manager {
            get { return Tools.GetManager(); }
        }

        public ActionResult GetDropDown(string id="",string nombre="IdDepartamento",string @default=null)
        {
            var list = Manager.Departamento.Get().OrderBy(t => t.Nombre).Select(t => new SelectListItem()
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
             
        public ActionResult GetNombreDepartamento(string idDepartamento)
        {
            var departamento = Manager.Departamento.Find(idDepartamento);
            var name = "...";
            if (departamento != null)
                name = departamento.Nombre;
            return PartialView("_Text", name);
        }
    }
}