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
    
    public partial class Usuario
    {
        public Usuario()
        {
            this.Contactos = new HashSet<Contacto>();
            this.EncuestasAsInformante = new HashSet<Encuesta>();
            this.EstablecimientosInformante = new HashSet<Establecimiento>();
            this.Roles = new HashSet<Rol>();
            this.CAT_ESTAB_ANALISTA = new HashSet<EstablecimientoAnalista>();
            this.CAT_ENCUESTA_ANALISTA = new HashSet<EncuestaAnalista>();
        }
    
        public decimal Identificador { get; set; }
        public string Login { get; set; }
        public Nullable<System.DateTime> creado { get; set; }
        public Nullable<System.DateTime> modificado { get; set; }
        public string usuario_creacion { get; set; }
        public string usuario_modificacion { get; set; }
    
        public virtual ICollection<Contacto> Contactos { get; set; }
        public virtual ICollection<Encuesta> EncuestasAsInformante { get; set; }
        public virtual ICollection<Establecimiento> EstablecimientosInformante { get; set; }
        public virtual ICollection<Rol> Roles { get; set; }
        public virtual ICollection<EstablecimientoAnalista> CAT_ESTAB_ANALISTA { get; set; }
        public virtual ICollection<EncuestaAnalista> CAT_ENCUESTA_ANALISTA { get; set; }
    }
}
