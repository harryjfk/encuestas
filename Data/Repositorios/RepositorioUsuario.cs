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
        UsuarioIntranetService1.UsuarioIntranetService miUsuarioService;
        UsuarioExtranetService.UsuarioExtranetService usuarioExtranetService;

        public IPagedList<UsuarioIntranet> GetUsuariosIntranetAnalista(Paginacion paginacion = null, Func<UsuarioIntranet, bool> filter = null)
        {
            try
            {
                var list = new List<UsuarioIntranet>();
                if (miUsuarioService == null)
                {
                    miUsuarioService = new UsuarioIntranetService1.UsuarioIntranetService();
                }

                var wsUsuariosIntranet = miUsuarioService.GetUsuarios(int.Parse(ConfigurationManager.AppSettings["idrol_analista"]), true, int.Parse(ConfigurationManager.AppSettings["IdApp"]), true, 1, true, 100, true);

                foreach (var item in wsUsuariosIntranet)
                {
                    UsuarioIntranet obj = new UsuarioIntranet();
                    obj.Identificador = item.codigotrabajador;
                    obj.Nombres = item.nombres;
                    obj.Apellidos = item.apellidos;
                    obj.DNI = item.dni;
                    obj.IdRol = item.idrol;
                    list.Add(obj);
                }

                if (filter != null)
                    list = list.Where(filter).ToList();
                if (paginacion == null)
                    return new PagedList<UsuarioIntranet>(list, 1, !list.Any() ? 1 : list.Count());
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
                    miUsuarioService = new UsuarioIntranetService1.UsuarioIntranetService();
                }

                var wsUsuariosIntranet = miUsuarioService.GetUsuarios(int.Parse(ConfigurationManager.AppSettings["idrol_administrador"]), true, int.Parse(ConfigurationManager.AppSettings["IdApp"]), true, 1, true, 100, true);

                foreach (var item in wsUsuariosIntranet)
                {
                    UsuarioIntranet obj = new UsuarioIntranet();
                    obj.Identificador = item.codigotrabajador;
                    obj.Nombres = item.nombres;
                    obj.Apellidos = item.apellidos;
                    obj.DNI = item.dni;
                    obj.IdRol = item.idrol;
                    list.Add(obj);
                }
                
                if (filter != null)
                    list = list.Where(filter).ToList();
                if (paginacion == null)
                    return new PagedList<UsuarioIntranet>(list, 1, !list.Any() ? 1 : list.Count());
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
                
                UsuarioIntranetService1.UsuarioIntranetService miUsuarioService = new UsuarioIntranetService1.UsuarioIntranetService();

                var wsUsuariosIntranet = miUsuarioService.GetUsuarios(0, false, int.Parse(ConfigurationManager.AppSettings["IdApp"]), true, 1, true, 100, true);
                
                foreach (var item in wsUsuariosIntranet)
                {
                    UsuarioIntranet obj = new UsuarioIntranet();
                    obj.Identificador = item.codigotrabajador;
                    obj.Nombres = item.nombres;
                    obj.Apellidos = item.apellidos;
                    obj.DNI = item.dni;
                    obj.IdRol = item.idrol;
                    list.Add(obj);
                }
                
                if (filter != null)
                    list = list.Where(filter).ToList();
                if (paginacion == null)
                    return new PagedList<UsuarioIntranet>(list, 1, !list.Any() ? 1 : list.Count);
                paginacion.Validate();
                return list.ToPagedList(paginacion.Page, paginacion.ItemsPerPage);
            }
            catch (Exception ex)
            {
                return new PagedList<UsuarioIntranet>(new List<UsuarioIntranet>(), 1, 1);
            }
        }

        public UsuarioIntranet FindUsuarioIntranet(int codigo)
        {
            try
            {
                if (miUsuarioService == null)
                {
                    miUsuarioService = new UsuarioIntranetService1.UsuarioIntranetService();
                }

                UsuarioIntranetService1.UsuarioIntranetExtendidoResponse usu = miUsuarioService.GetUsuario(codigo, true, 0, false, int.Parse(ConfigurationManager.AppSettings["IdApp"]), true);

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

        //public UsuarioIntranet FindUsuarioIntranet(int codigo, int idRol)
        //{
        //    throw new NotImplementedException();
        //}

        //public UsuarioIntranet GetUsuarioIntranetById(int id)
        //{
        //    throw new NotImplementedException();
        //}

        public IPagedList<UsuarioExtranet> GetUsuariosExtranet(Paginacion paginacion = null, Func<UsuarioExtranet, bool> filter = null)
        {
            try
            {
                var list = new List<UsuarioExtranet>();

                if (usuarioExtranetService == null)
                {
                    usuarioExtranetService = new UsuarioExtranetService.UsuarioExtranetService();
                }

                foreach (var item in usuarioExtranetService.GetUsuarios(int.Parse(ConfigurationManager.AppSettings["idrol_informante"]), true, int.Parse(ConfigurationManager.AppSettings["IdApp"]), true, 1, true, 100, true))
                {
                    UsuarioExtranet obj = new UsuarioExtranet();
                    obj.Identificador = item.idcontactoextranet;
                    obj.Login = item.login;
                    obj.Nombres = item.nombres;
                    obj.Apellidos = item.apellidos;
                    obj.Email = item.email;
                    obj.Ruc = item.ruc;
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
                if (usuarioExtranetService == null)
                {
                    usuarioExtranetService = new UsuarioExtranetService.UsuarioExtranetService();
                }

                UsuarioExtranetService.UsuarioExtranetExtendidoResponse usu = usuarioExtranetService.GetUsuario(codigo, true, 0, false, int.Parse(ConfigurationManager.AppSettings["IdApp"]), true);

                if (usu != null)
                {
                    UsuarioExtranet obj = new UsuarioExtranet();
                    obj.Identificador = usu.idcontactoextranet;
                    obj.Login = usu.login;
                    obj.Nombres = usu.nombres;
                    obj.Apellidos = usu.apellidos;
                    obj.Email = usu.email;
                    obj.Ruc = usu.ruc;
                    return obj;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public UsuarioIntranet AutenticateIntranet(string login, string password)
        {
            return null;
        }

        public UsuarioExtranet AutenticateExtranet(string login, string password)
        {
            return null;
        }
    }
}