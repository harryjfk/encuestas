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
    
    public partial class ViewProcentajeEncuestaExtadistica
    {
        public long id_encuesta { get; set; }
        public decimal estado_encuesta { get; set; }
        public long id_establecimiento { get; set; }
        public System.DateTime fecha { get; set; }
        public long id_ciiu { get; set; }
        public decimal id_analista { get; set; }
        public decimal estado_validacion { get; set; }
        public string codigo_ciiu { get; set; }
        public string nombre_ciiu { get; set; }
        public string login_analista { get; set; }
    }
}
