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
    public class PreguntaManager:GenericManager<Pregunta>
    {
        public PreguntaManager(GenericRepository<Pregunta> repository, Manager manager) : base(repository, manager)
        {
        }

        public PreguntaManager(Entities context, Manager manager) : base(context, manager)
        {
        }

        public override OperationResult<Pregunta> Delete(Pregunta element)
        {
            if (element.Encuesta!=null)
                return new OperationResult<Pregunta>(element) { Errors = new List<string>() { "Hay encuestas relacionadas" }, Success = false };
            return base.Delete(element);
        }

        public override List<string> Validate(Pregunta element)
        {
            var list= base.Validate(element);
            //list.Required(element,t=>t.Texto,"Texto");
            list.Required(element,t=>t.Estado,"Estado");

            list.MaxLength(element, t => t.Texto, 1000, "Texto");
            list.MaxLength(element, t => t.orden, 4, "Órden");

            return list;
        }
    }
}
