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
    public class ValorProduccionManager : GenericManager<ValorProduccion>
    {
        public ValorProduccionManager(GenericRepository<ValorProduccion> repository, Manager manager)
            : base(repository, manager)
        {
        }

        public ValorProduccionManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }

        public void Generate(long idEncuesta)
        {
            var manager = Manager;
            var encuesta = manager.EncuestaEstadistica.Find(idEncuesta);
            if (encuesta == null) return;
            while (encuesta.CAT_VALOR_PROD_MENSUAL.Any(t=>t.ProductosMateriaPropia!=null))
            {
                var first = encuesta.CAT_VALOR_PROD_MENSUAL.FirstOrDefault(t=>t.ProductosMateriaPropia != null);
                if (first.ProductosMateriaTerceros == null)
                {
                    manager.ValorProduccionManager.Delete(first);
                    manager.ValorProduccionManager.SaveChanges();
                }
                else
                {
                    first.ProductosMateriaPropia = null;
                    manager.ValorProduccionManager.Modify(first);
                    manager.ValorProduccionManager.SaveChanges();
                }
            }
            //MATERIA PRIMA PROPIA
            var volumen = encuesta.VolumenProduccionMensual;
            var tempp = volumen.MateriasPropia.Select(t => t.Id).ToList();
            var listp = Manager.MateriaPropiaManager.Get(t => tempp.Contains(t.Id)).ToList();
            listp.ForEach(t => t.LineaProducto = Manager.LineaProducto.Find(t.IdLineaProducto));
            foreach (var grupo in listp.GroupBy(t=>t.LineaProducto.IdCiiu))
            {
                var valor =
                    manager.ValorProduccionManager.Get(t => t.id_ciiu == grupo.Key && t.id_encuesta == encuesta.Id)
                        .FirstOrDefault();
                if (valor == null)
                {
                    valor = new ValorProduccion()
                    {
                        id_ciiu = grupo.Key,
                        id_encuesta = encuesta.Id,
                        ProductosMateriaPropia = grupo.Sum(t => t.ValorUnitario*t.Produccion)
                    };
                    manager.ValorProduccionManager.Add(valor);
                    manager.ValorProduccionManager.SaveChanges();
                }
                else
                {
                    valor.ProductosMateriaPropia = grupo.Sum(t => t.ValorUnitario*t.Produccion);
                    manager.ValorProduccionManager.Modify(valor);
                    manager.ValorProduccionManager.SaveChanges();
                }
            }

            //MATERIA PRIMA TERCERO
            var temp = volumen.MateriasTercero.Select(t => t.Id).ToList();
            var list = Manager.MateriaTercerosManager.Get(t => temp.Contains(t.Id)).ToList();
            list.ForEach(t => t.LineaProducto = Manager.LineaProducto.Find(t.IdLineaProducto));
            foreach (var materia in list.Where(t=> t!=null&&t.LineaProducto!=null).GroupBy(t=>t.LineaProducto.IdCiiu))
            {
                var tem =
                    encuesta.CAT_VALOR_PROD_MENSUAL.FirstOrDefault(
                        t => t.id_ciiu == materia.Key);
                if (tem == null)
                {
                    tem = new ValorProduccion()
                    {
                        id_encuesta = encuesta.Id,
                        id_ciiu = materia.Key
                    };
                    manager.ValorProduccionManager.Add(tem);
                    manager.ValorProduccionManager.SaveChanges();
                }
            }
            var delete = new List<long>();
            foreach (var valor in encuesta.CAT_VALOR_PROD_MENSUAL.Where(t=>t.ProductosMateriaPropia==null))
            {
                var tem = volumen.MateriasTercero.FirstOrDefault(t => t.LineaProducto.IdCiiu == valor.id_ciiu);
                if (tem == null)
                {
                    delete.Add(valor.Id);
                }
            }
            foreach (var l in delete)
            {
                manager.ValorProduccionManager.Delete(l);
                manager.ValorProduccionManager.SaveChanges();
            }
            Manager.VentasPaisExtranjeroManager.Generate(idEncuesta);
        }

        public bool ValidarMateriaPropia(long id, decimal? valor,long idCiiu)
        {
            var materia = Manager.ValorProduccionManager.Find(id);
          
            if (materia == null) return false;
            var encuesta = materia.CAT_ENCUESTA_ESTADISTICA;
            var encuestas =
                Manager.EncuestaEstadistica.Get(
                    t => t.IdEstablecimiento == encuesta.IdEstablecimiento && t.Id != encuesta.Id && t.Fecha < encuesta.Fecha);
            var materias = encuestas.Select(
                 t =>
                     t.CAT_VALOR_PROD_MENSUAL.FirstOrDefault(
                         h => h.id_ciiu == idCiiu));

            var historico = materias.Select(t => (double)t.ProductosMateriaPropia.GetValueOrDefault()).ToList();
            historico.Add((double)valor.GetValueOrDefault());
            var desviacion = historico.DesviacionEstandar();
            var avg = historico.Average();
            var mult = desviacion * 3;
            var min = Math.Abs(avg - mult);
            var max = avg + mult;
            return (double)valor.GetValueOrDefault() <= max && (double)valor.GetValueOrDefault() >= min;
        }

        public bool ValidarMateriaTerceros(long id, decimal? valor, long idCiiu)
        {
            var materia = Manager.ValorProduccionManager.Find(id);

            if (materia == null) return false;
            var encuesta = materia.CAT_ENCUESTA_ESTADISTICA;
            var encuestas =
                Manager.EncuestaEstadistica.Get(
                    t => t.IdEstablecimiento == encuesta.IdEstablecimiento && t.Id != encuesta.Id && t.Fecha < encuesta.Fecha);
            var materias = encuestas.SelectMany(
                 t =>
                     t.CAT_VALOR_PROD_MENSUAL.Where(h=>h.id_ciiu==materia.id_ciiu));

            var historico = materias.Where(t=>t!=null).Select(t => (double)t.ProductosMateriaTerceros.GetValueOrDefault()).ToList();
            historico.Add((double)valor.GetValueOrDefault());
            var desviacion = historico.DesviacionEstandar();
            var avg = historico.Average();
            var mult = desviacion * 3;
            var min = Math.Abs(avg - mult);
            var max = avg + mult;
            return (double)valor.GetValueOrDefault() <= max && (double)valor.GetValueOrDefault() >= min;
        }




        public object GetHistoryMateriaPrimaTerceros(long id)
        {
            var materia = Manager.ValorProduccionManager.Find(id);
            if (materia == null) return new List<NumberTableItem>();
            var encuesta = materia.CAT_ENCUESTA_ESTADISTICA;
            var now = encuesta.Fecha;
            var encuestas =
                Manager.EncuestaEstadistica.Get(
                    t => t.IdEstablecimiento == encuesta.IdEstablecimiento && (t.Fecha.Year == now.Year || t.Fecha.Year == now.Year - 1));
            var materias = encuestas.Select(
                 t =>
                     t.CAT_VALOR_PROD_MENSUAL.FirstOrDefault(
                         h => h.id_ciiu == materia.id_ciiu)).ToList();

            var encuestasd =
               Manager.EncuestaEstadistica.Get(
                   t => t.IdEstablecimiento == encuesta.IdEstablecimiento && t.Id != encuesta.Id && t.Fecha < encuesta.Fecha);
            var materiasd = encuestasd.Select(
                 t =>
                     t.CAT_VALOR_PROD_MENSUAL.FirstOrDefault(
                         h => h.id_ciiu == materia.id_ciiu));
            var historico = materiasd.Select(t => (double)t.ProductosMateriaTerceros.GetValueOrDefault()).ToList();
            var desviacion = historico.DesviacionEstandar();
            var avg = historico.Average();
            var mult = desviacion * 3;
            var min = Math.Abs(avg - mult);
            var max = avg + mult;
            return materias.Select(t => new NumberTableItem()
            {
                Month = t.CAT_ENCUESTA_ESTADISTICA.Fecha.ToString("MMMM", CultureInfo.GetCultureInfo("es")),
                Year = t.CAT_ENCUESTA_ESTADISTICA.Fecha.Year,
                Value = t.ProductosMateriaTerceros.GetValueOrDefault(),
                MonthNumber = t.CAT_ENCUESTA_ESTADISTICA.Fecha.Month,
                Desviacion = desviacion,
                Promedio = avg,
                Maximo = max,
                Minimo = min
            }).ToList();
        }
    }
}
