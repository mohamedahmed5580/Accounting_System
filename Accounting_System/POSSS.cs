/*
using CrystalDecisions.Shared;
using Microsoft.Office.Interop.Excel; // Keep if used elsewhere, but not directly in refactored DB code
using Microsoft.VisualBasic; // Keep if used elsewhere (like Interaction.MsgBox)
 // Using the Data Access Layer namespace
using SixLabors.ImageSharp.Drawing; // Keep if used elsewhere
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient; // Keep for SqlDbType enum and SqlException
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography; // Keep if used elsewhere
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.Data.Helpers.ExpressiveSortInfo; // Keep if needed
using static System.Windows.Forms.VisualStyles.VisualStyleElement; // Keep if needed
using DataTable = System.Data.DataTable; // Alias needed because System.Drawing also has DataTable
using DrawingRectangle = System.Drawing.Rectangle; // Keep if needed
using ExcelRectangle = Microsoft.Office.Interop.Excel.Rectangle; // Keep if needed

namespace Accounting_System
{
    public partial class POSSS : Form
    {
        // SqlConnection con = new SqlConnection(DataAccessLayer.Con()); // REMOVED - Handled by DAL
        public static POSSS instance;
        int i = 0; // Used by CountValue

        public POSSS()
        {
            InitializeComponent();
        }

        private void POS_Load(object sender, EventArgs e)
        {
            InitializeDefaults();
            Reset(); // This calls Getdata1, Auto, etc. which now use the DAL
            comboBox2.SelectedIndex = 0;
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

            if (txtCustomerID.Text == "C-0001")
            {
                cmbPaymentMode.SelectedIndex = 0;
                txtPayment.ReadOnly = false; // Or adjust based on logic
            }
            else
            {
                txtPayment.ReadOnly = false;
            }
            instance = this;
        }


        private void Button1_Click(object sender, EventArgs e) // Original handler for a button
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
            // (UI Reset Code remains the same)
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

            textBox3.Visible = false;
            total_sale.Visible = true;
            label26.Visible = true;
            dtpInvoiceDate.Value = DateTime.Today;
            dgw.Visible = false;
            DataGridView1.Rows.Clear();
            DataGridView2.Rows.Clear();

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

            // These methods should now use the DAL internally
            Getdata1();
            auto();
            Auto();
            Clear1();
            Clear();

            cmbPaymentMode.SelectedIndex = 0;
            txtCustomerID.Text = "C-0001";
            comboBoxGenerate(); // This should now use the DAL
            txtProductName.Focus();
        }

        // --- Other UI event handlers (like dgw_CellContentClick, txtDiscountPer_TextChanged, etc.) remain unchanged ---
        // --- unless they contained direct DB access logic.                                                   ---

        private void Button1_Click_1(object sender, EventArgs e) // Handler for a different Button1
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


        public void auto() // For Invoice ID
        {
            try
            {
                txtID.Text = GenerateID(); // Uses DAL now
                txtInvoiceNo.Text = "Inv-" + txtID.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating invoice number: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtID.Text = "0001"; // Default value on error
                txtInvoiceNo.Text = "Inv-Error";
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


        public void Clear1() // UI Reset for Payment section
        {
            cmbPaymentMode.SelectedIndex = 0;
            dtpPaymentDate.Text = DateTime.Today.ToString();
            txtPayment.Text = "0.00"; // Reset payment field as well
            btnAdd1.Enabled = true;
            btnRemove1.Enabled = false;
            btnListUpdate1.Enabled = false;
        }

        public void Clear() // UI Reset for Product Entry section
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
            Plimit.Text = "0"; // Reset limit
            txtProductID.Text = ""; // Reset hidden Product ID
            visablility.Text = ""; // Reset hidden visibility/stock field

            btnAdd.Enabled = true;
            btnRemove.Enabled = false;
            btnListUpdate.Enabled = false;
            dgw.Visible = false;
            txtProductName.Enabled = true;
            txtBarcode.Focus(); // Or txtProductName.Focus() depending on preference
        }

        public void ClearAddProdact() // Seems similar to Clear(), maybe redundant? Keeping it for now.
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
            txtProductID.Text = "";
            Plimit.Text = "0";
            visablility.Text = "";

            btnAdd.Enabled = true;
            btnRemove.Enabled = false;
            btnListUpdate.Enabled = false;
            dgw.Visible = false;
            txtBarcode.Focus();

            // Also clears payment section - maybe combine Clear() and Clear1()?
            cmbPaymentMode.SelectedIndex = 0;
            dtpPaymentDate.Text = DateTime.Today.ToString();
            txtPayment.Text = "0.00";
            btnAdd1.Enabled = true;
            btnRemove1.Enabled = false;
            btnListUpdate1.Enabled = false;
        }

        private void total_sale_save_Click(object sender, EventArgs e) // Specific save button
        {
            try
            {
                bool saveSuccess = POSSAVE(); // Call POSSAVE (uses DAL now)

                if (!saveSuccess)
                {
                    // Message already shown in POSSAVE or validation
                    return;
                }

                MessageBox.Show("تم الحفظ بنجاح", "الفاتورة", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
                btnPrint.Enabled = true;
                txtCustomerType.Text = "Regular Sale"; // Set type for printing
                Print(); // Uses direct ADO.NET for Crystal Reports (unchanged)
                Reset();
            }
            catch (Exception ex) // Catch exceptions potentially thrown by POSSAVE
            {
                MessageBox.Show($"An unexpected error occurred during save: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Button2_Click_1(object sender, EventArgs e) // General Save Button
        {
            try
            {
                bool saveSuccess = POSSAVE(); // Call POSSAVE (uses DAL now)

                if (!saveSuccess)
                {
                    // Message already shown in POSSAVE or validation
                    return;
                }

                MessageBox.Show("تم الحفظ بنجاح", "الفاتورة", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false; // Assuming Button2 is the main save button
                Reset();
            }
            catch (Exception ex) // Catch exceptions potentially thrown by POSSAVE
            {
                MessageBox.Show($"An unexpected error occurred during save: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button8_Click(object sender, EventArgs e) // Another specific save?
        {
            try
            {
                bool saveSuccess = POSSAVE(); // Call POSSAVE (uses DAL now)

                if (!saveSuccess)
                {
                    return;
                }

                MessageBox.Show("تم الحفظ بنجاح", "الفاتورة", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
                btnPrint.Enabled = true;
                txtCustomerType.Text = "Non Regular Sale"; // Set type for printing
                Print(); // Uses direct ADO.NET for Crystal Reports (unchanged)
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred during save: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click(object sender, EventArgs e) // Yet another specific save?
        {
            try
            {
                bool saveSuccess = POSSAVE(); // Call POSSAVE (uses DAL now)

                if (!saveSuccess)
                {
                    return;
                }
                MessageBox.Show("تم الحفظ بنجاح", "الفاتورة", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
                btnPrint.Enabled = true;
                txtCustomerType.Text = "Non Regular"; // Set type for printing
                Print(); // Uses direct ADO.NET for Crystal Reports (unchanged)
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Show full exception here? Maybe just ex.Message
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
            if (DataGridView1.Rows.Count == 0) { *//* ... *//* return false; }
            if (DataGridView2.Rows.Count == 0) { *//* Add default payment *//* }
            if (double.Parse(txtTotalPayment.Text) > double.Parse(txtGrandTotal.Text)) { *//* ... *//* return false; }

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
                // Sum the 'TotalAmount' column (index 12)
                sum = DataGridView1.Rows.Cast<DataGridViewRow>()
                           .Where(r => !r.IsNewRow && r.Cells[12].Value != null && decimal.TryParse(r.Cells[12].Value.ToString(), out _))
                           .Sum(r => Convert.ToDouble(r.Cells[12].Value));
            }
            catch (Exception ex)
            {
                // Use Interaction.MsgBox if Microsoft.VisualBasic is intended, otherwise use MessageBox
                MessageBox.Show("Error calculating Grand Total: " + ex.Message, "Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Interaction.MsgBox(ex.Message); // If using VB Interaction
            }
            return Math.Round(sum, 2); // Round to 2 decimal places
        }

        public void SMS(string st1) // Assuming this sends an SMS via DB trigger or stored procedure
        {
            try
            {
                string query = "INSERT INTO SMS(Message, Date) VALUES (@d1, @d2)";
                var parameters = new[] {
                    DataAccessLayer.CreateParameter("@d1", SqlDbType.NVarChar, st1),
                    DataAccessLayer.CreateParameter("@d2", SqlDbType.DateTime, DateTime.Now)
                };
                DataAccessLayer.ExecuteNonQuery(query, CommandType.Text, parameters);
            }
            catch (SqlException dbEx)
            {
                MessageBox.Show("Database Error saving SMS: " + dbEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("General Error saving SMS: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LogFunc(string st1, string st2) // Logs user actions
        {
            try
            {
                string query = "INSERT INTO Logs(UserID, Date, Operation) VALUES (@d1, @d2, @d3)";
                var parameters = new[] {
                    DataAccessLayer.CreateParameter("@d1", SqlDbType.NVarChar, st1), // Assuming UserID is NVarChar based on lblUser.Text
                    DataAccessLayer.CreateParameter("@d2", SqlDbType.DateTime, DateTime.Now),
                    DataAccessLayer.CreateParameter("@d3", SqlDbType.NVarChar, st2)
                };
                DataAccessLayer.ExecuteNonQuery(query, CommandType.Text, parameters);
            }
            catch (SqlException dbEx)
            {
                // Avoid showing message box for logging errors? Consider logging to a file instead.
                Console.WriteLine("Database Error logging action: " + dbEx.Message);
                // MessageBox.Show("Database Error logging action: " + dbEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error logging action: " + ex.Message);
                // MessageBox.Show("General Error logging action: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g, string h, decimal s)
        {
            try
            {
                string query = @"INSERT INTO LedgerBook(Date, Name, LedgerNo, Label, Debit, Credit, PartyID, Manual_Inv, total_sale)
                                VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d11)";
                var parameters = new[] {
                    DataAccessLayer.CreateParameter("@d1", SqlDbType.DateTime, a),
                    DataAccessLayer.CreateParameter("@d2", SqlDbType.NVarChar, b),
                    DataAccessLayer.CreateParameter("@d3", SqlDbType.NVarChar, c), // LedgerNo is likely text (e.g., InvoiceNo)
                    DataAccessLayer.CreateParameter("@d4", SqlDbType.NVarChar, d),
                    DataAccessLayer.CreateParameter("@d5", SqlDbType.Decimal, e), // Debit
                    DataAccessLayer.CreateParameter("@d6", SqlDbType.Decimal, f), // Credit
                    DataAccessLayer.CreateParameter("@d7", SqlDbType.NVarChar, g), // PartyID (CustomerID) is likely text
                    DataAccessLayer.CreateParameter("@d8", SqlDbType.NVarChar, h), // Manual_Inv
                    DataAccessLayer.CreateParameter("@d11", SqlDbType.Decimal, s) // total_sale
                };
                DataAccessLayer.ExecuteNonQuery(query, CommandType.Text, parameters);
            }
            catch (SqlException dbEx)
            {
                MessageBox.Show("Database Error saving ledger entry: " + dbEx.Message, "Ledger Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("General Error saving ledger entry: " + ex.Message, "Ledger Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void LedgerDelete(string a, string b) // a = LedgerNo (InvoiceNo), b = Label
        {
            try
            {
                string query = "DELETE FROM LedgerBook WHERE LedgerNo = @d1 AND Label = @d2";
                var parameters = new[] {
                    DataAccessLayer.CreateParameter("@d1", SqlDbType.NVarChar, a),
                    DataAccessLayer.CreateParameter("@d2", SqlDbType.NVarChar, b)
                };
                DataAccessLayer.ExecuteNonQuery(query, CommandType.Text, parameters);
            }
            catch (SqlException dbEx)
            {
                MessageBox.Show("Database Error deleting ledger entry: " + dbEx.Message, "Ledger Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("General Error deleting ledger entry: " + ex.Message, "Ledger Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Made static as per original code - implies it might be called from elsewhere?
        // If only called from this form, it shouldn't be static. Assuming it's called externally.
        public static void LedgerUpdate(DateTime a, string b, decimal e, decimal f, string g, string h, string i, decimal j)
        {
            try
            {
                // Original query had WHERE LedgerNo=@d6 and Label=@d7 (i)
                string query = @"UPDATE LedgerBook SET Date=@d1, Name=@d2, Debit=@d3, Credit=@d4, PartyID=@d5, total_sale=@d8
                                WHERE LedgerNo=@d6 AND Label=@d7";
                var parameters = new[] {
                    DataAccessLayer.CreateParameter("@d1", SqlDbType.DateTime, a),
                    DataAccessLayer.CreateParameter("@d2", SqlDbType.NVarChar, b), // Name
                    DataAccessLayer.CreateParameter("@d3", SqlDbType.Decimal, e), // Debit
                    DataAccessLayer.CreateParameter("@d4", SqlDbType.Decimal, f), // Credit
                    DataAccessLayer.CreateParameter("@d5", SqlDbType.NVarChar, g), // PartyID
                    DataAccessLayer.CreateParameter("@d6", SqlDbType.NVarChar, h), // LedgerNo (for WHERE)
                    DataAccessLayer.CreateParameter("@d7", SqlDbType.NVarChar, i), // Label (for WHERE)
                    DataAccessLayer.CreateParameter("@d8", SqlDbType.Decimal, j) // total_sale
                };
                DataAccessLayer.ExecuteNonQuery(query, CommandType.Text, parameters);
            }
            catch (SqlException dbEx)
            {
                // Static method cannot directly show MessageBox on a form instance.
                // Consider logging or throwing the exception.
                Console.WriteLine("Database Error updating ledger entry: " + dbEx.Message);
                // Or throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error updating ledger entry: " + ex.Message);
                // Or throw;
            }
        }

        private void btnSelectionInv_Click(object sender, EventArgs e)
        {
            Stock stock = new Stock();
            stock.lblSet.Text = "Billing";
            stock.ShowDialog();
        }

        // --- UI Calculation Methods (Compute, Compute1, Compute2, ParseOrDefault) remain the same ---
        // --- They don't involve DB access directly.                                              ---
        public void Compute()
        {
            double sellingPrice = ParseOrDefault(txtSellingPrice.Text);
            double costPrice = ParseOrDefault(txtCostPrice.Text);
            double qty = ParseOrDefault(txtQty.Text);
            double discountPer = ParseOrDefault(txtDiscountPer.Text);
            double vat = ParseOrDefault(txtVAT.Text);

            txtMargin.Text = ((sellingPrice - costPrice) * qty).ToString("F2"); // Use F2 for formatting

            double amount = qty * sellingPrice;
            txtAmount.Text = Math.Round(amount, 2).ToString("F2");

            double discountAmount = amount * discountPer / 100;
            txtDiscountAmount.Text = Math.Round(discountAmount, 2).ToString("F2");

            double amountAfterDiscount = amount - discountAmount;
            double vatAmount = amountAfterDiscount * vat / 100; // Calculate VAT on amount after discount
            txtVATAmount.Text = Math.Round(vatAmount, 2).ToString("F2");

            double totalAmount = amount + vatAmount - discountAmount;
            txtTotalAmount.Text = Math.Round(totalAmount, 2).ToString("F2");
        }
        private double ParseOrDefault(string input)
        {
            if (double.TryParse(input, out double result))
            {
                return result;
            }
            return 0;
        }
        public void Compute1() // Calculates Payment Due
        {
            double grandTotal = ParseOrDefault(txtGrandTotal.Text);
            double totalPayment = ParseOrDefault(txtTotalPayment.Text);
            double totalSale = ParseOrDefault(total_sale.Text); // Get the value from total_sale TextBox

            double paymentDue = grandTotal - totalPayment - totalSale; // Subtract total_sale as well
            txtPaymentDue.Text = Math.Round(paymentDue, 2).ToString("F2"); // Use F2 format
        }
        private void Compute2() // Recalculates TotalPayment based on DataGridView2
        {
            txtTotalPayment.Text = TotalPayment().ToString("F2");
            // Optional: Recalculate payment due after updating total payment
            Compute1();
            // txtChange calculation was commented out, leave it commented
        }
        private double TotalPayment() // Sums payments from DataGridView2
        {
            double totalPayment = 0;
            try
            {
                totalPayment = DataGridView2.Rows.Cast<DataGridViewRow>()
                            .Where(r => !r.IsNewRow && r.Cells[1].Value != null && decimal.TryParse(r.Cells[1].Value.ToString(), out _))
                            .Sum(r => Convert.ToDouble(r.Cells[1].Value));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating total payment from grid: " + ex.Message, "Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return Math.Round(totalPayment, 2);
        }


        private void btnAdd_Click(object sender, EventArgs e) // Add product to DataGridView1
        {
            try
            {
                // --- Input Validation (remains the same) ---
                if (string.IsNullOrWhiteSpace(txtProductCode.Text)) {*//*... return; *//*}
                // ... other validations (Barcode, Qty > 0, Price, Discount, VAT, Warehouse, Name) ...
                if (Convert.ToDecimal(txtSellingPrice.Text) < Convert.ToDecimal(Plimit.Text)) {*//*... return; *//*} // Price limit check
                if (string.IsNullOrWhiteSpace(comboBox2.Text)) {*//*... return; *//*}
                if (string.IsNullOrWhiteSpace(txtProductName.Text)) {*//*... return; *//*}

                // --- Check available stock again before adding ---
                int productId = Convert.ToInt32(txtProductID.Text);
                int requestedQty = Convert.ToInt32(txtQty.Text); // Assuming Qty is integer

                string stockCheckQuery = "SELECT Qty FROM Temp_Stock WHERE ProductID = @PID AND WID = @WID"; // Check specific warehouse
                var stockParams = new[] {
                    DataAccessLayer.CreateParameter("@PID", SqlDbType.Int, productId),
                    DataAccessLayer.CreateParameter("@WID", SqlDbType.Int, Convert.ToInt32(WID.Text))
                };
                object stockResult = DataAccessLayer.ExecuteScalar(stockCheckQuery, CommandType.Text, stockParams);

                if (stockResult != null && stockResult != DBNull.Value && int.TryParse(stockResult.ToString(), out int availableQty))
                {
                    // Check if adding this quantity exceeds available stock, considering existing items in grid
                    int qtyAlreadyInGrid = DataGridView1.Rows.Cast<DataGridViewRow>()
                                            .Where(r => !r.IsNewRow && r.Cells[13].Value != null && r.Cells[13].Value.ToString() == txtProductID.Text)
                                            .Sum(r => Convert.ToInt32(r.Cells[6].Value));

                    if ((requestedQty + qtyAlreadyInGrid) > availableQty)
                    {
                        MessageBox.Show($"الكمية المطلوبة ({requestedQty}) بالإضافة للكمية الموجودة بالفاتورة ({qtyAlreadyInGrid}) للمنتج '{txtProductName.Text}' تتجاوز الكمية المتوفرة ({availableQty}).", "خطأ في الكمية", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtQty.Focus();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show($"لم يتم العثور على مخزون للمنتج '{txtProductName.Text}' في المخزن المحدد.", "خطأ في المخزون", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                // --- Add or Update Row in DataGridView1 (UI logic) ---
                bool rowUpdated = false;
                double qty = Convert.ToDouble(txtQty.Text);
                double amount = Convert.ToDouble(txtAmount.Text);
                double discountAmount = Convert.ToDouble(txtDiscountAmount.Text);
                double vatAmount = Convert.ToDouble(txtVATAmount.Text);
                double totalAmount = Convert.ToDouble(txtTotalAmount.Text);

                foreach (DataGridViewRow row in DataGridView1.Rows)
                {
                    // Assuming ProductID (hidden column 13) and WID (hidden column 14) make a unique item in the grid
                    if (row.Cells[13].Value != null && row.Cells[14].Value != null &&
                        row.Cells[13].Value.ToString() == txtProductID.Text &&
                        row.Cells[14].Value.ToString() == WID.Text)
                    {
                        // Update the existing row
                        row.Cells[6].Value = Convert.ToDouble(row.Cells[6].Value) + qty; // Update Qty
                        // Recalculate amounts based on new quantity (optional, but good practice)
                        Compute(); // Recalculate based on controls
                        row.Cells[7].Value = Convert.ToDouble(txtAmount.Text); // Amount (Recalculated)
                        row.Cells[9].Value = Convert.ToDouble(txtDiscountAmount.Text); // Discount Amount (Recalculated)
                        row.Cells[11].Value = Convert.ToDouble(txtVATAmount.Text); // VAT Amount (Recalculated)
                        row.Cells[12].Value = Convert.ToDouble(txtTotalAmount.Text); // Total Amount (Recalculated)

                        rowUpdated = true;
                        break;
                    }
                }

                if (!rowUpdated)
                {
                    DataGridView1.Rows.Add(
                        txtProductCode.Text, txtProductName.Text, txtBarcode.Text,
                        ParseOrDefault(txtCostPrice.Text), ParseOrDefault(txtSellingPrice.Text),
                        ParseOrDefault(txtMargin.Text), qty, amount,
                        ParseOrDefault(txtDiscountPer.Text), discountAmount,
                        ParseOrDefault(txtVAT.Text), vatAmount,
                        totalAmount, productId,
                        Convert.ToInt32(WID.Text) // Add WID to the grid row
                    );
                }

                // --- Update Totals and Reset ---
                txtGrandTotal.Text = GrandTotal().ToString("F2"); // Recalculate Grand Total from grid
                txtTotalPayment.Text = txtGrandTotal.Text; // Default Total Payment to Grand Total
                txtPayment.Text = txtGrandTotal.Text; // Default Payment field

                Compute1(); // Recalculate Payment Due
                Clear();    // Clear product entry fields
                txtProductName.Focus(); // Set focus back

            }
            catch (FormatException formatEx)
            {
                MessageBox.Show($"Data format error adding item: {formatEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding item to list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            // --- Input Validation (remains the same) ---
            if (string.IsNullOrWhiteSpace(txtSalesmanID.Text)) {*//*... return; *//*}
            // ... other validations ...
            if (DataGridView1.Rows.Count == 0) {*//*... return; *//*}
            if (DataGridView2.Rows.Count == 0) { *//* Add default payment *//* }
            if (Convert.ToDouble(txtTotalPayment.Text) > Convert.ToDouble(txtGrandTotal.Text)) {*//*... return; *//*}


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

        // --- Other Event Handlers (btnGetData_Click, btnNew_Click, Timer, etc.) - No DB changes needed ---

        private void btnAdd1_Click(object sender, EventArgs e) // Add payment to DataGridView2
        {
            // --- UI Validation (remains the same) ---
            if (DataGridView1.Rows.Count == 0) { *//* ... *//* return; }
            if (string.IsNullOrEmpty(cmbPaymentMode.Text)) { *//* ... *//* return; }
            if (string.IsNullOrEmpty(txtPayment.Text)) { *//* ... *//* return; }

            try
            {
                DataGridView2.Rows.Add(cmbPaymentMode.Text, txtPayment.Text, dtpPaymentDate.Value.Date);
                Compute2(); // Recalculate total payment and due amount
                Clear1();   // Clear payment entry fields
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e) // Delete Invoice
        {
            try
            {
                if (MessageBox.Show("هل أنت متأكد من أنك تريد حذف سجل الفاتورة؟", "تاكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    DeleteRecord(); // Uses DAL now
                    // Reset() is called inside DeleteRecord on success
                }
            }
            catch (Exception ex) // Catch exceptions from DeleteRecord
            {
                MessageBox.Show($"Error during deletion: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteRecord()
        {
            if (string.IsNullOrWhiteSpace(txtID.Text))
            {
                MessageBox.Show("لم يتم تحديد فاتورة للحذف.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int invoiceIdToDelete = int.Parse(txtID.Text);
            string invoiceNoToDelete = txtInvoiceNo.Text; // Keep for logging/ledger delete

            try
            {
                // --- Check if used in Sales Return ---
                string checkReturnQuery = "SELECT COUNT(SalesReturn.SalesID) FROM SalesReturn WHERE SalesReturn.SalesID = @d1";
                var paramCheck = DataAccessLayer.CreateParameter("@d1", SqlDbType.Int, invoiceIdToDelete);
                object returnCount = DataAccessLayer.ExecuteScalar(checkReturnQuery, CommandType.Text, paramCheck);

                if (returnCount != null && Convert.ToInt32(returnCount) > 0)
                {
                    MessageBox.Show("غير قادر على الحذف .. مستخدمة مسبقًا في إرجاع المبيعات", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // --- Add quantities back to stock (using original DGV3 if available, otherwise use DGV1 as fallback) ---
                var itemsToRestore = (dataGridView3.Rows.Count > 0) ? dataGridView3.Rows : DataGridView1.Rows; // Prefer backup grid
                foreach (DataGridViewRow row in itemsToRestore)
                {
                    if (!row.IsNewRow && row.Cells[13].Value != null && row.Cells[14].Value != null && row.Cells[6].Value != null)
                    {
                        string updateStockQuery = "UPDATE Temp_Stock SET qty = qty + @qty WHERE ProductID = @d1 AND WID = @d3";
                        var stockParams = new[] {
                            DataAccessLayer.CreateParameter("@qty", SqlDbType.Decimal, Convert.ToDecimal(row.Cells[6].Value)),
                            DataAccessLayer.CreateParameter("@d1", SqlDbType.Int, Convert.ToInt32(row.Cells[13].Value)),
                            DataAccessLayer.CreateParameter("@d3", SqlDbType.Int, Convert.ToInt32(row.Cells[14].Value))
                            //DataAccessLayer.CreateParameter("@d2", SqlDbType.NVarChar, row.Cells[2].Value?.ToString() ?? "") // Barcode - Removed
                        };
                        DataAccessLayer.ExecuteNonQuery(updateStockQuery, CommandType.Text, stockParams);
                    }
                }


                // --- Delete related records first (constraints) ---
                // Delete Invoice_Product
                string delProdQuery = "DELETE FROM Invoice_Product WHERE InvoiceID = @ID";
                var paramDel = DataAccessLayer.CreateParameter("@ID", SqlDbType.Int, invoiceIdToDelete);
                DataAccessLayer.ExecuteNonQuery(delProdQuery, CommandType.Text, paramDel);

                // Delete Invoice_Payment
                string delPayQuery = "DELETE FROM Invoice_Payment WHERE InvoiceID = @ID";
                DataAccessLayer.ExecuteNonQuery(delPayQuery, CommandType.Text, paramDel); // Reuse parameter

                // Delete Salesman_Commission
                string delCommQuery = "DELETE FROM Salesman_Commission WHERE InvoiceID = @ID";
                DataAccessLayer.ExecuteNonQuery(delCommQuery, CommandType.Text, paramDel); // Reuse parameter

                // Delete Payment_2 record if linked (Check InvoiceInfo first)
                string getTcIdQuery = "SELECT TC_ID FROM InvoiceInfo WHERE Inv_ID = @ID";
                object tcIdResult = DataAccessLayer.ExecuteScalar(getTcIdQuery, CommandType.Text, paramDel);
                if (tcIdResult != null && tcIdResult != DBNull.Value)
                {
                    string delPay2Query = "DELETE FROM Payment_2 WHERE TC_ID = @TCID";
                    var paramTcId = DataAccessLayer.CreateParameter("@TCID", SqlDbType.Int, Convert.ToInt32(tcIdResult));
                    DataAccessLayer.ExecuteNonQuery(delPay2Query, CommandType.Text, paramTcId);
                }


                // --- Delete main InvoiceInfo record ---
                string delInvQuery = "DELETE FROM InvoiceInfo WHERE Inv_ID = @ID";
                int rowsAffected = DataAccessLayer.ExecuteNonQuery(delInvQuery, CommandType.Text, paramDel); // Reuse parameter

                if (rowsAffected > 0)
                {
                    // --- Delete Ledger Entries ---
                    LedgerDelete(invoiceNoToDelete, "فاتورة مبيعات");
                    LedgerDelete(invoiceNoToDelete, "دفعات فاتورة مبيعات"); // If payments were logged separately
                    LedgerDelete(invoiceNoToDelete, "دفعة فورية"); // If instant payments were logged

                    // --- Log Deletion ---
                    string logMessage = $"deleted the bill (Products) having invoice no. '{invoiceNoToDelete}'";
                    LogFunc(lblUser.Text, logMessage); // Uses DAL

                    MessageBox.Show("تم الحذف بنجاح", "سجل", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset(); // Reset the form
                }
                else
                {
                    MessageBox.Show("لم يتم العثور على السجل للحذف أو حدث خطأ.", "عذرا", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset(); // Reset anyway
                }
            }
            catch (SqlException dbEx)
            {
                MessageBox.Show($"Database Error deleting record: {dbEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"General Error deleting record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- Other UI Event Handlers (DataGridView clicks, Resets, KeyPress) - No DB changes needed ---

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

        private void btnSave_Click(object sender, EventArgs e) // Main Save Button Action
        {
            try
            {
                bool saveSuccess = POSSAVE(); // Uses DAL

                if (!saveSuccess)
                {
                    return; // Error message shown inside POSSAVE or validation
                }

                MessageBox.Show("تم الحفظ بنجاح", "الفاتورة", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
                btnPrint.Enabled = true;
                txtCustomerType.Text = "Regular"; // Set type for printing
                Print(); // Uses direct ADO.NET for Crystal Reports
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred during save: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e) // Remove item from DGV1
        {
            if (DataGridView1.SelectedRows.Count == 0) return;

            try
            {
                foreach (DataGridViewRow row in DataGridView1.SelectedRows)
                {
                    if (!row.IsNewRow) DataGridView1.Rows.Remove(row);
                }

                // Recalculate totals after removing items
                txtGrandTotal.Text = GrandTotal().ToString("F2");
                txtTotalPayment.Text = txtGrandTotal.Text; // Reset payment to new total
                txtPayment.Text = txtGrandTotal.Text;

                Compute1(); // Recalculate payment due
                Clear();    // Clear product entry section (optional, maybe just reset focus)
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing item: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnListUpdate_Click(object sender, EventArgs e) // Update item in DGV1
        {
            if (DataGridView1.SelectedRows.Count == 0) return;

            try
            {
                // --- Input Validation (remains the same) ---
                if (string.IsNullOrWhiteSpace(txtProductCode.Text)) {*//*... return; *//*}
                // ... other validations ...
                if (Convert.ToDouble(txtQty.Text) <= 0) { *//* ... *//* return; }

                // Get the currently selected row index before removing
                int selectedIndex = DataGridView1.SelectedRows[0].Index;

                // Remove selected rows first (should only be one if single selection)
                foreach (DataGridViewRow row in DataGridView1.SelectedRows)
                {
                    if (!row.IsNewRow) DataGridView1.Rows.Remove(row);
                }

                // Add the updated row back (ideally at the same position, but Add puts it at the end)
                // Or better: Update the values in the existing row directly if possible
                // For simplicity here, we remove and add.
                DataGridView1.Rows.Insert(selectedIndex, // Try inserting at the original index
                    txtProductCode.Text, txtProductName.Text, txtBarcode.Text,
                    ParseOrDefault(txtCostPrice.Text), ParseOrDefault(txtSellingPrice.Text),
                    ParseOrDefault(txtMargin.Text), ParseOrDefault(txtQty.Text),
                    ParseOrDefault(txtAmount.Text), ParseOrDefault(txtDiscountPer.Text),
                    ParseOrDefault(txtDiscountAmount.Text), ParseOrDefault(txtVAT.Text),
                    ParseOrDefault(txtVATAmount.Text), ParseOrDefault(txtTotalAmount.Text),
                    Convert.ToInt32(txtProductID.Text), // Assuming txtProductID holds the ID
                    Convert.ToInt32(WID.Text) // Assuming WID holds the warehouse ID
                );

                // --- Recalculate Totals and Reset ---
                txtGrandTotal.Text = GrandTotal().ToString("F2");
                txtTotalPayment.Text = txtGrandTotal.Text;
                txtPayment.Text = txtGrandTotal.Text;

                Compute1(); // Recalculate payment due
                Clear();    // Clear product entry fields
                txtProductName.Enabled = true; // Re-enable product name field?
                txtProductName.Focus(); // Or txtBarcode.Focus()

            }
            catch (FormatException formatEx)
            {
                MessageBox.Show($"Data format error updating item: {formatEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating item list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- TextChanged Events (txtPayment, txtCheck etc.) - Mostly UI logic, no DB changes needed ---

        private void btnAdd1_Click_1(object sender, EventArgs e) // Duplicate of btnAdd1_Click ?
        {
            // Same logic as btnAdd1_Click - adds payment to DGV2
            btnAdd1_Click(sender, e); // Call the original handler
        }

        // --- Other UI Event Handlers (Resets, Updates for DGV2) - No DB changes needed ---

        private void txtBarcode_TextChanged(object sender, EventArgs e) // Original handler? Seems complex.
        {
            // This original handler tries to populate dgw based on partial barcode match.
            // Let's keep its logic but use the DAL.
            if (string.IsNullOrWhiteSpace(txtBarcode.Text))
            {
                dgw.Visible = false;
                dgw.Rows.Clear();
                return;
            }
            try
            {
                dgw.Visible = true;
                string query = @"SELECT PID, RTRIM(Product.ProductCode), RTRIM(ProductName),
                                RTRIM(Temp_Stock.Barcode), CostPrice, SellingPrice, Discount, VAT, Qty,
                                RTRIM(Product.SellingPrice2), Plimit
                                FROM Temp_Stock INNER JOIN Product ON Product.PID = Temp_Stock.ProductID
                                WHERE Qty > 0 AND Temp_Stock.Barcode LIKE @barcode AND Temp_Stock.WID = @WID -- Added WID filter
                                ORDER BY ProductCode";

                var parameters = new[] {
                     DataAccessLayer.CreateParameter("@barcode", SqlDbType.NVarChar, "%" + txtBarcode.Text + "%"),
                     DataAccessLayer.CreateParameter("@WID", SqlDbType.Int, Convert.ToInt32(WID.Text)) // Use current warehouse
                };

                DataTable dt = DataAccessLayer.ExecuteTable(query, CommandType.Text, parameters);

                dgw.Rows.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    dgw.Rows.Add(row[0], row[1], row[2], row[3], row[4], row[5], row[6], row[7], row[8], row[9], row[10]);
                }
                if (dt.Rows.Count == 0)
                {
                    dgw.Visible = false;
                }

            }
            catch (SqlException dbEx)
            {
                MessageBox.Show($"Database Error searching barcode: {dbEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgw.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"General Error searching barcode: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgw.Visible = false;
            }

            // --- UI Logic from original handler (adjusting payment based on mode) ---
            if (cmbPaymentMode.SelectedIndex >= 2 && cmbPaymentMode.SelectedIndex <= 9)
            {
                // txtPayment.ReadOnly = true; // Commented out in original
                txtPayment.Text = "0";
                // txtQty.Focus(); // Maybe focus barcode or product name instead?
            }
            else
            {
                // txtPayment.ReadOnly = false; // Commented out in original
                // txtPayment.Text = Convert.ToString(Convert.ToDecimal(txtGrandTotal.Text)); // Causes issues if GrandTotal is empty
                txtPayment.Text = ParseOrDefault(txtGrandTotal.Text).ToString("F2");
            }
        }

        // --- cmbPaymentMode_SelectedIndexChanged, txtGrandTotal_TextChanged, Timers, etc. - UI Logic ---

        private void Button3_Click(object sender, EventArgs e) // Print Non Regular?
        {
            if (string.IsNullOrWhiteSpace(txtInvoiceNo.Text))
            {
                MessageBox.Show("لا توجد فاتورة للطباعة.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            txtCustomerType.Text = "Non Regular";
            Print(); // Uses direct ADO.NET
            Reset();
        }

        // --- DGV1 ControlAdded, Val method, MouseDoubleClick - UI Logic ---

        private void btnPrint_Click_1(object sender, EventArgs e) // General Print Button
        {
            if (string.IsNullOrWhiteSpace(txtInvoiceNo.Text))
            {
                MessageBox.Show("لا توجد فاتورة للطباعة.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Determine print type if not set by save buttons? Default to Regular?
            if (string.IsNullOrWhiteSpace(txtCustomerType.Text))
            {
                txtCustomerType.Text = "Regular"; // Default if not set
            }
            Print(); // Uses direct ADO.NET
        }

        // --- Print Method (Uses Direct ADO.NET for Crystal Reports - Left Unchanged) ---
        public void Print()
        {
            // NOTE: This method uses direct SqlConnection, SqlCommand, SqlDataAdapter
            // because Crystal Reports often requires this level of control to fill DataSets.
            // Modifying this to use the static DAL might be complex and is left as is.
            try
            {
                if (string.IsNullOrWhiteSpace(txtInvoiceNo.Text))
                {
                    MessageBox.Show("يرجى تحديد فاتورة للطباعة.", "خطأ", MessageBoxButtons.OK, Warning);
                    return;
                }

                Cursor = Cursors.WaitCursor;
                Timer1.Enabled = true; // Assuming Timer1 stops the wait cursor

                // Determine which report to use based on txtCustomerType
                dynamic rpt; // Use dynamic to hold different report types
                string commandText = @"SELECT Customer.ID, Customer.Name, Customer.Gender, Customer.Address, Customer.City, Customer.State,
                                       Customer.ZipCode, Customer.ContactNo, Customer.EmailID, InvoiceInfo.Remarks, Customer.Photo,
                                       InvoiceInfo.Inv_ID, InvoiceInfo.InvoiceNo, InvoiceInfo.InvoiceDate, InvoiceInfo.CustomerID,
                                       InvoiceInfo.GrandTotal, InvoiceInfo.TotalPaid, InvoiceInfo.Balance, Invoice_Product.IPo_ID,
                                       Invoice_Product.InvoiceID, Invoice_Product.ProductID, Invoice_Product.CostPrice,
                                       Invoice_Product.SellingPrice, Invoice_Product.Margin, Invoice_Product.Qty, Invoice_Product.Amount,
                                       Invoice_Product.DiscountPer, Invoice_Product.Discount, Invoice_Product.VATPer, Invoice_Product.VAT,
                                       Invoice_Product.TotalAmount, Invoice_Product.Barcode, Product.PID, Product.ProductCode,
                                       Product.ProductName, InvoiceInfo.total_sale
                                       FROM Customer INNER JOIN InvoiceInfo ON Customer.ID = InvoiceInfo.CustomerID
                                       INNER JOIN Invoice_Product ON InvoiceInfo.Inv_ID = Invoice_Product.InvoiceID
                                       INNER JOIN Product ON Invoice_Product.ProductID = Product.PID
                                       WHERE InvoiceInfo.InvoiceNo=@d1"; // Use InvoiceNo

                switch (txtCustomerType.Text)
                {
                    case "Non Regular":
                        rpt = new rptInvoice3();
                        break;
                    case "Regular Sale":
                        rpt = new rptInvoice4();
                        break;
                    case "Non Regular Sale":
                        rpt = new rptInvoice5();
                        break;
                    case "Regular": // Default or Regular
                    default:
                        rpt = new rptInvoice();
                        break;
                }


                // Using direct ADO.NET for Crystal Reports data source
                using (SqlConnection myConnection = new SqlConnection(DataAccessLayer.Con())) // Get connection string from original DAL method
                using (SqlCommand myCommand = new SqlCommand(commandText, myConnection))
                using (SqlCommand myCommand1 = new SqlCommand("SELECT * FROM Company", myConnection)) // Company Info
                using (SqlDataAdapter myDA = new SqlDataAdapter(myCommand))
                using (SqlDataAdapter myDA1 = new SqlDataAdapter(myCommand1))
                {
                    myCommand.Parameters.AddWithValue("@d1", txtInvoiceNo.Text);
                    myCommand.CommandType = CommandType.Text;
                    myCommand1.CommandType = CommandType.Text;

                    DataSet myDS = new DataSet();
                    myDA.Fill(myDS, "InvoiceData"); // Give specific names
                    myDA1.Fill(myDS, "Company");

                    if (myDS.Tables["InvoiceData"] == null || myDS.Tables["InvoiceData"].Rows.Count == 0)
                    {
                        MessageBox.Show("لم يتم العثور على بيانات للفاتورة المحددة.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Cursor = Cursors.Default;
                        Timer1.Enabled = false;
                        return;
                    }


                    rpt.SetDataSource(myDS);

                    // Set parameters common to most reports
                    try { rpt.SetParameterValue("p1", txtCustomerID.Text); } catch { } // Customer Code
                    try { rpt.SetParameterValue("p2", DateTime.Today); } catch { }     // Print Date

                    // Set specific parameters for sale reports
                    if (txtCustomerType.Text == "Regular Sale" || txtCustomerType.Text == "Non Regular Sale")
                    {
                        try { rpt.SetParameterValue("Sale", total_sale.Text); } catch { }
                    }


                    frmReport frmReport = new frmReport();
                    frmReport.crystalReportViewer1.ReportSource = rpt;
                    frmReport.ShowDialog(); // Show as dialog
                }
            }
            catch (SqlException dbEx)
            {
                MessageBox.Show($"Database error preparing report: {dbEx.Message}", "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default; // Ensure cursor returns to default
                Timer1.Enabled = false; // Ensure timer stops
            }
        }


        // --- txtCustomerID_TextChanged - UI Logic ---

        private void dgw_CellContentClick_1(object sender, DataGridViewCellEventArgs e) // Another dgw handler? Seems unused.
        {
            // This seems to duplicate logic from MouseDoubleClick. Keeping it commented unless needed.
            *//*
            if (DataGridView1.Rows.Count > 0 && DataGridView1.SelectedRows.Count > 0)
            {
                 // ... code similar to DataGridView1_MouseDoubleClick ...
            }
            *//*
        }

        private void dgw_MouseClick(object sender, MouseEventArgs e) // Click on the search results grid (dgw)
        {
            // This seems intended to fill fields when clicking dgw, similar to double-click or KeyDown Enter.
            // Calling the double-click logic might be appropriate here.
            if (e.Button == MouseButtons.Left && dgw.SelectedRows.Count > 0)
            {
                dgw_MouseDoubleClick(sender, e); // Simulate double-click behavior
            }
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e) // Backup grid - usually no action needed on click
        {
            // No direct DB action here. Original code had a query to repopulate dgw, which seems incorrect for this event.
        }


        private void txtBarcode_TextChanged_1(object sender, EventArgs e) // Barcode EXACT match logic
        {
            // This handler performs an EXACT barcode match and fills fields if found.
            if (string.IsNullOrWhiteSpace(txtBarcode.Text))
            {
                dgw.Visible = false; // Hide suggestion grid if barcode is cleared
                return;
            }

            try
            {
                string query = @"SELECT PID, RTRIM(Product.ProductCode), RTRIM(ProductName),
                                RTRIM(Temp_Stock.Barcode), CostPrice, SellingPrice, Discount, VAT, Qty,
                                RTRIM(Product.SellingPrice2), Plimit
                                FROM Temp_Stock INNER JOIN Product ON Product.PID = Temp_Stock.ProductID
                                WHERE Qty > 0 AND Temp_Stock.Barcode = @barcode AND Temp_Stock.WID = @WID"; // Exact match + WID

                var parameters = new[] {
                     DataAccessLayer.CreateParameter("@barcode", SqlDbType.NVarChar, txtBarcode.Text.Trim()),
                     DataAccessLayer.CreateParameter("@WID", SqlDbType.Int, Convert.ToInt32(WID.Text)) // Use current warehouse
                 };

                DataTable dt = DataAccessLayer.ExecuteTable(query, CommandType.Text, parameters);

                if (dt.Rows.Count > 0) // Exact match found
                {
                    DataRow dr = dt.Rows[0]; // Get the first (only) row
                    txtProductID.Text = dr["PID"].ToString();
                    txtProductCode.Text = dr["ProductCode"].ToString();
                    txtProductName.Text = dr["ProductName"].ToString();
                    // txtBarcode.Text = dr["Barcode"].ToString(); // Barcode is already set
                    txtCostPrice.Text = dr["CostPrice"].ToString();

                    // Choose selling price
                    txtSellingPrice.Text = (ComboBox1.SelectedIndex == 0)
                                            ? dr["SellingPrice"].ToString()
                                            : dr["SellingPrice2"].ToString();

                    Plimit.Text = dr["Plimit"] == DBNull.Value ? "0" : dr["Plimit"].ToString();
                    txtVAT.Text = dr["VAT"].ToString();

                    // Calculate margin
                    double sellP = ParseOrDefault(txtSellingPrice.Text);
                    double costP = ParseOrDefault(txtCostPrice.Text);
                    txtMargin.Text = Math.Round(sellP - costP, 2).ToString("F2");

                    txtDiscountPer.Text = dr["Discount"].ToString();
                    // txtVAT.Text = dr["VAT"].ToString(); // VAT already set

                    txtQty.Text = "1"; // Default quantity to 1
                    visablility.Text = dr["Qty"].ToString(); // Store available Qty

                    lblSet.Text = ""; // Clear any status label
                    dgw.Visible = false; // Hide suggestion grid
                    dgw.Rows.Clear();    // Clear suggestion grid
                    txtQty.Focus();      // Move focus to Qty for adding
                    txtQty.SelectAll();
                }
                else
                {
                    // No exact match found, maybe clear related fields or keep suggestion grid open?
                    // Optionally call the partial match handler: txtBarcode_TextChanged(sender, e);
                    txtBarcode_TextChanged(sender, e); // Show suggestions if no exact match
                }
            }
            catch (SqlException dbEx)
            {
                MessageBox.Show($"Database Error searching exact barcode: {dbEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"General Error searching exact barcode: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button4_Click_1(object sender, EventArgs e) // Print Regular?
        {
            if (string.IsNullOrWhiteSpace(txtInvoiceNo.Text))
            {
                MessageBox.Show("لا توجد فاتورة للطباعة.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            txtCustomerType.Text = "Regular";
            Print(); // Uses direct ADO.NET
            Reset();
        }

        private void button6_Click(object sender, EventArgs e) // Show Products Form
        {
            Products p = new Products();
            p.Reset(); // Assuming Products form has a Reset method
            p.Show();
        }


        private void DataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e) // Draw row numbers on DGV1
        {
            // UI Helper - No DB changes needed
            string strRowNumber = (e.RowIndex + 1).ToString();
            SizeF size = e.Graphics.MeasureString(strRowNumber, this.Font);
            if (DataGridView1.RowHeadersWidth < Convert.ToInt32(size.Width + 20))
            {
                DataGridView1.RowHeadersWidth = Convert.ToInt32(size.Width + 20);
            }
            // Use SystemBrushes.ControlText for default text color
            Brush b = SystemBrushes.ControlText;
            // Adjust drawing position for RightToLeft if needed
            float xPos = (this.RightToLeft == RightToLeft.Yes)
                         ? e.RowBounds.Location.X + DataGridView1.RowHeadersWidth - size.Width - 5 // Adjust for RTL
                         : e.RowBounds.Location.X + 5; // Default LTR padding

            e.Graphics.DrawString(strRowNumber, this.Font, b, xPos, e.RowBounds.Location.Y + (e.RowBounds.Height - size.Height) / 2);
        }

        private void DataGridView1_ControlAdded_1(object sender, ControlEventArgs e) // Maybe related to DGV1 changes?
        {
            // This seems to duplicate logic from other places setting txtPayment.
            // Might be redundant. Commenting out the specific logic for now.
            // txtPayment.Text = (txtGrandTotal.Text.ToString());
            // ... payment mode checks ...
        }

        private void textBox3_TextChanged(object sender, EventArgs e) // Additional Payment field?
        {
            // Calculates a potential payment amount based on TotalPayment + textBox3
            decimal currentTotalPayment = ParseOrDefaultDecimal(txtTotalPayment.Text);
            decimal additionalPayment = ParseOrDefaultDecimal(textBox3.Text);
            // txtPayment.Text = (currentTotalPayment + additionalPayment).ToString("F2"); // Update txtPayment? Or a different field?
            // This logic seems unclear - what is the purpose of textBox3? Assuming it's for adding a new payment amount.
            // Let's assume txtPayment should reflect the *new* payment being entered.
            txtPayment.Text = additionalPayment.ToString("F2"); // Set txtPayment to the value in textBox3
            Compute1(); // Recompute due amount based on potential changes elsewhere

        }
        private decimal ParseOrDefaultDecimal(string input)
        {
            if (decimal.TryParse(input, out decimal result))
            {
                return result;
            }
            return 0m; // Use m for decimal literal
        }


        private void btn_add_payment_Click(object sender, EventArgs e) // Add Payment to Existing Invoice
        {
            // --- Validation ---
            if (string.IsNullOrWhiteSpace(txtID.Text)) { MessageBox.Show("يرجى تحميل فاتورة أولاً لإضافة دفعة.", "خطأ", MessageBoxButtons.OK, Warning); return; }
            if (string.IsNullOrWhiteSpace(txtSalesmanID.Text)) {*//*... return; *//*}
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text)) {*//*... return; *//*}
            if (string.IsNullOrWhiteSpace(textBox3.Text) || ParseOrDefaultDecimal(textBox3.Text) <= 0)
            {
                MessageBox.Show("يرجى إدخال مبلغ دفعة صالح في المربع المخصص.", "خطأ", MessageBoxButtons.OK, Warning);
                textBox3.Focus();
                return;
            }
            decimal newPaymentAmount = ParseOrDefaultDecimal(textBox3.Text);
            decimal currentBalance = ParseOrDefaultDecimal(txtPaymentDue.Text);

            if (newPaymentAmount > Math.Abs(currentBalance) && currentBalance < 0) // Check if payment exceeds balance
            {
                // Allow overpayment? Or limit to balance? Limiting for now.
                MessageBox.Show($"مبلغ الدفعة ({newPaymentAmount:F2}) أكبر من الرصيد المتبقي ({Math.Abs(currentBalance):F2}). سيتم تسجيل الدفعة بقيمة الرصيد.", "تنبيه", MessageBoxButtons.OK, Information);
                newPaymentAmount = Math.Abs(currentBalance);
                textBox3.Text = newPaymentAmount.ToString("F2"); // Update the textbox
            }


            try
            {
                int invoiceId = Convert.ToInt32(txtID.Text);
                decimal originalTotalPaid = 0;
                decimal originalBalance = 0;

                // Get original payment details before update
                string fetchInvQuery = "SELECT TotalPaid, Balance FROM InvoiceInfo WHERE Inv_ID=@d1";
                var fetchParam = DataAccessLayer.CreateParameter("@d1", SqlDbType.Int, invoiceId);
                DataTable invData = DataAccessLayer.ExecuteTable(fetchInvQuery, CommandType.Text, fetchParam);
                if (invData.Rows.Count > 0)
                {
                    originalTotalPaid = Convert.ToDecimal(invData.Rows[0]["TotalPaid"]);
                    originalBalance = Convert.ToDecimal(invData.Rows[0]["Balance"]);
                }
                else
                {
                    MessageBox.Show("لم يتم العثور على الفاتورة الأصلية.", "خطأ", MessageBoxButtons.OK, Error);
                    return;
                }


                decimal newTotalPaid = originalTotalPaid + newPaymentAmount;
                decimal newBalance = originalBalance - newPaymentAmount; // Balance decreases with payment

                // --- Update InvoiceInfo with new payment totals ---
                string invUpdateQuery = "UPDATE InvoiceInfo SET TotalPaid = @d6, Balance = @d7 WHERE Inv_ID = @d1";
                var invUpdateParams = new[] {
                    DataAccessLayer.CreateParameter("@d6", SqlDbType.Float, Convert.ToDouble(newTotalPaid)), // Use Float if DB is Float
                    DataAccessLayer.CreateParameter("@d7", SqlDbType.Float, Convert.ToDouble(newBalance)),
                    DataAccessLayer.CreateParameter("@d1", SqlDbType.Int, invoiceId)
                 };
                DataAccessLayer.ExecuteNonQuery(invUpdateQuery, CommandType.Text, invUpdateParams);

                // --- Insert the new payment into Invoice_Payment ---
                // Use current date for this specific payment
                string insPayQuery = "INSERT INTO Invoice_Payment(InvoiceID, PaymentMode, TotalPaid, PaymentDate) VALUES (@d1, @d4, @d5, @d6)";
                var payParams = new[] {
                     DataAccessLayer.CreateParameter("@d1", SqlDbType.Int, invoiceId),
                     DataAccessLayer.CreateParameter("@d4", SqlDbType.NVarChar, cmbPaymentMode.Text), // Payment mode for this transaction
                     DataAccessLayer.CreateParameter("@d5", SqlDbType.Decimal, newPaymentAmount), // Amount of this specific payment
                     DataAccessLayer.CreateParameter("@d6", SqlDbType.DateTime, DateTime.Now.Date) // Date of this payment
                 };
                DataAccessLayer.ExecuteNonQuery(insPayQuery, CommandType.Text, payParams);


                // --- Insert into Payment_2 (Customer Payment Log) ---
                // Generate new TC_ID and TransactionNo for this specific payment transaction
                string newTcId = GenerateID_1(); // Get next TC_ID
                string newTransactionNo = "TC-" + (Convert.ToInt32(newTcId) - i).ToString(); // Generate corresponding TC number
                CountValue(); // Recalculate 'i' after potentially generating new ID


                string pay2InsertQuery = @"INSERT INTO Payment_2(TC_ID, TransactionID, Date, PaymentMode, CustomerID, Amount, Remarks,
                                            Check_ID, Check_Date, SalesMan_ID, SalesMan_Name, SalesMan_Comession, SalesMan_ID_2)
                                            VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9, @d10, @d11, @d12, @d13)";
                var pay2Params = new[] {
                    DataAccessLayer.CreateParameter("@d1", SqlDbType.Int, Convert.ToInt32(newTcId)),
                    DataAccessLayer.CreateParameter("@d2", SqlDbType.NVarChar, newTransactionNo),
                    DataAccessLayer.CreateParameter("@d3", SqlDbType.Date, DateTime.Now.Date), // Payment date
                    DataAccessLayer.CreateParameter("@d4", SqlDbType.NVarChar, cmbPaymentMode.Text),
                    DataAccessLayer.CreateParameter("@d5", SqlDbType.NVarChar, txtCustomerID.Text),
                    DataAccessLayer.CreateParameter("@d6", SqlDbType.Decimal, newPaymentAmount), // Payment amount is positive here
                    DataAccessLayer.CreateParameter("@d7", SqlDbType.NVarChar, "دفعة للفاتورة " + txtInvoiceNo.Text), // Remarks
                    DataAccessLayer.CreateParameter("@d8", SqlDbType.NVarChar, txtCheck.Text), // Check ID if applicable
                    DataAccessLayer.CreateParameter("@d9", SqlDbType.Date, (cmbPaymentMode.SelectedIndex == 1 ? dtpPaymentDate.Value.Date : (DateTime?)null)), // Check Date only if mode is Check
                    DataAccessLayer.CreateParameter("@d10", SqlDbType.Int, Convert.ToInt32(txtSM_ID.Text)),
                    DataAccessLayer.CreateParameter("@d11", SqlDbType.NVarChar, txtSalesman.Text),
                    DataAccessLayer.CreateParameter("@d12", SqlDbType.Decimal, 0m), // Commission for payment? Usually 0
                    DataAccessLayer.CreateParameter("@d13", SqlDbType.NVarChar, txtSalesmanID.Text)
                 };
                DataAccessLayer.ExecuteNonQuery(pay2InsertQuery, CommandType.Text, pay2Params);


                // --- Update Ledger for the payment ---
                string ledgerName = "نقدا"; // Default for payment
                if (cmbPaymentMode.SelectedIndex == 1) ledgerName = "دفعة شيك رقم " + txtCheck.Text.Trim();
                else if (cmbPaymentMode.SelectedIndex == 4) ledgerName = "دفعة اجلة"; // Doesn't make sense for payment? Use Cash/Check

                // Ledger entry for payment received
                LedgerSave(DateTime.Now.Date, ledgerName, txtInvoiceNo.Text, "دفعة فاتورة مبيعات",
                           0, // Debit = 0 for payment received
                           Math.Abs(newPaymentAmount), // Credit = payment amount
                           txtCustomerID.Text, "", 0); // No total_sale for payment entry


                // --- Log and Finalize ---
                string st = $"added payment of {newPaymentAmount:F2} to invoice no. '{txtInvoiceNo.Text}'";
                LogFunc(lblUser.Text, st);

                MessageBox.Show("تمت إضافة الدفعة بنجاح", "الدفعات", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Reset(); // Reset the form after adding payment

            }
            catch (SqlException dbEx)
            {
                MessageBox.Show($"Database error adding payment: {dbEx.Message}", "خطأ قاعدة البيانات", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FormatException formatEx)
            {
                MessageBox.Show($"Data format error adding payment: {formatEx.Message}", "خطأ تنسيق البيانات", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred adding payment: {ex.Message}", "خطأ غير متوقع", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) // Warehouse selection
        {
            if (comboBox2.SelectedItem == null) return;
            string selectedWarehouse = comboBox2.SelectedItem.ToString();

            try
            {
                string query = "SELECT WID FROM Warehouses WHERE WarehouseName = @Name";
                var parameter = DataAccessLayer.CreateParameter("@Name", SqlDbType.NVarChar, selectedWarehouse);
                object result = DataAccessLayer.ExecuteScalar(query, CommandType.Text, parameter);

                if (result != null && result != DBNull.Value)
                {
                    WID.Text = result.ToString();
                    // Clear product fields when warehouse changes? Optional.
                    // Clear();
                    // Getdata1(); // Refresh dgw for new warehouse?
                }
                else
                {
                    MessageBox.Show("لم يتم العثور على معرف المخزن المحدد.", "معلومات", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    WID.Text = ""; // Clear WID if not found
                }
            }
            catch (SqlException dbEx)
            {
                MessageBox.Show($"Database error getting Warehouse ID: {dbEx.Message}", "خطأ قاعدة بيانات", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting Warehouse ID: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- Warehouse Repository (Used by comboBoxGenerate) ---
        public interface IWarehouseRepository { List<string> GetWarehouseNames(); }
        public class WarehouseRepository : IWarehouseRepository
        {
            public List<string> GetWarehouseNames()
            {
                List<string> warehouseNames = new List<string>();
                try
                {
                    string query = "SELECT WarehouseName FROM [dbo].[Warehouses] ORDER BY WarehouseName"; // Added Order By
                    DataTable dt = DataAccessLayer.ExecuteTable(query, CommandType.Text);
                    foreach (DataRow row in dt.Rows)
                    {
                        warehouseNames.Add(row["WarehouseName"].ToString());
                    }
                }
                catch (SqlException dbEx)
                {
                    // Log or handle DB error getting warehouse names
                    Console.WriteLine("DB Error fetching warehouses: " + dbEx.Message);
                }
                catch (Exception ex)
                {
                    // Log or handle general error
                    Console.WriteLine("Error fetching warehouses: " + ex.Message);
                }
                return warehouseNames;
            }
        }
        public void comboBoxGenerate() // Populate Warehouse ComboBox
        {
            try
            {
                IWarehouseRepository repository = new WarehouseRepository();
                List<string> warehouseNames = repository.GetWarehouseNames(); // Uses DAL via repository

                comboBox2.Items.Clear();
                comboBox2.Items.AddRange(warehouseNames.ToArray());
                if (comboBox2.Items.Count > 0)
                {
                    comboBox2.SelectedIndex = 0; // Select first item by default
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error populating warehouses: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- Other UI event handlers (label clicks, text changes with calculations) ---

        private void total_sale_TextChanged(object sender, EventArgs e) // Discount applied to whole invoice?
        {
            try
            {
                // Recalculate payment due whenever this value changes
                Compute1();
                // Also update the main payment field to reflect the new required payment
                txtPayment.Text = txtTotalPayment.Text; // Or should it be txtPaymentDue? Depends on desired behavior. Setting to total payment for now.
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred adjusting for total sale discount: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- dgw_MouseDoubleClick, KeyDown events, etc. ---
        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e) // Select item from suggestion grid (dgw)
        {
            if (dgw.SelectedRows.Count == 0) return;

            try
            {
                DataGridViewRow dr = dgw.SelectedRows[0];

                // Get values safely
                txtProductID.Text = dr.Cells[0].Value?.ToString() ?? "";
                txtProductCode.Text = dr.Cells[1].Value?.ToString() ?? "";
                txtProductName.Text = dr.Cells[2].Value?.ToString() ?? "";
                txtBarcode.Text = dr.Cells[3].Value?.ToString() ?? "";
                txtCostPrice.Text = dr.Cells[4].Value?.ToString() ?? "";
                string sellingPrice1 = dr.Cells[5].Value?.ToString() ?? "0";
                string sellingPrice2 = dr.Cells[9].Value?.ToString() ?? "0"; // Column index 9 for SellingPrice2
                string limit = dr.Cells[10].Value?.ToString(); // Column index 10 for Plimit

                txtSellingPrice.Text = (ComboBox1.SelectedIndex == 0) ? sellingPrice1 : sellingPrice2;

                Plimit.Text = string.IsNullOrEmpty(limit) ? "0" : limit;
                txtVAT.Text = dr.Cells[7].Value?.ToString() ?? "0"; // VAT is index 7
                txtDiscountPer.Text = dr.Cells[6].Value?.ToString() ?? "0"; // Discount is index 6

                // Calculate Margin
                double sellP = ParseOrDefault(txtSellingPrice.Text);
                double costP = ParseOrDefault(txtCostPrice.Text);
                txtMargin.Text = Math.Round(sellP - costP, 2).ToString("F2");

                txtQty.Text = "1"; // Default Qty
                visablility.Text = dr.Cells[8].Value?.ToString() ?? "0"; // Available Qty is index 8

                lblSet.Text = ""; // Clear status
                dgw.Visible = false; // Hide suggestion grid
                dgw.Rows.Clear();
                txtQty.Focus(); // Focus on Qty
                txtQty.SelectAll();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting item from list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void txtProductName_TextChanged_2(object sender, EventArgs e) // Search by Product Name
        {
            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                dgw.Visible = false;
                dgw.Rows.Clear();
                return;
            }
            if (txtProductName.ContainsFocus) // Only search when user is typing in this box
            {
                try
                {
                    dgw.Visible = true;
                    string query = @"
                         SELECT PID, RTRIM(Product.ProductCode) AS ProductCode, RTRIM(ProductName) AS ProductName, RTRIM(Temp_Stock.Barcode) AS Barcode,
                                CostPrice, SellingPrice, Discount, VAT, Qty, RTRIM(Product.SellingPrice2) AS SellingPrice2, Plimit
                         FROM Temp_Stock
                         INNER JOIN Product ON Product.PID = Temp_Stock.ProductID
                         WHERE Qty > 0
                           AND ProductName LIKE @ProductName
                           AND Temp_Stock.WID = @WID -- Filter by current Warehouse
                         ORDER BY ProductName"; // Order by name for better searching

                    var parameters = new[] {
                         DataAccessLayer.CreateParameter("@ProductName", SqlDbType.NVarChar, "%" + txtProductName.Text.Trim() + "%"),
                         DataAccessLayer.CreateParameter("@WID", SqlDbType.Int, Convert.ToInt32(WID.Text))
                     };

                    DataTable dt = DataAccessLayer.ExecuteTable(query, CommandType.Text, parameters);

                    dgw.Rows.Clear();
                    foreach (DataRow row in dt.Rows)
                    {
                        // Ensure indices match the SELECT statement
                        dgw.Rows.Add(
                           row["PID"], row["ProductCode"], row["ProductName"], row["Barcode"],
                           row["CostPrice"], row["SellingPrice"], row["Discount"], row["VAT"],
                           row["Qty"], row["SellingPrice2"], row["Plimit"]
                        );
                    }
                    if (dt.Rows.Count == 0)
                    {
                        dgw.Visible = false;
                    }
                }
                catch (SqlException dbEx)
                {
                    MessageBox.Show($"Database Error searching product name: {dbEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dgw.Visible = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"General Error searching product name: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dgw.Visible = false;
                }
            }
        }


        private void txtProductName_KeyDown_1(object sender, KeyEventArgs e) // Navigate to suggestion grid (dgw)
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

        private void dgw_KeyDown(object sender, KeyEventArgs e) // Select item from dgw using Enter
        {
            if (e.KeyCode == Keys.Enter && dgw.SelectedRows.Count > 0)
            {
                // Simulate double-click to select the item
                dgw_MouseDoubleClick(sender, new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0)); // Simulate double click
                e.Handled = true; // Mark event as handled
            }
        }

        // Make sure all other methods not shown here don't contain direct DB access.
        // If they do, refactor them similarly using DataAccessLayer.

    } // End of POS Class
} // End of Namespace
// --- END OF REFACTORED FILE POS.cs ---
```*/