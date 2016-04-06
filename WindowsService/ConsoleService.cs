using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Managers;
using Ninject;
using Ninject.Modules;

namespace WindowsService
{
    public class ConsoleService
    {
        public static Manager GetManager(string usuario = "system")
        {
            IKernel kernel = new StandardKernel();
            kernel.Load(new Bind());
            var manager = kernel.Get<Manager>();
            manager.UsuarioAutenticado = usuario;
            return manager;
        }

        public ConsoleService()
        {
        }

        public void Run()
        {
            Console.WriteLine("Iniciando servicio como consola");
            // Console.WriteLine("CTRL + BREAK para terminar");
            Console.WriteLine("Enviando notificaciones...");

            try
            {
                GetManager().ParametrizacionEnvioManager.EnviarNotificaciones();
            }
            catch (Exception e1)
            {
                Console.WriteLine("No se pudieron enviar notificaciones. Error=" + e1.Message);
            }

            Console.WriteLine("Finalizado");
            Console.ReadLine();
        }
    }
}
