using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Repositorios;
using Entity;

namespace Domain.Managers
{
    public class LineaProductoManager:GenericManager<LineaProducto>
    {
        public LineaProductoManager(GenericRepository<LineaProducto> repository, Manager manager) : base(repository, manager)
        {
        }

        public LineaProductoManager(Entities context, Manager manager) : base(context, manager)
        {
        }

        public bool ToggleUnidadMedida(long idLineaProducto, long idUnidadMedida, long? idUnidadConversion = null, decimal? factorConversion = null)
        {
            var manager = Manager;
            var unidadMedida = manager.UnidadMedida.Find(idUnidadMedida);
            var lineaProducto = manager.LineaProducto.Find(idLineaProducto);
            if (unidadMedida == null || lineaProducto == null) return false;
            var asigned = unidadMedida.LineasProductoUnidadMedida.FirstOrDefault(t => t.id_linea_producto == lineaProducto.Id);
            idUnidadConversion = idUnidadConversion > 0 ? idUnidadConversion : null;
            if (asigned == null)
            {
                //unidadMedida.LineasProducto.Add(lineaProducto);
                var lin = new LineaProductoUnidadMedida()
                {
                    id_linea_producto = lineaProducto.Id,
                    id_unidad_medida = unidadMedida.Id,
                    id_unidad_conversion = idUnidadConversion,
                    factor_conversion = factorConversion.GetValueOrDefault()
                };
                manager.LineaProductoUnidadMedidaManager.Add(lin);
                manager.LineaProductoUnidadMedidaManager.SaveChanges();
            }
            else
            {
               // unidadMedida.LineasProducto.Remove(lineaProducto);
                //lineaProducto.UnidadesMedida.Remove(unidadMedida);
                //asigned.UnidadMedida.
                var any =
                    Manager.AñoBaseManager.Get(
                        t =>
                            t.id_linea_producto == asigned.id_linea_producto &&
                            t.id_unidad_medida == asigned.id_unidad_medida).Any();
                if (any) return false;
                manager.LineaProductoUnidadMedidaManager.Delete(asigned.Id);
                manager.UnidadMedida.SaveChanges();
            }
            return true;
        }

        public override OperationResult<LineaProducto> Delete(LineaProducto element)
        {
            if (element.LineasProductoEstablecimiento.Count > 0)
                return new OperationResult<LineaProducto>(element) { Errors = new List<string>() { "Hay establecimientos relacionados" }, Success = false };
            return base.Delete(element);
        }

        public override List<string> Validate(LineaProducto element)
        {
            var list= base.Validate(element);
            list.Required(element,t=>t.Nombre,"Nombre");
            list.Required(element,t=>t.Codigo,"Codigo");
            list.Required(element,t=>t.IdCiiu,"CIIU");
            
            var lengthCodigo = element.Codigo.ToString().Length;
            if (!(lengthCodigo == 7 || lengthCodigo == 10))
            {
                list.Add(string.Format("El campo \"{0}\" no cuenta con la cantidad de caracteres válidos", "Código"));
            }
            
            list.MaxLength(element,t=>t.Nombre,255,"Codigo");
            return list;
        }
    }
}
