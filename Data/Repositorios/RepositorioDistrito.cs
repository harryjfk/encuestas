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
    public class RepositorioDistrito : IRepositorioDistrito
    {
        public IPagedList<Entity.Distrito> Get(Paginacion paginacion = null)
        {
            try
            {
                var connection = Conexion.CrearConexion().Crear();
                var query = string.Format("SELECT DISTINCT SUBSTRING({0}.{2},1,6) as COD, {0}.{1},{0}.{2} FROM {0}",
                    ConfigurationManager.AppSettings["ubigeo"] ?? "UBIGEO"
                    , ConfigurationManager.AppSettings["ubigeo.distrito"] ?? "DISTRITO"
                    , ConfigurationManager.AppSettings["ubigeo.ubigeo"] ?? "UBIGEO");
                var result = Operacion.Ejecutar(connection, query);
                var list = new List<Distrito>();
                if (result != null)
                {
                    list.AddRange(from DataRowView item in result
                                  select new Distrito
                                  {
                                      Codigo = Convert.ToString(item["COD"]),
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

        public Entity.Distrito Find(string codigo)
        {
            try
            {
                var connection = Conexion.CrearConexion().Crear();
                var query = string.Format("SELECT * FROM {0} WHERE {0}.{1} LIKE @codigo",
                    ConfigurationManager.AppSettings["ubigeo"] ?? "UBIGEO",
                    ConfigurationManager.AppSettings["ubigeo.ubigeo"] ?? "UBIGEO");
                var result = Operacion.Ejecutar(connection, query,
                    new SqlParameter("@codigo", string.Format("{0}", codigo)));
                var list = new List<Distrito>();
                if (result != null)
                {
                    list.AddRange(from DataRowView item in result
                                  select new Distrito
                                  {
                                      Codigo = Convert.ToString(item[ConfigurationManager.AppSettings["ubigeo.ubigeo"] ?? "UBIGEO"])/*.Skip(4).Take(2).Aggregate("", (t, h) => t + h)*/,
                                      Nombre = Convert.ToString(item[ConfigurationManager.AppSettings["ubigeo.distrito"] ?? "DISTRITO"]),
                                      CodigoCompleto = Convert.ToString(item[ConfigurationManager.AppSettings["ubigeo.ubigeo"] ?? "UBIGEO"])
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
