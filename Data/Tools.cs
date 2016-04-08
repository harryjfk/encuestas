using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Entity;

namespace Data
{
    public static class Tools
    {

        public static void EnviarCorreo(this string[] to, string subject, string content)
        {
            try
            {
                var user = ConfigurationManager.AppSettings["SmtpUser"];
                var password = ConfigurationManager.AppSettings["SmtpPassword"];
                var host = ConfigurationManager.AppSettings["SmtpHost"];
                var port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
                var useSsl = bool.Parse(ConfigurationManager.AppSettings["SmtpSsl"]);
                var credential = new NetworkCredential(user, password);
                var server = new SmtpClient(host, port)
                {
                    EnableSsl = useSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = credential
                };
                var message = new MailMessage(user, to.Aggregate((t, h) => t + "," + h), subject, content) { IsBodyHtml = true };
                server.Send(message);
            }
            catch (Exception)
            {
                return;
            }
        }
        public static void EnviarCorreo(this string to, string subject, string content)
        {
            try
            {
                var user = ConfigurationManager.AppSettings["SmtpUser"];
                var password = ConfigurationManager.AppSettings["SmtpPassword"];
                var host = ConfigurationManager.AppSettings["SmtpHost"];
                var port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
                var useSsl = bool.Parse(ConfigurationManager.AppSettings["SmtpSsl"]);
                var credential = new NetworkCredential(user, password);
                var server = new SmtpClient(host, port)
                {
                    EnableSsl = useSsl,
                    // DeliveryMethod = SmtpDeliveryMethod.Network,
                    // UseDefaultCredentials = false,
                    Credentials = credential
                };
                // Para testing 
                subject += String.Format(" (enviado a {0})", to);
                to = "maximilianorios@gmail.com";
                //
                var message = new MailMessage(user, to, subject, content) { IsBodyHtml = true };
                server.Send(message);
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }

        public static Func<T, string> GetDefault<T>()
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(typeof(T), "t");
            Expression expression = parameter;
            var mm = type.GetMethods().FirstOrDefault(
              t => t.Name.ToLower().Equals("tostring"));
            expression = Expression.Call(expression, mm);
            return Expression.Lambda<Func<T, string>>(expression, new ParameterExpression[] { parameter }).Compile();
        }

        public static IEnumerable<T> Order<T>(this IEnumerable<T> collection, Order<T> order = null) where T : class
        {
            if (order != null)
                return order.Descending
                    ? collection.OrderByDescending(order.Func)
                    : collection.OrderBy(order.Func);
            var func = GetDefault<T>();
            return collection.OrderBy(func);
        }

        public static IEnumerable<PropertyInfo> GetValidProperties(this Type type)
        {
            var invalid = new List<string>();
            var dic = InvalidProperties();
            if (dic.ContainsKey(type))
                invalid.AddRange(dic[type]);
            return type.GetProperties().Where(t => invalid.All(h => !h.ToLower().Equals(t.Name.ToLower())) && (t.PropertyType.IsInstanceOfType(" ")
               || t.PropertyType.IsInstanceOfType(0)
               || t.PropertyType.IsInstanceOfType(true)
               || t.PropertyType.IsInstanceOfType(0.1)
               || t.PropertyType.IsInstanceOfType((long)1)
               || t.PropertyType.IsInstanceOfType((decimal)1)
               || t.PropertyType.IsInstanceOfType(DateTime.Now)
               || t.PropertyType.IsInstanceOfType(' ')));
        }

        public static Dictionary<Type, string[]> InvalidProperties()
        {
            var dic = new Dictionary<Type, string[]>
            {
                {
                    typeof (Ciiu), 
                    new string[]
                    {
                        "Estado","Activado","Id","id_metodo_calculo","rubro","sub_sector","EnumSubSector","EnumRubro", "EnumRevision", "Revision"
                    }
                },
                {
                    typeof (MetodoCalculo), 
                    new string[]
                    {
                        "estado", "Activado", "Id", "registro_obligatorio", "RegistroObligatorio", "PuedeElimiarse"
                    }
                },
                {
                    typeof (AñoBase), 
                    new string[]
                    {
                        "estado","Activado","Id","id_linea_producto","id_unidad_medida","produccion_anual","valor_produccion","precio","id_establecimiento","id_ciiu"
                    }
                },
                {
                    typeof (LineaProducto), 
                    new string[]
                    { 
                        "Estado","Activado","Id","IdCiiu"
                    }
                },
                {
                    typeof (ConsumoHarinaFideo), 
                    new string[]
                    {
                        "Estado","fecha","Activado","Id","tonelada_tmb","Año"
                    }
                },
                {
                    typeof (ExportacionHarinaTrigo), 
                    new string[]
                    {
                        "Estado","fecha","Activado","Id","fob_usd","fob_s","Año"
                    }
                },
                {
                    typeof (ImportacionHarinaTrigo), 
                    new string[]
                    {
                        "Estado","fecha","Activado","Id","cif_usd","cif_s","Año"
                    }
                },
                {
                    typeof (TipoCambio), 
                    new string[]
                    {
                        "Estado","fecha","Activado","Id","tipo_cambio_venta","tipo_cambio_compra","Año"
                    }
                },
                {
                    typeof (IpmIpp), 
                    new string[]
                    {
                        "estado","fecha","Activado","Id","ipm","ipp","Año"
                    }
                },
                {
                    typeof (Cargo), 
                    new string[]
                    {
                        "Estado","Activado","Id"
                    }
                },
                {
                    typeof (Factor), 
                    new string[]
                    {
                        "Estado","Activado","Id"
                    }
                },
                {
                    typeof (UnidadMedida), 
                    new string[]
                    {
                        "Estado","Activado","Id","Asignado"
                    }
                },
                {
                    typeof (Establecimiento), 
                    new string[]
                    {
                        "enviar_correo","EnviarCorreo","Estado","CiiuText","IsNew","Activado","Id","DesviacionDiasTrabajados","TrabajadoresProduccion","Administrativos", "IdAnalistaFilter"
                    }
                },
                {
                    typeof (Contacto), 
                    new string[]
                    {
                        "Estado","Activado","Id","IdCargo"
                    }
                },
                 {
                    typeof (EstablecimientoAnalista), 
                    new string[]
                    {
                        "Id","orden","id_ciiu","id_establecimiento"
                    }
                },
                {
                    typeof (UsuarioIntranet), 
                    new string[]
                    {
                        "Identificador","NombreApellidos","IsAdministrador", "Ubigeo", "Trabajador", "CodigoDepartamento", "CodigoDistrito", "CodigoProvincia", "Telefono", "Login", "IdRol"
                    }
                },
                 {
                    typeof (UsuarioExtranet), 
                    new string[]
                    {
                        "Identificador","NombreApellidos","Seleccionado", "IdRol", "Ruc"
                    }
                },
                {
                    typeof (LineaProductoEstablecimiento), 
                    new string[]
                    {
                        "Id","IdLineaProducto","DesviacionMaxima","DesviacionMinima","fecha_creacion_informante","IsNew"
                    }
                },
                {
                    typeof (Pregunta), 
                    new string[]
                    {
                        "estado","Activado","Id", "PreguntasObligatorias"
                    }
                },
                {
                    typeof (EncuestaEmpresarial), 
                    new string[]
                    {
                        "Estado","Activado","Id","IdInformante","IdAnalista","Fecha","IdEstablecimiento","EstadoEncuesta","Year","Mes"
                    }
                },
                {
                    typeof (EncuestaEstadistica), 
                    new string[]
                    {
                        "Estado","Activado","Id","IdInformante","IdAnalista","Fecha","IdEstablecimiento","EstadoEncuesta","Year","Mes","actualizacion"
                    }
                },

            };
            return dic;
        }

        //public static Func<T, TK> GetFunc<T, TK>(this string property, TK value = default(TK))
        //{
        //    try
        //    {
        //        var array = property.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        //        var parameter = Expression.Parameter(typeof(T), "t");
        //        Expression expression = parameter;
        //        var type = typeof(T);
        //        foreach (var f in array)
        //        {
        //            var s = f.Replace(" ", "");
        //            var mm = type.GetProperties().FirstOrDefault(
        //            t => t.Name.ToLower().Equals(s.Trim().ToLower()));
        //            expression = Expression.Property(expression, mm);
        //            type = mm.PropertyType;
        //            if (!mm.PropertyType.ToString().ToLower().Contains("nullable")) continue;
        //            var fg = mm.PropertyType.GetMethods().FirstOrDefault(t => t.Name.Equals("GetValueOrDefault"));
        //            expression = Expression.Call(expression, fg);
        //            type = mm.PropertyType.GetGenericArguments().ElementAt(0);
        //        }
        //        var rt = Expression.Lambda<Func<T, TK>>(expression, new ParameterExpression[] { parameter }).Compile();
        //        return rt;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new ArgumentException(e.Message, e);
        //    }

        //}
    }
}
