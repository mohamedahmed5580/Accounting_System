using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Accounting_System.Repositories;

namespace Accounting_System.Repositories
{
    public class SalesRepository
    {
        public class InvoiceInfo
        {
             public int Inv_ID { get; set; }
             public string InvoiceNo { get; set; }
             public DateTime InvoiceDate { get; set; }
             public int CustomerID { get; set; }
             public double GrandTotal { get; set; }
             public double TotalPaid { get; set; }
             public double Balance { get; set; }
             public string Remarks { get; set; }
             public int SalesmanID { get; set; }
             public int? TC_ID { get; set; } // Nullable
             public int WID { get; set; }
             public decimal total_sale { get; set; }
        }

        public class InvoiceProduct
        {
            public int InvoiceID { get; set; }
            public int ProductID { get; set; }
            public string Barcode { get; set; }
            public decimal CostPrice { get; set; }
            public decimal SellingPrice { get; set; }
            public decimal Margin { get; set; }
            public decimal Qty { get; set; }
            public decimal Amount { get; set; }
            public decimal DiscountPer { get; set; }
            public decimal Discount { get; set; }
            public decimal VATPer { get; set; }
            public decimal VAT { get; set; }
            public decimal TotalAmount { get; set; }
        }
        
        public class PaymentInfo
        {
             public int TC_ID { get; set; }
             public string TransactionID { get; set; }
             public DateTime Date { get; set; }
             public string PaymentMode { get; set; }
             public string CustomerID { get; set; }
             public decimal Amount { get; set; } // Negative for payments?
             public string Remarks { get; set; }
             public string Check_ID { get; set; }
             public DateTime Check_Date { get; set; }
             public int SalesMan_ID { get; set; }
             public string SalesMan_Name { get; set; }
             public decimal SalesMan_Comession { get; set; }
             public string SalesMan_ID_2 { get; set; }
        }

        public string GenerateNextInvoiceID()
        {
             string sql = "SELECT ISNULL(MAX(Inv_ID), 0) FROM InvoiceInfo";
             int lastId = DapperHelper.ExecuteScalar<int>(sql);
             return (lastId + 1).ToString("D4");
        }
        
        public int GetStockQuantity(int productId)
        {
             string sql = "SELECT Qty FROM Temp_Stock WHERE ProductID = @ProductID";
             return DapperHelper.ExecuteScalar<int>(sql, new { ProductID = productId });
        }

        public void SaveInvoice(InvoiceInfo invoice, List<InvoiceProduct> products, PaymentInfo payment = null)
        {
            using (var cn = DapperHelper.GetConnection())
            {
                cn.Open();
                using (var transaction = cn.BeginTransaction())
                {
                    try
                    {
                        // 1. Handle Payment/Transaction if exists
                        if (payment != null)
                        {
                            // Logic to check existing payment or insert new
                             // Simplified for brevity - assumes Insert
                             string sqlPay = @"INSERT INTO Payment_2(TC_ID, TransactionID, [Date], PaymentMode, CustomerID, Amount, Remarks, 
                                                Check_ID, Check_Date, SalesMan_ID, SalesMan_Name, SalesMan_Comession, SalesMan_ID_2) 
                                                VALUES (@TC_ID, @TransactionID, @Date, @PaymentMode, @CustomerID, @Amount, @Remarks, 
                                                @Check_ID, @Check_Date, @SalesMan_ID, @SalesMan_Name, @SalesMan_Comession, @SalesMan_ID_2)";
                             cn.Execute(sqlPay, payment, transaction);
                        }

                        // 2. Insert Invoice Header
                        string sqlInv = @"INSERT INTO InvoiceInfo(Inv_ID, InvoiceNo, InvoiceDate, CustomerID, GrandTotal, TotalPaid, Balance, Remarks, SalesmanID, TC_ID, WID, total_sale) 
                                        VALUES (@Inv_ID, @InvoiceNo, @InvoiceDate, @CustomerID, @GrandTotal, @TotalPaid, @Balance, @Remarks, @SalesmanID, @TC_ID, @WID, @total_sale)";
                        cn.Execute(sqlInv, invoice, transaction);

                        // 3. Insert/Update Products
                        // Note: The logic in POS.cs had a check for EXISTS, but usually for a new invoice it's INSERT.
                        string sqlProd = @"INSERT INTO Invoice_Product(InvoiceID, Barcode, CostPrice, SellingPrice, Margin, Qty, Amount, DiscountPer, Discount, VATPer, VAT, TotalAmount, ProductID) 
                                           VALUES (@InvoiceID, @Barcode, @CostPrice, @SellingPrice, @Margin, @Qty, @Amount, @DiscountPer, @Discount, @VATPer, @VAT, @TotalAmount, @ProductID)";
                        
                        cn.Execute(sqlProd, products, transaction); // Dapper executes for each item in list!

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
