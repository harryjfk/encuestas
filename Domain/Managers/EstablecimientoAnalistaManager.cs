using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Repositorios;
using Entity;

namespace Domain.Managers
{
    public class EstablecimientoAnalistaManager:GenericManager<EstablecimientoAnalista>
    {
        public EstablecimientoAnalistaManager(GenericRepository<EstablecimientoAnalista> repository, Manager manager) : base(repository, manager)
        {
        }

        public EstablecimientoAnalistaManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }

      

        public override List<string> Validate(EstablecimientoAnalista element)
        {
            var list= base.Validate(element);
            list.Required(element,t=>t.id_ciiu,"CIIU");
            list.Required(element, t => t.id_establecimiento, "Establecimiento");
            list.Required(element, t => t.id_analista, "Analista");
            return list;
        }
    }
}
