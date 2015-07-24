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
    public class EncuestaManager:GenericManager<Encuesta>
    {
        public EncuestaManager(GenericRepository<Encuesta> repository, Manager manager) : base(repository, manager)
        {
        }

        public EncuestaManager(Entities context, Manager manager) : base(context, manager)
        {
        }

        public override List<string> Validate(Encuesta element)
        {
            var list= base.Validate(element);
            list.Required(element,t=>t.IdEstablecimiento,"Establecimiento");
            list.Required(element,t=>t.Fecha,"Fecha");

            list.MaxLength(element,t=>t.Justificacion,1000,"Justificacion");
            return list;
        }
    }
}
