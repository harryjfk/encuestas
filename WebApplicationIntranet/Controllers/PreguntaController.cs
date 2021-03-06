﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Domain.Managers;
using Entity;

namespace WebApplication.Controllers
{
    public class PreguntaController : BaseController<Pregunta>
    {
        public ActionResult GetDorpDown(string id, string nombre = "IdPregunta", string @default = null)
        {
           var list =OwnManager.Get(t => t.Activado).Select(t => new SelectListItem()
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

        public ActionResult GetDorpDownAsociada(string id,List<long>noIncluir,  string nombre = "IdPregunta", string @default = null)
        {
            var list = OwnManager.Get(t => t.Activado && t.IdEncuestaEmpresarial==null && noIncluir.All(h=>h!=t.Id)).Select(t => new SelectListItem()
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

        public override ActionResult Index()
        {
            Query = Query ?? new Query<Pregunta>();
            Query = Query.Validate();
            Func<Pregunta, bool> nFilter = t => t.IdEncuestaEmpresarial == null;
            if (Query.Filter != null)
            {
                var oFilter = (Func<Pregunta, bool>) Query.Filter.Clone();
                Query.Filter = t => oFilter(t) && nFilter(t);
            }
            else
            {
                Query.Filter = nFilter;
            }
            OwnManager.Get(Query);
            ModelState.Clear();
            return View("Index", Query);
        }

        public ActionResult CreatePosibleRespuesta(long id)
        {
            var manager = OwnManager;
            var pregunta = manager.Find(id);
            if (pregunta == null)
            {
                ModelState.AddModelError("Error", "No se pudo encontrar el elemento.");
                return RedirectToAction("Index");
            }
            var posResp = pregunta.PosiblesRespuestas.FirstOrDefault();
            posResp = posResp ?? new PosibleRespuesta()
            {
                IdPregunta = pregunta.Id,
                Valores = new List<Valor>()
            };
            return PartialView("_CreatePosibleRespuesta", posResp);
        }

        [HttpPost]
        public ActionResult CreatePosibleRespuestaPost(PosibleRespuesta respuesta)
        {
            if (ModelState.IsValid)
            {
                var manager = Manager;
                var op = respuesta.Id == 0 ?
                    manager.PosibleRespuesta.Add(respuesta) :
                    manager.PosibleRespuesta.Modify(respuesta);
                if (op.Success)
                {
                    manager.PosibleRespuesta.SaveChanges();
                    var c = RenderRazorViewToString("_Table", Query);
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
    }
}
