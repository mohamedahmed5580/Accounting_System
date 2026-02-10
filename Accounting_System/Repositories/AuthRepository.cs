using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Accounting_System.Repositories;

namespace Accounting_System.Repositories
{
    public class AuthRepository
    {
        public class UserInfo
        {
            public string UserID { get; set; }
            public string Password { get; set; }
            public string UserType { get; set; }
            public string Active { get; set; }
        }

        public UserInfo Login(string userId, string password)
        {
            string sql = @"
                SELECT UserID, Password, UserType, Active 
                FROM Registration 
                WHERE UserID = @UserID AND Password = @Password AND Active = 'Yes'";

            return DapperHelper.QueryFirstOrDefault<UserInfo>(sql, new { UserID = userId, Password = password });
        }
        
        // Example of Async Login (Recommended for future)
        public async Task<UserInfo> LoginAsync(string userId, string password)
        {
             string sql = @"
                SELECT UserID, Password, UserType, Active 
                FROM Registration 
                WHERE UserID = @UserID AND Password = @Password AND Active = 'Yes'";

            return await DapperHelper.QueryFirstOrDefaultAsync<UserInfo>(sql, new { UserID = userId, Password = password });
        }
    }
}
