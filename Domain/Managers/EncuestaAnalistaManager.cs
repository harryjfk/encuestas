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
    public class EncuestaAnalistaManager : GenericManager<EncuestaAnalista>
    {
        public EncuestaAnalistaManager(GenericRepository<EncuestaAnalista> repository, Manager manager)
            : base(repository, manager)
        {
        }

        public EncuestaAnalistaManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }

       

        
        public override List<string> Validate(EncuestaAnalista element)
        {
            var list= base.Validate(element);
            list.Required(element,t=>t.id_ciiu,"CIIU");
            list.Required(element, t => t.id_analista, "Analista");
            list.Required(element, t => t.id_encuesta, "Encuesta");
            list.Required(element, t => t.orden, "Orden");
            return list;
        }
    }
}
