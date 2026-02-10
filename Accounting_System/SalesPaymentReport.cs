
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Accounting_System
{
    public partial class SalesPaymentReport : Form
    {
        public SalesPaymentReport()
        {
            InitializeComponent();
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    Cursor = Cursors.WaitCursor;
                    Timer1.Enabled = true;
                    frmReport frmReport = new frmReport();

                    con.Open();
                    string query = @"
            SELECT 
                Invoice_Product.InvoiceID AS EntityID,
                Invoice_Product.ProductID,
                Product.ProductName,
                Invoice_Product.Barcode,
                Invoice_Product.Qty,
                Invoice_Product.TotalAmount,
                Temp_Stock.Qty AS StockQty,
                Invoice_Payment.PaymentDate,
                'بيع' AS EntityType
            FROM 
                Invoice_Product
            JOIN 
                Invoice_Payment ON Invoice_Product.InvoiceID = Invoice_Payment.InvoiceID
            JOIN 
                Product ON Invoice_Product.ProductID = Product.PID
            JOIN 
                Temp_Stock ON Temp_Stock.ProductID = Product.PID
            WHERE 
                Invoice_Payment.PaymentDate BETWEEN @d1 AND @d2
                AND Product.ProductName LIKE '%' + @productName + '%'

            UNION ALL

            SELECT 
                Stock_Product.StockID AS EntityID,
                Stock_Product.ProductID,
                Product.ProductName,
                Stock_Product.Barcode,
                Stock_Product.Qty,
                -Stock_Product.TotalAmount AS TotalAmount, -- Make TotalAmount negative
                Temp_Stock.Qty AS StockQty,
                Stock.Date AS PaymentDate,
                'شراء' AS EntityType
            FROM 
                Stock_Product
            JOIN 
                Product ON Stock_Product.ProductID = Product.PID
            JOIN 
                Stock ON Stock.ST_ID = Stock_Product.StockID
            JOIN 
                Temp_Stock ON Temp_Stock.ProductID = Product.PID
            WHERE 
                Stock.Date BETWEEN @d1 AND @d2
                AND Product.ProductName LIKE '%' + @productName + '%'
            ORDER BY 
                Product.ProductName;";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", dtpDateFrom.Value.Date); // Replace startDate with your variable
                        cmd.Parameters.AddWithValue("@d2", dtpDateTo.Value.Date);   // Replace endDate with your variable
                        cmd.Parameters.AddWithValue("@productName", comboBox1.Text); // Replace productName with your variable

                        using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            dt.Columns.Add("EntityID", typeof(int));
                            dt.Columns.Add("ProductID", typeof(string));
                            dt.Columns.Add("ProductName", typeof(string));
                            dt.Columns.Add("Barcode", typeof(string));
                            dt.Columns.Add("Qty", typeof(double));
                            dt.Columns.Add("TotalAmount", typeof(double));
                            dt.Columns.Add("StockQty", typeof(double));
                            dt.Columns.Add("EntityType", typeof(string));

                            // Fill the DataTable with data from the SQL query
                            adp.Fill(dt);

                            rptGeneralSPReport rpt = new rptGeneralSPReport();
                            rpt.SetDataSource(dt);

                            frmReport.crystalReportViewer1.ReportSource = rpt;
                            frmReport.ShowDialog();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                Timer1.Enabled = false;
            }


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void LoadProductNames()
        {
            // Define the query to select both PID (or ProductCode) and ProductName for binding
            string query = "SELECT PID, ProductName FROM [dbo].[Product]";

            using (SqlConnection connection = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable productTable = new DataTable();

                    // Fill DataTable with product data
                    adapter.Fill(productTable);

                    // Bind to comboBox1
                    comboBox1.DataSource = productTable;
                    comboBox1.DisplayMember = "ProductName"; // What user sees in the ComboBox
                    comboBox1.ValueMember = "PID";           // The actual value behind each item (e.g., PID)


                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }

        private void SalesPaymentReport_Load(object sender, EventArgs e)
        {
            LoadProductNames();
        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }
    }
}
