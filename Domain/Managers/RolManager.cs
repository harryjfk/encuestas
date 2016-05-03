using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Repositorios;
using Entity;

namespace Domain.Managers
{
    public class RolManager:GenericManager<Rol>
    {
        public RolManager(GenericRepository<Rol> repository, Manager manager) : base(repository, manager)
        {
        }

        public RolManager(Entities context, Manager manager) : base(context, manager)
        {
        }
        public override void Seed()
        {
            var roles = new Rol[]
            {
                new Rol()
                {
                    Id = 1,
                    Activado = true,
                    Nombre = "Administrador",
                    Usuarios = Manager.Usuario.Get(t=>t.Identificador==-1).ToList()
                },
                new Rol()
                {
                    Id = 2,
                    Activado = true,
                    Nombre = "Analista",
                },
                new Rol()
                {
                    Id = 3,
                    Activado = true,
                    Nombre = "Informante",
                }
            };
            AddOrUpdate(t=>t.Id,roles);
        }
    }
}
