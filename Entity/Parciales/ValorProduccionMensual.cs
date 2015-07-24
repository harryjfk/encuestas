using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
   public partial class ValorProduccion
    {
       public bool MateriaPrimaTercerosActivada
       {
           get
           {
               try
               {
                   return
                       this.CAT_ENCUESTA_ESTADISTICA.VolumenProduccionMensual.MateriasTercero.Any(
                           t => t.LineaProducto.IdCiiu == this.id_ciiu);
               }
               catch (Exception)
               {
                   return false;
               }
           }
       }
    }
}
