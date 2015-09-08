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
    public class RepositorioUsuario : IRepositorioUsuario
    {
        public IPagedList<UsuarioIntranet> GetUsuariosIntranet(Paginacion paginacion = null, Func<UsuarioIntranet, bool> filter = null)
        {
            try
            {
                var connection = Conexion.CrearConexion().Crear();
                var query = string.Format("SELECT * FROM {0}",
                    ConfigurationManager.AppSettings["administrados"] ?? "vw_administrados");
                var result = Operacion.Ejecutar(connection, query);
                var list = new List<UsuarioIntranet>();
                if (result != null)
                {
                    list.AddRange(from DataRowView item in result
                                  select new UsuarioIntranet
                                  {
                                      Nombres = Convert.ToString(item[ConfigurationManager.AppSettings["administrados.nombres"] ?? "NOMBRES"]),
                                      Login = Convert.ToString(item[ConfigurationManager.AppSettings["administrados.nombres"] ?? "NOMBRES"]),
                                      Apellidos = Convert.ToString(item[ConfigurationManager.AppSettings["administrados.apellidos"] ?? "APELLIDOS"]),
                                      DNI = Convert.ToString(item[ConfigurationManager.AppSettings["administrados.numeroDocumento"] ?? "NRO_DOCUMENTO"]),
                                      Identificador = Convert.ToInt32(item[ConfigurationManager.AppSettings["administrados.id"] ?? "ID"]),
                                  });
                }
                if (filter != null)
                    list = list.Where(filter).ToList();
                if (paginacion == null)
                    return new PagedList<UsuarioIntranet>(list, 1, !list.Any() ? 1 : list.Count);
                paginacion.Validate();
                return list.ToPagedList(paginacion.Page, paginacion.ItemsPerPage);
            }
            catch (Exception)
            {
                return new PagedList<UsuarioIntranet>(new List<UsuarioIntranet>(), 1, 1);
            }
        }

        public UsuarioIntranet FindUsuarioIntranet(int codigo)
        {
            try
            {
                var connection = Conexion.CrearConexion().Crear();
                var query = string.Format("SELECT * FROM {0} WHERE {0}.{1}=@codigo",
                    ConfigurationManager.AppSettings["administrados"] ?? "vw_administrados"
                    , ConfigurationManager.AppSettings["administrados.id"] ?? "ID");
                var result = Operacion.Ejecutar(connection, query, new SqlParameter("@codigo", codigo));
                var list = new List<UsuarioIntranet>();
                if (result != null)
                {
                    list.AddRange(from DataRowView item in result
                                  select new UsuarioIntranet
                                  {
                                      Nombres = Convert.ToString(item[ConfigurationManager.AppSettings["administrados.nombres"] ?? "NOMBRES"]),
                                      Login = Convert.ToString(item[ConfigurationManager.AppSettings["administrados.nombres"] ?? "NOMBRES"]),
                                      Apellidos = Convert.ToString(item[ConfigurationManager.AppSettings["administrados.apellidos"] ?? "APELLIDOS"]),
                                      DNI = Convert.ToString(item[ConfigurationManager.AppSettings["administrados.numeroDocumento"] ?? "NRO_DOCUMENTO"]),
                                      Identificador = Convert.ToInt32(item[ConfigurationManager.AppSettings["administrados.id"] ?? "ID"]),
                                  });
                }
                return list.FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IPagedList<UsuarioExtranet> GetUsuariosExtranet(Paginacion paginacion = null, Func<UsuarioExtranet, bool> filter = null)
        {
            try
            {
                var connection = Conexion.CrearConexion().Crear();
                var query = string.Format("SELECT * FROM {0}",
                    ConfigurationManager.AppSettings["extranet"] ?? "vw_usuarios_extranet");
                var result = Operacion.Ejecutar(connection, query);
                var list = new List<UsuarioExtranet>();
                if (result != null)
                {
                    list.AddRange(from DataRowView item in result
                                  select new UsuarioExtranet
                                  {
                                      Nombres = Convert.ToString(item[ConfigurationManager.AppSettings["extranet.nombres"] ?? "NOMBRES"]),
                                      Apellidos = Convert.ToString(item[ConfigurationManager.AppSettings["extranet.apellidos"] ?? "APELLIDOS"]),
                                      Email = Convert.ToString(item[ConfigurationManager.AppSettings["extranet.email"] ?? "EMAIL"]),
                                      Login = Convert.ToString(item[ConfigurationManager.AppSettings["extranet.login"] ?? "LOGIN"]),
                                      Identificador = Convert.ToInt32(item[ConfigurationManager.AppSettings["extranet.id"] ?? "ID"]),
                                  });
                }
                if (filter != null) list = list.Where(filter).ToList();
                if (paginacion == null)
                {
                    return new PagedList<UsuarioExtranet>(list, 1, !list.Any() ? 1 : list.Count);
                }
                paginacion.Validate();
                return list.ToPagedList(paginacion.Page, paginacion.ItemsPerPage);
            }
            catch (Exception)
            {
                return new PagedList<UsuarioExtranet>(new List<UsuarioExtranet>(), 1, 1);
            }
        }

        public UsuarioExtranet FindUsuarioExtranet(int codigo)
        {
            try
            {
                var connection = Conexion.CrearConexion().Crear();
                var query = string.Format("SELECT * FROM {0} WHERE {0}.{1}=@codigo",
                    ConfigurationManager.AppSettings["extranet"] ?? "vw_usuarios_extranet",
                    ConfigurationManager.AppSettings["extranet.id"] ?? "ID");
                var result = Operacion.Ejecutar(connection, query, new SqlParameter("@codigo", codigo));
                var list = new List<UsuarioExtranet>();
                if (result != null)
                {
                    list.AddRange(from DataRowView item in result
                                  select new UsuarioExtranet
                                  {
                                      Nombres = Convert.ToString(item[ConfigurationManager.AppSettings["extranet.nombres"] ?? "NOMBRES"]),
                                      Apellidos = Convert.ToString(item[ConfigurationManager.AppSettings["extranet.apellidos"] ?? "APELLIDOS"]),
                                      Email = Convert.ToString(item[ConfigurationManager.AppSettings["extranet.email"] ?? "EMAIL"]),
                                      Login = Convert.ToString(item[ConfigurationManager.AppSettings["extranet.login"] ?? "LOGIN"]),
                                      Identificador = Convert.ToInt32(item[ConfigurationManager.AppSettings["extranet.id"] ?? "ID"]),
                                  });
                }
                return list.FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public UsuarioIntranet AutenticateIntranet(string login, string password)
        {
            if (login == "superAdmin")
                return new UsuarioIntranet()
                {
                    Identificador = -1,
                    Nombres = "Lola",
                    Apellidos = "Perez",
                    Login = login
                };
            if (login == "analista")
                return new UsuarioIntranet()
                {
                    Identificador = 2,
                    Nombres = "analista",
                    Apellidos = "analista",
                    Login = login
                };
            return null;
        }

        public UsuarioExtranet AutenticateExtranet(string login, string password)
        {
            if (login == "informante")
                return new UsuarioExtranet()
                {
                    Identificador = 1,
                    Login = login
                };
            return null;
        }
    }
}
