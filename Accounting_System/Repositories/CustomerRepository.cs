using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Accounting_System.Repositories;

namespace Accounting_System.Repositories
{
    public class CustomerRepository
    {
        public class Customer
        {
            public int ID { get; set; }
            public string CustomerID { get; set; }
            public string Name { get; set; }
            public string Gender { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string ContactNo { get; set; }
            public string EmailID { get; set; }
            public string Remarks { get; set; }
            public string State { get; set; }
            public string ZipCode { get; set; }
            public byte[] Photo { get; set; }
            public decimal OpeningBalance { get; set; }
            public string OpeningBalanceType { get; set; }
            public string CustomerType { get; set; }
        }

        public IEnumerable<string> GetStates()
        {
             string sql = "SELECT DISTINCT RTRIM(State) FROM Customer ORDER BY 1";
             return DapperHelper.Query<string>(sql);
        }

        public Customer GetCustomerById(string customerId)
        {
            // Note: The original code queried by 'ID' column (int), but here we might want to query by 'CustomerID' string as well or ID.
            // Based on AddCustomer.cs line 53: SELECT * FROM Customer WHERE ID = @ID
            string sql = "SELECT * FROM Customer WHERE ID = @ID";
            return DapperHelper.QueryFirstOrDefault<Customer>(sql, new { ID = customerId });
        }
        
         public Customer GetCustomerByCode(string customerCode)
        {
            string sql = "SELECT * FROM Customer WHERE CustomerID = @CustomerID";
            return DapperHelper.QueryFirstOrDefault<Customer>(sql, new { CustomerID = customerCode });
        }

        public bool CustomerNameExists(string name)
        {
            string sql = "SELECT COUNT(1) FROM Customer WHERE Name = @Name";
            int count = DapperHelper.ExecuteScalar<int>(sql, new { Name = name });
            return count > 0;
        }

        public string GetNextID()
        {
             // Logic from AddCustomer.cs GenerateID
             string sql = "SELECT TOP 1 ID FROM Customer ORDER BY ID DESC";
             int? lastId = DapperHelper.ExecuteScalar<int?>(sql);
             
             int nextId = (lastId ?? 0) + 1;
             return nextId.ToString("D3"); // 001, 002...
        }

        public void AddCustomer(Customer customer)
        {
             string sql = @"
                INSERT INTO Customer(ID, CustomerID, [Name], Gender, Address, City, ContactNo, EmailID, Remarks, State, ZipCode, Photo, OpeningBalance, OpeningBalanceType, CustomerType) 
                VALUES (@ID, @CustomerID, @Name, @Gender, @Address, @City, @ContactNo, @EmailID, @Remarks, @State, @ZipCode, @Photo, @OpeningBalance, @OpeningBalanceType, 'Regular')";
             
             DapperHelper.Execute(sql, customer);
        }

        public void UpdateCustomer(Customer customer)
        {
             string sql = @"
                UPDATE Customer 
                SET CustomerID = @CustomerID, [Name] = @Name, Gender = @Gender, Address = @Address, City = @City, 
                    ContactNo = @ContactNo, EmailID = @EmailID, Remarks = @Remarks, State = @State, ZipCode = @ZipCode, 
                    Photo = @Photo, CustomerType = 'Regular', OpeningBalance = @OpeningBalance, OpeningBalanceType = @OpeningBalanceType 
                WHERE ID = @ID";
             
             DapperHelper.Execute(sql, customer);
        }

        public void DeleteCustomer(string customerId)
        {
             string sql = "DELETE FROM Customer WHERE CustomerID = @CustomerID";
             DapperHelper.Execute(sql, new { CustomerID = customerId });
        }
    }
}
