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
    
    public partial class EstablecimientoAnalista
    {
        public long Id { get; set; }
        public decimal orden { get; set; }
        public long id_establecimiento { get; set; }
        public decimal id_analista { get; set; }
        public long id_ciiu { get; set; }
    
        public virtual Establecimiento CAT_ESTABLECIMIENTO { get; set; }
        public virtual Usuario SEG_USUARIO { get; set; }
        public virtual Ciiu CAT_CIIU { get; set; }        
    }
}
