//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
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
            this.DAT_ENCUESTA_ANALISTA = new HashSet<EncuestaAnalista>();
            this.DAT_ENCUESTA_AUDITORIA = new HashSet<Auditoria>();
        }
    
        public long Id { get; set; }
        public long IdEstablecimiento { get; set; }
        public decimal Estado { get; set; }
        public System.DateTime Fecha { get; set; }
        public string Justificacion { get; set; }
        public Nullable<decimal> IdInformante { get; set; }
        public Nullable<System.DateTime> creado { get; set; }
        public Nullable<System.DateTime> modificado { get; set; }
        public string usuario_creacion { get; set; }
        public string usuario_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_ultimo_envio { get; set; }
    
        public virtual Establecimiento Establecimiento { get; set; }
        public virtual Usuario Informante { get; set; }
        public virtual ICollection<EncuestaAnalista> DAT_ENCUESTA_ANALISTA { get; set; }
        public virtual ICollection<Auditoria> DAT_ENCUESTA_AUDITORIA { get; set; }
    }
}
