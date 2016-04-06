using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService
{
    static class producemain
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main()
        {
            // Evaluar si está configurado para consola y no iniciar el servicio Windows
            bool console = false;
            string[] args = Environment.GetCommandLineArgs();
            foreach (string arg in args)
            {
                if (arg.Substring(0, 9).ToLower() == "/console:")
                {
                    string parmValue = arg.Substring(9).ToLower();
                    if (parmValue == "true")
                    {
                        // Lanzar aplicación de consola
                        console = true;
                    }
                }
            }

            if (!console)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
                { 
                    new produce() 
                };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                ConsoleService service = new ConsoleService();
                service.Run();
            }
        }
    }
}
