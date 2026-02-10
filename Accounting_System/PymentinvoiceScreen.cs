using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace Accounting_System
{
    public partial class PymentinvoiceScreen : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        private static PymentinvoiceScreen _instance;
        public static PymentinvoiceScreen Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new PymentinvoiceScreen();
                }
                return _instance;
            }
        }
        public PymentinvoiceScreen()
        {
            InitializeComponent();
            Getdata();
            comboBox2.SelectedIndex = comboBox2.Items.Count > 0 ? 0 : -1;

        }
        public void Getdata()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = @"SELECT ST_ID, RTRIM(InvoiceNo) AS InvoiceNo, Date, RTRIM(PurchaseType) AS PurchaseType, Supplier.ID, RTRIM(Supplier.SupplierID) AS SupplierID, RTRIM(Supplier.Name) AS SupplierName, SubTotal, DiscountPer, Discount, RTRIM(Stock.Remarks)  , VATAmt, FreightCharges, OtherCharges, PreviousDue, Total, RoundOff, GrandTotal, TotalPayment, PaymentDue, VATPer ,CurrencieName, RTRIM(Warehouses.WarehouseName), RTRIM(Warehouses.WID)
                             FROM Supplier 
                             JOIN Stock ON Supplier.ID = Stock.SupplierID 
                             JOIN Warehouses ON Stock.WID = Warehouses.WID  
                             WHERE Warehouses.WID = @d3
                             ORDER BY [ST_ID] DESC;";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@d3", WID.Text);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], rdr[16], rdr[17], rdr[18], rdr[19], rdr[20], rdr[21], rdr[22], rdr[23]);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            foreach (DataGridViewRow row in dgw.Rows)
            {

                if (row.Cells["Column9"].Value.ToString() == "0")
                {
                    row.DefaultCellStyle.BackColor = Color.Green;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                }
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
        }
        private void btnGetData_Click(object sender, EventArgs e)
        {

            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(@"SELECT ST_ID, RTRIM(InvoiceNo) AS InvoiceNo, Date, RTRIM(PurchaseType) AS PurchaseType, Supplier.ID, RTRIM(Supplier.SupplierID) AS SupplierID, RTRIM(Supplier.Name) AS SupplierName, SubTotal, DiscountPer, Discount, VATPer, VATAmt, FreightCharges, OtherCharges, PreviousDue, Total, RoundOff, GrandTotal, TotalPayment, PaymentDue, RTRIM(Stock.Remarks),CurrencieName , RTRIM(Warehouses.WarehouseName), RTRIM(Warehouses.WID)
                                              FROM Supplier 
                                              JOIN Stock ON Supplier.ID = Stock.SupplierID 
                                              JOIN Warehouses ON Stock.WID = Warehouses.WID  
                                              WHERE [Date] BETWEEN @d1 AND @d2 AND Warehouses.WID = @d3
                                              ORDER BY [Date];", con);

                    cmd.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                    cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;
                    cmd.Parameters.AddWithValue("@d3", WID.Text);

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        dgw.Rows.Clear();
                        while (rdr.Read())
                        {
                            dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], rdr[16], rdr[17], rdr[18], rdr[19], rdr[20], rdr[21], rdr[22], rdr[23]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        

        private void txtSupplierName_TextChanged(object sender, EventArgs e)
        {

            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(@"SELECT ST_ID, RTRIM(InvoiceNo) AS InvoiceNo, Date, RTRIM(PurchaseType) AS PurchaseType, Supplier.ID, RTRIM(Supplier.SupplierID) AS SupplierID, RTRIM(Supplier.Name) AS SupplierName, SubTotal, DiscountPer, Discount, VATPer, VATAmt, FreightCharges, OtherCharges, PreviousDue, Total, RoundOff, GrandTotal, TotalPayment, PaymentDue, RTRIM(Stock.Remarks),CurrencieName , RTRIM(Warehouses.WarehouseName), RTRIM(Warehouses.WID)
                                              FROM Supplier 
                                              JOIN Stock ON Supplier.ID = Stock.SupplierID 
                                              JOIN Warehouses ON Stock.WID = Warehouses.WID  
                                              WHERE [Name] LIKE @SupplierName AND Warehouses.WID = @d3
                                              ORDER BY [Date];", con);
                    cmd.Parameters.AddWithValue("@SupplierName", "%" + txtSupplierName.Text + "%");
                    cmd.Parameters.AddWithValue("@d3", WID.Text);

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        dgw.Rows.Clear();
                        while (rdr.Read())
                        {
                            dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], rdr[16], rdr[17], rdr[18], rdr[19], rdr[20], rdr[21], rdr[22], rdr[23]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void Reset()
        {
            txtSupplierName.Text = "";
            dtpDateFrom.Text = DateTime.Today.ToString();
            dtpDateTo.Text = DateTime.Today.ToString();
            Getdata();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnReset_Click_1(object sender, EventArgs e)
        {
            Reset();
        }

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        // Assuming 'con' is a SqlConnection object defined at the class level

        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (dgw.SelectedRows.Count == 0)
                {
                    MessageBox.Show("No invoice selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataGridViewRow dr = dgw.SelectedRows[0];
                string connectionString = DataAccessLayer.Con();

                if (string.IsNullOrEmpty(connectionString))
                {
                    MessageBox.Show("The connection string is not initialized. Please check the configuration.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (lblSet.Text == "Purchase")
                {
                    LoadPurchaseData(dr, connectionString);
                }
                else if (lblSet.Text == "PR")
                {
                    LoadPurchaseReturnData(dr, connectionString);
                }
                else if (lblSet.Text == "Shipping")
                {
                    LoadShippingData(dr);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPurchaseData(DataGridViewRow dr, string connectionString)
        {

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string sql = "SELECT PID, RTRIM(Product.ProductCode), RTRIM(Productname), RTRIM(Stock_Product.Barcode), " +
                                 "Qty, Price, TotalAmount, Stock_Product.TotalCurrency " +
                                 "FROM Stock " +
                                 "JOIN Stock_Product ON Stock.ST_ID = Stock_Product.StockID " +
                                 "JOIN Product ON Product.PID = Stock_Product.ProductID " +
                                 "WHERE ST_ID = @st_id";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@st_id", dr.Cells[0].Value);
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            Pymentinvoice.instance.DataGridView1.Rows.Clear();
                            while (rdr.Read())
                            {
                                Pymentinvoice.instance.DataGridView1.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            Pymentinvoice.instance.dtpDate.Text = dr.Cells[2].Value.ToString();
            Pymentinvoice.instance.cmbPurchaseType.Text = dr.Cells[3].Value.ToString();
            Pymentinvoice.instance.txtSup_ID.Text = dr.Cells[4].Value.ToString();
            Pymentinvoice.instance.txtSupplierID.Text = dr.Cells[5].Value.ToString();
            Pymentinvoice.instance.txtSupplierName.Text = dr.Cells[6].Value.ToString();
            Pymentinvoice.instance.txtSubTotal.Text = dr.Cells[7].Value.ToString();
            Pymentinvoice.instance.txtDiscPer.Text = dr.Cells[8].Value.ToString();
            Pymentinvoice.instance.txtDisc.Text = dr.Cells[9].Value.ToString();
            Pymentinvoice.instance.txtVATPer.Text = dr.Cells[10].Value.ToString();
            Pymentinvoice.instance.txtVATAmt.Text = dr.Cells[11].Value.ToString();
            Pymentinvoice.instance.txtFreightCharges.Text = dr.Cells[12].Value.ToString();
            Pymentinvoice.instance.txtOtherCharges.Text = dr.Cells[13].Value.ToString();
            Pymentinvoice.instance.txtPreviousDue.Text = dr.Cells[14].Value.ToString();
            Pymentinvoice.instance.txtTotal.Text = dr.Cells[15].Value.ToString();
            Pymentinvoice.instance.txtRoundOff.Text = dr.Cells[16].Value.ToString();
            Pymentinvoice.instance.txtGrandTotal.Text = dr.Cells[17].Value.ToString();
            Pymentinvoice.instance.txtTotalPaid.Text = dr.Cells[18].Value.ToString();
            Pymentinvoice.instance.txtBalance.Text = dr.Cells[19].Value.ToString();
            Pymentinvoice.instance.txtRemarks.Text = dr.Cells[20].Value.ToString();
            Pymentinvoice.instance.comboBox1.Text = dr.Cells[21].Value.ToString();

            // Populate fields in Pymentinvoice instance
            Pymentinvoice.instance.txtST_ID.Text = dr.Cells[0].Value.ToString();
            Pymentinvoice.instance.txtInvoiceNo.Text = dr.Cells[1].Value.ToString();
            // Configure UI elements
            Pymentinvoice.instance.btnSave.Enabled = false;
            Pymentinvoice.instance.DataGridView1.Enabled = true;
            Pymentinvoice.instance.btnAdd.Enabled = false;
            Pymentinvoice.instance.GetSupplierBalance1();
            Pymentinvoice.instance.btnDelete.Enabled = true;
            Pymentinvoice.instance.btnSelection.Enabled = false;
            Pymentinvoice.instance.button5.Enabled = false;

            this.Close();
        }

        private void LoadPurchaseReturnData(DataGridViewRow dr, string connectionString)
        {
            PurchaseReturn frmPurchaseReturn = PurchaseReturn.instance;
            frmPurchaseReturn.txtPurchaseID.Text = dr.Cells[0].Value.ToString();
            frmPurchaseReturn.txtPurchaseInvoiceNo.Text = dr.Cells[1].Value.ToString();
            frmPurchaseReturn.dtpPurchaseDate.Text = dr.Cells[2].Value.ToString();
            frmPurchaseReturn.txtSup_ID.Text = dr.Cells[4].Value.ToString();
            frmPurchaseReturn.txtSupplierID.Text = dr.Cells[5].Value.ToString();
            frmPurchaseReturn.txtSupplierName.Text = dr.Cells[6].Value.ToString();
            frmPurchaseReturn.txtDiscPer.Text = dr.Cells[8].Value.ToString();
            frmPurchaseReturn.txtDisc.Text = dr.Cells[9].Value.ToString();
            frmPurchaseReturn.txtVatPer.Text = dr.Cells[20].Value.ToString();
            frmPurchaseReturn.txtVATAmt.Text = dr.Cells[11].Value.ToString();
            frmPurchaseReturn.P_method.Text = dr.Cells[3].Value.ToString();
            frmPurchaseReturn.totalpay.Text = dr.Cells[19].Value.ToString();
            frmPurchaseReturn.textBox1.Text = dr.Cells[18].Value.ToString();
            frmPurchaseReturn.textBox2.Text = dr.Cells[17].Value.ToString();
            frmPurchaseReturn.comboBox1.Text = dr.Cells[21].Value.ToString();


            frmPurchaseReturn.textBox2.Text = dr.Cells[17].Value.ToString();
            frmPurchaseReturn.textBox2.Text = dr.Cells[17].Value.ToString();
            frmPurchaseReturn.auto();
            frmPurchaseReturn.btnSelection.Enabled = false;

            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("The connection string is not initialized. Please check the configuration.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string sql = "SELECT ProductID, RTRIM(ProductCode) AS ProductCode, RTRIM(ProductName) AS ProductName, " +
                                 "RTRIM(Stock_Product.Barcode) AS Barcode, Stock_Product.Qty, Price, TotalAmount, " +
                                 "RTRIM(Warehouses.WarehouseName) AS WarehouseName, RTRIM(Warehouses.WID) AS WarehouseID,TotalCurrency  " +
                                 "FROM Stock " +
                                 "JOIN Stock_Product ON Stock.ST_ID = Stock_Product.StockID " +
                                 "JOIN Product ON Stock_Product.ProductID = Product.PID " +
                                 "JOIN Warehouses ON Stock.WID = Warehouses.WID " +
                                 "WHERE ST_ID = @st_id AND Warehouses.WID = @d3;";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@st_id", dr.Cells[0].Value);
                    cmd.Parameters.AddWithValue("@d3", WID.Text);

                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (rdr.Read())
                    {
                        frmPurchaseReturn.DataGridView2.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9]);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
                this.Close();
        }

        private void LoadShippingData(DataGridViewRow dr)
        {
            ShippingCom_pyment.instance.TextBox1.Text = dr.Cells[0].Value.ToString();
            ShippingCom_pyment.instance.textBox4.Text = dr.Cells[0].Value.ToString();
            ShippingCom_pyment.instance.textBox5.Text = dr.Cells[1].Value.ToString();
            this.Hide();
        }
        private void PymentinvoiceScreen_Load(object sender, EventArgs e)
        {
            comboBoxGenerate();
            comboBox2.SelectedIndex = comboBox2.Items.Count > 0 ? 0 : -1;
            Getdata();


        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            ExportExcel(dgw);
        }
        public static void ExportExcel(object obj)
        {
            short rowsTotal, colsTotal;
            short I, j, iC;
            Cursor.Current = Cursors.WaitCursor;
            var xlApp = new Excel.Application();
            try
            {
                var excelBook = xlApp.Workbooks.Add();
                var excelWorksheet = (Excel.Worksheet)excelBook.Worksheets[1];
                xlApp.Visible = true;

                rowsTotal = (short)((DataGridView)obj).RowCount;
                colsTotal = (short)(((DataGridView)obj).Columns.Count - 1);
                excelWorksheet.Cells.Select();
                excelWorksheet.Cells.Delete();
                for (iC = 0; iC <= colsTotal; iC++)
                {
                    excelWorksheet.Cells[1, iC + 1].Value = ((DataGridView)obj).Columns[iC].HeaderText;
                }
                for (I = 0; I < rowsTotal; I++)
                {
                    for (j = 0; j <= colsTotal; j++)
                    {
                        excelWorksheet.Cells[I + 2, j + 1].Value = ((DataGridView)obj).Rows[I].Cells[j].Value;
                    }
                }
                excelWorksheet.Rows["1:1"].Font.FontStyle = "Bold";
                excelWorksheet.Rows["1:1"].Font.Size = 12;

                excelWorksheet.Cells.Columns.AutoFit();
                excelWorksheet.Cells.Select();
                excelWorksheet.Cells.EntireColumn.AutoFit();
                excelWorksheet.Cells[1, 1].Select();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                xlApp = null;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
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
            Getdata();
        }

        private void PymentinvoiceScreen_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void PymentinvoiceScreen_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
