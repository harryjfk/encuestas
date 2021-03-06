﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Domain.Managers;
using Entity;
using OfficeOpenXml;

namespace WebApplication.Controllers
{
    public class EstablecimientoController : BaseController<Establecimiento>
    {
        ServiceSunat.WCFSistemasService miserviciosunat;

        public static Establecimiento Establecimiento { get; set; }
        public static Query<Ciiu> QueryCiiuAsignados { get; set; }
        public static Query<Ciiu> QueryCiiuNoAsignados { get; set; }

        public ActionResult GetDorpDown(string id, string nombre = "IdEstablecimiento", string @default = null)
        {
            var list = OwnManager.Get(t => t.Activado).Select(t => new SelectListItem()
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
        public ActionResult GetOrden(string id, long idEstablecimiento, string nombre = "orden", string @default = null)
        {
            var establecimiento = Manager.Establecimiento.Find(idEstablecimiento);
            var included = establecimiento.CAT_ESTAB_ANALISTA.Select(t => t.orden);
            var count = establecimiento.Ciius.Count;
            var list = new List<SelectListItem>();
            for (int i = 1; i <= count; i++)
            {
                if (!included.Contains(i))
                {
                    list.Add(new SelectListItem()

                    {
                        Text = i.ToString(),
                        Value = i.ToString(),
                        Selected = i.ToString() == id
                    });
                }
            }

            if (@default != null)
                list.Insert(0, new SelectListItem()
                {
                    Selected = id == "0",
                    Value = "0",
                    Text = @default
                });
            return View("_DropDown", Tuple.Create<IEnumerable<SelectListItem>, string>(list, nombre));
        }

        public ActionResult GetCiiuAsignarAnalista(string id, long idEstablecimiento, string nombre = "IdCiiu", string @default = null)
        {
            var est = Manager.Establecimiento.Find(idEstablecimiento);
            var included = est.CAT_ESTAB_ANALISTA.Select(t => t.id_ciiu);
            var list = est.Ciius.Where(t => !included.Contains(t.Id)).Select(t => new SelectListItem()
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

        public JsonResult Toggle(long id)
        {
            var manager = OwnManager;
            var element = manager.Find(id);
            if (element != null)
            {
                element.Activado = !element.Activado;
                manager.Modify(element);
                manager.SaveChanges();
            }
            OwnManager.Get(Query);
            var c = RenderRazorViewToString("_Table", Query);
            var result = new
            {
                Success = true,
                Data = c
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public override ActionResult Edit(long id)
        {
            var element = OwnManager.Find(id);
            if (element == null)
            {
                element = new Establecimiento();
                Manager.Establecimiento.FillIdentificador(element);
            }
            return PartialView("_Create", element);
        }

        public override JsonResult CreatePost(Establecimiento element, params string[] properties)
        {
            var old = Manager.Establecimiento.Find(element.Id);

            if (old == null) return base.CreatePost(element);
            //element.IdAnalista = old.IdAnalista;
            element.IdInformante = old.IdInformante;
            return base.CreatePost(element);
        }

        public ActionResult GetCiiu(long id)
        {
            var establecimiento = Manager.Establecimiento.Find(id);
            if (establecimiento == null) return HttpNotFound("Establecimiento no encontrado");
            Establecimiento = establecimiento;
            QueryCiiuAsignados = QueryCiiuAsignados ?? new Query<Ciiu>();
            QueryCiiuAsignados = QueryCiiuAsignados.Validate();
            QueryCiiuAsignados.Criteria = QueryCiiuAsignados.Criteria ?? new Ciiu();
            ViewBag.Establecimiento = establecimiento;
            QueryCiiuAsignados.BuildFilter();
            QueryCiiuAsignados.Paginacion.ItemsPerPage = 10;
            Manager.Establecimiento.GetAllCiiu(QueryCiiuAsignados, id);
            return View("Ciiu", QueryCiiuAsignados);
        }

        public ActionResult ToggleCiiu(long id)
        {
            if (Establecimiento == null)
                return HttpNotFound("Establecimiento no encontrado");
            Manager.Establecimiento.ToggleCiiu(Establecimiento.Id, id);
            Manager.Establecimiento.GetAllCiiu(QueryCiiuAsignados, Establecimiento.Id);
            return PartialView("_TableCiiu", QueryCiiuAsignados);
        }

        [HttpPost]
        public virtual ActionResult BuscarCiiu(Ciiu criteria)
        {
            if (Establecimiento == null) return HttpNotFound("Establecimiento no encontrado");
            QueryCiiuAsignados = QueryCiiuAsignados ?? new Query<Ciiu>().Validate();
            QueryCiiuAsignados.Criteria = criteria;
            QueryCiiuAsignados.Paginacion = Query.Paginacion ?? new Paginacion();
            QueryCiiuAsignados.Paginacion.Page = 1;
            QueryCiiuAsignados.BuildFilter();
            return RedirectToAction("GetCiiu", new { id = Establecimiento.Id });
        }

        public virtual ActionResult PageCiiu(int page)
        {
            if (Establecimiento == null) return HttpNotFound("Establecimiento no encontrado");
            QueryCiiuAsignados = QueryCiiuAsignados ?? new Query<Ciiu>().Validate();
            QueryCiiuAsignados.Paginacion.Page = page;
            return RedirectToAction("GetCiiu", new { id = Establecimiento.Id });
        }

        public ActionResult GetCiiuNoAsignados()
        {
            if (Establecimiento == null) return HttpNotFound("Establecimiento no encontrado");
            QueryCiiuNoAsignados = QueryCiiuNoAsignados ?? new Query<Ciiu>();
            QueryCiiuNoAsignados = QueryCiiuNoAsignados.Validate();
            QueryCiiuNoAsignados.Criteria = QueryCiiuNoAsignados.Criteria ?? new Ciiu();
            ViewBag.Establecimiento = Establecimiento;
            QueryCiiuNoAsignados.BuildFilter();
            QueryCiiuNoAsignados.Paginacion.ItemsPerPage = 10;
            Manager.Establecimiento.GetCiiuNoAsignados(QueryCiiuNoAsignados, Establecimiento.Id);
            return View("CiiuNoAsignados", QueryCiiuAsignados);
        }
        
        public JsonResult GetSunat(string id)
        {
            if (miserviciosunat == null)
            {
                miserviciosunat = new ServiceSunat.WCFSistemasService();
            }
            ServiceSunat.BuscarEmpresaResult entidad = new ServiceSunat.BuscarEmpresaResult();
            entidad = miserviciosunat.BuscarSunatProduccionParametro(id);

            return Json(entidad.ddp_nombreField, JsonRequestBehavior.AllowGet);
            
        }
    }
}
