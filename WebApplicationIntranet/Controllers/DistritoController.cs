using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Managers;
using Seguridad.PRODUCE;

namespace WebApplication.Controllers
{
    /*[Authorize]
    [Autorizacion]*/
    public class DistritoController : Controller
    {
        public Manager Manager {
            get { return Tools.GetManager(); }
        }
        public ActionResult GetDropDown(string id="",string nombre="IdDistrito",string @default=null,string idProvincia=null)
        {
            var list = (idProvincia == null ? Manager.Distrito.Get() : Manager.Provincia.GetDistritos(idProvincia)).OrderBy(t => t.Nombre).Select(t => new SelectListItem()
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
        public ActionResult GetNombreDistrito(string idDistrito)
        {
            var distrito = Manager.Distrito.Find(idDistrito);
            var name = "...";
            if (distrito != null)
                name = distrito.Nombre;
            return PartialView("_Text", name);
        }
    }
}