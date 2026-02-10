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

namespace Accounting_System
{
    public partial class StockInAndOutReport : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public StockInAndOutReport()
        {
            InitializeComponent();
        }

        private void StockInAndOutReport_Load(object sender, EventArgs e)
        {
            comboBoxGenerate(); 
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
                    SqlCommand cmd = new SqlCommand("SELECT Product.ProductCode, ProductName, CostPrice, Discount, VAT, Qty, Warehouses.WarehouseName FROM Temp_Stock INNER JOIN Product ON Product.PID = Temp_Stock.ProductID INNER JOIN Warehouses ON Temp_Stock.WID = Warehouses.WID WHERE Qty = 0  And Warehouses.WID =@WID  ORDER BY ProductName", con);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    cmd.Parameters.AddWithValue("@WID", WID.Text.Trim()); // Assuming WIDD is a TextBox

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
                    SqlCommand cmd = new SqlCommand(@"
                                        SELECT Product.ProductCode, Product.ProductName, Product.CostPrice, Product.Discount, Product.VAT, Temp_Stock.Qty, Warehouses.WarehouseName
                                        FROM Temp_Stock
                                        INNER JOIN Product ON Product.PID = Temp_Stock.ProductID
                                        INNER JOIN Warehouses ON Temp_Stock.WID = Warehouses.WID
                                        WHERE Temp_Stock.Qty > 0 And Warehouses.WID =@WID 
                                        ORDER BY Product.ProductName", con);    

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    cmd.Parameters.AddWithValue("@WID", WID.Text.Trim()); // Assuming WIDD is a TextBox

                    // Assuming 'Stock11' is a dataset that has a table with the structure that matches the query
                    Stock11 stock11Dataset = new Stock11();  // Assuming Stock11 is a typed dataset
                    adp.Fill(stock11Dataset.Tables["Stock1"]);  // Fill the appropriate table in Stock11


                    // Fetch the total price
                    decimal totalPrice = 0;
                    string query = "SELECT sum(CostPrice * Qty) FROM Temp_Stock, Product, Warehouses WHERE Product.PID = Temp_Stock.ProductID AND Warehouses.WID=Temp_Stock.WID AND Warehouses.WID =@WID AND Qty > 0";
                    SqlCommand cmd1 = new SqlCommand(query, con);
                    cmd1.Parameters.AddWithValue("@WID", WID.Text.Trim());

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
                    SqlCommand cmd = new SqlCommand(@"
                                                    SELECT Product.ProductCode, Product.ProductName, Product.CostPrice, Product.Discount, Product.VAT, Temp_Stock.Qty, Warehouses.WarehouseName
                                                    FROM Temp_Stock
                                                    INNER JOIN Product ON Product.PID = Temp_Stock.ProductID
                                                    INNER JOIN Warehouses ON Temp_Stock.WID = Warehouses.WID
                                                    WHERE Temp_Stock.Qty < 5 AND Temp_Stock.Qty > 0 And Warehouses.WID =@WID
                                                    ORDER BY Product.ProductName", con);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    cmd.Parameters.AddWithValue("@WID", WID.Text.Trim()); // Assuming WIDD is a TextBox

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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // الخطوة 2: الحصول على اسم المخزن المحدد من الـ ComboBox
                string selectedWarehouse = comboBox2.SelectedItem.ToString();

                // الخطوة 3: تعريف استعلام SQL لجلب WID و WarehouseName بناءً على اسم المخزن المحدد
                string query = "SELECT WID, WarehouseName FROM Warehouses WHERE WarehouseName = @Name";

                // الخطوة 4: إنشاء اتصال بقاعدة البيانات
                using (SqlConnection conn = new SqlConnection(DataAccessLayer.Con()))
                {
                    try
                    {
                        conn.Open();

                        // الخطوة 5: إنشاء SqlCommand لتنفيذ الاستعلام
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            // الخطوة 6: إضافة اسم المخزن المحدد كمعامل لمنع هجمات SQL Injection
                            cmd.Parameters.AddWithValue("@Name", selectedWarehouse);

                            // الخطوة 7: تنفيذ الاستعلام واستخدام SqlDataReader لجلب WID و WarehouseName
                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                if (rdr.Read()) // التحقق من وجود سجل واحد على الأقل
                                {
                                    // استرجاع 'WID' و 'WarehouseName' من القارئ
                                    int wid = rdr["WID"] != DBNull.Value ? Convert.ToInt32(rdr["WID"]) : 0;
                                    string warehouseName = rdr["WarehouseName"] != DBNull.Value ? rdr["WarehouseName"].ToString() : string.Empty;

                                    // الخطوة 8: تعيين القيم المسترجعة إلى الـ TextBoxes الخاصة بك
                                    WID.Text = wid.ToString();
                                    // إذا كان لديك TextBox لاسم المخزن، يمكنك تعيينه هنا
                                    // مثلاً: WarehouseNameTextBox.Text = warehouseName;
                                }
                                else
                                {
                                    MessageBox.Show("لم يتم العثور على المخزن المحدد.", "معلومات", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        // الخطوة 9: معالجة استثناءات SQL الخاصة
                        MessageBox.Show($"حدث خطأ في قاعدة البيانات: {sqlEx.Message}", "خطأ قاعدة بيانات", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        // الخطوة 10: معالجة أي استثناءات عامة أخرى
                        MessageBox.Show($"حدث خطأ غير متوقع: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ في قاعدة البيانات: {ex.Message}", "خطأ قاعدة بيانات", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        public void comboBoxGenerate()
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT WarehouseName FROM [dbo].[Warehouses]", con);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    comboBox2.Items.Clear();
                    while (rdr.Read()) // Loops through all available rows
                    {
                        comboBox2.Items.Add(rdr["WarehouseName"].ToString());
                    }

                }
            }
            comboBox2.SelectedIndex = 0;
        }

    }

}
