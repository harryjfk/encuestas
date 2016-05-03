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
    public class UsuarioIntranetController : Controller
    {
        public UsuarioIntranet UsuarioActual
        {
            get
            {
                var user = this.GetLogued();
                return user != null ? Manager.Usuario.FindUsuarioIntranet((int)user.Identificador) : null;
            }
        }
        
        public Query<UsuarioIntranet> QueryAdministrador { get; set; }
        public Query<UsuarioIntranet> QueryAnalista { get; set; }        
        public Query<EstablecimientoAnalista> QueryEstablecimientosAsignados { get; set; }
        public Query<Establecimiento> QueryEstablecimientosNoAsignados { get; set; }
        public Query<Establecimiento> QueryEstablecimientosEncuestas { get; set; }
        
        public virtual Manager Manager
        {
            get
            {
                return Tools.GetManager(this.User.Identity.Name);
            }
        }

        public ActionResult GetDorpDown(string id, string nombre = "IdUsuarioIntranet", string @default = null)
        {
            var list = Manager.Usuario.GetUsuariosIntranet().Select(t => new SelectListItem()
            {
                Text = t.ToString(),
                Value = t.Identificador.ToString(),
                Selected = t.Identificador.ToString() == id
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

        #region ViewAdministrador
        public virtual ActionResult IndexAdministrador()
        {
            QueryAdministrador = GetQueryAdministrador();
            Manager.Usuario.GetUsuariosIntranet(QueryAdministrador);
            ModelState.Clear();
            return View("IndexAdministrador", QueryAdministrador);
        }

        [HttpPost]
        public virtual ActionResult BuscarAdministrador(UsuarioIntranet criteria)
        {
            Session["CriteriaAdministrador"] = criteria;
            Session["PageAdministrador"] = 1;
            ModelState.Clear();
            return RedirectToAction("IndexAdministrador");
        }

        public virtual ActionResult PageAdministrador(int page)
        {
            Session["PageAdministrador"] = page;
            ModelState.Clear();
            return RedirectToAction("IndexAdministrador");
        }

        public JsonResult SetAdministrador(int id)
        {
            QueryAdministrador = GetQueryAdministrador();
            var manager = Manager;
            manager.Usuario.MarcarAdministrador(id);
            manager.Usuario.GetUsuariosIntranet(QueryAdministrador);
            var c = this.RenderRazorViewToString("_TableAdministrador", QueryAdministrador);
            var result = new
            {
                Success = true,
                Data = c
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private Query<UsuarioIntranet> GetQueryAdministrador()
        {
            QueryAdministrador = QueryAdministrador ?? new Query<UsuarioIntranet>();
            QueryAdministrador = QueryAdministrador.Validate();
            QueryAdministrador.Paginacion = QueryAdministrador.Paginacion ?? new Paginacion();

            if (Session["CriteriaAdministrador"] != null)
            {
                if (Session["CriteriaAdministrador"] is UsuarioIntranet)
                {
                    QueryAdministrador.Criteria = (UsuarioIntranet)Session["CriteriaAdministrador"];
                }
                else
                {
                    Session["CriteriaAdministrador"] = null;
                    Session["PageAdministrador"] = null;
                    Session["OrdenAdministrador"] = null;
                }
            }

            if (Session["PageAdministrador"] != null)
            {
                QueryAdministrador.Paginacion.Page = (int)Session["PageAdministrador"];
            }

            if (Session["OrdenAdministrador"] != null)
            {
                QueryAdministrador.Order = (Order<UsuarioIntranet>)Session["OrderAdministrador"];
            }

            QueryAdministrador.BuildFilter();

            return QueryAdministrador;
        }
        #endregion

        #region ViewAnalista
        public virtual ActionResult IndexAnalista()
        {
            QueryAnalista = GetQueryAnalista();
            Manager.Usuario.GetUsuariosIntranetAnalista(QueryAnalista);
            ModelState.Clear();
            return View("IndexAnalista", QueryAnalista);
        }

        [HttpPost]
        public virtual ActionResult BuscarAnalista(UsuarioIntranet criteria)
        {
            Session["CriteriaAnalista"] = criteria;
            Session["PageAnalista"] = 1;
            ModelState.Clear();
            return RedirectToAction("IndexAnalista");
        }

        public virtual ActionResult PageAnalista(int page)
        {
            Session["PageAnalista"] = page;
            ModelState.Clear();
            return RedirectToAction("IndexAnalista");
        }

        protected Query<UsuarioIntranet> GetQueryAnalista()
        {
            QueryAnalista = QueryAnalista ?? new Query<UsuarioIntranet>();
            QueryAnalista = QueryAnalista.Validate();
            QueryAnalista.Paginacion = QueryAnalista.Paginacion ?? new Paginacion();

            if (Session["CriteriaAnalista"] != null)
            {
                if (Session["CriteriaAnalista"] is UsuarioIntranet)
                {
                    QueryAnalista.Criteria = (UsuarioIntranet)Session["CriteriaAnalista"];
                }
                else
                {
                    Session["CriteriaAnalista"] = null;
                    Session["PageAnalista"] = null;
                    Session["OrdenAnalista"] = null;
                }
            }

            if (Session["PageAnalista"] != null)
            {
                QueryAnalista.Paginacion.Page = (int)Session["PageAnalista"];
            }

            if (Session["OrdenAnalista"] != null)
            {
                QueryAnalista.Order = (Order<UsuarioIntranet>)Session["OrderAnalista"];
            }

            QueryAnalista.BuildFilter();

            return QueryAnalista;
        }
        #endregion

        #region ViewAsignacionEstablecimientoAnalista
        public ActionResult Establecimientos(int id)
        {
            var user = Manager.Usuario.FindUsuarioIntranet(id);
            ViewBag.NombreAnalista = user.Trabajador;

            if (user == null) return HttpNotFound("Analista no encontrado");
            EstablecimientoAnalista criteria;

            if (Session["CriteriaEstablecimientoAnalista"] == null)
            {
                criteria = new EstablecimientoAnalista();                
            }
            else
            {
                criteria = (EstablecimientoAnalista)Session["CriteriaEstablecimientoAnalista"];                
            }

            criteria.id_analista = id;
            Session["CriteriaEstablecimientoAnalista"] = criteria;

            QueryEstablecimientosAsignados = GetQueryEstablecimientosAsignados();
            Manager.EstablecimientoAnalistaManager.Get(QueryEstablecimientosAsignados);

            return View("Establecimientos", QueryEstablecimientosAsignados);
        }        

        [HttpPost]
        public virtual ActionResult BuscarEstablecimientosAsignados(EstablecimientoAnalista criteria)
        {
            int idAnalista = 0;
            QueryEstablecimientosAsignados = GetQueryEstablecimientosAsignados();
            idAnalista = (int) QueryEstablecimientosAsignados.Criteria.id_analista;
            Session["CriteriaEstablecimientoAnalista"] = criteria;
            Session["PageEstablecimientoAnalista"] = 1;
            ModelState.Clear();

            return RedirectToAction("Establecimientos", new { id = idAnalista });
        }

        public ActionResult PageEstablecimientosAsignados(int page)
        {
            int idAnalista = 0;
            QueryEstablecimientosAsignados = GetQueryEstablecimientosAsignados();
            idAnalista = (int)QueryEstablecimientosAsignados.Criteria.id_analista;
            Session["PageEstablecimientoAnalista"] = page;
            ModelState.Clear();

            return RedirectToAction("Establecimientos", new { id = idAnalista });
        }

        public Query<EstablecimientoAnalista> GetQueryEstablecimientosAsignados()
        {
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados ?? new Query<EstablecimientoAnalista>();
            QueryEstablecimientosAsignados = QueryEstablecimientosAsignados.Validate();
            QueryEstablecimientosAsignados.Criteria = new EstablecimientoAnalista();
            QueryEstablecimientosAsignados.Paginacion = QueryEstablecimientosAsignados.Paginacion ?? new Paginacion();

            if (Session["CriteriaEstablecimientoAnalista"] != null)
            {
                if (Session["CriteriaEstablecimientoAnalista"] is EstablecimientoAnalista)
                {
                    QueryEstablecimientosAsignados.Criteria = (EstablecimientoAnalista) Session["CriteriaEstablecimientoAnalista"];
                }
                else
                {
                    Session["CriteriaEstablecimientoAnalista"] = null;
                    Session["PageEstablecimientoAnalista"] = null;
                    Session["OrdenEstablecimientoAnalista"] = null;
                }
            }

            if (Session["PageEstablecimientoAnalista"] != null)
            {
                QueryEstablecimientosAsignados.Paginacion.Page = (int)Session["PageEstablecimientoAnalista"];
            }

            if (Session["OrdenEstablecimientoAnalista"] != null)
            {
                QueryEstablecimientosAsignados.Order = (Order<EstablecimientoAnalista>)Session["OrderEstablecimientoAnalista"];
            }

            QueryEstablecimientosAsignados.BuildFilter();

            return QueryEstablecimientosAsignados;
        }

        public JsonResult AsignarEstablecimiento(long id, long idCiiu, int orden)
        {
            int idAnalista = 0;
            QueryEstablecimientosAsignados = GetQueryEstablecimientosAsignados();
            QueryEstablecimientosNoAsignados = GetQueryEstablecimientosNoAsignados();
            idAnalista = (int)QueryEstablecimientosAsignados.Criteria.id_analista;
            Manager.Usuario.AsignarEstablecimientoAnalista(idAnalista, id, idCiiu, orden);
          
            Manager.Establecimiento.GetNoAsignadosAnalistas(QueryEstablecimientosNoAsignados);
            var noAsignados = this.RenderRazorViewToString("EstablecimientosNoAsignados", QueryEstablecimientosNoAsignados);
                       
            Manager.EstablecimientoAnalistaManager.Get(QueryEstablecimientosAsignados);
            var asignados = this.RenderRazorViewToString("_TableEstablecimientosAsignados", QueryEstablecimientosAsignados);

            var result = new
            {
                Success = true,
                Asignados = asignados,
                NoAsignados = noAsignados
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EliminarEstablecimiento(long id)
        {
            int idAnalista = 0;
            QueryEstablecimientosAsignados = GetQueryEstablecimientosAsignados();
            idAnalista = (int)QueryEstablecimientosAsignados.Criteria.id_analista;

            Manager.Usuario.EliminarEstablecimientoAnalista(id);

            return RedirectToAction("Establecimientos", new { id = idAnalista });
        }

        public ActionResult EstablecimientosNoAsignados()
        {
            QueryEstablecimientosNoAsignados = GetQueryEstablecimientosNoAsignados();
            Manager.Establecimiento.GetNoAsignadosAnalistas(QueryEstablecimientosNoAsignados);
            return PartialView("EstablecimientosNoAsignados", QueryEstablecimientosNoAsignados);
        }
                
        public virtual ActionResult BuscarEstablecimientosNoAsignados(Establecimiento criteria)
        {
            Session["CriteriaEstablecimientosNoAsignados"] = criteria;
            Session["PageEstablecimientosNoAsignados"] = 1;
            ModelState.Clear();
            return RedirectToAction("EstablecimientosNoAsignados");
        }
        
        public ActionResult PageEstablecimientosNoAsignados(int page)
        {
            Session["PageEstablecimientosNoAsignados"] = page;
            ModelState.Clear();
            return RedirectToAction("EstablecimientosNoAsignados");
        }

        public Query<Establecimiento> GetQueryEstablecimientosNoAsignados()
        {
            QueryEstablecimientosNoAsignados = QueryEstablecimientosNoAsignados ?? new Query<Establecimiento>();
            QueryEstablecimientosNoAsignados = QueryEstablecimientosNoAsignados.Validate();
            QueryEstablecimientosNoAsignados.Paginacion = QueryEstablecimientosNoAsignados.Paginacion ?? new Paginacion();

            if (Session["CriteriaEstablecimientosNoAsignados"] != null)
            {
                if (Session["CriteriaEstablecimientosNoAsignados"] is Establecimiento)
                {
                    QueryEstablecimientosNoAsignados.Criteria = (Establecimiento)Session["CriteriaEstablecimientosNoAsignados"];
                }
                else
                {
                    Session["CriteriaEstablecimientosNoAsignados"] = null;
                    Session["PageEstablecimientosNoAsignados"] = null;
                    Session["OrdenEstablecimientosNoAsignados"] = null;
                }
            }

            if (Session["PageEstablecimientosNoAsignados"] != null)
            {
                QueryEstablecimientosNoAsignados.Paginacion.Page = (int)Session["PageEstablecimientosNoAsignados"];
            }

            if (Session["OrdenEstablecimientosNoAsignados"] != null)
            {
                QueryEstablecimientosNoAsignados.Order = (Order<Establecimiento>)Session["OrderEstablecimientosNoAsignados"];
            }

            QueryEstablecimientosNoAsignados.BuildFilter();

            return QueryEstablecimientosNoAsignados;
        }
        #endregion

        //public ActionResult EstablecimientosEncuestaEmpresarial(UserInformation user)
        //{
        //    //Analista = Manager.Usuario.FindUsuarioIntranet(user.Id);

        //    Analista = UsuarioActual;

        //    if (Analista == null) return this.HttpNotFound("No se pudo encontrar el analista");
        //    QueryEstablecimientosAsignados = QueryEstablecimientosAsignados ?? new Query<EstablecimientoAnalista>();
        //    QueryEstablecimientosAsignados = QueryEstablecimientosAsignados.Validate();
        //    QueryEstablecimientosAsignados.Criteria = QueryEstablecimientosAsignados.Criteria ?? new EstablecimientoAnalista();
        //    QueryEstablecimientosAsignados.Criteria.id_analista = Analista.Identificador;
        //    QueryEstablecimientosAsignados.BuildFilter();
        //    QueryEstablecimientosAsignados.Paginacion.ItemsPerPage = 10;
        //    Manager.EstablecimientoAnalistaManager.Get(QueryEstablecimientosAsignados);
        //    if (QueryEstablecimientosAsignados.Elements.Count == 1 && QueryEstablecimientosAsignados.Filter == null)
        //    {
        //        var establecimiento = QueryEstablecimientosAsignados.Elements.FirstOrDefault();
        //        return RedirectToAction("Encuestas", "EncuestaEmpresarialAnalista", new { id = establecimiento.id_establecimiento });
        //    }
        //   // ViewBag.UserId = user.Id;
        //    ViewBag.UserId = Analista.Identificador;          
        //    return View("EstablecimientosEncuestaEmpresarial", QueryEstablecimientosAsignados);
        //}

        //[HttpPost]
        //public virtual ActionResult BuscarEstablecimientosEncuestaEmpresarial(EstablecimientoAnalista criteria)
        //{
        //    if (Analista == null) return HttpNotFound("Analista no encontrado");
        //    QueryEstablecimientosAsignados = QueryEstablecimientosAsignados ?? new Query<EstablecimientoAnalista>();
        //    QueryEstablecimientosAsignados = QueryEstablecimientosAsignados.Validate();
        //    QueryEstablecimientosAsignados.Criteria = criteria;
        //    QueryEstablecimientosAsignados.Paginacion = QueryEstablecimientosAsignados.Paginacion ?? new Paginacion();
        //    QueryEstablecimientosAsignados.Paginacion.Page = 1;
        //    QueryEstablecimientosAsignados.BuildFilter();
        //    return RedirectToAction("EstablecimientosEncuestaEmpresarial", new { id = Analista.Identificador });
        //}

        //public ActionResult PageEstablecimientosEncuestaEmpresarial(int page)
        //{
        //    if (Analista == null) return HttpNotFound("Analista no encontrado");
        //    QueryEstablecimientosAsignados = QueryEstablecimientosAsignados ?? new Query<EstablecimientoAnalista>();
        //    QueryEstablecimientosAsignados = QueryEstablecimientosAsignados.Validate();
        //    QueryEstablecimientosAsignados.Paginacion.Page = page;
        //    return RedirectToAction("EstablecimientosEncuestaEmpresarial", new { id = Analista.Identificador });
        //}

        public ActionResult EstablecimientosEncuestas(UserInformation user)
        {
            //brb
            //decimal IdAnalista = Manager.Usuario.FindUsuarioIntranet(user.Id).Identificador;
            //endbrb
            
            decimal IdAnalista = UsuarioActual.Identificador;

            ViewBag.UserId = IdAnalista;

            bool FirstLoad = false;

            if (Session["CriteriaEncuestas"] != null)
            {
                if (Session["CriteriaEncuestas"] is Establecimiento == false)
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
                Establecimiento criteria = new Establecimiento();
                criteria.IdAnalistaFilter = IdAnalista;
                Session["CriteriaEncuestas"] = criteria;
            }

            QueryEstablecimientosEncuestas = GetQueryEstablecimientosEncuestas();
            Manager.Establecimiento.Get("CAT_ESTAB_ANALISTA", QueryEstablecimientosEncuestas);

            return View("EstablecimientosEncuestas", QueryEstablecimientosEncuestas);
        }

        [HttpPost]
        public virtual ActionResult BuscarEstablecimientosEncuestas(Establecimiento criteria)
        {
            criteria.IdAnalistaFilter = ((Establecimiento)Session["CriteriaEncuestas"]).IdAnalistaFilter;
            Session["CriteriaEncuestas"] = criteria;
            Session["PageEncuestas"] = 1;
            ModelState.Clear();
            return RedirectToAction("EstablecimientosEncuestas");
        }

        public Query<Establecimiento> GetQueryEstablecimientosEncuestas()
        {
            QueryEstablecimientosEncuestas = QueryEstablecimientosEncuestas ?? new Query<Establecimiento>();
            QueryEstablecimientosEncuestas = QueryEstablecimientosEncuestas.Validate();
            QueryEstablecimientosEncuestas.Criteria = new Establecimiento();
            QueryEstablecimientosEncuestas.Paginacion = QueryEstablecimientosEncuestas.Paginacion ?? new Paginacion();

            if (Session["CriteriaEncuestas"] != null)
            {
                if (Session["CriteriaEncuestas"] is Establecimiento)
                {
                    QueryEstablecimientosEncuestas.Criteria = (Establecimiento)Session["CriteriaEncuestas"];
                }
                else
                {
                    Session["CriteriaEncuestas"] = null;
                    Session["PageEncuestas"] = null;
                    Session["OrdenEncuestas"] = null;
                }
            }

            if (Session["PageEncuestas"] != null)
            {
                QueryEstablecimientosEncuestas.Paginacion.Page = (int)Session["PageEncuestas"];
            }

            if (Session["OrdenEncuestas"] != null)
            {
                QueryEstablecimientosEncuestas.Order = (Order<Establecimiento>)Session["OrderEncuestas"];
            }

            QueryEstablecimientosEncuestas.BuildFilter();

            return QueryEstablecimientosEncuestas;
        }
    }
}
