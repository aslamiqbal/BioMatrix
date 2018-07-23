using IqbalCoreLibrary.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioMetrixCore.Utilities
{
  public  class DBAccess
    {
         
        public static string connectionstring { get {
                return System.Configuration.ConfigurationManager.ConnectionStrings["ZK_db"].ConnectionString;
            } }
        public static string connectionstringMainDB
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["mems"].ConnectionString;
            }
        }
        public static MsSql Sql { get {
                return new MsSql(connectionstring);
            } }
       
        public static MsSql SqlMainDB
        {
            get
            {
                return new MsSql(connectionstringMainDB);
            }
        }
        public static void CloseConnection(MsSql msSql)
        {
            msSql.Close();
            msSql.Dispose();
        }
    }
}
