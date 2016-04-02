using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Repositorios;
using Entity;
using PagedList;

namespace Domain.Managers
{
    public class EstablecimientoManager : GenericManager<Establecimiento>
    {
        public EstablecimientoManager(GenericRepository<Establecimiento> repository, Manager manager)
            : base(repository, manager)
        {
        }

        public EstablecimientoManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }
        
        public override OperationResult<Establecimiento> Add(Establecimiento element)
        {
            
            FillIdentificador(element);
            return base.Add(element);
        }

        public void FillIdentificador(Establecimiento element)
        {
            var conf = ConfigurationManager.AppSettings["identificadorInicialEstablecimiento"] ?? "1";
            var last = Get().OrderBy(t => Convert.ToInt32(t.IdentificadorInterno)).LastOrDefault();
            long n = 1;
            long.TryParse(conf, out n);
            if (last != null)
            {
                long.TryParse(last.IdentificadorInterno, out n);
                n++;
            }
            element.IdentificadorInterno = n.ToString().PadLeft(10, '0');
        }

        public IPagedList GetNoAsignadosAnalistas(Query<Establecimiento> query)
        {
            Func<Establecimiento, bool> filter = 
                t => (query.Filter == null || query.Filter(t)) 
                    && t.Activado && t.Ciius.Count>t.CAT_ESTAB_ANALISTA.Count;

            var temp = Manager.Establecimiento.Get(filter, null, query.Order);
            
            if (query.Paginacion != null)
            {
                var list = temp.ToPagedList(query.Paginacion.Page, query.Paginacion.ItemsPerPage);
                query.Elements = list;
                return list;
            }
            else
            {
                var establecimientos = temp.ToArray();
                var list = establecimientos.ToPagedList(1, establecimientos.Any() ? establecimientos.Count() : 1);
                query.Elements = list;
                return list;
            }
        }

        public IPagedList GetNoAsignadosInformantes(Query<Establecimiento> query)
        {
            var temp = Repository.Get(query.Filter, null, query.Order).Where(t => t.IdInformante == null);
            if (query.Paginacion != null)
            {
                var list = temp.ToPagedList(query.Paginacion.Page, query.Paginacion.ItemsPerPage);
                query.Elements = list;
                return list;
            }
            else
            {
                var establecimientos = temp as Establecimiento[] ?? temp.ToArray();
                var list = establecimientos.ToPagedList(1, establecimientos.Any() ? establecimientos.Count() : 1);
                query.Elements = list;
                return list;
            }
        }       

        public override OperationResult<Establecimiento> Delete(Establecimiento element)
        {
            if (element.Encuestas.Count > 0)
                return new OperationResult<Establecimiento>(element) { Errors = new List<string>() { "Hay encuestas relacionadas" }, Success = false };
            return base.Delete(element);
        }

        public override List<string> Validate(Establecimiento element)
        {
            var list = base.Validate(element);
            list.Required(element, t => t.Nombre, "Nombre");
            //list.Required(element, t => t.RazonSocial, "RazonSocial");
            list.Required(element, t => t.Ruc, "Ruc");
            //list.Required(element, t => t.SituacionJuridica, "SituacionJuridica");
            list.Required(element, t => t.Telefono, "Telefono");
            //list.Required(element, t => t.TrabajadoresProduccion, "TrabajadoresProduccion");
            //list.Required(element, t => t.Administrativos, "Administrativos");
            // list.Required(element, t => t.DesviacionDiasTrabajados, "DesviacionDiasTrabajados");
            list.Required(element, t => t.Direccion, "Direccion");
            //list.Required(element, t => t.Fax, "Fax");
            //list.Required(element, t => t.IdentificadorInterno, "IdentificadorInterno");
            list.Required(element, t => t.Ubigeo, "Distrito");
            if(element.IdDepartamento==null ||element.IdDepartamento.Trim().Replace("0","")=="")
                list.Add("El campo Departamento es obligatorio");
            if (element.IdProvincia == null || element.IdProvincia.Trim().Replace("0", "") == "")
                list.Add("El campo Provincia es obligatorio");
            if (element.Ubigeo != null)
            {
                var text = element.Ubigeo.Replace("0", "");
                if (text.Length < 3)
                    list.Add("El campo Distrito es obligatorio");
            }

            list.MaxLength(element, t => t.Nombre, 150, "Nombre");
            list.MaxLength(element, t => t.RazonSocial, 1000, "RazonSocial");
            list.MaxLength(element, t => t.Ruc, 11, "Ruc");
            list.MaxLength(element, t => t.Telefono, 10, "Telefono");
            list.MaxLength(element, t => t.Direccion, 255, "Direccion");
            list.MaxLength(element, t => t.Fax, 100, "Fax");
            list.MaxLength(element, t => t.IdentificadorInterno, 100, "IdentificadorInterno");
            list.ValidRUC(element, t => t.Ruc, "Ruc");

            list.PhoneNumber(element, t => t.Telefono, "Telefono");
            list.PhoneNumber(element, t => t.Fax, "Fax");

            return list;
        }

        public void GetCiiuAsignados(Query<Ciiu> query, long idestablecimiento)
        {
            var manager = Manager;
            var establecimiento = manager.Establecimiento.Find(idestablecimiento);
            if (establecimiento == null) return;
            var ciiu = establecimiento.Ciius.Select(t => t.Ciiu).Where(query.Filter);
            query.Elements = ciiu.ToPagedList(query.Paginacion.Page, query.Paginacion.ItemsPerPage);
        }

        public void AddCiiu(long idCiiu, long idEstablecimiento)
        {
            var manager = Manager;
            var estab = manager.Establecimiento.Find(idEstablecimiento);
            var ciiu = manager.Ciiu.Find(idCiiu);
            if (estab != null && ciiu != null)
            {
                Manager.CiiuEstablecimientoManager.Add(
                        new CiiuEstablecimiento
                        {
                            IdCiiu = idCiiu,
                            IdEstablecimiento = idEstablecimiento
                        });
                manager.Establecimiento.SaveChanges();
            }
        }

        //public void DeleteCiiu(long idCiiu, long idEstablecimiento)
        //{
        //    var manager = Manager;
        //    var estab = manager.Establecimiento.Find(idEstablecimiento);
        //    var ciiu = manager.Ciiu.Find(idCiiu);
        //    if (estab != null && ciiu != null)
        //    {
        //        Manager.CiiuEstablecimientoManager.Delete(ciiuT.Id);
        //        manager.Establecimiento.SaveChanges();
        //    }
        //}

        public void GetCiiuNoAsignados(Query<Ciiu> query, long idEstablecimiento)
        {
            var manager = Manager;
            var establecimiento = manager.Establecimiento.Find(idEstablecimiento);
            if (establecimiento == null) return;
            var ciiu = manager.Ciiu.Get(query.Filter).Where(t => establecimiento.Ciius.Select(x => x.Ciiu).All(h => h.Id != t.Id));
            query.Elements = ciiu.ToPagedList(query.Paginacion.Page, query.Paginacion.ItemsPerPage);
        }

        public void GetAllCiiu(Query<Ciiu> query, long idEstablecimiento)
        {
            var manager = Manager;
            var establecimiento = manager.Establecimiento.Find(idEstablecimiento);
            if (establecimiento == null) return;
            var asig = establecimiento.Ciius;
            var ciius = asig.Select(t => t.Ciiu);            
            var all = ciius.Union(manager.Ciiu.Get(t=>t.Activado));
            var ciiu = all.Where(query.Filter).ToPagedList(query.Paginacion.Page, query.Paginacion.ItemsPerPage);
            foreach (var c in ciiu)
            {
                if (ciius.Any(t => t.Id == c.Id))
                    c.Seleccionado = true;
            }
            query.Elements = ciiu;
        }

        public void ToggleCiiu(long idEstablecimiento, long idCiiu)
        {
            var establecimiento = Manager.Establecimiento.Find(idEstablecimiento);
            var ciiu = Manager.Ciiu.Find(idCiiu);
            if (establecimiento != null && ciiu != null)
            {
                var ciiuT = establecimiento.Ciius.FirstOrDefault(t => t.Ciiu.Id == idCiiu);
                if (ciiuT == null)
                {
                    Manager.CiiuEstablecimientoManager.Add(
                        new CiiuEstablecimiento
                        {
                            IdCiiu = idCiiu,
                            IdEstablecimiento = idEstablecimiento
                        });
                }
                else
                {
                    Manager.CiiuEstablecimientoManager.Delete(ciiuT.Id);
                }
                Manager.Establecimiento.SaveChanges();
            }
        }
    }
}
