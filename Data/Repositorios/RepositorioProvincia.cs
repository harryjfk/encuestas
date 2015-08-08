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
    public class RepositorioProvincia : IRepositorioProvincia
    {
        public IPagedList<Entity.Provincia> Get(Paginacion paginacion = null)
        {
            try
            {
                var connection = Conexion.CrearConexion().Crear();
                var query = string.Format("SELECT DISTINCT SUBSTRING({0}.{2},1,4) as {2}, {0}.{1} FROM {0}",
                    ConfigurationManager.AppSettings["ubigeo"] ?? "UBIGEO"
                    , ConfigurationManager.AppSettings["ubigeo.provincia"] ?? "PROVINCIA"
                    , ConfigurationManager.AppSettings["ubigeo.ubigeo"] ?? "UBIGEO");
                var result = Operacion.Ejecutar(connection, query);
                var list = new List<Provincia>();
                if (result != null)
                {
                    list.AddRange(from DataRowView item in result
                                  select new Provincia
                                  {
                                      Codigo = Convert.ToString(item[ConfigurationManager.AppSettings["ubigeo.ubigeo"] ?? "UBIGEO"]),
                                      Nombre = Convert.ToString(item[ConfigurationManager.AppSettings["ubigeo.provincia"] ?? "PROVINCIA"]),
                                      
                                  });
                }
                if (paginacion == null)
                    return new PagedList<Provincia>(list, 1, !list.Any() ? 1 : list.Count);
                paginacion.Validate();
                return list.ToPagedList(paginacion.Page, paginacion.ItemsPerPage);
            }
            catch (Exception)
            {
                return new PagedList<Provincia>( new List<Provincia>(),1,1);
            }
        }

        public Entity.Provincia Find(string codigo)
        {
            try
            {
                var connection = Conexion.CrearConexion().Crear();
                var query = string.Format("SELECT * FROM {0} WHERE {0}.{1} LIKE @codigo",
                    ConfigurationManager.AppSettings["ubigeo"] ?? "UBIGEO",
                    ConfigurationManager.AppSettings["ubigeo.ubigeo"] ?? "UBIGEO");
                var result = Operacion.Ejecutar(connection, query,
                    new SqlParameter("@codigo", string.Format("{0}__", codigo)));
                var list = new List<Provincia>();
                if (result != null)
                {
                    list.AddRange(from DataRowView item in result
                                  select new Provincia
                                  {
                                      Codigo = Convert.ToString(item[ConfigurationManager.AppSettings["ubigeo.ubigeo"] ?? "UBIGEO"]).Take(4).Aggregate("", (t, h) => t + h),
                                      Nombre = Convert.ToString(item[ConfigurationManager.AppSettings["ubigeo.provincia"] ?? "PROVINCIA"]),
                                  });
                }
                return list.FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IPagedList<Distrito> GetDistritos(string codigoProvincia, Paginacion paginacion = null)
        {
            try
            {
                var connection = Conexion.CrearConexion().Crear();
                var query = string.Format("SELECT * FROM {0} WHERE {0}.{1} LIKE @codigo",
                    ConfigurationManager.AppSettings["ubigeo"] ?? "UBIGEO",
                    ConfigurationManager.AppSettings["ubigeo.ubigeo"] ?? "UBIGEO");
                var result = Operacion.Ejecutar(connection, query,
                    new SqlParameter("@codigo", string.Format("{0}__", codigoProvincia)));
                var list = new List<Distrito>();
                if (result != null)
                {
                    list.AddRange(from DataRowView item in result
                                  select new Distrito
                                  {
                                      Codigo = Convert.ToString(item[ConfigurationManager.AppSettings["ubigeo.ubigeo"] ?? "UBIGEO"])/*.Skip(4).Take(2).Aggregate("", (t, h) => t + h)*/,
                                      Nombre = Convert.ToString(item[ConfigurationManager.AppSettings["ubigeo.distrito"] ?? "DISTRITO"]),
                                      CodigoCompleto = Convert.ToString(item[ConfigurationManager.AppSettings["ubigeo.ubigeo"] ?? "UBIGEO"]),

                                  });
                }
                if (paginacion == null)
                    return new PagedList<Distrito>(list, 1, !list.Any() ? 1 : list.Count);
                paginacion.Validate();
                return list.ToPagedList(paginacion.Page, paginacion.ItemsPerPage);
            }
            catch (Exception)
            {
                return new PagedList<Distrito>( new List<Distrito>(),1,1);
            }
        }
    }
}
