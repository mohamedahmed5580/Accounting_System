using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Pharmacy.DL;

namespace Accounting_System
{
    public partial class StockInAndOutReport : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public StockInAndOutReport()
        {
            InitializeComponent();
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnStockOut_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // Fetch data for the report and fill the Stock11 dataset
                    SqlCommand cmd = new SqlCommand("SELECT Product.ProductCode, ProductName, CostPrice, Discount, VAT, Qty FROM Temp_Stock, Product WHERE Product.PID = Temp_Stock.ProductID AND Qty <= 0 ORDER BY ProductName", con);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    // Assuming 'Stock11' is a dataset that has a table with the structure that matches the query
                    Stock11 stock11Dataset = new Stock11();  // Assuming Stock11 is a typed dataset
                    adp.Fill(stock11Dataset.Tables["Stock1"]);  // Fill the appropriate table in Stock11


                    rptStockOut rpt = new rptStockOut();
                    rpt.SetDataSource(stock11Dataset);
                    rpt.SetParameterValue("p1", DateTime.Today);

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

        private void btnStockIn_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // Fetch data for the report and fill the Stock11 dataset
                    SqlCommand cmd = new SqlCommand("SELECT Product.ProductCode, ProductName, CostPrice, Discount, VAT, Qty FROM Temp_Stock, Product WHERE Product.PID = Temp_Stock.ProductID AND Qty > 0 ORDER BY ProductName", con);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    // Assuming 'Stock11' is a dataset that has a table with the structure that matches the query
                    Stock11 stock11Dataset = new Stock11();  // Assuming Stock11 is a typed dataset
                    adp.Fill(stock11Dataset.Tables["Stock1"]);  // Fill the appropriate table in Stock11


                    // Fetch the total price
                    decimal totalPrice = 0;
                    string query = "SELECT sum(CostPrice * Qty) FROM Temp_Stock, Product WHERE Product.PID = Temp_Stock.ProductID AND Qty > 0";
                    SqlCommand cmd1 = new SqlCommand(query, con);

                    var result = cmd1.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        totalPrice = Convert.ToDecimal(result);
                    }

                    // Close the connection after all database operations are done
                    con.Close();

                    // Set up the report with Stock11 dataset
                    rptStockIn rpt = new rptStockIn();
                    rpt.SetDataSource(stock11Dataset);

                    // Set the parameters in the report
                    rpt.SetParameterValue("p1", DateTime.Today);
                    rpt.SetParameterValue("total_price", totalPrice);

                    // Show the report
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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // Fetch data for the report and fill the Stock11 dataset
                    SqlCommand cmd = new SqlCommand("SELECT Product.ProductCode, ProductName, CostPrice, Discount, VAT, Qty FROM Temp_Stock, Product WHERE Product.PID = Temp_Stock.ProductID AND Qty < 5 AND Qty > 0 ORDER BY ProductName", con);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    // Assuming 'Stock11' is a dataset that has a table with the structure that matches the query
                    Stock11 stock11Dataset = new Stock11();  // Assuming Stock11 is a typed dataset
                    adp.Fill(stock11Dataset.Tables["Stock1"]);  // Fill the appropriate table in Stock11



                    rptStockIn_1 rpt = new rptStockIn_1();
                    rpt.SetDataSource(stock11Dataset);
                    rpt.SetParameterValue("p1", DateTime.Today);

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
