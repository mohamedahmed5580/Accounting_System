using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Accounting_System.Pymentinvoice;
using System.Transactions;

namespace Accounting_System
{
    public partial class SalesReturn : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());


        private static SalesReturn _instance;
        public static SalesReturn instance;
        public static SalesReturn Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new SalesReturn();
                }
                return _instance;
            }
        }

        public SalesReturn()
        { 
            InitializeComponent();
            txtReturnQty.TextChanged += new EventHandler(txtRetuenQty_TextChanged);
            instance = this;
        }
        private void SalesReturn_Load(object sender, EventArgs e)
        {
        }
        private void txtSRNO_TextChanged(object sender, EventArgs e)
        {
        }
        // Install-Package Microsoft.VisualBasic
        private string str;
        private string st;

        private string GenerateID()
        {
            string value = "0000";
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    // Fetch the latest ID from the database
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 SR_ID FROM SalesReturn ORDER BY SR_ID DESC", con))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                value = rdr["SR_ID"].ToString();
                            }
                        }
                    }

                    // Increase the ID by 1
                    int intValue = int.Parse(value) + 1;
                    value = intValue.ToString("D4"); // Format the integer as a 4-digit number, padded with zeros
                }
                catch (Exception ex)
                {
                    // If an error occurs, check the connection state and close it if necessary.
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                    value = "0000";
                }
            }
            return value;
        }


        public void Reset()
        {
            txtSRNO.Text = "";
            txtSRID.Text = "";
            dtpSRDate.Text = DateTime.Today.ToString("d");
            dtpSalesDate.Text = DateTime.Today.ToString("d");
            txtSalesID.Text = "";
            txtSalesInvoiceNo.Text = "";
            txtCustomerID.Text = "";
            txtCustomerName.Text = "";
            txtcust_ID.Text = "";
            txtGrandTotal.Text = "";
            btnSave.Enabled = true;
            btnDelete.Enabled = false;
            DataGridView1.Enabled = true;
            btnAdd.Enabled = true;
            pnlCalc.Enabled = true;
            btnRemove.Enabled = false;
            DataGridView1.Rows.Clear();
            DataGridView2.Rows.Clear();
            Clear();
            btnSelection.Enabled = true;
            lblSet.Text = "";
            auto();
        }

        public void auto()
        {
            try
            {
                txtSRID.Text = GenerateID();
                txtSRNO.Text = "SR-" + GenerateID();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnSelection_Click(object sender, EventArgs e)
        {
          
        }


        public double GrandTotal()
        {

    
                double sum = 0;
                try
                {
                    foreach (DataGridViewRow r in DataGridView1.Rows)
                    {
                        if (r.Cells[10].Value != null)
                        {
                            sum += Convert.ToDouble(r.Cells[10].Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return sum;

        }

        public void Clear()
        {
            txtBarcode.Text = "";
            txtProductCode.Text = "";
            txtProductName.Text = "";
            txtQty.Text = "";
            txtPrice.Text = "";
            txtDiscountPer.Text = "";
            txtDiscountAmount.Text = "";
            txtVATPer.Text = "";
            txtVATAmount.Text = "";
            txtReturnQty.Text = "";
            txtTotalAmount.Text = "";
            txtCostPrice.Text = "";
            txtMargin.Text = "";
            WName.Text = "";
            WID.Text = "";
            btnAdd.Enabled = true;
            btnRemove.Enabled = false;
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtProductName.Text == "")
                {
                    MessageBox.Show("الرجاء إدراج كود الصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProductName.Focus();
                    return;
                }
                if (txtBarcode.Text == "")
                {
                    MessageBox.Show("الرجاء كتابة الباركود", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBarcode.Focus();
                    return;
                }
                if (txtQty.Text == "")
                {
                    MessageBox.Show("الرجاء كتابة الكمية", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQty.Focus();
                    return;
                }

                if (Val(txtQty.Text) == 0)
                {
                    MessageBox.Show("الكمية يجب أن تكون أكبر من صفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQty.Focus();
                    return;
                }
                if (Val(totalsale.Text) > 0)
                {
                    MessageBox.Show("لا يوجد ارجاع لفاتورة ذات خصم كلي", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQty.Focus();
                    return;
                }
                // txtReturnQty.Text = 1
                if (txtReturnQty.Text == "")
                {
                    MessageBox.Show("الرجاء كتابة الكمية المرتجعة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtReturnQty.Focus();
                    return;
                }
                if (Val(txtReturnQty.Text) == 0)
                {
                    MessageBox.Show("الكمية المرتجعة يجب أن تكون أكبر من صفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtReturnQty.Focus();
                    return;
                }
                if (Val(txtReturnQty.Text) > Val(txtQty.Text))
                {
                    MessageBox.Show("الكمية المرتجعة لا يجب أن تكون أكبر من الكمية المباعة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtReturnQty.Text = "";
                    txtReturnQty.Focus();
                    return;
                }
              /*  if (Val(textBox2.Text) > 0)
                {
                    MessageBox.Show(" عذر لا يمكن ارجاع فاتورة اجل يرجي الذهاب والتعديل علي الفاتورة اولا", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtReturnQty.Text = "";
                    txtReturnQty.Focus();
                    return;
                }*/

                // Insert or update the DataGridView
                if (DataGridView1.Rows.Count == 0)
                {
                    DataGridView1.Rows.Add(txtProductCode.Text, txtProductName.Text, txtBarcode.Text, Val(txtPrice.Text),
                        Val(txtQty.Text), Val(txtDiscountPer.Text), Val(txtDiscountAmount.Text), Val(txtVATPer.Text),
                        Val(txtVATAmount.Text), Val(txtReturnQty.Text), Val(txtTotalAmount.Text), Val(txtProductID.Text),
                        Val(txtCostPrice.Text), Val(txtMargin.Text) * Val(txtReturnQty.Text), WName.Text, WID.Text);

                    double k = Math.Round(GrandTotal(), 2);
                    txtGrandTotal.Text = k.ToString();
                    txtPaymentDue.Text = (Convert.ToDouble(txtPaymentDue.Text) - k).ToString();

                    if (Convert.ToDecimal(txtPaymentDue.Text) <= 0)
                    {
                        txtPaymentDue.Text = "0";
                    }

                    Clear();
                }
                else
                {
                    foreach (DataGridViewRow row in DataGridView1.Rows)
                    {
                        if (txtBarcode.Text == row.Cells[2].Value.ToString())
                        {
                            MessageBox.Show("هذا الباركود مضاف مسبقا في شبكة البيانات", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtBarcode.Focus();
                            return;
                        }
                        if (txtBarcode.Text == row.Cells[2].Value & txtProductID.Text == row.Cells[11].Value.ToString())
                        {
                            MessageBox.Show("هذا الصنف مضاف مسبقا في شبكة البيانات", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtBarcode.Focus();
                            return;
                        }
                    }
                    DataGridView1.Rows.Add(txtProductCode.Text, txtProductName.Text, txtBarcode.Text, Val(txtPrice.Text),
                        Val(txtQty.Text), Val(txtDiscountPer.Text), Val(txtDiscountAmount.Text), Val(txtVATPer.Text),
                        Val(txtVATAmount.Text), Val(txtReturnQty.Text), Val(txtTotalAmount.Text), Val(txtProductID.Text),
                        Val(txtCostPrice.Text), Val(txtMargin.Text) * Val(txtReturnQty.Text), WName.Text, WID.Text);

                    double k1 = Math.Round(GrandTotal(), 2);
                    txtGrandTotal.Text = k1.ToString();
                    Clear();
                }
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
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
                txtGrandTotal.Text = k.ToString("F2");
                btnRemove.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void txtRetuenQty_TextChanged(object sender, EventArgs e)
        {
            double i = 0d;
            i = (double)(Val(txtReturnQty.Text) * Val(txtPrice.Text));
            i = Math.Round(i, 2);
            txtTotalAmount.Text = i.ToString();
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSalesInvoiceNo.Text))
                {
                    MessageBox.Show("الرجاء اختيار فاتورة المبيعات", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSalesInvoiceNo.Focus();
                    return;
                }

                if (DataGridView1.Rows.Count == 0 || DataGridView1.Rows.Cast<DataGridViewRow>().All(row => row.IsNewRow))
                {
                    MessageBox.Show("عذراً لا يوجد أصناف مرتجعة في شبكة البيانات", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            // Validate input fields before conversion
                            if (!int.TryParse(txtSRID.Text, out int srID))
                            {
                                MessageBox.Show("Invalid SR_ID. Please enter a valid number.");
                                return;
                            }

                            if (!int.TryParse(txtSalesID.Text, out int salesID))
                            {
                                MessageBox.Show("Invalid SalesID. Please enter a valid number.");
                                return;
                            }

                            if (!decimal.TryParse(txtGrandTotal.Text, out decimal grandTotal))
                            {
                                MessageBox.Show("Invalid GrandTotal. Please enter a valid amount.");
                                return;
                            }

                            // Insert into SalesReturn
                            string insertSalesReturnQuery = "INSERT INTO SalesReturn(SR_ID, SRNo, Date, SalesID, GrandTotal) VALUES (@d1, @d2, @d3, @d4, @d5)";
                            using (SqlCommand cmd = new SqlCommand(insertSalesReturnQuery, con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@d1", srID);
                                cmd.Parameters.AddWithValue("@d2", txtSRNO.Text);
                                cmd.Parameters.AddWithValue("@d3", dtpSRDate.Value.Date);
                                cmd.Parameters.AddWithValue("@d4", salesID);
                                cmd.Parameters.AddWithValue("@d5", grandTotal);
                                cmd.ExecuteNonQuery();
                            }

                            // Insert into SalesReturn_Join
                            string insertSalesReturnJoinQuery = @"
                 INSERT INTO SalesReturn_Join(SalesReturnID, Barcode, Price, Qty, DiscountPer, Discount, VATPer, VAT, ReturnQty, TotalAmount, ProductID, CostPrice, Margin) 
                 VALUES (@SRID, @Barcode, @Price, @Qty, @DiscountPer, @Discount, @VATPer, @VAT, @ReturnQty, @TotalAmount, @ProductID, @CostPrice, @Margin)";

                            foreach (DataGridViewRow row in DataGridView1.Rows)
                            {
                                if (!row.IsNewRow)
                                {
                                    // Safely retrieve and convert cell values
                                    string barcode = row.Cells[2].Value?.ToString() ?? "";
                                    decimal price = GetDecimalValue(row.Cells[3].Value);
                                    decimal qty = GetDecimalValue(row.Cells[4].Value);
                                    decimal discountPer = GetDecimalValue(row.Cells[5].Value);
                                    decimal discount = GetDecimalValue(row.Cells[6].Value);
                                    decimal vatPer = GetDecimalValue(row.Cells[7].Value);
                                    decimal vat = GetDecimalValue(row.Cells[8].Value);
                                    decimal returnQty = GetDecimalValue(row.Cells[9].Value);
                                    decimal totalAmount = GetDecimalValue(row.Cells[10].Value);
                                    int productId = GetIntValue(row.Cells[11].Value);
                                    decimal costPrice = GetDecimalValue(row.Cells[12].Value);
                                    decimal margin = GetDecimalValue(row.Cells[13].Value);

                                    using (SqlCommand cmd = new SqlCommand(insertSalesReturnJoinQuery, con, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@SRID", srID);
                                        cmd.Parameters.AddWithValue("@Barcode", barcode);
                                        cmd.Parameters.AddWithValue("@Price", price);
                                        cmd.Parameters.AddWithValue("@Qty", qty);
                                        cmd.Parameters.AddWithValue("@DiscountPer", discountPer);
                                        cmd.Parameters.AddWithValue("@Discount", discount);
                                        cmd.Parameters.AddWithValue("@VATPer", vatPer);
                                        cmd.Parameters.AddWithValue("@VAT", vat);
                                        cmd.Parameters.AddWithValue("@ReturnQty", returnQty);
                                        cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                                        cmd.Parameters.AddWithValue("@ProductID", productId);
                                        cmd.Parameters.AddWithValue("@CostPrice", costPrice);
                                        cmd.Parameters.AddWithValue("@Margin", margin);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }

                            // Fetch the current totals from InvoiceInfo
                            string fetchTotalsQuery = "SELECT TotalPaid, Balance FROM InvoiceInfo WHERE Inv_ID = @InvoiceID";
                            decimal currentTotalPayment = 0;
                            decimal currentPaymentDue = 0;

                            using (SqlCommand fetchCmd = new SqlCommand(fetchTotalsQuery, con, transaction))
                            {
                                fetchCmd.Parameters.AddWithValue("@InvoiceID", salesID);
                                using (SqlDataReader reader = fetchCmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        currentTotalPayment = Convert.ToDecimal(reader["TotalPaid"]);
                                        currentPaymentDue = Convert.ToDecimal(reader["Balance"]);
                                    }
                                }
                            }

                            // Calculate new values
                            decimal amountToDeduct = grandTotal; // Total deduction amount
                            decimal remainingAmount = amountToDeduct;

                            decimal newPaymentDue = currentPaymentDue - remainingAmount;
                            if (newPaymentDue < 0)
                            {
                                remainingAmount = Math.Abs(newPaymentDue);
                                newPaymentDue = 0;
                            }
                            else
                            {
                                remainingAmount = 0;
                            }

                            decimal newTotalPayment = currentTotalPayment - remainingAmount;
                            if (newTotalPayment < 0)
                            {
                                newTotalPayment = 0;
                            }

                            // Update Invoice_Product
                            string updateQuery = "UPDATE Invoice_Product SET Qty = Qty - @qtyDifference, TotalAmount = TotalAmount - @total, Amount = Amount - @total WHERE ProductID = @productId AND InvoiceID=@d2";

                            foreach (DataGridViewRow row in DataGridView1.Rows)
                            {
                                if (!row.IsNewRow)
                                {
                                    using (SqlCommand cmdUpdate = new SqlCommand(updateQuery, con, transaction))
                                    {
                                        int productId = GetIntValue(row.Cells[11].Value);
                                        decimal qtyReturned = GetDecimalValue(row.Cells[9].Value);
                                        decimal totalAmount = GetDecimalValue(row.Cells[10].Value);

                                        cmdUpdate.Parameters.AddWithValue("@qtyDifference", qtyReturned);
                                        cmdUpdate.Parameters.AddWithValue("@productId", productId);
                                        cmdUpdate.Parameters.AddWithValue("@total", totalAmount);
                                        cmdUpdate.Parameters.AddWithValue("@d2", txtSalesID.Text);
                                        cmdUpdate.ExecuteNonQuery();
                                    }
                                }
                            }

                            // Update InvoiceInfo
                            string updateStockQuery = @"
                 UPDATE InvoiceInfo 
                 SET GrandTotal = GrandTotal - @GrandTotal, 
                     TotalPaid = @NewTotalPayment,
                     Balance = @NewPaymentDue
                 WHERE Inv_ID = @StockID";

                            using (SqlCommand updateCmd = new SqlCommand(updateStockQuery, con, transaction))
                            {
                                updateCmd.Parameters.AddWithValue("@GrandTotal", Convert.ToDecimal(txtGrandTotal.Text));
                                updateCmd.Parameters.AddWithValue("@NewTotalPayment", newTotalPayment);
                                updateCmd.Parameters.AddWithValue("@NewPaymentDue", newPaymentDue);
                                updateCmd.Parameters.AddWithValue("@StockID", salesID);
                                int rowsAffected = updateCmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {

                                    if ( Convert.ToDecimal(txtPaymentDueP.Text) - grandTotal > 0)
                                    {

                                    /*    decimal deff = Math.Abs(Convert.ToDecimal(totalsale.Text) - grandTotal);
                                        LedgerUpdate(dtpSRDate.Value.Date, "نقدا", txtSalesInvoiceNo.Text, "فاتورة مبيعات", deff, Convert.ToDecimal(txtTotalPayment.Text), txtCustomerID.Text, "", con, transaction);

*/
                                    }
                                    else
                                    {
                                        decimal deff = Math.Abs( Convert.ToDecimal(txtPaymentDueP.Text) - grandTotal);
                                        LedgerSave(dtpSRDate.Value.Date, "نقدا", txtSalesInvoiceNo.Text, "مردودات مبيعات", 0, deff, txtCustomerID.Text, "", con, transaction);


                                        

                                    }



                                }
                                else
                                {
                                    MessageBox.Show("No records were updated. Check the StockID.");
                                }
                            }

                            // Update Invoice_Payment
                            string updateInvoicePaymentQuery = @"
                 UPDATE Invoice_Payment
                 SET TotalPaid = TotalPaid - @d5, PaymentDate = @d6
                 WHERE InvoiceID = @d1";

                            using (SqlCommand cmd = new SqlCommand(updateInvoicePaymentQuery, con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@d1", salesID);
                                cmd.Parameters.AddWithValue("@d5", remainingAmount);
                                cmd.Parameters.AddWithValue("@d6", DateTime.Now);
                                cmd.ExecuteNonQuery();
                            }

                            // Update Temp_Stock for each row in DataGridView
                            string updateStockQueryy = "UPDATE Temp_Stock SET Qty = Qty + @Qty WHERE ProductID = @ProductID AND Barcode = @Barcode";
                            foreach (DataGridViewRow row in DataGridView1.Rows)
                            {
                                if (!row.IsNewRow)
                                {
                                    using (SqlCommand cmd = new SqlCommand(updateStockQueryy, con, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@Qty", row.Cells[9].Value);
                                        cmd.Parameters.AddWithValue("@ProductID", row.Cells[11].Value);
                                        cmd.Parameters.AddWithValue("@Barcode", row.Cells[2].Value);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }

                            transaction.Commit();
                            MessageBox.Show("تم الحفظ بنجاح", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            btnSave.Enabled = false;
                            Reset();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Fix `LedgerSave` to use the same transaction
        public void LedgerSave(DateTime date, string name, string ledgerNo, string label, decimal debit, decimal credit, string partyID, string manualInv, SqlConnection con, SqlTransaction transaction)
        {
            string cb = @"
        INSERT INTO LedgerBook(Date, Name, LedgerNo, Label, Debit, Credit, PartyID, Manual_Inv) 
        VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8)";

            using (SqlCommand cmd = new SqlCommand(cb, con, transaction))
            {
                cmd.Parameters.AddWithValue("@d1", date);
                cmd.Parameters.AddWithValue("@d2", name);
                cmd.Parameters.AddWithValue("@d3", ledgerNo);
                cmd.Parameters.AddWithValue("@d4", label);
                cmd.Parameters.AddWithValue("@d5", debit);
                cmd.Parameters.AddWithValue("@d6", credit);
                cmd.Parameters.AddWithValue("@d7", partyID);
                cmd.Parameters.AddWithValue("@d8", manualInv);
                cmd.ExecuteNonQuery();
            }
        }
        public void LedgerUpdate(
                    DateTime date,
                    string name,
                    string invoiceNo,
                    string label,
                    decimal debit,
                    decimal credit,
                    string partyID,
                    string ledgerNo,
                    SqlConnection con,
                    SqlTransaction transaction)
        {
            string updateQuery = @"
        UPDATE LedgerBook
        SET Date = @d1,
            Name = @d2,
            label = @d4,
            Debit = @d5,
            Credit = @d6,
            PartyID = @d7
        WHERE LedgerNo = @d8 AND Label = @d9";

            using (SqlCommand cmd = new SqlCommand(updateQuery, con, transaction))
            {
                cmd.Parameters.Add("@d1", SqlDbType.DateTime).Value = date;
                cmd.Parameters.Add("@d2", SqlDbType.NVarChar).Value = name;
                cmd.Parameters.Add("@d4", SqlDbType.NVarChar).Value = label;
                cmd.Parameters.Add("@d5", SqlDbType.Decimal).Value = debit;
                cmd.Parameters.Add("@d6", SqlDbType.Decimal).Value = credit;
                cmd.Parameters.Add("@d7", SqlDbType.NVarChar).Value = partyID;
                cmd.Parameters.Add("@d8", SqlDbType.NVarChar).Value = ledgerNo; // Used in WHERE clause
                cmd.Parameters.Add("@d9", SqlDbType.NVarChar).Value = label;   // Used in WHERE clause

                cmd.ExecuteNonQuery();
            }
        }
        private decimal GetDecimalValue(object value)
        {
            if (value != null && decimal.TryParse(value.ToString(), out decimal result))
            {
                return result;
            }
            return 0; // Or a default value that makes sense for your context
        }
        private int GetIntValue(object value)
        {
            if (value != null && int.TryParse(value.ToString(), out int result))
            {
                return result;
            }
            return 0; // Or a default value that makes sense for your context
        }


        private double Val(string text)
        {
            double.TryParse(text, out double result);
            return result;
        }
        private void DeleteRecord()
        {
            try
            {
                // Validate input fields before conversion
                if (!int.TryParse(txtSRID.Text, out int srID))
                {
                    MessageBox.Show("Invalid SR_ID. Please enter a valid number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtSalesID.Text, out int salesID))
                {
                    MessageBox.Show("Invalid SalesID. Please enter a valid number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtGrandTotal.Text, out decimal grandTotal))
                {
                    MessageBox.Show("Invalid GrandTotal. Please enter a valid amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int rowsAffected = 0;

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // Delete from SalesReturn table
                    string deleteQuery = "DELETE FROM SalesReturn WHERE SR_ID = @SRID";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@SRID", srID);
                        rowsAffected = cmd.ExecuteNonQuery();
                    }

                    if (rowsAffected > 0)
                    {
                        // Update Temp_Stock table
                        foreach (DataGridViewRow row in DataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                object productId = row.Cells[11].Value ?? DBNull.Value;
                                object barcode = row.Cells[2].Value ?? DBNull.Value;
                                object wid = row.Cells[15].Value ?? DBNull.Value;
                                object qty = row.Cells[9].Value ?? DBNull.Value;

                                // Skip if any of the required values are null
                                if (productId == DBNull.Value || barcode == DBNull.Value || wid == DBNull.Value || qty == DBNull.Value)
                                    continue;

                                using (SqlConnection conUpdate = new SqlConnection(DataAccessLayer.Con()))
                                {
                                    conUpdate.Open();
                                    string updateTempStockQuery = "UPDATE Temp_Stock SET Qty = Qty + @Qty WHERE ProductID = @ProductID AND Barcode = @Barcode AND WID = @WID";
                                    using (SqlCommand cmdUpdate = new SqlCommand(updateTempStockQuery, conUpdate))
                                    {
                                        cmdUpdate.Parameters.AddWithValue("@ProductID", Convert.ToInt32(productId));
                                        cmdUpdate.Parameters.AddWithValue("@Barcode", barcode.ToString());
                                        cmdUpdate.Parameters.AddWithValue("@WID", wid.ToString());
                                        cmdUpdate.Parameters.AddWithValue("@Qty", Convert.ToDouble(qty));
                                        cmdUpdate.ExecuteNonQuery();
                                    }
                                }
                            }
                        }

                        // Fetch current totals from InvoiceInfo

                        string fetchTotalsQuery = "SELECT TotalPaid, Balance FROM InvoiceInfo WHERE Inv_ID = @InvoiceID";
                        decimal currentTotalPayment = 0;
                        decimal currentPaymentDue = 0;

                        using (SqlCommand fetchCmd = new SqlCommand(fetchTotalsQuery, con))
                        {
                            fetchCmd.Parameters.AddWithValue("@InvoiceID", salesID);

                            using (SqlDataReader reader = fetchCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    currentTotalPayment = Convert.ToDecimal(reader["TotalPaid"]);
                                    currentPaymentDue = Convert.ToDecimal(reader["Balance"]);
                                }
                            }
                        }

                        // Calculate new values
                        decimal amountToDeduct = grandTotal; // Total deduction amount
                        decimal remainingAmount = amountToDeduct;

                        decimal newPaymentDue = currentPaymentDue + remainingAmount;
                        if (newPaymentDue < 0)
                        {
                            remainingAmount = Math.Abs(newPaymentDue);
                            newPaymentDue = 0;
                        }
                        else
                        {
                            remainingAmount = 0;
                        }

                        decimal newTotalPayment = currentTotalPayment - remainingAmount;
                        if (newTotalPayment < 0)
                        {
                            newTotalPayment = 0;
                        }

                        // Update Invoice_Product table
                        string updateInvoiceProductQuery = @"
                    UPDATE Invoice_Product 
                    SET Qty = Qty + @QtyDifference, 
                        TotalAmount = TotalAmount + @TotalAmount, 
                        Amount = Amount + @Amount 
                    WHERE ProductID = @ProductID";

                        foreach (DataGridViewRow row in DataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                int productId = Convert.ToInt32(row.Cells[11].Value);
                                double qtyReturned = Convert.ToDouble(row.Cells[9].Value);
                                double totalAmount = Convert.ToDouble(row.Cells[10].Value);

                                using (SqlCommand cmdUpdate = new SqlCommand(updateInvoiceProductQuery, con))
                                {
                                    cmdUpdate.Parameters.AddWithValue("@QtyDifference", qtyReturned);
                                    cmdUpdate.Parameters.AddWithValue("@TotalAmount", totalAmount);
                                    cmdUpdate.Parameters.AddWithValue("@Amount", totalAmount);
                                    cmdUpdate.Parameters.AddWithValue("@ProductID", productId);
                                    cmdUpdate.ExecuteNonQuery();
                                }
                            }
                        }

                        // Update InvoiceInfo table
                        string updateInvoiceInfoQuery = @"
                    UPDATE InvoiceInfo 
                    SET GrandTotal = GrandTotal + @GrandTotal, 
                        TotalPaid = @NewTotalPayment, 
                        Balance = @NewPaymentDue 
                    WHERE Inv_ID = @InvoiceID";

                        using (SqlCommand updateCmd = new SqlCommand(updateInvoiceInfoQuery, con))
                        {
                            updateCmd.Parameters.AddWithValue("@GrandTotal", amountToDeduct);
                            updateCmd.Parameters.AddWithValue("@NewTotalPayment", newTotalPayment);
                            updateCmd.Parameters.AddWithValue("@NewPaymentDue", newPaymentDue);
                            updateCmd.Parameters.AddWithValue("@InvoiceID", salesID);
                            updateCmd.ExecuteNonQuery();
                        }

                        // Update Invoice_Payment table
                        string updateInvoicePaymentQuery = @"
                    UPDATE Invoice_Payment 
                    SET TotalPaid = TotalPaid + @RemainingAmount, 
                        PaymentDate = @PaymentDate 
                    WHERE InvoiceID = @InvoiceID";

                        using (SqlCommand cmd = new SqlCommand(updateInvoicePaymentQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@InvoiceID", salesID);
                            cmd.Parameters.AddWithValue("@RemainingAmount", remainingAmount);
                            cmd.Parameters.AddWithValue("@PaymentDate", DateTime.Now);
                            cmd.ExecuteNonQuery();
                        }

                        // Delete ledger entries related to this sales return
                        LedgerDelete(txtSalesInvoiceNo.Text, "مردودات مبيعات من " + txtCustomerName.Text);
                        LedgerDelete(txtSalesInvoiceNo.Text, "مردودات مبيعات");

                        // Log the deletion action
                        string logMessage = "deleted the Sales Return record having SR No. '" + txtSRNO.Text + "'";
                        LogFunc(lblUser.Text, logMessage);

                        // Notify success
                        MessageBox.Show("تم الحذف بنجاح", "السجلات", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Reset the form
                        Reset();
                        // RefreshRecords(); // Uncomment if needed
                    }
                    else
                    {
                        // Notify no records were found
                        MessageBox.Show("لا يوجد سجلات", "عذرًا", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Reset();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void txtReturnQty_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' | e.KeyChar > '9') & e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("هل أنت متأكد أنك تريد حذف هذا السجل?", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    DeleteRecord();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Reset();

        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            SalesReturnRecord frmSalesReturnRecord = new SalesReturnRecord();

            frmSalesReturnRecord.lblSet.Text = "SR";
            frmSalesReturnRecord.Reset();
            frmSalesReturnRecord.Show();
        }

        private void DataGridView2_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                if (DataGridView2.Rows.Count > 0)
                {
                    // Ensure Clear() is defined and called correctly
                    Clear();

                    if (DataGridView2.SelectedRows.Count > 0)
                    {
                        DataGridViewRow dr = DataGridView2.SelectedRows[0];
                        txtProductID.Text = dr.Cells[11].Value.ToString();
                        txtProductCode.Text = dr.Cells[0].Value.ToString();
                        txtProductName.Text = dr.Cells[1].Value.ToString();
                        txtBarcode.Text = dr.Cells[2].Value.ToString();
                        txtPrice.Text = dr.Cells[3].Value.ToString();
                        txtQty.Text = dr.Cells[4].Value.ToString();
                        txtDiscountPer.Text = dr.Cells[6].Value.ToString();
                        txtDiscountAmount.Text = dr.Cells[7].Value.ToString();
                        txtVATPer.Text = dr.Cells[8].Value.ToString();
                        txtVATAmount.Text = dr.Cells[9].Value.ToString();
                        txtCostPrice.Text = dr.Cells[12].Value.ToString();
                        WID.Text=dr.Cells[14].Value.ToString();   
                        WName.Text = dr.Cells[15].Value.ToString();  
                        
                        // Convert cell values to numeric types before division
                        decimal margin = Convert.ToDecimal(dr.Cells[13].Value) / Convert.ToDecimal(dr.Cells[4].Value);
                        txtMargin.Text = margin.ToString();

                        txtReturnQty.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Please select a row first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataGridView2_RowPostPaint(object sender, System.Windows.Forms.DataGridViewRowPostPaintEventArgs e)
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

        private void DataGridView1_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (DataGridView1.Rows.Count > 0)
            {
                if (lblSet.Text == "Not allowed")
                {
                    btnRemove.Enabled = false;
                }
                else
                {
                    btnRemove.Enabled = true;
                }
            }
        }

        private void DataGridView1_RowPostPaint(object sender, System.Windows.Forms.DataGridViewRowPostPaintEventArgs e)
        {
            string strRowNumber = (e.RowIndex + 1).ToString();
            SizeF size = e.Graphics.MeasureString(strRowNumber, this.Font);
            if (DataGridView1.RowHeadersWidth < Convert.ToInt32(size.Width + 20))
            {
                DataGridView1.RowHeadersWidth = Convert.ToInt32(size.Width + 20);
            }
            Brush b = SystemBrushes.ControlText;
            e.Graphics.DrawString(strRowNumber, this.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + (e.RowBounds.Height - size.Height) / 2);

        }
        public void SMS(string st1)
        {

            con.Open();
            string cb = "insert into SMS(Message,Date) VALUES (@d1,@d2)";
            SqlCommand cmd = new SqlCommand(cb);
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@d1", st1);
            cmd.Parameters.AddWithValue("@d2", DateTime.Now);
            cmd.ExecuteReader();
            con.Close();
        }
        public void LogFunc(string st1, string st2)
        {

            con.Open();
            string cb = "insert into Logs(UserID,Date,Operation) VALUES (@d1,@d2,@d3)";
            SqlCommand cmd = new SqlCommand(cb);
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@d1", st1);
            cmd.Parameters.AddWithValue("@d2", DateTime.Now);
            cmd.Parameters.AddWithValue("@d3", st2);
            cmd.ExecuteReader();
            con.Close();
        }
        public void SMSFunc(string st1, string st2, string st3)
        {
            st3 = st3.Replace("@MobileNo", st1).Replace("@Message", st2);
            HttpWebRequest request;
            HttpWebResponse response = default;
            var myUri = new Uri(st3);
            request = (HttpWebRequest)WebRequest.Create(myUri);
            response = (HttpWebResponse)request.GetResponse();
        }
        public string Encrypt(string password)
        {
            string strmsg = string.Empty;
            byte[] encode = new byte[password.Length];
            encode = Encoding.UTF8.GetBytes(password);
            strmsg = Convert.ToBase64String(encode);
            return strmsg;
        }

        public string Decrypt(string encryptpwd)
        {
            string decryptpwd = string.Empty;
            var encodepwd = new UTF8Encoding();
            var Decode = encodepwd.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encryptpwd);
            int charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            decryptpwd = new string(decoded_char);
            return decryptpwd;
        }
        public void RefreshRecords()
        {
  /*          frmStockBalance obj = (frmStockBalance)Application.OpenForms("frmStockBalance");
            obj.Getdata();
            obj.DataGridView1.Refresh();
            obj.DataGridView1.Update();*/
        }
    
       
        public void LedgerDelete(string a, string b)
        {

            con.Open();
            string cq = "delete from LedgerBook where LedgerNo=@d1 and Label=@d2";
            SqlCommand cmd = new SqlCommand(cq);
            cmd.Parameters.AddWithValue("@d1", a);
            cmd.Parameters.AddWithValue("@d2", b);
            cmd.Connection = con;
            cmd.ExecuteReader();
            con.Close();
        }
      
        public void SupplierLedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g)
        {

            con.Open();
            string cb = "insert into SupplierLedgerBook(Date, Name, LedgerNo, Label,Debit,Credit,PartyID) Values (@d1,@d2,@d3,@d4,@d5,@d6,@d7)";
            SqlCommand cmd = new SqlCommand(cb);
            cmd.Parameters.AddWithValue("@d1", a);
            cmd.Parameters.AddWithValue("@d2", b);
            cmd.Parameters.AddWithValue("@d3", c);
            cmd.Parameters.AddWithValue("@d4", d);
            cmd.Parameters.AddWithValue("@d5", e);
            cmd.Parameters.AddWithValue("@d6", f);
            cmd.Parameters.AddWithValue("@d7", g);
            cmd.Connection = con;
            cmd.ExecuteReader();
            con.Close();
        }
        public void SendMail(string s1, string s2, string s3, string s5, string s6, int s7, string s8, string s9)
        {
            var msg = new MailMessage();
            try
            {
                msg.From = new MailAddress(s1);
                msg.To.Add(s2);
                msg.Body = s3;
                msg.IsBodyHtml = true;
                msg.Subject = s5;
                var smt = new SmtpClient(s6);
                smt.Port = s7;
                smt.Credentials = new NetworkCredential(s8, s9);
                smt.EnableSsl = true;
                smt.Send(msg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void SupplierLedgerDelete(string a)
        {

            con.Open();
            string cq = "delete from SupplierLedgerBook where LedgerNo=@d1";
            SqlCommand cmd = new SqlCommand(cq);
            cmd.Parameters.AddWithValue("@d1", a);
            cmd.Connection = con;
            cmd.ExecuteReader();
            con.Close();
        }
        public void SupplierLedgerUpdate(DateTime a, string b, decimal e, decimal f, string g, string h)
        {

            con.Open();
            string cb = "Update SupplierLedgerBook set Date=@d1, Name=@d2,Debit=@d3,Credit=@d4 where LedgerNo=@d5 and Label=@d6";
            SqlCommand cmd = new SqlCommand(cb);
            cmd.Parameters.AddWithValue("@d1", a);
            cmd.Parameters.AddWithValue("@d2", b);
            cmd.Parameters.AddWithValue("@d3", e);
            cmd.Parameters.AddWithValue("@d4", f);
            cmd.Parameters.AddWithValue("@d5", g);
            cmd.Parameters.AddWithValue("@d6", h);
            cmd.Connection = con;
            cmd.ExecuteReader();
            con.Close();
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnSelection_BackgroundImageChanged(object sender, EventArgs e)
        {

        }

        private void txtGrandTotal_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSelection_Click_1(object sender, EventArgs e)
        {
            SalesInvoiceScreen frmSalesInvoiceRecord = new SalesInvoiceScreen();
            frmSalesInvoiceRecord.lblSet.Text = "SR";
            frmSalesInvoiceRecord.Reset();
            frmSalesInvoiceRecord.ShowDialog();
        }

      

        private void txtSalesInvoiceNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void DataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Label16_Click(object sender, EventArgs e)
        {

        }

        private void GroupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void pnlCalc_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Label7_Click(object sender, EventArgs e)
        {

        }

        private void Label6_Click(object sender, EventArgs e)
        {

        }

        private void txtTotalPayment_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    } 
 }

