//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class VentasProductosEstablecimientos
    {
        public VentasProductosEstablecimientos()
        {
            this.VentasServicioManufactura = new HashSet<VentasServicioManufactura>();
            this.CAT_VENTAS_PAIS_EXTRANJERO = new HashSet<VentasPaisExtranjero>();
        }
    
        public long Identificador { get; set; }
        public short brindo_servicios { get; set; }
    
        public virtual ICollection<VentasServicioManufactura> VentasServicioManufactura { get; set; }
        public virtual EncuestaEstadistica Encuesta { get; set; }
        public virtual ICollection<VentasPaisExtranjero> CAT_VENTAS_PAIS_EXTRANJERO { get; set; }
    }
}