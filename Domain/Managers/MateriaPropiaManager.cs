using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using Data;
using Data.Repositorios;
using Entity;

namespace Domain.Managers
{
    public class MateriaPropiaManager : GenericManager<MateriaPropia>
    {
        public MateriaPropiaManager(GenericRepository<MateriaPropia> repository, Manager manager)
            : base(repository, manager)
        {
        }
        public MateriaPropiaManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }
        public bool ValidarProduccion(long id, decimal? produccion)
        {
            var materia = Manager.MateriaPropiaManager.Find(id);
            if (materia == null) return false;
            var encuesta = materia.VolumenProduccion.Encuesta;
            var encuestas =
                Manager.EncuestaEstadistica.Get(
                    t => t.IdEstablecimiento == encuesta.IdEstablecimiento && t.Id != encuesta.Id && t.Fecha<encuesta.Fecha);
            var materias = encuestas.Select(
                 t =>
                     t.VolumenProduccionMensual.MateriasPropia.FirstOrDefault(
                         h => h.IdLineaProducto == materia.IdLineaProducto));

            var historico = materias.Where(t=>t!=null).Select(t => (double)t.Produccion.GetValueOrDefault()).ToList();
            historico.Add((double)produccion.GetValueOrDefault());
            var desviacion = historico.DesviacionEstandar();
            var avg = historico.Average();
            var mult = desviacion * 3;
            var min = Math.Abs(avg - mult);
            var max = avg + mult;
            return (double)produccion.GetValueOrDefault() <= max && (double)produccion.GetValueOrDefault() >= min;
        }
        public bool ValidarValorUnitario(long id, decimal? valorUnitario)
        {
            var materia = Manager.MateriaPropiaManager.Find(id);
            if (materia == null) return false;
            var encuesta = materia.VolumenProduccion.Encuesta;
            var encuestas =
                Manager.EncuestaEstadistica.Get(
                    t => t.IdEstablecimiento == encuesta.IdEstablecimiento && t.Id != encuesta.Id && t.Fecha < encuesta.Fecha);
            var materias = encuestas.Select(
                 t =>
                     t.VolumenProduccionMensual.MateriasPropia.FirstOrDefault(
                         h => h.IdLineaProducto == materia.IdLineaProducto));

            var historico = materias.Where(t=>t!=null).Select(t => (double)t.ValorUnitario.GetValueOrDefault()).ToList();
            historico.Add((double)valorUnitario.GetValueOrDefault());
            var desviacion = historico.DesviacionEstandar();
            var avg = historico.Average();
            var mult = desviacion * 3;
            var min = Math.Abs(avg - mult);
            var max = avg + mult;
            return (double)valorUnitario.GetValueOrDefault() <= max && (double)valorUnitario.GetValueOrDefault() >= min;
        }
        public bool ValidarVentasPais(long id, decimal? ventasPais)
        {
            var materia = Manager.MateriaPropiaManager.Find(id);
            if (materia == null) return false;
            var encuesta = materia.VolumenProduccion.Encuesta;
            var encuestas =
                Manager.EncuestaEstadistica.Get(
                    t => t.IdEstablecimiento == encuesta.IdEstablecimiento && t.Id != encuesta.Id && t.Fecha < encuesta.Fecha);
            var materias = encuestas.Select(
                 t =>
                     t.VolumenProduccionMensual.MateriasPropia.FirstOrDefault(
                         h => h.IdLineaProducto == materia.IdLineaProducto));

            var historico = materias.Where(t => t != null).Select(t => (double)t.VentasPais.GetValueOrDefault()).ToList();
            historico.Add((double)ventasPais.GetValueOrDefault());
            var desviacion = historico.DesviacionEstandar();
            var avg = historico.Average();
            var mult = desviacion * 3;
            var min = Math.Abs(avg - mult);
            var max = avg + mult;
            return (double)ventasPais.GetValueOrDefault() <= max && (double)ventasPais.GetValueOrDefault() >= min;
        }

        public bool ValidarVentasExtranjero(long id, decimal? ventasExtranjero)
        {
            var materia = Manager.MateriaPropiaManager.Find(id);
            if (materia == null) return false;
            var encuesta = materia.VolumenProduccion.Encuesta;
            var encuestas =
                Manager.EncuestaEstadistica.Get(
                    t => t.IdEstablecimiento == encuesta.IdEstablecimiento && t.Id != encuesta.Id && t.Fecha < encuesta.Fecha);
            var materias = encuestas.Select(
                 t =>
                     t.VolumenProduccionMensual.MateriasPropia.FirstOrDefault(
                         h => h.IdLineaProducto == materia.IdLineaProducto));

            var historico = materias.Where(t => t != null).Select(t => (double)t.VentasExtranjero.GetValueOrDefault()).ToList();
            historico.Add((double)ventasExtranjero.GetValueOrDefault());
            var desviacion = historico.DesviacionEstandar();
            var avg = historico.Average();
            var mult = desviacion * 3;
            var min = Math.Abs(avg - mult);
            var max = avg + mult;
            return (double)ventasExtranjero.GetValueOrDefault() <= max && (double)ventasExtranjero.GetValueOrDefault() >= min;
        }

        public List<NumberTableItem> GetHistoryProduccion(long idMateriaPropia)
        {
            var materia = Manager.MateriaPropiaManager.Find(idMateriaPropia);
            if (materia == null) return new List<NumberTableItem>();
            var encuesta = materia.VolumenProduccion.Encuesta;
            var now = encuesta.Fecha;
            var encuestas =
                Manager.EncuestaEstadistica.Get(
                    t => t.IdEstablecimiento == encuesta.IdEstablecimiento && (t.Fecha.Year==now.Year|| t.Fecha.Year==now.Year-1));
            var materias = encuestas.Select(
                 t =>
                     t.VolumenProduccionMensual.MateriasPropia.FirstOrDefault(
                         h => h.IdLineaProducto == materia.IdLineaProducto)).ToList();

            var encuestasd =
               Manager.EncuestaEstadistica.Get(
                   t => t.IdEstablecimiento == encuesta.IdEstablecimiento && t.Id != encuesta.Id && t.Fecha < encuesta.Fecha);
            var materiasd = encuestasd.Select(
                 t =>
                     t.VolumenProduccionMensual.MateriasPropia.FirstOrDefault(
                         h => h.IdLineaProducto == materia.IdLineaProducto));

            var historico = materiasd.Select(t => (double)t.Produccion.GetValueOrDefault()).ToList();
            var desviacion = historico.DesviacionEstandar();
            var avg = historico.Average();
            var mult = desviacion * 3;
            var min = Math.Abs(avg - mult);
            var max = avg + mult;
            return materias.Select(t=> new NumberTableItem()
            {
                Month = t.VolumenProduccion.Encuesta.Fecha.ToString("MMMM",CultureInfo.GetCultureInfo("es")),
                Year = t.VolumenProduccion.Encuesta.Fecha.Year,
                Value = t.Produccion.GetValueOrDefault(),
                MonthNumber = t.VolumenProduccion.Encuesta.Fecha.Month,
                Desviacion = desviacion,
                Maximo = max,
                Minimo = min,
                Promedio = avg
            }).ToList();
        }
        public List<NumberTableItem> GetHistoryValorUnitario(long idMateriaPropia)
        {
            var materia = Manager.MateriaPropiaManager.Find(idMateriaPropia);
            if (materia == null) return new List<NumberTableItem>();
            var encuesta = materia.VolumenProduccion.Encuesta;
            var now = encuesta.Fecha;
            var encuestas =
                Manager.EncuestaEstadistica.Get(
                    t => t.IdEstablecimiento == encuesta.IdEstablecimiento && (t.Fecha.Year == now.Year || t.Fecha.Year == now.Year - 1));
            var materias = encuestas.Select(
                 t =>
                     t.VolumenProduccionMensual.MateriasPropia.FirstOrDefault(
                         h => h.IdLineaProducto == materia.IdLineaProducto)).ToList();

            var encuestasd =
               Manager.EncuestaEstadistica.Get(
                   t => t.IdEstablecimiento == encuesta.IdEstablecimiento && t.Id != encuesta.Id && t.Fecha < encuesta.Fecha);
            var materiasd = encuestasd.Select(
                 t =>
                     t.VolumenProduccionMensual.MateriasPropia.FirstOrDefault(
                         h => h.IdLineaProducto == materia.IdLineaProducto));
            var historico = materiasd.Select(t => (double)t.ValorUnitario.GetValueOrDefault()).ToList();
            var desviacion = historico.DesviacionEstandar();
            var avg = historico.Average();
            var mult = desviacion * 3;
            var min = Math.Abs(avg - mult);
            var max = avg + mult;
            return materias.Select(t => new NumberTableItem()
            {
                Month = t.VolumenProduccion.Encuesta.Fecha.ToString("MMMM", CultureInfo.GetCultureInfo("es")),
                Year = t.VolumenProduccion.Encuesta.Fecha.Year,
                Value = t.ValorUnitario.GetValueOrDefault(),
                MonthNumber = t.VolumenProduccion.Encuesta.Fecha.Month,
                Desviacion = desviacion,
                Promedio = avg,
                Maximo = max,
                Minimo = min
            }).ToList();
        }

        public List<NumberTableItem> GetHistoryVentasPais(long idMateriaPropia)
        {
            var materia = Manager.MateriaPropiaManager.Find(idMateriaPropia);
            if (materia == null) return new List<NumberTableItem>();
            var encuesta = materia.VolumenProduccion.Encuesta;
            var now = encuesta.Fecha;
            var encuestas =
                Manager.EncuestaEstadistica.Get(
                    t => t.IdEstablecimiento == encuesta.IdEstablecimiento && (t.Fecha.Year == now.Year || t.Fecha.Year == now.Year - 1));
            var materias = encuestas.Select(
                 t =>
                     t.VolumenProduccionMensual.MateriasPropia.FirstOrDefault(
                         h => h.IdLineaProducto == materia.IdLineaProducto)).ToList();

            var encuestasd =
               Manager.EncuestaEstadistica.Get(
                   t => t.IdEstablecimiento == encuesta.IdEstablecimiento && t.Id != encuesta.Id && t.Fecha < encuesta.Fecha);
            var materiasd = encuestasd.Select(
                 t =>
                     t.VolumenProduccionMensual.MateriasPropia.FirstOrDefault(
                         h => h.IdLineaProducto == materia.IdLineaProducto));
            var historico = materiasd.Select(t => (double)t.VentasPais.GetValueOrDefault()).ToList();
            var desviacion = historico.DesviacionEstandar();
            var avg = historico.Average();
            var mult = desviacion * 3;
            var min = Math.Abs(avg - mult);
            var max = avg + mult;
            return materias.Select(t => new NumberTableItem()
            {
                Month = t.VolumenProduccion.Encuesta.Fecha.ToString("MMMM", CultureInfo.GetCultureInfo("es")),
                Year = t.VolumenProduccion.Encuesta.Fecha.Year,
                Value = t.VentasPais.GetValueOrDefault(),
                MonthNumber = t.VolumenProduccion.Encuesta.Fecha.Month,
                Desviacion = desviacion,
                Promedio = avg,
                Maximo = max,
                Minimo = min
            }).ToList();
        }
        public List<NumberTableItem> GetHistoryVentasExtranjero(long idMateriaPropia)
        {
            var materia = Manager.MateriaPropiaManager.Find(idMateriaPropia);
            if (materia == null) return new List<NumberTableItem>();
            var encuesta = materia.VolumenProduccion.Encuesta;
            var now = encuesta.Fecha;
            var encuestas =
                Manager.EncuestaEstadistica.Get(
                    t => t.IdEstablecimiento == encuesta.IdEstablecimiento && (t.Fecha.Year == now.Year || t.Fecha.Year == now.Year - 1));
            var materias = encuestas.Select(
                 t =>
                     t.VolumenProduccionMensual.MateriasPropia.FirstOrDefault(
                         h => h.IdLineaProducto == materia.IdLineaProducto)).ToList();

            var encuestasd =
               Manager.EncuestaEstadistica.Get(
                   t => t.IdEstablecimiento == encuesta.IdEstablecimiento && t.Id != encuesta.Id && t.Fecha < encuesta.Fecha);
            var materiasd = encuestasd.Select(
                 t =>
                     t.VolumenProduccionMensual.MateriasPropia.FirstOrDefault(
                         h => h.IdLineaProducto == materia.IdLineaProducto));
            var historico = materiasd.Select(t => (double)t.VentasExtranjero.GetValueOrDefault()).ToList();
            var desviacion = historico.DesviacionEstandar();
            var avg = historico.Average();
            var mult = desviacion * 3;
            var min = Math.Abs(avg - mult);
            var max = avg + mult;
            return materias.Select(t => new NumberTableItem()
            {
                Month = t.VolumenProduccion.Encuesta.Fecha.ToString("MMMM", CultureInfo.GetCultureInfo("es")),
                Year = t.VolumenProduccion.Encuesta.Fecha.Year,
                Value = t.VentasExtranjero.GetValueOrDefault(),
                MonthNumber = t.VolumenProduccion.Encuesta.Fecha.Month,
                Desviacion = desviacion,
                Promedio = avg,
                Maximo = max,
                Minimo = min
            }).ToList();
        }
        public override List<string> Validate(MateriaPropia element)
        {
            var list = base.Validate(element);
            list.Required(element, t => t.IdLineaProducto, "Línea de Producto");
            list.Required(element, t => t.IdUnidadMedida, "Unidad de Medida");
            if (element.IdLineaProducto <= 0)
            {
                list.Add("La línea de producto es requerida");
            }
            if (element.IdUnidadMedida <= 0)
            {
                list.Add("La unidad de medida es requerida");
            }
            return list;
        }
    }
}
