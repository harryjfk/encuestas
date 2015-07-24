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
    public class VentaServicioManufacturaManager:GenericManager<VentasServicioManufactura>
    {
        public VentaServicioManufacturaManager(GenericRepository<VentasServicioManufactura> repository, Manager manager) : base(repository, manager)
        {
        }

        public VentaServicioManufacturaManager(Entities context, Manager manager) : base(context, manager)
        {
        }
        public bool ValidarVentaPais(long id, decimal? valor)
        {
            if (valor == null) return true;
            var materia = Manager.VentaServicioManufacturaManager.Find(id);

            if (materia == null) return true;
            var encuesta = materia.VentaProductoEstablecimiento.Encuesta;
            var encuestas =
                Manager.EncuestaEstadistica.Get(
                    t => t.IdEstablecimiento == encuesta.IdEstablecimiento && t.Id != encuesta.Id && t.Fecha < encuesta.Fecha);
            var materias = encuestas.SelectMany(
                 t =>
                     t.VentasProductosEstablecimiento.VentasServicioManufactura.Where(h => h.ciiu == materia.ciiu));

            var historico = materias.Where(t => t != null).Select(t => (double)t.venta.GetValueOrDefault()).ToList();
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
            if (valor == null) return true;
            var materia = Manager.VentaServicioManufacturaManager.Find(id);

            if (materia == null) return true;
            var encuesta = materia.VentaProductoEstablecimiento.Encuesta;
            var encuestas =
                Manager.EncuestaEstadistica.Get(
                    t => t.IdEstablecimiento == encuesta.IdEstablecimiento && t.Id != encuesta.Id && t.Fecha < encuesta.Fecha);
            var materias = encuestas.SelectMany(
                 t =>
                     t.VentasProductosEstablecimiento.VentasServicioManufactura.Where(h => h.ciiu == materia.ciiu));

            var historico = materias.Where(t => t != null).Select(t => (double)t.venta_extranjero.GetValueOrDefault()).ToList();
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
            var materia = Manager.VentaServicioManufacturaManager.Find(id);
            if (materia == null) return new List<NumberTableItem>();
            var encuesta = materia.VentaProductoEstablecimiento.Encuesta;
            var now = encuesta.Fecha;
            var encuestas =
                Manager.EncuestaEstadistica.Get(
                    t => t.IdEstablecimiento == encuesta.IdEstablecimiento && (t.Fecha.Year == now.Year || t.Fecha.Year == now.Year - 1));
            var materias = encuestas.Select(
                 t =>
                     t.VentasProductosEstablecimiento.VentasServicioManufactura.FirstOrDefault(
                         h => h.ciiu == materia.ciiu)).ToList();

            var encuestasd =
               Manager.EncuestaEstadistica.Get(
                   t => t.IdEstablecimiento == encuesta.IdEstablecimiento && t.Id != encuesta.Id && t.Fecha < encuesta.Fecha);
            var materiasd = encuestasd.Select(
                 t =>
                     t.VentasProductosEstablecimiento.VentasServicioManufactura.FirstOrDefault(
                         h => h.ciiu == materia.ciiu)).Where(t => t != null);
            var historico = materiasd.Select(t => (double)t.venta.GetValueOrDefault()).ToList();
            var desviacion = historico.DesviacionEstandar();
            var avg = historico.Average();
            var mult = desviacion * 3;
            var min = Math.Abs(avg - mult);
            var max = avg + mult;
            return materias.Where(t => t != null).Select(t => new NumberTableItem()
            {
                Month = t.VentaProductoEstablecimiento.Encuesta.Fecha.ToString("MMMM", CultureInfo.GetCultureInfo("es")),
                Year = t.VentaProductoEstablecimiento.Encuesta.Fecha.Year,
                Value = t.venta.GetValueOrDefault(),
                MonthNumber = t.VentaProductoEstablecimiento.Encuesta.Fecha.Month,
                Desviacion = desviacion,
                Promedio = avg,
                Maximo = max,
                Minimo = min
            }).ToList();
        }
        public object GetHistoryVentasExtranjero(long id)
        {
            var materia = Manager.VentaServicioManufacturaManager.Find(id);
            if (materia == null) return new List<NumberTableItem>();
            var encuesta = materia.VentaProductoEstablecimiento.Encuesta;
            var now = encuesta.Fecha;
            var encuestas =
                Manager.EncuestaEstadistica.Get(
                    t => t.IdEstablecimiento == encuesta.IdEstablecimiento && (t.Fecha.Year == now.Year || t.Fecha.Year == now.Year - 1));
            var materias = encuestas.Select(
                 t =>
                     t.VentasProductosEstablecimiento.VentasServicioManufactura.FirstOrDefault(
                         h => h.ciiu == materia.ciiu)).ToList();

            var encuestasd =
               Manager.EncuestaEstadistica.Get(
                   t => t.IdEstablecimiento == encuesta.IdEstablecimiento && t.Id != encuesta.Id && t.Fecha < encuesta.Fecha);
            var materiasd = encuestasd.Select(
                 t =>
                     t.VentasProductosEstablecimiento.VentasServicioManufactura.FirstOrDefault(
                         h => h.ciiu == materia.ciiu)).Where(t => t != null);
            var historico = materiasd.Select(t => (double)t.venta_extranjero.GetValueOrDefault()).ToList();
            var desviacion = historico.DesviacionEstandar();
            var avg = historico.Average();
            var mult = desviacion * 3;
            var min = Math.Abs(avg - mult);
            var max = avg + mult;
            return materias.Where(t => t != null).Select(t => new NumberTableItem()
            {
                Month = t.VentaProductoEstablecimiento.Encuesta.Fecha.ToString("MMMM", CultureInfo.GetCultureInfo("es")),
                Year = t.VentaProductoEstablecimiento.Encuesta.Fecha.Year,
                Value = t.venta_extranjero.GetValueOrDefault(),
                MonthNumber = t.VentaProductoEstablecimiento.Encuesta.Fecha.Month,
                Desviacion = desviacion,
                Promedio = avg,
                Maximo = max,
                Minimo = min
            }).ToList();
        }
    }
}
