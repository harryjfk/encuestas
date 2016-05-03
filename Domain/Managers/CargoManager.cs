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
    public class CargoManager:GenericManager<Cargo>
    {
        public CargoManager(GenericRepository<Cargo> repository, Manager manager) : base(repository, manager)
        {
        }

        public CargoManager(Entities context, Manager manager) : base(context, manager)
        {
        }

        public override OperationResult<Cargo> Delete(Cargo element)
        {
            if (element.Contactos.Count > 0)
                return new OperationResult<Cargo>(element) {Errors = new List<string>() {"Hay registros relacionados"},Success = false};
            return base.Delete(element);
        }

        public override List<string> Validate(Cargo element)
        {
            var list= base.Validate(element);
            list.Required(element,t=>t.Nombre,"Nombre");
            list.MaxLength(element,t=>t.Nombre,50,"Nombre");
            return list;
        }
    }
}
