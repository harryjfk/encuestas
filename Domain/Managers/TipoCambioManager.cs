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
    public class TipoCambioManager:GenericManager<TipoCambio>
    {
        public TipoCambioManager(GenericRepository<TipoCambio> repository, Manager manager)
            : base(repository, manager)
        {
        }

        public TipoCambioManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }

        public override List<string> Validate(TipoCambio element)
        {
            var list= base.Validate(element);
            list.Required(element, t => t.tipo_cambio_compra, "Tipo de Cambio Compra");
            list.Required(element, t => t.tipo_cambio_ventas, "Tipo de Cambio Venta");
            return list;
        }

        public void Generate()
        {
            var now = DateTime.Now;
            for (int i = 1; i < 13; i++)
            {
                var element = Get(t => t.fecha.Month == i && t.fecha.Year == now.Year).FirstOrDefault();
                if (element == null)
                {
                    element = new TipoCambio()
                    {
                        fecha = new DateTime(now.Year,i,1),
                        Activado = true
                    };
                    Add(element);
                }
                SaveChanges();
            }
        }
    }
}
