using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Repositorios;
using Entity;

namespace Domain.Managers
{
    public class VentasPaisExtranjeroManager:GenericManager<VentasPaisExtranjero>
    {
        public VentasPaisExtranjeroManager(GenericRepository<VentasPaisExtranjero> repository, Manager manager) : base(repository, manager)
        {
        }

        public VentasPaisExtranjeroManager(Entities context, Manager manager) : base(context, manager)
        {
        }

        public void Generate(long idEncuesta)
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.Find(idEncuesta);
            if (encuesta == null)
                return;
            var venta = encuesta.VentasProductosEstablecimiento;
            if (venta == null)
            {
                venta = new VentasProductosEstablecimientos()
                {
                    Identificador = encuesta.Id
                };
                manager.VentasProductosEstablecimientoManager.Add(venta);
                manager.VentasProductosEstablecimientoManager.SaveChanges();
            }

            foreach (var group in encuesta.VolumenProduccionMensual.MateriasPropia.GroupBy(t=>t.LineaProducto.IdCiiu))
            {
                var first = venta.CAT_VENTAS_PAIS_EXTRANJERO.FirstOrDefault(t => t.id_ciiu == group.Key);
                if (first == null)
                {
                    first = new VentasPaisExtranjero()
                    {
                        id_ciiu = group.Key,
                        id_ventas_producto = venta.Identificador,
                    };
                    manager.VentasPaisExtranjeroManager.Add(first);
                    manager.VentasPaisExtranjeroManager.SaveChanges();
                }
                //var ventaPais = group.Sum(t => t.VentasPais);
                //first.VentaPais = ventaPais;
               // manager.VentasPaisExtranjeroManager.Modify(first);
                //manager.VentasPaisExtranjeroManager.SaveChanges();
                //COMO SE CALCULA LAS VENTAS EN EL EXTRANJERO
            }
        }

        public bool ValidarVentaPais(long id, decimal? valor)
        {
            if (valor == null || valor==0) return true;
            var materia = Manager.VentasPaisExtranjeroManager.Find(id);

            if (materia == null) return false;
            var encuesta = materia.CAT_VENTAS_PROD_ESTAB.Encuesta;
            var encuestas =
                Manager.EncuestaEstadistica.Get(
                    t => t.IdEstablecimiento == encuesta.IdEstablecimiento && t.Id != encuesta.Id && t.Fecha < encuesta.Fecha);
            var materias = encuestas.SelectMany(
                 t =>
                     t.VentasProductosEstablecimiento.CAT_VENTAS_PAIS_EXTRANJERO.Where(h=>h.id_ciiu==materia.id_ciiu));

            var historico = materias.Where(t => t != null).Select(t => (double)t.VentaPais.GetValueOrDefault()).ToList();
            historico.Add((double)valor.GetValueOrDefault());
            var desviacion = historico.DesviacionEstandar();
            var avg = historico.Average();
            var mult = desviacion * 3;
            var min = Math.Abs(avg - mult);
            var max = avg + mult;
            return (double)valor.GetValueOrDefault() <= max && (double)valor.GetValueOrDefault() >= min;
        }
        public bool ValidarVentaExtranjero(long id, decimal? valor)
        {
            if (valor == null || valor == 0) return true;
            var materia = Manager.VentasPaisExtranjeroManager.Find(id);

            if (materia == null) return false;
            var encuesta = materia.CAT_VENTAS_PROD_ESTAB.Encuesta;
            var encuestas =
                Manager.EncuestaEstadistica.Get(
                    t => t.IdEstablecimiento == encuesta.IdEstablecimiento && t.Id != encuesta.Id && t.Fecha < encuesta.Fecha);
            var materias = encuestas.SelectMany(
                 t =>
                     t.VentasProductosEstablecimiento.CAT_VENTAS_PAIS_EXTRANJERO.Where(h => h.id_ciiu == materia.id_ciiu));

            var historico = materias.Where(t => t != null).Select(t => (double)t.VentaExtranjero.GetValueOrDefault()).ToList();
            historico.Add((double)valor.GetValueOrDefault());
            var desviacion = historico.DesviacionEstandar();
            var avg = historico.Average();
            var mult = desviacion * 3;
            var min = Math.Abs(avg - mult);
            var max = avg + mult;
            return (double)valor.GetValueOrDefault() <= max && (double)valor.GetValueOrDefault() >= min;
        }

        public object GetHistoryVentasPais(long id)
        {
            var materia = Manager.VentasPaisExtranjeroManager.Find(id);
            if (materia == null) return new List<NumberTableItem>();
            var encuesta = materia.CAT_VENTAS_PROD_ESTAB.Encuesta;
            var now = encuesta.Fecha;
            var encuestas =
                Manager.EncuestaEstadistica.Get(
                    t => t.IdEstablecimiento == encuesta.IdEstablecimiento && (t.Fecha.Year == now.Year || t.Fecha.Year == now.Year - 1));
            var materias = encuestas.Select(
                 t =>
                     t.VentasProductosEstablecimiento.CAT_VENTAS_PAIS_EXTRANJERO.FirstOrDefault(
                         h => h.id_ciiu == materia.id_ciiu)).ToList();

            var encuestasd =
               Manager.EncuestaEstadistica.Get(
                   t => t.IdEstablecimiento == encuesta.IdEstablecimiento && t.Id != encuesta.Id && t.Fecha < encuesta.Fecha);
            var materiasd = encuestasd.Select(
                 t =>
                     t.VentasProductosEstablecimiento.CAT_VENTAS_PAIS_EXTRANJERO.FirstOrDefault(
                         h => h.id_ciiu == materia.id_ciiu)).Where(t => t != null);
            var historico = materiasd.Select(t => (double)t.VentaPais.GetValueOrDefault()).ToList();
            var desviacion = historico.DesviacionEstandar();
            var avg = historico.Average();
            var mult = desviacion * 3;
            var min = Math.Abs(avg - mult);
            var max = avg + mult;
            return materias.Where(t=>t!=null).Select(t => new NumberTableItem()
            {
                Month = t.CAT_VENTAS_PROD_ESTAB.Encuesta.Fecha.ToString("MMMM", CultureInfo.GetCultureInfo("es")),
                Year = t.CAT_VENTAS_PROD_ESTAB.Encuesta.Fecha.Year,
                Value = t.VentaPais.GetValueOrDefault(),
                MonthNumber = t.CAT_VENTAS_PROD_ESTAB.Encuesta.Fecha.Month,
                Desviacion = desviacion,
                Promedio = avg,
                Maximo = max,
                Minimo = min
            }).ToList();
        }
        public object GetHistoryVentasExtranjero(long id)
        {
            var materia = Manager.VentasPaisExtranjeroManager.Find(id);
            if (materia == null) return new List<NumberTableItem>();
            var encuesta = materia.CAT_VENTAS_PROD_ESTAB.Encuesta;
            var now = encuesta.Fecha;
            var encuestas =
                Manager.EncuestaEstadistica.Get(
                    t => t.IdEstablecimiento == encuesta.IdEstablecimiento && (t.Fecha.Year == now.Year || t.Fecha.Year == now.Year - 1));
            var materias = encuestas.Select(
                 t =>
                     t.VentasProductosEstablecimiento.CAT_VENTAS_PAIS_EXTRANJERO.FirstOrDefault(
                         h => h.id_ciiu == materia.id_ciiu)).ToList();

            var encuestasd =
               Manager.EncuestaEstadistica.Get(
                   t => t.IdEstablecimiento == encuesta.IdEstablecimiento && t.Id != encuesta.Id && t.Fecha < encuesta.Fecha);
            var materiasd = encuestasd.Select(
                 t =>
                     t.VentasProductosEstablecimiento.CAT_VENTAS_PAIS_EXTRANJERO.FirstOrDefault(
                         h => h.id_ciiu == materia.id_ciiu)).Where(t => t != null);
            var historico = materiasd.Select(t => (double)t.VentaExtranjero.GetValueOrDefault()).ToList();
            var desviacion = historico.DesviacionEstandar();
            var avg = historico.Average();
            var mult = desviacion * 3;
            var min = Math.Abs(avg - mult);
            var max = avg + mult;
            return materias.Where(t => t != null).Select(t => new NumberTableItem()
            {
                Month = t.CAT_VENTAS_PROD_ESTAB.Encuesta.Fecha.ToString("MMMM", CultureInfo.GetCultureInfo("es")),
                Year = t.CAT_VENTAS_PROD_ESTAB.Encuesta.Fecha.Year,
                Value = t.VentaExtranjero.GetValueOrDefault(),
                MonthNumber = t.CAT_VENTAS_PROD_ESTAB.Encuesta.Fecha.Month,
                Desviacion = desviacion,
                Promedio = avg,
                Maximo = max,
                Minimo = min
            }).ToList();
        }
    }
}
