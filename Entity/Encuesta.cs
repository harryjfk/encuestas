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
    
    public partial class Encuesta
    {
        public Encuesta()
        {
            this.CAT_AUDITORIA = new HashSet<Auditoria>();
        }
    
        public long Id { get; set; }
        public long IdEstablecimiento { get; set; }
        public decimal Estado { get; set; }
        public System.DateTime Fecha { get; set; }
        public string Justificacion { get; set; }
        public Nullable<decimal> IdAnalista { get; set; }
        public Nullable<decimal> IdInformante { get; set; }
        public Nullable<System.DateTime> creado { get; set; }
        public Nullable<System.DateTime> modificado { get; set; }
        public string usuario_creacion { get; set; }
        public string usuario_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_ultimo_envio { get; set; }
    
        public virtual Usuario Analista { get; set; }
        public virtual Establecimiento Establecimiento { get; set; }
        public virtual Usuario Informante { get; set; }
        public virtual ICollection<Auditoria> CAT_AUDITORIA { get; set; }
    }
}
