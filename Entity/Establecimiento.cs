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
    
    public partial class Establecimiento
    {
        public Establecimiento()
        {
            this.Contactos = new HashSet<Contacto>();
            this.Encuestas = new HashSet<Encuesta>();
            this.LineasProductoEstablecimiento = new HashSet<LineaProductoEstablecimiento>();
            this.Ciius = new HashSet<Ciiu>();
            this.CAT_AÑO_BASE = new HashSet<AñoBase>();
            this.CAT_ESTAB_ANALISTA = new HashSet<EstablecimientoAnalista>();
        }
    
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string IdentificadorInterno { get; set; }
        public string Ubigeo { get; set; }
        public string RazonSocial { get; set; }
        public string Ruc { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Fax { get; set; }
        public decimal Estado { get; set; }
        public Nullable<decimal> IdInformante { get; set; }
        public string Observaciones { get; set; }
        public Nullable<System.DateTime> creado { get; set; }
        public Nullable<System.DateTime> modificado { get; set; }
        public string usuario_creacion { get; set; }
        public string usuario_modificacion { get; set; }
        public Nullable<decimal> tipo_establecimiento { get; set; }
        public decimal enviar_correo { get; set; }
        public Nullable<System.DateTime> ultima_notificacion { get; set; }
    
        public virtual ICollection<Contacto> Contactos { get; set; }
        public virtual ICollection<Encuesta> Encuestas { get; set; }
        public virtual ICollection<LineaProductoEstablecimiento> LineasProductoEstablecimiento { get; set; }
        public virtual Usuario Informante { get; set; }
        public virtual ICollection<Ciiu> Ciius { get; set; }
        public virtual ICollection<AñoBase> CAT_AÑO_BASE { get; set; }
        public virtual ICollection<EstablecimientoAnalista> CAT_ESTAB_ANALISTA { get; set; }
    }
}
