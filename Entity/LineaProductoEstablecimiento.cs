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
    
    public partial class LineaProductoEstablecimiento
    {
        public long IdEstablecimiento { get; set; }
        public long IdLineaProducto { get; set; }
        public long Id { get; set; }
        public Nullable<System.DateTime> creado { get; set; }
        public Nullable<System.DateTime> modificado { get; set; }
        public string usuario_creacion { get; set; }
        public string usuario_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_creacion_informante { get; set; }
        public Nullable<decimal> peso { get; set; }
    
        public virtual Establecimiento Establecimiento { get; set; }
        public virtual LineaProducto LineaProducto { get; set; }
    }
}
