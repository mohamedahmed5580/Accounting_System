using Accounting_System;
using CrystalDecisions.Shared;
using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;
using SixLabors.ImageSharp.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.Data.Helpers.ExpressiveSortInfo;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using DataTable = System.Data.DataTable;
using DrawingRectangle = System.Drawing.Rectangle;
using ExcelRectangle = Microsoft.Office.Interop.Excel.Rectangle;
using static Accounting_System.ModFunc;
using DevExpress.Utils.About;
namespace Accounting_System
{

    public partial class POS : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public static POS instance=null;
        public static POS Instancee()
        {
           if(instance == null)
           {
                instance = new POS();
           }

           return instance;
        }

        int i = 0;
        public POS()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            // إعداد القيم الافتراضية للنموذج
            InitializeDefaults();

            // تحميل البيانات وتحديث واجهة المستخدم
            Reset();

            // تعيين العنصر الأول لقائمة أخرى

        }
        private void POS_Load(object sender, EventArgs e)
        {
            instance = this;

        }

        private void InitializeDefaults()
        {
            cmbPaymentMode.SelectedIndex = 0;
            txtCustomerID.Text = "C-0001";
            txtCustomerName.Text = "عميل نقدي";
            txtCID.Text = "1";
            txtContactNo.Text = "00000000";
            txtBarcode.Focus();
            txtSM_ID.Text = "1";
            txtSalesmanID.Text = "SM-0001";
            txtSalesman.Text = "مندوب 1";
            txtCommissionPer.Text = "0.000";

            // التأكد من معرف العميل وتحديث وضع الدفع
            if (txtCustomerID.Text == "C-0001")
            {
                cmbPaymentMode.SelectedIndex = 0;
                txtPayment.ReadOnly = false;
            }
            else
            {
                txtPayment.ReadOnly = false;
            }

            // تعيين النسخة الحالية للنموذج لاستخدامها لاحقاً
            instance = this;
        }


        private void Button1_Click(object sender, EventArgs e)
        {
            SalesManScreen salesMan = new SalesManScreen();
            salesMan.lblSet.Text = "POS Entry";
            salesMan.Show();
        }
        public void Getdata1()
        {
            try
            {
                string query = @"SELECT PID, RTRIM(Product.ProductCode), RTRIM(ProductName), RTRIM(Temp_Stock.Barcode),
                               (CostPrice), (SellingPrice), (Discount), (VAT), Qty, RTRIM(Product.SellingPrice2)
                               FROM Temp_Stock
                               INNER JOIN Product ON Product.PID = Temp_Stock.ProductID
                               WHERE Qty > 0
                               ORDER BY ProductCode";

                DataTable dt = DataAccessLayer.ExecuteTable(query, CommandType.Text);

                dgw.Rows.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    // Ensure correct column indices or names match the query result
                    dgw.Rows.Add(row[0], row[1], row[2], row[3], row[4], row[5], row[6], row[7], row[8], row[9]);
                }
            }
            catch (SqlException dbEx)
            {
                MessageBox.Show("Database Error: " + dbEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("General Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            auto(); // Assuming auto() also uses DAL now
        }



        public void Reset()
        {
            this.SuspendLayout();
            try
            {

                // إعادة تعيين الحقول النصية والقوائم
                ComboBox1.SelectedIndex = 0;
                txtRemarks.Text = "";
                TextBox1.Text = "";
                txtAmount.Text = "";
                txtCostPrice.Text = "";
                txtDiscountAmount.Text = "";
                txtDiscountPer.Text = "";
                txtMargin.Text = "";
                txtProductCode.Text = "";
                txtProductName.Text = "";
                txtQty.Text = "";
                txtSellingPrice.Text = "";
                txtTotalAmount.Text = "";
                txtTotalQty.Text = "";
                txtVAT.Text = "";
                txtVATAmount.Text = "";
                txtGrandTotal.Text = "";
                txtTotalPayment.Text = "";
                txtTransactionNo_1.Text = "";
                txtT_ID_1.Text = "";
                textBox3.Text = "";
                txtPayment.Text = "0.00";
                txtTotalPayment.Text = "0.00";
                txtPaymentDue.Text = "0.00";
                Plimit.Text = "0";
                total_sale.Clear();

                // تحديث الرؤية لعناصر الواجهة
                textBox3.Visible = false;
                total_sale.Visible = true;
                label26.Visible = true;

                // إعادة تعيين التاريخ إلى تاريخ اليوم
                dtpInvoiceDate.Value = DateTime.Today;

                // إعادة تعيين DataGridView وإزالة الصفوف القديمة
                dgw.Visible = false;
                DataGridView1.Rows.Clear();
                DataGridView2.Rows.Clear();

                // تعيين حالات الأزرار
                btnDelete.Enabled = false;
                btnUpdate.Enabled = false;
                btnSave.Enabled = true;
                btnRemove.Enabled = false;
                btnAdd.Enabled = true;
                btnRemove1.Enabled = false;
                btnAdd1.Enabled = true;
                btnPrint.Enabled = false;
                Button2.Enabled = true;
                Button3.Enabled = false;
                button4.Enabled = false;
                Button1.Enabled = true;
                Getdata1();

                // استدعاء عمليات إضافية للمسح وإعادة التهيئة
                auto();
                Auto();
                Clear1();
                Clear();

                // إعادة تعيين وضع الدفع ومعرف العميل وقائمة معينة
                cmbPaymentMode.SelectedIndex = 0;
                txtCustomerID.Text = "C-0001";
                comboBoxGenerate();
                txtProductName.Focus();
            }
            finally
            {
                this.ResumeLayout(true); // Resume layout and repaint
            }
        }
        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void txtDiscountPer_TextChanged(object sender, EventArgs e)
        {
            Compute();
        }

        private void Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Button4_Click(object sender, EventArgs e)
        {

        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            SalesManScreen salesManScreen = new SalesManScreen();
            salesManScreen.lblSet.Text = "Billing";
            salesManScreen.Show();
        }
        private void CountValue()
        {
            try
            {
                string sql = "SELECT COUNT(TC_ID) FROM Payment_2 WHERE Amount = 0";
                object result = DataAccessLayer.ExecuteScalar(sql, CommandType.Text);
                i = (result == DBNull.Value || result == null) ? 0 : Convert.ToInt32(result);
            }
            catch (SqlException dbEx)
            {
                MessageBox.Show("Database Error counting payments: " + dbEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                i = 0; // Reset on error
            }
            catch (Exception ex)
            {
                MessageBox.Show("General Error counting payments: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                i = 0; // Reset on error
            }
        }

        private void Auto() // For Transaction ID
        {
            try
            {
                CountValue(); // Uses DAL now
                txtT_ID.Text = GenerateID_1(); // Uses DAL now
                txtTransactionNo.Text = "TC-" + (Convert.ToInt32(txtT_ID.Text) - i).ToString();
            }
            catch (FormatException formatEx) // Catch potential error from Convert.ToInt32
            {
                MessageBox.Show("Error generating transaction number format: " + formatEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtT_ID.Text = "0000"; // Default value on error
                txtTransactionNo.Text = "TC-Error";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating transaction number: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtT_ID.Text = "0000"; // Default value on error
                txtTransactionNo.Text = "TC-Error";
            }
        }


        private string GenerateID_1() // For Transaction ID (TC_ID)
        {
            string value = "0000";
            try
            {
                string query = "SELECT TOP 1 TC_ID FROM Payment_2 ORDER BY TC_ID DESC";
                object result = DataAccessLayer.ExecuteScalar(query, CommandType.Text);

                if (result != null && result != DBNull.Value)
                {
                    value = result.ToString();
                }

                // Rest of the ID generation logic (incrementing and padding)
                if (int.TryParse(value, out int numericValue))
                {
                    value = (numericValue + 1).ToString();
                    // Adjust the value with leading zeros
                    value = value.PadLeft(4, '0'); // Simpler padding
                }
                else
                {
                    value = "0001"; // Start from 1 if parsing fails or no records found
                }

            }
            catch (SqlException dbEx)
            {
                MessageBox.Show("Database Error generating TC_ID: " + dbEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                value = "0000"; // Default on DB error
            }
            catch (Exception ex)
            {
                MessageBox.Show("General Error generating TC_ID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                value = "0000"; // Default on general error
            }
            return value;
        }


        private void btnSelect_Click(object sender, EventArgs e)
        {

            CTOPOS ctopos = new CTOPOS();
            ctopos.lblSet.Text = "Billing";
            ctopos.Show();

        }


        private void button5_Click(object sender, EventArgs e)
        {
            AddCustomer addCustomer = new AddCustomer();
            addCustomer.Reset();
            addCustomer.lblSet.Text = "TOPOS";
            addCustomer.Show();
        }

        private void txtCustomerName_TextChanged(object sender, EventArgs e)
        {

        }


        public void auto()
        {
            try
            {
                txtID.Text = GenerateID();
                txtInvoiceNo.Text = "Inv-" + GenerateID();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void auto1() // Generate Customer ID/Code
        {
            try
            {
                txtCID.Text = GenerateID1(); // Uses DAL now
                txtCustomerID.Text = "C-" + txtCID.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating customer code: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCID.Text = "0001";
                txtCustomerID.Text = "C-Error";
            }
        }

        private string GenerateID() // For Invoice ID (Inv_ID)
        {
            string value = "0001"; // Default value
            try
            {
                string query = "SELECT ISNULL(MAX(Inv_ID), 0) FROM InvoiceInfo;";
                object result = DataAccessLayer.ExecuteScalar(query, CommandType.Text);

                if (result != null && result != DBNull.Value)
                {
                    if (int.TryParse(result.ToString(), out int numericValue))
                    {
                        numericValue++; // Increment the ID by 1
                        value = numericValue.ToString("D4"); // Format as 4 digits with leading zeros
                    }
                }
                // If result is null or DBNull, or parsing fails, 'value' remains "0001"
            }
            catch (SqlException dbEx)
            {
                MessageBox.Show($"Database Error generating Inv_ID: {dbEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                value = "0001"; // Default on DB error
            }
            catch (Exception ex)
            {
                MessageBox.Show($"General Error generating Inv_ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                value = "0001"; // Default on general error
            }
            return value;
        }




        public void Clear1()
        {
            cmbPaymentMode.SelectedIndex = 0;
            dtpPaymentDate.Text = DateTime.Today.ToString();
            btnAdd1.Enabled = true;
            btnRemove1.Enabled = false;
            btnListUpdate1.Enabled = false;
        }

        public void Clear()
        {

            txtBarcode.Text = "";
            txtProductCode.Text = "";
            txtProductName.Text = "";
            txtCostPrice.Text = "";
            txtSellingPrice.Text = "";
            txtMargin.Text = "";
            txtQty.Text = "";
            txtAmount.Text = "";
            txtDiscountPer.Text = "";
            txtDiscountAmount.Text = "";
            txtVAT.Text = "";
            txtVATAmount.Text = "";
            txtTotalAmount.Text = "";
            Plimit.Text = "";
            btnAdd.Enabled = true;
            btnRemove.Enabled = false;
            btnListUpdate.Enabled = false;
            dgw.Visible = false;
            txtProductName.Enabled = true;
            txtBarcode.Focus();
            visablility.Text = "";
            txtProductName.Focus();

        }
        public void ClearAddProdact()
        {

            // txtDiscountPer.Text = ""
            txtBarcode.Text = "";
            txtProductCode.Text = "";
            txtProductName.Text = "";
            txtCostPrice.Text = "";
            txtSellingPrice.Text = "";
            txtMargin.Text = "";
            txtQty.Text = "";
            txtAmount.Text = "";
            txtDiscountPer.Text = "";
            txtDiscountAmount.Text = "";
            txtVAT.Text = "";
            txtVATAmount.Text = "";
            Plimit.Text = "";
            btnAdd.Enabled = true;
            btnRemove.Enabled = false;
            btnListUpdate.Enabled = false;
            dgw.Visible = false;
            txtBarcode.Focus();
            cmbPaymentMode.SelectedIndex = 0;
            dtpPaymentDate.Text = DateTime.Today.ToString();
            btnAdd1.Enabled = true;
            btnRemove1.Enabled = false;
            btnListUpdate1.Enabled = false;
        }
        private void total_sale_save_Click(object sender, EventArgs e)
        {
            try
            {
                bool saveSuccess = POSSAVE(); // Call POSSAVE and check its result

                if (!saveSuccess) // If POSSAVE() fails, stop execution
                {
                    MessageBox.Show("فشل في الحفظ، يرجى التحقق من البيانات.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // Final confirmation message
                MessageBox.Show("تم الحفظ بنجاح", "الفاتورة", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
                btnPrint.Enabled = true;
                txtCustomerType.Text = "Regular Sale";
                Print();
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred during save: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }


        private void Button2_Click_1(object sender, EventArgs e)
        {
            try
            {

                bool saveSuccess = POSSAVE(); // Call POSSAVE and check its result

                if (!saveSuccess) // If POSSAVE() fails, stop execution
                {
                    MessageBox.Show("فشل في الحفظ، يرجى التحقق من البيانات.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Final confirmation message
                MessageBox.Show("تم الحفظ بنجاح", "الفاتورة", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                bool saveSuccess = POSSAVE(); // Call POSSAVE and check its result

                if (!saveSuccess) // If POSSAVE() fails, stop execution
                {
                    MessageBox.Show("فشل في الحفظ، يرجى التحقق من البيانات.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // Final confirmation message
                MessageBox.Show("تم الحفظ بنجاح", "الفاتورة", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
                btnPrint.Enabled = true;
                txtCustomerType.Text = "Non Regular Sale";
                Print();
                Reset();
            }
            catch (Exception ex)
            {

            }
        }
        private void button7_Click(object sender, EventArgs e)
        {

            try
            {
                bool saveSuccess = POSSAVE(); // Call POSSAVE and check its result

                if (!saveSuccess) // If POSSAVE() fails, stop execution
                {
                    MessageBox.Show("فشل في الحفظ، يرجى التحقق من البيانات.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // Final confirmation message
                MessageBox.Show("تم الحفظ بنجاح", "الفاتورة", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
                btnPrint.Enabled = true;
                txtCustomerType.Text = "Non Regular";
                Print();
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }

        public bool POSSAVE()
        {
            // --- Validation Checks (Remain the same) ---
            if (string.IsNullOrWhiteSpace(txtSalesmanID.Text))
            {
                MessageBox.Show("الرجاء تحديد رقم البائع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            // ... other validations ...
            if (DataGridView1.Rows.Count == 0) { return false; }
            if (DataGridView2.Rows.Count == 0) { }
            if (double.Parse(txtTotalPayment.Text) > double.Parse(txtGrandTotal.Text)) { return false; }

            if (DataGridView2.Rows.Count == 0)
            {
                DataGridView2.Rows.Add(cmbPaymentMode.Text, txtPayment.Text, dtpPaymentDate.Value.Date);
                txtTotalPayment.Text = Math.Round(TotalPayment(), 2).ToString();

                Compute1();
            }
            try // Wrap DAL operations in try-catch
            {
                // --- Company Profile Check ---
                string companyCheckQuery = "SELECT COUNT(*) FROM Company";
                object companyCount = DataAccessLayer.ExecuteScalar(companyCheckQuery, CommandType.Text);
                if (companyCount == null || Convert.ToInt32(companyCount) == 0)
                {
                    MessageBox.Show("الرجاء اضافة ملف تعريفي للشركة في القائمة الرئيسية", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                // --- Stock Quantity Check ---
                foreach (DataGridViewRow row in DataGridView1.Rows)
                {
                    if (row.IsNewRow) continue;
                    if (row.Cells[13].Value == null || row.Cells[6].Value == null) continue; // Check for null cell values

                    string stockCheckQuery = "SELECT Qty FROM Temp_Stock WHERE ProductID = @d1";
                    var paramProdId = DataAccessLayer.CreateParameter("@d1", SqlDbType.Int, Convert.ToInt32(row.Cells[13].Value));
                    object result = DataAccessLayer.ExecuteScalar(stockCheckQuery, CommandType.Text, paramProdId);

                    if (result != null && result != DBNull.Value && int.TryParse(result.ToString(), out int availableQty))
                    {
                        int addedQty = Convert.ToInt32(row.Cells[6].Value); // Use Convert.ToInt32
                        if (addedQty > availableQty)
                        {
                            MessageBox.Show($"الكمية المطلوبة ({addedQty}) للمنتج '{row.Cells[1]?.Value}' (باركود: {row.Cells[2]?.Value}) تتجاوز الكمية المتوفرة ({availableQty}).", "خطأ في الكمية", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show($"لم يتم العثور على مخزون للمنتج '{row.Cells[1]?.Value}' (باركود: {row.Cells[2]?.Value}) أو حدث خطأ.", "خطأ في المخزون", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false; // Stop if stock check fails
                    }
                }

                // --- Insert Customer (if new) ---
                if (!txtCustomerName.ReadOnly)
                {
                    auto1(); // Generates new Customer ID using DAL
                    string custInsertQuery = "INSERT INTO Customer(ID, CustomerID, [Name], Gender, Address, City, ContactNo, EmailID, Remarks, State, ZipCode, Photo, CustomerType) " +
                                                "VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9, @d10, @d11, @d12, 'Non Regular')";
                    byte[] photoBytes;
                    using (var ms = new MemoryStream())
                    {
                        var bmpImage = new Bitmap(Properties.Resources.if_icons_user1); // Ensure this resource exists
                        bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        photoBytes = ms.ToArray();
                    }

                    var custParams = new[] {
            DataAccessLayer.CreateParameter("@d1", SqlDbType.Int, Convert.ToInt32(txtCID.Text)),
            DataAccessLayer.CreateParameter("@d2", SqlDbType.NVarChar, txtCustomerID.Text),
            DataAccessLayer.CreateParameter("@d3", SqlDbType.NVarChar, txtCustomerName.Text),
            DataAccessLayer.CreateParameter("@d4", SqlDbType.NVarChar, string.Empty),
            DataAccessLayer.CreateParameter("@d5", SqlDbType.NVarChar, string.Empty),
            DataAccessLayer.CreateParameter("@d6", SqlDbType.NVarChar, string.Empty),
            DataAccessLayer.CreateParameter("@d7", SqlDbType.NVarChar, txtContactNo.Text),
            DataAccessLayer.CreateParameter("@d8", SqlDbType.NVarChar, string.Empty),
            DataAccessLayer.CreateParameter("@d9", SqlDbType.NVarChar, string.Empty),
            DataAccessLayer.CreateParameter("@d10", SqlDbType.NVarChar, string.Empty),
            DataAccessLayer.CreateParameter("@d11", SqlDbType.NVarChar, string.Empty),
            DataAccessLayer.CreateParameter("@d12", SqlDbType.VarBinary, photoBytes)
        };
                    DataAccessLayer.ExecuteNonQuery(custInsertQuery, CommandType.Text, custParams);
                    txtCustomerType.Text = "Non Regular";
                }

                // --- Handle Payment Due (Insert/Update Payment_2) ---
                decimal paymentDue = Convert.ToDecimal(txtPaymentDue.Text);
                if (paymentDue != 0) // Check if there is a balance
                {
                    // Check if TC_ID exists
                    string checkPayQuery = "SELECT COUNT(*) FROM Payment_2 WHERE TC_ID = @d1";
                    var paramTcIdCheck = DataAccessLayer.CreateParameter("@d1", SqlDbType.Int, Convert.ToInt32(txtT_ID.Text));
                    object payCount = DataAccessLayer.ExecuteScalar(checkPayQuery, CommandType.Text, paramTcIdCheck);
                    bool recordExists = (payCount != null && Convert.ToInt32(payCount) > 0);

                    string payQuery;
                    var payParams = new List<SqlParameter> {
            DataAccessLayer.CreateParameter("@d1", SqlDbType.Int, Convert.ToInt32(txtT_ID.Text)),
            DataAccessLayer.CreateParameter("@d2", SqlDbType.NVarChar, txtTransactionNo.Text),
            DataAccessLayer.CreateParameter("@d3", SqlDbType.Date, dtpPaymentDate.Value.Date),
            DataAccessLayer.CreateParameter("@d4", SqlDbType.NVarChar, cmbPaymentMode.Text), // Assumes payment mode text is suitable
            DataAccessLayer.CreateParameter("@d5", SqlDbType.NVarChar, txtCustomerID.Text), // CustomerID is NVarChar
            DataAccessLayer.CreateParameter("@d6", SqlDbType.Decimal, -paymentDue), // Store balance as negative amount
            DataAccessLayer.CreateParameter("@d7", SqlDbType.NVarChar, txtRemarks.Text),
            DataAccessLayer.CreateParameter("@d8", SqlDbType.NVarChar, txtCheck.Text), // Assuming Check ID is text
            DataAccessLayer.CreateParameter("@d9", SqlDbType.Date, dtpPaymentDate.Value.Date), // Check Date
            DataAccessLayer.CreateParameter("@d10", SqlDbType.Int, Convert.ToInt32(txtSM_ID.Text)), // SalesMan ID (numeric)
            DataAccessLayer.CreateParameter("@d11", SqlDbType.NVarChar, txtSalesman.Text), // SalesMan Name
            DataAccessLayer.CreateParameter("@d12", SqlDbType.Decimal, Convert.ToDecimal(txtCommissionPer.Text)), // Commission %
            DataAccessLayer.CreateParameter("@d13", SqlDbType.NVarChar, txtSalesmanID.Text) // SalesMan Code (text)
        };

                    if (recordExists)
                    {
                        // Update existing record
                        payQuery = @"UPDATE Payment_2 SET TransactionID = @d2, [Date] = @d3, PaymentMode = @d4, CustomerID = @d5,
                        Amount = @d6, Remarks = @d7, Check_ID = @d8, Check_Date = @d9, SalesMan_ID = @d10,
                        SalesMan_Name = @d11, SalesMan_Comession = @d12, SalesMan_ID_2 = @d13
                        WHERE TC_ID = @d1";
                    }
                    else
                    {
                        // Insert new record
                        payQuery = @"INSERT INTO Payment_2(TC_ID, TransactionID, [Date], PaymentMode, CustomerID, Amount, Remarks,
                        Check_ID, Check_Date, SalesMan_ID, SalesMan_Name, SalesMan_Comession, SalesMan_ID_2)
                        VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9, @d10, @d11, @d12, @d13)";
                    }
                    DataAccessLayer.ExecuteNonQuery(payQuery, CommandType.Text, payParams.ToArray());
                }


                // --- Insert Invoice Info ---
                string invoiceQuery;
                var invParams = new List<SqlParameter> {
            DataAccessLayer.CreateParameter("@d1", SqlDbType.Int, Convert.ToInt32(txtID.Text)),
            DataAccessLayer.CreateParameter("@d2", SqlDbType.NVarChar, txtInvoiceNo.Text),
            DataAccessLayer.CreateParameter("@d3", SqlDbType.Date, dtpInvoiceDate.Value.Date),
            DataAccessLayer.CreateParameter("@d4", SqlDbType.Int, Convert.ToInt32(txtCID.Text)),
            DataAccessLayer.CreateParameter("@d5", SqlDbType.Float, Convert.ToDouble(txtGrandTotal.Text)),
            DataAccessLayer.CreateParameter("@d6", SqlDbType.Float, Convert.ToDouble(txtTotalPayment.Text)),
            DataAccessLayer.CreateParameter("@d7", SqlDbType.Float, Convert.ToDouble(txtPaymentDue.Text)),
            DataAccessLayer.CreateParameter("@d8", SqlDbType.NVarChar, txtRemarks.Text),
            DataAccessLayer.CreateParameter("@d9", SqlDbType.Int, Convert.ToInt32(txtSM_ID.Text)),
            DataAccessLayer.CreateParameter("@d11", SqlDbType.Int, Convert.ToInt32(WID.Text)), // WID
            DataAccessLayer.CreateParameter("@d12", SqlDbType.Decimal, Convert.ToDecimal(total_sale.Text)) // total_sale
        };

                if (paymentDue != 0)
                {
                    invoiceQuery = @"INSERT INTO InvoiceInfo(Inv_ID, InvoiceNo, InvoiceDate, CustomerID, GrandTotal, TotalPaid, Balance, Remarks, SalesmanID, TC_ID, WID, total_sale)
                            VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9, @d10, @d11, @d12)";
                    invParams.Add(DataAccessLayer.CreateParameter("@d10", SqlDbType.Int, Convert.ToInt32(txtT_ID.Text))); // TC_ID only if balance
                }
                else
                {
                    invoiceQuery = @"INSERT INTO InvoiceInfo(Inv_ID, InvoiceNo, InvoiceDate, CustomerID, GrandTotal, TotalPaid, Balance, Remarks, SalesmanID, WID, total_sale)
                            VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9, @d11, @d12)"; // No TC_ID if no balance
                }
                DataAccessLayer.ExecuteNonQuery(invoiceQuery, CommandType.Text, invParams.ToArray());


                // --- Insert Salesman Commission ---
                decimal totalMargin = DataGridView1.Rows.Cast<DataGridViewRow>()
                                        .Where(r => !r.IsNewRow && r.Cells[12].Value != null) // Use Margin column index (5) or TotalAmount (12)? Assuming 12 based on original code
                                        .Sum(r => Convert.ToDecimal(r.Cells[12].Value)); // Use index 12 for TotalAmount

                decimal commission = totalMargin * Convert.ToDecimal(txtCommissionPer.Text) / 100;
                string commQuery = "INSERT INTO Salesman_Commission(InvoiceID, CommissionPer, Commission) VALUES (@T1, @T2, @T3)";
                var commParams = new[] {
        DataAccessLayer.CreateParameter("@T1", SqlDbType.Int, Convert.ToInt32(txtID.Text)),
        DataAccessLayer.CreateParameter("@T2", SqlDbType.Decimal, Convert.ToDecimal(txtCommissionPer.Text)),
        DataAccessLayer.CreateParameter("@T3", SqlDbType.Decimal, commission)
    };
                DataAccessLayer.ExecuteNonQuery(commQuery, CommandType.Text, commParams);

                // --- Insert/Update Invoice Products ---
                string invoiceProductQuery = @"
            IF EXISTS (SELECT 1 FROM Invoice_Product WHERE InvoiceID = @d1 AND ProductID = @d13)
                UPDATE Invoice_Product
                SET Barcode = @d2, CostPrice = @d3, SellingPrice = @d4, Margin = @d5,
                    Qty = @d6, Amount = @d7, DiscountPer = @d8, Discount = @d9,
                    VATPer = @d10, VAT = @d11, TotalAmount = @d12
                WHERE InvoiceID = @d1 AND ProductID = @d13
            ELSE
                INSERT INTO Invoice_Product(InvoiceID, Barcode, CostPrice, SellingPrice, Margin, Qty, Amount, DiscountPer, Discount, VATPer, VAT, TotalAmount, ProductID)
                VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9, @d10, @d11, @d12, @d13)";

                foreach (DataGridViewRow row in DataGridView1.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        var prodParams = new[] {
                DataAccessLayer.CreateParameter("@d1", SqlDbType.Int, Convert.ToInt32(txtID.Text)), // InvoiceID
                DataAccessLayer.CreateParameter("@d13", SqlDbType.Int, Convert.ToInt32(row.Cells[13].Value)), // ProductID
                DataAccessLayer.CreateParameter("@d2", SqlDbType.NVarChar, row.Cells[2].Value?.ToString() ?? ""),  // Barcode
                DataAccessLayer.CreateParameter("@d3", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[3].Value)), // CostPrice
                DataAccessLayer.CreateParameter("@d4", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[4].Value)), // SellingPrice
                DataAccessLayer.CreateParameter("@d5", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[5].Value)), // Margin
                DataAccessLayer.CreateParameter("@d6", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[6].Value)), // Qty
                DataAccessLayer.CreateParameter("@d7", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[7].Value)), // Amount
                DataAccessLayer.CreateParameter("@d8", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[8].Value)), // DiscountPer
                DataAccessLayer.CreateParameter("@d9", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[9].Value)), // Discount
                DataAccessLayer.CreateParameter("@d10", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[10].Value)), // VATPer
                DataAccessLayer.CreateParameter("@d11", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[11].Value)), // VAT
                DataAccessLayer.CreateParameter("@d12", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[12].Value)) // TotalAmount
                };
                        DataAccessLayer.ExecuteNonQuery(invoiceProductQuery, CommandType.Text, prodParams);
                    }
                }


                // --- Insert Invoice Payments ---
                string invPayQuery = "INSERT INTO Invoice_Payment(InvoiceID, PaymentMode, TotalPaid, PaymentDate) VALUES (@d1, @d4, @d5, @d6)";
                foreach (DataGridViewRow row in DataGridView2.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        var invPayParams = new[] {
                    DataAccessLayer.CreateParameter("@d1", SqlDbType.Int, Convert.ToInt32(txtID.Text)),
                    DataAccessLayer.CreateParameter("@d4", SqlDbType.NVarChar, row.Cells[0].Value?.ToString() ?? ""), // PaymentMode
                    DataAccessLayer.CreateParameter("@d5", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[1].Value)), // TotalPaid
                    DataAccessLayer.CreateParameter("@d6", SqlDbType.DateTime, Convert.ToDateTime(row.Cells[2].Value)) // PaymentDate
            };
                        DataAccessLayer.ExecuteNonQuery(invPayQuery, CommandType.Text, invPayParams);
                    }
                }

                // --- Update Stock ---
                string stockUpdateQuery = "UPDATE Temp_Stock SET Qty = Qty - @Qty WHERE ProductID = @d1 AND WID = @d3"; // Removed Barcode check, assuming ProductID+WID is unique enough for stock
                foreach (DataGridViewRow row in DataGridView1.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        var stockParams = new[] {
                DataAccessLayer.CreateParameter("@Qty", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[6].Value)),
                DataAccessLayer.CreateParameter("@d1", SqlDbType.Int, Convert.ToInt32(row.Cells[13].Value)),
                //DataAccessLayer.CreateParameter("@d2", SqlDbType.NVarChar, row.Cells[2].Value?.ToString() ?? ""), // Barcode - Removed
                    DataAccessLayer.CreateParameter("@d3", SqlDbType.Int, Convert.ToInt32(row.Cells[14].Value)) // WID
            };
                        DataAccessLayer.ExecuteNonQuery(stockUpdateQuery, CommandType.Text, stockParams);
                    }
                }

                // --- Ledger Entries ---
                string ledgerName = "نقدا"; // Default
                if (cmbPaymentMode.SelectedIndex == 1) ledgerName = "شيك رقم " + txtCheck.Text.Trim();
                else if (cmbPaymentMode.SelectedIndex == 4) ledgerName = "اجل";

                LedgerSave(dtpPaymentDate.Value.Date, ledgerName, txtInvoiceNo.Text, "فاتورة مبيعات",
                            Math.Abs(Convert.ToDecimal(txtGrandTotal.Text)),
                            Math.Abs(Convert.ToDecimal(txtTotalPayment.Text)),
                            txtCustomerID.Text, "", Math.Abs(Convert.ToDecimal(total_sale.Text))); // Uses DAL internally

            }
            catch (SqlException dbEx)
            {
                MessageBox.Show($"Database error during save: {dbEx.Message}\n(Check item quantities and database constraints)", "خطأ قاعدة البيانات", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; // Indicate failure
            }
            catch (FormatException formatEx)
            {
                MessageBox.Show($"Data format error during save: {formatEx.Message}\n(Check numeric fields like Quantity, Price, etc.)", "خطأ تنسيق البيانات", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred during save: {ex.Message}", "خطأ غير متوقع", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; // Indicate failure
            }

            return true; // Indicate success
        }
        private double GrandTotal()
        {
            double sum = 0d;
            try
            {
                foreach (DataGridViewRow r in this.DataGridView1.Rows)
                    // txtPayment.Text = sum
                    sum = sum + Convert.ToDouble(r.Cells[12].Value.ToString());
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
            }
            return sum;
        }


    

        private void btnSelectionInv_Click(object sender, EventArgs e)
        {
            Stock stock = new Stock();
            stock.lblSet.Text = "Billing";
            stock.ShowDialog();
        }

        private void txtSellingPrice_TextChanged(object sender, EventArgs e)
        {

        }
        private void txtVAT_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtDiscountAmount_TextChanged(object sender, EventArgs e)
        {
            txtTotalAmount.Text = (
            (string.IsNullOrWhiteSpace(txtAmount.Text) ? 0 : Convert.ToDouble(txtAmount.Text)) +
            (string.IsNullOrWhiteSpace(txtVATAmount.Text) ? 0 : Convert.ToDouble(txtVATAmount.Text)) -
            (string.IsNullOrWhiteSpace(txtDiscountAmount.Text) ? 0 : Convert.ToDouble(txtDiscountAmount.Text))
        ).ToString("F2");

        }
        public void Compute()
        {
            double num1, num2, num3, num4, num5;

            // Use helper method to parse or return 0 if empty/invalid
            double sellingPrice = ParseOrDefault(txtSellingPrice.Text);
            double costPrice = ParseOrDefault(txtCostPrice.Text);
            double qty = ParseOrDefault(txtQty.Text);
            //(Val(txtQty.Text) - Val(rival.Text)
            double discountPer = ParseOrDefault(txtDiscountPer.Text);
            double vat = ParseOrDefault(txtVAT.Text);

            // Calculate Margin
            txtMargin.Text = ((sellingPrice - costPrice) * qty).ToString("F2");

            // Calculate Amount
            num1 = qty * sellingPrice;
            num1 = Math.Round(num1, 2);
            txtAmount.Text = num1.ToString("F2");

            // Calculate Discount Amount
            num2 = num1 * discountPer / 100;
            num2 = Math.Round(num2, 2);
            txtDiscountAmount.Text = num2.ToString("F2");

            // Calculate VAT Amount
            num3 = num1 - num2;
            num4 = vat * num3 / 100;
            num4 = Math.Round(num4, 2);
            txtVATAmount.Text = num4.ToString("F2");

            // Calculate Total Amount
            num5 = num1 + num4 - num2;
            num5 = Math.Round(num5, 2);
            txtTotalAmount.Text = num5.ToString("F2");
        }

        // Helper method to parse a string to double or return 0 if invalid/empty
        private double ParseOrDefault(string input)
        {
            if (double.TryParse(input, out double result))
            {
                return result;
            }
            return 0;
        }



        private void txtQty_TextChanged(object sender, EventArgs e)
        {

            Compute();

        }


        public void Compute1()
        {
            double i = 0d;
            i = ParseOrDefault(txtGrandTotal.Text) - ParseOrDefault(txtTotalPayment.Text);
            i = Math.Round(i, 2);
            txtPaymentDue.Text = i.ToString();
            txtPaymentDue.Text = (Convert.ToDouble(txtPaymentDue.Text) - Convert.ToDouble(total_sale.Text)).ToString();

        }
        private void Compute2()
        {
            double totalPayment = 0;
            foreach (DataGridViewRow row in DataGridView2.Rows)
            {
                if (!row.IsNewRow)
                {
                    if (double.TryParse(row.Cells[1].Value.ToString(), out double paymentAmount))
                    {
                        totalPayment += paymentAmount;
                    }
                }
            }

            txtTotalPayment.Text = totalPayment.ToString("F2");

            if (double.TryParse(txtGrandTotal.Text, out double grandTotal) &&
                double.TryParse(txtTotalPayment.Text, out totalPayment))
            {
                /*                txtChange.Text = (totalPayment - grandTotal).ToString("F2");
                */
            }
        }

        private double TotalPayment()
        {
            double totalPayment = 0;
            foreach (DataGridViewRow row in DataGridView2.Rows)
            {
                if (!row.IsNewRow)
                {
                    if (double.TryParse(row.Cells[1].Value.ToString(), out double paymentAmount))
                    {
                        totalPayment += paymentAmount;
                    }
                }
            }
            return totalPayment;
        }
       
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Set default values for fields if empty
                txtQty.Text = string.IsNullOrWhiteSpace(txtQty.Text) ? "1" : txtQty.Text;
                txtDiscountPer.Text = string.IsNullOrWhiteSpace(txtDiscountPer.Text) ? "0" : txtDiscountPer.Text;
                txtVAT.Text = string.IsNullOrWhiteSpace(txtVAT.Text) ? "0" : txtVAT.Text;

                int reorderPoint = 0;  // Variable to store the ReorderPoint value

                string connectionString = DataAccessLayer.Con();  // Replace with your actual connection string
                string query = "SELECT ReorderPoint FROM Product WHERE PID = @PID";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PID", txtProductID.Text);

                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();  // ExecuteScalar returns the first column of the first row in the result set

                        if (result != null)
                        {
                            reorderPoint = Convert.ToInt32(result);  // Convert the result to int
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }

                // Validation checks for required fields
                if (string.IsNullOrWhiteSpace(txtProductCode.Text))
                {
                    MessageBox.Show("الرجاء إدراج رقم الصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProductCode.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtBarcode.Text))
                {
                    MessageBox.Show("الرجاء إدراج الباركود للصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBarcode.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtSellingPrice.Text))
                {
                    MessageBox.Show("الرجاء كتلبة السعر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSellingPrice.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtDiscountPer.Text))
                {
                    MessageBox.Show("الرجاء تحديد الخصم %", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDiscountPer.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtVAT.Text))
                {
                    MessageBox.Show("الرجاء تحديد الضريبة %", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtVAT.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtQty.Text))
                {
                    MessageBox.Show("الرجاء تحديد الكمية", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQty.Focus();
                    return;
                }
                if (txtQty.Text == "0")
                {
                    MessageBox.Show("الكمية يجب ان تكون اكبر من الصفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQty.Focus();
                    return;
                }

                if (Convert.ToDecimal(txtSellingPrice.Text) < Convert.ToDecimal(Plimit.Text))
                {
                    MessageBox.Show("لا يمكن تخطي حد السعر " + Plimit.Text, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(comboBox2.Text))
                {
                    MessageBox.Show("الرجاء تحديد المخزن", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    comboBox2.Focus();
                    return;
                }


                string connectionString1 = DataAccessLayer.Con();  // Replace with your actual connection string
                string query1 = "SELECT Qty FROM Product WHERE PID = @PID";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PID", txtProductID.Text);

                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();  // ExecuteScalar returns the first column of the first row in the result set

                        if (result != null)
                        {
                            reorderPoint = Convert.ToInt32(result);  // Convert the result to int
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }


                // Check if the product already exists in the DataGridView
                bool rowUpdated = false;
                double qty = Convert.ToDouble(txtQty.Text);
                double amount = Convert.ToDouble(txtAmount.Text);
                double discountAmount = Convert.ToDouble(txtDiscountAmount.Text);
                double vatAmount = Convert.ToDouble(txtVATAmount.Text);
                double totalAmount = Convert.ToDouble(txtTotalAmount.Text);

                foreach (DataGridViewRow row in DataGridView1.Rows)
                {
                    // Assuming ProductCode is the unique identifier for products
                    if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == txtProductCode.Text)
                    {
                        // Update the existing row
                        row.Cells[6].Value = Convert.ToDouble(row.Cells[6].Value) + qty; // Update Qty
                        row.Cells[7].Value = Convert.ToDouble(row.Cells[7].Value) + amount; // Update Amount
                        row.Cells[9].Value = Convert.ToDouble(row.Cells[9].Value) + discountAmount; // Update Discount
                        row.Cells[11].Value = Convert.ToDouble(row.Cells[11].Value) + vatAmount; // Update VAT
                        row.Cells[12].Value = Convert.ToDouble(row.Cells[12].Value) + totalAmount; // Update Total Amount

                        rowUpdated = true;
                        break;
                    }
                }
                // If product was not found, add a new row
                if (!rowUpdated)
                {
                    DataGridView1.Rows.Add(
                        txtProductCode.Text, txtProductName.Text, txtBarcode.Text,
                        Convert.ToDouble(txtCostPrice.Text), Convert.ToDouble(txtSellingPrice.Text),
                        Convert.ToDouble(txtMargin.Text), qty, amount,
                        Convert.ToDouble(txtDiscountPer.Text), discountAmount,
                        Convert.ToDouble(txtVAT.Text), vatAmount,
                        totalAmount, Convert.ToDouble(txtProductID.Text),
                        WID.Text
                    );
                }

                // Update totals
                double grandTotal = GrandTotal();
                grandTotal = Math.Round(grandTotal, 2);
                txtGrandTotal.Text = grandTotal.ToString("F2");
                txtTotalPayment.Text = grandTotal.ToString("F2");

                // Recompute necessary values
                Compute1();

                // Reset specific fields
                txtPayment.Text = txtGrandTotal.Text.ToString();
                txtDiscountPer.Text = "";
                txtAmount.Text = "";
                txtDiscountAmount.Text = "";
                Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerateID1() // For Customer ID
        {
            string value = "0000";
            try
            {
                string query = "SELECT TOP 1 ID FROM Customer ORDER BY ID DESC";
                object result = DataAccessLayer.ExecuteScalar(query, CommandType.Text);

                if (result != null && result != DBNull.Value)
                {
                    value = result.ToString();
                }

                if (int.TryParse(value, out int idValue))
                {
                    idValue++;
                    value = idValue.ToString("D4"); // Format to 4 digits
                }
                else
                {
                    value = "0001"; // Default if parsing fails or no records
                }
            }
            catch (SqlException dbEx)
            {
                MessageBox.Show("Database Error generating Customer ID: " + dbEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                value = "0001"; // Default on error
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating Customer ID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                value = "0001"; // Default on error
            }
            return value;
        }
        private void btnUpdate_Click(object sender, EventArgs e) // Update existing Invoice
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtSalesmanID.Text))
            {
                MessageBox.Show("الرجاء تحديد رقم البائع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Button1.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("الرجاء تحديد العميل", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (DataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("الرجاء اضافة اصناف للشبكة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (DataGridView2.Rows.Count == 0)
            {
                DataGridView2.Rows.Add(cmbPaymentMode.Text, txtPayment.Text, dtpPaymentDate.Value.Date);
                double j = Math.Round(TotalPayment(), 2);
                txtTotalPayment.Text = j.ToString();
                Compute1();
            }
            if (cmbPaymentMode.SelectedIndex == 0)
            {
                DataGridView2.Rows.Clear();
                DataGridView2.Rows.Add(cmbPaymentMode.Text, txtPayment.Text, dtpPaymentDate.Value.Date);
                double j = Math.Round(TotalPayment(), 2);
                txtTotalPayment.Text = j.ToString();
                Compute1();
            }
            if (Convert.ToDouble(txtTotalPayment.Text) > Convert.ToDouble(txtGrandTotal.Text))
            {
                MessageBox.Show("المبلغ المدفوع لا يكون اكبر من قيمة الفاتورة", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            try // Wrap all DB operations
            {
                // --- Stock Quantity Check (Compare DGV1 with DGV3 - Original state) ---
                foreach (DataGridViewRow row1 in DataGridView1.Rows)
                {
                    if (row1.IsNewRow || row1.Cells[13].Value == null || row1.Cells[6].Value == null) continue;

                    int productId1 = Convert.ToInt32(row1.Cells[13].Value);
                    int wid1 = Convert.ToInt32(row1.Cells[14].Value);
                    int qty1 = Convert.ToInt32(row1.Cells[6].Value);

                    // Find the original quantity from dataGridView3 (backup grid)
                    int originalQty = dataGridView3.Rows.Cast<DataGridViewRow>()
                                        .Where(r3 => !r3.IsNewRow && r3.Cells[13].Value != null && r3.Cells[14].Value != null &&
                                                     r3.Cells[13].Value.ToString() == productId1.ToString() &&
                                                     r3.Cells[14].Value.ToString() == wid1.ToString())
                                        .Select(r3 => Convert.ToInt32(r3.Cells[6].Value))
                                        .FirstOrDefault(); // Gets 0 if not found

                    int qtyDifference = qty1 - originalQty;

                    if (qtyDifference > 0) // Only check if quantity increased or item is new
                    {
                        string stockCheckQuery = "SELECT Qty FROM Temp_Stock WHERE ProductID = @PID AND WID = @WID";
                        var stockParams = new[] {
                            DataAccessLayer.CreateParameter("@PID", SqlDbType.Int, productId1),
                            DataAccessLayer.CreateParameter("@WID", SqlDbType.Int, wid1)
                         };
                        object stockResult = DataAccessLayer.ExecuteScalar(stockCheckQuery, CommandType.Text, stockParams);

                        if (stockResult != null && stockResult != DBNull.Value && int.TryParse(stockResult.ToString(), out int availableQty))
                        {
                            if (qtyDifference > availableQty) // Check if the *increase* exceeds available stock
                            {
                                MessageBox.Show($"الكمية المضافة ({qtyDifference}) للمنتج '{row1.Cells[1]?.Value}' تتجاوز الكمية المتوفرة ({availableQty}).", "خطأ في الكمية", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return; // Stop update
                            }
                        }
                        else
                        {
                            MessageBox.Show($"لم يتم العثور على مخزون للمنتج '{row1.Cells[1]?.Value}' في المخزن المحدد.", "خطأ في المخزون", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }


                // --- Update InvoiceInfo ---
                string invUpdateQuery = @"UPDATE InvoiceInfo SET InvoiceNo=@d2, CustomerID=@d4, GrandTotal=@d5,
                                       TotalPaid=@d6, Balance=@d7, Remarks=@d8, SalesmanID=@d9, InvoiceDate=@d3,
                                       WID=@d10, total_sale=@d11
                                       WHERE Inv_ID=@d1";
                var invUpdateParams = new[] {
                    DataAccessLayer.CreateParameter("@d1", SqlDbType.Int, Convert.ToInt32(txtID.Text)),
                    DataAccessLayer.CreateParameter("@d2", SqlDbType.NVarChar, txtInvoiceNo.Text),
                    DataAccessLayer.CreateParameter("@d4", SqlDbType.Int, Convert.ToInt32(txtCID.Text)),
                    DataAccessLayer.CreateParameter("@d5", SqlDbType.Float, Convert.ToDouble(txtGrandTotal.Text)),
                    DataAccessLayer.CreateParameter("@d6", SqlDbType.Float, Convert.ToDouble(txtTotalPayment.Text)),
                    DataAccessLayer.CreateParameter("@d7", SqlDbType.Float, Convert.ToDouble(txtPaymentDue.Text)),
                    DataAccessLayer.CreateParameter("@d8", SqlDbType.NVarChar, txtRemarks.Text),
                    DataAccessLayer.CreateParameter("@d9", SqlDbType.Int, Convert.ToInt32(txtSM_ID.Text)),
                    DataAccessLayer.CreateParameter("@d3", SqlDbType.Date, dtpInvoiceDate.Value.Date),
                    DataAccessLayer.CreateParameter("@d10", SqlDbType.Int, Convert.ToInt32(WID.Text)),
                    DataAccessLayer.CreateParameter("@d11", SqlDbType.Decimal, Convert.ToDecimal(total_sale.Text))
                 };
                DataAccessLayer.ExecuteNonQuery(invUpdateQuery, CommandType.Text, invUpdateParams);


                // --- Delete and Re-insert Invoice Products ---
                string delProdQuery = "DELETE FROM Invoice_Product WHERE InvoiceID=@d1";
                var delProdParam = DataAccessLayer.CreateParameter("@d1", SqlDbType.Int, Convert.ToInt32(txtID.Text));
                DataAccessLayer.ExecuteNonQuery(delProdQuery, CommandType.Text, delProdParam);

                string insProdQuery = @"INSERT INTO Invoice_Product(InvoiceID, Barcode, CostPrice, SellingPrice, Margin, Qty,
                                        Amount, DiscountPer, Discount, VATPer, VAT, TotalAmount, ProductID)
                                        VALUES (@d1, @d4, @d5, @d6, @d7, @d8, @d9, @d10, @d11, @d12, @d13, @d14, @d15)";
                foreach (DataGridViewRow row in DataGridView1.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        var prodParams = new[] {
                             DataAccessLayer.CreateParameter("@d1", SqlDbType.Int, Convert.ToInt32(txtID.Text)),
                             DataAccessLayer.CreateParameter("@d4", SqlDbType.NVarChar, row.Cells[2].Value?.ToString() ?? ""), // Barcode
                             DataAccessLayer.CreateParameter("@d5", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[3].Value)),// CostPrice
                             DataAccessLayer.CreateParameter("@d6", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[4].Value)),// SellingPrice
                             DataAccessLayer.CreateParameter("@d7", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[5].Value)), // Margin
                             DataAccessLayer.CreateParameter("@d8", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[6].Value)), // Qty
                             DataAccessLayer.CreateParameter("@d9", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[7].Value)), // Amount
                             DataAccessLayer.CreateParameter("@d10", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[8].Value)),// DiscountPer
                             DataAccessLayer.CreateParameter("@d11", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[9].Value)),// Discount
                             DataAccessLayer.CreateParameter("@d12", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[10].Value)),// VATPer
                             DataAccessLayer.CreateParameter("@d13", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[11].Value)),// VAT
                             DataAccessLayer.CreateParameter("@d14", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[12].Value)),// TotalAmount
                             DataAccessLayer.CreateParameter("@d15", SqlDbType.Int, Convert.ToInt32(row.Cells[13].Value)) // ProductID
                        };
                        DataAccessLayer.ExecuteNonQuery(insProdQuery, CommandType.Text, prodParams);
                    }
                }


                // --- Delete and Re-insert Invoice Payments ---
                string delPayQuery = "DELETE FROM Invoice_Payment WHERE InvoiceID=@d1";
                var delPayParam = DataAccessLayer.CreateParameter("@d1", SqlDbType.Int, Convert.ToInt32(txtID.Text));
                DataAccessLayer.ExecuteNonQuery(delPayQuery, CommandType.Text, delPayParam);

                string insPayQuery = "INSERT INTO Invoice_Payment(InvoiceID, PaymentMode, TotalPaid, PaymentDate) VALUES (@d1, @d4, @d5, @d6)";
                foreach (DataGridViewRow row in DataGridView2.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        var payParams = new[] {
                            DataAccessLayer.CreateParameter("@d1", SqlDbType.Int, Convert.ToInt32(txtID.Text)),
                            DataAccessLayer.CreateParameter("@d4", SqlDbType.NVarChar, row.Cells[0].Value?.ToString() ?? ""),
                            DataAccessLayer.CreateParameter("@d5", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[1].Value)),
                            DataAccessLayer.CreateParameter("@d6", SqlDbType.DateTime, Convert.ToDateTime(row.Cells[2].Value))
                        };
                        DataAccessLayer.ExecuteNonQuery(insPayQuery, CommandType.Text, payParams);
                    }
                }


                // --- Update Stock (Complex Logic: Adjust based on difference between DGV1 and DGV3) ---
                // 1. Create a map of original items (DGV3) for quick lookup: ProductID+WID -> Qty
                var originalItems = dataGridView3.Rows.Cast<DataGridViewRow>()
                    .Where(r3 => !r3.IsNewRow && r3.Cells[13].Value != null && r3.Cells[14].Value != null)
                    .ToDictionary(
                        r3 => $"{r3.Cells[13].Value}_{r3.Cells[14].Value}", // Key: PID_WID
                        r3 => Convert.ToInt32(r3.Cells[6].Value)           // Value: Qty
                    );

                // 2. Iterate through current items (DGV1)
                foreach (DataGridViewRow row1 in DataGridView1.Rows)
                {
                    if (row1.IsNewRow || row1.Cells[13].Value == null || row1.Cells[14].Value == null) continue;

                    int productId1 = Convert.ToInt32(row1.Cells[13].Value);
                    int wid1 = Convert.ToInt32(row1.Cells[14].Value);
                    int currentQty = Convert.ToInt32(row1.Cells[6].Value);
                    string itemKey = $"{productId1}_{wid1}";

                    int originalQty = 0;
                    if (originalItems.ContainsKey(itemKey))
                    {
                        originalQty = originalItems[itemKey];
                        originalItems.Remove(itemKey); // Remove from map once processed
                    }

                    int qtyDifference = currentQty - originalQty;

                    if (qtyDifference != 0) // Only update if quantity changed
                    {
                        // If qtyDifference > 0, subtract from stock. If < 0, add to stock.
                        string stockUpdateQuery = "UPDATE Temp_Stock SET Qty = Qty - @QtyDiff WHERE ProductID = @PID AND WID = @WID";
                        var stockUpdateParams = new[] {
                            DataAccessLayer.CreateParameter("@QtyDiff", SqlDbType.Int, qtyDifference), // Use the difference directly
                            DataAccessLayer.CreateParameter("@PID", SqlDbType.Int, productId1),
                            DataAccessLayer.CreateParameter("@WID", SqlDbType.Int, wid1)
                        };
                        DataAccessLayer.ExecuteNonQuery(stockUpdateQuery, CommandType.Text, stockUpdateParams);
                    }
                }

                // 3. Process items that were in DGV3 but are NOT in DGV1 (removed items) - Add their quantity back to stock
                foreach (var removedItem in originalItems)
                {
                    string[] keyParts = removedItem.Key.Split('_');
                    int removedProductId = int.Parse(keyParts[0]);
                    int removedWid = int.Parse(keyParts[1]);
                    int removedQty = removedItem.Value;

                    string stockAddBackQuery = "UPDATE Temp_Stock SET Qty = Qty + @Qty WHERE ProductID = @PID AND WID = @WID";
                    var stockAddParams = new[] {
                        DataAccessLayer.CreateParameter("@Qty", SqlDbType.Int, removedQty),
                        DataAccessLayer.CreateParameter("@PID", SqlDbType.Int, removedProductId),
                        DataAccessLayer.CreateParameter("@WID", SqlDbType.Int, removedWid)
                    };
                    DataAccessLayer.ExecuteNonQuery(stockAddBackQuery, CommandType.Text, stockAddParams);
                }


                // --- Update Payment_2 if balance exists (txtT_ID_1 is the original TC_ID if loaded) ---
                if (!string.IsNullOrWhiteSpace(txtT_ID_1.Text)) // Check if updating an invoice that had a balance record
                {
                    // Update the existing Payment_2 record linked via TC_ID
                    decimal paymentDue = Convert.ToDecimal(txtPaymentDue.Text);
                    string payUpdateQuery = @"UPDATE Payment_2 SET TransactionID = @d2, Date = @d3, PaymentMode = @d4, CustomerID = @d5,
                                            Amount = @d6, Remarks = @d7, Check_ID = @d8, Check_Date = @d9, SalesMan_ID = @d10,
                                            SalesMan_Name = @d11, SalesMan_Comession = @d12, SalesMan_ID_2 = @d13
                                            WHERE TC_ID = @d1";
                    var payUpdateParams = new[] {
                        DataAccessLayer.CreateParameter("@d1", SqlDbType.Int, Convert.ToInt32(txtT_ID_1.Text)), // Original TC_ID
                        DataAccessLayer.CreateParameter("@d2", SqlDbType.NVarChar, txtTransactionNo_1.Text), // Original Transaction No
                        DataAccessLayer.CreateParameter("@d3", SqlDbType.Date, dtpPaymentDate.Value.Date),
                        DataAccessLayer.CreateParameter("@d4", SqlDbType.NVarChar, cmbPaymentMode.Text),
                        DataAccessLayer.CreateParameter("@d5", SqlDbType.NVarChar, txtCustomerID.Text),
                        DataAccessLayer.CreateParameter("@d6", SqlDbType.Decimal, -paymentDue), // Update with new balance
                        DataAccessLayer.CreateParameter("@d7", SqlDbType.NVarChar, txtRemarks.Text),
                        DataAccessLayer.CreateParameter("@d8", SqlDbType.NVarChar, txtCheck.Text),
                        DataAccessLayer.CreateParameter("@d9", SqlDbType.Date, dtpPaymentDate.Value.Date), // Check Date
                        DataAccessLayer.CreateParameter("@d10", SqlDbType.Int, Convert.ToInt32(txtSM_ID.Text)),
                        DataAccessLayer.CreateParameter("@d11", SqlDbType.NVarChar, txtSalesman.Text),
                        DataAccessLayer.CreateParameter("@d12", SqlDbType.Decimal, Convert.ToDecimal(txtCommissionPer.Text)),
                        DataAccessLayer.CreateParameter("@d13", SqlDbType.NVarChar, txtSalesmanID.Text)
                     };
                    DataAccessLayer.ExecuteNonQuery(payUpdateQuery, CommandType.Text, payUpdateParams);
                    // Also need to ensure InvoiceInfo's TC_ID is correctly linked or nulled if balance becomes 0
                    string invTcIdUpdateQuery = "UPDATE InvoiceInfo SET TC_ID = @TCID WHERE Inv_ID = @InvID";
                    var invTcIdParams = new[]{
                         DataAccessLayer.CreateParameter("@TCID", SqlDbType.Int, (paymentDue == 0) ? (object)DBNull.Value : Convert.ToInt32(txtT_ID_1.Text)),
                         DataAccessLayer.CreateParameter("@InvID", SqlDbType.Int, Convert.ToInt32(txtID.Text))
                     };
                    DataAccessLayer.ExecuteNonQuery(invTcIdUpdateQuery, CommandType.Text, invTcIdParams);

                }
                // Potential case: Invoice now has a balance, but didn't before (Need to Insert into Payment_2 and link TC_ID in InvoiceInfo) - Add if needed


                // --- Update Ledger ---
                string ledgerName = "نقدا"; // Default
                if (cmbPaymentMode.SelectedIndex == 1) ledgerName = "شيك رقم " + txtCheck.Text.Trim();
                else if (cmbPaymentMode.SelectedIndex == 4) ledgerName = "اجل";

                LedgerUpdate(dtpPaymentDate.Value.Date, ledgerName,
                            Math.Abs(Convert.ToDecimal(txtGrandTotal.Text)),
                            Math.Abs(Convert.ToDecimal(txtTotalPayment.Text)),
                            txtCustomerID.Text, txtInvoiceNo.Text, "فاتورة مبيعات",
                            Math.Abs(Convert.ToDecimal(total_sale.Text))); // Uses DAL now

                // --- Log and Finalize ---
                string st = $"updated the bill (Products) having invoice no. '{txtInvoiceNo.Text}'";
                LogFunc(lblUser.Text, st); // Uses DAL now

                btnUpdate.Enabled = false;
                MessageBox.Show("تم التعديل بنجاح", "السجلات", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Reset();

            }
            catch (SqlException dbEx)
            {
                MessageBox.Show($"Database error during update: {dbEx.Message}", "خطأ قاعدة البيانات", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FormatException formatEx)
            {
                MessageBox.Show($"Data format error during update: {formatEx.Message}", "خطأ تنسيق البيانات", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred during update: {ex.Message}", "خطأ غير متوقع", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnGetData_Click(object sender, EventArgs e)
        {
            SalesInvoiceScreen frmSalesInvoiceRecord = new SalesInvoiceScreen();
            frmSalesInvoiceRecord.lblSet.Text = "Sales Invoice";
            frmSalesInvoiceRecord.Reset();
            frmSalesInvoiceRecord.ShowDialog();

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Reset();

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            Timer1.Enabled = false;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            /*            PrintDocument();  // Replace FileSystem.Print with appropriate printing logic
            */
        }

        private void btnAdd1_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("الرجاء اضافة اصناف للشبكة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(cmbPaymentMode.Text))
                {
                    MessageBox.Show("الرجاء اختيار طريقة دفع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbPaymentMode.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txtPayment.Text))
                {
                    MessageBox.Show("الرجاء كتابة المبلغ المدفوع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPayment.Focus();
                    return;
                }

                DataGridView2.Rows.Add(cmbPaymentMode.Text, txtPayment.Text, dtpPaymentDate.Value.Date);
                double j = TotalPayment();
                txtTotalPayment.Text = Math.Round(j, 2).ToString();
                Compute1();
                Clear1();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);  // Replace Interaction.MsgBox with MessageBox
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("هل أنت متأكد من أنك تريد حذف سجل الفاتورة؟", "تاكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    DeleteRecord();

                    Reset();  // Replace FileSystem.Reset with appropriate form reset method
                    cmbPaymentMode.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteRecord()
        {
            try
            {
                int rowsAffected = 0;

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {

                    con.Open();

                    string queryCheck = "SELECT Inv_ID FROM InvoiceInfo INNER JOIN SalesReturn ON SalesReturn.SalesID = InvoiceInfo.Inv_ID WHERE Inv_ID = @d1";
                    using (SqlCommand cmdCheck = new SqlCommand(queryCheck, con))
                    {
                        cmdCheck.Parameters.AddWithValue("@d1", int.Parse(txtID.Text));
                        using (SqlDataReader rdr = cmdCheck.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                MessageBox.Show("غير قادر على الحذف .. مستخدمة مسبقًا في إرجاع المبيعات", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }

                    string queryDelete = "DELETE FROM InvoiceInfo WHERE Inv_ID = @d1";
                    using (SqlCommand cmdDelete = new SqlCommand(queryDelete, con))
                    {
                        cmdDelete.Parameters.AddWithValue("@d1", int.Parse(txtID.Text));
                        rowsAffected = cmdDelete.ExecuteNonQuery();
                    }

                    if (rowsAffected > 0)
                    {
                        foreach (DataGridViewRow row in DataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                string updateStockQuery = "UPDATE Temp_Stock SET qty = qty + @qty WHERE ProductID = @d1";
                                using (SqlCommand cmdUpdateStock = new SqlCommand(updateStockQuery, con))
                                {
                                    cmdUpdateStock.Parameters.AddWithValue("@qty", decimal.Parse(row.Cells[6].Value.ToString()));
                                    cmdUpdateStock.Parameters.AddWithValue("@d1", int.Parse(row.Cells[13].Value.ToString()));
                                    cmdUpdateStock.Parameters.AddWithValue("@d2", row.Cells[2].Value.ToString());
                                    cmdUpdateStock.ExecuteNonQuery();
                                }
                            }
                        }


                        using (SqlCommand cmd = new SqlCommand("DELETE FROM Salesman_Commission WHERE InvoiceID = @ID", con))
                        {
                            cmd.Parameters.AddWithValue("@ID", txtID.Text);

                            cmd.ExecuteNonQuery();

                        }

                        LedgerDelete(txtInvoiceNo.Text, "فاتورة مبيعات");
                        LedgerDelete(txtInvoiceNo.Text, "دفعة فورية");

                        string logMessage = $"deleted the bill (Products) having invoice no. '{txtInvoiceNo.Text}'";
                        LogFunc(lblUser.Text, logMessage);

                        MessageBox.Show("تم الحذف بنجاح", "سجل", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FileSystem.Reset();
                    }
                    else
                    {
                        MessageBox.Show("لايوجد سجل", "عذرا", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FileSystem.Reset();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRemove1_Click(object sender, EventArgs e)
        {

        }

        private void txtPayment_KeyPress(object sender, KeyPressEventArgs e)
        {

        }



        private void DataGridView2_MouseClick(object sender, MouseEventArgs e)
        {
            if (DataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow row = DataGridView2.SelectedRows[0];
                cmbPaymentMode.Text = row.Cells[0].Value.ToString();
                txtPayment.Text = row.Cells[1].Value.ToString();
                dtpPaymentDate.Value = Convert.ToDateTime(row.Cells[2].Value);

                btnRemove1.Enabled = true;
                btnListUpdate1.Enabled = true;
                btnAdd1.Enabled = false;
            }
        }

        private void btnListReset1_Click(object sender, EventArgs e)
        {
            Clear1();
        }

        private void btnListReset_Click(object sender, EventArgs e)
        {
            Clear();  // Assuming you have a Clear method for other purposes
        }

        private void DataGridView2_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            string strRowNumber = (e.RowIndex + 1).ToString();
            SizeF size = e.Graphics.MeasureString(strRowNumber, this.Font);
            if (DataGridView2.RowHeadersWidth < Convert.ToInt32(size.Width + 20))
            {
                DataGridView2.RowHeadersWidth = Convert.ToInt32(size.Width + 20);
            }
            Brush b = SystemBrushes.ControlText;
            e.Graphics.DrawString(strRowNumber, this.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + (e.RowBounds.Height - size.Height) / 2);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string cs = DataAccessLayer.Con();
            try
            {
                POSSAVE();

                // Final confirmation message
                MessageBox.Show("تم الحفظ بنجاح", "الفاتورة", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
                btnPrint.Enabled = true;
                txtCustomerType.Text = "Regular";
                Print();
                Reset();
            }
            catch (Exception ex)
            {

            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in DataGridView1.SelectedRows)
                {
                    DataGridView1.Rows.Remove(row);
                }

                double k = 0;
                k = GrandTotal();
                k = Math.Round(k, 2);
                txtGrandTotal.Text = k.ToString();
                Compute();
                Compute1();
                Clear();
                txtPayment.Text = txtGrandTotal.Text.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnListUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(txtProductCode.Text))
                {
                    MessageBox.Show("الرجاء ادخال المنتج", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProductCode.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtBarcode.Text))
                {
                    MessageBox.Show("الرجاء إدراج الباركود للصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBarcode.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtSellingPrice.Text))
                {
                    MessageBox.Show("الرجاء كتابة السعر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSellingPrice.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDiscountPer.Text))
                {
                    MessageBox.Show("الرجاء تحديد الخصم %", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDiscountPer.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtVAT.Text))
                {
                    MessageBox.Show("الرجاء تحديد الضريبة %", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtVAT.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtQty.Text))
                {
                    MessageBox.Show("الرجاء تحديد الكمية", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQty.Focus();
                    return;
                }
                if (Convert.ToDouble(txtQty.Text) <= 0)

                {
                    MessageBox.Show("الكمية يجب ان تكون اكبر من الصفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQty.Focus();
                    return;
                }

                /*if (Convert.ToDecimal(txtQty.Text) <= 0)
                {
                    MessageBox.Show("الكمية يجب ان تكون اكبر من الصفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQty.Focus();
                    return;
                }*/

                // Remove selected rows before adding the new one
                foreach (DataGridViewRow row in DataGridView1.SelectedRows)
                {
                    DataGridView1.Rows.Remove(row);
                }

                DataGridView1.Rows.Add(
                  txtProductCode.Text, txtProductName.Text, txtBarcode.Text,
                  Convert.ToDouble(txtCostPrice.Text), Convert.ToDouble(txtSellingPrice.Text),
                  Convert.ToDouble(txtMargin.Text), Convert.ToDouble(txtQty.Text),
                  Convert.ToDouble(txtAmount.Text), Convert.ToDouble(txtDiscountPer.Text),
                  Convert.ToDouble(txtDiscountAmount.Text), Convert.ToDouble(txtVAT.Text),
                  Convert.ToDouble(txtVATAmount.Text), Convert.ToDouble(txtTotalAmount.Text),
                  Convert.ToDouble(txtProductID.Text), WID.Text
              );

                // Calculate Grand Total and update the textbox
                double grandTotal = Math.Round(GrandTotal(), 2);
                cmbPaymentMode.SelectedIndex = 0;
                txtGrandTotal.Text = grandTotal.ToString();
                txtPayment.Text = txtGrandTotal.Text.ToString();
                // Set payment amount to the total amount
                // Recompute other values if necessary
                Compute1();

                // Clear input fields
                Clear();
                txtProductName.Enabled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnListReset_Click_1(object sender, EventArgs e)
        {
            Clear();
        }

        private void txtPayment_TextChanged(object sender, EventArgs e)
        {
            /*
                        if (txtCustomerID.Text == "C-0001")
                        {
                            cmbPaymentMode.SelectedIndex = 0;
                            txtPayment.ReadOnly = true;
                            txtPayment.Text = Val(txtGrandTotal.Text);
                            txtTotalPayment.Text = Val(txtGrandTotal.Text);
                            Compute1();
                        }
                        else
                        {
                            txtPayment.ReadOnly = false;

                        }*/

            if (txtCustomerID.Text == "C-0001")
            {
                /*                cmbPaymentMode.SelectedIndex = 0;
                 *                
                *//*                txtPayment.Text = (txtGrandTotal.Text.ToString());
                                txtTotalPayment.Text = txtGrandTotal.Text.ToString();*/
                /*                txtPayment.ReadOnly = true;*/
                /*                txtPayment.Text = txtGrandTotal.Text;
                *//*                txtTotalPayment.Text = txtGrandTotal.Text;
                */
                Compute1();
            }
            else
            {
                /*                txtPayment.ReadOnly = false;
                */
            }

            /*
                        if (cmbPaymentMode.SelectedIndex == 4)
                        {
                            txtPayment.ReadOnly = true;

                        }*/

        }
        private void txtCheck_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnAdd1_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Check if there are rows in DataGridView1
                if (DataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("الرجاء اضافة اصناف للشبكة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check if a payment mode is selected
                if (string.IsNullOrWhiteSpace(cmbPaymentMode.Text))
                {
                    MessageBox.Show("الرجاء اختيار طريقة دفع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbPaymentMode.Focus();
                    return;
                }

                // Check if payment amount is provided
                if (string.IsNullOrWhiteSpace(txtPayment.Text) || !decimal.TryParse(txtPayment.Text, out _))
                {
                    MessageBox.Show("الرجاء كتابة المبلغ المدفوع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPayment.Focus();
                    return;
                }
                // Use Regular Expression to allow only numbers and decimals
                if (!Regex.IsMatch(txtPayment.Text, @"^\d+(\.\d{1,2})?$"))
                {
                    MessageBox.Show("المبلغ المدفوع يجب أن يكون رقمًا صالحًا", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPayment.Focus();
                    return;
                }
                // Add payment information to DataGridView2
                DataGridView2.Rows.Add(cmbPaymentMode.Text, txtPayment.Text, dtpPaymentDate.Value.Date);

                // Calculate and round the total payment
                double totalPayment = TotalPayment();
                totalPayment = Math.Round(totalPayment, 2);
                txtTotalPayment.Text = totalPayment.ToString();

                // Perform additional computations
                Compute1();

                // Clear any additional data or reset fields if needed
                Clear1();
            }
            catch (Exception ex)
            {
                // Show detailed error message
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnListReset1_Click_1(object sender, EventArgs e)
        {
            Clear1();
        }

        private void btnListUpdate1_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("الرجاء اضافة اصناف ", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cmbPaymentMode.Text == "")
                {
                    MessageBox.Show("الرجاء اختيار طريقة دفع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbPaymentMode.Focus();
                    return;
                }
                if (txtPayment.Text == "")
                {
                    MessageBox.Show("الرجاء كتابة المبلغ المدفوع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPayment.Focus();
                    return;
                }
                foreach (DataGridViewRow row in DataGridView2.SelectedRows)
                    DataGridView2.Rows.Remove(row);
                DataGridView2.Rows.Add(cmbPaymentMode.Text, Convert.ToDecimal(txtPayment.Text), dtpPaymentDate.Value.Date);
                double j = 0d;
                j = TotalPayment();
                j = Math.Round(j, 2);
                txtTotalPayment.Text = j.ToString();
                Compute1();
                Clear1();
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
            }
        }

        private void btnRemove1_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Remove selected rows from DataGridView2
                foreach (DataGridViewRow row in DataGridView2.SelectedRows)
                {
                    // Ensure the row is not a new row before attempting to remove
                    if (!row.IsNewRow)
                    {
                        DataGridView2.Rows.Remove(row);
                    }
                }

                // Calculate and round the total payment
                double totalPayment = TotalPayment();
                totalPayment = Math.Round(totalPayment, 2);
                txtTotalPayment.Text = totalPayment.ToString();

                // Perform additional computations
                Compute1();
                Compute();

                // Clear any additional data or reset fields if needed
                Clear1();
            }
            catch (Exception ex)
            {
                // Show detailed error message
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void txtTotalPayment_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtQty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnAdd_Click(this, EventArgs.Empty);
            }

        }

        private void txtBarcode_TextChanged(object sender, EventArgs e)
        {

            try
            {
                // Make the DataGridView visible
                dgw.Visible = true;

                // Establish the SQL connection
                con.Open();

                // Define the SQL query with parameterized barcode search
                SqlCommand cmd = new SqlCommand(@"SELECT PID, RTRIM(Product.ProductCode), RTRIM(ProductName), 
                                          RTRIM(Temp_Stock.Barcode), CostPrice, SellingPrice, Discount, VAT, Qty 
                                          FROM Temp_Stock, Product 
                                          WHERE Product.PID = Temp_Stock.ProductID AND Qty > 0 
                                          AND Temp_Stock.Barcode LIKE @barcode 
                                          ORDER BY ProductCode", con);

                // Add the parameter for the barcode search
                cmd.Parameters.AddWithValue("@barcode", "%" + txtBarcode.Text + "%");

                // Execute the query and read the results
                SqlDataReader rdr = cmd.ExecuteReader();

                // Clear existing rows in the DataGridView
                dgw.Rows.Clear();

                // Populate the DataGridView with the results from the query
                while (rdr.Read())
                {
                    dgw.Rows.Add(rdr["PID"], rdr["ProductCode"], rdr["ProductName"], rdr["Barcode"],
                                 rdr["CostPrice"], rdr["SellingPrice"], rdr["Discount"], rdr["VAT"], rdr["Qty"]);
                }
            }
            catch (Exception ex)
            {
                // Display error message if an exception occurs
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Ensure the SqlDataReader and SqlConnection are closed properly
                if (con != null && con.State == ConnectionState.Open) con.Close();
            }

            // Update the payment text and other controls based on the selected payment mode
            if (cmbPaymentMode.SelectedIndex == 2 || cmbPaymentMode.SelectedIndex == 3 ||
                cmbPaymentMode.SelectedIndex == 4 || cmbPaymentMode.SelectedIndex == 5 ||
                cmbPaymentMode.SelectedIndex == 6 || cmbPaymentMode.SelectedIndex == 7 ||
                cmbPaymentMode.SelectedIndex == 8 || cmbPaymentMode.SelectedIndex == 9)
            {
                /*                txtPayment.ReadOnly = true;
                */
                txtPayment.Text = "0";
                txtQty.Focus();
            }
            else
            {
                /*                txtPayment.ReadOnly = false;
                */
                txtPayment.Text = Convert.ToString(Convert.ToDecimal(txtGrandTotal.Text));
            }
        }

        private void cmbPaymentMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPaymentMode.SelectedIndex == 2 || cmbPaymentMode.SelectedIndex == 3 ||
                    cmbPaymentMode.SelectedIndex == 4 || cmbPaymentMode.SelectedIndex == 5 ||
                    cmbPaymentMode.SelectedIndex == 6 || cmbPaymentMode.SelectedIndex == 7 ||
                    cmbPaymentMode.SelectedIndex == 8 || cmbPaymentMode.SelectedIndex == 9)
            {
                txtPayment.Text = "0";
                /*                txtPayment.ReadOnly = true;
                */
                if (txtCustomerID.Text == "C-0001")
                {
                    txtPayment.Text = Convert.ToString(Convert.ToDecimal(txtGrandTotal.Text));
                    /*                   
                    */                    /*                    txtPayment.ReadOnly = true;
                                        */
                }
            }
            else
            {
                /*                txtPayment.ReadOnly = false;
                */
                /*                txtPayment.Text = txtGrandTotal.Text;
                */
            }

            if (cmbPaymentMode.SelectedIndex == 4)
            {
                txtPayment.Text = "0.00";

            }

        }

        private void txtGrandTotal_TextChanged(object sender, EventArgs e)
        {

        }

        private void ToolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm:ss tt");
        }

        private void Timer1_Tick_1(object sender, EventArgs e)
        {

            Cursor = Cursors.Default;
            Timer1.Enabled = false;
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            txtCustomerType.Text = "Non Regular";
            Print();
            Reset();
        }


        private void DataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void DataGridView1_ControlAdded(object sender, ControlEventArgs e)
        {
            txtPayment.Text = (txtGrandTotal.Text);

            // List of payment mode indices that require a specific condition
            int[] specialPaymentModes = { 2, 3, 4, 5, 6, 7, 8, 9 };

            if (specialPaymentModes.Contains(cmbPaymentMode.SelectedIndex))
            {
                txtPayment.Text = "0";
                /*                txtPayment.ReadOnly = true;
                */
            }
            else
            {
                /*                txtPayment.ReadOnly = false;
                */
                txtPayment.Text = (txtGrandTotal.Text);
            }
        }

        private decimal Val(string text)
        {
            // Convert the text to a decimal, handle cases where conversion might fail
            if (decimal.TryParse(text, out decimal result))
            {
                return result;
            }
            return 0;
        }

        private void txtSalesman_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtAmount_TextChanged(object sender, EventArgs e)
        {

        }

        private void DataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            if (DataGridView1.Rows.Count > 0)
            {


                btnAdd.Enabled = false;

                DataGridViewRow row = DataGridView1.SelectedRows[0];

                txtProductCode.Text = row.Cells[0].Value.ToString();
                txtProductName.Text = row.Cells[1].Value.ToString();
                txtBarcode.Text = row.Cells[2].Value.ToString();
                txtCostPrice.Text = row.Cells[3].Value.ToString();
                txtSellingPrice.Text = row.Cells[4].Value.ToString();
                txtMargin.Text = row.Cells[5].Value.ToString();
                txtQty.Text = row.Cells[6].Value.ToString();
                txtAmount.Text = row.Cells[7].Value.ToString();
                txtDiscountPer.Text = row.Cells[8].Value.ToString();
                txtDiscountAmount.Text = row.Cells[9].Value.ToString();
                txtVAT.Text = row.Cells[10].Value.ToString();
                txtVATAmount.Text = row.Cells[11].Value.ToString();
                txtTotalAmount.Text = row.Cells[12].Value.ToString();
                txtProductID.Text = row.Cells[13].Value.ToString();


                txtProductName.Enabled = false;
                txtQty.Focus();
                dgw.Visible = false;
                btnRemove.Enabled = true;
                btnListUpdate.Enabled = true;
                btnListUpdate.Focus();
            }
            txtPayment.Text = (txtGrandTotal.Text.ToString());

        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void DataGridView1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void GroupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void btnPrint_Click_1(object sender, EventArgs e)
        {
            Print();
        }
        public void Print()
        {
            try
            {
                if (txtCustomerType.Text == "Regular")
                {
                    Cursor = Cursors.WaitCursor;
                    Timer1.Enabled = true;
                    rptInvoice rpt = new rptInvoice(); // The report you created.
                    SqlConnection myConnection = new SqlConnection(DataAccessLayer.Con());
                    SqlCommand MyCommand = new SqlCommand();
                    SqlCommand MyCommand1 = new SqlCommand();
                    SqlDataAdapter myDA = new SqlDataAdapter();
                    SqlDataAdapter myDA1 = new SqlDataAdapter();
                    DataSet myDS = new DataSet(); // The DataSet you created.
                    MyCommand.Connection = myConnection;
                    MyCommand1.Connection = myConnection;

                    MyCommand.CommandText = "Select Customer.ID, Customer.Name, Customer.Gender, Customer.Address, Customer.City, Customer.State, Customer.ZipCode, Customer.ContactNo, Customer.EmailID, InvoiceInfo.Remarks, Customer.Photo, InvoiceInfo.Inv_ID, InvoiceInfo.InvoiceNo, InvoiceInfo.InvoiceDate, InvoiceInfo.CustomerID, InvoiceInfo.GrandTotal, InvoiceInfo.TotalPaid, InvoiceInfo.Balance, Invoice_Product.IPo_ID, Invoice_Product.InvoiceID, Invoice_Product.ProductID, Invoice_Product.CostPrice, Invoice_Product.SellingPrice, Invoice_Product.Margin, Invoice_Product.Qty, Invoice_Product.Amount, Invoice_Product.DiscountPer, Invoice_Product.Discount, Invoice_Product.VATPer, Invoice_Product.VAT, Invoice_Product.TotalAmount, Invoice_Product.Barcode, Product.PID, Product.ProductCode, Product.ProductName ,InvoiceInfo.total_sale FROM Customer INNER JOIN InvoiceInfo On Customer.ID = InvoiceInfo.CustomerID INNER JOIN Invoice_Product On InvoiceInfo.Inv_ID = Invoice_Product.InvoiceID INNER JOIN Product On Invoice_Product.ProductID = Product.PID where InvoiceInfo.Invoiceno=@d1";
                    MyCommand.Parameters.AddWithValue("@d1", txtInvoiceNo.Text);

                    MyCommand1.CommandText = "Select * from Company";
                    MyCommand.CommandType = CommandType.Text;
                    MyCommand1.CommandType = CommandType.Text;

                    myDA.SelectCommand = MyCommand;
                    myDA1.SelectCommand = MyCommand1;

                    myDA.Fill(myDS, "InvoiceInfo");
                    myDA.Fill(myDS, "Invoice_Product");
                    myDA.Fill(myDS, "Customer");
                    myDA.Fill(myDS, "Product");
                    myDA1.Fill(myDS, "Company");

                    rpt.SetDataSource(myDS);
                    rpt.SetParameterValue("p1", txtCustomerID.Text);
                    rpt.SetParameterValue("p2", DateTime.Today);
                    frmReport frmReport = new frmReport();

                    frmReport.crystalReportViewer1.ReportSource = rpt;
                    frmReport.ShowDialog();
                }
                else if (txtCustomerType.Text == "Non Regular")
                {
                    Cursor = Cursors.WaitCursor;
                    Timer1.Enabled = true;
                    rptInvoice3 rpt = new rptInvoice3(); // The report you created.
                    SqlConnection myConnection = new SqlConnection(DataAccessLayer.Con());
                    SqlCommand MyCommand = new SqlCommand();
                    SqlCommand MyCommand1 = new SqlCommand();
                    SqlDataAdapter myDA = new SqlDataAdapter();
                    SqlDataAdapter myDA1 = new SqlDataAdapter();
                    DataSet myDS = new DataSet(); // The DataSet you created.
                    MyCommand.Connection = myConnection;
                    MyCommand1.Connection = myConnection;

                    MyCommand.CommandText = "Select Customer.ID, Customer.Name, Customer.Gender, Customer.Address, Customer.City, Customer.State, Customer.ZipCode, Customer.ContactNo, Customer.EmailID, InvoiceInfo.Remarks, Customer.Photo, InvoiceInfo.Inv_ID, InvoiceInfo.InvoiceNo, InvoiceInfo.InvoiceDate, InvoiceInfo.CustomerID, InvoiceInfo.GrandTotal, InvoiceInfo.TotalPaid, InvoiceInfo.Balance, Invoice_Product.IPo_ID, Invoice_Product.InvoiceID, Invoice_Product.ProductID, Invoice_Product.CostPrice, Invoice_Product.SellingPrice, Invoice_Product.Margin, Invoice_Product.Qty, Invoice_Product.Amount, Invoice_Product.DiscountPer, Invoice_Product.Discount, Invoice_Product.VATPer, Invoice_Product.VAT, Invoice_Product.TotalAmount, Invoice_Product.Barcode, Product.PID, Product.ProductCode, Product.ProductName,InvoiceInfo.total_sale FROM Customer INNER JOIN InvoiceInfo On Customer.ID = InvoiceInfo.CustomerID INNER JOIN Invoice_Product On InvoiceInfo.Inv_ID = Invoice_Product.InvoiceID INNER JOIN Product On Invoice_Product.ProductID = Product.PID where InvoiceInfo.Invoiceno=@d1";
                    MyCommand.Parameters.AddWithValue("@d1", txtInvoiceNo.Text);

                    MyCommand1.CommandText = "Select * from Company";
                    MyCommand.CommandType = CommandType.Text;
                    MyCommand1.CommandType = CommandType.Text;

                    myDA.SelectCommand = MyCommand;
                    myDA1.SelectCommand = MyCommand1;

                    myDA.Fill(myDS, "InvoiceInfo");
                    myDA.Fill(myDS, "Invoice_Product");
                    myDA.Fill(myDS, "Customer");
                    myDA.Fill(myDS, "Product");
                    myDA1.Fill(myDS, "Company");

                    rpt.SetDataSource(myDS);
                    rpt.SetParameterValue("p1", txtCustomerID.Text);
                    rpt.SetParameterValue("p2", DateTime.Today);
                    frmReport frmReport = new frmReport();

                    frmReport.crystalReportViewer1.ReportSource = rpt;
                    frmReport.ShowDialog();
                }
                else if (txtCustomerType.Text == "Regular Sale")
                {
                    Cursor = Cursors.WaitCursor;
                    Timer1.Enabled = true;
                    rptInvoice4 rpt = new rptInvoice4(); // The report you created.
                    SqlConnection myConnection = new SqlConnection(DataAccessLayer.Con());
                    SqlCommand MyCommand = new SqlCommand();
                    SqlCommand MyCommand1 = new SqlCommand();
                    SqlDataAdapter myDA = new SqlDataAdapter();
                    SqlDataAdapter myDA1 = new SqlDataAdapter();
                    DataSet myDS = new DataSet(); // The DataSet you created.
                    MyCommand.Connection = myConnection;
                    MyCommand1.Connection = myConnection;

                    MyCommand.CommandText = "Select Customer.ID, Customer.Name, Customer.Gender, Customer.Address, Customer.City, Customer.State, Customer.ZipCode, Customer.ContactNo, Customer.EmailID, InvoiceInfo.Remarks, Customer.Photo, InvoiceInfo.Inv_ID, InvoiceInfo.InvoiceNo, InvoiceInfo.InvoiceDate, InvoiceInfo.CustomerID, InvoiceInfo.GrandTotal, InvoiceInfo.TotalPaid, InvoiceInfo.Balance, Invoice_Product.IPo_ID, Invoice_Product.InvoiceID, Invoice_Product.ProductID, Invoice_Product.CostPrice, Invoice_Product.SellingPrice, Invoice_Product.Margin, Invoice_Product.Qty, Invoice_Product.Amount, Invoice_Product.DiscountPer, Invoice_Product.Discount, Invoice_Product.VATPer, Invoice_Product.VAT, Invoice_Product.TotalAmount, Invoice_Product.Barcode, Product.PID, Product.ProductCode, Product.ProductName,InvoiceInfo.total_sale FROM Customer INNER JOIN InvoiceInfo On Customer.ID = InvoiceInfo.CustomerID INNER JOIN Invoice_Product On InvoiceInfo.Inv_ID = Invoice_Product.InvoiceID INNER JOIN Product On Invoice_Product.ProductID = Product.PID where InvoiceInfo.Invoiceno=@d1";
                    MyCommand.Parameters.AddWithValue("@d1", txtInvoiceNo.Text);

                    MyCommand1.CommandText = "Select * from Company";
                    MyCommand.CommandType = CommandType.Text;
                    MyCommand1.CommandType = CommandType.Text;

                    myDA.SelectCommand = MyCommand;
                    myDA1.SelectCommand = MyCommand1;

                    myDA.Fill(myDS, "InvoiceInfo");
                    myDA.Fill(myDS, "Invoice_Product");
                    myDA.Fill(myDS, "Customer");
                    myDA.Fill(myDS, "Product");
                    myDA1.Fill(myDS, "Company");

                    rpt.SetDataSource(myDS);
                    rpt.SetParameterValue("p1", txtCustomerID.Text);
                    rpt.SetParameterValue("p2", DateTime.Today);
                    rpt.SetParameterValue("Sale", total_sale.Text);
                    frmReport frmReport = new frmReport();

                    frmReport.crystalReportViewer1.ReportSource = rpt;
                    frmReport.ShowDialog();
                }
                else if (txtCustomerType.Text == "Non Regular Sale")
                {
                    Cursor = Cursors.WaitCursor;
                    Timer1.Enabled = true;
                    rptInvoice5 rpt = new rptInvoice5(); // The report you created.
                    SqlConnection myConnection = new SqlConnection(DataAccessLayer.Con());
                    SqlCommand MyCommand = new SqlCommand();
                    SqlCommand MyCommand1 = new SqlCommand();
                    SqlDataAdapter myDA = new SqlDataAdapter();
                    SqlDataAdapter myDA1 = new SqlDataAdapter();
                    DataSet myDS = new DataSet(); // The DataSet you created.
                    MyCommand.Connection = myConnection;
                    MyCommand1.Connection = myConnection;

                    MyCommand.CommandText = "Select Customer.ID, Customer.Name, Customer.Gender, Customer.Address, Customer.City, Customer.State, Customer.ZipCode, Customer.ContactNo, Customer.EmailID, InvoiceInfo.Remarks, Customer.Photo, InvoiceInfo.Inv_ID, InvoiceInfo.InvoiceNo, InvoiceInfo.InvoiceDate, InvoiceInfo.CustomerID, InvoiceInfo.GrandTotal, InvoiceInfo.TotalPaid, InvoiceInfo.Balance, Invoice_Product.IPo_ID, Invoice_Product.InvoiceID, Invoice_Product.ProductID, Invoice_Product.CostPrice, Invoice_Product.SellingPrice, Invoice_Product.Margin, Invoice_Product.Qty, Invoice_Product.Amount, Invoice_Product.DiscountPer, Invoice_Product.Discount, Invoice_Product.VATPer, Invoice_Product.VAT, Invoice_Product.TotalAmount, Invoice_Product.Barcode, Product.PID, Product.ProductCode, Product.ProductName,InvoiceInfo.total_sale FROM Customer INNER JOIN InvoiceInfo On Customer.ID = InvoiceInfo.CustomerID INNER JOIN Invoice_Product On InvoiceInfo.Inv_ID = Invoice_Product.InvoiceID INNER JOIN Product On Invoice_Product.ProductID = Product.PID where InvoiceInfo.Invoiceno=@d1";
                    MyCommand.Parameters.AddWithValue("@d1", txtInvoiceNo.Text);

                    MyCommand1.CommandText = "Select * from Company";
                    MyCommand.CommandType = CommandType.Text;
                    MyCommand1.CommandType = CommandType.Text;

                    myDA.SelectCommand = MyCommand;
                    myDA1.SelectCommand = MyCommand1;

                    myDA.Fill(myDS, "InvoiceInfo");
                    myDA.Fill(myDS, "Invoice_Product");
                    myDA.Fill(myDS, "Customer");
                    myDA.Fill(myDS, "Product");
                    myDA1.Fill(myDS, "Company");

                    rpt.SetDataSource(myDS);
                    rpt.SetParameterValue("p1", txtCustomerID.Text);
                    rpt.SetParameterValue("p2", DateTime.Today);
                    rpt.SetParameterValue("Sale", total_sale.Text);
                    frmReport frmReport = new frmReport();

                    frmReport.crystalReportViewer1.ReportSource = rpt;
                    frmReport.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtProductCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSalesmanID_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCustomerID_TextChanged(object sender, EventArgs e)
        {
            if (txtCustomerID.Text == "C-0001")
            {
                cmbPaymentMode.SelectedIndex = 0;
                txtPayment.Text = Convert.ToString(Val(txtGrandTotal.Text));
                /*                txtPayment.ReadOnly = true;
                */
            }
            else
            {
                txtPayment.Text = Convert.ToString(Val(txtGrandTotal.Text));
                /*                txtPayment.ReadOnly = false;
                */
            }

        }

        private void dgw_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (DataGridView1.Rows.Count > 0)
            {
                if (lblSet.Text == "Not Allowed")
                {
                    btnRemove.Enabled = true;
                    btnListUpdate.Enabled = true;
                }
                else
                {
                    btnRemove.Enabled = true;
                    btnListUpdate.Enabled = true;
                }

                btnAdd.Enabled = false;

                DataGridViewRow row = DataGridView1.SelectedRows[0];

                txtProductCode.Text = row.Cells[0].Value.ToString();
                txtProductName.Text = row.Cells[1].Value.ToString();
                txtBarcode.Text = row.Cells[2].Value.ToString();
                txtCostPrice.Text = row.Cells[3].Value.ToString();
                txtSellingPrice.Text = row.Cells[4].Value.ToString();
                txtMargin.Text = row.Cells[5].Value.ToString();
                txtQty.Text = row.Cells[6].Value.ToString();
                txtAmount.Text = row.Cells[7].Value.ToString();
                txtDiscountPer.Text = row.Cells[8].Value.ToString();
                txtDiscountAmount.Text = row.Cells[9].Value.ToString();
                txtVAT.Text = row.Cells[10].Value.ToString();
                txtVATAmount.Text = row.Cells[11].Value.ToString();
                txtTotalAmount.Text = row.Cells[12].Value.ToString();
                txtProductID.Text = row.Cells[13].Value.ToString();

                txtQty.Focus();
                dgw.Visible = false;
            }

        }

        private void dgw_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT RTRIM(ID),RTRIM(CustomerID),RTRIM([Name]),RTRIM(Gender), RTRIM(Address),RTRIM(City),RTRIM(State),RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID),RTRIM(Remarks),Photo from Customer where CustomerType='Regular' order by ID", con);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dgw.Rows.Clear();
                while (rdr.Read() == true)
                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11]);
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtProductName_TextChanged(object sender, EventArgs e)
        {
            string productName = txtProductName.Text.Trim();
            string warehouseIdText = WID.Text.Trim(); // Get Warehouse ID text

            if (string.IsNullOrEmpty(productName))
            {
                // If search text is cleared, hide the results grid
                dgw.Visible = false;
                dgw.Rows.Clear(); // Also clear previous results
                return; // Exit the method
            }

            // Proceed only if product name is not empty
            dgw.Visible = true;

            // The SQL query remains the same
            string query = @"SELECT PID, RTRIM(Product.ProductCode), RTRIM(ProductName), RTRIM(Temp_Stock.Barcode), 
                        CostPrice, SellingPrice, Discount, VAT, Qty, RTRIM(Product.SellingPrice2), Plimit 
                        FROM Temp_Stock, Product 
                        WHERE Product.PID = Temp_Stock.ProductID 
                        AND Qty > 0 
                        AND ProductName LIKE @ProductName 
                        AND Temp_Stock.WID LIKE @WID
                        ORDER BY ProductCode";

            try
            {
                // 1. Prepare parameters using DataAccessLayer helper
                var productNameParam = DataAccessLayer.CreateParameter("@ProductName", SqlDbType.NVarChar, "%" + productName + "%");

                // WID Parameter: Handle it carefully. If WID should be an exact match (integer), use SqlDbType.Int.
                // If it can be empty or partial search, keep NVarChar and LIKE.
                // Assuming it should be an exact match if provided, otherwise search all.
                SqlParameter widParam;
                if (string.IsNullOrEmpty(warehouseIdText))
                {
                    // If WID TextBox is empty, search across all warehouses
                    widParam = DataAccessLayer.CreateParameter("@WID", SqlDbType.NVarChar, "%"); // Match any WID using LIKE '%'
                                                                                                 // Alternatively, modify the query to remove the WID condition if empty
                }
                else if (int.TryParse(warehouseIdText, out int warehouseId)) // Ensure WID is a valid integer if not empty
                {
                    // If WID is a valid integer, use exact match (assuming '=' is better than LIKE for performance here)
                    // You might need to adjust the query to use '=' instead of 'LIKE' in this case
                    // query = query.Replace("ts.WID LIKE @WID", "ts.WID = @WID"); // Simple replace (careful)
                    widParam = DataAccessLayer.CreateParameter("@WID", SqlDbType.Int, warehouseId);
                }
                else
                {
                    // If WID text is not empty but not a valid int, maybe show an error or default to searching all?
                    MessageBox.Show("Warehouse ID is not a valid number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    widParam = DataAccessLayer.CreateParameter("@WID", SqlDbType.NVarChar, "-1"); // Search for an invalid WID to return nothing
                }


                // 2. Execute query using DataAccessLayer
                DataTable dt = DataAccessLayer.ExecuteTable(query, CommandType.Text, productNameParam, widParam);

                // 3. Populate the DataGridView (dgw)
                dgw.Rows.Clear(); // Clear existing rows before adding new ones

                if (dt.Rows.Count > 0)
                {
                    // Add rows from DataTable to the DataGridView
                    foreach (DataRow row in dt.Rows)
                    {
                        // Use column indices (0 to 10) as per the SELECT statement
                        dgw.Rows.Add(
                            row[0], // PID
                            row[1], // ProductCode
                            row[2], // ProductName
                            row[3], // Barcode
                            row[4], // CostPrice
                            row[5], // SellingPrice
                            row[6], // Discount
                            row[7], // VAT
                            row[8], // Qty
                            row[9], // SellingPrice2
                            row[10] // Plimit
                        );
                    }
                }
                else
                {
                    // No results found, ensure grid is empty (already cleared)
                    // Optionally display a message in a Label or StatusBar
                }
            }
            catch (SqlException dbEx)
            {
                // Handle potential database errors
                dgw.Visible = false; // Hide grid on error
                MessageBox.Show($"Database Error searching products: {dbEx.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Handle other potential errors
                dgw.Visible = false; // Hide grid on error
                MessageBox.Show($"An error occurred searching products: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
      
        private void GroupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void txtBarcode_TextChanged_1(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBarcode.Text))
            {
                // Make the DataGridView visible
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    dgw.Visible = true;
                    // Establish the SQL connection
                    con.Open();

                    // Define the SQL query with an exact match for the barcode
                    SqlCommand cmd = new SqlCommand(@"SELECT PID, RTRIM(Product.ProductCode), RTRIM(ProductName), 
                                          RTRIM(Temp_Stock.Barcode), CostPrice, SellingPrice, Discount, VAT, Qty, SellingPrice2, Plimit 
                                          FROM Temp_Stock, Product 
                                          WHERE Product.PID = Temp_Stock.ProductID 
                                          AND Qty > 0 
                                          AND Temp_Stock.Barcode = @barcode 
                                          ORDER BY ProductCode", con);

                    // Add the parameter for the barcode search
                    cmd.Parameters.AddWithValue("@barcode", txtBarcode.Text.Trim());

                    // Execute the query and read the results

                    // Clear existing rows in the DataGridView
                    dgw.Rows.Clear();

                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                // Since there are 9 columns, ensure you only access indices 0-8
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10]);
                            }
                            if (dgw.Rows.Count > 0)
                            {
                                DataGridViewRow dr = dgw.SelectedRows[0];
                                txtProductID.Text = dr.Cells[0].Value.ToString();
                                txtProductCode.Text = dr.Cells[1].Value.ToString();
                                txtProductName.Text = dr.Cells[2].Value.ToString();
                                txtBarcode.Text = dr.Cells[3].Value.ToString();
                                txtCostPrice.Text = dr.Cells[4].Value.ToString();

                                if (ComboBox1.SelectedIndex == 0)
                                {
                                    txtSellingPrice.Text = dr.Cells[5].Value.ToString();
                                }
                                else
                                {
                                    txtSellingPrice.Text = dr.Cells[9].Value.ToString();
                                }
                                if (string.IsNullOrEmpty(dr.Cells[10].Value?.ToString()))
                                {
                                    Plimit.Text = "0";
                                }
                                else
                                {
                                    Plimit.Text = dr.Cells[10].Value.ToString();
                                }

                                txtVAT.Text = dr.Cells[7].Value.ToString();

                                double num = Convert.ToDouble(dr.Cells[5].Value) - Convert.ToDouble(dr.Cells[4].Value);
                                num = Math.Round(num, 2);
                                txtMargin.Text = num.ToString();

                                txtDiscountPer.Text = dr.Cells[6].Value.ToString();
                                txtVAT.Text = dr.Cells[7].Value.ToString();
                                txtPayment.Text = txtTotalAmount.Text.ToString();
                                txtQty.Text = "1";
                                visablility.Text = dr.Cells[8].Value.ToString();

                                lblSet.Text = "";
                                dgw.Visible = false;
                            }
                        }
                        else
                        {
                            // If no rows are found, optionally notify the user or take another action
                            dgw.Visible = false; // Optionally hide the DataGridView if no matches are found
                        }
                    }
                }
            }
            else
            {
                dgw.Visible = false;
            }
        }

        private void txtPaymentDue_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            txtCustomerType.Text = "Regular";
            Print();
            Reset();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Products p = new Products();
            p.Reset();
            p.Show();
        }

        private void txtSellingPrice_TextChanged_1(object sender, EventArgs e)
        {
            Compute();
        }

        private void dataGridView3_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void DataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void DataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            string strRowNumber = (e.RowIndex + 1).ToString();
            SizeF size = e.Graphics.MeasureString(strRowNumber, this.Font);
            if (DataGridView1.RowHeadersWidth < Convert.ToInt32(size.Width + 20))
            {
                DataGridView1.RowHeadersWidth = Convert.ToInt32(size.Width + 20);
            }
            Brush b = SystemBrushes.Window; // .ControlText
            e.Graphics.DrawString(strRowNumber, this.Font, b, e.RowBounds.Location.X + DataGridView1.Width - 25, e.RowBounds.Location.Y + (e.RowBounds.Height - size.Height) / 2);

        }

        private void DataGridView1_ControlAdded_1(object sender, ControlEventArgs e)
        {
            txtPayment.Text = (txtGrandTotal.Text.ToString());
            if (cmbPaymentMode.SelectedIndex == 2 | cmbPaymentMode.SelectedIndex == 3 | cmbPaymentMode.SelectedIndex == 2 | cmbPaymentMode.SelectedIndex == 4 | cmbPaymentMode.SelectedIndex == 5 | cmbPaymentMode.SelectedIndex == 6 | cmbPaymentMode.SelectedIndex == 7 | cmbPaymentMode.SelectedIndex == 8 | cmbPaymentMode.SelectedIndex == 9)
            {
                txtPayment.Text = "0";

            }
            else
            {
                txtPayment.Text = (txtGrandTotal.Text.ToString());
            }
        }

        private void DataGridView1_CellContentClick_2(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgw_CellContentClick_2(object sender, DataGridViewCellEventArgs e)
        {

        }


        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            decimal totalPayment = 0;
            decimal additionalPayment = 0;

            // Check if txtTotalPayment is not empty or contains a valid decimal number
            if (decimal.TryParse(txtTotalPayment.Text, out totalPayment) == false)
            {
                totalPayment = 0;
            }

            // Check if textBox3 is not empty or contains a valid decimal number
            if (decimal.TryParse(textBox3.Text, out additionalPayment) == false)
            {
                additionalPayment = 0;
            }

            // Calculate and set the result
            txtPayment.Text = (totalPayment + additionalPayment).ToString();


        }

        private void btn_add_payment_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtSalesmanID.Text))
            {
                MessageBox.Show("الرجاء تحديد رقم البائع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Button1.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("الرجاء تحديد العميل", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (DataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("الرجاء اضافة اصناف للشبكة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (textBox3.Text == null || textBox3.Text == "")
            {
                MessageBox.Show("يرجي كتابة دفعه ", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (DataGridView2.Rows.Count == 0)
            {
                DataGridView2.Rows.Add(cmbPaymentMode.Text, txtPayment.Text, dtpPaymentDate.Value.Date);
                double j = Math.Round(TotalPayment(), 2);
                txtTotalPayment.Text = j.ToString();
                Compute1();
            }
            if (cmbPaymentMode.SelectedIndex == 0)
            {
                DataGridView2.Rows.Clear();
                DataGridView2.Rows.Add(cmbPaymentMode.Text, txtPayment.Text, dtpPaymentDate.Value.Date);
                double j = Math.Round(TotalPayment(), 2);
                txtTotalPayment.Text = j.ToString();
                Compute1();
            }
            if (Convert.ToDouble(txtTotalPayment.Text) > Convert.ToDouble(txtGrandTotal.Text))
            {
                MessageBox.Show("المبلغ المدفوع لا يكون اكبر من قيمة الفاتورة", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();

                // Update InvoiceInfo
                string cb = "Update InvoiceInfo set InvoiceNo=@d2, CustomerID=@d4, GrandTotal=@d5, TotalPaid=@d6, Balance=@d7, Remarks=@d8, SalesmanID=@d9 where INV_ID=@d1";
                using (SqlCommand cmd = new SqlCommand(cb, con))
                {

                    cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                    cmd.Parameters.AddWithValue("@d2", txtInvoiceNo.Text);
                    cmd.Parameters.AddWithValue("@d4", Convert.ToInt32(txtCID.Text));
                    cmd.Parameters.AddWithValue("@d5", Convert.ToDouble(txtGrandTotal.Text));
                    cmd.Parameters.AddWithValue("@d6", Convert.ToDouble(txtTotalPayment.Text));
                    cmd.Parameters.AddWithValue("@d7", Convert.ToDouble(txtPaymentDue.Text));
                    cmd.Parameters.AddWithValue("@d8", txtRemarks.Text);
                    cmd.Parameters.AddWithValue("@d9", Convert.ToInt32(txtSM_ID.Text));
                    cmd.ExecuteNonQuery();
                }

                // Delete existing Invoice_Product entries
                using (SqlCommand cmd = new SqlCommand("delete from Invoice_Product where InvoiceID=@d1", con))
                {
                    cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                    cmd.ExecuteNonQuery();
                }

                // Insert new Invoice_Product entries
                string cb1 = "insert into Invoice_Product(InvoiceID, Barcode, CostPrice, SellingPrice, Margin, Qty, Amount, DiscountPer, Discount, VATPer, VAT, TotalAmount, ProductID) VALUES (@d1, @d4, @d5, @d6, @d7, @d8, @d9, @d10, @d11, @d12, @d13, @d14, @d15)";
                using (SqlCommand cmd = new SqlCommand(cb1, con))
                {
                    foreach (DataGridViewRow row in DataGridView1.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                            cmd.Parameters.AddWithValue("@d4", row.Cells[2].Value);
                            cmd.Parameters.AddWithValue("@d5", Convert.ToDecimal(row.Cells[3].Value));
                            cmd.Parameters.AddWithValue("@d6", Convert.ToDecimal(row.Cells[4].Value));
                            cmd.Parameters.AddWithValue("@d7", Convert.ToDecimal(row.Cells[5].Value));
                            cmd.Parameters.AddWithValue("@d8", Convert.ToDecimal(row.Cells[6].Value));
                            cmd.Parameters.AddWithValue("@d9", Convert.ToDecimal(row.Cells[7].Value));
                            cmd.Parameters.AddWithValue("@d10", Convert.ToDecimal(row.Cells[8].Value));
                            cmd.Parameters.AddWithValue("@d11", Convert.ToDecimal(row.Cells[9].Value));
                            cmd.Parameters.AddWithValue("@d12", Convert.ToDecimal(row.Cells[10].Value));
                            cmd.Parameters.AddWithValue("@d13", Convert.ToDecimal(row.Cells[11].Value));
                            cmd.Parameters.AddWithValue("@d14", Convert.ToDecimal(row.Cells[12].Value));
                            cmd.Parameters.AddWithValue("@d15", Convert.ToInt32(row.Cells[13].Value));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                // Delete existing Invoice_Payment entries
                using (SqlCommand cmd = new SqlCommand("delete from Invoice_Payment where InvoiceID=@d1", con))
                {
                    cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                    cmd.ExecuteNonQuery();
                }

                // Insert new Invoice_Payment entries
                string cb2 = "insert into Invoice_Payment(InvoiceID, PaymentMode, TotalPaid, PaymentDate) VALUES (@d1, @d4, @d5, @d6)";
                using (SqlCommand cmd = new SqlCommand(cb2, con))
                {
                    foreach (DataGridViewRow row in DataGridView2.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                            cmd.Parameters.AddWithValue("@d4", row.Cells[0].Value);
                            cmd.Parameters.AddWithValue("@d5", Convert.ToDecimal(row.Cells[1].Value));
                            cmd.Parameters.AddWithValue("@d6", Convert.ToDateTime(row.Cells[2].Value));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open(); // Open the connection once outside the loop

                foreach (DataGridViewRow row1 in DataGridView1.Rows)
                {
                    bool matchFound = false;
                    foreach (DataGridViewRow row3 in dataGridView3.Rows)
                    {
                        // Check if Barcode matches (assuming Cells[2] is Barcode in both grids)
                        if (row1.Cells[2].Value != null && row3.Cells[2].Value != null &&
                            row1.Cells[2].Value.ToString() == row3.Cells[2].Value.ToString())
                        {
                            matchFound = true;

                            // Get the Qty values from both DataGridViews
                            double qty1 = Convert.ToDouble(row1.Cells[6].Value); // Qty from DataGridView1
                            double qty3 = Convert.ToDouble(row3.Cells[6].Value); // Qty from DataGridView3

                            // Calculate the quantity difference
                            double qtyDifference = qty1 - qty3;

                            // Only update if there is a difference
                            if (qtyDifference != 0)
                            {
                                string stockUpdateQuery = "Update Temp_Stock set Qty = Qty - @Qty where ProductID=@d1";
                                if (qtyDifference < 0) // If the qty3 is greater than qty1, we add the difference
                                {
                                    stockUpdateQuery = "Update Temp_Stock set Qty = Qty + @Qty where ProductID=@d1";
                                    qtyDifference = Math.Abs(qtyDifference); // Convert the difference to a positive value for addition
                                }

                                using (SqlCommand cmd = new SqlCommand(stockUpdateQuery, con))
                                {
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.AddWithValue("@Qty", qtyDifference);
                                    cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(row1.Cells[13].Value)); // ProductID from DataGridView1
                                    cmd.Parameters.AddWithValue("@d2", row1.Cells[2].Value); // Barcode from DataGridView1
                                    cmd.ExecuteNonQuery();
                                }

                            }

                            // Exit the inner loop once a match is found
                            break;
                        }
                    }

                    // If no match was found, update Qty for row1 as a standalone entry
                    if (!matchFound)
                    {
                        string stockUpdateQuery = "Update Temp_Stock set Qty = Qty - @Qty where ProductID=@d1";
                        using (SqlCommand cmd = new SqlCommand(stockUpdateQuery, con))
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@Qty", Convert.ToInt32(row1.Cells[6].Value)); // Qty from DataGridView1
                            cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(row1.Cells[13].Value)); // ProductID from DataGridView1
                            cmd.Parameters.AddWithValue("@d2", row1.Cells[2].Value); // Barcode from DataGridView1
                            cmd.ExecuteNonQuery();
                        }

                    }
                }

                con.Close(); // Close the connection after the loop
            }
            if (txtT_ID_1.Text != "")
            {
                using (SqlConnection conn = new SqlConnection(DataAccessLayer.Con()))
                {
                    conn.Open();
                    string cb = "INSERT INTO Payment_2(TC_ID, TransactionID, Date, PaymentMode, CustomerID, Amount, Remarks, Check_ID, Check_Date, SalesMan_ID, SalesMan_Name, SalesMan_Comession, SalesMan_ID_2) " +
                                "VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9, @d10, @d11, @d12, @d13)";
                    using (SqlCommand cmd = new SqlCommand(cb, conn))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtT_ID.Text));
                        cmd.Parameters.AddWithValue("@d2", txtTransactionNo.Text);
                        cmd.Parameters.AddWithValue("@d3", DateTime.Now.Date);
                        cmd.Parameters.AddWithValue("@d4", cmbPaymentMode.Text);
                        cmd.Parameters.AddWithValue("@d5", txtCustomerID.Text);
                        cmd.Parameters.AddWithValue("@d6", Convert.ToDecimal(textBox3.Text) == null ? 0 : Convert.ToDecimal(textBox3.Text));
                        cmd.Parameters.AddWithValue("@d7", txtRemarks.Text);
                        cmd.Parameters.AddWithValue("@d8", txtCheck.Text);
                        cmd.Parameters.AddWithValue("@d9", dtpPaymentDate.Value.Date);
                        //cmd.Parameters.AddWithValue("@d10", txtBank.Text);
                        cmd.Parameters.AddWithValue("@d10", txtSM_ID.Text);
                        cmd.Parameters.AddWithValue("@d11", txtSalesman.Text);
                        cmd.Parameters.AddWithValue("@d12", txtCommissionPer.Text);
                        cmd.Parameters.AddWithValue("@d13", txtSalesmanID.Text);

                        cmd.ExecuteNonQuery();
                    }
                }
                if (cmbPaymentMode.SelectedIndex == 1)
                {
                    LedgerSave(dtpPaymentDate.Value.Date, "شيك رقم" + txtCheck.Text.Trim(), txtInvoiceNo.Text, "دفعات فاتورة مبيعات", 0, Math.Abs(Convert.ToDecimal(textBox3.Text)), txtCustomerID.Text, "", Math.Abs(Convert.ToDecimal(total_sale.Text)));
                }
                else if (cmbPaymentMode.SelectedIndex == 4)
                {
                    LedgerSave(dtpPaymentDate.Value.Date, "اجل", txtInvoiceNo.Text, "دفعات فاتورة مبيعات", 0, Math.Abs(Convert.ToDecimal(textBox3.Text)), txtCustomerID.Text, "", Math.Abs(Convert.ToDecimal(total_sale.Text)));
                }
                else
                {
                    LedgerSave(dtpPaymentDate.Value.Date, "نقدا", txtInvoiceNo.Text, "دفعات فاتورة مبيعات", 0, Math.Abs(Convert.ToDecimal(textBox3.Text)), txtCustomerID.Text, "", Math.Abs(Convert.ToDecimal(total_sale.Text)));
                }
            }
            else
            {
                if (Convert.ToDecimal(txtPaymentDue.Text) != 0)
                {
                    using (SqlConnection conn = new SqlConnection(DataAccessLayer.Con()))
                    {
                        conn.Open();
                        string cb = "INSERT INTO Payment_2(TC_ID, TransactionID, Date, PaymentMode, CustomerID, Amount, Remarks, Check_ID, Check_Date, SalesMan_ID, SalesMan_Name, SalesMan_Comession, SalesMan_ID_2) " +
                                    "VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9, @d10, @d11, @d12, @d13)";
                        using (SqlCommand cmd = new SqlCommand(cb, conn))
                        {
                            cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtT_ID.Text));
                            cmd.Parameters.AddWithValue("@d2", txtTransactionNo.Text);
                            cmd.Parameters.AddWithValue("@d3", dtpPaymentDate.Value.Date);
                            cmd.Parameters.AddWithValue("@d4", cmbPaymentMode.Text);
                            cmd.Parameters.AddWithValue("@d5", txtCustomerID.Text);
                            cmd.Parameters.AddWithValue("@d6", -Convert.ToDecimal(txtPaymentDue.Text));
                            cmd.Parameters.AddWithValue("@d7", txtRemarks.Text);
                            cmd.Parameters.AddWithValue("@d8", txtCheck.Text);
                            cmd.Parameters.AddWithValue("@d9", dtpPaymentDate.Value.Date);
                            //cmd.Parameters.AddWithValue("@d10", txtBank.Text);
                            cmd.Parameters.AddWithValue("@d10", txtSM_ID.Text);
                            cmd.Parameters.AddWithValue("@d11", txtSalesman.Text);
                            cmd.Parameters.AddWithValue("@d12", txtCommissionPer.Text);
                            cmd.Parameters.AddWithValue("@d13", txtSalesmanID.Text);

                            cmd.ExecuteNonQuery();
                        }
                    }
                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();
                        string updateQuery = "UPDATE InvoiceInfo SET InvoiceNo = @d2, InvoiceDate = @d3, CustomerID = @d4, GrandTotal = @d5, TotalPaid = @d6, Balance = @d7, Remarks = @d8, SalesmanID = @d9, TC_ID = @d10 WHERE Inv_ID = @d1";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text)); // Assuming Inv_ID is the primary key used for updating
                            cmd.Parameters.AddWithValue("@d2", txtInvoiceNo.Text);
                            cmd.Parameters.AddWithValue("@d3", dtpInvoiceDate.Value.Date);
                            cmd.Parameters.AddWithValue("@d4", Convert.ToInt32(txtCID.Text));
                            cmd.Parameters.AddWithValue("@d5", Convert.ToDouble(txtGrandTotal.Text));
                            cmd.Parameters.AddWithValue("@d6", Convert.ToDouble(txtTotalPayment.Text));
                            cmd.Parameters.AddWithValue("@d7", Convert.ToDouble(txtPaymentDue.Text));
                            cmd.Parameters.AddWithValue("@d8", txtRemarks.Text);
                            cmd.Parameters.AddWithValue("@d9", Convert.ToInt32(txtSM_ID.Text));
                            cmd.Parameters.AddWithValue("@d10", txtT_ID.Text);
                            cmd.ExecuteNonQuery();
                        }
                    }



                }

                if (cmbPaymentMode.SelectedIndex == 4)
                {

                    LedgerUpdate(dtpPaymentDate.Value.Date, "اجل", Math.Abs(Convert.ToDecimal(txtGrandTotal.Text)), Math.Abs(Convert.ToDecimal(txtTotalPayment.Text)), txtCustomerID.Text, txtInvoiceNo.Text, "فاتورة مبيعات", Math.Abs(Convert.ToDecimal(total_sale.Text)));


                }
                else if (cmbPaymentMode.SelectedIndex == 1)
                {

                    LedgerUpdate(dtpPaymentDate.Value.Date, "شيك رقم" + txtCheck.Text.Trim(), Math.Abs(Convert.ToDecimal(txtGrandTotal.Text)), Math.Abs(Convert.ToDecimal(txtTotalPayment.Text)), txtCustomerID.Text, txtInvoiceNo.Text, "فاتورة مبيعات", Math.Abs(Convert.ToDecimal(total_sale.Text)));


                }
                else
                {
                    LedgerUpdate(dtpPaymentDate.Value.Date, "نقدا", Math.Abs(Convert.ToDecimal(txtGrandTotal.Text)), Math.Abs(Convert.ToDecimal(txtTotalPayment.Text)), txtCustomerID.Text, txtInvoiceNo.Text, "فاتورة مبيعات", Math.Abs(Convert.ToDecimal(total_sale.Text)));
                }



            }



            // Log the update action
            string st = $"updated the bill (Products) having invoice no. '{txtInvoiceNo.Text}'";
            LogFunc(lblUser.Text, st);

            // Disable the update button after successful update
            btnUpdate.Enabled = false;
            MessageBox.Show("تم اضافة الدفعات بنجاح", "السجلات", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Reset();
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
        public interface IWarehouseRepository
        {
            List<string> GetWarehouseNames();
        }

        public class WarehouseRepository : IWarehouseRepository
        {
            public List<string> GetWarehouseNames()
            {
                List<string> warehouseNames = new List<string>();
                try
                {
                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT WarehouseName FROM [dbo].[Warehouses]", con))
                        {
                            con.Open();
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    // Assuming WarehouseName is never null.
                                    warehouseNames.Add(reader["WarehouseName"].ToString());
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception details as needed.
                    // Logging improves maintainability and troubleshooting.
                    // Optionally, rethrow the exception or return an empty list.
                    // For now, we simply return an empty list.
                }
                return warehouseNames;
            }
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


        private void TextBox2_TextChanged(object sender, EventArgs e)
        {

        }



        private void label27_Click(object sender, EventArgs e)
        {

        }


        private void txtQtyAfter_TextChanged(object sender, EventArgs e)
        {
            Compute();

        }

        private void txtQtyK_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTotalAmount_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtVATAmount_TextChanged(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void total_sale_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // If the text boxes are empty, set their values to "0"
                if (string.IsNullOrWhiteSpace(total_sale.Text))
                {
                    total_sale.Text = "0";
                    total_sale.SelectionStart = total_sale.Text.Length; // Set the cursor at the end
                }
                if (string.IsNullOrWhiteSpace(txtTotalPayment.Text))
                {
                    txtTotalPayment.Text = "0";
                    txtTotalPayment.SelectionStart = txtTotalPayment.Text.Length; // Set the cursor at the end
                }
                if (string.IsNullOrWhiteSpace(txtGrandTotal.Text))
                {
                    txtGrandTotal.Text = "0";
                    txtGrandTotal.SelectionStart = txtGrandTotal.Text.Length; // Set the cursor at the end
                }

                // Ensure both text boxes have valid numeric values before performing the calculation
                decimal totalSale = 0;
                decimal grandTotal = 0;

                // Try parsing the values
                if (decimal.TryParse(total_sale.Text.Trim(), out totalSale) &&
                    decimal.TryParse(txtGrandTotal.Text.Trim(), out grandTotal))
                {
                    // Calculate the result
                    decimal result = grandTotal - totalSale;

                    // Update the txtGrandTotal with the result
                    txtTotalPayment.Text = result.ToString("N2"); // Format to 2 decimal places
                    txtPayment.Text = txtTotalPayment.Text;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /*
                        // Try parsing the values
                        if (decimal.TryParse(   .Text.Trim(), out totalSale) &&
                            decimal.TryParse(txtGrandTotal.Text.Trim(), out grandTotal))
                        {
                            // Calculate the result
                            decimal result = grandTotal - totalSale;

                            // Update the txtGrandTotal with the result
                            txtTotalPayment.Text = result.ToString("N2"); // Format to 2 decimal places
                            txtPayment.Text = txtGrandTotal.Text;

                        }*/

        private void label27_Click_1(object sender, EventArgs e)
        {

        }
        private void DataGridView1_RowPostPaint_1(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            string strRowNumber = (e.RowIndex + 1).ToString();
            SizeF size = e.Graphics.MeasureString(strRowNumber, this.Font);

            if (DataGridView1.RowHeadersWidth < Convert.ToInt32(size.Width + 20))
            {
                DataGridView1.RowHeadersWidth = Convert.ToInt32(size.Width + 20);
            }

            Brush b = new SolidBrush(Color.Black); // You can use SystemBrushes.ControlText for text color if needed
            e.Graphics.DrawString(strRowNumber, this.Font, b, e.RowBounds.Location.X + DataGridView1.Width - 25, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2));
        }

        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (dgw.Rows.Count > 0)
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];
                    txtProductID.Text = dr.Cells[0].Value.ToString();
                    txtProductCode.Text = dr.Cells[1].Value.ToString();
                    txtProductName.Text = dr.Cells[2].Value.ToString();
                    txtBarcode.Text = dr.Cells[3].Value.ToString();
                    txtCostPrice.Text = dr.Cells[4].Value.ToString();

                    if (ComboBox1.SelectedIndex == 0)
                    {
                        txtSellingPrice.Text = dr.Cells[5].Value.ToString();
                    }
                    else
                    {
                        txtSellingPrice.Text = dr.Cells[9].Value.ToString();
                    }

                    txtVAT.Text = dr.Cells[7].Value.ToString();

                    double num = Convert.ToDouble(dr.Cells[5].Value) - Convert.ToDouble(dr.Cells[4].Value);
                    num = Math.Round(num, 2);
                    txtMargin.Text = num.ToString();
                    if (string.IsNullOrEmpty(dr.Cells[10].Value?.ToString()))
                    {
                        Plimit.Text = "0";
                    }
                    else
                    {
                        Plimit.Text = dr.Cells[10].Value.ToString();
                    }

                    txtDiscountPer.Text = dr.Cells[6].Value.ToString();
                    txtVAT.Text = dr.Cells[7].Value.ToString();

                    txtQty.Focus();
                    visablility.Text = dr.Cells[8].Value.ToString();

                    lblSet.Text = "";
                    dgw.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void txtProductName_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void dgw_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        public void fillProductName()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(@"SELECT RTRIM(ProductName) 
                                                     FROM Temp_Stock 
                                                     INNER JOIN Product ON Product.PID = Temp_Stock.ProductID 
                                                     WHERE Qty > 0 
                                                     AND Temp_Stock.WID LIKE @WID 
                                                     AND ProductName LIKE @ProductName 
                                                     ORDER BY ProductCode", con))
                    {
                        cmd.Parameters.AddWithValue("@WID", WID.Text.Trim() + "%");
                        cmd.Parameters.AddWithValue("@ProductName", "%" + txtProductName.Text.Trim() + "%");

                        using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adp.Fill(dt);
                            txtProductName.Items.Clear();

                            if (dt.Rows.Count > 0)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    txtProductName.Items.Add(row[0].ToString());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void txtProductName_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.PageDown || e.KeyCode == Keys.Down) // Use Down Arrow too
            {
                if (dgw.Visible && dgw.Rows.Count > 0)
                {
                    dgw.Focus();
                    if (dgw.CurrentRow == null && dgw.Rows.Count > 0)
                    {
                        dgw.CurrentCell = dgw.Rows[0].Cells[GetFirstVisibleColumnIndex(dgw)]; // Select first cell of first row
                    }
                    else if (dgw.CurrentRow != null)
                    {
                        // Optional: Move selection down if already focused
                        int currentIdx = dgw.CurrentRow.Index;
                        if (currentIdx < dgw.Rows.Count - 1)
                        {
                            dgw.CurrentCell = dgw.Rows[currentIdx + 1].Cells[GetFirstVisibleColumnIndex(dgw)];
                        }
                    }
                    if (dgw.CurrentRow != null) dgw.CurrentRow.Selected = true;
                    e.Handled = true; // Prevent default keydown behavior
                }
            }
        }
        private int GetFirstVisibleColumnIndex(DataGridView grid)
        {
            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (col.Visible) return col.Index;
            }
            return 0; // Default to first column if none are visible (unlikely)
        }
        private void dgw_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && dgw.CurrentRow != null)
            {
                DataGridViewRow row = dgw.CurrentRow; // Get the selected row

                // Get values safely (prevent NullReferenceException)
                string productID = row.Cells[0].Value?.ToString() ?? "";
                string productCode = row.Cells[1].Value?.ToString() ?? "";
                string productName = row.Cells[2].Value?.ToString() ?? "";
                string barcode = row.Cells[3].Value?.ToString() ?? "";
                string costPrice = row.Cells[4].Value?.ToString() ?? "";
                string sellingPrice1 = row.Cells[5].Value?.ToString() ?? "";
                string discount = row.Cells[6].Value?.ToString() ?? "";
                string vat = row.Cells[7].Value?.ToString() ?? "";
                string qty = row.Cells[8].Value?.ToString() ?? "";
                string sellingPrice2 = row.Cells[9].Value?.ToString() ?? "";
                string plimit = row.Cells[10].Value?.ToString() ?? "0"; // Default to "0" if null

                // Assign values to textboxes
                txtProductID.Text = productID;
                txtProductCode.Text = productCode;
                txtProductName.Text = productName;
                txtBarcode.Text = barcode;
                txtCostPrice.Text = costPrice;

                // Choose selling price based on ComboBox selection
                txtSellingPrice.Text = (ComboBox1.SelectedIndex == 0) ? sellingPrice1 : sellingPrice2;

                txtVAT.Text = vat;

                // Calculate Margin (Ensure conversion is safe)
                if (double.TryParse(sellingPrice1, out double sellPrice) && double.TryParse(costPrice, out double cost))
                {
                    txtMargin.Text = Math.Round(sellPrice - cost, 2).ToString();
                }
                else
                {
                    txtMargin.Text = "0"; // Default margin to 0 if conversion fails
                }

                Plimit.Text = plimit;
                txtDiscountPer.Text = discount;
                txtVAT.Text = vat;

                // Move focus to quantity input
                txtQty.Focus();

                visablility.Text = qty;

                lblSet.Text = "";

                // Hide DataGridView after selection
                dgw.Visible = false;

                // Move focus back to txtProductName
                txtQty.Focus();
                e.Handled = true; // Prevent default Enter key behavior
            }
        }

        private void txtProductName1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

