using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public partial class VentasPaisExtranjero
    {
        public bool VentaPaisActivado
        {
            get
            {
                try
                {
                    var encuesta = this.CAT_VENTAS_PROD_ESTAB.Encuesta;
                    var volumenes =
                        encuesta.VolumenProduccionMensual.MateriasPropia.Where(
                            t => t.LineaProducto.IdCiiu == this.id_ciiu);
                    var sum = volumenes.Sum(t => t.VentasPais);
                    return sum > 0;
                }
                catch (Exception)
                {
                    return VentaPais != null;
                }
            }
        }
        public bool VentaExtranjeroActivado
        {
            get
            {
                try
                {
                    var encuesta = this.CAT_VENTAS_PROD_ESTAB.Encuesta;
                    var volumenes =
                        encuesta.VolumenProduccionMensual.MateriasPropia.Where(
                            t => t.LineaProducto.IdCiiu == this.id_ciiu);
                    var sum = volumenes.Sum(t => t.VentasExtranjero);
                    return sum > 0;
                }
                catch (Exception)
                {
                    return VentaExtranjero != null;
                }
            }
        }
    }
}
