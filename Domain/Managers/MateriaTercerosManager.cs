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
    public class MateriaTercerosManager:GenericManager<MateriaTerceros>
    {
        public MateriaTercerosManager(GenericRepository<MateriaTerceros> repository, Manager manager) : base(repository, manager)
        {
        }

        public MateriaTercerosManager(Entities context, Manager manager) : base(context, manager)
        {
        }

        public bool ValidarProduccion(long idEncuesta,long idLineaProducto, decimal? produccion)
        {
           var encuesta = Manager.EncuestaEstadistica.Find(idEncuesta);
            if (encuesta == null) return false;
            var encuestas =
                Manager.EncuestaEstadistica.Get(
                    t => t.IdEstablecimiento == encuesta.IdEstablecimiento && t.Id != encuesta.Id && t.Fecha < encuesta.Fecha);
            var materias = encuestas.Select(
                 t =>
                     t.VolumenProduccionMensual.MateriasTercero.FirstOrDefault(
                         h => h.IdLineaProducto == idLineaProducto));

            var historico = materias.Where(t=>t!=null).Select(t => double.Parse(t.UnidadProduccion)).ToList();
            historico.Add((double)produccion.GetValueOrDefault());
            var desviacion = historico.DesviacionEstandar();
            var avg = historico.Average();
            var mult = desviacion * 3;
            var min = Math.Abs(avg - mult);
            var max = avg + mult;
            return (double)produccion.GetValueOrDefault() <= max && (double)produccion.GetValueOrDefault() >= min;
        }
        public override List<string> Validate(MateriaTerceros element)
        {
            var list = base.Validate(element);
            list.Required(element, t => t.IdLineaProducto, "Línea de Producto");
            list.Required(element, t => t.IdUnidadMedida, "Unidad de Medida");
            list.Required(element, t => t.UnidadProduccion, "Producción");
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

        public List<NumberTableItem> GetHistoryProduccion(long idMateriaPropia)
        {
            var materia = Manager.MateriaTercerosManager.Find(idMateriaPropia);
            if (materia == null) return new List<NumberTableItem>();
            var encuesta = materia.VolumenProduccion.Encuesta;
            var now = encuesta.Fecha;
            var encuestas =
                Manager.EncuestaEstadistica.Get(
                    t => t.IdEstablecimiento == encuesta.IdEstablecimiento && (t.Fecha.Year == now.Year || t.Fecha.Year == now.Year - 1));
            var materias = encuestas.Select(
                 t =>
                     t.VolumenProduccionMensual.MateriasTercero.FirstOrDefault(
                         h => h.IdLineaProducto == materia.IdLineaProducto)).ToList();

            var encuestasd =
               Manager.EncuestaEstadistica.Get(
                   t => t.IdEstablecimiento == encuesta.IdEstablecimiento && t.Id != encuesta.Id && t.Fecha < encuesta.Fecha);
            var materiasd = encuestasd.Select(
                 t =>
                     t.VolumenProduccionMensual.MateriasTercero.FirstOrDefault(
                         h => h.IdLineaProducto == materia.IdLineaProducto));
            var historico = materiasd.Select(t => double.Parse(t.UnidadProduccion)).ToList();
            var desviacion = historico.DesviacionEstandar();
            var avg = historico.Average();
            var mult = desviacion * 3;
            var min = Math.Abs(avg - mult);
            var max = avg + mult;
            return materias.Select(t => new NumberTableItem()
            {
                Month = t.VolumenProduccion.Encuesta.Fecha.ToString("MMMM", CultureInfo.GetCultureInfo("es")),
                Year = t.VolumenProduccion.Encuesta.Fecha.Year,
                Value = decimal.Parse(t.UnidadProduccion),
                MonthNumber = t.VolumenProduccion.Encuesta.Fecha.Month,
                Desviacion = desviacion,
                Promedio = avg,
                Maximo = max,
                Minimo = min
            }).ToList();
        }
    
    }
}
