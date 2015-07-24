using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DataAccess
{
    public class Operacion
    {
        public static DataView Ejecutar(SqlConnection connection, string query,params SqlParameter [] parameters)
        {
            var command = new SqlCommand(query, connection);
            foreach (var sqlParameter in parameters)
            {
                command.Parameters.Add(sqlParameter);
            }
            var adapter = new SqlDataAdapter(command);
            var data = new DataSet();
            adapter.Fill(data);
            return data.Tables.Count > 0 ? data.Tables[0].DefaultView : null;
        }
    }
}
