using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting_System
{
 
    public interface IProductRepository
    {
        Products GetProductById(int id);
        IEnumerable<Products> GetAllProducts();
        void AddProduct(Products product);
        void UpdateProduct(Products product);
        void DeleteProduct(int id);
    }

    public class SqlProductRepository : IProductRepository
    {
        public Products GetProductById(int id)
        {
            // TODO: use using blocks and ExecuteScalar or ExecuteReader as needed
            throw new NotImplementedException();
        }

        public IEnumerable<Products> GetAllProducts()
        {
            // TODO: Use DataReader to build a list of domain objects
            throw new NotImplementedException();
        }

        public void AddProduct(Products product)
        {
            // TODO: Build SqlCommand with parameters from product and execute insert
            throw new NotImplementedException();
        }

        public void UpdateProduct(Products product)
        {
            throw new NotImplementedException();
        }

        public void DeleteProduct(int id)
        {
            throw new NotImplementedException();
        }
    }

    // Domain model representing a single product.
  
}
