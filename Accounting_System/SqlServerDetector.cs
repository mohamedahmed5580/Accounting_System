using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting_System
{
    public static class SqlServerDetector
    {
        public static string GetServerName()
        {
            string machineName = Environment.MachineName;

            string[] possibleServers =
            {
            @"(localdb)\MSSQLLocalDB",
            machineName + @"\MSSQLLocalDB",
            machineName + @"\SQLEXPRESS"
        };

            foreach (string server in possibleServers)
            {
                if (CanConnect(server))
                    return server;
            }

            return null; // لا يوجد سيرفر متاح
        }

        private static bool CanConnect(string serverName)
        {
            try
            {
                string connStr =
                    $"Data Source={serverName};Initial Catalog=master;Integrated Security=True;Connect Timeout=2";

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
