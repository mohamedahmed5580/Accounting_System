using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Accounting_System.Repositories
{
    public static class DapperHelper
    {
        // Use the existing connection string logic from DataAccessLayer
        public static string ConnectionString => DataAccessLayer.Con();

        public static IDbConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        // --- Helper Methods to Reduce Boilerplate ---

        public static async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null)
        {
            using (var db = GetConnection())
            {
               return await db.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
            }
        }
        
        public static T QueryFirstOrDefault<T>(string sql, object param = null, IDbTransaction transaction = null)
        {
            using (var db = GetConnection())
            {
               return db.QueryFirstOrDefault<T>(sql, param, transaction);
            }
        }

        public static async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null)
        {
            using (var db = GetConnection())
            {
                return await db.QueryAsync<T>(sql, param, transaction);
            }
        }
        
         public static IEnumerable<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null)
        {
            using (var db = GetConnection())
            {
                return db.Query<T>(sql, param, transaction);
            }
        }

        public static async Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null)
        {
             using (var db = GetConnection())
            {
                return await db.ExecuteAsync(sql, param, transaction);
            }
        }
        
        public static int Execute(string sql, object param = null, IDbTransaction transaction = null)
        {
             using (var db = GetConnection())
            {
                return db.Execute(sql, param, transaction);
            }
        }
        
        public static T ExecuteScalar<T>(string sql, object param = null, IDbTransaction transaction = null)
        {
             using (var db = GetConnection())
            {
                return db.ExecuteScalar<T>(sql, param, transaction);
            }
        }
        
    }
}
