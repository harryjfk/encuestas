using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
   public partial class MateriaPropia
    {
       public bool IsFirst
       {
           get
           {
               try
               {
                   return !this.VolumenProduccion.Encuesta.Establecimiento.Encuestas.Where(
                       t => t.Id != VolumenProduccion.Encuesta.Id && t.Fecha < VolumenProduccion.Encuesta.Fecha).OfType<EncuestaEstadistica>().Any(t => t.VolumenProduccionMensual.MateriasPropia.Any(h => h.IdLineaProducto == this.IdLineaProducto));
               }
               catch (Exception)
               {
                   return true;
               }
           }
       }

       public bool IsValid
       {
           get
           {
               var val1 = Existencia.GetValueOrDefault() + Produccion.GetValueOrDefault() + OtrosIngresos.GetValueOrDefault();
               var val2 = VentasPais.GetValueOrDefault() + VentasExtranjero.GetValueOrDefault() + OtrasSalidas.GetValueOrDefault();
               return (val1 >= val2);

           }
       }

    }
}
