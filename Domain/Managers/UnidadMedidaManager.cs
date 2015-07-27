using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Repositorios;
using Entity;
using PagedList;

namespace Domain.Managers
{
    public class UnidadMedidaManager : GenericManager<UnidadMedida>
    {
        public UnidadMedidaManager(GenericRepository<UnidadMedida> repository, Manager manager)
            : base(repository, manager)
        {
        }

        public override OperationResult<UnidadMedida> Delete(UnidadMedida element)
        {
            if (element.LineasProductoUnidadMedida.Count > 0)
                return new OperationResult<UnidadMedida>(element) { Errors = new List<string>() { "Hay líneas de producto relacionadas" }, Success = false };
            return base.Delete(element);
        }

        public UnidadMedidaManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }

        public void GetAsignadas(Query<UnidadMedida> query, long idLineaProducto)
        {
            query.Filter = query.Filter ?? new Func<UnidadMedida, bool>(t => true);
           // Get(query);
            var lineaProd = Manager.LineaProducto.Find(idLineaProducto);
            if (lineaProd == null) return;
            var all = Manager.UnidadMedida.Get(t => t.Activado == true).ToList();
            query.Elements = all.Any() ? all.ToPagedList(query.Paginacion.Page, query.Paginacion.ItemsPerPage) : new PagedList<UnidadMedida>(all, 1, 1);
            var list = lineaProd.LineasProductoUnidadMedida.Select(t => t.id_unidad_medida).ToList();
            foreach (var um in query.Elements.Where(um => list.Contains(um.Id)))
                um.Asignado = true;
        }

        public override List<string> Validate(UnidadMedida element)
        {
            var list = base.Validate(element);
            list.Required(element, t => t.Abreviatura, "Abreviatura");
            list.Required(element, t => t.Descripcion, "Descripcion");
            list.MaxLength(element, t => t.Descripcion, 100, "Descripcion");
            list.MaxLength(element, t => t.Abreviatura, 5, "Descripcion");
            return list;
        }

        public override void Seed()
        {

        }
    }
}
