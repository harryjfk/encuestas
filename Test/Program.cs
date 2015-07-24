using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Data.Contratos;
using Data.Repositorios;
using Domain.Managers;
using Entity;

using Ninject.Modules;
using Ninject;


namespace Test
{

    class Bind : NinjectModule
    {
        public override void Load()
        {
            Bind<IRepositorioDepartamento>().To<RepositorioDepartamento>();
            Bind<IRepositorioProvincia>().To<RepositorioProvincia>();
            Bind<IRepositorioDistrito>().To<RepositorioDistrito>();
            Bind<IRepositorioUbigeo>().To<RepositorioUbigeo>();
            Bind<IRepositorioUsuario>().To<RepositorioUsuario>();
        }
    }
    class Program
    {
        //static IQueryable<T> Get<T>( IEnumerable<T> elements,string text)
        //{

        //}
        static void Main(string[] args)
        {
            Test.Update += Test_Update;
            var r="hola lola".Calculate();
            var r2 = "hola lola".Calculate();
            var r3 = "hola lola".Calculate();
            var match = System.Text.RegularExpressions.Regex.Match("IdPepe", "Id(.+)");
            var ok = match.Groups[1].Success;
            var m = match.Groups[1].Value;
            var bytes = Encoding.UTF8.GetBytes("{'type':45,'count':45}");

           var reder= JsonReaderWriterFactory.CreateJsonReader(bytes,new XmlDictionaryReaderQuotas(){});
            for (int i = 0; i < reder.AttributeCount; i++)
            {
                var attribute = reder.GetAttribute(i);
                //var value=reder.Read()
            }

            IKernel kernel = new StandardKernel();
            kernel.Load(new Bind());
            var manager = kernel.Get<Manager>();
            manager.Seed();
            var ciiu = manager.GetManager<Entity.Ciiu>();
            //var list = ciiu.Get(null, new Paginacion()
            // {
            //     ItemsPerPage = 10,
            //     Page = 1
            // }, new Order<string>()
            // {
            //     Property = "nombre"
            // }).ToList();
            var tsr = manager.Ubigeo.Get(new Paginacion() { Page = 1, ItemsPerPage = 2 });
            //var manager = new Manager(null);
            var extra = manager.Usuario.GetUsuariosExtranet();
            var intra = manager.Usuario.GetUsuariosIntranet();
            var all = manager.Departamento.Get();


        }

        static void Test_Update(int x)
        {
            
        }

        




    }
}
