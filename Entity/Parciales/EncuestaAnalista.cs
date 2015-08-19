using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
   public partial class EncuestaAnalista
    {

       public EstadoEncuesta EstadoEncuesta
       {
           get
           {
               return (EstadoEncuesta)Enum.Parse(typeof(EstadoEncuesta), estado.ToString());
           }
           set
           {
               estado = (int)value;
           }
       }

       public bool IsCurrent
       {
           get
           {
               return current == 1;
           }
           
       }

       public bool IsPast
       {
           get
           {
               return current == 2;
           }
       }
       public bool IsWaiting
       {
           get
           {
               return current == 0;
           }
       }
       public Func<EncuestaAnalista, bool> BuildFilter()
       {
           return null;
       }
    }
}
