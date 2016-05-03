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
    public class AuditoriaManager:GenericManager<Auditoria>
    {
        public AuditoriaManager(GenericRepository<Auditoria> repository, Manager manager) : base(repository, manager)
        {
        }

        public AuditoriaManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }

      

        public override List<string> Validate(Auditoria element)
        {
            var list= base.Validate(element);
            list.Required(element,t=>t.id_encuesta,"Encuesta");
            list.Required(element, t => t.fecha, "Fecha");
            list.Required(element, t => t.accion, "Acción");
            list.Required(element, t => t.usuario, "Usuario");
            return list;
        }
    }
}
