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
    public class MetodoCalculoManager:GenericManager<MetodoCalculo>
    {
        public MetodoCalculoManager(GenericRepository<MetodoCalculo> repository, Manager manager)
            : base(repository, manager)
        {
        }

        public MetodoCalculoManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }

        public override List<string> Validate(MetodoCalculo element)
        {
            var list= base.Validate(element);
            list.Required(element,t=>t.nombre,"Nombre");
            list.MaxLength(element,t=>t.nombre,20,"Nombre");
            return list;
        }
    }
}
