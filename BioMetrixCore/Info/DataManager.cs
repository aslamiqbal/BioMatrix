using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore.Info
{
    class DataManager
    {
        public static string OraConnString()
        {
            return String.Format("Server=.;Database=ZKAccess;User ID=sa;Password=newtec.com;Trusted_Connection=False;");
        }
        public static void ExecuteNonQuery(string ConnectionString, string query)
        {
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand myCommand = new SqlCommand(query, myConnection))
                {
                    myConnection.Open();
                    myCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
