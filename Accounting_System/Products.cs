using BarcodeStandard;
using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;
using Pharmacy.DL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing.Common;
using ZXing;
using Excel = Microsoft.Office.Interop.Excel;
using DevExpress.Utils.About;

namespace Accounting_System
{
    public partial class Products : Form
    {
        SqlConnection cn = new SqlConnection(DataAccessLayer.Con());
        private Pymentinvoice frmPurchaseEntry;

        public Products()
        {
            InitializeComponent();
            base.Load += Products_Load;
            this.KeyDown += new KeyEventHandler(Products_KeyDown);
            cmbCategory.SelectedIndexChanged += new EventHandler(cmbCategory_SelectedIndexChanged);
            txtCostPrice.KeyPress += new KeyPressEventHandler(txtPrice_KeyPress);
            txtDiscount.KeyPress += new KeyPressEventHandler(txtDiscount_KeyPress);
            txtVAT.KeyPress += new KeyPressEventHandler(txtVAT_KeyPress);
            txtSellingPrice.KeyPress += new KeyPressEventHandler(txtSellingPrice_KeyPress);
            dgw.MouseClick += new MouseEventHandler(dgw_MouseClick);
            txtBarcode.KeyPress += new KeyPressEventHandler(txtBarcode_KeyPress);
            TextBox1.TextChanged += new EventHandler(TextBox1_TextChanged1);
            DataGridView1.MouseDoubleClick += new MouseEventHandler(DataGridView1_MouseDoubleClick);
            TextBox1.KeyPress += new KeyPressEventHandler(TextBox1_KeyPress_1);
            CheckBox1.CheckedChanged += new EventHandler(CheckBox1_CheckedChanged);
            txtBarcode.KeyDown += new KeyEventHandler(txtBarcode_KeyDown);

        }



        private void TextBox1_TextChanged1(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(TextBox1.Text))
                {
                    DataGridView1.Visible = true;
                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();
                        string query = "SELECT PID, RTRIM(ProductCode), RTRIM(ProductName), SubCategoryID, RTRIM(CategoryName), RTRIM(SubCategoryName), RTRIM(Description), CostPrice, SellingPrice, Discount, VAT, ReorderPoint, RTRIM(Barcode), OpeningStock, ManufacturingDate, ExpiryDate, SellingPrice2 " +
                                       "FROM Category " +
                                       "JOIN SubCategory ON Category.CategoryName = SubCategory.Category " +
                                       "JOIN Product ON Product.SubCategoryID = SubCategory.ID " +
                                       "WHERE Barcode LIKE @barcode " +
                                       "ORDER BY ProductCode";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@barcode", "%" + TextBox1.Text + "%");
                            using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                            {
                                DataGridView1.Rows.Clear();
                                while (rdr.Read())
                                {
                                    DataGridView1.Rows.Add(
                                        rdr["PID"],
                                        rdr["ProductCode"],
                                        rdr["ProductName"],
                                        rdr["SubCategoryID"],
                                        rdr["CategoryName"],
                                        rdr["SubCategoryName"],
                                        rdr["Description"],
                                        rdr["CostPrice"],
                                        rdr["SellingPrice"],
                                        rdr["Discount"],
                                        rdr["VAT"],
                                        rdr["ReorderPoint"],
                                        rdr["Barcode"],
                                        rdr["OpeningStock"],
                                        rdr["ManufacturingDate"],
                                        rdr["ExpiryDate"],
                                        rdr["SellingPrice2"]
                                    );
                                }
                            }
                        }
                    }
                }
                else
                {
                    DataGridView1.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Products_Load(object sender, EventArgs e)
        {

            fillCategory();
            LoadCurrenciesToComboBox();
            Getdata11();
            cmbCategory.SelectedIndex = 0;
            cmbSubCategory.SelectedIndex = 0;
            txtBarcode.Focus();
            dtpExpiryDate.Enabled = false;
            dtpManufacturingDate.Enabled = false;

        }
        private void txtProductCode_TextChanged(object sender, EventArgs e)
        {

        }
        // Install-Package Microsoft.VisualBasic
        public void Getdata11()
        {

            
                try
                {
                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();
                        string query = "Select PID, RTRIM(ProductCode), RTRIM(Productname), SubCategoryID, RTRIM(CategoryName), RTRIM(SubCategoryName), RTRIM(Description), CostPrice, SellingPrice, Discount, VAT, ReorderPoint, RTRIM(Barcode), OpeningStock from Category, SubCategory, Product where Category.CategoryName = SubCategory.Category and Product.SubCategoryID = SubCategory.ID order by ProductCode";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            DataGridView1.Rows.Clear();
                            while (rdr.Read())
                            {
                                DataGridView1.Rows.Add(
                                    rdr[0].ToString(),
                                    rdr[1].ToString(),
                                    rdr[2].ToString(),
                                    rdr[3].ToString(),
                                    rdr[4].ToString(),
                                    rdr[5].ToString(),
                                    rdr[6].ToString(),
                                    rdr[7].ToString(),
                                    rdr[8].ToString(),
                                    rdr[9].ToString(),
                                    rdr[10].ToString(),
                                    rdr[11].ToString(),
                                    rdr[12].ToString(),
                                    rdr[13].ToString()
                                );
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            

        }

       


        private void auto()
        {
            try
            {
                txtID.Text = GenerateID();
                txtProductCode.Text = "P-" + GenerateeID();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Reset()
        {
            dtpExpiryDate.Enabled = false;
            dtpManufacturingDate.Enabled = false;
            dtpExpiryDate.Value = DateTime.Today;
            dtpManufacturingDate.Value = DateTime.Today;
            txtSellingPrice2.Text = "";
            txtBarcode.Text = "";
            TextBox1.Text = "";
            txtCostPrice.Text = "";
            txtProductCode.Text = "";
            txtDiscount.Text = "0";
            txtSellingPrice.Text = "";
            txtVAT.Text = "0";
            txtOpeningStock.Text = "";
            txtReorderPoint.Text = "";
            txtFeatures.Text = "";
            txtProductName.Text = "";
            cmbCategory.SelectedIndex = -1;
            Plimit.Text = "";
            cmbSubCategory.SelectedIndex = -1;
            txtOpeningStock.ReadOnly = false;
            txtOpeningStock.Enabled = true;
            cmbSubCategory.Enabled = false;
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            button1.Enabled = false;
            btnDelete.Enabled = false;
            Picture.Image = Properties.Resources._12;
            dgw.Rows.Clear();
            btnRemove.Enabled = false;
            btnExportExcel.Enabled = false;
            auto();
            // GenerateBarcode();
            comboBox1.SelectedIndex = 0;
            Plimit.Text = "";
        }

        private void txtVAT_KeyPress(object sender, KeyPressEventArgs e)
        {
            char keyChar = e.KeyChar;

            if (char.IsControl(keyChar))
            {
                // Allow all control characters.
            }
            else if (char.IsDigit(keyChar) || keyChar == '.')
            {
                string text = txtVAT.Text;
                int selectionStart = txtVAT.SelectionStart;
                int selectionLength = txtVAT.SelectionLength;

                // Construct the new text with the current key press.
                text = text.Substring(0, selectionStart) + keyChar + text.Substring(selectionStart + selectionLength);

                // Check if the new text is a valid integer and its length.
                if (int.TryParse(text, out _) && text.Length > 16)
                {
                    // Reject an integer that is longer than 16 digits.
                    e.Handled = true;
                }
                // Check if the new text is a valid double and decimal places.
                else if (double.TryParse(text, out _) && text.IndexOf('.') < text.Length - 3)
                {
                    // Reject a real number with more than two decimal places.
                    e.Handled = true;
                }
            }
            else
            {
                // Reject all other characters.
                e.Handled = true;
            }
        }
        private void txtSellingPrice_KeyPress(object sender,KeyPressEventArgs e)
        {

        }


        private void fillCategory()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlDataAdapter adp = new SqlDataAdapter())
                    {
                        adp.SelectCommand = new SqlCommand("SELECT DISTINCT RTRIM(CategoryName) FROM Category", con);
                        DataSet ds = new DataSet("ds");
                        adp.Fill(ds);
                        System.Data.DataTable dtable = ds.Tables[0];
                        cmbCategory.Items.Clear();
                        foreach (DataRow drow in dtable.Rows)
                        {
                            cmbCategory.Items.Add(drow[0].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private string GenerateeID()
        {
            string value = "000";
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    // Fetch the latest ID from the database
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 PID FROM Product ORDER BY PID DESC", con))
                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            value = rdr["PID"].ToString();
                        }
                    }

                    // Increase the ID by 1
                    int numericValue = int.Parse(value);
                    numericValue += 1;
                    value = numericValue.ToString("D5"); // Ensure the string is padded with leading zeros if necessary
                }
                catch (Exception ex)
                {
                    // If an error occurs, set the value to "0000"
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                    value = "000";
                }
            }
            return value;
        }


        private void txtOpeningStock_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            // If (e.KeyChar < Chr(48) Or e.KeyChar > Chr(57)) And e.KeyChar <> Chr(8) Then
            // e.Handled = True
            // End If
        }
        private void txtBarcode_KeyPress(object sender,KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }



        // Button1 Click event to show frmSalesLocations
        private void Button1_Click(object sender, EventArgs e)
        {
            // Me.Hide()
            // Dim frm As New frmSalesLocations
            // frm.lblSet.Text = "Product Entry"
            // frm.Reset()
        }

        // Button2 Click event to generate and set a new barcode


        // Button4 Click event to show frmCategory
        private void Button4_Click(object sender, EventArgs e)
        {

        }

        // Button3 Click event to show frmSubCategory
        private void Button3_Click(object sender, EventArgs e)
        {
            /*            frmSubCategory.lblUser.Text = lblUser.Text;
                        frmSubCategory.Reset();
                        frmSubCategory.ShowDialog();*/
        }


        // TextBox1 TextChanged event to filter DataGridView based on barcode
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(TextBox1.Text))
                {
                    DataGridView1.Visible = true;
                    cn.Open();
                    SqlCommand cmd = new SqlCommand(
                        "SELECT PID, RTRIM(ProductCode), RTRIM(Productname), SubCategoryID, RTRIM(CategoryName), RTRIM(SubCategoryName), " +
                        "RTRIM(Description), CostPrice, SellingPrice, Discount, VAT, ReorderPoint, RTRIM(Barcode), OpeningStock, " +
                        "ManufacturingDate, ExpiryDate, SellingPrice2 " +
                        "FROM Category, SubCategory, Product " +
                        "WHERE Category.CategoryName = SubCategory.Category AND Product.SubCategoryID = SubCategory.ID AND Barcode LIKE '%" +
                        TextBox1.Text + "%' ORDER BY ProductCode", cn);
                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    DataGridView1.Rows.Clear();
                    while (rdr.Read())
                    {
                        DataGridView1.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], rdr[16]);
                    }
                    cn.Close();
                }
                else
                {
                    DataGridView1.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // Button5 Click event to show frmProductRecord
        private void Button5_Click(object sender, EventArgs e)
        {
            /*DataGridView1.Visible = false;
            var frm = new frmProductRecord();
            frm.lblSet.Text = "Product Entry";
            frm.Reset();
            frm.ShowDialog();*/
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Button3_Click_1(object sender, EventArgs e)
        {
            SubCategory subc = new SubCategory();
            subc.Show();
        }

        private void Button4_Click_1(object sender, EventArgs e)
        {
            Category category = new Category();
            category.lblUser.Text = lblUser.Text;

            category.Show();

        }
        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cmbSubCategory.Enabled = true;
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string ct = "SELECT DISTINCT RTRIM(SubCategoryName) FROM SubCategory INNER JOIN Category ON SubCategory.Category = Category.CategoryName WHERE CategoryName = @d1";
                    using (SqlCommand cmd = new SqlCommand(ct, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", cmbCategory.Text);
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            cmbSubCategory.Items.Clear();
                            while (rdr.Read())
                            {
                                cmbSubCategory.Items.Add(rdr[0].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void txtDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            char keyChar = e.KeyChar;

            if (char.IsControl(keyChar))
            {
                // Allow all control characters (e.g., backspace).
            }
            else if (char.IsDigit(keyChar) || keyChar == '.')
            {
                string text = txtDiscount.Text;
                int selectionStart = txtDiscount.SelectionStart;
                int selectionLength = txtDiscount.SelectionLength;

                // Construct the new text with the current key press.
                text = text.Substring(0, selectionStart) + keyChar + text.Substring(selectionStart + selectionLength);

                // Check if the new text is a valid integer and its length.
                if (int.TryParse(text, out _) && text.Length > 16)
                {
                    // Reject an integer that is longer than 16 digits.
                    e.Handled = true;
                }
                // Check if the new text is a valid double and decimal places.
                else if (double.TryParse(text, out _) && text.IndexOf('.') < text.Length - 3)
                {
                    // Reject a real number with more than two decimal places.
                    e.Handled = true;
                }
            }
            else
            {
                // Reject all other characters.
                e.Handled = true;
            }
        }
        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
/*            char keyChar = e.KeyChar;

            if (char.IsControl(keyChar))
            {
                // Allow all control characters.
            }
            else if (char.IsDigit(keyChar) || keyChar == '.')
            {
                string text = txtCostPrice.Text;
                int selectionStart = txtCostPrice.SelectionStart;
                int selectionLength = txtCostPrice.SelectionLength;

                text = text.Substring(0, selectionStart) + keyChar + text.Substring(selectionStart + selectionLength);

                if (int.TryParse(text, out _) && text.Length > 16)
                {
                    // Reject an integer that is longer than 16 digits.
                    e.Handled = true;
                }
                else if (double.TryParse(text, out _) && text.IndexOf('.') < text.Length - 3)
                {
                    // Reject a real number with too many decimal places.
                    e.Handled = true;
                }
            }
            else
            {
                // Reject all other characters.
                e.Handled = true;
            }*/
        }
        private void btnSave_Click_1(object sender, EventArgs e)
        {
            if (txtProductName.Text.Trim().Length == 0)
            {
                txtProductName.Focus();
                return;
            }

            if (cmbCategory.Text.Trim().Length == 0)
            {
                MessageBox.Show("الرجاء تحديد الفئة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCategory.Focus();
                return;
            }

            if (cmbSubCategory.Text.Trim().Length == 0)
            {
                MessageBox.Show("الرجاء تحديد الفئة الفرعية", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbSubCategory.Focus();
                return;
            }

            if (txtCostPrice.Text.Trim().Length == 0 || !double.TryParse(txtCostPrice.Text, out double costPrice))
            {
                MessageBox.Show("الرجاء كتابة سعر الشراء الصحيح", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCostPrice.Focus();
                return;
            }

            if (txtDiscount.Text.Trim().Length == 0 || !double.TryParse(txtDiscount.Text, out double discount))
            {
                MessageBox.Show("الرجاء كتابة مبلغ الخصم الصحيح", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiscount.Focus();
                return;
            }

            if (txtSellingPrice.Text.Trim().Length == 0 || !double.TryParse(txtSellingPrice.Text, out double sellingPrice))
            {
                MessageBox.Show("الرجاء كتابة سعر البيع الصحيح", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSellingPrice.Focus();
                return;
            }

            if (txtVAT.Text.Trim().Length == 0 || !double.TryParse(txtVAT.Text, out double vat))
            {
                MessageBox.Show("الرجاء كتابة مبلغ الضريبة % الصحيح", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtVAT.Focus();
                return;
            }
            
            if (string.IsNullOrWhiteSpace(Plimit.Text))
            {
                MessageBox.Show("الرجاء كتابة حد السعر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Plimit.Focus();
                return;
            }
            if (txtBarcode.Text.Trim().Length == 0)
            {
                MessageBox.Show("الرجاء كتابة الباركود", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBarcode.Focus();
                return;
            }
            if (comboBox1.Text == "اختر عملة")
            {
                MessageBox.Show("الرجاء اختيار عملة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox1.Focus();
                return;
            }
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string ct = "select Barcode from Product where Barcode=@d1";
                    using (SqlCommand cmd = new SqlCommand(ct, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtBarcode.Text);
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                MessageBox.Show("هذا الباركود موجود مسبقا", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtBarcode.Text = "";
                                txtBarcode.Focus();
                                return;
                            }
                        }
                    }
                    con.Close();
                }
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {

                    con.Open();
                    string ct1 = "select Barcode from Temp_Stock where Barcode=@d1";

                    using (SqlCommand cmd = new SqlCommand(ct1, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtBarcode.Text);
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                MessageBox.Show("هذا الباركود موجود مسبقا", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtBarcode.Text = "";
                                txtBarcode.Focus();
                                return;
                            }
                        }
                    }

                    Fill();
                    auto();

                    con.Close();
                }
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string cb = @"insert into Product(PID,ProductCode, Productname, SubCategoryID, Description, CostPrice, 
                          SellingPrice, Discount, VAT, ReorderPoint,OpeningStock,Barcode,ExpiryDate,
                          ManufacturingDate,SellingPrice2,Plimit,CurrencyName) 
                          VALUES (" + txtID.Text + ",@d1,@d2,@d3,@d4,@d5,@d6,@d7,@d8,@d9,@d10,@d11,@d12,@d13,@d14,@d15,@d16)";
                    using (SqlCommand cmd = new SqlCommand(cb, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtProductCode.Text);
                        cmd.Parameters.AddWithValue("@d2", txtProductName.Text);
                        cmd.Parameters.AddWithValue("@d3", int.TryParse(txtSubCategoryID.Text, out int subCategoryID) ? subCategoryID : 0);
                        cmd.Parameters.AddWithValue("@d4", txtFeatures.Text);
                        cmd.Parameters.AddWithValue("@d5", costPrice);
                        cmd.Parameters.AddWithValue("@d6", sellingPrice);
                        cmd.Parameters.AddWithValue("@d7", discount);
                        cmd.Parameters.AddWithValue("@d8", vat);
                        cmd.Parameters.AddWithValue("@d9", int.TryParse(txtReorderPoint.Text, out int reorderPoint) ? reorderPoint : 0);
                        cmd.Parameters.AddWithValue("@d10", int.TryParse(txtOpeningStock.Text, out int openingStock) ? openingStock : 0);
                        cmd.Parameters.AddWithValue("@d11", txtBarcode.Text);
                        cmd.Parameters.AddWithValue("@d14", txtSellingPrice2.Text);
                        cmd.Parameters.AddWithValue("@d15", int.TryParse(Plimit.Text, out int Plimitt) ? Plimitt : 0);
                        cmd.Parameters.AddWithValue("@d16", comboBox1.Text);
                        if (CheckBox1.Checked)
                        {
                            cmd.Parameters.AddWithValue("@d12", dtpExpiryDate.Value);
                            cmd.Parameters.AddWithValue("@d13", dtpManufacturingDate.Value);

                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@d12", DBNull.Value);
                            cmd.Parameters.AddWithValue("@d13", DBNull.Value);
                        }

                        cmd.ExecuteNonQuery();
                    }

                    con.Close();
                }

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string ck = "insert into Product_Join(ProductID,photo) VALUES (" + txtID.Text + ",@d2)";
                    using (SqlCommand cmd = new SqlCommand(ck, con))
                    {
                        foreach (DataGridViewRow row in dgw.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    Bitmap img = (Bitmap)row.Cells[0].Value;
                                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    byte[] data = ms.ToArray();
                                    SqlParameter p = new SqlParameter("@d2", SqlDbType.Image) { Value = data };
                                    cmd.Parameters.Add(p);
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                }
                            }
                        }
                    }
                }

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string cb1 = "INSERT INTO[sa].[dbo].[Temp_Stock](ProductID, Qty, Barcode, BarcodeImage) " +
                           "VALUES (@ProductID, @Qty, @Barcode, @BarcodeImage)";
                    using (SqlCommand cmd = new SqlCommand(cb1, con))
                    {
                        cmd.Parameters.AddWithValue("@ProductID", txtID.Text.Trim());
                        cmd.Parameters.AddWithValue("@Qty", txtOpeningStock.Text.Trim());
                        cmd.Parameters.AddWithValue("@Barcode", txtBarcode.Text.Trim());
                        var ms = new MemoryStream();

                        var bmpImage = new Bitmap(picBarcode.Image);
                        bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        byte[] data = ms.ToArray();
                        cmd.Parameters.Add(new SqlParameter("@BarcodeImage", SqlDbType.Image) { Value = data });

                        cmd.ExecuteNonQuery();
                    }
                    con.Close();

                    MessageBox.Show("تم الحفظ بنجاح", "سجلات الأصناف", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                btnSave.Enabled = false;
                DataGridView1.Visible = false;
                auto();
                FileSystem.Reset();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (lblSet.Text == "TOPYMENT")
            {
                this.Hide();
            }

            Reset();

        }
        public static void LogFunc(string st1, string st2)
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                string cb = "INSERT INTO Logs(UserID, Date, Operation) VALUES (@d1, @d2, @d3)";
                using (var cmd = new SqlCommand(cb, con))
                {
                    cmd.Parameters.AddWithValue("@d1", st1);
                    cmd.Parameters.AddWithValue("@d2", DateTime.Now);
                    cmd.Parameters.AddWithValue("@d3", st2);
                    cmd.ExecuteReader();
                }
            }
        }



        private void btnNew_Click_1(object sender, EventArgs e)
        {
            FileSystem.Reset();
            DataGridView1.Visible = false;
            Reset();
        }

        private void CheckBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (CheckBox1.Checked)
            {
                dtpExpiryDate.Enabled = true;
                dtpManufacturingDate.Enabled = true;
                dtpExpiryDate.Value = DateTime.Today;
                dtpManufacturingDate.Value = DateTime.Today;
            }
            else
            {
                dtpExpiryDate.Enabled = false;
                dtpManufacturingDate.Enabled = false;
            }
        }

        private void Panel3_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void btnUpdate_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtProductCode.Text))
                {
                    MessageBox.Show("الرجاء كتابة كود الصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProductCode.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtProductName.Text))
                {
                    MessageBox.Show("الرجاء كتابة اسم الصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProductName.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(cmbCategory.Text))
                {
                    MessageBox.Show("الرجاء تحديد الفئة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbCategory.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(cmbSubCategory.Text))
                {
                    MessageBox.Show("الرجاء تحديد الفئة الفرعية", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbSubCategory.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtCostPrice.Text))
                {
                    MessageBox.Show("الرجاء كتابة سعر الشراء", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCostPrice.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtDiscount.Text))
                {
                    MessageBox.Show("الرجاء كتابة مبلغ الخصم", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDiscount.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtSellingPrice.Text))
                {
                    MessageBox.Show("الرجاء كتابة سعر البيع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSellingPrice.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtVAT.Text))
                {
                    MessageBox.Show("الرجاء كتابة الضريبة لهذا الصنف %", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtVAT.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtReorderPoint.Text))
                {
                    MessageBox.Show("الرجاء كتابة كمية حد الطلب لهذا الصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtReorderPoint.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtOpeningStock.Text))
                {
                    MessageBox.Show("الرجاء كتابة الرصيد الافتتاحي لهذا الصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtOpeningStock.Focus();
                    return;
                }

                // Check for barcode existence
                if (txtBarcode.Text != txtBCode.Text)
                {
                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();
                        string ct1 = "SELECT Barcode FROM Temp_Stock WHERE Barcode=@d1";
                        using (SqlCommand cmd = new SqlCommand(ct1, con))
                        {
                            cmd.Parameters.AddWithValue("@d1", txtBarcode.Text);
                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                if (rdr.Read())
                                {
                                    MessageBox.Show("هذا الباركود موجود مسبقا", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    txtBarcode.Text = "";
                                    txtBarcode.Focus();
                                    return;
                                }
                            }
                        }
                    }
                }

                // Update Product
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string cb = "UPDATE Product SET Productname=@d2, SubCategoryID=@d3, Description=@d4, CostPrice=@d5, SellingPrice=@d6, Discount=@d7, VAT=@d8, ReorderPoint=@d9, ProductCode=@d1, Barcode=@d10, OpeningStock=@d11, ExpiryDate=@d12, ManufacturingDate=@d13, SellingPrice2=@d14,Plimit=@d15 , CurrencyName=@d16 WHERE PID=@id";
                    using (SqlCommand cmd = new SqlCommand(cb, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtProductCode.Text);
                        cmd.Parameters.AddWithValue("@d2", txtProductName.Text);
                        cmd.Parameters.AddWithValue("@d3", Convert.ToInt32(txtSubCategoryID.Text));
                        cmd.Parameters.AddWithValue("@d4", txtFeatures.Text);
                        cmd.Parameters.AddWithValue("@d5", Convert.ToDecimal(txtCostPrice.Text));
                        cmd.Parameters.AddWithValue("@d6", Convert.ToDecimal(txtSellingPrice.Text));
                        cmd.Parameters.AddWithValue("@d7", Convert.ToDecimal(txtDiscount.Text));
                        cmd.Parameters.AddWithValue("@d8", Convert.ToDecimal(txtVAT.Text));
                        cmd.Parameters.AddWithValue("@d9", Convert.ToInt32(txtReorderPoint.Text));
                        cmd.Parameters.AddWithValue("@d10", txtBarcode.Text);
                        cmd.Parameters.AddWithValue("@d11", Convert.ToInt32(txtOpeningStock.Text));
                        cmd.Parameters.AddWithValue("@d14", txtSellingPrice2.Text);
                        cmd.Parameters.AddWithValue("@d15", int.TryParse(Plimit.Text, out int Plimitt) ? Plimitt : 0);
                        cmd.Parameters.AddWithValue("@d16", comboBox1.Text);

                        if (CheckBox1.Checked)
                        {
                            cmd.Parameters.AddWithValue("@d12", dtpExpiryDate.Value);
                            cmd.Parameters.AddWithValue("@d13", dtpManufacturingDate.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@d12", DBNull.Value);
                            cmd.Parameters.AddWithValue("@d13", DBNull.Value);
                        }

                        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(txtID.Text));
                        cmd.ExecuteNonQuery();
                    }
                }

                // Update Temp_Stock
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string sql = "UPDATE Temp_Stock SET Barcode=@d1, Qty=@d3 WHERE Barcode=@d2";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtBarcode.Text);
                        cmd.Parameters.AddWithValue("@d2", txtBCode.Text);
                        cmd.Parameters.AddWithValue("@d3", Convert.ToInt32(txtOpeningStock.Text));
                        cmd.ExecuteNonQuery();
                    }
                }

                // Delete old Product_Join records and insert new ones
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string cb1 = "DELETE FROM Product_Join WHERE ProductID=@d1";
                    using (SqlCommand cmd = new SqlCommand(cb1, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                        cmd.ExecuteNonQuery();
                    }

                    string ck = "INSERT INTO Product_Join(ProductID, Photo) VALUES (@d1, @d2)";
                    using (SqlCommand cmd = new SqlCommand(ck, con))
                    {
                        cmd.Prepare();
                        foreach (DataGridViewRow row in dgw.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    Image img = (Image)row.Cells[0].Value;
                                    using (Bitmap bmpImage = new Bitmap(img))
                                    {
                                        bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                        byte[] data = ms.ToArray();
                                        cmd.Parameters.Add(new SqlParameter("@d2", SqlDbType.Image) { Value = data });
                                        cmd.ExecuteNonQuery();
                                        cmd.Parameters.Clear();
                                    }
                                }
                            }
                        }
                    }
                }

                LogFunc(lblUser.Text, $"updated the Product '{txtProductName.Text}' having Product code '{txtProductCode.Text}'");
                MessageBox.Show("تم التعديل بنجاح", "Product Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnUpdate.Enabled = false;
                DataGridView1.Visible = false;

                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("هل أنت متأكد أنك تريد حذف سجل هذا الصنف?", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    DeleteRecord();
                    // Optionally refresh records here
                    DataGridView1.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Reset();
        }

        private void DeleteRecord()
        {
            try
            {
                // Check if the product is linked with stock
                if (CheckProductLink("SELECT PID FROM Product INNER JOIN Stock_Product ON Product.PID = Stock_Product.ProductID WHERE PID=@d1", "لا يمكن حذف هذا الصنف , لأنه يوجد له كميات بالمخزن")) return;

                // Check if the product is linked with sales invoices
                if (CheckProductLink("SELECT PID FROM Product INNER JOIN Invoice_Product ON Product.PID = Invoice_Product.ProductID WHERE PID=@d1", "لا يمكن حذف هذا الصنف , لأنه يوجد عمليات بيع على هذا الصنف")) return;

                // Check if the product is linked with quotations
                if (CheckProductLink("SELECT PID FROM Product INNER JOIN Quotation_Join ON Product.PID = Quotation_Join.ProductID WHERE PID=@d1", "لا يمكن حذف هذا الصنف لأنه يوجد عروض أسعار على هذا الصنف")) return;

                // Check if the product is linked with purchase invoices
                if (CheckProductLink("SELECT PID FROM Product INNER JOIN Invoice1_Product ON Product.PID = Invoice1_Product.ProductID WHERE PID=@d1", "لا يمكن حذف هذا الصنف لأنه يوجد عمليات شراء على هذا الصنف")) return;

                // Delete the product if no links found
                DeleteProduct();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool CheckProductLink(string query, string errorMessage)
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            MessageBox.Show(errorMessage, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return true; // Link found, stop further execution
                        }
                    }
                }
            }
            return false; // No link found, continue execution
        }

        private void DeleteProduct()
        {
            int rowsAffected = 0;

            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                string query = "DELETE FROM Product WHERE PID=@d1";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            if (rowsAffected > 0)
            {
                LogFunc(lblUser.Text, $"deleted the Product '{txtProductName.Text}' having Product code '{txtProductCode.Text}'");
                MessageBox.Show("تم الحذف بنجاح", "سجلات الأصناف", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FileSystem.Reset();
            }
            else
            {
                MessageBox.Show("لا يوجد سجلات", "عذرًا", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FileSystem.Reset();
            }
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            ProductsScreen ps = new ProductsScreen();
            ps.lblSet.Text = "Product Entry";
            ps.Show();
        }
        private void Fill()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT ID FROM SubCategory WHERE Category=@d1 AND SubCategoryName=@d2";
                        cmd.Parameters.AddWithValue("@d1", cmbCategory.Text);
                        cmd.Parameters.AddWithValue("@d2", cmbSubCategory.Text);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                txtSubCategoryID.Text = rdr.GetInt32(0).ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void Button1_Click_1(object sender, EventArgs e)
        {

        }

        private void Button6_Click_1(object sender, EventArgs e)
        {
            dtpExpiryDate.Value = DateTime.Today;
            dtpManufacturingDate.Value = DateTime.Today;
        }
        private void TextBox1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != (char)8)
            {
                e.Handled = true; // Ignore the key press
            }
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            dtpExpiryDate.Value = DateTime.Today;
            dtpManufacturingDate.Value = DateTime.Today;
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox1.Checked == true)
            {
                dtpExpiryDate.Enabled = true;
                dtpManufacturingDate.Enabled = true;
                dtpExpiryDate.Value = DateTime.Today;
                dtpManufacturingDate.Value = DateTime.Today;
            }

            else
            {
                dtpExpiryDate.Enabled = false;
                dtpManufacturingDate.Enabled = false;

            }
        }

        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                txtCostPrice.Focus();

            }
        }
        private void Browse_Click(object sender, EventArgs e)
        {
            SetupSampleData();
        }
        private void SetupSampleData()
        {
            // Use an OpenFileDialog to select the image
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|PDF Files|*.pdf|Word Files|*.doc;";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    string fileExtension = Path.GetExtension(filePath).ToLower();

                    try
                    {
                        if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".bmp")
                        {
                            // Display the selected image in the PictureBox
                            if (Picture == null)
                            {
                                MessageBox.Show("PictureBox is not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            Picture.Image = new Bitmap(filePath);
                        }
                        else if (fileExtension == ".pdf" || fileExtension == ".doc" || fileExtension == ".docx")
                        {
                            // For PDF or Word files, just store or process the file path as needed
                            // Here, just showing a message with the file path
                            MessageBox.Show($"File selected: {filePath}", "File Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // You could store the file path in a variable or database for later use
                        }
                        else
                        {
                            MessageBox.Show("Unsupported file type selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (FileNotFoundException ex)
                    {
                        MessageBox.Show("File not found: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (OutOfMemoryException ex)
                    {
                        MessageBox.Show("Image file is too large or not a valid image format: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            dgw.Rows.Add(Picture.Image);
        }
        public void ExportExcel(DataGridView dataGridView)
        {
            int rowsTotal, colsTotal;
            int I, j, iC;
            Cursor.Current = Cursors.WaitCursor;
            Excel.Application xlApp = new Excel.Application();

            try
            {
                Excel.Workbook excelBook = xlApp.Workbooks.Add();
                Excel.Worksheet excelWorksheet = (Excel.Worksheet)excelBook.Worksheets[1];
                xlApp.Visible = true;

                rowsTotal = dataGridView.RowCount;
                colsTotal = dataGridView.Columns.Count - 3;

                excelWorksheet.Cells.Select();
                excelWorksheet.Cells.Delete();

                for (iC = 0; iC <= colsTotal; iC++)
                {
                    excelWorksheet.Cells[1, iC + 1].Value = dataGridView.Columns[iC].HeaderText;
                }

                for (I = 0; I < rowsTotal; I++)
                {
                    for (j = 0; j <= colsTotal; j++)
                    {
                        excelWorksheet.Cells[I + 2, j + 1].Value = dataGridView.Rows[I].Cells[j].Value;
                    }
                }


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


        private void DataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (DataGridView1.SelectedRows.Count > 0)
                {
                    DataGridViewRow dr = DataGridView1.SelectedRows[0];
                    txtID.Text = dr.Cells[0].Value.ToString();
                    txtProductCode.Text = dr.Cells[1].Value.ToString();
                    txtProductName.Text = dr.Cells[2].Value.ToString();
                    txtSubCategoryID.Text = dr.Cells[3].Value.ToString();
                    cmbCategory.Text = dr.Cells[4].Value.ToString();
                    cmbSubCategory.Text = dr.Cells[5].Value.ToString();
                    txtFeatures.Text = dr.Cells[6].Value.ToString();
                    txtCostPrice.Text = dr.Cells[7].Value.ToString();
                    txtSellingPrice.Text = dr.Cells[8].Value.ToString();
                    txtDiscount.Text = dr.Cells[9].Value.ToString();
                    txtVAT.Text = dr.Cells[10].Value.ToString();
                    txtReorderPoint.Text = dr.Cells[11].Value.ToString();
                    txtBarcode.Text = dr.Cells[12].Value.ToString();
                    txtBCode.Text = dr.Cells[12].Value.ToString();
                    txtOpeningStock.Text = dr.Cells[13].Value.ToString();
                    txtSellingPrice2.Text = dr.Cells[16].Value.ToString();

                    if (dr.Cells[14].Value == DBNull.Value)
                    {
                        dtpExpiryDate.Value = DateTime.Today;
                        dtpManufacturingDate.Value = DateTime.Today;
                        CheckBox1.Checked = false;
                    }
                    else
                    {
                        dtpExpiryDate.Value = Convert.ToDateTime(dr.Cells[15].Value);
                        dtpManufacturingDate.Value = Convert.ToDateTime(dr.Cells[14].Value);
                        CheckBox1.Checked = true;
                    }

                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();
                        string query = "SELECT Photo FROM Product JOIN Product_Join ON Product.PID = Product_Join.ProductID WHERE Product.PID = @d1";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@d1", dr.Cells[0].Value.ToString());
                            using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                            {
                                DataGridView1.Rows.Clear();
                                while (rdr.Read())
                                {
                                    byte[] data = (byte[])rdr[0];
                                    using (MemoryStream ms = new MemoryStream(data))
                                    {
                                        Image img4 = Image.FromStream(ms);
                                        // Assuming you want to add the image to some control or DataGridView column
                                        // Example: Adding to a DataGridViewImageColumn (not shown in original code)
                                        // DataGridView1.Rows.Add(img4);
                                    }
                                }
                            }
                        }
                    }

                    btnUpdate.Enabled = true;
                    btnDelete.Enabled = true;
                    btnSave.Enabled = false;
                    txtOpeningStock.Enabled = true;
                    txtOpeningStock.ReadOnly = false;
                    DataGridView1.Visible = false;

                    // lblSet.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            ExportExcel(DataGridView1);
        }

        private void Button5_Click_1(object sender, EventArgs e)
        {
            ProductsScreen ps = new ProductsScreen();
            ps.lblSet.Text = "Product Entry";
/*            ps.FormClosed += (s, args) => this.Show(); // Show Product form when ProductsScreen is closed
*/            ps.Show();
            this.Hide();

        }
        private void OpenProductsScreen()
        {
            ProductsScreen frmProductsScreen = new ProductsScreen();
            frmProductsScreen.FormClosed += (s, args) => this.Show(); // Show Product form when ProductsScreen is closed
            this.Hide(); // Hide the Product form
            frmProductsScreen.Show(); // Show the ProductsScreen form
        }



        private void dtpManufacturingDate_ValueChanged(object sender, EventArgs e)
        {

        }
        private string GenerateID()
        {
            string value = "0000";
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    // Fetch the latest ID from the database
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 PID FROM Product ORDER BY PID DESC", con))
                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            value = rdr["PID"].ToString();
                        }
                    }
                    // we need to replace them. If necessary.
                    if (Convert.ToDouble(value) <= 9) // Value is between 0 and 10
                    {
                        value = "000" + value;
                    }
                    else if (Convert.ToDouble(value) <= 99) // Value is between 9 and 100
                    {
                        value = "00" + value;
                    }
                    else if (Convert.ToDouble(value) <= 999) // Value is between 999 and 1000
                    {
                        value = "0" + value;
                    }
                    // Increase the ID by 1
                    int numericValue = int.Parse(value);
                    numericValue += 1;
                    value = numericValue.ToString("D4"); // Ensure the string is padded with leading zeros if necessary
                }
                catch (Exception ex)
                {
                    // If an error occurs, set the value to "0000"
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                    value = "0000";
                }
            }
            return value;
        }
        private void Button2_Click_1(object sender, EventArgs e)
        {
            // Generate barcode text
            txtBarcode.Clear();
            string generatedID = GenerateID();
            string barcodeWithoutChecksum = (10000000000 + int.Parse(generatedID)).ToString(); // 8 digits
/*
            // Ensure we have 11 digits first (add leading zeros if necessary)
            while (barcodeWithoutChecksum.Length < 11)
            {
                barcodeWithoutChecksum =   barcodeWithoutChecksum+ "0";
            }*/

            // Calculate the checksum digit
            int checksum = CalculateUPCAChecksum(barcodeWithoutChecksum);
            string fullBarcode = barcodeWithoutChecksum + checksum.ToString(); // 12 digits

            txtBarcode.Text = barcodeWithoutChecksum;

            // Ensure the barcode text is valid for the specified format
            if (fullBarcode.Length >= 12)
            {
                BarcodeWriter barcodeWriter = new BarcodeWriter
                {
                    Format = BarcodeFormat.UPC_A // Specify the correct barcode format
                };
                picBarcode.Image = barcodeWriter.Write(barcodeWithoutChecksum);
            }
            else
            {
                MessageBox.Show("Invalid barcode length. UPC-A barcodes must be 12 digits long.");
            }
        }

        // Method to calculate the checksum digit for UPC-A
        private int CalculateUPCAChecksum(string barcodeWithoutChecksum)
        {
            int sumOdd = 0;
            int sumEven = 0;

            for (int i = 0; i < barcodeWithoutChecksum.Length; i++)
            {
                int digit = int.Parse(barcodeWithoutChecksum[i].ToString());
                if (i % 2 == 0) // Odd positions (0-based index)
                {
                    sumOdd += digit * 3;
                }
                else // Even positions
                {
                    sumEven += digit;
                }
            }

            int totalSum = sumOdd + sumEven;
            int checksum = (10 - (totalSum % 10)) % 10; // Calculate checksum digit

            return checksum;
        }


        private void Label18_Click(object sender, EventArgs e)
        {

        }

        private void txtOpeningStock_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Picture_Click(object sender, EventArgs e)
        {

        }

        private void BRemove_Click(object sender, EventArgs e)
        {
            Picture.Image= Properties.Resources.noThing;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgw.SelectedRows)
                    dgw.Rows.Remove(row);
                btnRemove.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void dgw_MouseClick(object sender,System.Windows.Forms.MouseEventArgs e)
        {
            if (dgw.Rows.Count > 0)
            {
                btnRemove.Enabled = true;
            }
        }

        private void Label10_Click(object sender, EventArgs e)
        {

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cmbCategory_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void picBarcode_Click(object sender, EventArgs e)
        {

        }

        private void txtBarcode_TextChanged(object sender, EventArgs e)
        {
            // Ensure the barcode text is valid for the specified format
            try
            {
                if (txtBarcode.Text.Length >= 12)
                {
                    BarcodeWriter barcodeWriter = new BarcodeWriter
                    {
                        Format = BarcodeFormat.EAN_13 // Specify the correct barcode format
                    };
                    picBarcode.Image = barcodeWriter.Write(txtBarcode.Text);
                }
            }
            catch(Exception ex)
            {
                 MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
          
        }

        private void btnSave_KeyDown(object sender, KeyEventArgs e)
        {
           
        }

        private void btnSave_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void Products_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Prevent the "ding" sound on Enter key press
                e.SuppressKeyPress = true;

                // Call the save button click event
                btnSave_Click_1(sender, e);
            }
        }

        private void txtOpeningStock_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtProductCode.Text))
                {
                    MessageBox.Show("الرجاء كتابة كود الصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProductCode.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtProductName.Text))
                {
                    MessageBox.Show("الرجاء كتابة اسم الصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProductName.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(cmbCategory.Text))
                {
                    MessageBox.Show("الرجاء تحديد الفئة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbCategory.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(cmbSubCategory.Text))
                {
                    MessageBox.Show("الرجاء تحديد الفئة الفرعية", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbSubCategory.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtCostPrice.Text))
                {
                    MessageBox.Show("الرجاء كتابة سعر الشراء", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCostPrice.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtDiscount.Text))
                {
                    MessageBox.Show("الرجاء كتابة مبلغ الخصم", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDiscount.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtSellingPrice.Text))
                {
                    MessageBox.Show("الرجاء كتابة سعر البيع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSellingPrice.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtVAT.Text))
                {
                    MessageBox.Show("الرجاء كتابة الضريبة لهذا الصنف %", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtVAT.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtReorderPoint.Text))
                {
                    MessageBox.Show("الرجاء كتابة كمية حد الطلب لهذا الصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtReorderPoint.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtOpeningStock.Text))
                {
                    MessageBox.Show("الرجاء كتابة الرصيد الافتتاحي لهذا الصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtOpeningStock.Focus();
                    return;
                }

                // Check for barcode existence
                if (txtBarcode.Text != txtBCode.Text)
                {
                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();
                        string ct1 = "SELECT Barcode FROM Temp_Stock WHERE Barcode=@d1";
                        using (SqlCommand cmd = new SqlCommand(ct1, con))
                        {
                            cmd.Parameters.AddWithValue("@d1", txtBarcode.Text);
                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                if (rdr.Read())
                                {
                                    MessageBox.Show("هذا الباركود موجود مسبقا", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    txtBarcode.Text = "";
                                    txtBarcode.Focus();
                                    return;
                                }
                            }
                        }
                    }
                }

                // Update Product
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string cb = "UPDATE Product SET Productname=@d2, SubCategoryID=@d3, Description=@d4, CostPrice=@d5, SellingPrice=@d6, Discount=@d7, VAT=@d8, ReorderPoint=@d9, ProductCode=@d1, Barcode=@d10, OpeningStock=@d11, ExpiryDate=@d12, ManufacturingDate=@d13, SellingPrice2=@d14 WHERE PID=@id";
                    using (SqlCommand cmd = new SqlCommand(cb, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtProductCode.Text);
                        cmd.Parameters.AddWithValue("@d2", txtProductName.Text);
                        cmd.Parameters.AddWithValue("@d3", Convert.ToInt32(txtSubCategoryID.Text));
                        cmd.Parameters.AddWithValue("@d4", txtFeatures.Text);
                        cmd.Parameters.AddWithValue("@d5", Convert.ToDecimal(txtCostPrice.Text));
                        cmd.Parameters.AddWithValue("@d6", Convert.ToDecimal(txtSellingPrice.Text));
                        cmd.Parameters.AddWithValue("@d7", Convert.ToDecimal(txtDiscount.Text));
                        cmd.Parameters.AddWithValue("@d8", Convert.ToDecimal(txtVAT.Text));
                        cmd.Parameters.AddWithValue("@d9", Convert.ToInt32(txtReorderPoint.Text));
                        cmd.Parameters.AddWithValue("@d10", txtBarcode.Text);
                        cmd.Parameters.AddWithValue("@d11", Convert.ToInt32(txtOpeningStock.Text));
                        cmd.Parameters.AddWithValue("@d14", txtSellingPrice2.Text);

                        if (CheckBox1.Checked)
                        {
                            cmd.Parameters.AddWithValue("@d12", dtpExpiryDate.Value);
                            cmd.Parameters.AddWithValue("@d13", dtpManufacturingDate.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@d12", DBNull.Value);
                            cmd.Parameters.AddWithValue("@d13", DBNull.Value);
                        }

                        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(txtID.Text));
                        cmd.ExecuteNonQuery();
                    }
                }

                // Update Temp_Stock
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string sql = "UPDATE Temp_Stock SET Barcode=@d1 WHERE Barcode=@d2";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtBarcode.Text);
                        cmd.Parameters.AddWithValue("@d2", txtBCode.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Delete old Product_Join records and insert new ones
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string cb1 = "DELETE FROM Product_Join WHERE ProductID=@d1";
                    using (SqlCommand cmd = new SqlCommand(cb1, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                        cmd.ExecuteNonQuery();
                    }

                    string ck = "INSERT INTO Product_Join(ProductID, Photo) VALUES (@d1, @d2)";
                    using (SqlCommand cmd = new SqlCommand(ck, con))
                    {
                        cmd.Prepare();
                        foreach (DataGridViewRow row in dgw.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    Image img = (Image)row.Cells[0].Value;
                                    using (Bitmap bmpImage = new Bitmap(img))
                                    {
                                        bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                        byte[] data = ms.ToArray();
                                        cmd.Parameters.Add(new SqlParameter("@d2", SqlDbType.Image) { Value = data });
                                        cmd.ExecuteNonQuery();
                                        cmd.Parameters.Clear();
                                    }
                                }
                            }
                        }
                    }
                }

                LogFunc(lblUser.Text, $"updated the Product '{txtProductName.Text}' having Product code '{txtProductCode.Text}'");
                MessageBox.Show("تم التعديل بنجاح", "Product Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnUpdate.Enabled = false;
                DataGridView1.Visible = false;

                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbSubCategory_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
               

                // Step 2: Get the selected currency name from the ComboBox
                string selectedCurrency = comboBox1.SelectedItem.ToString().Trim();

                // Step 3: Define the SQL query to fetch both id and price based on the selected currency name
                string query = "SELECT id, Price FROM Currencies WHERE Name = @Name";

                // Step 4: Create a connection to the database
                using (SqlConnection conn = new SqlConnection(DataAccessLayer.Con()))
                {
                    try
                    {
                        conn.Open();

                        // Step 5: Create a SqlCommand to execute the query
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            // Step 6: Add the selected currency name as a parameter to prevent SQL injection
                            cmd.Parameters.AddWithValue("@Name", selectedCurrency);

                            // Step 7: Execute the query and use SqlDataReader to fetch both id and price
                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                if (rdr.Read()) // Check if at least one record is returned
                                {
                                    // Retrieve the 'id' and 'Price' from the reader
                                    int currencyId = rdr["id"] != DBNull.Value ? Convert.ToInt32(rdr["id"]) : 0;
                                    int price = rdr["Price"] != DBNull.Value ? Convert.ToInt32(rdr["Price"]) : 0;

                                    // Step 8: Assign the retrieved values to the respective TextBoxes
                                    CUID.Text = currencyId.ToString();
                                    Cprice.Text = price.ToString();
                                }
                               
                            }
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        // Step 9: Handle SQL-related exceptions specifically
                        MessageBox.Show($"A database error occurred: {sqlEx.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LogError(sqlEx); // Ensure this method logs the error details appropriately
                    }
                    catch (Exception ex)
                    {
                        // Step 10: Handle any other general exceptions
                        MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LogError(ex); // Ensure this method logs the error details appropriately
                    }
                }
            }
            catch (Exception ex) { 
             MessageBox.Show($"A database error occurred: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }
        private void LogError(Exception ex)
        {
            // Implement logging logic here (e.g., write to a file, event log, etc.)
            // Example:
            System.IO.File.AppendAllText("error_log.txt", $"{DateTime.Now}: {ex.Message}{Environment.NewLine}");
        }
        private void LoadCurrenciesToComboBox()
        {
            // Step 1: SQL query to fetch currency names from the Currencies table
            string query = "SELECT Name FROM Currencies";

            // Step 2: Create a connection to the database
            using (SqlConnection conn = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    conn.Open();

                    // Step 3: Create a SqlCommand to execute the query
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                // Step 4: Clear any existing items in the ComboBox
                                comboBox1.Items.Clear();

                                // Step 5: Loop through the result set and add each currency to the ComboBox
                                while (reader.Read())
                                {
                                    comboBox1.Items.Add(reader["Name"].ToString());
                                }
                            }
                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Step 6: Handle any errors
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }

            comboBox1.SelectedIndex = 0;
        }

        private void Cprice_Click(object sender, EventArgs e)
        {
                
        }

        private void Plimit_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
