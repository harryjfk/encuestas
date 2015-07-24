using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Repositorios;
using Entity;

namespace Domain.Managers
{
    public class ContactoManager : GenericManager<Contacto>
    {
        public ContactoManager(GenericRepository<Contacto> repository, Manager manager)
            : base(repository, manager)
        {
        }

        public ContactoManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }

        public void EstablecerPredeterminado(long idContacto)
        {
            var contacto = Find(idContacto);
            if (contacto == null) return;
            foreach (var c in Get(t => t.IdEstablecimiento == contacto.IdEstablecimiento))
            {
                c.Activado = false;
                Modify(c);
                SaveChanges();
            }
            contacto.Activado = true;
            SaveChanges();
            //var manager = Manager;
            //var establecimiento = manager.Establecimiento.Find(contacto.IdEstablecimiento);
            //if (establecimiento != null)
            //{
            //    establecimiento.IdInformante = contacto.IdUsuario;
            //    manager.Establecimiento.Modify(establecimiento);
            //    manager.Establecimiento.SaveChanges();
            //}
        }

        public override OperationResult<Contacto> Add(Contacto element)
        {
            var operation = base.Add(element);
            if (element.Activado)
                EstablecerPredeterminado(element.Id);
            return operation;
        }

        public override OperationResult<Contacto> Modify(Contacto element, params string[] properties)
        {
            var operation = base.Modify(element,properties);
            if (element.Activado)
                EstablecerPredeterminado(element.Id);
            return operation;
        }

        public override OperationResult<Contacto> Delete(Contacto element)
        {
            if (element.Activado)
            {
                var manager = Manager;
                var establecimiento = manager.Establecimiento.Find(element.IdEstablecimiento);
                if (establecimiento != null)
                {
                    establecimiento.IdInformante = null;
                    manager.Establecimiento.Modify(establecimiento);
                    manager.Establecimiento.SaveChanges();
                }
            }
            return base.Delete(element);
        }

        public override List<string> Validate(Contacto element)
        {
            var list = base.Validate(element);
            list.Required(element, t => t.Nombre, "Nombre");
            list.Required(element, t => t.Telefono, "Telefono");
            list.Required(element, t => t.Correo, "Correo");
            // list.Required(element, t => t.Celular, "Celular");
            // list.Required(element, t => t.Anexo, "Anexo");
            // list.Required(element, t => t.IdCargo, "Cargo");
            list.Required(element, t => t.IdEstablecimiento, "Establecimiento");

            list.MaxLength(element, t => t.Nombre, 100, "Nombre");
            list.MaxLength(element, t => t.Telefono, 10, "Telefono");
            list.MaxLength(element, t => t.Correo, 50, "Correo");
            list.MaxLength(element, t => t.Anexo, 5, "Correo");
            list.MaxLength(element, t => t.Celular, 10, "Celular");

            list.PhoneNumber(element, t => t.Celular, "Celular");
            list.PhoneNumber(element, t => t.Telefono, "Telefono");

            list.Email(element, t => t.Correo, "Correo");

            return list;
        }

        public void ToggleFromUser(int idUsuario, long idEstablecimiento)
        {
            var manager = Manager;
            var user = Manager.Usuario.FindUsuarioExtranet(idUsuario);
            if (user == null) return;
            var establecimiento = Manager.Establecimiento.Find(idEstablecimiento);
            if (establecimiento == null)
                return;
            var userG = Manager.Usuario.Find(idUsuario);
            if (userG == null)
            {
                userG = new Usuario()
                {
                    Login = user.Login,
                    Identificador = user.Identificador
                };
                manager.Usuario.Add(userG);
                manager.Usuario.SaveChanges();
            }
            if (establecimiento.IdInformante == null || establecimiento.IdInformante != idUsuario)
                establecimiento.IdInformante = idUsuario;
            else
            {
                establecimiento.IdInformante = null;
            }
            manager.Establecimiento.Modify(establecimiento);
            manager.Establecimiento.SaveChanges();


            //var contacto = establecimiento.Contactos.FirstOrDefault(t => t.IdUsuario == idUsuario);
            //if (contacto == null)
            //{
            //    contacto = new Contacto()
            //    {
            //        Activado = true,
            //        Nombre = user.Nombres,
            //        Telefono = "0000",
            //        Correo = user.Email,
            //        IdEstablecimiento = idEstablecimiento,
            //        IdUsuario = idUsuario
            //    };
            //    Add(contacto);
            //    SaveChanges();
            //    EstablecerPredeterminado(contacto.Id);
            //    return;
            //}
            //manager.Contacto.Delete(contacto.Id);
            //manager.Contacto.SaveChanges();

        }
    }
}
