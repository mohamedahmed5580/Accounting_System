
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Accounting_System
{
    public partial class PaymentProductReport : Form
    {
        public PaymentProductReport()
        {
            InitializeComponent();
        }

        private void PaymentProductReport_Load(object sender, EventArgs e)
        {
            LoadProductNames();
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

        private void btnGetData_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // SQL query to get the data for the report
                    string query1 = @"SELECT 
                             Stock_Product.StockID, 
                             Stock_Product.ProductID, 
                             Product.ProductName , 
                             Supplier.SupplierID, 
                             Supplier.Name AS SupplierName ,
                             Stock_Product.Barcode , 
                             SUM(Stock_Product.Qty) AS Qty,  -- Aggregated quantity
                             SUM(Stock_Product.TotalAmount) AS TotalAmount,  -- Aggregated total amount
                             MAX(Temp_Stock.Qty) AS StockQty,  -- MAX to avoid repetition
                             MAX(Stock.Date) AS PaymentDate  -- Latest payment date
                        FROM 
                             Stock_Product
                        JOIN Product ON Stock_Product.ProductID = Product.PID
                        JOIN Stock ON Stock.ST_ID = Stock_Product.StockID
                        JOIN Temp_Stock ON Temp_Stock.ProductID = Product.PID
                        JOIN Supplier ON Supplier.ID = Stock.SupplierID
                        WHERE Stock.Date Between @d1 AND @d2
                                AND Product.ProductName LIKE '%' + @productName + '%' 
                        GROUP BY 
                             Stock_Product.StockID, 
                             Stock_Product.ProductID, 
                             Product.ProductName, 
                             Supplier.SupplierID, 
                             Supplier.Name, 
                             Stock_Product.Barcode
                        ORDER BY Product.ProductName;";

                    SqlCommand cmd = new SqlCommand(query1, con);
                    cmd.Parameters.AddWithValue("@d1", dtpDateFrom.Value.Date);  // Adjust for your date input controls
                    cmd.Parameters.AddWithValue("@d2", dtpDateTo.Value.Date);    // Adjust for your date input controls
                    cmd.Parameters.AddWithValue("@productName", comboBox1.Text);  // Assuming you have a TextBox for product name search

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    // Creating a DataTable to hold the data for the report
                    DataTable dt = new DataTable();
                    dt.Columns.Add("StockID", typeof(int));
                    dt.Columns.Add("ProductID", typeof(int));
                    dt.Columns.Add("ProductName", typeof(string));
                    dt.Columns.Add("Barcode", typeof(string));
                    dt.Columns.Add("Qty", typeof(float));
                    dt.Columns.Add("TotalAmount", typeof(float));
                    dt.Columns.Add("Units", typeof(string));
                    dt.Columns.Add("StockQty", typeof(float));
                    dt.Columns.Add("PaymentDate", typeof(DateTime));
                    dt.Columns.Add("SupplierName", typeof(string));
                    dt.Columns.Add("SupplierID", typeof(string));

                    // Fill the DataTable with data from the SQL query
                    adp.Fill(dt);

                    con.Close();

                    // Set up the report with the DataTable as data source
                    rptPaymentProductReport rpt = new rptPaymentProductReport();
                    rpt.SetDataSource(dt);

                    // Set any parameters in the report if needed
                    rpt.SetParameterValue("p1", dtpDateFrom.Value);
                    rpt.SetParameterValue("p2", dtpDateTo.Value);

                    // Show the report in a new form with CrystalReportViewer
                    frmReport reportForm = new frmReport();
                    reportForm.crystalReportViewer1.ReportSource = rpt;
                    reportForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
