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
    
    public partial class Valor
    {
        public long Id { get; set; }
        public string Texto { get; set; }
        public long IdPosibleRespuesta { get; set; }
        public Nullable<long> IdPregunta { get; set; }
        public decimal Estado { get; set; }
        public Nullable<System.DateTime> creado { get; set; }
        public Nullable<System.DateTime> modificado { get; set; }
        public string usuario_creacion { get; set; }
        public string usuario_modificacion { get; set; }
        public decimal es_personalizado { get; set; }
        public string texto_personalizado { get; set; }
        public Nullable<decimal> orden { get; set; }
    
        public virtual PosibleRespuesta PosibleRespuesta { get; set; }
        public virtual Pregunta Pregunta { get; set; }
    }
}
