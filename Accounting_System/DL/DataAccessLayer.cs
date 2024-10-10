using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Pharmacy.DL
{
    class DataAccessLayer
    {

        public static string Con()
        {
            if (Accounting_System.Properties.Settings.Default.Mode == true)
                return string.Format("Data Source={0}; Initial Catalog={1};Integrated Security=true", Accounting_System.Properties.Settings.Default.Server, Accounting_System.Properties.Settings.Default.Database);
            else
            {
                // Remote connection with port number
                return string.Format("Data Source={0},{1};Initial Catalog={2};Integrated Security=false;User ID={3};Password={4}",
                    Accounting_System.Properties.Settings.Default.Server,
                    "1433", // Specify the port here
                    Accounting_System.Properties.Settings.Default.Database,
                    Accounting_System.Properties.Settings.Default.Name,
                    Accounting_System.Properties.Settings.Default.Pass);
            }
        }

        public static SqlConnection cn = new SqlConnection(Con());      

        private static void Open()
        {
            if (cn.State == ConnectionState.Closed)
            {
                try
                {
                    cn.Open();
                }
                catch (SqlException ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                }
            }
                
        }

        private static void Close()
        {
            if (cn.State == ConnectionState.Open)
            {
                try
                {
                    cn.Close();
                }
                catch (SqlException ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                }
            }
                
        }
        public static void ExecuteNonQuery(string Query, CommandType Type, params SqlParameter[] parameters)
        {
            try
            {
                Open();
                SqlCommand Command = new SqlCommand(Query, cn);
                Command.CommandType = Type;
                Command.Parameters.AddRange(parameters);
                Command.ExecuteNonQuery();
                Close();
            }
            catch (SqlException ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        public static DataTable ExecuteTable(string Query, CommandType Type, params SqlParameter[] parameters)
        {

            try
            {
                Open();
                SqlCommand Command = new SqlCommand(Query, cn);
                Command.CommandType = Type;
                Command.Parameters.AddRange(parameters);

                SqlDataAdapter da = new SqlDataAdapter(Command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Close();
                return dt;
            }
            catch (SqlException ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                return new DataTable();
            }
        }

        public static SqlParameter CreateParameter(string name, SqlDbType Type, object Value)
        {
            SqlParameter param = new SqlParameter();
            param.ParameterName = name;
            param.SqlDbType = Type;
            param.Value = Value;
            return param;
        }

    }
}
