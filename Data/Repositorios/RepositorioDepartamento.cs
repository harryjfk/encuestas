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
    public class RepositorioDepartamento : IRepositorioDepartamento
    {
        public IPagedList<Entity.Departamento> Get(Paginacion paginacion = null)
        {
            try
            {
                var connection = Conexion.CrearConexion().Crear();
                var query = string.Format("SELECT DISTINCT SUBSTRING({0}.{2},1,2) as {2}, {0}.{1} FROM {0}",
                    ConfigurationManager.AppSettings["ubigeo"] ?? "UBIGEO"
                    , ConfigurationManager.AppSettings["ubigeo.departamento"] ?? "DEPARTAMENTO"
                    , ConfigurationManager.AppSettings["ubigeo.ubigeo"] ?? "UBIGEO");
                var result = Operacion.Ejecutar(connection, query);
                var list = new List<Departamento>();
                if (result != null)
                {
                    list.AddRange(from DataRowView item in result
                                  select new Departamento
                                  {
                                      Codigo = Convert.ToString(item[ConfigurationManager.AppSettings["ubigeo.ubigeo"] ?? "UBIGEO"]),
                                      Nombre = Convert.ToString(item[ConfigurationManager.AppSettings["ubigeo.departamento"] ?? "DEPARTAMENTO"]),

                                  });
                }
                if (paginacion == null)
                    return new PagedList<Departamento>(list, 1, !list.Any() ? 1 : list.Count);
                paginacion.Validate();
                return list.ToPagedList(paginacion.Page, paginacion.ItemsPerPage);
            }
            catch (Exception)
            {
                return new PagedList<Departamento>( new List<Departamento>(),1,1);
            }
        }

        public Entity.Departamento Find(string codigo)
        {
            try
            {
                var connection = Conexion.CrearConexion().Crear();
                var query = string.Format("SELECT * FROM {0} WHERE {0}.{1} LIKE @codigo",
                    ConfigurationManager.AppSettings["ubigeo"] ?? "UBIGEO",
                    ConfigurationManager.AppSettings["ubigeo.ubigeo"] ?? "UBIGEO");
                var result = Operacion.Ejecutar(connection, query,
                    new SqlParameter("@codigo", string.Format("{0}____", codigo)));
                var list = new List<Departamento>();
                if (result != null)
                {
                    list.AddRange(from DataRowView item in result
                                  select new Departamento
                                  {
                                      Codigo = Convert.ToString(item[ConfigurationManager.AppSettings["ubigeo.ubigeo"] ?? "UBIGEO"]).Take(2).Aggregate("", (t, h) => t + h),
                                      Nombre = Convert.ToString(item[ConfigurationManager.AppSettings["ubigeo.departamento"] ?? "DEPARTAMENTO"]),
                                  });
                }
                return list.FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IPagedList<Provincia> GetProvincias(string codigoDepartamento, Paginacion paginacion = null)
        {
            try
            {
                var connection = Conexion.CrearConexion().Crear();
                var query = string.Format("SELECT DISTINCT SUBSTRING({0}.{1},1,4) AS COD,{0}.{2} FROM {0} WHERE {0}.{1} LIKE @codigo",
                    ConfigurationManager.AppSettings["ubigeo"] ?? "UBIGEO",
                    ConfigurationManager.AppSettings["ubigeo.ubigeo"] ?? "UBIGEO"
                    , ConfigurationManager.AppSettings["ubigeo.provincia"] ?? "PROVINCIA");
                var result = Operacion.Ejecutar(connection, query,
                    new SqlParameter("@codigo", string.Format("{0}____", codigoDepartamento)));
                var list = new List<Provincia>();
                if (result != null)
                {
                    list.AddRange(from DataRowView item in result
                                  select new Provincia
                                  {
                                      Codigo = Convert.ToString(item["COD"]).Take(4).Aggregate("", (t, h) => t + h),
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
    }
}
