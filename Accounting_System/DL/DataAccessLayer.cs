using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using Dapper;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Accounting_System
{
    /// <summary>
    /// Provides static methods for interacting with the database using Dapper.
    /// IMPORTANT: This has been refactored to use Dapper internally while maintaining backward compatibility.
    /// All public methods preserve their original signatures so existing code works without modification.
    /// </summary>
    [Obsolete("Do not use static connection directly. Use the DataAccessLayer methods instead.")]
    public static class DataAccessLayer
    {
        /// <summary>
        /// Gets the connection string based on application settings.
        /// </summary>
        public static string Con()
        {
            if (Accounting_System.Properties.Settings.Default.Mode == true)
            {
                return string.Format(@"Data Source={0}; Initial Catalog={1};Integrated Security=true",
                    Accounting_System.Properties.Settings.Default.Server,
                    Accounting_System.Properties.Settings.Default.Database);
            }
            else
            {
                return string.Format("Data Source={0},{1};Initial Catalog={2};Integrated Security=false;User ID={3};Password={4}",
                    Accounting_System.Properties.Settings.Default.Server,
                    "1433",
                    Accounting_System.Properties.Settings.Default.Database,
                    Accounting_System.Properties.Settings.Default.Name,
                    Accounting_System.Properties.Settings.Default.Pass);
            }
        }

        /// <summary>
        /// DEPRECATED: This static connection is kept ONLY for legacy compatibility.
        /// NEW CODE SHOULD NOT USE THIS. It will be initialized but never used internally.
        /// </summary>
        [Obsolete("Do not use static connection directly. Use the DataAccessLayer methods instead.")]
        public static SqlConnection cn = new SqlConnection(Con());

        /// <summary>
        /// Helper method to convert SqlParameter[] to Dapper DynamicParameters
        /// </summary>
        private static DynamicParameters ConvertToDapperParameters(SqlParameter[] parameters)
        {
            var dynamicParams = new DynamicParameters();
            if (parameters != null && parameters.Length > 0)
            {
                foreach (var param in parameters)
                {
                    dynamicParams.Add(param.ParameterName, param.Value, param.DbType, param.Direction, param.Size);
                }
            }
            return dynamicParams;
        }

        /// <summary>
        /// Executes a Transact-SQL statement and returns the number of rows affected.
        /// </summary>
        public static int ExecuteNonQuery(string query, CommandType commandType, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(Con()))
            {
                var dapperParams = ConvertToDapperParameters(parameters);
                return connection.Execute(query, dapperParams, commandType: commandType);
            }
        }

        /// <summary>
        /// Executes the query and returns the results as a DataTable.
        /// BACKWARD COMPATIBILITY: Converts Dapper results to DataTable for existing code.
        /// </summary>
        public static DataTable ExecuteTable(string query, CommandType commandType, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(Con()))
            {
                var dapperParams = ConvertToDapperParameters(parameters);

                // Use Dapper to query and convert to DataTable
                var reader = connection.ExecuteReader(query, dapperParams, commandType: commandType);
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
            }
        }

        /// <summary>
        /// LEGACY: Returns DataTable asynchronously (though DataTable.Load is still synchronous internally)
        /// Prefer using QueryAsync<T> for new code.
        /// </summary>
        public static async Task<DataTable> ExecuteTableAsync(string query, CommandType commandType, params SqlParameter[] parameters)
        {
            using (SqlConnection cnLocal = new SqlConnection(Con()))
            {
                await cnLocal.OpenAsync();
                var dapperParams = ConvertToDapperParameters(parameters);

                // ExecuteReader async
                var reader = await cnLocal.ExecuteReaderAsync(query, dapperParams, commandType: commandType);

                // DataTable.Load is synchronous (no async version exists)
                DataTable dt = new DataTable();
                dt.Load(reader);

                return dt;
            }
        }

        /// <summary>
        /// Executes the query and returns a SqlDataReader.
        /// IMPORTANT: Caller is responsible for closing the reader and connection.
        /// </summary>
        public static SqlDataReader ExecuteReader(string query, CommandType commandType, params SqlParameter[] parameters)
        {
            SqlConnection connection = new SqlConnection(Con());
            connection.Open();

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    cmd.Parameters.Add(new SqlParameter(param.ParameterName, param.SqlDbType) { Value = param.Value ?? DBNull.Value });
                }
            }

            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// Async version of ExecuteReader
        /// </summary>
        public static async System.Threading.Tasks.Task<SqlDataReader> ExecuteReaderAsync(string query, CommandType commandType, params SqlParameter[] parameters)
        {
            SqlConnection connection = new SqlConnection(Con());
            await connection.OpenAsync();

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.CommandType = commandType;
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    cmd.Parameters.Add(new SqlParameter(param.ParameterName, param.SqlDbType) { Value = param.Value ?? DBNull.Value });
                }
            }

            try
            {
                return (SqlDataReader)await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }
            catch
            {
                connection.Close();
                throw;
            }
        }

        /// <summary>
        /// Executes the query and returns the first column of the first row.
        /// </summary>
        public static object ExecuteScalar(string query, CommandType commandType, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(Con()))
            {
                var dapperParams = ConvertToDapperParameters(parameters);
                return connection.ExecuteScalar(query, dapperParams, commandType: commandType);
            }
        }

        /// <summary>
        /// Fills a DataSet with data from the query.
        /// </summary>
        public static void FillDataSet(DataSet ds, string tableName, string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            if (ds == null)
                throw new ArgumentNullException(nameof(ds), "DataSet cannot be null.");
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName), "Table name cannot be null or empty.");

            using (SqlConnection connection = new SqlConnection(Con()))
            {
                var dapperParams = ConvertToDapperParameters(parameters);
                var reader = connection.ExecuteReader(commandText, dapperParams, commandType: commandType);

                DataTable dataTable = new DataTable(tableName);
                dataTable.Load(reader);

                ds.Tables.Add(dataTable);
            }
        }

        /// <summary>
        /// Creates a SqlParameter.
        /// </summary>
        public static SqlParameter CreateParameter(string name, SqlDbType type, object value)
        {
            return new SqlParameter(name, type) { Value = value ?? DBNull.Value };
        }

        /// <summary>
        /// Creates a SqlParameter with size.
        /// </summary>
        public static SqlParameter CreateParameter(string name, SqlDbType type, int size, object value)
        {
            return new SqlParameter(name, type, size) { Value = value ?? DBNull.Value };
        }

        /// <summary>
        /// Logs activity to console (or database if implemented)
        /// </summary>
        public static void LogActivity(string user, string action)
        {
            Console.WriteLine($"ACTIVITY LOG: User='{user}', Action='{action}'");
        }

        // ==================== DAPPER-NATIVE METHODS (Optional, for new code) ====================

        /// <summary>
        /// NEW: Execute a query and return strongly-typed results using Dapper.
        /// Use this for new code to avoid DataTable overhead.
        /// </summary>
        public static IEnumerable<T> Query<T>(string query, object parameters = null, CommandType commandType = CommandType.Text)
        {
            using (SqlConnection connection = new SqlConnection(Con()))
            {
                return connection.Query<T>(query, parameters, commandType: commandType);
            }
        }

        /// <summary>
        /// NEW: Execute a query and return a single result or null.
        /// </summary>
        public static T QueryFirstOrDefault<T>(string query, object parameters = null, CommandType commandType = CommandType.Text)
        {
            using (SqlConnection connection = new SqlConnection(Con()))
            {
                return connection.QueryFirstOrDefault<T>(query, parameters, commandType: commandType);
            }
        }

        /// <summary>
        /// NEW: Execute a command (INSERT/UPDATE/DELETE) using Dapper.
        /// </summary>
        public static int Execute(string query, object parameters = null, CommandType commandType = CommandType.Text)
        {
            using (SqlConnection connection = new SqlConnection(Con()))
            {
                return connection.Execute(query, parameters, commandType: commandType);
            }
        }

        /// <summary>
        /// NEW: Execute scalar query using Dapper with strongly-typed result.
        /// </summary>
        public static T ExecuteScalar<T>(string query, object parameters = null, CommandType commandType = CommandType.Text)
        {
            using (SqlConnection connection = new SqlConnection(Con()))
            {
                return connection.ExecuteScalar<T>(query, parameters, commandType: commandType);
            }
        }

        public static async Task ExecuteSafeAction(Button btn, Action action)
        {
            if (btn == null) return;

            btn.Enabled = false;
            try
            {
                await Task.Run(action);
            }
            finally
            {
                // نستخدم Invoke لأننا قد نكون في Thread مختلف
                btn.Invoke((Action)(() => btn.Enabled = true));
            }
        }
    }
}