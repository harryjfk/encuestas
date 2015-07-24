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
    public class LineaProductoEstablecimientoManager:GenericManager<LineaProductoEstablecimiento>
    {
        public LineaProductoEstablecimientoManager(GenericRepository<LineaProductoEstablecimiento> repository, Manager manager) : base(repository, manager)
        {
        }

        public LineaProductoEstablecimientoManager(Entities context, Manager manager) : base(context, manager)
        {
        }

        public override List<string> Validate(LineaProductoEstablecimiento element)
        {
            var list= base.Validate(element);
            list.Required(element,t=>t.IdEstablecimiento,"Establecimiento");
            //list.Required(element,t=>t.IdLineaProducto,"Linea de Producto");
            if (element.IdLineaProducto==null||element.IdLineaProducto<1)
            {
                list.Add("La Línea de Producto es obligatoria.");
            }
          
            return list;
        }
    }
}
