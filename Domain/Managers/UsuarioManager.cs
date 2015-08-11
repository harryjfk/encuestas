using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Contratos;
using Data.Repositorios;
using Entity;
using PagedList;

namespace Domain.Managers
{
    public class UsuarioManager : GenericManager<Usuario>
    {
        public IRepositorioUsuario Repositorio { get; set; }

        public UsuarioManager(GenericRepository<Usuario> repository, Manager manager, IRepositorioUsuario repositorio)
            : base(repository, manager)
        {
            Repositorio = repositorio;
        }

        public UsuarioManager(Entities context, Manager manager, IRepositorioUsuario repositorio)
            : base(context, manager)
        {
            Repositorio = repositorio;
        }

        public UsuarioIntranet AutenticateIntranetPRODUCE(string id, string login, string rol, string nombre)
        {
            var manager = Manager;
            Usuario user = new Usuario();
            user.Identificador = int.Parse(id);
            user.Login = login;
            user.Nombres = nombre;
            Usuario usr = manager.Usuario.Find(user.Identificador);

            if (usr == null)
            {
                usr = new Usuario()
                {
                    Login = user.Login,
                    Identificador = user.Identificador,
                    Nombres = user.Nombres
                };
                manager.Usuario.Add(usr);
                manager.Usuario.SaveChanges();
                usr.Roles = manager.Rol.Get(t => t.Nombre.Equals(rol)).ToList();
                manager.Usuario.SaveChanges();
            }
            UsuarioIntranet usuarioIntranet = new UsuarioIntranet()
            {
                Identificador = usr.Identificador,
                Nombres = usr.Nombres,
                Apellidos = "",
                Login = usr.Login
            };

            return usuarioIntranet;
        }

        public UsuarioIntranet AutenticateIntranet(string login, string password)
        {
            var manager = Manager;
            var user = Repositorio.AutenticateIntranet(login, password);
            if (user != null)
            {
                var usr = manager.Usuario.Find(user.Identificador);
                if (usr == null)
                {
                    usr = new Usuario()
                    {
                        Login = user.Login,
                        Identificador = user.Identificador
                    };
                    manager.Usuario.Add(usr);
                    manager.Usuario.SaveChanges();
                    usr.Roles = manager.Rol.Get(t => t.Nombre.Equals("Analista")).ToList();
                    manager.Usuario.SaveChanges();
                }
                else
                {
                    if (usr.Roles.All(t => !t.Nombre.Equals("Administrador")))
                    {
                        usr.Roles = manager.Rol.Get(t => t.Nombre.Equals("Analista")).ToList();
                    }
                    usr.Login = user.Login;
                    manager.Usuario.Modify(usr);
                    manager.Usuario.SaveChanges();
                }
                return user;
            }
            return null;
        }
        public UsuarioExtranet AutenticateExtranet(string login, string password)
        {
            var manager = Manager;
            var user = Repositorio.AutenticateExtranet(login, password);
            if (user != null)
            {
                var usr = manager.Usuario.Find(user.Identificador);
                if (usr == null)
                {
                    usr = new Usuario()
                    {
                        Login = user.Login,
                        Identificador = user.Identificador
                    };
                    manager.Usuario.Add(usr);
                    manager.Usuario.SaveChanges();
                    usr.Roles = manager.Rol.Get(t => t.Nombre.Equals("Informante")).ToList();
                    manager.Usuario.SaveChanges();
                }
                else
                {
                    usr.Roles = manager.Rol.Get(t => t.Nombre.Equals("Informante")).ToList();
                    usr.Login = user.Login;
                    manager.Usuario.Modify(usr);
                    manager.Usuario.SaveChanges();
                }
                return user;
            }
            return user;
        }

        public Usuario Autenticate(string login, string password)
        {
            var extra = AutenticateExtranet(login, password);
            var intra = AutenticateIntranet(login, password);
            if (intra != null)
            {
                var user = Find(intra.Identificador);
                return user ?? intra;
            }
            if (extra != null)
            {
                var user = Find(extra.Identificador);
                return user ?? extra;
            }
            return null;
        }

        public IPagedList<UsuarioExtranet> GetUsuariosExtranet(Paginacion paginacion = null)
        {
            var list = Repositorio.GetUsuariosExtranet(paginacion);
            foreach (var usr in list)
            {
                var te = Find(usr.Identificador);
                if (te == null) continue;
                usr.Roles = te.Roles.ToList();
                usr.EstablecimientosInformante = te.EstablecimientosInformante.ToList();
            }
            return list;
        }

        public IPagedList<UsuarioIntranet> GetUsuariosIntranetAnalista(Query<UsuarioIntranet> query)
        {
            var list = Repositorio.GetUsuariosIntranetAnalista(query.Paginacion, query.Filter);
            foreach (var usr in list)
            {
                var te = Find(usr.Identificador);
                if (te == null) continue;
                usr.Roles = te.Roles.ToList();
                //usr.EstablecimientosAnalista = te.EstablecimientosAnalista.ToList();
            }
            query.Elements = list;
            return list;
        }

        public IPagedList<UsuarioIntranet> GetUsuariosIntranetAdministrador(Query<UsuarioIntranet> query)
        {
            var list = Repositorio.GetUsuariosIntranetAdministrador(query.Paginacion, query.Filter);
            foreach (var usr in list)
            {
                var te = Find(usr.Identificador);
                if (te == null) continue;
                usr.Roles = te.Roles.ToList();
                //usr.EstablecimientosAnalista = te.EstablecimientosAnalista.ToList();
            }
            query.Elements = list;
            return list;
        }

        public IPagedList<UsuarioIntranet> GetUsuariosIntranet(Paginacion paginacion = null)
        {
            var list = Repositorio.GetUsuariosIntranet(paginacion);
            foreach (var usr in list)
            {
                var te = Find(usr.Identificador);
                if (te == null) continue;
                usr.Roles = te.Roles.ToList();
                //usr.EstablecimientosAnalista = te.EstablecimientosAnalista.ToList();
            }
            return list;
        }

        public IPagedList<UsuarioIntranet> GetUsuariosIntranet(Query<UsuarioIntranet> query)
        {
            var list = Repositorio.GetUsuariosIntranet(query.Paginacion, query.Filter);
            foreach (var usr in list)
            {
                var te = Find(usr.Identificador);
                if (te == null) continue;
                usr.Roles = te.Roles.ToList();
               // usr.EstablecimientosAnalista = te.EstablecimientosAnalista.ToList();
            }
            query.Elements = list;
            return list;
        }
        public IPagedList<UsuarioExtranet> GetUsuariosExtranet(Query<UsuarioExtranet> query, long idEstablecimiento)
        {
            var list = Repositorio.GetUsuariosExtranet(query.Paginacion, query.Filter);
            foreach (var usr in list)
            {
                var te = Find(usr.Identificador);
                if (te == null) continue;
                usr.Roles = te.Roles.ToList();
                usr.EstablecimientosInformante = te.EstablecimientosInformante.ToList();
                usr.Seleccionado = te.EstablecimientosInformante.Any(t => t.Id == idEstablecimiento);
            }
            query.Elements = list;
            return list;
        }
        public IPagedList<UsuarioExtranet> GetUsuariosExtranetContacto(Query<UsuarioExtranet> query, long idEstablecimiento)
        {
            var list = Repositorio.GetUsuariosExtranet();
            if (string.IsNullOrEmpty(query.Criteria.NombreApellidos) || string.IsNullOrWhiteSpace(query.Criteria.NombreApellidos))
                list =
                    Get(t => t.EstablecimientosInformante.Any(h => h.Id == idEstablecimiento))
                    .Select(t => Repositorio.FindUsuarioExtranet((int)t.Identificador))
                    .ToPagedList(query.Paginacion.Page, query.Paginacion.ItemsPerPage);

            else
            {
                list = Repositorio.GetUsuariosExtranet(query.Paginacion, query.Filter);
            }
            foreach (var usr in list)
            {
                var te = Find(usr.Identificador);
                if (te == null) continue;
                usr.Roles = te.Roles.ToList();
                usr.EstablecimientosInformante = te.EstablecimientosInformante.ToList();
                usr.Seleccionado = te.EstablecimientosInformante.Any(t=>t.Id==idEstablecimiento);
            }
            query.Elements = list;
            return list;
        }

        public UsuarioExtranet FindUsuarioExtranet(int codigo)
        {
            var usr = Repositorio.FindUsuarioExtranet(codigo);
            var te = Find(usr.Identificador);
            if (te == null) return usr;
            usr.Roles = te.Roles.ToList();
            usr.EstablecimientosInformante = te.EstablecimientosInformante.ToList();
            return usr;
        }
        public UsuarioIntranet FindUsuarioIntranet(int codigo)
        {
            var usr = Repositorio.FindUsuarioIntranet(codigo);
            var te = Find(usr.Identificador);
            if (te == null) return usr;
            usr.Roles = te.Roles.ToList();
            //usr.EstablecimientosAnalista = te.EstablecimientosAnalista.ToList();
            return usr;
        }

        public UsuarioIntranet FindUsuarioIntranet(int codigo, int idRol)
        {
            var usr = Repositorio.FindUsuarioIntranet(codigo, idRol);
            var te = Find(usr.Identificador);
            if (te == null) return usr;
            usr.Roles = te.Roles.ToList();
           // usr.EstablecimientosAnalista = te.EstablecimientosAnalista.ToList();
            return usr;
        }

        public void MarcarAdministrador(int codigo)
        {
            var manager = Manager;
            var user = manager.Usuario.Find(codigo);
            if (user == null)
            {
                var tr = manager.Usuario.FindUsuarioIntranet(codigo);
                var nUser = new Usuario() { Identificador = tr.Identificador, Login = tr.Login };
                manager.Usuario.Add(nUser);
                manager.Usuario.SaveChanges();
                user = manager.Usuario.Find(codigo);
            }
            var rol = manager.Rol.Get(t => t.Nombre == "Administrador").FirstOrDefault();
            if (rol == null)
            {
                rol = new Rol()
                {
                    Nombre = "Administrador",
                    Activado = true,

                };
                manager.Rol.Add(rol);
                manager.Rol.SaveChanges();
            }
            while (rol.Usuarios.Any(t => t.Identificador != -1))
            {
                rol.Usuarios.Remove(rol.Usuarios.FirstOrDefault(t => t.Identificador != -1));
                manager.Rol.SaveChanges();
            }
            rol.Usuarios.Add(user);
            manager.Usuario.SaveChanges();

        }

        public void AsignarEstablecimientoAnalista(int idUsuario, long idEstablecimiento)
        {
            var manager = Manager;
            var se = manager.Usuario.FindUsuarioIntranet(idUsuario);
            var establecimiento = manager.Establecimiento.Find(idEstablecimiento);
            if (se == null || establecimiento == null) return;
            var user = manager.Usuario.Find(idUsuario);
            if (user == null)
            {
                user = new Usuario()
                {
                    Login = se.Login,
                    Identificador = se.Identificador
                };
                manager.Usuario.Add(user);
                manager.Usuario.SaveChanges();
                user = manager.Usuario.Find(idUsuario);
            }
            //establecimiento.Analista = user;
            //establecimiento.IdAnalista = idUsuario;
            manager.Establecimiento.SaveChanges();

            foreach (var enc in establecimiento.Encuestas)
            {
                if (enc.IdAnalista == null)
                {
                    enc.IdAnalista = idUsuario;
                    manager.Encuesta.Modify(enc);
                }
            }
            manager.Encuesta.SaveChanges();
        }

        public void AsignarEstablecimientoAnalista(UsuarioIntranet usuario, long idEstablecimiento)
        {
            int idUsuario = (int)usuario.Identificador;
            int idRol = (int)usuario.IdRol;
            var manager = Manager;
            var se = manager.Usuario.FindUsuarioIntranet(idUsuario, idRol);
            var establecimiento = manager.Establecimiento.Find(idEstablecimiento);
            if (se == null || establecimiento == null) return;
            var user = manager.Usuario.Find(idUsuario);
            if (user == null)
            {
                user = new Usuario()
                {
                    Login = se.Login,
                    Identificador = se.Identificador
                };
                manager.Usuario.Add(user);
                manager.Usuario.SaveChanges();
                user = manager.Usuario.Find(idUsuario);
            }
            //establecimiento.Analista = user;
            //establecimiento.IdAnalista = idUsuario;
            manager.Establecimiento.SaveChanges();

            foreach (var enc in establecimiento.Encuestas)
            {
                if (enc.IdAnalista == null)
                {
                    enc.IdAnalista = idUsuario;
                    manager.Encuesta.Modify(enc);
                }
            }
            manager.Encuesta.SaveChanges();
        }

        public void EliminarEstablecimientoAnalista(long idEstablecimiento)
        {
            var manager = Manager;
            var establecimiento = manager.Establecimiento.Find(idEstablecimiento);
            if (establecimiento == null) return;
            //establecimiento.IdAnalista = null;
            manager.Establecimiento.Modify(establecimiento);
            manager.Establecimiento.SaveChanges();
        }
        public void AsignarEstablecimientoInformante(int idUsuario, long idEstablecimiento)
        {
            var manager = Manager;
            var se = manager.Usuario.FindUsuarioExtranet(idUsuario);
            var establecimiento = manager.Establecimiento.Find(idEstablecimiento);
            if (se == null || establecimiento == null) return;
            var user = manager.Usuario.Find(idUsuario);
            if (user == null)
            {
                user = new Usuario()
                {
                    Login = se.Login,
                    Identificador = se.Identificador
                };
                manager.Usuario.Add(user);
                manager.Usuario.SaveChanges();
                user = manager.Usuario.Find(idUsuario);
            }
            establecimiento.Informante = user;
            establecimiento.IdInformante = idUsuario;
            manager.Establecimiento.SaveChanges();
        }
        public void EliminarEstablecimientoInformante(long idEstablecimiento)
        {
            var manager = Manager;
            var establecimiento = manager.Establecimiento.Find(idEstablecimiento);
            if (establecimiento == null) return;
            establecimiento.IdInformante = null;
            manager.Establecimiento.Modify(establecimiento);
            manager.Establecimiento.SaveChanges();
        }
        public override void Seed()
        {
            var user = Find(-1);
            if (user == null)
            {
                user = new Usuario { Identificador = -1, Roles = Manager.Rol.Get(t => t.Nombre.Equals("Administrador")).ToList(), Login = "superAdmin" };
                Add(user);
                SaveChanges();
            }
            else
            {
                user.Roles.Clear();
                user.Roles = Manager.Rol.Get(t => t.Nombre.Equals("Administrador")).ToList();
                user.Login = "superAdmin";
                SaveChanges();
            }
        }

    }
}
