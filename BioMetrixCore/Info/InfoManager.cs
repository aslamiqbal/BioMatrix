using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore.Info
{
    class InfoManager
    {
        SqlConnection connection = new SqlConnection(Info.DataManager.OraConnString());
        public void SaveInformation(ICollection<MachineInfo> lstMachineInfo)
        {

            String connectionString = Info.DataManager.OraConnString();
            foreach (MachineInfo ms in lstMachineInfo)
            {
                string query = @"INSERT INTO ZKAttnMst([DeviceID],[UserID],[LogTime],[IsSend]) VALUES ";
                query+=$"('{ms.MachineNumber}','{ms.IndRegID}','{ms.DateTimeRecord.ToString()}',0)";
               // Info.DataManager.ExecuteNonQuery(connectionString, query);
                 
            }
        }
    }
}
