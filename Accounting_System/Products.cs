using BarcodeStandard;
using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;
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
        public static Products _instance  ;
        public static Products instance()
        {
            if (_instance == null)
            {
                _instance = new Products();
            }

            return _instance;
        }
        private ProductInitializer _productInitializer;


        public Products()
        {
            InitializeComponent();
            
            _productInitializer = new ProductInitializer(this);
            try
            {
                if (lblSet.Text != "Stock")
                {
                    // Delegate the reset logic to the initializer.
                    _productInitializer.ResetControls();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            _instance = this;
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
          
        }
      
        public void comboBoxGenerate()
        {
            try
            {
                // Create the repository instance (preferably injected via dependency injection).
                IWarehouseRepository repository = new WarehouseRepository();
                List<string> warehouseNames = repository.GetWarehouseNames();

                // Clear the combobox items and fill them from the repository data.
                comboBox2.Items.Clear();
                foreach (string name in warehouseNames)
                {
                    comboBox2.Items.Add(name);
                }
            }
            catch (Exception ex)
            {
                // Consider logging the exception details.
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtProductCode_TextChanged(object sender, EventArgs e)
        {
         
        }

        // Helper method to clear all fields
        private void ClearFields()
        {
            txtID.Clear();
            txtProductCode.Clear();
            txtProductName.Clear();
            txtSubCategoryID.Clear();
            cmbCategory.SelectedIndex = -1;
            cmbSubCategory.SelectedIndex = -1;
            txtFeatures.Clear();
            txtCostPrice.Clear();
            txtSellingPrice.Clear();
            txtDiscount.Clear();
            txtVAT.Clear();
            txtReorderPoint.Clear();
            txtBarcode.Clear();
            txtBCode.Clear();
            txtOpeningStock.Clear();
            Plimit.Clear();
            dtpExpiryDate.Value = DateTime.Now;
            dtpManufacturingDate.Value = DateTime.Now;
            dtpExpiryDate.Enabled = true;
            dtpManufacturingDate.Enabled = true;
            picBarcode.Image = null;

            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            button1.Enabled = false;
            btnSave.Enabled = true;
            txtOpeningStock.Enabled = false;
            txtOpeningStock.ReadOnly = true;
        }        // Install-Package Microsoft.VisualBasic
        public void Getdata()
        {
            try
            {
                // 1. Define the SQL Query (Using modern JOIN syntax is recommended)
                // Original Query using implicit joins:
                // string query = "Select PID, RTRIM(ProductCode), RTRIM(Productname), SubCategoryID, RTRIM(CategoryName), RTRIM(SubCategoryName), RTRIM(Description), CostPrice, SellingPrice, Discount, VAT, ReorderPoint, RTRIM(Barcode), OpeningStock from Category, SubCategory, Product where Category.CategoryName = SubCategory.Category and Product.SubCategoryID = SubCategory.ID order by ProductCode";

                // Rewritten Query using explicit INNER JOINs (More readable and standard)
                string query = @"
                SELECT
                    p.PID,
                    RTRIM(p.ProductCode),
                    RTRIM(p.ProductName), -- Corrected typo Productname -> ProductName
                    p.SubCategoryID,
                    RTRIM(c.CategoryName),
                    RTRIM(sc.SubCategoryName),
                    RTRIM(p.Description),
                    p.CostPrice,
                    p.SellingPrice,
                    p.Discount,
                    p.VAT,
                    p.ReorderPoint,
                    RTRIM(p.Barcode),
                    p.OpeningStock
                FROM
                    Product p
                INNER JOIN
                    SubCategory sc ON p.SubCategoryID = sc.ID
                INNER JOIN
                    Category c ON sc.Category = c.CategoryName -- Fixed JOIN condition based on original WHERE
                ORDER BY
                    p.ProductCode";

                // 2. Execute the query using the DataAccessLayer
                // No parameters needed for this specific query
                System.Data.DataTable dt = DataAccessLayer.ExecuteTable(query, CommandType.Text);

                // 3. Populate the DataGridView from the DataTable
                DataGridView1.SuspendLayout(); // Suspend layout for performance
                DataGridView1.Rows.Clear();   // Clear existing rows

                foreach (DataRow row in dt.Rows)
                {
                    // Add a new row to the DataGridView using data from the DataTable row
                    // Ensure the number of items added matches the grid columns
                    DataGridView1.Rows.Add(
                        row[0],  // PID
                        row[1],  // ProductCode
                        row[2],  // ProductName
                        row[3],  // SubCategoryID
                        row[4],  // CategoryName
                        row[5],  // SubCategoryName
                        row[6],  // Description
                        row[7],  // CostPrice
                        row[8],  // SellingPrice
                        row[9],  // Discount
                        row[10], // VAT
                        row[11], // ReorderPoint
                        row[12], // Barcode
                        row[13]  // OpeningStock
                    );
                }
            }
            catch (SqlException dbEx) // Catch specific database exceptions
            {
                MessageBox.Show("Database Error loading products: " + dbEx.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) // Catch general exceptions
            {
                MessageBox.Show("An error occurred loading products: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (DataGridView1 != null)
                { // Check if grid exists before resuming layout
                    DataGridView1.ResumeLayout(true); // Resume layout even if errors occurred
                }
            }
        }




        public void auto()
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
            Plimit.Text = "";
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
            fillCategory();
            cmbCategory.SelectedIndex = cmbCategory.Items.Count > 0 ? 0 : -1;
            cmbSubCategory.SelectedIndex = cmbSubCategory.Items.Count > 0 ? 0 : -1;
            comboBoxGenerate();
            Getdata();
            txtBarcode.Focus();
            dtpExpiryDate.Enabled = false;
            dtpManufacturingDate.Enabled = false;
            auto();
            comboBox2.SelectedIndex = comboBox2.Items.Count > 0 ? 0 : -1;

            comboBox2.SelectedIndex = 0;
            // GenerateBarcode();
        }

        public interface IProductValidator
        {
            bool Validate(ProductDto dto, out string errorMessage);
        }
        public interface IProductRepository
        {
            bool BarcodeExists(string barcode);
            void AddProduct(ProductDto product);
            void AddProductImages(int productId, List<Image> images);
            void AddTempStock(int productId, int qty, string barcode, Image barcodeImage, int warehouseId);
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
            if (comboBox2.Text == "اختر عملة")
            {
                MessageBox.Show("الرجاء اختيار مخزن", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox2.Focus();
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

            if (txtBarcode.Text.Trim().Length == 0)
            {
                MessageBox.Show("الرجاء كتابة الباركود", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBarcode.Focus();
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
                          ManufacturingDate,SellingPrice2,Plimit,WID) 
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
                        cmd.Parameters.AddWithValue("@d16", WID.Text);

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
                    string query = "INSERT INTO [dbo].[Temp_Stock] (ProductID, Qty, Barcode, BarcodeImage, WID) " +
                          "VALUES (@ProductID, @Qty, @Barcode, @BarcodeImage, @WID)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ProductID", txtID.Text.Trim());
                        cmd.Parameters.AddWithValue("@Qty", txtOpeningStock.Text.Trim());
                        cmd.Parameters.AddWithValue("@Barcode", txtBarcode.Text.Trim());
                        cmd.Parameters.AddWithValue("@WID", Convert.ToInt16(WID.Text.Trim()));
                        var ms = new MemoryStream();

                        var bmpImage = new Bitmap(picBarcode.Image);
                        bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        byte[] data = ms.ToArray();
                        cmd.Parameters.Add(new SqlParameter("@BarcodeImage", SqlDbType.Image) { Value = data });

                        cmd.ExecuteNonQuery();
                    }
                    con.Close();


                }
                MessageBox.Show("تم الحفظ بنجاح", "سجلات الأصناف", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
/*            char keyChar = e.KeyChar;

            if (char.IsControl(keyChar))
            {
                // Allow all control characters.
            }
            else if (char.IsDigit(keyChar) || keyChar == '.')
            {
                string text = txtSellingPrice.Text;
                int selectionStart = txtSellingPrice.SelectionStart;
                int selectionLength = txtSellingPrice.SelectionLength;

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
            }*/
        }


        public void fillCategory()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT RTRIM(CategoryName) FROM Category", con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cmbCategory.Items.Clear();
                        while (reader.Read())
                        {
                            // Assuming CategoryName is the first column and is of type string.
                            cmbCategory.Items.Add(reader.GetString(0));
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
            string generatedId = "000";
            try
            {
                // Open the connection using a using block to ensure proper disposal
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    // ExecuteScalar returns the first column of the first row in the result set
                    using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 PID FROM Product ORDER BY PID DESC", con))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            generatedId = result.ToString();
                        }
                    }
                }

                // Parse the current PID value and increment it, defaulting to zero if parsing fails
                int numericValue = 0;
                int.TryParse(generatedId, out numericValue);
                numericValue++;

                // Format the numeric value to a string with leading zeros (total length 5)
                generatedId = numericValue.ToString("D5");
            }
            catch (Exception ex)
            {
                // Log the error as needed here (for example, using a logging framework)
                // In case of error, you could also rethrow the exception or decide on another fallback.
                generatedId = "000";
            }
            return generatedId;
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
           fillSubCategory();   

        }
        public void fillSubCategory()
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
                if (lblSet.Text == "Stock")
                {
                    if (string.IsNullOrWhiteSpace(txtProductCode.Text))
                    {
                        MessageBox.Show("الرجاء كتابة كود الصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtProductCode.Focus();
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(comboBox2.Text))
                    {
                        MessageBox.Show("الرجاء كتابة اسم المخزن", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    if (!decimal.TryParse(txtCostPrice.Text, out decimal costPrice))
                    {
                        MessageBox.Show("الرجاء كتابة سعر الشراء صالح", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCostPrice.Focus();
                        return;
                    }
                    if (!decimal.TryParse(txtDiscount.Text, out decimal discount))
                    {
                        MessageBox.Show("الرجاء كتابة مبلغ الخصم صالح", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtDiscount.Focus();
                        return;
                    }
                    if (!decimal.TryParse(txtSellingPrice.Text, out decimal sellingPrice))
                    {
                        MessageBox.Show("الرجاء كتابة سعر البيع صالح", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtSellingPrice.Focus();
                        return;
                    }
                    if (!decimal.TryParse(txtVAT.Text, out decimal vat))
                    {
                        MessageBox.Show("الرجاء كتابة الضريبة لهذا الصنف % صالح", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtVAT.Focus();
                        return;
                    }
                    if (!int.TryParse(txtReorderPoint.Text, out int reorderPoint))
                    {
                        MessageBox.Show("الرجاء كتابة كمية حد الطلب لهذا الصنف صالحة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtReorderPoint.Focus();
                        return;
                    }
                    if (!int.TryParse(txtOpeningStock.Text, out int openingStock))
                    {
                        MessageBox.Show("الرجاء كتابة الرصيد الافتتاحي لهذا الصنف صالح", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtOpeningStock.Focus();
                        return;
                    }

                    double barcodee;
                    if (!double.TryParse(txtBarcode.Text, out barcodee))
                    {
                        MessageBox.Show("الرجاء كتابة باركود صالح", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtBarcode.Focus();
                        return;
                    }
                    double bcode = Convert.ToDouble(txtBCode.Text);

                    // Check for barcode existence
                    if (barcodee != bcode)
                    {
                        using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                        {
                            con.Open();
                            string ct1 = "SELECT Barcode FROM Temp_Stock WHERE Barcode=@d1 ";
                            using (SqlCommand cmd = new SqlCommand(ct1, con))
                            {
                                cmd.Parameters.AddWithValue("@d1", txtBarcode.Text);
                                using (SqlDataReader rdr = cmd.ExecuteReader())
                                {
                                    if (rdr.Read())
                                    {
                                        MessageBox.Show("هذا الباركود موجود مسبقا", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        string cb = "UPDATE Product SET Productname=@d2, SubCategoryID=@d3, Description=@d4, CostPrice=@d5, SellingPrice=@d6, Discount=@d7, VAT=@d8, ReorderPoint=@d9, ProductCode=@d1, Barcode=@d10, OpeningStock=@d11, ExpiryDate=@d12, ManufacturingDate=@d13, SellingPrice2=@d14, [Plimit]=@d15,WID=@d16 WHERE PID=@id";
                        using (SqlCommand cmd = new SqlCommand(cb, con))
                        {
                            cmd.Parameters.AddWithValue("@d1", txtProductCode.Text);
                            cmd.Parameters.AddWithValue("@d2", txtProductName.Text);
                            cmd.Parameters.AddWithValue("@d3", Convert.ToInt32(txtSubCategoryID.Text));
                            cmd.Parameters.AddWithValue("@d4", txtFeatures.Text);
                            cmd.Parameters.AddWithValue("@d5", costPrice);
                            cmd.Parameters.AddWithValue("@d6", sellingPrice);
                            cmd.Parameters.AddWithValue("@d7", discount);
                            cmd.Parameters.AddWithValue("@d8", vat);
                            cmd.Parameters.AddWithValue("@d9", reorderPoint);
                            cmd.Parameters.AddWithValue("@d10", txtBarcode.Text);
                            cmd.Parameters.AddWithValue("@d11", openingStock);
                            cmd.Parameters.AddWithValue("@d14", txtSellingPrice2.Text);
                            cmd.Parameters.AddWithValue("@d15", Convert.ToInt32(Plimit.Text));
                            cmd.Parameters.AddWithValue("@d16", WID.Text);

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
                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();

                        // Check if the product already exists in Temp_Stock
                        string checkQuery = "SELECT COUNT(*) FROM [dbo].[Temp_Stock] WHERE ProductID = @ProductID";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                        {
                            checkCmd.Parameters.AddWithValue("@ProductID", txtID.Text.Trim());
                            int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                            if (count > 0) // If the product exists, update it
                            {
                                string updateQuery = "UPDATE [dbo].[Temp_Stock] SET Qty = @Qty, Barcode = @Barcode, BarcodeImage = @BarcodeImage, WID = @WID WHERE ProductID = @ProductID";

                                using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                                {
                                    updateCmd.Parameters.AddWithValue("@ProductID", txtID.Text.Trim());
                                    updateCmd.Parameters.AddWithValue("@Barcode", txtBarcode.Text.Trim());
                                    updateCmd.Parameters.AddWithValue("@Qty", txtOpeningStock.Text.Trim());
                                    updateCmd.Parameters.AddWithValue("@WID", Convert.ToInt16(WID.Text.Trim()));

                                    var ms = new MemoryStream();
                                    var bmpImage = new Bitmap(picBarcode.Image);
                                    bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    byte[] data = ms.ToArray();
                                    updateCmd.Parameters.Add(new SqlParameter("@BarcodeImage", SqlDbType.Image) { Value = data });

                                    updateCmd.ExecuteNonQuery();
                                }

                            }
                            else // If the product does not exist, insert a new record
                            {
                                string insertQuery = "INSERT INTO [dbo].[Temp_Stock] (ProductID, Qty, Barcode, BarcodeImage, WID) " +
                                    "VALUES (@ProductID, @Qty, @Barcode, @BarcodeImage, @WID)";

                                using (SqlCommand insertCmd = new SqlCommand(insertQuery, con))
                                {
                                    insertCmd.Parameters.AddWithValue("@ProductID", txtID.Text.Trim());
                                    insertCmd.Parameters.AddWithValue("@Qty", txtOpeningStock.Text.Trim());
                                    insertCmd.Parameters.AddWithValue("@Barcode", txtBarcode.Text.Trim());
                                    insertCmd.Parameters.AddWithValue("@WID", Convert.ToInt16(WID.Text.Trim()));

                                    var ms = new MemoryStream();
                                    var bmpImage = new Bitmap(picBarcode.Image);
                                    bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    byte[] data = ms.ToArray();
                                    insertCmd.Parameters.Add(new SqlParameter("@BarcodeImage", SqlDbType.Image) { Value = data });
                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                        }
                        con.Close();
                        Stock stock = Stock.instance;
                        stock.Getdata();
                    }

                   
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(txtProductCode.Text))
                    {
                        MessageBox.Show("الرجاء كتابة كود الصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtProductCode.Focus();
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(comboBox2.Text))
                    {
                        MessageBox.Show("الرجاء كتابة اسم المخزن", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    if (!decimal.TryParse(txtCostPrice.Text, out decimal costPrice))
                    {
                        MessageBox.Show("الرجاء كتابة سعر الشراء صالح", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCostPrice.Focus();
                        return;
                    }
                    if (!decimal.TryParse(txtDiscount.Text, out decimal discount))
                    {
                        MessageBox.Show("الرجاء كتابة مبلغ الخصم صالح", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtDiscount.Focus();
                        return;
                    }
                    if (!decimal.TryParse(txtSellingPrice.Text, out decimal sellingPrice))
                    {
                        MessageBox.Show("الرجاء كتابة سعر البيع صالح", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtSellingPrice.Focus();
                        return;
                    }
                    if (!decimal.TryParse(txtVAT.Text, out decimal vat))
                    {
                        MessageBox.Show("الرجاء كتابة الضريبة لهذا الصنف % صالح", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtVAT.Focus();
                        return;
                    }
                    if (!int.TryParse(txtReorderPoint.Text, out int reorderPoint))
                    {
                        MessageBox.Show("الرجاء كتابة كمية حد الطلب لهذا الصنف صالحة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtReorderPoint.Focus();
                        return;
                    }
                    if (!int.TryParse(txtOpeningStock.Text, out int openingStock))
                    {
                        MessageBox.Show("الرجاء كتابة الرصيد الافتتاحي لهذا الصنف صالح", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtOpeningStock.Focus();
                        return;
                    }

                    double barcodee;
                    if (!double.TryParse(txtBarcode.Text, out barcodee))
                    {
                        MessageBox.Show("الرجاء كتابة باركود صالح", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtBarcode.Focus();
                        return;
                    }
                    double bcode = Convert.ToDouble(txtBCode.Text);

                    // Check for barcode existence
                    if (barcodee != bcode)
                    {
                        using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                        {
                            con.Open();
                            string ct1 = "SELECT Barcode FROM Temp_Stock WHERE Barcode=@d1 ";
                            using (SqlCommand cmd = new SqlCommand(ct1, con))
                            {
                                cmd.Parameters.AddWithValue("@d1", txtBarcode.Text);
                                using (SqlDataReader rdr = cmd.ExecuteReader())
                                {
                                    if (rdr.Read())
                                    {
                                        MessageBox.Show("هذا الباركود موجود مسبقا", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        string cb = "UPDATE Product SET Productname=@d2, SubCategoryID=@d3, Description=@d4, CostPrice=@d5, SellingPrice=@d6, Discount=@d7, VAT=@d8, ReorderPoint=@d9, ProductCode=@d1, Barcode=@d10, OpeningStock=@d11, ExpiryDate=@d12, ManufacturingDate=@d13, SellingPrice2=@d14, [Plimit]=@d15,WID=@d16 WHERE PID=@id";
                        using (SqlCommand cmd = new SqlCommand(cb, con))
                        {
                            cmd.Parameters.AddWithValue("@d1", txtProductCode.Text);
                            cmd.Parameters.AddWithValue("@d2", txtProductName.Text);
                            cmd.Parameters.AddWithValue("@d3", Convert.ToInt32(txtSubCategoryID.Text));
                            cmd.Parameters.AddWithValue("@d4", txtFeatures.Text);
                            cmd.Parameters.AddWithValue("@d5", costPrice);
                            cmd.Parameters.AddWithValue("@d6", sellingPrice);
                            cmd.Parameters.AddWithValue("@d7", discount);
                            cmd.Parameters.AddWithValue("@d8", vat);
                            cmd.Parameters.AddWithValue("@d9", reorderPoint);
                            cmd.Parameters.AddWithValue("@d10", txtBarcode.Text);
                            cmd.Parameters.AddWithValue("@d11", openingStock);
                            cmd.Parameters.AddWithValue("@d14", txtSellingPrice2.Text);
                            cmd.Parameters.AddWithValue("@d15", Convert.ToInt32(Plimit.Text));
                            cmd.Parameters.AddWithValue("@d16", WID.Text);

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

                    // Remaining update and insertion logic...
                    // Remaining update and insertion logic...
                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();

                        // Check if the product already exists in Temp_Stock
                        string checkQuery = "SELECT COUNT(*) FROM [dbo].[Temp_Stock] WHERE ProductID = @ProductID";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                        {
                            checkCmd.Parameters.AddWithValue("@ProductID", txtID.Text.Trim());
                            int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                            if (count > 0) // If the product exists, update it
                            {
                                string updateQuery = "UPDATE [dbo].[Temp_Stock] SET Barcode = @Barcode, BarcodeImage = @BarcodeImage, WID = @WID WHERE ProductID = @ProductID";

                                using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                                {
                                    updateCmd.Parameters.AddWithValue("@ProductID", txtID.Text.Trim());
                                    updateCmd.Parameters.AddWithValue("@Barcode", txtBarcode.Text.Trim());
                                    updateCmd.Parameters.AddWithValue("@WID", Convert.ToInt16(WID.Text.Trim()));

                                    var ms = new MemoryStream();
                                    var bmpImage = new Bitmap(picBarcode.Image);
                                    bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    byte[] data = ms.ToArray();
                                    updateCmd.Parameters.Add(new SqlParameter("@BarcodeImage", SqlDbType.Image) { Value = data });

                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                            else // If the product does not exist, insert a new record
                            {
                                string insertQuery = "INSERT INTO [dbo].[Temp_Stock] (ProductID, Qty, Barcode, BarcodeImage, WID) " +
                                    "VALUES (@ProductID, @Qty, @Barcode, @BarcodeImage, @WID)";

                                using (SqlCommand insertCmd = new SqlCommand(insertQuery, con))
                                {
                                    insertCmd.Parameters.AddWithValue("@ProductID", txtID.Text.Trim());
                                    insertCmd.Parameters.AddWithValue("@Qty", txtOpeningStock.Text.Trim());
                                    insertCmd.Parameters.AddWithValue("@Barcode", txtBarcode.Text.Trim());
                                    insertCmd.Parameters.AddWithValue("@WID", Convert.ToInt16(WID.Text.Trim()));

                                    var ms = new MemoryStream();
                                    var bmpImage = new Bitmap(picBarcode.Image);
                                    bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    byte[] data = ms.ToArray();
                                    insertCmd.Parameters.Add(new SqlParameter("@BarcodeImage", SqlDbType.Image) { Value = data });

                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                        }
                        con.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            LogFunc(lblUser.Text, $"تم التعديل '{txtProductName.Text}' ذي الرقم'{txtProductCode.Text}'");
            MessageBox.Show("تم التعديل بنجاح", "Product Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnUpdate.Enabled = false;
            DataGridView1.Visible = false;

            Reset();
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

            // Calculate the checksum digit
            int checksum = CalculateUPCAChecksum(barcodeWithoutChecksum);
            string fullBarcode = barcodeWithoutChecksum + checksum.ToString(); // 12 digits

            txtBarcode.Text = barcodeWithoutChecksum; // Display the full barcode (including checksum)


            if (fullBarcode.Length == 12) // For UPC-A barcodes
            {
                BarcodeWriter barcodeWriter = new BarcodeWriter
                {
                    Format = BarcodeFormat.UPC_A // Specify the correct barcode format
                };
                picBarcode.Image = barcodeWriter.Write(txtBarcode.Text);
            }
            else if (fullBarcode.Length == 13) // For EAN-13 barcodes
            {
                BarcodeWriter barcodeWriter = new BarcodeWriter
                {
                    Format = BarcodeFormat.EAN_13 // Specify the correct barcode format
                };
                picBarcode.Image = barcodeWriter.Write(txtBarcode.Text);
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

        private void Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void picBarcode_Click(object sender, EventArgs e)
        {

        }

        private void txtBarcode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtBarcode.Text.Length == 12) // For UPC-A barcodes
                {
                    BarcodeWriter barcodeWriter = new BarcodeWriter
                    {
                        Format = BarcodeFormat.UPC_A // Specify the correct barcode format
                    };
                    picBarcode.Image = barcodeWriter.Write(txtBarcode.Text);
                }
                else if (txtBarcode.Text.Length == 13) // For EAN-13 barcodes
                {
                    BarcodeWriter barcodeWriter = new BarcodeWriter
                    {
                        Format = BarcodeFormat.EAN_13 // Specify the correct barcode format
                    };
                    picBarcode.Image = barcodeWriter.Write(txtBarcode.Text);
                }
                else
                {
                    //picBarcode.Image = null; // Clear the image if the length is invalid
                }
            }
            catch (Exception ex)
            {
            }

            if (txtBarcode.Text.Length > 13)
            {
                // Truncate the text to 13 characters if it exceeds the limit
                txtBarcode.Text = txtBarcode.Text.Substring(0, 13);
                // Set the caret to the end of the truncated text
                txtBarcode.SelectionStart = txtBarcode.Text.Length;
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

                

                // Update Product
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string cb = "UPDATE Product SET Productname=@d2, SubCategoryID=@d3, Description=@d4, CostPrice=@d5, SellingPrice=@d6, Discount=@d7, VAT=@d8, ReorderPoint=@d9, ProductCode=@d1, Barcode=@d10, OpeningStock=@d11, ExpiryDate=@d12, ManufacturingDate=@d13, SellingPrice2=@d14, Plimit=@d15 WHERE PID=@id";
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
                        cmd.Parameters.AddWithValue("@d15", Convert.ToDecimal(Plimit.Text));

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
                    string sql = "UPDATE Temp_Stock SET Barcode=@d1, WID=@WID, Qty=@d3 WHERE Barcode=@d2";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtBarcode.Text);
                        cmd.Parameters.AddWithValue("@d2", txtBCode.Text);
                        cmd.Parameters.AddWithValue("@d3", txtOpeningStock.Text);
                        cmd.Parameters.AddWithValue("@WID", WID.Text);
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
                        LogError(sqlEx); // تأكد من أن هذا الأسلوب يسجل تفاصيل الخطأ بشكل مناسب
                    }
                    catch (Exception ex)
                    {
                        // الخطوة 10: معالجة أي استثناءات عامة أخرى
                        MessageBox.Show($"حدث خطأ غير متوقع: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LogError(ex); // تأكد من أن هذا الأسلوب يسجل تفاصيل الخطأ بشكل مناسب
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ في قاعدة البيانات: {ex.Message}", "خطأ قاعدة بيانات", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LogError(Exception ex)
        {
            // Implement logging logic here (e.g., write to a file, event log, etc.)
            // Example:
            System.IO.File.AppendAllText("error_log.txt", $"{DateTime.Now}: {ex.Message}{Environment.NewLine}");
        }

        private void WID_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtBarcode_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (txtBarcode == null)
                return;

            switch (e.KeyCode)
            {
                case Keys.PageUp:
                    // Move to the previous TextBox
                    e.Handled = true;
                    break;

                case Keys.PageDown:
                    // Move to the next TextBox
                    txtProductName.Focus(); // Replace 'textBox1' with your first TextBox name

                    e.Handled = true;
                    break;

                case Keys.Home:
                    // Move to the first TextBox
                    e.Handled = true;
                    break;

                case Keys.End:
                    // Move to the last TextBox
                    e.Handled = true;
                    break;
            }
        }

        private void txtProductName_KeyDown(object sender, KeyEventArgs e)
        {
            if (txtBarcode == null)
                return;

            switch (e.KeyCode)
            {
                case Keys.Enter:
                    // Move to the previous TextBox
                    Plimit.Focus(); // Replace 'textBox1' with your first TextBox name
                    e.Handled = true;
                    break;

                case Keys.PageDown:
                    // Move to the next TextBox
                    Plimit.Focus(); // Replace 'textBox1' with your first TextBox name

                    e.Handled = true;
                    break;

                case Keys.Home:
                    // Move to the first TextBox
                    Plimit.Focus(); // Replace 'textBox1' with your first TextBox name
                    e.Handled = true;
                    break;

                case Keys.End:
                    // Move to the last TextBox
                    Plimit.Focus(); // Replace 'textBox1' with your first TextBox name
                    e.Handled = true;
                    break;

            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            Category category = new Category(); 
            category.Show();
        }

        private void Button4_Click_2(object sender, EventArgs e)
        {
            Category category = new Category();
            category.Show();
        }

        private void cmbSubCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            Fill();

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            SubCategory subCategory = new SubCategory();
            subCategory.Show();
        }

        private void TextBox1_TextChanged_1(object sender, EventArgs e)
        {
            if (lblSet.Text == "Stock")
            {
                if (!string.IsNullOrEmpty(TextBox1.Text))
                {
                    dataGridView2.Visible = true;
                    GetFilteredData("ProductName", TextBox1.Text);
                }
                else
                {
                    dataGridView2.Visible = false;
                }
                // Ensure dataGridView2 is visible if there is filtered data
                dataGridView2.Visible = dataGridView2.Rows.Count > 0;

                // Automatically select the first matching row (if any)
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (row.Cells[1].Value?.ToString() == txtProductCode.Text)
                    {
                        row.Selected = true;
                        dataGridView2.CurrentCell = row.Cells[1]; // Focus on the selected cell
                        break;
                    }
                }

                // If a row is selected, populate the fields
                if (dataGridView2.SelectedRows.Count > 0)
                {
                    DataGridViewRow dr = dataGridView2.SelectedRows[0];

                    txtID.Text = dr.Cells[0].Value?.ToString() ?? "";
                    txtProductCode.Text = dr.Cells[1].Value?.ToString() ?? "";
                    txtProductName.Text = dr.Cells[2].Value?.ToString() ?? "";
                    cmbCategory.Text = dr.Cells[4].Value?.ToString() ?? "";
                    txtSubCategoryID.Text = dr.Cells[3].Value?.ToString() ?? "";
                    cmbSubCategory.Text = dr.Cells[5].Value?.ToString() ?? "";
                    txtFeatures.Text = dr.Cells[6].Value?.ToString() ?? "";
                    txtCostPrice.Text = dr.Cells[7].Value?.ToString() ?? "";
                    txtSellingPrice.Text = dr.Cells[8].Value?.ToString() ?? "";
                    txtDiscount.Text = dr.Cells[9].Value?.ToString() ?? "";
                    txtVAT.Text = dr.Cells[10].Value?.ToString() ?? "";
                    txtReorderPoint.Text = dr.Cells[11].Value?.ToString() ?? "";
                    txtBarcode.Text = dr.Cells[12].Value?.ToString() ?? "";
                    txtBCode.Text = dr.Cells[12].Value?.ToString() ?? "";
                    txtOpeningStock.Text = dr.Cells[13].Value?.ToString() ?? "";
                    Plimit.Text = dr.Cells[17].Value?.ToString() ?? "";

                    bool expiryDateIsValid = DateTime.TryParse(dr.Cells[15].Value?.ToString(), out DateTime expiryDate);
                    bool manufacturingDateIsValid = DateTime.TryParse(dr.Cells[14].Value?.ToString(), out DateTime manufacturingDate);

                    if (expiryDateIsValid && manufacturingDateIsValid)
                    {
                        dtpExpiryDate.Value = expiryDate;
                        dtpManufacturingDate.Value = manufacturingDate;
                        dtpExpiryDate.Enabled = false;
                        dtpManufacturingDate.Enabled = false;

                    }

                    txtSellingPrice2.Text = dr.Cells[16].Value?.ToString() ?? "";

                    // Validate barcode and generate it
                    string barcodeValue = dr.Cells[12]?.Value?.ToString()?.Trim() ?? string.Empty;

                    if (string.IsNullOrEmpty(barcodeValue) || !long.TryParse(barcodeValue, out _))
                    {
                        MessageBox.Show("Invalid barcode format. Please check the product barcode.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        // Select appropriate barcode format
                        BarcodeFormat barcodeFormat = (barcodeValue.Length == 12 || barcodeValue.Length == 13)
                                                      ? BarcodeFormat.UPC_A
                                                      : BarcodeFormat.CODE_128;

                        BarcodeWriter barcodeWriter = new BarcodeWriter
                        {
                            Format = barcodeFormat
                        };

                        picBarcode.Image = barcodeWriter.Write(barcodeValue);
                    }
                    // Enable/disable buttons as needed
                    btnUpdate.Enabled = true;
                    btnDelete.Enabled = true;
                    button1.Enabled = true;
                    btnSave.Enabled = false;
                    txtOpeningStock.Enabled = true;
                    txtOpeningStock.ReadOnly = false;

                    // Hide the dataGridView after selecting the row
                    dataGridView2.Visible = false;
                }
                

            }
            else
            {
                if (!string.IsNullOrEmpty(TextBox1.Text))
                {
                    dataGridView2.Visible = true;
                    GetFilteredData("ProductName", TextBox1.Text);
                }
                else
                {
                    dataGridView2.Visible = false;
                }
            }

        }

        private void dataGridView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {

                DataGridViewRow dr = dataGridView2.SelectedRows[0];
                
                txtID.Text = dr.Cells[0].Value?.ToString() ?? "";
                txtProductCode.Text = dr.Cells[1].Value?.ToString() ?? "";
                txtProductName.Text = dr.Cells[2].Value?.ToString() ?? "";
              
                txtFeatures.Text = dr.Cells[6].Value?.ToString() ?? "";
                txtCostPrice.Text = dr.Cells[7].Value?.ToString() ?? "";
                txtSellingPrice.Text = dr.Cells[8].Value?.ToString() ?? "";
                txtDiscount.Text = dr.Cells[9].Value?.ToString() ?? "";
                txtVAT.Text = dr.Cells[10].Value?.ToString() ?? "";
                txtReorderPoint.Text = dr.Cells[11].Value?.ToString() ?? "";
                txtBarcode.Text = dr.Cells[12].Value?.ToString() ?? "";
                txtBCode.Text = dr.Cells[12].Value?.ToString() ?? "";
                txtOpeningStock.Text = dr.Cells[13].Value?.ToString() ?? "";
                Plimit.Text = dr.Cells[17].Value?.ToString() ?? "";

                bool expiryDateIsValid = DateTime.TryParse(dr.Cells[15].Value?.ToString(), out DateTime expiryDate);
                bool manufacturingDateIsValid = DateTime.TryParse(dr.Cells[14].Value?.ToString(), out DateTime manufacturingDate);

                if (expiryDateIsValid && manufacturingDateIsValid)
                {
                    dtpExpiryDate.Value = expiryDate;
                    dtpManufacturingDate.Value = manufacturingDate;
                    dtpExpiryDate.Enabled = false;
                    dtpManufacturingDate.Enabled = false;

                }

                txtSellingPrice2.Text = dr.Cells[16].Value?.ToString() ?? "";
                txtSubCategoryID.Text = dr.Cells[3].Value?.ToString() ?? "";
                cmbCategory.Text = dr.Cells[4].Value?.ToString() ?? "";
                cmbSubCategory.Text = dr.Cells[5].Value?.ToString() ?? "";
                // Validate barcode and generate it
                string barcodeValue = dr.Cells[12]?.Value?.ToString()?.Trim() ?? string.Empty;

                if (string.IsNullOrEmpty(barcodeValue) || !long.TryParse(barcodeValue, out _))
                {
                    MessageBox.Show("Invalid barcode format. Please check the product barcode.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // Select appropriate barcode format
                    BarcodeFormat barcodeFormat = (barcodeValue.Length == 12 || barcodeValue.Length == 13)
                                                  ? BarcodeFormat.UPC_A
                                                  : BarcodeFormat.CODE_128;

                    BarcodeWriter barcodeWriter = new BarcodeWriter
                    {
                        Format = barcodeFormat
                    };

                    picBarcode.Image = barcodeWriter.Write(barcodeValue);
                }


                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
                button1.Enabled = true;
                btnSave.Enabled = false;
                txtOpeningStock.Enabled = true;
                txtOpeningStock.ReadOnly = false;
                dataGridView2.Visible = false;

            }
        }


        private void GetFilteredData(string columnName, string filterText)
        {
            try
            {
                // Validate columnName to prevent SQL injection
                if (columnName != "ProductName" && columnName != "p.Barcode" &&
                    columnName != "CategoryName" && columnName != "SubCategoryName")
                {
                    throw new ArgumentException("Invalid column name");
                }

                using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
                {
                    cn.Open();
                    string query = $@"
                SELECT
                    p.PID, 
                    RTRIM(p.ProductCode) AS ProductCode,
                    RTRIM(p.ProductName) AS ProductName,
                    p.SubCategoryID,
                    RTRIM(c.CategoryName) AS CategoryName,
                    RTRIM(sc.SubCategoryName) AS SubCategoryName,
                    RTRIM(p.Description) AS Description,
                    p.CostPrice,
                    p.SellingPrice,
                    p.Discount,
                    p.VAT,
                    p.ReorderPoint,
                    p.Barcode,  -- Specify p.Barcode to resolve ambiguity
                    p.OpeningStock,
                    p.ManufacturingDate,
                    p.ExpiryDate,
                    p.SellingPrice2,
                    p.Plimit
                FROM 
                    Category c
                INNER JOIN 
                    SubCategory sc ON c.CategoryName = sc.Category
                INNER JOIN 
                    Product p ON p.SubCategoryID = sc.ID
                INNER JOIN 
                    Temp_Stock ts ON ts.ProductID = p.PID
                INNER JOIN 
                    Warehouses w ON ts.WID = w.WID
                WHERE 
                    {columnName} LIKE @FilterText
                ORDER BY 
                    p.ProductCode;";

                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@FilterText", "%" + filterText.Trim() + "%");

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dataGridView2.Rows.Clear();
                            while (rdr.Read())
                            {
                                dataGridView2.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], rdr[16], rdr[17]);
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


    }
}
