﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Data.Repositorios;
using Entity;

namespace Domain.Managers
{
    public class ParametrizacionEnvioManager : GenericManager<ParametrizacionEnvio>
    {
        public ParametrizacionEnvioManager(GenericRepository<ParametrizacionEnvio> repository, Manager manager)
            : base(repository, manager)
        {
        }

        public ParametrizacionEnvioManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }

        public void Generate()
        {
            var estadistica = Get(t => t.tipo_encuesta == "Estadistica").FirstOrDefault();
            var empresarial = Get(t => t.tipo_encuesta == "Empresarial").FirstOrDefault();
            if (estadistica == null)
            {
                estadistica = new ParametrizacionEnvio()
                {
                    tipo_encuesta = "Estadistica",
                    Activado = false,
                    comienzo = DateTime.Now,
                    envio_1 = 5,
                    envio_2 = 25,
                    mensaje = "Tiene encuestas estadísticas sin llenar"
                };
                Add(estadistica);
                SaveChanges();
            }
            if (empresarial == null)
            {
                empresarial = new ParametrizacionEnvio()
                {
                    tipo_encuesta = "Empresarial",
                    Activado = false,
                    comienzo = DateTime.Now,
                    envio_1 = 5,
                    envio_2 = 25,
                    mensaje = "Tiene encuestas de opinión empresarial sin llenar"
                };
                Add(empresarial);
                SaveChanges();
            }

        }

        public override List<string> Validate(ParametrizacionEnvio element)
        {
            var list = base.Validate(element);
            list.Required(element, t => t.mensaje, "Mensaje");
            list.Required(element, t => t.comienzo, "Comienzo");
            list.Required(element, t => t.envio_1, "Envío 1");
            list.Required(element, t => t.envio_1, "Envío 2");
            list.MaxLength(element, t => t.mensaje, 500, "Mensaje");
            return list;
        }
        public override OperationResult<ParametrizacionEnvio> Modify(ParametrizacionEnvio element, params string[] properties)
        {
            return base.Modify(element, "tipo_encuesta");
        }

        public void EnviarNotificaciones()
        {
            EnviarNotificacionesEncuestaEstadistica();
            EnviarNotificacionesEncuestaEmpresarial();
        }

        public void EnviarNotificacionesEncuestaEstadistica()
        {
            var parametro = Get(t => t.tipo_encuesta == "Estadistica").FirstOrDefault();
            if (parametro != null && parametro.Activado)
            {
                var now = DateTime.Now;
                if (now.Day == parametro.envio_1 || now.Day == parametro.envio_2)
                {
                    var establecimientos = Manager.Establecimiento.Get(t => t.EnviarCorreo
                                                                            && (t.ultima_notificacion == null
                                                                            || (now.Year >= t.ultima_notificacion.GetValueOrDefault().Year
                                                                            && now.Month >= t.ultima_notificacion.GetValueOrDefault().Month
                                                                            && now.Day > t.ultima_notificacion.GetValueOrDefault().Day
                                                                            )

                                                        )).ToList();
                    var temp = now.AddMonths(-1);
                    establecimientos =
                        establecimientos.Where(
                            t =>
                                t.Encuestas.OfType<EncuestaEstadistica>()
                                    .Any(h => h.EstadoEncuesta == EstadoEncuesta.NoEnviada && (h.Fecha.Year == temp.Year && h.Fecha.Month == temp.Month))).ToList();
                    //var to =
                    //    establecimientos.Where(
                    //        t => t.ContactoPredeterminado != null && t.ContactoPredeterminado.Correo != null)
                    //        .Select(t => t.ContactoPredeterminado.Correo).ToArray();

                    foreach (var s in establecimientos)
                    {
                        var predeterminado = s.ContactoPredeterminado;
                        if(predeterminado==null || predeterminado.Correo==null)continue;
                        var s1 = predeterminado.Correo;
                        var s2 = s;
                        ThreadPool.QueueUserWorkItem(state =>
                        {
                            s2.ultima_notificacion = DateTime.Now;
                            Manager.Establecimiento.Modify(s2);
                            Manager.Establecimiento.SaveChanges();
                            var thread = new Thread(Exc);
                            thread.Start(new[] { s1, parametro.mensaje });
                        });
                    }
                }

            }
        }
        public void EnviarNotificacionesEncuestaEmpresarial()
        {
            var parametro = Get(t => t.tipo_encuesta == "Empresarial").FirstOrDefault();
            if (parametro != null && parametro.Activado)
            {
                var now = DateTime.Now;
                if (now.Day == parametro.envio_1 || now.Day == parametro.envio_2)
                {
                    var establecimientos = Manager.Establecimiento.Get(t => t.EnviarCorreo
                                                                            && (t.ultima_notificacion == null
                                                                            || (now.Year >= t.ultima_notificacion.GetValueOrDefault().Year
                                                                            && now.Month >= t.ultima_notificacion.GetValueOrDefault().Month
                                                                            && now.Day > t.ultima_notificacion.GetValueOrDefault().Day
                                                                            )

                                                        )).ToList();
                    var temp = now.AddMonths(-1);
                    establecimientos =
                        establecimientos.Where(
                            t =>
                                t.Encuestas.OfType<EncuestaEmpresarial>()
                                    .Any(h => h.EstadoEncuesta == EstadoEncuesta.NoEnviada && (h.Fecha.Year == temp.Year && h.Fecha.Month == temp.Month))).ToList();
                    //var to =
                    //    establecimientos.Where(
                    //        t => t.ContactoPredeterminado != null && t.ContactoPredeterminado.Correo != null)
                    //        .Select(t => t.ContactoPredeterminado.Correo).ToArray();

                    foreach (var s in establecimientos)
                    {
                        var predeterminado = s.ContactoPredeterminado;
                        if (predeterminado == null || predeterminado.Correo == null) continue;
                        var s1 = predeterminado.Correo;
                        var s2 = s;
                        ThreadPool.QueueUserWorkItem(state =>
                        {
                            s2.ultima_notificacion = DateTime.Now;
                            Manager.Establecimiento.Modify(s2);
                            Manager.Establecimiento.SaveChanges();
                            var thread = new Thread(Exc);
                            thread.Start(new[] { s1, parametro.mensaje });
                        });
                    }
                }

            }
        }

        void Exc(Object ob)
        {
            try
            {
                var array = (string[])ob;
                var message = array[1];
                var to = array[0];
                to.EnviarCorreo("Notificación", message);
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
