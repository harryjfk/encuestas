using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
   public partial class Pregunta
   {
       public TipoPregunta TipoPregunta
       {
           get { return Comienza != null ? TipoPregunta.Temporal : TipoPregunta.Permanente; }
       }

       public bool Activado
       {
           get { return Estado == 1; }
           set { Estado = value ? 1 : 0; }
       }

       public bool PreguntasObligatorias
       {
           get { return todas_alternativas_oblig == 1; }
           set { todas_alternativas_oblig = value ? 1 : 0; }
       }

        public Func<Pregunta, bool> BuildFilter()
        {
            return t => t.IdEncuestaEmpresarial == null;
        }

       public override string ToString()
       {
           var subName = Texto;
           return String.Format("{0}", subName);
       }
   }
}
