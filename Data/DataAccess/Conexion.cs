using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Data.DataAccess
{
    public class Conexion
    {
        private static SqlConnection _conexion { get; set; }
        private static Conexion _singleConexion { get; set; }
        private string DataSource { get; set; }
        private string Catalog { get; set; }
        private string UserId { get; set; }
        private string Password { get; set; }

        public Conexion(string dataSource, string catalog, string userId, string password)
        {
            DataSource = dataSource;
            Catalog = catalog;
            UserId = userId;
            Password = password;
        }
        private SqlConnection Nuevo()
        {
            var sec = new SecureString();
            foreach (var ch in Password)
            {
                sec.AppendChar(ch);
            }
            sec.MakeReadOnly();
            var con = new SqlConnection(string.Format(@"Data Source={0};Initial Catalog={1};", DataSource, Catalog),
                new SqlCredential(UserId, sec));
            con.Open();
            return con;
        }

        public SqlConnection Crear()
        {
            return _conexion ?? (_conexion = Nuevo());
        }

        public static Conexion CrearConexion()
        {
            return _singleConexion ??
                   (_singleConexion =
                       new Conexion(ConfigurationManager.AppSettings["dataSource"],
                           ConfigurationManager.AppSettings["catalog"],
                           ConfigurationManager.AppSettings["user"],
                           ConfigurationManager.AppSettings["password"]));
        }


    }
}
