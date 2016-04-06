using System;
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
using Microsoft.AspNet.Identity;

namespace WebApplication.Controllers
{
    /*[Authorize]
    [Autorizacion]*/
    public class HomeController : Controller
    {
        public Manager Manager {
            get { return Tools.GetManager(); }
        }

        public ActionResult Index(UserInformation user)
        {
            // Acá se agrega un fix. Como el método de autenticación tiene la cookie activada va a recuperarla siempre
            // cuando se vuelva a ingresar al sitio. Por lo cual la sesión se perdió pero el usuario está autenticado
            string currentLoginUser = string.Empty;
            if (Request.IsAuthenticated)
            {
                if (Session["login"] == null)
                {
                    // Indica que la sesión se perdió pero está autenticado. Recuperar el usuario
                    currentLoginUser = User.Identity.GetUserName();
                    Session["login"] = currentLoginUser;
                }
                else
                {
                    currentLoginUser = Session["login"].ToString();
                }
            }

            var usuario = Manager.Usuario.Repositorio.GetUsuariosIntranet(null, t => t.Login == currentLoginUser);
            if (usuario.Count > 0)
            {
                user = new UserInformation()
                {
                    Id = Convert.ToInt32(usuario.First().Identificador),
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
                            R = usuario.First().Roles.First().Nombre
                        }
                    }

                };

                Session["uid"] = user.Id;
                Session["pr"] = user.Aplicaciones.First().R;
            }
            else
            {
                Session["uid"] = 0;
                Session["pr"] = "";
            }

            //brb
            //Session["uid"] = user.Id;

            //string identificador = user.Aplicaciones.First().A.ToString();
            //string rol = "";

            //foreach (var item in user.Aplicaciones)
            //{
            //    if (item.R == "Analista" || item.R == "Administrador")
            //    {
            //        rol = item.R;
            //        break;
            //    }
            //}

            //ViewBag.Identificador = identificador;
            //ViewBag.Rol = rol;
            //Session["pr"] = rol;
            //ViewBag.Empresa = user.Empresa;
            //ViewBag.Id = user.Id;
            //ViewBag.Login = user.Login;
            //ViewBag.Ndocumento = user.Ndocumento;
            //ViewBag.Nombre = user.Nombre;
            //ViewBag.Tipo = user.Tipo;
            //ViewBag.IdentityName = this.User.Identity.Name;
            //Manager.Usuario.AutenticateIntranetPRODUCE(user.Id.ToString(), user.Login, rol, user.Nombre);
            //endbrb

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
            var a = Manager.Establecimiento.Get();

            if (ModelState.IsValid)
            {
                //var user = Manager.Usuario.AutenticateIntranet(model.Login, model.Password);
                if (Membership.ValidateUser(model.Login, model.Password))
                {
                    var user = Manager.Usuario.Autenticate(model.Login, model.Password);
                    
                    FormsAuthentication.SetAuthCookie(model.Login, false);
                    Session["login"] = user.Login;
                    //this.WriteMessage("Iniciando sesion", model.Login);
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
            //this.WriteMessage("Cerrando sesion");
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}