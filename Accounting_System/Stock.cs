using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using Excel = Microsoft.Office.Interop.Excel;

namespace Accounting_System
{
    public partial class Stock : Form
    {
        public static Stock instance;
        public Stock()
        {
            InitializeComponent();
            txtProductName.TextChanged += new EventHandler(txtProductName_TextChanged);
            txtBarcode.TextChanged += new EventHandler(txtBarcode_TextChanged);
            dgw.MouseDoubleClick += new MouseEventHandler(dgw_MouseDoubleClick);
            instance=this;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            btnReset.Enabled = false;
            try
            {
                txtProductName.Text = "";
                txtBarcode.Text = "";
                Getdata();
            }
            finally
            {
                btnReset.Enabled = true;
            }
        }

        public void Getdata()
        {
            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con())) // Replace with your connection string
            {
                using (SqlCommand cmd = new SqlCommand("SELECT PID, RTRIM(Product.ProductCode), RTRIM(ProductName), RTRIM(Temp_Stock.Barcode), " +
                                                        "CostPrice, SellingPrice, Discount, VAT, Qty,QTYP , " +
                                                        "RTRIM(Product.SellingPrice2), BarcodeImage, Plimit, WarehouseName , Warehouses.WID " +
                                                        "FROM Temp_Stock " +
                                                        "INNER JOIN Product ON Product.PID = Temp_Stock.ProductID " +
                                                        "INNER JOIN Warehouses ON Temp_Stock.WID = Warehouses.WID " +
                                                        "WHERE Qty > 0 AND Warehouses.WID = @WID " +
                                                        "ORDER BY Product.ProductCode", cn))
                {
                    cmd.Parameters.AddWithValue("@WID", WIDD.Text.Trim()); // Assuming WIDD is a TextBox

                    try
                    {
                        cn.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear(); // Clear previous data

                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4],
                                             rdr[5], rdr[6], rdr[7], rdr[8], rdr[9],
                                             rdr[10], rdr[11], rdr[12], rdr[13], rdr[14]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Stock_Load(object sender, EventArgs e)
        {
            Getdata();
            comboBoxGenerate();
            comboBox2.SelectedIndex = 0;
            fillSupplier();
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
        private void GetdataE()
        {
            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con())) // Replace with your connection string
            {
                using (SqlCommand cmd = new SqlCommand("SELECT  PID, RTRIM(Product.ProductCode), RTRIM(ProductName), RTRIM(Temp_Stock.Barcode), " +
                                                        "CostPrice, SellingPrice, Discount, VAT, Qty,QTYP, " +
                                                        "RTRIM(Product.SellingPrice2), BarcodeImage, Plimit, WarehouseName  , Warehouses.WID  " +
                                                        "FROM Temp_Stock " +
                                                        "INNER JOIN Product ON Product.PID = Temp_Stock.ProductID " +
                                                        "INNER JOIN Warehouses ON Temp_Stock.WID = Warehouses.WID " +
                                                        "WHERE Qty > 0 AND Warehouses.WID = @WID " +
                                                        "ORDER BY Product.ProductCode", cn))
                {
                    cmd.Parameters.AddWithValue("@WID", WIDD.Text.Trim()); // Assuming WIDD is a TextBox

                    try
                    {
                        cn.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear(); // Clear previous data

                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4],
                                             rdr[5], rdr[6], rdr[7], rdr[8], rdr[9],
                                             rdr[10], rdr[11], rdr[12], rdr[13], rdr[14]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
                GetBalance();

            }
        }
        public void ExportExcel(object obj)
        {
            GetdataE();
            int rowsTotal, colsTotal;
            int I, j, iC;
            Cursor.Current = Cursors.WaitCursor;
            var xlApp = new Excel.Application();
            Excel.Workbook excelBook = null;
            Excel.Worksheet excelWorksheet = null;

            try
            {
                var dgv = obj as DataGridView;
                if (dgv == null || dgv.RowCount == 0)
                {
                    MessageBox.Show("No data to export!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                excelBook = xlApp.Workbooks.Add();
                excelWorksheet = (Excel.Worksheet)excelBook.Worksheets[1];
                xlApp.Visible = true;

                rowsTotal = dgv.RowCount;
                colsTotal = dgv.Columns.Count;

                // Clear previous data
                excelWorksheet.Cells.Clear();

                // Write column headers
                for (iC = 0; iC < colsTotal; iC++)
                {
                    excelWorksheet.Cells[1, iC + 1] = dgv.Columns[iC].HeaderText;
                }

                // Write cell data
                for (I = 0; I < rowsTotal; I++)
                {
                    for (j = 0; j < colsTotal; j++)
                    {
                        var cellValue = dgv.Rows[I].Cells[j].Value;
                        excelWorksheet.Cells[I + 2, j + 1] = (cellValue != null) ? cellValue.ToString() : "";
                    }
                }

                // Formatting
                excelWorksheet.Rows["1:1"].Font.Bold = true;
                excelWorksheet.Rows["1:1"].Font.Size = 12;
                excelWorksheet.Columns.AutoFit();

                // Select first cell
                excelWorksheet.Cells[1, 1].Select();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            finally
            {
                // Release COM objects
                if (excelWorksheet != null) Marshal.ReleaseComObject(excelWorksheet);
                if (excelBook != null) Marshal.ReleaseComObject(excelBook);
                if (xlApp != null) Marshal.ReleaseComObject(xlApp);

                Cursor.Current = Cursors.Default;
            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            btnExportExcel.Enabled = false;
            try
            {
                ExportExcel(dgw);
            }
            finally
            {
                btnExportExcel.Enabled = true;
            }
        }
        private void txtProductName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT PID, RTRIM(Product.ProductCode), RTRIM(ProductName), RTRIM(Temp_Stock.Barcode), " +
                                    " CostPrice, SellingPrice, Discount, VAT, Qty, QTYP, " +
                                    "RTRIM(Product.SellingPrice2), BarcodeImage, Plimit, WarehouseName  , Warehouses.WID " +
                                    "FROM Temp_Stock " +
                                    "INNER JOIN Product ON Product.PID = Temp_Stock.ProductID " +
                                    "INNER JOIN Warehouses ON Temp_Stock.WID = Warehouses.WID " +
                                   "WHERE Product.PID = Temp_Stock.ProductID AND Qty > 0 AND ProductName LIKE @ProductName AND Warehouses.WID = @WID  " +
                                   "ORDER BY ProductCode";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ProductName", "%" + txtProductName.Text + "%");
                        cmd.Parameters.AddWithValue("@WID", WIDD.Text.Trim());
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        private void txtBarcode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT PID, RTRIM(Product.ProductCode), RTRIM(ProductName), RTRIM(Temp_Stock.Barcode), " +
                                    "CostPrice, SellingPrice, Discount, VAT, Qty,QTYP, " +
                                    "RTRIM(Product.SellingPrice2), BarcodeImage, Plimit, WarehouseName , Warehouses.WID " +
                                    "FROM Temp_Stock  " +
                                    "INNER JOIN Product ON Product.PID = Temp_Stock.ProductID " +
                                    "INNER JOIN Warehouses ON Warehouses.WID = Temp_Stock.WID " +
                                    "WHERE Warehouses.WID = @WID AND Qty > 0 AND Temp_Stock.Barcode LIKE @Barcode " +
                                    "ORDER BY ProductCode";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Barcode", "%" + txtBarcode.Text + "%");
                        cmd.Parameters.AddWithValue("@WID", WIDD.Text.Trim());

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (dgw.Rows.Count > 0 && dgw.SelectedRows.Count > 0)
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];
                    if (lblSet.Text == "Billing")
                    {
                        POS pOS = POS.Instancee();
                        pOS.txtProductID.Text = dr.Cells[0].Value.ToString();
                        pOS.txtProductCode.Text = dr.Cells[1].Value.ToString();
                        pOS.txtProductName.Text = dr.Cells[2].Value.ToString();
                        pOS.txtBarcode.Text = dr.Cells[3].Value.ToString();
                        pOS.txtCostPrice.Text = dr.Cells[4].Value.ToString();
                        if (pOS.ComboBox1.SelectedIndex == 0)
                        {
                            pOS.txtSellingPrice.Text = dr.Cells[5].Value.ToString();
                        }
                        else
                        {
                            pOS.txtSellingPrice.Text = dr.Cells[9].Value.ToString();
                        }

                        pOS.txtAmount.Text = dr.Cells[5].Value.ToString();
                        double num;
                        num = Convert.ToDouble(dr.Cells[5].Value) - Convert.ToDouble(dr.Cells[4].Value);
                        num = Math.Round(num, 2);
                        pOS.txtMargin.Text = num.ToString();
                        pOS.Plimit.Text = dr.Cells[12].Value.ToString();

                        pOS.txtVAT.Text = dr.Cells[7].Value.ToString();
                        pOS.txtDiscountPer.Text = dr.Cells[6].Value.ToString();
                        pOS.dgw.Visible = false;
                        pOS.txtQty.Focus();
                        pOS.WID.Text = WIDD.Text;
                        pOS.comboBox2.Text = dr.Cells[13].Value.ToString();
                        pOS.visablility.Text=dr.Cells[8].Value.ToString();
                        pOS.txtQty.Focus();
                        this.Close();

                    }
                    else if (lblSet.Text == "WTransport")
                    {

                        Warehouses_transportation.instance.PID.Text = dr.Cells[0].Value.ToString();
                        Warehouses_transportation.instance.FtxtWIDTxt.Text = WIDD.Text.ToString();
                        Warehouses_transportation.instance.txtProtuct.Text = dr.Cells[2].Value.ToString();
                        Warehouses_transportation.instance.Qtyin.Text = dr.Cells[8].Value.ToString();
                        Warehouses_transportation.instance.txtFWN.Text = dr.Cells[13].Value.ToString();
                        Warehouses_transportation.instance.Barcode.Text = dr.Cells[3].Value.ToString();

                        // Check if the value is not null and is a byte array
                        if (dr.Cells[10].Value is byte[] imageData)
                        {
                            using (MemoryStream ms = new MemoryStream(imageData))
                            {
                                // Convert byte array to image
                                Warehouses_transportation.instance.pictureBox1.Image = Image.FromStream(ms);
                            }

                        }
                        else
                        {
                            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
                            {
                                cn.Open();
                                using (SqlCommand cmd = new SqlCommand("SELECT BarcodeImage FROM Temp_Stock WHERE ProductID = @ProductID", cn))
                                {
                                    cmd.Parameters.AddWithValue("@ProductID", dr.Cells[0].Value.ToString());

                                    var result = cmd.ExecuteScalar(); // Execute the query and get the result

                                    if (result == null || result == DBNull.Value)
                                    {
                                        MessageBox.Show("No image found for the specified Product ID.", "Image Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    else
                                    {
                                        // Convert result to byte array and load it into PictureBox
                                        byte[] imageDataa = (byte[])result;
                                        using (MemoryStream ms = new MemoryStream(imageDataa))
                                        {
                                            Warehouses_transportation.instance.pictureBox1.Image = Image.FromStream(ms);
                                        }
                                    }
                                }
                            }


                        }


                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("No row selected or no rows available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {


                // الخطوة 3: تعريف استعلام SQL لجلب WID و WarehouseName بناءً على اسم المخزن المحدد

                string query = "SELECT WID, WarehouseName FROM Warehouses WHERE WarehouseName = @Name";

                // الخطوة 4: إنشاء اتصال بقاعدة البيانات
                using (SqlConnection conn = new SqlConnection(DataAccessLayer.Con()))
                {
                    try
                    {
                        conn.Open();
                        string selectedWarehouse = comboBox2.SelectedItem.ToString();

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

                                    // الخطوة 8: تعيين القيم المسترجعة إلى الـ TextBoxes الخاصة بك
                                    WIDD.Text = wid.ToString();
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
            GetBalance();

        }

        private void cmbSupplierName_SelectedIndexChanged(object sender, EventArgs e)
        {
                try
            {
                string a = string.Empty;
                string b = string.Empty;
                string c = string.Empty;
                txtSupplierID.Text = string.Empty;

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT RTRIM(ID), RTRIM(Address), RTRIM(City), RTRIM(ContactNo) FROM Supplier WHERE Name = @d1";
                        cmd.Parameters.AddWithValue("@d1", cmbSupplierName.Text);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                txtSupplierID.Text = rdr.GetValue(0).ToString();

                            }
                            else if (cmbSupplierName.Text == "")
                            {
                                txtSupplierID.Text = string.Empty;
                                Getdata();


                            }

                        }
                    }
                }
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    string query = "SELECT PID, RTRIM(Product.ProductCode), RTRIM(ProductName), RTRIM(Temp_Stock.Barcode), " +
                                   "CostPrice, SellingPrice, Discount, VAT, Qty,QTYP, " +
                                   "RTRIM(Product.SellingPrice2), BarcodeImage, Plimit, WarehouseName , Warehouses.WID " +
                                   "FROM Temp_Stock " +
                                   "INNER JOIN Product ON Product.PID = Temp_Stock.ProductID " +
                                   "INNER JOIN Warehouses ON Temp_Stock.WID = Warehouses.WID " +
                                   "INNER JOIN Supplier ON Supplier.ID = Temp_Stock.SupplierID " +
                                   "WHERE Product.PID = Temp_Stock.ProductID AND Qty > 0 " +
                                   "AND Supplier.Name LIKE @ProductName AND Warehouses.WID = @WID " +
                                   "AND Supplier.ID = @SupplierID " +
                                   "ORDER BY ProductCode";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ProductName", "%" + cmbSupplierName.Text.Trim() + "%");
                        cmd.Parameters.AddWithValue("@WID", WIDD.Text.Trim());
                        cmd.Parameters.AddWithValue("@SupplierID", txtSupplierID.Text.Trim());

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14]);
                            }
                        }
                    }
                }
                GetBalance();

            }
            catch
            {
              return;
            }

         
        }
        private void fillSupplier()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    SqlDataAdapter adp = new SqlDataAdapter();
                    adp.SelectCommand = new SqlCommand("SELECT RTRIM(Name) FROM Supplier", con);
                    DataSet ds = new DataSet("ds");
                    adp.Fill(ds);
                    System.Data.DataTable dtable = ds.Tables[0];
                    cmbSupplierName.Items.Clear();
                    foreach (DataRow drow in dtable.Rows)
                    {
                        cmbSupplierName.Items.Add(drow[0].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        private void cmbSupplierName_TextChanged(object sender, EventArgs e)
        {
            if (cmbSupplierName.Text == "")
            {
                txtSupplierID.Text = string.Empty;
                Getdata();


            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
         
        }
        public void GetBalance()
        {
            try
            {
                var total1 = default(double);
                var total2 = default(double);
                var total3 = default(double);
                // Dim Row1 As DataGridViewRow
                foreach (DataGridViewRow Row in dgw.Rows)
                {
                    DataGridViewTextBoxCell celv = Row.Cells[5] as DataGridViewTextBoxCell;
                    DataGridViewTextBoxCell celv1 = Row.Cells[8] as DataGridViewTextBoxCell;
                    DataGridViewTextBoxCell celv2 = Row.Cells[9] as DataGridViewTextBoxCell;


                    if (Information.IsNumeric(celv.Value) == true)
                    {
                        if (!string.IsNullOrEmpty((celv.Value).ToString()))
                        {
                            total1 += Convert.ToDouble(celv.Value)*Convert.ToDouble(celv1.Value);
                        }
                        if (!string.IsNullOrEmpty(celv1.Value.ToString()))
                        {
                            total2 += Convert.ToDouble(celv1.Value);

                        }
                        if (!string.IsNullOrEmpty(celv2.Value.ToString()))
                        {
                            total3 += Convert.ToDouble(celv2.Value);

                        }


                    }

                }
                TextBox1.Text = total1.ToString();
                TextBox2.Text = total2.ToString();
                TextBox3.Text = total3.ToString();
            }
            catch
            {
                return;
            }
           

        }

        private void txtSupplierID_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            if (checkBox1.IsOn)
            {
                TextBox2.Visible = true;
                Label4.Visible = true;
                Label7.Visible = true;
                Label8.Visible = true;
                TextBox1.Visible = true;
                TextBox3.Visible = true;
                GetBalance();
            }
            else
            {
                TextBox2.Visible = false;
                Label4.Visible = false;
                Label7.Visible = false;
                Label8.Visible = false;
                TextBox1.Visible = false;
                TextBox3.Visible = false;
            }
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure a row is selected in the DataGridView
                if (dgw.SelectedRows.Count == 0 || dgw.SelectedRows[0].Index < 0)
                {
                    MessageBox.Show("Please select a product from the list.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Ensure the DataGridView has rows
                if (dgw.Rows.Count == 0)
                {
                    MessageBox.Show("No products available in the list.", "Empty List", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Retrieve the selected row
                DataGridViewRow dr = dgw.SelectedRows[0];

                // Initialize the Products form
                Products products = new Products();
                products.lblSet.Text = "Stock";

                // Populate fields using column names for better maintainability
                products.Reset();
                products.TextBox1.Text = dr.Cells[2]?.Value?.ToString() ?? "N/A";
                products.txtOpeningStock.Text = dr.Cells[8]?.Value?.ToString() ?? "0";

                // Show the Products form
                products.Show();
            }
            catch (Exception ex)
            {
                // Handle unexpected errors gracefully
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(ex.StackTrace); // Log the stack trace for debugging
            }
        }
    }
}
