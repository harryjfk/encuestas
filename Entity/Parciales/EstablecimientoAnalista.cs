using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
   public partial class EstablecimientoAnalista
   {
        public Func<EstablecimientoAnalista, bool> BuildFilter()
        {         
            Func<EstablecimientoAnalista, bool> filterRuc = t => t.Id > 0;
            Func<EstablecimientoAnalista, bool> filterId = t => t.Id > 0;
            Func<EstablecimientoAnalista, bool> filterRazonSocial = t => t.Id > 0;

            if (CAT_ESTABLECIMIENTO != null)
            {
                if (!string.IsNullOrEmpty(CAT_ESTABLECIMIENTO.Ruc) && !string.IsNullOrWhiteSpace(CAT_ESTABLECIMIENTO.Ruc))
                {
                    var temp = CAT_ESTABLECIMIENTO.Ruc;
                    filterRuc = t => t.CAT_ESTABLECIMIENTO.Ruc.Contains(temp);
                }

                if (!string.IsNullOrEmpty(CAT_ESTABLECIMIENTO.IdentificadorInterno) && !string.IsNullOrWhiteSpace(CAT_ESTABLECIMIENTO.IdentificadorInterno))
                {
                    var temp = CAT_ESTABLECIMIENTO.IdentificadorInterno;
                    filterId = t => t.CAT_ESTABLECIMIENTO.IdentificadorInterno.Contains(temp);
                }

                if (!string.IsNullOrEmpty(CAT_ESTABLECIMIENTO.Nombre) && !string.IsNullOrWhiteSpace(CAT_ESTABLECIMIENTO.Nombre))
                {
                    var temp = CAT_ESTABLECIMIENTO.Nombre;
                    filterRazonSocial = t => t.CAT_ESTABLECIMIENTO.Nombre.Contains(temp);
                }                
            }            

            return t => filterRuc(t) && filterId(t) && filterRazonSocial(t);
        }
    }
}
