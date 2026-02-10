
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
    public partial class CustomerInvProductReport : Form
    {
        public CustomerInvProductReport()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {


                // Get the selected customer name
                string selectedCustomer = comboBox1.Text.Trim();

                // Define the query to retrieve customer details based on the name
                string query = "SELECT ID FROM [dbo].[Customer] WHERE Name = @Name";

                using (SqlConnection conn = new SqlConnection(DataAccessLayer.Con()))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Add the parameter value
                        cmd.Parameters.AddWithValue("@Name", selectedCustomer);

                        // Execute the query
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read()) // Check if a record is returned
                            {
                                // Retrieve and set the CustomerID in the CID TextBox
                                CID.Text = rdr["ID"]?.ToString() ?? string.Empty;
                            }
                            else
                            {
                                // Show a message if no matching record is found
                                CID.Clear();
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Handle SQL exceptions
                MessageBox.Show($"حدث خطأ في قاعدة البيانات: {sqlEx.Message}", "خطأ قاعدة بيانات", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                MessageBox.Show($"حدث خطأ غير متوقع: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadCustomerNames()
        {
            // Define the query to select both CustomerID and Name for binding
            string query = "SELECT ID, Name FROM [dbo].[Customer]";

            using (SqlConnection connection = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable customerTable = new DataTable();

                    // Fill DataTable with customer data
                    adapter.Fill(customerTable);

                    // Bind the DataTable to comboBox1
                    comboBox1.DataSource = customerTable;
                    comboBox1.DisplayMember = "Name";  // Display the Name in the ComboBox
                    comboBox1.ValueMember = "ID";  // Use CustomerID as the value


                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CustomerInvProductReport_Load(object sender, EventArgs e)
        {
            LoadCustomerNames();
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // Corrected SQL query with "WHERE" instead of "Whare"

                    string query1 = @"SELECT DISTINCT
                                        Customer.ID,
                                        Customer.Name as CustomerName,
                                        Customer.ContactNo,
                                        InvoiceInfo.InvoiceNo,
                                        Invoice_Product.InvoiceID,
                                        Invoice_Product.ProductID,
                                        Product.ProductName as ProductName,
                                        Invoice_Product.Qty,
                                        Invoice_Product.SellingPrice,
                                        Invoice_Product.TotalAmount,
                                        InvoiceInfo.InvoiceDate
                                    FROM Invoice_Product 
                                    JOIN Invoice_Payment ON Invoice_Product.InvoiceID = Invoice_Payment.InvoiceID 
                                    JOIN Product ON Invoice_Product.ProductID = Product.PID 
                                    JOIN Temp_Stock ON Temp_Stock.ProductID = Product.PID 
                                    JOIN [dbo].[InvoiceInfo] ON [dbo].[InvoiceInfo].Inv_ID = Invoice_Product.InvoiceID
                                    JOIN [dbo].[Customer] ON [dbo].[InvoiceInfo].CustomerID = Customer.ID
                                    WHERE Customer.ID = @d1 
                                    AND InvoiceInfo.InvoiceDate between  @d2 AND @d3
                                    ORDER BY Invoice_Product.Qty DESC;
                                    ";

                    SqlCommand cmd = new SqlCommand(query1, con);

                    // Add parameters
                    cmd.Parameters.AddWithValue("@d1", CID.Text.Trim());
                    cmd.Parameters.AddWithValue("@d2", dtpDateFrom.Value.Date); // Use only the date part
                    cmd.Parameters.AddWithValue("@d3", dtpDateTo.Value.Date); // Include full day of dtpDateTo

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    // DataTable to hold query results
                    DataTable dt = new DataTable();
                    dt.Columns.Add("InvoiceNo", typeof(string));
                    dt.Columns.Add("CustomerID", typeof(int));
                    dt.Columns.Add("CustomerName", typeof(string));
                    dt.Columns.Add("ContactNo", typeof(string));
                    dt.Columns.Add("ProductID", typeof(int));
                    dt.Columns.Add("ProductName", typeof(string));
                    dt.Columns.Add("Qty", typeof(float));
                    dt.Columns.Add("TotalAmount", typeof(float));
                    dt.Columns.Add("SellingPrice", typeof(float));
                    dt.Columns.Add("InvoiceDate", typeof(DateTime));

                    // Fill the DataTable with data
                    adp.Fill(dt);

                    con.Close();

                    // Set up the report with the DataTable as data source
                    rptCustomerProductInvReport rpt = new rptCustomerProductInvReport();
                    rpt.SetDataSource(dt);

                    // Set parameters in the report if needed
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
    }
}
