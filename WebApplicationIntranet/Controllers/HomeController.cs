﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Domain.Managers;
using Entity;
using WebApplication;
using WebApplication.Models;
using Seguridad.PRODUCE;
//using Data.UsuarioIntranetService;


namespace WebApplication.Controllers
{
    
    //[Authorize]
    //[Autorizacion]
    public class HomeController : Controller
    {
        public Manager Manager {
            get { return Tools.GetManager(); }
        }
        public ActionResult Index(UserInformation user)
        {

            user = new UserInformation()
            {
                Id = 2,
                Tipo = "Tipo",
                Nombre = "Nombre",
                Login = "administrativo",
                Ndocumento = "0012345",
                Empresa = "Empresa",
                Aplicaciones = new List<Aplicaciones>()
                {
                    new Aplicaciones()
                    {
                        A = 123,
                        R = "Administrativo"
                    }
                }

            };

            Session["uid"] = user.Id;
            
            string identificador = user.Aplicaciones.First().A.ToString();
            string rol = user.Aplicaciones.First().R;
            ViewBag.Identificador = identificador;
            ViewBag.Rol = rol;
            Session["pr"] = rol;
            ViewBag.Empresa = user.Empresa;
            ViewBag.Id = user.Id;
            ViewBag.Login = user.Login;
            ViewBag.Ndocumento = user.Ndocumento;
            ViewBag.Nombre = user.Nombre;
            ViewBag.Tipo = user.Tipo;
            ViewBag.IdentityName = this.User.Identity.Name;

            //Manager.Usuario.AutenticateIntranetPRODUCE(user.Id.ToString(), user.Login, rol, user.Nombre);           

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Credenciales model)
        {

            if (ModelState.IsValid)
            {
                //var user = Manager.Usuario.AutenticateIntranet(model.Login, model.Password);
                if (Membership.ValidateUser(model.Login,model.Password))
                {
                    var user = Manager.Usuario.Autenticate(model.Login, model.Password);
                    FormsAuthentication.SetAuthCookie(model.Login, false);
                    this.WriteMessage("Iniciando sesion", model.Login);
                    //ViewData.Add("user",Manager.Usuario.FindRol(user.Id));
                    if (user.Roles.Any(t => t.Nombre.Equals("Informante")))
                        return RedirectToAction("EstablecimientosEncuestaEmpresarial", "UsuarioExtranet");
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("Error", "Usuario o contraseña incorrecto.");
                return View("Index");
            }
            return View(model);

        }
        public ActionResult SignOut()
        {
            this.WriteMessage("Cerrando sesion");
            FormsAuthentication.SignOut();;
            return RedirectToAction("Index", "Home");
        }
    }
}