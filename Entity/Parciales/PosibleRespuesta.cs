using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
   public partial class PosibleRespuesta
   {
       public TipoPosibleRespuesta Tipo
       {
           get
           {
               return (TipoPosibleRespuesta)Enum.Parse(typeof (TipoPosibleRespuesta), TipoPosibleRespuesta.ToString());
           }
           set
           {
               Tipo = value;
           }
       }
   }
}
