using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Parciales;

namespace Entity
{
    public partial class Auditoria
    {

        public string TipoEncuesta
        {
            get
            {
                if (Encuesta == null) return "Ninguna";
                if (Encuesta is EncuestaEmpresarial)
                    return "Encuesta Empresarial";
                return "Encuesta Estadística";
            }
        }

        public AccionEnum AccionEnum
        {
            get
            {
                return (AccionEnum) Enum.Parse(typeof(AccionEnum), accion.ToString());
            }
            set
            {
                accion = (int) value;
            }
        }

        public Func<Auditoria, bool> BuildFilter()
        {
            return null;
        }
    }
}
