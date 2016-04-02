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
using Domain;

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
            Console.WriteLine("Iniciando...");
            // Test.Update += Test_Update;
            // var r="hola lola".Calculate();
            // var r2 = "hola lola".Calculate();
            // var r3 = "hola lola".Calculate();
            // var match = System.Text.RegularExpressions.Regex.Match("IdPepe", "Id(.+)");
            // var ok = match.Groups[1].Success;
            // var m = match.Groups[1].Value;
            // var bytes = Encoding.UTF8.GetBytes("{'type':45,'count':45}");

            //var reder= JsonReaderWriterFactory.CreateJsonReader(bytes,new XmlDictionaryReaderQuotas(){});
            // for (int i = 0; i < reder.AttributeCount; i++)
            // {
            //     var attribute = reder.GetAttribute(i);
            //     //var value=reder.Read()
            // }            
            IKernel kernel = new StandardKernel();
            kernel.Load(new Bind());
            var manager = kernel.Get<Manager>();
            manager.Seed();
            Console.WriteLine("Cargado");
            //var ciiu = manager.GetManager<Entity.Ciiu>();
            ////var list = ciiu.Get(null, new Paginacion()
            //// {
            ////     ItemsPerPage = 10,
            ////     Page = 1
            //// }, new Order<string>()
            //// {
            ////     Property = "nombre"
            //// }).ToList();
            //var tsr = manager.Ubigeo.Get(new Paginacion() { Page = 1, ItemsPerPage = 2 });
            ////var manager = new Manager(null);
            //var extra = manager.Usuario.GetUsuariosExtranet();
            //var intra = manager.Usuario.GetUsuariosIntranet();
            //var all = manager.Departamento.Get();
            Console.WriteLine("Ejecutando...");

            //Query<UsuarioExtranet> QueryAdministrador = new Query<UsuarioExtranet>();
            //QueryAdministrador = QueryAdministrador.Validate();
            //QueryAdministrador.Criteria = new UsuarioExtranet();            
            //QueryAdministrador.Paginacion = QueryAdministrador.Paginacion ?? new Paginacion();
            //QueryAdministrador.BuildFilter();

            //manager.Usuario.GetUsuariosExtranet(QueryAdministrador);

            //Console.WriteLine(QueryAdministrador.Elements.Count());
            //Console.WriteLine(manager.Establecimiento.Get(t => t.Id == 1).FirstOrDefault().Ciius.FirstOrDefault().Ciiu.ToString());
            string xd = "";
            //xd = xd + "--"  + manager.ReporteManager.GetIVF(2016)[0].Fecha.ToString("dd/MM/yyy");
            //xd = xd + "--" + manager.ReporteManager.GetIVF(2016)[0].IdCiiu.ToString();
            //xd = xd + "--" + manager.ReporteManager.GetIVF(2016)[0].IVF.ToString();
            Console.WriteLine(xd);
            Console.WriteLine("Ejecutado...");
            //FORMATO PRODUCE
            //Console.WriteLine(string.Format("{0:0,0.0000}", 20000999.23).Replace(",", " ").Replace(".", ","));
            //var establecimiento = manager.Establecimiento.Get(t => t.Id == 56).FirstOrDefault();
            //var encuesta = manager.EncuestaEstadistica.FindById(93);

            //var old = manager.EncuestaEstadistica.Get(t => t.IdEstablecimiento == establecimiento.Id).Where(t => t.Fecha < encuesta.Fecha).OrderBy(t => t.Fecha).ToList(); 

            //foreach (var lp in establecimiento.LineasProductoEstablecimiento)
            //{
            //    var um = lp.LineaProducto.LineasProductoUnidadMedida.FirstOrDefault();
            //    if (um != null)
            //    {
            //        var mp = new MateriaPropia()
            //        {
            //            IdLineaProducto = lp.LineaProducto.Id,
            //            IdUnidadMedida = um.id_unidad_medida.GetValueOrDefault(),                        
            //            JustificacionValorUnitario = null,
            //            JustificacionProduccion = null,
            //            justificacion_venta_pais = null,
            //            justificacion_venta_extranjero = null
            //        };                    
            //                                                                                                                                                                      // mp.IsFirst = true;
            //        if (old.Count > 0)
            //        {
            //            var lastEnc = old.LastOrDefault();                        
            //            if (lastEnc != null)
            //            {
            //                var materiaPropia =
            //                    lastEnc.VolumenProduccionMensual.MateriasPropia.FirstOrDefault(
            //                        t => t.IdLineaProducto == mp.IdLineaProducto);
            //                //var materiaPropias = lps as IList<MateriaPropia> ?? lps.ToList();
            //                //var allProd = materiaPropia;
            //                //var allOtrosIngresos = materiaPropia.Sum(t => t.OtrosIngresos);
            //                //var allVentasPais = materiaPropias.Sum(t => t.VentasPais);
            //                //var allVentasExtranjero = materiaPropias.Sum(t => t.VentasExtranjero);
            //                //var allOtrasSalidas = materiaPropias.Sum(t => t.OtrasSalidas);
            //                // mp.IsFirst = false;
            //                mp.Existencia = (materiaPropia.Existencia.GetValueOrDefault() + materiaPropia.Produccion.GetValueOrDefault() + materiaPropia.OtrosIngresos.GetValueOrDefault()) -
            //                                (materiaPropia.VentasPais.GetValueOrDefault() + materiaPropia.VentasExtranjero.GetValueOrDefault() + materiaPropia.OtrasSalidas.GetValueOrDefault());

            //                Console.WriteLine("Existencia:" + String.Format("{0:0.0000}", materiaPropia.Existencia));
            //                Console.WriteLine("Produccion:" + String.Format("{0:0.0000}", materiaPropia.Produccion));
            //                Console.WriteLine("OtrosIngresos:" + String.Format("{0:0.0000}", materiaPropia.OtrosIngresos));
            //                Console.WriteLine("VentasPais:" + String.Format("{0:0.0000}", materiaPropia.VentasPais));
            //                Console.WriteLine("VentasExtranjero:" + String.Format("{0:0.0000}", materiaPropia.VentasExtranjero));
            //                Console.WriteLine("OtrasSalidas:" + String.Format("{0:0.0000}", materiaPropia.OtrasSalidas));

            //                Console.WriteLine(mp.IdLineaProducto.ToString() + "----" + mp.Existencia.ToString());
            //            }
            //        }
            //    }
            //}
            Console.ReadKey();
            
        }

        static void Test_Update(int x)
        {
            
        }

        




    }
}
