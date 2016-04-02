using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Domain.Managers;
using Entity;
using OfficeOpenXml;
using Seguridad.PRODUCE;

namespace WebApplication.Controllers
{
    /*[Authorize]
    [Autorizacion]*/
    public class EstablecimientoController : BaseController<Establecimiento>
    {
        ServiceSunat.WCFSistemasService miserviciosunat;
        
        public Query<Ciiu> QueryCiiuAsignados { get; set; }       

        private long IdEstablecimiento
        {
            get
            {
                if (Session["IdEstablecimiento"] != null)
                {
                    return (long)Session["IdEstablecimiento"];
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                Session["IdEstablecimiento"] = value;
            }
        }

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
            var list = est.Ciius.Select(t => t.Ciiu).Where(t => !included.Contains(t.Id)).Select(t => new SelectListItem()
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
            Query = GetQuery();

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

            if (old != null)
            {
                element.IdInformante = old.IdInformante;
            }

            return base.CreatePost(element);
        }
        
        #region Ciiu
        public ActionResult GetCiiu(long id)
        {
            var establecimiento = Manager.Establecimiento.Find(id);
            if (establecimiento == null) return HttpNotFound("Establecimiento no encontrado");
            ViewBag.Establecimiento = establecimiento;
            IdEstablecimiento = id;

            QueryCiiuAsignados = GetQueryCiiu();
            Manager.Establecimiento.GetAllCiiu(QueryCiiuAsignados, id);
            return View("Ciiu", QueryCiiuAsignados);
        }

        public ActionResult ToggleCiiu(long id)
        {
            QueryCiiuAsignados = GetQueryCiiu();
            Manager.Establecimiento.ToggleCiiu(IdEstablecimiento, id);
            Manager.Establecimiento.GetAllCiiu(QueryCiiuAsignados, IdEstablecimiento);
            return PartialView("_TableCiiu", QueryCiiuAsignados);
        }

        [HttpPost]
        public virtual ActionResult BuscarCiiu(Ciiu criteria)
        {
            Session["CriteriaCiiuAsignados"] = criteria;
            Session["PageCiiuAsignados"] = 1;
            ModelState.Clear();
            return RedirectToAction("GetCiiu", new { id = IdEstablecimiento });
        }

        public virtual ActionResult PageCiiu(int page)
        {           
            Session["PageCiiuAsignados"] = page;
            ModelState.Clear();
            return RedirectToAction("GetCiiu", new { id = IdEstablecimiento });
        }

        public Query<Ciiu> GetQueryCiiu()
        {
            QueryCiiuAsignados = QueryCiiuAsignados ?? new Query<Ciiu>();
            QueryCiiuAsignados = QueryCiiuAsignados.Validate();
            QueryCiiuAsignados.Criteria = new Ciiu();
            QueryCiiuAsignados.Paginacion = QueryCiiuAsignados.Paginacion ?? new Paginacion();
            QueryCiiuAsignados.Paginacion.ItemsPerPage = 10;

            if (Session["CriteriaCiiuAsignados"] != null)
            {
                if (Session["CriteriaCiiuAsignados"] is Ciiu)
                {
                    QueryCiiuAsignados.Criteria = (Ciiu)Session["CriteriaCiiuAsignados"];
                }
                else
                {
                    Session["CriteriaCiiuAsignados"] = null;
                    Session["PageCiiuAsignados"] = null;
                    Session["OrdenCiiuAsignados"] = null;
                }
            }

            if (Session["PageCiiuAsignados"] != null)
            {
                QueryCiiuAsignados.Paginacion.Page = (int)Session["PageCiiuAsignados"];
            }

            if (Session["OrdenCiiuAsignados"] != null)
            {
                QueryCiiuAsignados.Order = (Order<Ciiu>)Session["OrderCiiuAsignados"];
            }

            QueryCiiuAsignados.BuildFilter();

            return QueryCiiuAsignados;
        }

        public ActionResult GetCiiuNoAsignados()
        {
            ViewBag.Establecimiento = Manager.Establecimiento.Find(IdEstablecimiento);

            Query<Ciiu> QueryCiiuNoAsignados = new Query<Ciiu>();
            QueryCiiuNoAsignados = QueryCiiuNoAsignados.Validate();
            QueryCiiuNoAsignados.Criteria = QueryCiiuNoAsignados.Criteria ?? new Ciiu();            
            QueryCiiuNoAsignados.BuildFilter();
            QueryCiiuNoAsignados.Paginacion.ItemsPerPage = 10;
            Manager.Establecimiento.GetCiiuNoAsignados(QueryCiiuNoAsignados, IdEstablecimiento);
            return View("CiiuNoAsignados", QueryCiiuAsignados);
        }
        #endregion

        public JsonResult GetSunat(string id)
        {
            if (miserviciosunat == null)
            {
                miserviciosunat = new ServiceSunat.WCFSistemasService();
            }
            ServiceSunat.BuscarEmpresaResult entidad = new ServiceSunat.BuscarEmpresaResult();
            entidad = miserviciosunat.Validacion_SUNAT(id);

            return Json(entidad.ddp_nombreField, JsonRequestBehavior.AllowGet);            
        }
    }
}
