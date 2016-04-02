using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Data.Contratos;
using Data.Repositorios;
using Domain.Managers;
using Entity;
using Microsoft.Owin;
using Ninject;
using Ninject.Modules;
using Owin;
using Seguridad;

[assembly: OwinStartupAttribute(typeof(WebApplication.Startup))]
namespace WebApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
    public class Bind : NinjectModule
    {
        public override void Load()
        {
            Bind<IRepositorioDepartamento>().To<RepositorioDepartamento>();
            Bind<IRepositorioProvincia>().To<RepositorioProvincia>();
            Bind<IRepositorioDistrito>().To<RepositorioDistrito>();
            Bind<IRepositorioUbigeo>().To<RepositorioUbigeo>();
            //brb
            Bind<IRepositorioUsuario>().To<RepositorioUsuarioLocal>();
            //endbrb
        }
    }
    public static class Tools
    {
        public static Manager GetManager(string usuario="")
        {
            IKernel kernel = new StandardKernel();
            kernel.Load(new Bind());
            var manager = kernel.Get<Manager>();
            manager.UsuarioAutenticado = usuario;
            return manager;
        }
        public static void WriteMessage(this Controller controller,string message,string user=null)
        {
            //var manager = new Manager();
            //if (controller.User.Identity.IsAuthenticated||user!=null)
            //{
            //    manager.Traza.Add(new Traza()
            //    {
            //        Fecha = DateTime.Now,
            //        Texto = message,
            //        Usuario = user??controller.User.Identity.Name,
            //        Tipo = TipoTraza.Informacion
            //    });
            //    manager.Traza.SaveChanges();
            //}
        }

        public static Usuario GetLogued(this Controller controller)
        {
            var login = controller.User.Identity.Name;
            return GetManager().Usuario.Get(t => t.Login == login).FirstOrDefault();
        }
        public static string RenderRazorViewToString( this Controller controller,string viewName, object model)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
