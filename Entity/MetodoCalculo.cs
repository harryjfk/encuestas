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
    
    public partial class MetodoCalculo
    {
        public MetodoCalculo()
        {
            this.CAT_CIIU = new HashSet<Ciiu>();
        }
    
        public long Id { get; set; }
        public string nombre { get; set; }
        public decimal estado { get; set; }
        public decimal registro_obligatorio { get; set; }
    
        public virtual ICollection<Ciiu> CAT_CIIU { get; set; }
    }
}
