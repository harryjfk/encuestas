using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Repositorios;
using Entity;
using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using Entity.Parciales;

namespace Domain.Managers
{
    public class IpmIppManager : GenericManager<IpmIpp>
    {
        public IpmIppManager(GenericRepository<IpmIpp> repository, Manager manager)
            : base(repository, manager)
        {
        }

        public IpmIppManager(Entities context, Manager manager)
            : base(context, manager)
        {
        }
        public override List<string> Validate(IpmIpp element)
        {
            var list = base.Validate(element);
            if (element.Id == 0) return list;
            list.RequiredAndNotZero(element, t => t.ipm, "IPM");
            //list.Required(element, t => t.ipp, "IPP");
            return list;
        }

        public void Generate(long idCiuu, int año)
        {
            var ciiu = Manager.Ciiu.Find(idCiuu);
            if (ciiu == null) return;
            for (int i = 1; i < 13; i++)
            {
                var element = Get(t => t.fecha.Month == i && t.fecha.Year == año && t.id_ciiu == idCiuu).FirstOrDefault();
                if (element == null)
                {
                    element = new IpmIpp()
                    {
                        fecha = new DateTime(año, i, 1),
                        Activado = true,
                        id_ciiu = idCiuu
                    };
                    Add(element);
                }
                SaveChanges();
            }
        }
        
        public List<IppCiiuValor> GetValorIppPorAnioMes(int anio, int mes)
        {
            SqlConnection connection = new SqlConnection();
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            List<IppCiiuValor> ippCiiusValor = new List<IppCiiuValor>();
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["IppConnection"].ConnectionString;
                
                connection.ConnectionString = connectionString;
                connection.Open();

                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select CIIU, [1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12] from dbo.View_report_4 where ANIO = @anio";
                cmd.Parameters.Add(new SqlParameter("@anio", anio));
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    IppCiiuValor ippCiiuValor = new IppCiiuValor();
                    ippCiiuValor.ciiu = reader["CIIU"].ToString();
                    if (reader[mes.ToString()] == DBNull.Value)
                    {
                        ippCiiuValor.valor = null;
                    }
                    else
                    {
                        ippCiiuValor.valor = Convert.ToDecimal(reader[mes.ToString()]);
                    }

                    ippCiiusValor.Add(ippCiiuValor);
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                
                cmd.Dispose();
                connection.Close();
                connection.Dispose();
            }

            return ippCiiusValor;
        }

        public void ProcessIpp(int anio, int mes)
        {
            var valoresIpp = GetValorIppPorAnioMes(anio, mes);
            
            foreach (var valorIpp in valoresIpp)
            {
                var element = Get(t => t.fecha.Month == mes && t.fecha.Year == anio && t.Ciiu.Codigo == valorIpp.ciiu).FirstOrDefault();

                if (element == null)
                {
                    long idCiuu = Manager.Ciiu.Get(t => t.Codigo == valorIpp.ciiu).FirstOrDefault().Id;
                    element = new IpmIpp()
                    {
                        fecha = new DateTime(anio, mes, 1),
                        Activado = true,
                        id_ciiu = idCiuu,
                        ipp = (valorIpp.valor == null) ? 0 : valorIpp.valor.Value
                    };
                    Add(element);
                }
                else
                {
                    element.ipp = (valorIpp.valor == null) ? 0 : valorIpp.valor.Value;
                    Modify(element);
                }
                SaveChanges();
            }            
        }

        public string SendIpp(decimal valorUnitarioFab, int factor, int anio, string mes, int idEstablecimiento, int idProducto, string ciiu, string codLineaProd)
        {
            SqlConnection connection = new SqlConnection();
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            string codRpta = "";
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["IppConnection"].ConnectionString;

                connection.ConnectionString = connectionString;
                connection.Open();

                cmd.Connection = connection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "P_GUARDAR_ENCUESTA";
                cmd.Parameters.Add(new SqlParameter("@VALOR_UV_FAB", valorUnitarioFab));
                cmd.Parameters.Add(new SqlParameter("@FACTOR", factor));
                cmd.Parameters.Add(new SqlParameter("@ANO_EJE", anio));
                cmd.Parameters.Add(new SqlParameter("@MES", mes));
                cmd.Parameters.Add(new SqlParameter("@ID_ESTABLECIMIENTO", idEstablecimiento));
                cmd.Parameters.Add(new SqlParameter("@ID_PROD", idProducto));
                cmd.Parameters.Add(new SqlParameter("@CIIU", ciiu));
                cmd.Parameters.Add(new SqlParameter("@COD", codLineaProd));
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    codRpta = reader["CO_RPTA"].ToString();
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                cmd.Dispose();
                connection.Close();
                connection.Dispose();
            }
            return codRpta;
        }
    }
}
