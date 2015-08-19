using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Data.Contratos;
using Data.Repositorios;
using Domain.Managers;
using Ninject;
using Ninject.Modules;

namespace WindowsService
{
    public partial class produce : ServiceBase
    {
        private System.Timers.Timer Timer = null;
        public static Manager GetManager(string usuario = "system")
        {
            IKernel kernel = new StandardKernel();
            kernel.Load(new Bind());
            var manager = kernel.Get<Manager>();
            manager.UsuarioAutenticado = usuario;
            return manager;
        }
        

        public produce()
        {
            InitializeComponent();
        }

        private void LogEvent(string message, EventLogEntryType type)
        {
            var eventLog = new EventLog { Source = "Produce", Log = "Application" };
            eventLog.WriteEntry(message, type);
        }

        protected override void OnStart(string[] args)
        {
            if (!EventLog.SourceExists("Produce"))
                EventLog.CreateEventSource("Produce", "Application");
            var message = String.Format("Produce starts on {0} {1}", DateTime.Now.ToString("dd-MMM-yyyy"),
                DateTime.Now.ToString("hh:mm:ss tt"));
            LogEvent(message, EventLogEntryType.Information);
           
            Timer = new Timer(72000000) { AutoReset = true };
            Timer.Elapsed += Timer_Elapsed;
            Timer.Start();
        }

        protected override void OnStop()
        {
            Timer.Stop();
            Timer.Dispose();
            Timer = null;
            var message = String.Format("Produce stops on {0} {1}", DateTime.Now.ToString("dd-MMM-yyyy"),
                DateTime.Now.ToString("hh:mm:ss tt"));
            LogEvent(message, EventLogEntryType.Information);
        }

        void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                GetManager().ParametrizacionEnvioManager.EnviarNotificaciones();
                LogEvent("Verificando establecimientos sin encuestas enviadas", EventLogEntryType.Information);
            }
            catch (Exception e1)
            {
                LogEvent(e1.Message, EventLogEntryType.Error);
            }
        }
    }
    public class Bind : NinjectModule
    {
        public override void Load()
        {
            Bind<IRepositorioDepartamento>().To<RepositorioDepartamento>();
            Bind<IRepositorioProvincia>().To<RepositorioProvincia>();
            Bind<IRepositorioDistrito>().To<RepositorioDistrito>();
            Bind<IRepositorioUbigeo>().To<RepositorioUbigeo>();
            Bind<IRepositorioUsuario>().To<RepositorioUsuario>();
        }
    }
}
