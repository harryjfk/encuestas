using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using Domain;
using Domain.Managers;
using Entity;
using PagedList;
using WebApplication.Models;
using System.Configuration;
using System.Net.Mail;
using System.Globalization;
using System.Web;
using System.IO;
using OfficeOpenXml;
using System.Diagnostics;
using Seguridad.PRODUCE;
using SelectPdf;

namespace WebApplication.Controllers
{
    /*[Authorize]
    [Autorizacion]*/
    public class EncuestaEstadisticaController : BaseController<EncuestaEstadistica>
    {
        private Query<Contacto> QueryContacto { get; set; }

        private long IdEstablecimiento
        {
            get
            {
                if (Session["IdEstabEncuesta"] != null)
                {
                    return (long)Session["IdEstabEncuesta"];
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                Session["IdEstabEncuesta"] = value;
            }
        }

        private long IdEncuestaEstadistica
        {
            get
            {
                if (Session["IdEncuestaEstadistica"] != null)
                {
                    return (long)Session["IdEncuestaEstadistica"];
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                Session["IdEncuestaEstadistica"] = value;
            }
        }

        private Query<EncuestaEstadistica> QueryEncuestaForUpdate { get; set; }

        public UsuarioIntranet UsuarioActual
        {
            get
            {
                var user = this.GetLogued();
                return user != null ? Manager.Usuario.FindUsuarioIntranet((int)user.Identificador) : null;
            }
        }

        private Query<Contacto> GetQueryContacto()
        {
            QueryContacto = QueryContacto ?? new Query<Contacto>();
            QueryContacto = QueryContacto.Validate();
            QueryContacto.Criteria = new Contacto();
            QueryContacto.Paginacion = QueryContacto.Paginacion ?? new Paginacion();
            QueryContacto.Paginacion.ItemsPerPage = 1;

            if (Session["CriteriaContactoEncuesta"] != null)
            {
                if (Session["CriteriaContactoEncuesta"] is Contacto)
                {
                    QueryContacto.Criteria = (Contacto)Session["CriteriaContactoEncuesta"];
                }
                else
                {
                    Session["CriteriaContactoEncuesta"] = null;
                    Session["PageContactoEncuesta"] = null;
                    Session["OrdenContactoEncuesta"] = null;
                }
            }

            if (Session["PageContactoEncuesta"] != null)
            {
                QueryContacto.Paginacion.Page = (int)Session["PageContactoEncuesta"];
            }

            if (Session["OrdenContactoEncuesta"] != null)
            {
                QueryContacto.Order = (Order<Contacto>)Session["OrderContactoEncuesta"];
            }

            QueryContacto.BuildFilter();

            return QueryContacto;
        }

        public ActionResult EncuestasAnalista(long id, UserInformation user)
        {
            var manager = Manager;
            //brb
            //var idUsuario = user.Id;
            //endbrb

            var idUsuario = this.GetLogued().Identificador;                        
            
            IdEstablecimiento = id;
            var establecimiento = Manager.Establecimiento.Find(id);
            if (establecimiento == null) return HttpNotFound("Establecimiento no encontrado");
            ViewBag.Establecimiento = establecimiento;

            bool FirstLoad = false;
            EncuestaEstadistica criteria = new EncuestaEstadistica();

            if (Session[CriteriaSesion] != null)
            {
                if (Session[CriteriaSesion] is EncuestaEstadistica == false)
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
                criteria.IdEstablecimiento = IdEstablecimiento;
                criteria.Year = DateTime.Now.Year;
                
            }
            else
            {
                criteria = (EncuestaEstadistica)Session[CriteriaSesion];
                criteria.IdEstablecimiento = IdEstablecimiento;
            }

            Session[CriteriaSesion] = criteria;

            Query = GetQuery();

            Manager.EncuestaEstadistica.GetAsignadosAnalista(Query, (long)idUsuario);

            ModelState.Clear();
            return View("IndexAnalista", Query);
        }

        [HttpPost]
        public ActionResult BuscarEncuestaAnalista(EncuestaEstadistica criteria)
        {            
            Session[CriteriaSesion] = criteria;
            Session[PageSesion] = 1;

            return RedirectToAction("EncuestasAnalista", new { id = IdEstablecimiento });
        }

        #region ViewUpdateEncuestas
        public ActionResult PreviousUpdate(long id)
        {
            var establecimiento = Manager.Establecimiento.Find(id);
            if (establecimiento == null) return HttpNotFound("Establecimiento no encontrado");
            ViewBag.Establecimiento = establecimiento;
            IdEstablecimiento = id;

            bool FirstLoad = false;

            if (Session["CriteriaForUpdate"] != null)
            {
                if (Session["CriteriaForUpdate"] is EncuestaEstadistica == false)
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
                EncuestaEstadistica criteria = new EncuestaEstadistica();
                criteria.IdEstablecimiento = id;
                criteria.Year = DateTime.Now.Year;

                Session["CriteriaForUpdate"] = criteria;
            }

            QueryEncuestaForUpdate = GetQueryEncuestaForUpdate();
            Manager.EncuestaEstadistica.GetForUpdate(QueryEncuestaForUpdate);
            ModelState.Clear();

            return View(QueryEncuestaForUpdate);
        }

        [HttpPost]
        public ActionResult SearchEncuestaForUpdate(EncuestaEstadistica criteria)
        {
            long EstablecimientoId = ((EncuestaEstadistica)Session["CriteriaForUpdate"]).IdEstablecimiento;
            criteria.IdEstablecimiento = EstablecimientoId;
            Session["CriteriaForUpdate"] = criteria;
            Session["PageForUpdate"] = 1;
            ModelState.Clear();            
            
            return RedirectToAction("PreviousUpdate", new { id = EstablecimientoId });
        }

        public virtual ActionResult PageForUpdate(int page)
        {
            long EstablecimientoId = ((EncuestaEstadistica)Session["CriteriaForUpdate"]).IdEstablecimiento;
            Session["PageForUpdate"] = page;
            ModelState.Clear();
            return RedirectToAction("PreviousUpdate", new { id = EstablecimientoId });
        }

        [HttpPost]
        public JsonResult OpenForUpdate(long idEncuesta, int tipoPermiso)
        {
            var manager = Manager;

            var encuestas = manager.EncuestaEstadistica.Get(t => t.actualizacion == 1);
            if (encuestas.Count == 0)
            {
                var encuesta = manager.EncuestaEstadistica.FindById(idEncuesta);

                if (tipoPermiso == 1)
                {
                    encuesta.actualizacion = 1;
                }
                else
                {
                    encuesta.EstadoEncuesta = EstadoEncuesta.NoEnviada;
                    encuesta.fecha_validacion = null;
                    encuesta.actualizacion = 1;

                    bool first = true;
                    foreach (var analista in encuesta.DAT_ENCUESTA_ANALISTA.OrderBy(t => t.orden))
                    {
                        if (first)
                        {
                            analista.current = 1;
                            first = false;
                        }
                        else
                        {
                            analista.current = 0;
                        }

                        analista.EstadoEncuesta = EstadoEncuesta.NoEnviada;

                        manager.EncuestaAnalistaManager.Modify(analista);                        
                    }

                    manager.EncuestaAnalistaManager.SaveChanges();
                }

                manager.EncuestaEstadistica.Modify(encuesta);
                manager.EncuestaEstadistica.SaveChanges();

                return Json("1");
            }
            else
            {
                return Json("2");
            }
        }

        public Query<EncuestaEstadistica> GetQueryEncuestaForUpdate()
        {
            QueryEncuestaForUpdate = QueryEncuestaForUpdate ?? new Query<EncuestaEstadistica>();
            QueryEncuestaForUpdate = QueryEncuestaForUpdate.Validate();
            QueryEncuestaForUpdate.Criteria = new EncuestaEstadistica();
            QueryEncuestaForUpdate.Paginacion = QueryEncuestaForUpdate.Paginacion ?? new Paginacion();

            if (Session["CriteriaForUpdate"] != null)
            {
                if (Session["CriteriaForUpdate"] is EncuestaEstadistica)
                {
                    QueryEncuestaForUpdate.Criteria = (EncuestaEstadistica)Session["CriteriaForUpdate"];
                }
                else
                {
                    Session["CriteriaForUpdate"] = null;
                    Session["PageForUpdate"] = null;
                    Session["OrdenForUpdate"] = null;
                }
            }

            if (Session["PageForUpdate"] != null)
            {
                QueryEncuestaForUpdate.Paginacion.Page = (int)Session["PageForUpdate"];
            }

            if (Session["OrdenForUpdate"] != null)
            {
                QueryEncuestaForUpdate.Order = (Order<EncuestaEstadistica>)Session["OrderForUpdate"];
            }

            QueryEncuestaForUpdate.BuildFilter();

            return QueryEncuestaForUpdate;
        }
        #endregion

        public ActionResult Validar(long id)
        {
            Manager.EncuestaEstadistica.Validar(id);
            return RedirectToAction("EncuestasAnalista", new { id = IdEstablecimiento });
        }

        [HttpPost]
        public ActionResult Observar(long id, string observacion)
        {
            Encuesta encuesta = Manager.Encuesta.Find(id);
            if (encuesta != null)
            {
                var contactoPredeterminado = encuesta.Establecimiento.ContactoPredeterminado;

                if (contactoPredeterminado != null)
                {
                    string correo = contactoPredeterminado.Correo;

                    if (!string.IsNullOrEmpty(correo))
                    {
                        try
                        {
                            var appSettings = ConfigurationManager.AppSettings;                                
                            string contenido = string.Format(appSettings["BodyObservacion"], encuesta.Fecha.ToString("yyyy", CultureInfo.GetCultureInfo("ES")).ToUpper(),
                                encuesta.Fecha.ToString("MMMM", CultureInfo.GetCultureInfo("ES")).ToUpper(),
                                encuesta.fecha_ultimo_envio.GetValueOrDefault().ToString("D", CultureInfo.GetCultureInfo("es")),
                                encuesta.Establecimiento.Nombre, observacion);
                            Data.Tools.EnviarCorreo(correo, appSettings["AsuntoObservacion"], contenido);
                        }
                        catch (Exception)
                        {

                            //throw;
                        }
                    }
                }
                
            }
            Manager.EncuestaEstadistica.Observar(id, observacion);
            return RedirectToAction("EncuestasAnalista", new { id = IdEstablecimiento });
        }

        [HttpPost]
        public JsonResult UpdatePrevious(EncuestaEstadistica encuesta)
        {
            var manager = Manager;

            List<string> errors = manager.EncuestaEstadistica.UpdatePrevious(encuesta);

            if (errors.Count == 0)
            {
                var res = new
                {
                    Success = true
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else
            {                
                var res = new
                {
                    Success = false,                    
                    Errors = errors
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase excelPoll)
        {

            if (excelPoll.ContentLength > 0)
            {
                string filePath = Path.Combine(HttpContext.Server.MapPath("../TempFiles"),
                                               Path.GetFileName(excelPoll.FileName));
                excelPoll.SaveAs(filePath);

                var excelPollFile = new FileInfo(filePath);

                List<EncuestaEstadisticaUploadModel> encuestaEstadisticasUpload = new List<EncuestaEstadisticaUploadModel>();
                List<EncuestaEstadisticaUploadModel_2> encuestaEstadisticasUpload_2 = new List<EncuestaEstadisticaUploadModel_2>();
                List<EncuestaEstadisticaUploadModel_3> encuestaEstadisticasUpload_3 = new List<EncuestaEstadisticaUploadModel_3>();
                List<EncuestaEstadisticaUploadModel_4> encuestaEstadisticasUpload_4 = new List<EncuestaEstadisticaUploadModel_4>();
                List<EncuestaEstadisticaUploadModel_5> encuestaEstadisticasUpload_5 = new List<EncuestaEstadisticaUploadModel_5>();

                using (var package = new ExcelPackage(excelPollFile))
                {
                    ExcelWorkbook workBook = package.Workbook;
                    if (workBook != null)
                    {
                        if (workBook.Worksheets.Count > 4)
                        {
                            int pageIndex = 1;
                            foreach (ExcelWorksheet currentWorksheet in workBook.Worksheets)
                            {
                                var end = currentWorksheet.Dimension.End;

                                for (int row = 2; row <= end.Row; ++row)
                                {
                                    if (currentWorksheet.Cells[row, 1].Value == null)
                                    {
                                        break;
                                    }

                                    if (pageIndex == 1)
                                    {
                                        EncuestaEstadisticaUploadModel encuestaEstadisticaUpload = new EncuestaEstadisticaUploadModel();

                                        encuestaEstadisticaUpload.IdInternoEstablecimiento = currentWorksheet.Cells[row, 1].Value.ToString();
                                        encuestaEstadisticaUpload.Fecha = Convert.ToDateTime(currentWorksheet.Cells[row, 2].Value);
                                        encuestaEstadisticaUpload.CodigoCIIU = currentWorksheet.Cells[row, 3].Value.ToString();
                                        encuestaEstadisticaUpload.CodigoLineaProducto = currentWorksheet.Cells[row, 6].Value.ToString();
                                        encuestaEstadisticaUpload.AbreviaturaUM = currentWorksheet.Cells[row, 7].Value.ToString();
                                        encuestaEstadisticaUpload.Materia = currentWorksheet.Cells[row, 14].Value.ToString();
                                        encuestaEstadisticaUpload.Produccion = Convert.ToDecimal(currentWorksheet.Cells[row, 10].Value);
                                        if (encuestaEstadisticaUpload.Materia == "P")
                                        {
                                            encuestaEstadisticaUpload.ValorUnitario = Convert.ToDecimal(currentWorksheet.Cells[row, 8].Value);
                                            encuestaEstadisticaUpload.Existencia = Convert.ToDecimal(currentWorksheet.Cells[row, 9].Value);
                                            encuestaEstadisticaUpload.VentasPais = Convert.ToDecimal(currentWorksheet.Cells[row, 11].Value);
                                            encuestaEstadisticaUpload.VentasExtranjero = Convert.ToDecimal(currentWorksheet.Cells[row, 12].Value);
                                            encuestaEstadisticaUpload.OtrasSalidas = Convert.ToDecimal(currentWorksheet.Cells[row, 13].Value);
                                        }

                                        encuestaEstadisticasUpload.Add(encuestaEstadisticaUpload);
                                    }

                                    if (pageIndex == 2)
                                    {
                                        EncuestaEstadisticaUploadModel_2 encuestaEstadisticaUpload_2 = new EncuestaEstadisticaUploadModel_2();

                                        encuestaEstadisticaUpload_2.IdInternoEstablecimiento = currentWorksheet.Cells[row, 1].Value.ToString();
                                        encuestaEstadisticaUpload_2.Fecha = Convert.ToDateTime(currentWorksheet.Cells[row, 2].Value);
                                        encuestaEstadisticaUpload_2.CodigoCIIU = currentWorksheet.Cells[row, 3].Value.ToString();

                                        if (currentWorksheet.Cells[row, 4].Value == null)
                                        {
                                            encuestaEstadisticaUpload_2.ValorTercero = null;
                                        }
                                        else
                                        {
                                            encuestaEstadisticaUpload_2.ValorTercero = Convert.ToDecimal(currentWorksheet.Cells[row, 4].Value);
                                        }

                                        if (currentWorksheet.Cells[row, 5].Value == null)
                                        {
                                            encuestaEstadisticaUpload_2.ValorVentaPais = null;
                                        }
                                        else
                                        {
                                            encuestaEstadisticaUpload_2.ValorVentaPais = Convert.ToDecimal(currentWorksheet.Cells[row, 5].Value);
                                        }

                                        if (currentWorksheet.Cells[row, 6].Value == null)
                                        {
                                            encuestaEstadisticaUpload_2.ValorVentaExtranjero = null;
                                        }
                                        else
                                        {
                                            encuestaEstadisticaUpload_2.ValorVentaExtranjero = Convert.ToDecimal(currentWorksheet.Cells[row, 6].Value);
                                        }

                                        encuestaEstadisticasUpload_2.Add(encuestaEstadisticaUpload_2);
                                    }

                                    if (pageIndex == 3)
                                    {
                                        EncuestaEstadisticaUploadModel_3 encuestaEstadisticaUpload_3 = new EncuestaEstadisticaUploadModel_3();
                                        encuestaEstadisticaUpload_3.IdInternoEstablecimiento = currentWorksheet.Cells[row, 1].Value.ToString();
                                        encuestaEstadisticaUpload_3.Fecha = Convert.ToDateTime(currentWorksheet.Cells[row, 2].Value);
                                        encuestaEstadisticaUpload_3.CodigoCIIU = currentWorksheet.Cells[row, 3].Value.ToString();
                                        encuestaEstadisticaUpload_3.VentaPais = Convert.ToDecimal(currentWorksheet.Cells[row, 4].Value);
                                        encuestaEstadisticaUpload_3.VentaExtranjero = Convert.ToDecimal(currentWorksheet.Cells[row, 5].Value);
                                        encuestaEstadisticasUpload_3.Add(encuestaEstadisticaUpload_3);
                                    }

                                    if (pageIndex == 4)
                                    {
                                        EncuestaEstadisticaUploadModel_4 encuestaEstadisticaUpload_4 = new EncuestaEstadisticaUploadModel_4();
                                        encuestaEstadisticaUpload_4.IdInternoEstablecimiento = currentWorksheet.Cells[row, 1].Value.ToString();
                                        encuestaEstadisticaUpload_4.Fecha = Convert.ToDateTime(currentWorksheet.Cells[row, 2].Value);
                                        encuestaEstadisticaUpload_4.DiasTrabajados = Convert.ToInt32(currentWorksheet.Cells[row, 3].Value);
                                        encuestaEstadisticaUpload_4.Trabajadores = Convert.ToInt32(currentWorksheet.Cells[row, 4].Value);
                                        encuestaEstadisticaUpload_4.Administrativos = Convert.ToInt32(currentWorksheet.Cells[row, 5].Value);
                                        encuestaEstadisticasUpload_4.Add(encuestaEstadisticaUpload_4);
                                    }

                                    if (pageIndex == 5)
                                    {
                                        EncuestaEstadisticaUploadModel_5 encuestaEstadisticaUpload_5 = new EncuestaEstadisticaUploadModel_5();

                                        encuestaEstadisticaUpload_5.IdInternoEstablecimiento = currentWorksheet.Cells[row, 1].Value.ToString();
                                        encuestaEstadisticaUpload_5.Fecha = Convert.ToDateTime(currentWorksheet.Cells[row, 2].Value);

                                        encuestaEstadisticaUpload_5.AumentoDemandaInterna = Convert.ToBoolean(currentWorksheet.Cells[row, 3].Value);
                                        encuestaEstadisticaUpload_5.AumentoCapacidadInstalada = Convert.ToBoolean(currentWorksheet.Cells[row, 4].Value);
                                        encuestaEstadisticaUpload_5.CambiosTecnologicos = Convert.ToBoolean(currentWorksheet.Cells[row, 5].Value);
                                        encuestaEstadisticaUpload_5.CampaniaEstacionalidadProducto = Convert.ToBoolean(currentWorksheet.Cells[row, 6].Value);
                                        encuestaEstadisticaUpload_5.IncrementoExportacion = Convert.ToBoolean(currentWorksheet.Cells[row, 7].Value);
                                        encuestaEstadisticaUpload_5.ReposicionExistencias = Convert.ToBoolean(currentWorksheet.Cells[row, 8].Value);

                                        encuestaEstadisticaUpload_5.CompetenciaDesleal = Convert.ToBoolean(currentWorksheet.Cells[row, 9].Value);
                                        encuestaEstadisticaUpload_5.ContrabandoPirateria = Convert.ToBoolean(currentWorksheet.Cells[row, 10].Value);
                                        encuestaEstadisticaUpload_5.DemandaInternaLimitada = Convert.ToBoolean(currentWorksheet.Cells[row, 11].Value);
                                        encuestaEstadisticaUpload_5.DificultadAccesoFinanciamiento = Convert.ToBoolean(currentWorksheet.Cells[row, 12].Value);
                                        encuestaEstadisticaUpload_5.DificultadAbastecimientoInsumos = Convert.ToBoolean(currentWorksheet.Cells[row, 13].Value);
                                        encuestaEstadisticaUpload_5.DisminucionExportaciones = Convert.ToBoolean(currentWorksheet.Cells[row, 14].Value);
                                        encuestaEstadisticaUpload_5.FaltaCapitalTrabajo = Convert.ToBoolean(currentWorksheet.Cells[row, 15].Value);
                                        encuestaEstadisticaUpload_5.FaltaEnergia = Convert.ToBoolean(currentWorksheet.Cells[row, 16].Value);
                                        encuestaEstadisticaUpload_5.FaltaPersonalCalificado = Convert.ToBoolean(currentWorksheet.Cells[row, 17].Value);
                                        encuestaEstadisticaUpload_5.MantenimientoEquipos = Convert.ToBoolean(currentWorksheet.Cells[row, 18].Value);
                                        encuestaEstadisticaUpload_5.VacacionesColectivas = Convert.ToBoolean(currentWorksheet.Cells[row, 19].Value);
                                        encuestaEstadisticaUpload_5.AltasExistencias = Convert.ToBoolean(currentWorksheet.Cells[row, 20].Value);
                                        encuestaEstadisticaUpload_5.HuelgaParos = Convert.ToBoolean(currentWorksheet.Cells[row, 21].Value);

                                        encuestaEstadisticasUpload_5.Add(encuestaEstadisticaUpload_5);
                                    }
                                }

                                pageIndex += 1;
                            }
                        }
                    }
                }

                System.IO.File.Delete(filePath);

                Manager.EncuestaEstadistica.GenerateUploadExcel(encuestaEstadisticasUpload, encuestaEstadisticasUpload_2, encuestaEstadisticasUpload_3, encuestaEstadisticasUpload_4, encuestaEstadisticasUpload_5);
            }

            return RedirectToAction("Index", "Establecimiento");
        }

        public ActionResult EncuestaAnalista(UserInformation user, long idEncuesta)
        {
            var manager = Manager;

            var encuesta = manager.EncuestaEstadistica.FindById(idEncuesta);
            if (encuesta == null) return HttpNotFound("Encuesta No encontrada");
            IdEncuestaEstadistica = encuesta.Id;

            var contactoPredeterminado = encuesta.Establecimiento.ContactoPredeterminado;

            if (contactoPredeterminado != null)
            {
                ViewBag.CorreoInformante = contactoPredeterminado.Correo;
            }
            else
            {
                ViewBag.CorreoInformante = "No hay contacto";
            }

            //brb
            //var idUsuario = user.Id;
            //endbrb

            var idUsuario = UsuarioActual.Identificador;

            if (user == null) return View("VerEncuesta", encuesta);

            var analist = encuesta.DAT_ENCUESTA_ANALISTA.Where(t => t.id_analista == idUsuario).OrderBy(t => t.orden).FirstOrDefault();
           
            if (analist == null || analist.IsWaiting) return HttpNotFound("Analista no encontrado");
            if (encuesta.EstadoEncuesta == EstadoEncuesta.Enviada && analist.IsCurrent)
                return View("EncuestaAnalista", encuesta);
            return View("VerEncuesta", encuesta);
        }

        public ActionResult Encuesta(long idEncuesta = 0)
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(idEncuesta);
            if (encuesta == null) return HttpNotFound("Encuesta No encontrada");
            IdEncuestaEstadistica = encuesta.Id;
            return View("Encuesta", encuesta);            
        }

        public ActionResult EncuestaViewPrint(long idEncuesta = 0)
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(idEncuesta);
            if (encuesta == null) return HttpNotFound("Encuesta No encontrada");
            IdEncuestaEstadistica = encuesta.Id;
            return View("EncuestaViewPrint", encuesta);
        }

        public ActionResult GetNombreDistrito(string idDistrito)
        {
            var distrito = Manager.Distrito.Find(idDistrito);
            var name = "...";
            if (distrito != null)
                name = distrito.Nombre;
            return PartialView("_TextView", name);
        }

        public ActionResult GetNombreDepartamento(string idDepartamento)
        {
            var departamento = Manager.Departamento.Find(idDepartamento);
            var name = "...";
            if (departamento != null)
                name = departamento.Nombre;
            return PartialView("_TextView", name);
        }

        public ActionResult GetNombreProvincia(string idProvincia)
        {
            var provincia = Manager.Provincia.Find(idProvincia);
            var name = "...";
            if (provincia != null)
                name = provincia.Nombre;
            return PartialView("_TextView", name);
        }

        public JsonResult DeleteProdMatTer(long id)
        {
            var manager = Manager;
            var element = manager.MateriaTercerosManager.Find(id);
            if (element == null)
            {
                var res = new
                {
                    Success = false,
                    Errors = new List<string>() { "Elemento no encontrado" }
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            var idVolumen = element.VolumenProduccion.Identificador;
            var op = manager.MateriaTercerosManager.Delete(id);
            manager.MateriaTercerosManager.SaveChanges();
            if (op.Success)
            {
                var elements = Manager.MateriaTercerosManager.Get(t => t.VolumenProduccion.Identificador == idVolumen);
                var data = RenderRazorViewToString("_TableProductoMateriaPrimaTerceros", elements);
                var res = new
                {
                    Success = true,
                    Data = data
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var res = new
                {
                    Success = false,
                    Errors = new List<string>() { "Elemento no encontrado" }
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditProdMatTer(long id = 0)
        {
            var producto = Manager.MateriaTercerosManager.Find(id);
            producto = producto ?? new MateriaTerceros();
            return PartialView("_ProductoMateriaPrimaTerceros", producto);
        }

        [HttpPost]
        public JsonResult CreateProdMatTer(MateriaTerceros producto)
        {
            if (ModelState.IsValid)
            {
                var manager = Manager;
                var encuesta = manager.EncuestaEstadistica.FindById(IdEncuestaEstadistica);
                producto.IdVolumenProduccion = encuesta.VolumenProduccionMensual.Identificador;
                var op = producto.Id == 0 ?
                    manager.MateriaTercerosManager.Add(producto) :
                    manager.MateriaTercerosManager.Modify(producto);
                if (op.Success)
                {
                    manager.MateriaTercerosManager.SaveChanges();
                    var elements = Manager.MateriaTercerosManager.Get(t => t.IdVolumenProduccion == encuesta.VolumenProduccionMensual.Identificador);
                    var data = RenderRazorViewToString("_TableProductoMateriaPrimaTerceros", elements);
                    var res = new
                    {
                        Success = true,
                        Data = data
                    };
                    return Json(res, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var result = new
                    {
                        Success = false,
                        Errors = new List<string>() { op.Errors[0] }
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var list = new List<string>();
                foreach (var v in ModelState.Values)
                    list.AddRange(v.Errors.Select(t => t.ErrorMessage));
                var result = new
                {
                    Success = false,
                    Errors = list
                };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult ContactosEncuesta(long id)
        {
            //var id = IdEstablecimiento;
            var manager = Manager;
            var establecimiento = manager.Establecimiento.Find(id);
            if (establecimiento == null) return HttpNotFound("Establecimiento no encontrado");
            Contacto criteria;
            if (Session["CriteriaContactoEncuesta"] == null)
            {
                criteria = new Contacto();
            }
            else
            {
                criteria = (Contacto)Session["CriteriaContactoEncuesta"];
            }

            criteria.IdEstablecimiento = id;
            Session["CriteriaContactoEncuesta"] = criteria;

            QueryContacto = GetQueryContacto();


            Manager.Contacto.Get(QueryContacto);
            return PartialView("_ContactosEncuesta", Tuple.Create<Contacto, Query<Contacto>>(null, QueryContacto));
        }

        [HttpPost]
        public virtual JsonResult CreateNuevoContacto(Contacto element)
        {
            if (ModelState.IsValid)
            {
                var id = IdEstablecimiento;
                var establecimiento = Manager.Establecimiento.Find(id);
                if (establecimiento == null)
                {
                    var result = new
                    {
                        Success = false,
                        Errors = new List<string>() { "Establecimiento no encontrado" }
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                element.IdEstablecimiento = IdEstablecimiento;

                Contacto criteria;
                if (Session["CriteriaContactoEncuesta"] == null)
                {
                    criteria = new Contacto();
                }
                else
                {
                    criteria = (Contacto)Session["CriteriaContactoEncuesta"];
                }

                criteria.IdEstablecimiento = id;
                Session["CriteriaContactoEncuesta"] = criteria;

                QueryContacto = GetQueryContacto();

                var manager = Manager.Contacto;
                var op = manager.Add(element);

                if (op.Success)
                {
                    manager.SaveChanges();
                    //this.WriteMessage(string.Format("Actualizando datos del ciiu {0}", ciiu.Nombre));
                    Manager.Contacto.Get(QueryContacto);
                    var c = RenderRazorViewToString("_ContactosEncuesta", Tuple.Create<Contacto, Query<Contacto>>(null, QueryContacto));
                    var result = new
                    {
                        Success = true,
                        Data = c
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = new
                    {
                        Success = false,
                        Errors = op.Errors.Select(t => t).ToList()
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var list = new List<string>();
                foreach (var v in ModelState.Values)
                    list.AddRange(v.Errors.Select(t => t.ErrorMessage));
                var result = new
                {
                    Success = false,
                    Errors = list
                };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }

        public virtual ActionResult PageContactoEncuesta(int page)
        {
            Session["PageContactoEncuesta"] = page;
            return RedirectToAction("ContactosEncuesta", new { id = IdEstablecimiento });
        }

        public ActionResult ToggleContacto(int id)
        {
            Manager.Contacto.EstablecerPredeterminado(id);
            var encuesta = Manager.EncuestaEstadistica.FindById(IdEncuestaEstadistica);
            return PartialView("_IdentificacionEstablecimiento", encuesta);
        }

        [HttpPost]
        public JsonResult SaveMateriaPropia(List<MateriaPropia> materias)
        {
            var manager = Manager;
            var produccion = new List<long>();
            var valorUnitario = new List<long>();
            var ventaPais = new List<long>();
            var ventaExtranjero = new List<long>();
            var errors = new List<string>();

            foreach (var materia in materias)
            {
                var mat = manager.MateriaPropiaManager.Find(materia.Id);
                if (mat != null)
                {
                    if (!materia.IsValid)
                    {
                        errors.Add(string.Format("Los valores insertados para la línea de producto {0} no son correctos. (Existencias + Producción + Otros Ingresos) ≥ (VentasPais + Ventas Extranjeros + Otras Salidas)", mat.LineaProducto.Nombre));
                        continue;
                    }
                    if (!materia.IsValidProduccionValorUnitario)
                    {
                        errors.Add(string.Format("Los valores insertados para la línea de producto {0} no son correctos. Sí digitó Valor Unitario o Producción, ambos campos son requeridos.", mat.LineaProducto.Nombre));
                        continue;
                    }

                    if (mat.IsFirst)
                    {
                        mat.Existencia = materia.Existencia;
                    }

                    var validProduccion = !Manager.MateriaPropiaManager.ValidarProduccion(materia.Id, materia.Produccion);
                    var validValorUnitario = !Manager.MateriaPropiaManager.ValidarValorUnitario(materia.Id, materia.ValorUnitario);
                    var validVentasPais = !Manager.MateriaPropiaManager.ValidarVentasPais(materia.Id, materia.VentasPais);
                    var validVentasExtranjero = !Manager.MateriaPropiaManager.ValidarVentasExtranjero(materia.Id, materia.VentasExtranjero);

                    if (validProduccion && materia.Produccion != null)
                    {
                        produccion.Add(materia.Id);
                        mat.JustificacionProduccion = (materia.JustificacionProduccion == null) ? " " : materia.JustificacionProduccion;
                    }
                    else
                    {
                        mat.JustificacionProduccion = null;
                    }

                    if (validValorUnitario && materia.ValorUnitario != null)
                    {
                        valorUnitario.Add(materia.Id);
                        mat.JustificacionValorUnitario = (materia.JustificacionValorUnitario == null) ? " " : materia.JustificacionValorUnitario;
                    }
                    else
                    {
                        mat.JustificacionValorUnitario = null;
                    }

                    if (validVentasPais && materia.VentasPais != null)
                    {
                        ventaPais.Add(materia.Id);
                        mat.justificacion_venta_pais = (materia.justificacion_venta_pais == null) ? " " : materia.justificacion_venta_pais;
                    }
                    else
                    {
                        mat.justificacion_venta_pais = null;
                    }

                    if (validVentasExtranjero && materia.VentasExtranjero != null)
                    {
                        ventaExtranjero.Add(materia.Id);
                        mat.justificacion_venta_extranjero = (materia.justificacion_venta_extranjero == null) ? " " : materia.justificacion_venta_extranjero;
                    }
                    else
                    {
                        mat.justificacion_venta_extranjero = null;
                    }

                    mat.IdUnidadMedida = materia.IdUnidadMedida > 0 ? materia.IdUnidadMedida : mat.IdUnidadMedida;
                    mat.OtrasSalidas = materia.OtrasSalidas;
                    mat.OtrosIngresos = materia.OtrosIngresos;
                    mat.Produccion = materia.Produccion;
                    mat.ValorUnitario = materia.ValorUnitario;
                    mat.VentasExtranjero = materia.VentasExtranjero;
                    mat.VentasPais = materia.VentasPais;

                    manager.MateriaPropiaManager.Modify(mat);
                    manager.MateriaPropiaManager.SaveChanges();
                }
                //else
                //{
                //    var result = new
                //    {
                //        Success = false
                //    };
                //    return Json(result, JsonRequestBehavior.AllowGet);
                //}
                //Manager.EncuestaEstadistica.UpdateExistenciasEncuestas(mat.VolumenProduccion.Encuesta.Id, mat.IdLineaProducto);
            }
            if (errors.Any())
            {
                var res = new
                {
                    Success = false,
                    Errors = errors
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            var encuesta = Manager.EncuestaEstadistica.FindById(IdEncuestaEstadistica);
            Manager.ValorProduccionManager.Generate(encuesta.Id);
            var d = this.RenderRazorViewToString("_ValorProduccionEstablecimiento", encuesta);
            var e = this.RenderRazorViewToString("_ValorVentasProductosEstablecimiento", encuesta);
            var result = new
            {
                Success = true,
                ValorUnitario = valorUnitario,
                Produccion = produccion,
                ValorProduccion = d,
                Ventas = e,
                VentasPais = ventaPais,
                VentasExtranjero = ventaExtranjero
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveValorProduccion(List<ValorProduccion> valores)
        {
            var manager = Manager;
            var list = new List<long>();
            var errors = new List<string>();
            foreach (var materia in valores)
            {
                var valor = manager.ValorProduccionManager.Find(materia.Id);
                if (valor != null)
                {
                    if (valor.MateriaPrimaTercerosActivada && materia.ProductosMateriaTerceros == null)
                    {
                        errors.Add(string.Format("Debe ingresar un valor en  Materia prima de terceros para el CIIU {0}", valor.CAT_CIIU.Nombre));
                        continue;
                    }

                    valor.ProductosMateriaTerceros = materia.ProductosMateriaTerceros;
                    var validmateriaTerceros = !Manager.ValorProduccionManager.ValidarMateriaTerceros(materia.Id, materia.ProductosMateriaTerceros);

                    if (validmateriaTerceros && materia.ProductosMateriaTerceros != null)
                    {
                        list.Add(materia.Id);
                        valor.justificacion_materia_terc = (materia.justificacion_materia_terc == null) ? " " : materia.justificacion_materia_terc;
                    }
                    else
                    {
                        valor.justificacion_materia_terc = null;
                    }

                    manager.ValorProduccionManager.Modify(valor);
                    manager.ValorProduccionManager.SaveChanges();
                }
            }

            if (errors.Count > 0)
            {
                var res = new
                {
                    Elements = new List<long>(),
                    Success = false,
                    Errors = errors
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            var result = new
            {
                Success = true,
                Elements = list
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveVentas(List<VentasPaisExtranjero> ventas)
        {
            var manager = Manager;
            var ext = new List<long>();
            var pais = new List<long>();
            var errors = new List<string>();
            bool esJustificado = true;
            foreach (var venta in ventas)
            {
                var valor = manager.VentasPaisExtranjeroManager.Find(venta.Id);
                var s = false;
                if (valor != null)
                {

                    if (valor.VentaExtranjeroActivado && venta.VentaExtranjero == null)
                    {
                        errors.Add(string.Format("Debe ingresar un valor en  Venta  extranjero para el CIIU {0}", valor.CAT_CIIU.Nombre));
                        s = true;
                    }
                    if (valor.VentaPaisActivado && venta.VentaPais == null)
                    {
                        errors.Add(string.Format("Debe ingresar un valor en  Venta  pais para el CIIU {0}", valor.CAT_CIIU.Nombre));
                        s = true;
                    }
                    if (s) continue;

                    var validExt =
                       !manager.VentasPaisExtranjeroManager.ValidarVentaExtranjero(venta.Id, venta.VentaExtranjero);
                    var validPais =
                        !manager.VentasPaisExtranjeroManager.ValidarVentaPais(venta.Id, venta.VentaPais);

                    if (validExt && venta.VentaExtranjero != null)
                    {
                        ext.Add(venta.Id);
                        if (venta.justificacion_venta_ext == null)
                        {
                            esJustificado = false;
                        }
                        venta.justificacion_venta_ext = (venta.justificacion_venta_ext == null) ? " " : venta.justificacion_venta_ext;
                    }
                    else
                    {
                        venta.justificacion_venta_ext = null;
                    }

                    if (validPais && venta.VentaPais != null)
                    {
                        pais.Add(venta.Id);
                        if (venta.justificacion_venta_pais == null)
                        {
                            esJustificado = false;
                        }
                        venta.justificacion_venta_pais = (venta.justificacion_venta_pais == null) ? " " : venta.justificacion_venta_pais;
                    }
                    else
                    {
                        venta.justificacion_venta_pais = null;
                    }

                    valor.VentaExtranjero = venta.VentaExtranjero;
                    valor.VentaPais = venta.VentaPais;
                    valor.justificacion_venta_ext = venta.justificacion_venta_ext;
                    valor.justificacion_venta_pais = venta.justificacion_venta_pais;

                    manager.VentasPaisExtranjeroManager.Modify(valor);
                    manager.VentasPaisExtranjeroManager.SaveChanges();
                }
            }
            if (errors.Count > 0)
            {
                var res = new
                {
                    Pais = new List<long>(),
                    Extranjero = new List<long>(),
                    Success = false,
                    Errors = errors
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            if ((ext.Count > 0 || pais.Count > 0) && esJustificado == false)
            {
                var res = new
                {
                    Pais = pais,
                    Extranjero = ext,
                    Success = false,
                    Errors = new List<string>() { "Debe proporcionar una justificación para los valores de ventas en el pais y ventas en el extranjero." }
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            var result = new
            {
                Success = true,
                Pais = pais,
                Extranjero = ext,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDorpDownLineaProducto(string id, long idCiiu = 0, string nombre = "IdLineaProducto", string @default = null)
        {
            var list = Manager.LineaProducto.Get(t => t.Activado
                && t.MateriaTercero.All(h => h.VolumenProduccion.Encuesta.Id != IdEncuestaEstadistica)
                && t.Ciiu.Establecimientos.Any(h => h.Id == IdEstablecimiento)
                && t.IdCiiu == idCiiu && t.LineasProductoEstablecimiento.All(h => h.IdEstablecimiento != IdEstablecimiento))
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

        public ActionResult GetDorpDownLineaProductoNotIncluded(string id, long idCiiu = 0, string nombre = "IdLineaProducto", string @default = null)
        {
            var list = Manager.LineaProducto.Get(t => t.Activado && t.LineasProductoUnidadMedida.Any()
                && t.MateriaTercero.All(h => h.VolumenProduccion.Encuesta.Id != IdEncuestaEstadistica)
                /*&& t.Ciiu.Establecimientos.Any(h=>h.Id==IdEstablecimiento*/
                && t.IdCiiu == idCiiu && t.LineasProductoEstablecimiento.All(h => h.IdEstablecimiento != IdEstablecimiento))
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

        public ActionResult GetDorpDownLineaProductop(string id, long idCiiu = 0, string nombre = "IdLineaProducto", string @default = null)
        {
            var notInclude = Manager.LineaProductoEstablecimiento.Get("LineaProducto", h => h.IdEstablecimiento == IdEstablecimiento);

            var list = Manager.LineaProducto.Get("LineasProductoUnidadMedida", t => t.Activado && t.LineasProductoUnidadMedida.Any()
                && !(notInclude.Any(p2 => p2.IdLineaProducto == t.Id) || notInclude.Any(p2 => p2.LineaProducto.Codigo.Substring(0, 7) == t.Codigo))
                && t.Codigo.Length == 7
                && t.IdCiiu == idCiiu).Select(t => new SelectListItem()
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

        public ActionResult GetDorpDownLineaProductoS(string id, string filter, string nombre = "IdLineaProducto", string @default = null, bool mt = false)
        {
            var notIncludeMT = Manager.MateriaTercerosManager.Get("LineaProducto", h => h.IdVolumenProduccion == IdEncuestaEstadistica);
            var notInclude = Manager.LineaProductoEstablecimiento.Get("LineaProducto", h => h.IdEstablecimiento == IdEstablecimiento);

            var list = Manager.LineaProducto.Get("LineasProductoUnidadMedida", t => t.Activado && t.LineasProductoUnidadMedida.Any()
                && ((mt == false && !(notInclude.Any(p2 => p2.IdLineaProducto == t.Id) || notInclude.Any(p2 => p2.LineaProducto.Codigo.Substring(0, 7) == t.Codigo)))
                || (mt == true && !(notIncludeMT.Any(p => p.IdLineaProducto == t.Id) || notIncludeMT.Any(p2 => p2.LineaProducto.Codigo.Substring(0, 7) == t.Codigo))))
                && t.Nombre.ToUpper().Contains(filter.ToUpper())
                && t.Codigo.Length == 7).Select(t => new SelectListItem()
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

        public JsonResult GetCiiuTextByProducto(string idProducto)
        {
            var ciiu = Manager.LineaProducto.Get(t => t.Id.ToString() == idProducto).First().Ciiu;
            return Json(new
            {
                Ciiu = ciiu.Nombre,
                Codigo = ciiu.Codigo,
                IdCiiu = ciiu.Id
            }
                );
        }

        public ActionResult GetDorpDownCiiu(string id = "0", string nombre = "IdCiiu", string @default = null)
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

        public ActionResult GetDorpDownAllCiiu(string id = "0", string nombre = "IdCiiu", string @default = null)
        {
            var list = Manager.Ciiu.Get(t => t.Activado
                /* && t.Establecimientos.Any(h => h.Id == IdEstablecimiento)*/)
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

        public ActionResult NuevaMateriaTerceros(long id = 0)
        {
            var mat = Manager.MateriaTercerosManager.Find(id);
            mat = mat ?? new MateriaTerceros();
            return PartialView("_NuevaMateriaTerceros", mat);
        }

        public ActionResult NuevaMateriaPropia(long id = 0)
        {
            var mat = Manager.MateriaPropiaManager.Find(id);
            mat = mat ?? new MateriaPropia();
            return PartialView("_NuevaMateriaPropia", mat);
        }

        [HttpPost]
        public JsonResult NuevaMateriaTerceros(MateriaTerceros materia)
        {
            var encuesta = Manager.EncuestaEstadistica.FindById(IdEncuestaEstadistica);
            materia.IdVolumenProduccion = encuesta.VolumenProduccionMensual.Identificador;
            var manager = Manager;
            var valid = !Manager.MateriaTercerosManager.ValidarProduccion(encuesta.Id, materia.IdLineaProducto,
                   decimal.Parse(materia.UnidadProduccion));
            if (valid && (string.IsNullOrEmpty(materia.justificacion_produccion) || string.IsNullOrWhiteSpace(materia.justificacion_produccion)))
            {
                var res = new
                {
                    Success = false,
                    Invalid = true,
                    Errors = new List<string>() { "Debe justificar el valor de producción" }
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            if (!valid)
            {
                materia.justificacion_produccion = null;
            }
            var op = materia.Id == 0
                ? manager.MateriaTercerosManager.Add(materia)
                : manager.MateriaTercerosManager.Modify(materia);
            if (op.Success)
            {
                manager.MateriaTercerosManager.SaveChanges();
                manager.ValorProduccionManager.Generate(IdEncuestaEstadistica);
                var encuestaSuccess = Manager.EncuestaEstadistica.FindById(IdEncuestaEstadistica);
                var list = encuestaSuccess.VolumenProduccionMensual.MateriasTercero;
                var c = this.RenderRazorViewToString("_TableProductoMateriaPrimaTerceros", list);
                var d = this.RenderRazorViewToString("_ValorProduccionEstablecimiento", encuestaSuccess);
                var e = this.RenderRazorViewToString("_ValorVentasProductosEstablecimiento", encuestaSuccess);

                var res = new
                {
                    Success = true,
                    Data = c,
                    ValorProduccion = d,
                    Ventas = e,
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var res = new
                {
                    Success = false,
                    Errors = op.Errors
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult NuevaMateriaPropia(MateriaPropia materia)
        {
            //adicionar linea de producto al establecimiento
            var encuesta = Manager.EncuestaEstadistica.FindById(IdEncuestaEstadistica);
            materia.IdVolumenProduccion = encuesta.VolumenProduccionMensual.Identificador;
            var manager = Manager;
            var op = materia.Id == 0
                ? manager.MateriaPropiaManager.Add(materia)
                : manager.MateriaPropiaManager.Modify(materia);
            var lin = Manager.LineaProducto.Find(materia.IdLineaProducto);
            if (op.Success && lin != null)
            {
                manager.EncuestaEstadistica.AddLineaProducto(IdEncuestaEstadistica, new LineaProducto() { IdCiiu = lin.IdCiiu, Id = materia.IdLineaProducto }, false);
                manager.MateriaPropiaManager.SaveChanges();
                manager.ValorProduccionManager.Generate(IdEncuestaEstadistica);
                var encuestaSuccess = Manager.EncuestaEstadistica.FindById(IdEncuestaEstadistica);
                var list = encuestaSuccess.VolumenProduccionMensual.MateriasPropia;
                var c = this.RenderRazorViewToString("_TableProductoMateriaPrimaPropia", list);
                var d = this.RenderRazorViewToString("_ValorProduccionEstablecimiento", encuestaSuccess);
                var e = this.RenderRazorViewToString("_ValorVentasProductosEstablecimiento", encuestaSuccess);
                var res = new
                {
                    Success = true,
                    Data = c,
                    ValorProduccion = d,
                    Ventas = e
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var res = new
                {
                    Success = false,
                    Errors = op.Errors
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AsignarNuevaLineaProducto(LineaProducto linea)
        {
            var manager = Manager;
            manager.EncuestaEstadistica.AddLineaProducto(IdEncuestaEstadistica, linea);
            var op = new List<string>();
            if (linea == null)
                op.Add("Datos Erroneos");
            if (linea != null && linea.Id < 1)
                op.Add("Debe seleccionar una Línea de Producto");
            if (linea != null && linea.IdCiiu < 1)
                op.Add("Debe seleccionar un CIIU");
            if (!op.Any())
            {
                manager.MateriaPropiaManager.SaveChanges();
                manager.ValorProduccionManager.Generate(IdEncuestaEstadistica);
                var encuesta = Manager.EncuestaEstadistica.FindById(IdEncuestaEstadistica);
                var list = encuesta.VolumenProduccionMensual.MateriasPropia;
                var c = this.RenderRazorViewToString("_TableProductoMateriaPrimaPropia", list);
                var d = this.RenderRazorViewToString("_ValorProduccionEstablecimiento", encuesta);
                var e = this.RenderRazorViewToString("_ValorVentasProductosEstablecimiento", encuesta);
                var res = new
                {
                    Success = true,
                    Data = c,
                    ValorProduccion = d,
                    Ventas = e
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var res = new
                {
                    Success = false,
                    Errors = op
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult EliminarMateriaTerceros(long id)
        {
            var manager = Manager;
            var op = manager.MateriaTercerosManager.Delete(id);
            if (op.Success)
            {
                manager.MateriaTercerosManager.SaveChanges();
                manager.ValorProduccionManager.Generate(IdEncuestaEstadistica);
                var encuesta = Manager.EncuestaEstadistica.FindById(IdEncuestaEstadistica);
                var list = encuesta.VolumenProduccionMensual.MateriasTercero;
                var c = this.RenderRazorViewToString("_TableProductoMateriaPrimaTerceros", list);
                var d = this.RenderRazorViewToString("_ValorProduccionEstablecimiento", encuesta);
                var e = this.RenderRazorViewToString("_ValorVentasProductosEstablecimiento", encuesta);
                var res = new
                {
                    Success = true,
                    Data = c,
                    ValorProduccion = d,
                    Ventas = e
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var res = new
                {
                    Success = false,
                    Errors = op.Errors
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetDorpDownUnidadMedida(long idLineaProducto, string id, string nombre = "IdUnidadMedida", string @default = null)
        {
            var include = Manager.LineaProductoUnidadMedidaManager.Get(h => h.id_linea_producto == idLineaProducto);

            var list = Manager.UnidadMedida.Get(t =>
                    include.Any(p2 => p2.id_unidad_medida == t.Id)).Select(t => new SelectListItem()
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

        public ActionResult FillFactoresIncremento()
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(IdEncuestaEstadistica);
            if (encuesta == null) return HttpNotFound("Encuesta no encontrada");
            var factores = encuesta.FactorProduccion.CAT_FACTOR1.Select(t => t.Id);
            encuesta.FactorProduccion.IncrementoB = true;
            manager.EncuestaEstadistica.Modify(encuesta);
            manager.EncuestaEstadistica.SaveChanges();
            var allFactores = Manager.Factor.Get(t => t.Activado);
            var incremento = new List<SelectListItem>();
            var enumerable = factores as IList<long> ?? factores.ToList();
            foreach (var f in allFactores.Where(t => t.TipoFactor == TipoFactor.Incremento))
            {
                incremento.Add(new SelectListItem()
                {
                    Text = f.Nombre,
                    Value = f.Id.ToString(),
                    Selected = enumerable.Any(t => t == f.Id)
                });
            }

            incremento.Add(new SelectListItem()
            {
                Text = "Otros",
                Value = "0",
                Selected = string.IsNullOrEmpty(encuesta.FactorProduccion.OtroFactor) ? false : true
            });

            ViewBag.OtroFactor = encuesta.FactorProduccion.OtroFactor;

            return PartialView("_FactoresIncremento", incremento);
        }

        public ActionResult FillFactoresIncrementoReadOnly()
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(IdEncuestaEstadistica);
            if (encuesta == null) return HttpNotFound("Encuesta no encontrada");
            var factores = encuesta.FactorProduccion.CAT_FACTOR1.Select(t => t.Id);
            encuesta.FactorProduccion.IncrementoB = true;
            manager.EncuestaEstadistica.Modify(encuesta);
            manager.EncuestaEstadistica.SaveChanges();
            var allFactores = Manager.Factor.Get(t => t.Activado);
            var incremento = new List<SelectListItem>();
            var enumerable = factores as IList<long> ?? factores.ToList();
            foreach (var f in allFactores.Where(t => t.TipoFactor == TipoFactor.Incremento))
            {
                incremento.Add(new SelectListItem()
                {
                    Text = f.Nombre,
                    Value = f.Id.ToString(),
                    Selected = enumerable.Any(t => t == f.Id)
                });
            }

            ViewBag.OtroFactor = encuesta.FactorProduccion.OtroFactor;

            incremento.Add(new SelectListItem()
            {
                Text = "Otros",
                Value = "0",
                Selected = string.IsNullOrEmpty(encuesta.FactorProduccion.OtroFactor) ? false : true
            });
            return PartialView("_FactoresIncrementoReadOnly", incremento);
        }

        public ActionResult FillFactoresDecrecimiento()
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(IdEncuestaEstadistica);
            if (encuesta == null) return HttpNotFound("Encuesta no encontrada");
            var factores = encuesta.FactorProduccion.CAT_FACTOR1.Select(t => t.Id);
            var allFactores = Manager.Factor.Get(t => t.Activado);
            var disminucion = new List<SelectListItem>();
            var enumerable = factores as IList<long> ?? factores.ToList();
            encuesta.FactorProduccion.DecrecimientoB = true;
            manager.EncuestaEstadistica.Modify(encuesta);
            manager.EncuestaEstadistica.SaveChanges();
            foreach (var f in allFactores.Where(t => t.TipoFactor == TipoFactor.Disminución))
            {
                disminucion.Add(new SelectListItem()
                {
                    Text = f.Nombre,
                    Value = f.Id.ToString(),
                    Selected = enumerable.Any(t => t == f.Id)
                });
            }

            ViewBag.OtroFactor = encuesta.FactorProduccion.OtroFactor;

            disminucion.Add(new SelectListItem()
            {
                Text = "Otros",
                Value = "0",
                Selected = string.IsNullOrEmpty(encuesta.FactorProduccion.OtroFactor) ? false : true
            });
            return PartialView("_FactoresIncremento", disminucion);
        }

        public ActionResult FillFactoresDecrecimientoReadOnly()
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(IdEncuestaEstadistica);
            if (encuesta == null) return HttpNotFound("Encuesta no encontrada");
            var factores = encuesta.FactorProduccion.CAT_FACTOR1.Select(t => t.Id);
            var allFactores = Manager.Factor.Get(t => t.Activado);
            var disminucion = new List<SelectListItem>();
            var enumerable = factores as IList<long> ?? factores.ToList();
            encuesta.FactorProduccion.DecrecimientoB = true;
            manager.EncuestaEstadistica.Modify(encuesta);
            manager.EncuestaEstadistica.SaveChanges();
            foreach (var f in allFactores.Where(t => t.TipoFactor == TipoFactor.Disminución))
            {
                disminucion.Add(new SelectListItem()
                {
                    Text = f.Nombre,
                    Value = f.Id.ToString(),
                    Selected = enumerable.Any(t => t == f.Id)
                });
            }

            ViewBag.OtroFactor = encuesta.FactorProduccion.OtroFactor;

            disminucion.Add(new SelectListItem()
            {
                Text = "Otros",
                Value = "0",
                Selected = string.IsNullOrEmpty(encuesta.FactorProduccion.OtroFactor) ? false : true
            });
            return PartialView("_FactoresIncrementoReadOnly", disminucion);
        }

        public void ProduccionNormal()
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(IdEncuestaEstadistica);
            if (encuesta == null) return;
            encuesta.FactorProduccion.ProduccionNormalB = true;
            manager.EncuestaEstadistica.Modify(encuesta);
            manager.EncuestaEstadistica.SaveChanges();
        }

        public void SetService(bool value)
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(IdEncuestaEstadistica);
            if (encuesta == null) return;
            encuesta.VentasProductosEstablecimiento.ServiciosActivados = value;
            manager.VentasProductosEstablecimientoManager.Modify(encuesta.VentasProductosEstablecimiento);
            manager.VentasProductosEstablecimientoManager.SaveChanges();

            if (value == false)
            {
                foreach (var venta in manager.VentaServicioManufacturaManager.Get(t => t.IdVentaProductoestablecimiento == encuesta.Id))
                {
                    venta.venta = null;
                    venta.venta_extranjero = null;
                    venta.justificacion_venta_pais = null;
                    venta.justificacion_venta_ext = null;
                    manager.VentaServicioManufacturaManager.Modify(venta);
                }

                manager.VentaServicioManufacturaManager.SaveChanges();
            }
        }

        public JsonResult SaveTrabajadoresDiasTrabajados(int? diasTrabajados, int? trabajadoresProduccion,
             int? administrativoPlanta)
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(IdEncuestaEstadistica);
            if (encuesta == null)
            {
                var res = new
                {
                    Success = false
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            };
            var errors = new List<string>();
            if (trabajadoresProduccion == null)
            {
                errors.Add("Debe introducir la cantidad de trabajadores en producción");
            }
            else if (trabajadoresProduccion < 1)
            {
                errors.Add("La cantidad de trabajadores en producción debe ser mayor que cero");
            }

            if (administrativoPlanta == null)
            {
                errors.Add("Debe introducir los administrativos en planta");
            }
            else
                if (administrativoPlanta < 1)
            {
                errors.Add("La cantidad de administrativos en planta debe ser mayor que cero");
            }
            var count = DateTime.DaysInMonth(encuesta.Fecha.Year, encuesta.Fecha.Month);
            if (diasTrabajados == null)
            {
                errors.Add("Debe introducir la cantidad de días trabajados");
            }
            else
                if (diasTrabajados < 1)
            {
                errors.Add("La cantidad de días trabajados debe ser mayor que cero");
            }
            if (diasTrabajados > count)
            {
                errors.Add("La cantidad de días trabajados debe ser menor que " + count);
            }
            if (errors.Count > 0)
            {
                var res = new
                {
                    Success = false,
                    Errors = errors
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            encuesta.TrabajadoresDiasTrabajados.TrabajadoresProduccion = trabajadoresProduccion;
            encuesta.TrabajadoresDiasTrabajados.DiasTrabajados = diasTrabajados;
            encuesta.TrabajadoresDiasTrabajados.AdministrativosPlanta = administrativoPlanta;
            manager.TrabajadoresDiasTrabajadosManager.Modify(encuesta.TrabajadoresDiasTrabajados);
            manager.TrabajadoresDiasTrabajadosManager.SaveChanges();
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveServicio(List<VentasServicioManufactura> services)
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(IdEncuestaEstadistica);
            var errors = new List<string>();
            bool esJustificado = true;
            if (encuesta == null)
            {
                var res = new
                {
                    Success = false,
                    Errors = new List<string>() { "No se pudo encontrar la encuesta" }
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            };
            var pais = new List<long>();
            var ext = new List<long>();
            if (encuesta.VentasProductosEstablecimiento.ServiciosActivados)
            {
                var any = false;
                foreach (var it in services)
                {
                    if (it.venta != null)
                    {
                        any = true;
                        break;
                    }
                }
                if (!any)
                    errors.Add("Usted señaló que el establecimiento brindó servicios a terceros, debe llenar al menos uno.");
            }
            foreach (var se in services)
            {
                var servicio =
               manager.VentaServicioManufacturaManager.Find(se.Id);
                if (servicio != null)
                {

                    servicio.venta_extranjero = se.venta_extranjero;
                    servicio.venta = se.venta;

                    var validPais = !manager.VentaServicioManufacturaManager.ValidarVentaPais(se.Id, se.venta);
                    var validExt = !manager.VentaServicioManufacturaManager.ValidarVentaExtranjero(se.Id, se.venta_extranjero);

                    if (validExt && se.venta_extranjero != null)
                    {
                        ext.Add(se.Id);
                        if (se.justificacion_venta_ext == null)
                        {
                            esJustificado = false;
                        }
                        se.justificacion_venta_ext = (se.justificacion_venta_ext == null) ? " " : se.justificacion_venta_ext;
                    }
                    else
                    {
                        se.justificacion_venta_ext = null;
                    }

                    if (validPais && se.venta != null)
                    {
                        pais.Add(se.Id);
                        if (se.justificacion_venta_pais == null)
                        {
                            esJustificado = false;
                        }
                        se.justificacion_venta_pais = (se.justificacion_venta_pais == null) ? " " : se.justificacion_venta_pais;
                    }
                    else
                    {
                        se.justificacion_venta_pais = null;
                    }

                    servicio.justificacion_venta_ext = se.justificacion_venta_ext;
                    servicio.justificacion_venta_pais = se.justificacion_venta_pais;

                    manager.VentaServicioManufacturaManager.Modify(servicio);
                    manager.VentaServicioManufacturaManager.SaveChanges();
                }
            }
            if ((pais.Count > 0 || ext.Count > 0) && esJustificado == false)
            {
                var te = new List<string>()
                {
                    "Debe justificar los valores de ventas en el pais y ventas en el extranjero"
                };
                te.AddRange(errors);
                var re = new
                {
                    Success = false,
                    Errors = te,
                    Pais = pais,
                    Extranjero = ext
                };
                return Json(re, JsonRequestBehavior.AllowGet);
            }
            if (errors.Any())
            {
                var re = new
                {
                    Success = false,
                    Errors = errors,
                    Pais = new List<long>(),
                    Extranjero = new List<long>()
                };
                return Json(re, JsonRequestBehavior.AllowGet);
            }
            var resp = new
            {
                Success = true,
                Pais = pais,
                Extranjero = ext
            };
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveInformante(EncuestaEstadistica encuesta)
        {
            var list = Manager.EncuestaEstadistica.SalvarContacto(encuesta);

            if (list.Count == 0)
            {
                var res = new
                {
                    Success = true,

                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            var resp = new
            {
                Success = false,
                Errors = list
            };
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public void SetFactorProduccion(long id, bool put)
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(IdEncuestaEstadistica);
            if (encuesta == null)
                return;

            if (id != 0)
            {
                var factor = manager.Factor.Find(id);
                if (factor == null)
                    return;

                if (put)
                {
                    encuesta.FactorProduccion.CAT_FACTOR1.Add(factor);
                    manager.EncuestaEstadistica.SaveChanges();
                }
                else
                {
                    var fr = encuesta.FactorProduccion.CAT_FACTOR1.FirstOrDefault(t => t.Id == id);
                    if (fr != null)
                    {
                        encuesta.FactorProduccion.CAT_FACTOR1.Remove(fr);
                        manager.FactorProducccionManager.SaveChanges();
                    }
                }
            }
        }

        [HttpPost]
        public void SetOtroFactorProduccion(bool put, string otroFactor)
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.FindById(IdEncuestaEstadistica);

            if (put)
            {
                encuesta.FactorProduccion.OtroFactor = otroFactor;
            }
            else
            {
                encuesta.FactorProduccion.OtroFactor = "";
            }

            manager.FactorProducccionManager.Modify(encuesta.FactorProduccion);
            manager.FactorProducccionManager.SaveChanges();
        }

        public ActionResult GetHistoricoProduccion(long id)
        {
            var historico = Manager.MateriaPropiaManager.GetHistoryProduccion(id);
            return PartialView("_TableNumbers", historico);
        }

        public ActionResult GetHistoricoValorUnitario(long id)
        {
            var historico = Manager.MateriaPropiaManager.GetHistoryValorUnitario(id);
            return PartialView("_TableNumbers", historico);
        }

        public ActionResult GetHistoricoVentasPais(long id)
        {
            var historico = Manager.MateriaPropiaManager.GetHistoryVentasPais(id);
            return PartialView("_TableNumbers", historico);
        }

        public ActionResult GetHistoricoVentasExtranjero(long id)
        {
            var historico = Manager.MateriaPropiaManager.GetHistoryVentasExtranjero(id);
            return PartialView("_TableNumbers", historico);
        }

        public ActionResult GetHistoricoProduccionTerceros(long id)
        {
            var historico = Manager.MateriaTercerosManager.GetHistoryProduccion(id);
            return PartialView("_TableNumbers", historico);
        }

        public ActionResult GetHistoricoMateriaPrimaTerceros(long id)
        {
            var historico = Manager.ValorProduccionManager.GetHistoryMateriaPrimaTerceros(id);
            return PartialView("_TableNumbers", historico);
        }

        public ActionResult GetHistoricoValorVentasVentasPais(long id)
        {
            var historico = Manager.VentasPaisExtranjeroManager.GetHistoryVentasPais(id);
            return PartialView("_TableNumbers", historico);
        }

        public ActionResult GetHistoricoValorVentasVentasExtranjero(long id)
        {
            var historico = Manager.VentasPaisExtranjeroManager.GetHistoryVentasExtranjero(id);
            return PartialView("_TableNumbers", historico);
        }

        public ActionResult GetHistoricoValorServicioVentasPais(long id)
        {
            var historico = Manager.VentaServicioManufacturaManager.GetHistoryVentasPais(id);
            return PartialView("_TableNumbers", historico);
        }

        public ActionResult GetHistoricoValorServicioVentasExtranjero(long id)
        {
            var historico = Manager.VentaServicioManufacturaManager.GetHistoryVentasExtranjero(id);
            return PartialView("_TableNumbers", historico);
        }

        
    }
}

