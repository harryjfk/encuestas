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
        UsuarioIntranetService.UsuarioIntranetService miUsuarioService;
        UsuarioExtranetService.UsuarioExtranetService usuarioExtranetService;

        public IPagedList<UsuarioIntranet> GetUsuariosIntranetAnalista(Paginacion paginacion = null, Func<UsuarioIntranet, bool> filter = null)
        {
            try
            {
                var list = new List<UsuarioIntranet>();
                if (miUsuarioService == null)
                {
                    miUsuarioService = new UsuarioIntranetService.UsuarioIntranetService();
                }
                
                foreach (var item in miUsuarioService.GetUsuarios(int.Parse(ConfigurationManager.AppSettings["idrol_analista"]), true, int.Parse(ConfigurationManager.AppSettings["IdApp"]), true, 1, true, 10, true))
                {                    
                    UsuarioIntranet obj = new UsuarioIntranet();
                    obj.Identificador = item.codigotrabajador;
                    obj.Nombres = item.nombres;
                    obj.Apellidos = item.apellidos;
                    obj.DNI = item.dni;
                    obj.IdRol = item.idrol;
                    list.Add(obj);                    
                }

                int listcount = list.Count;

                if (filter != null)
                    list = list.Where(filter).ToList();
                if (paginacion == null)
                    return new PagedList<UsuarioIntranet>(list, 1, !list.Any() ? 1 : listcount);
                paginacion.Validate();
                return list.ToPagedList(paginacion.Page, paginacion.ItemsPerPage);
            }
            catch (Exception)
            {
                return new PagedList<UsuarioIntranet>(new List<UsuarioIntranet>(), 1, 1);
            }
        }

        public IPagedList<UsuarioIntranet> GetUsuariosIntranetAdministrador(Paginacion paginacion = null, Func<UsuarioIntranet, bool> filter = null)
        {
            try
            {
                var list = new List<UsuarioIntranet>();
                if (miUsuarioService == null)
                {
                    miUsuarioService = new UsuarioIntranetService.UsuarioIntranetService();
                }
                foreach (var item in miUsuarioService.GetUsuarios(int.Parse(ConfigurationManager.AppSettings["idrol_administrador"]), true, int.Parse(ConfigurationManager.AppSettings["IdApp"]), true, 1, true, 10, true))
                {
                    UsuarioIntranet obj = new UsuarioIntranet();
                    obj.Identificador = item.codigotrabajador;
                    obj.Nombres = item.nombres;
                    obj.Apellidos = item.apellidos;
                    obj.DNI = item.dni;
                    obj.IdRol = item.idrol;
                    list.Add(obj);
                }


                int listcount = list.Count;

                if (filter != null)
                    list = list.Where(filter).ToList();
                if (paginacion == null)
                    return new PagedList<UsuarioIntranet>(list, 1, !list.Any() ? 1 : listcount);
                paginacion.Validate();
                return list.ToPagedList(paginacion.Page, paginacion.ItemsPerPage);
            }
            catch (Exception)
            {
                return new PagedList<UsuarioIntranet>(new List<UsuarioIntranet>(), 1, 1);
            }
        }

        public IPagedList<UsuarioIntranet> GetUsuariosIntranet(Paginacion paginacion = null, Func<UsuarioIntranet, bool> filter = null)
        {
            try
            {
                var list = new List<UsuarioIntranet>();
                //var connection = Conexion.CrearConexion().Crear();
                //var query = string.Format("SELECT * FROM {0}",
                //    ConfigurationManager.AppSettings["administrados"] ?? "vw_administrados");
                //var result = Operacion.Ejecutar(connection, query);
                //if (result != null)
                //{
                //    list.AddRange(from DataRowView item in result
                //                  select new UsuarioIntranet
                //                  {
                //                      Nombres = Convert.ToString(item[ConfigurationManager.AppSettings["administrados.nombres"] ?? "NOMBRES"]),
                //                      Login = Convert.ToString(item[ConfigurationManager.AppSettings["administrados.nombres"] ?? "NOMBRES"]),
                //                      Apellidos = Convert.ToString(item[ConfigurationManager.AppSettings["administrados.apellidos"] ?? "APELLIDOS"]),
                //                      DNI = Convert.ToString(item[ConfigurationManager.AppSettings["administrados.numeroDocumento"] ?? "NRO_DOCUMENTO"]),
                //                      Identificador = Convert.ToInt32(item[ConfigurationManager.AppSettings["administrados.id"] ?? "ID"]),
                //                  });
                //}

                ////list.Add(new UsuarioIntranet()
                ////{
                ////    Nombres = "Usuario",
                ////    Apellidos = "Analista",
                ////    Login = "analista",
                ////    DNI = "48490453",
                ////    Identificador = 2
                ////});
                UsuarioIntranetService.UsuarioIntranetService miUsuarioService = new UsuarioIntranetService.UsuarioIntranetService();

                foreach (var item in miUsuarioService.GetUsuarios(int.Parse(ConfigurationManager.AppSettings["idrol_analista"]), true, int.Parse(ConfigurationManager.AppSettings["IdApp"]), true, 1, true, 10, true))
                {
                    UsuarioIntranet obj = new UsuarioIntranet();
                    obj.Identificador = item.codigotrabajador;
                    obj.Nombres = item.nombres;
                    obj.Apellidos = item.apellidos;
                    obj.DNI = item.dni;
                    obj.IdRol = item.idrol;
                    list.Add(obj);

                }

                int listcount = list.Count;

                if (filter != null)
                    list = list.Where(filter).ToList();
                if (paginacion == null)
                    return new PagedList<UsuarioIntranet>(list, 1, !list.Any() ? 1 : listcount);
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
                //var list = new List<UsuarioIntranet>();
                //var connection = Conexion.CrearConexion().Crear();
                //var query = string.Format("SELECT * FROM {0} WHERE {0}.{1}=@codigo",
                //    ConfigurationManager.AppSettings["administrados"] ?? "vw_administrados"
                //    , ConfigurationManager.AppSettings["administrados.id"] ?? "ID");
                //var result = Operacion.Ejecutar(connection, query, new SqlParameter("@codigo", codigo));

                //if (result != null)
                //{
                //    list.AddRange(from DataRowView item in result
                //                  select new UsuarioIntranet
                //                  {
                //                      Nombres = Convert.ToString(item[ConfigurationManager.AppSettings["administrados.nombres"] ?? "NOMBRES"]),
                //                      Login = Convert.ToString(item[ConfigurationManager.AppSettings["administrados.nombres"] ?? "NOMBRES"]),
                //                      Apellidos = Convert.ToString(item[ConfigurationManager.AppSettings["administrados.apellidos"] ?? "APELLIDOS"]),
                //                      DNI = Convert.ToString(item[ConfigurationManager.AppSettings["administrados.numeroDocumento"] ?? "NRO_DOCUMENTO"]),
                //                      Identificador = Convert.ToInt32(item[ConfigurationManager.AppSettings["administrados.id"] ?? "ID"]),
                //                  });
                //}
                //return list.FirstOrDefault();

                ////return new UsuarioIntranet()
                ////{
                ////    Nombres = "Usuario",
                ////    Apellidos = "Analista",
                ////    Login = "analista",
                ////    DNI = "48490453",
                ////    Identificador = 2
                ////};

                if (miUsuarioService == null)
                {
                    miUsuarioService = new UsuarioIntranetService.UsuarioIntranetService();
                }

                UsuarioIntranetService.UsuarioIntranetExtendidoResponse usu = miUsuarioService.GetUsuario(codigo, true, 0, false, int.Parse(ConfigurationManager.AppSettings["IdApp"]), true);

                if (usu != null)
                {
                    UsuarioIntranet obj = new UsuarioIntranet();
                    obj.Identificador = usu.codigotrabajador;
                    obj.Login = usu.login;
                    obj.Nombres = usu.nombres;
                    obj.Apellidos = usu.apellidos;
                    obj.DNI = usu.dni;
                    obj.IdRol = usu.idrol;
                    obj.Telefono = usu.telefono;
                    return obj;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public UsuarioIntranet FindUsuarioIntranet(int codigo, int idRol)
        {
            throw new NotImplementedException();
        }

        public UsuarioIntranet GetUsuarioIntranetById(int id)
        {
            throw new NotImplementedException();
        }

        public IPagedList<UsuarioExtranet> GetUsuariosExtranet(Paginacion paginacion = null, Func<UsuarioExtranet, bool> filter = null)
        {
            try
            {
                var list = new List<UsuarioExtranet>();
                //var connection = Conexion.CrearConexion().Crear();
                //var query = string.Format("SELECT * FROM {0}",
                //    ConfigurationManager.AppSettings["extranet"] ?? "vw_usuarios_extranet");
                //var result = Operacion.Ejecutar(connection, query);
                //if (result != null)
                //{
                //    list.AddRange(from DataRowView item in result
                //                  select new UsuarioExtranet
                //                  {
                //                      Nombres = Convert.ToString(item[ConfigurationManager.AppSettings["extranet.nombres"] ?? "NOMBRES"]),
                //                      Apellidos = Convert.ToString(item[ConfigurationManager.AppSettings["extranet.apellidos"] ?? "APELLIDOS"]),
                //                      Email = Convert.ToString(item[ConfigurationManager.AppSettings["extranet.email"] ?? "EMAIL"]),
                //                      Login = Convert.ToString(item[ConfigurationManager.AppSettings["extranet.login"] ?? "LOGIN"]),
                //                      Identificador = Convert.ToInt32(item[ConfigurationManager.AppSettings["extranet.id"] ?? "ID"]),
                //                  });
                //}

                ////list.Add(new UsuarioExtranet()
                ////{
                ////    Nombres = "Bryan Fernando",
                ////    Apellidos = "Chauca Hinostroza",
                ////    NombreApellidos = "Bryan Fernando Chauca Hinostroza",
                ////    Email = "brayan_che_6@hotmail.com",
                ////    Login = "informante",
                ////    Identificador = 1
                ////});

                if (usuarioExtranetService == null)
                {
                    usuarioExtranetService = new UsuarioExtranetService.UsuarioExtranetService();
                }
                
                foreach (var item in usuarioExtranetService.GetUsuarios(0, true, int.Parse(ConfigurationManager.AppSettings["IdApp"]), true, 1, true, 10, true))
                {
                    UsuarioExtranet obj = new UsuarioExtranet();
                    obj.Identificador = item.idcontactoextranet;
                    obj.Login = item.login;
                    obj.Nombres = item.nombres;
                    obj.Apellidos = item.apellidos;
                    obj.Email = item.email;
                    list.Add(obj);
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
                //var connection = Conexion.CrearConexion().Crear();
                //var query = string.Format("SELECT * FROM {0} WHERE {0}.{1}=@codigo",
                //    ConfigurationManager.AppSettings["extranet"] ?? "vw_usuarios_extranet",
                //    ConfigurationManager.AppSettings["extranet.id"] ?? "ID");
                //var result = Operacion.Ejecutar(connection, query, new SqlParameter("@codigo", codigo));
                //var list = new List<UsuarioExtranet>();
                //if (result != null)
                //{
                //    list.AddRange(from DataRowView item in result
                //                  select new UsuarioExtranet
                //                  {
                //                      Nombres = Convert.ToString(item[ConfigurationManager.AppSettings["extranet.nombres"] ?? "NOMBRES"]),
                //                      Apellidos = Convert.ToString(item[ConfigurationManager.AppSettings["extranet.apellidos"] ?? "APELLIDOS"]),
                //                      Email = Convert.ToString(item[ConfigurationManager.AppSettings["extranet.email"] ?? "EMAIL"]),
                //                      Login = Convert.ToString(item[ConfigurationManager.AppSettings["extranet.login"] ?? "LOGIN"]),
                //                      Identificador = Convert.ToInt32(item[ConfigurationManager.AppSettings["extranet.id"] ?? "ID"]),
                //                  });
                //}
                //return list.FirstOrDefault();

                return new UsuarioExtranet()
                {
                    Nombres = "Bryan Fernando",
                    Apellidos = "Chauca Hinostroza",
                    NombreApellidos = "Bryan Fernando Chauca Hinostroza",
                    Email = "brayan_che_6@hotmail.com",
                    Login = "informante",
                    Identificador = 1
                };

                //if (usuarioExtranetService == null)
                //{
                //    usuarioExtranetService = new UsuarioExtranetService.UsuarioExtranetService();
                //}

                //UsuarioExtranetService.UsuarioExtranetExtendidoResponse usu = usuarioExtranetService.GetUsuario(codigo, true, 0, false, int.Parse(ConfigurationManager.AppSettings["IdApp"]), true);

                //if (usu != null)
                //{
                //    UsuarioExtranet obj = new UsuarioExtranet();
                //    obj.Identificador = usu.idcontactoextranet;
                //    obj.Login = usu.login;
                //    obj.Nombres = usu.nombres;
                //    obj.Apellidos = usu.apellidos;
                //    obj.Email = usu.email;
                //    return obj;
                //}
                //return null;
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
