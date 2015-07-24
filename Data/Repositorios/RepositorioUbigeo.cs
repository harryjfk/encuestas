using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Contratos;
using Data.DataAccess;
using Entity;
using PagedList;

namespace Data.Repositorios
{
    public class RepositorioUbigeo : IRepositorioUbigeo
    {
        public IPagedList<Entity.Ubigeo> Get(Paginacion paginacion = null)
        {
            try
            {
                var connection = Conexion.CrearConexion().Crear();
                var query = string.Format("SELECT * FROM {0}",
                    ConfigurationManager.AppSettings["ubigeo"] ?? "UBIGEO");
                var result = Operacion.Ejecutar(connection, query);
                var list = new List<Ubigeo>();
                if (result != null)
                {
                    list.AddRange(from DataRowView item in result
                                  select new Ubigeo
                                  {
                                      Codigo = Convert.ToString(item[ConfigurationManager.AppSettings["ubigeo.ubigeo"] ?? "UBIGEO"]),
                                      Departamento = Convert.ToString(item[ConfigurationManager.AppSettings["ubigeo.departamento"] ?? "DEPARTAMENTO"]),
                                      Provincia = Convert.ToString(item[ConfigurationManager.AppSettings["ubigeo.provincia"] ?? "PROVINCIA"]),
                                      Distrito = Convert.ToString(item[ConfigurationManager.AppSettings["ubigeo.distrito"] ?? "DISTRITO"])
                                  });
                }
                if (paginacion == null)
                    return new PagedList<Ubigeo>(list, 1, !list.Any() ? 1 : list.Count);
                paginacion.Validate();
                return list.ToPagedList(paginacion.Page, paginacion.ItemsPerPage);
            }
            catch (Exception)
            {
                return new PagedList<Ubigeo>( new List<Ubigeo>(),1,1);
            }
        }

        public Entity.Ubigeo Find(string codigo)
        {
            try
            {
                var connection = Conexion.CrearConexion().Crear();
                var query = string.Format("SELECT * FROM {0} WHERE {0}.{1} = @codigo",
                    ConfigurationManager.AppSettings["ubigeo"] ?? "UBIGEO",
                    ConfigurationManager.AppSettings["ubigeo.ubigeo"] ?? "UBIGEO");
                var result = Operacion.Ejecutar(connection, query,
                    new SqlParameter("@codigo", codigo));
                var list = new List<Ubigeo>();
                if (result != null)
                {
                    list.AddRange(from DataRowView item in result
                                  select new Ubigeo
                                  {
                                      Codigo = Convert.ToString(item[ConfigurationManager.AppSettings["ubigeo.ubigeo"] ?? "UBIGEO"]),
                                      Departamento = Convert.ToString(item[ConfigurationManager.AppSettings["ubigeo.departamento"] ?? "DEPARTAMENTO"]),
                                      Provincia = Convert.ToString(item[ConfigurationManager.AppSettings["ubigeo.provincia"] ?? "PROVINCIA"]),
                                      Distrito = Convert.ToString(item[ConfigurationManager.AppSettings["ubigeo.distrito"] ?? "DISTRITO"])
                                  });
                }
                return list.FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
