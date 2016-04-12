using Data.Contratos;
using Entity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositorios
{
    public class RepositorioUsuarioLocal : IRepositorioUsuario
    {
        private List<UsuarioIntranet> GetListUsuarioIntranet()
        {
            var list = new List<UsuarioIntranet>();

            list.Add(new UsuarioIntranet()
            {
                Identificador = -1,
                Nombres = "Lola",
                Apellidos = "Perez",
                Login = "superAdmin",
                Roles = new List<Rol>()
                {
                    new Rol() { Id = 1, Nombre = "Administrador" }
                }
            });

            list.Add(new UsuarioIntranet()
            {
                Identificador = 100,
                Nombres = "Elard",
                Apellidos = "Molina",
                Login = "emolina",
                Roles = new List<Rol>()
                {
                    new Rol() { Id = 2, Nombre = "Analista" }
                }
            });

            list.Add(new UsuarioIntranet()
            {
                Identificador = 101,
                Nombres = "Liz",
                Apellidos = "Julca",
                Login = "lizjulca",
                Roles = new List<Rol>()
                {
                    new Rol() { Id = 2, Nombre = "Analista" }
                }
            });
            
            list.Add(new UsuarioIntranet()
            {
                Identificador = 102,
                Nombres = "Patricia",
                Apellidos = "Cordova",
                Login = "pcordova",
                Roles = new List<Rol>()
                {
                    new Rol() { Id = 2, Nombre = "Analista" }
                }
            });

            list.Add(new UsuarioIntranet()
            {
                Identificador = 103,
                Nombres = "Vanessa",
                Apellidos = "Castro",
                Login = "vcastro",
                Roles = new List<Rol>()
                {
                    new Rol() { Id = 2, Nombre = "Analista" }
                }
            });

            list.Add(new UsuarioIntranet()
            {
                Identificador = 104,
                Nombres = "Bladimir",
                Apellidos = "Huaraca",
                Login = "bhuaraca",
                Roles = new List<Rol>()
                {
                    new Rol() { Id = 2, Nombre = "Analista" }
                }
            });

            list.Add(new UsuarioIntranet()
            {
                Identificador = 105,
                Nombres = "Juan",
                Apellidos = "Gambini",
                Login = "jgambini",
                Roles = new List<Rol>()
                {
                    new Rol() { Id = 2, Nombre = "Analista" }
                }
            });

            list.Add(new UsuarioIntranet()
            {
                Identificador = 106,
                Nombres = "Luis",
                Apellidos = "Valderrama",
                Login = "lvr",
                Roles = new List<Rol>()
                {
                    new Rol() { Id = 2, Nombre = "Analista" }
                }
            });

            return list;
        }

        private List<UsuarioExtranet> GetListUsuarioExtranet()
        {
            var list = new List<UsuarioExtranet>();

            list.Add(new UsuarioExtranet()
            {
                Identificador = 11,
                Nombres = "Informante",
                Apellidos = "1",
                NombreApellidos = "Informante 1",
                Email = "Informante1@hotmail.com",
                Login = "20255135253",
                Ruc = "25750426"
            });

            list.Add(new UsuarioExtranet()
            {
                Identificador = 12,
                Nombres = "Informante",
                Apellidos = "2",
                NombreApellidos = "Informante 2",
                Email = "Informante2@hotmail.com",
                Login = "20101538673",
                Ruc = "25750426"
            });

            list.Add(new UsuarioExtranet()
            {
                Identificador = 13,
                Nombres = "Informante",
                Apellidos = "3",
                NombreApellidos = "Informante 3",
                Email = "Informante3@hotmail.com",
                Login = "20100015014",
                Ruc = "09549688"
            });

            list.Add(new UsuarioExtranet()
            {
                Identificador = 14,
                Nombres = "Informante",
                Apellidos = "4",
                NombreApellidos = "Informante 4",
                Email = "Informante4@hotmail.com",
                Login = "20103342091",
                Ruc = "09549688"
            });

            list.Add(new UsuarioExtranet()
            {
                Identificador = 15,
                Nombres = "Informante",
                Apellidos = "5",
                NombreApellidos = "Informante 5",
                Email = "Informante5@hotmail.com",
                Login = "20100012856",
                Ruc = "09577640"
            });

            list.Add(new UsuarioExtranet()
            {
                Identificador = 16,
                Nombres = "Informante",
                Apellidos = "6",
                NombreApellidos = "Informante 6",
                Email = "Informante6@hotmail.com",
                Login = "20100005302",
                Ruc = "09577640"
            });

            list.Add(new UsuarioExtranet()
            {
                Identificador = 17,
                Nombres = "Informante",
                Apellidos = "7",
                NombreApellidos = "Informante 7",
                Email = "Informante7@hotmail.com",
                Login = "20504524176",
                Ruc = "42891898"
            });

            list.Add(new UsuarioExtranet()
            {
                Identificador = 18,
                Nombres = "Informante",
                Apellidos = "8",
                NombreApellidos = "Informante 8",
                Email = "Informante8@hotmail.com",
                Login = "20296745317",
                Ruc = "42891898"
            });

            list.Add(new UsuarioExtranet()
            {
                Identificador = 19,
                Nombres = "Informante",
                Apellidos = "9",
                NombreApellidos = "Informante 9",
                Email = "Informante9@hotmail.com",
                Login = "20468770475",
                Ruc = "43711336"
            });

            list.Add(new UsuarioExtranet()
            {
                Identificador = 20,
                Nombres = "Informante",
                Apellidos = "10",
                NombreApellidos = "Informante 10",
                Email = "Informante10@hotmail.com",
                Login = "20518240839",
                Ruc = "43711336"
            });

            list.Add(new UsuarioExtranet()
            {
                Identificador = 21,
                Nombres = "Informante",
                Apellidos = "11",
                NombreApellidos = "Informante 11",
                Email = "Informante11@hotmail.com",
                Login = "20330444372",
                Ruc = "10811417"
            });

            list.Add(new UsuarioExtranet()
            {
                Identificador = 22,
                Nombres = "Informante",
                Apellidos = "12",
                NombreApellidos = "Informante 12",
                Email = "Informante12@hotmail.com",
                Login = "20100712599",
                Ruc = "10811417"
            });

            list.Add(new UsuarioExtranet()
            {
                Identificador = 23,
                Nombres = "Informante",
                Apellidos = "13",
                NombreApellidos = "Informante 13",
                Email = "Informante13@hotmail.com",
                Login = "20100192064",
                Ruc = "08446809"
            });

            list.Add(new UsuarioExtranet()
            {
                Identificador = 24,
                Nombres = "Informante",
                Apellidos = "14",
                NombreApellidos = "Informante 14",
                Email = "Informante14@hotmail.com",
                Login = "20100078369",
                Ruc = "08446809"
            });

            return list;
        }

        public IPagedList<UsuarioIntranet> GetUsuariosIntranetAnalista(Paginacion paginacion = null, Func<UsuarioIntranet, bool> filter = null)
        {
            try
            {
                var list = GetListUsuarioIntranet().Where(t=>t.Roles.FirstOrDefault().Id == 2).ToList();
                
                if (filter != null)
                {
                    list = list.Where(filter).ToList();
                }

                int listcount = list.Count();

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
                var list = GetListUsuarioIntranet().Where(t => t.Roles.FirstOrDefault().Id == 1).ToList();

                if (filter != null)
                {
                    list = list.Where(filter).ToList();
                }

                int listcount = list.Count();
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
                var list = GetListUsuarioIntranet();

                if (filter != null)
                {
                    list = list.Where(filter).ToList();
                }

                int listcount = list.Count();

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
                return GetListUsuarioIntranet().Where(t => t.Identificador == codigo).FirstOrDefault();               
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
        //    return GetListUsuarioIntranet().Where(t => t.Identificador == id).FirstOrDefault();
        //}

        public IPagedList<UsuarioExtranet> GetUsuariosExtranet(Paginacion paginacion = null, Func<UsuarioExtranet, bool> filter = null)
        {
            try
            {
                var list = GetListUsuarioExtranet();

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
                return GetListUsuarioExtranet().Where(t => t.Identificador == codigo).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public UsuarioIntranet AutenticateIntranet(string login, string password)
        {
            return GetListUsuarioIntranet().Where(t => t.Login == login).FirstOrDefault();
        }

        public UsuarioExtranet AutenticateExtranet(string login, string password)
        {
            return GetListUsuarioExtranet().Where(t => t.Login == login && t.Ruc == password).FirstOrDefault();
        }
    }
}
