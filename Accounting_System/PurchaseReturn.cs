using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Accounting_System.Pymentinvoice;

namespace Accounting_System
{
    public partial class PurchaseReturn : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        private static PurchaseReturn _instance;
        public static PurchaseReturn instance;
        public static PurchaseReturn Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new PurchaseReturn();
                }
                return _instance;
            }
        }

        public PurchaseReturn()
        {
            InitializeComponent();
            DataGridView1.MouseClick += new MouseEventHandler(DataGridView1_MouseClick);
            instance=this;
            LoadCurrenciesToComboBox();
        }


        private string str;
        private string st;
        private double num1, num2, num3, num4, num5, num6, num7, num8, num9, num10, num11;


        private string GenerateID()
        {
            string value = "0000";
            string connectionString = DataAccessLayer.Con(); // Assuming this method fetches the connection string

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT TOP 1 PR_ID FROM PurchaseReturn ORDER BY PR_ID DESC";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                value = rdr["PR_ID"].ToString();
                            }
                        }
                    }

                    // Convert the string ID to integer, increment by 1 and format it back to a 4-digit string
                    int id = int.Parse(value);
                    id += 1;
                    value = id.ToString("D4");
                }
                catch (Exception ex)
                {
                    // Log the exception if needed
                    // e.g., Console.WriteLine(ex.Message);
                    value = "0000";
                }
            }

            return value;
        }

        /*
                private string GenerateID()
                {

                    string value = "00";
                    try
                    {
                        // Fetch the latest ID from the database
                        con.Open();
                        SqlCommand cmd = new SqlCommand("SELECT TOP 1 PR_ID FROM PurchaseReturn ORDER BY PR_ID DESC", con);
                        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            value = rdr["PR_ID"].ToString();
                        }
                        rdr.Close();
                        // Increase the ID by 1
                        value = (value + 1).ToString();
                        // Because incrementing a string with an integer removes 0's
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
                    return value;
                }
        */
        public void Reset()
        {
            txtPRNO.Text = string.Empty;
            txtPRID.Text = string.Empty;
            dtpPRDate.Value = DateTime.Today;
            dtpPurchaseDate.Value = DateTime.Today;
            txtPurchaseID.Text = string.Empty;
            txtPurchaseInvoiceNo.Text = string.Empty;
            txtDiscPer.Text = "0.00";
            txtDisc.Text = "0.00";
            txtVatPer.Text = "0.00";
            txtVATAmt.Text = "0.00";
            txtSubTotal.Text = string.Empty;
            txtTotal.Text = string.Empty;
            txtSupplierID.Text = string.Empty;
            txtSupplierName.Text = string.Empty;
            txtSup_ID.Text = string.Empty;
            txtGrandTotal.Text = string.Empty;
            txtRoundOff.Text = "0.00";
            btnSave.Enabled = true;
            btnDelete.Enabled = false;
            DataGridView1.Enabled = true;
            btnAdd.Enabled = true;
            pnlCalc.Enabled = true;
            btnRemove.Enabled = false;
            DataGridView1.Rows.Clear();
            DataGridView2.Rows.Clear();
            btnSelection.Enabled = true;
            lblSet.Text = string.Empty;
            btnUpdate.Enabled = false;
            P_method.Clear();   
            Clear();
            auto();
        }


        public void Compute()
        {
            num6 = Val(txtSubTotal.Text) * Val(txtDiscPer.Text) / 100;
            num6 = Math.Round(num6, 2);
            txtDisc.Text = num6.ToString();
            num7 = Val(txtSubTotal.Text) - num6;
            num8 = num7 * Val(txtVatPer.Text) / 100;
            num8 = Math.Round(num8, 2);
            txtVATAmt.Text = num8.ToString();
            num1 = num7 + Val(txtVATAmt.Text);
            num1 = Math.Round(num1, 2);
            txtTotal.Text = num1.ToString();
            num2 = Math.Round(num1, 1);
            num3 = num2 - num1;
            num3 = Math.Round(num3, 2);
            txtRoundOff.Text = num3.ToString();
            num4 = Val(txtTotal.Text) + Val(txtRoundOff.Text);
            num4 = Math.Round(num4, 2);
            txtGrandTotal.Text = num4.ToString();

        }

        public void auto()
        {
            try
            {
                txtPRID.Text = GenerateID();
                txtPRNO.Text = "PR-" + GenerateID();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnSelection_Click(object sender, EventArgs e)
        {
            PymentinvoiceScreen frmPurchaseRecord = new PymentinvoiceScreen();

            frmPurchaseRecord.lblSet.Text = "PR";
            frmPurchaseRecord.Reset();
            frmPurchaseRecord.ShowDialog();

        }

        private double Val(string text)
        {
            double.TryParse(text, out double result);
            return result;
        }

        private void DataGridView2_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                if (DataGridView2.Rows.Count > 0)
                {
                    Clear();
                    DataGridViewRow dr = DataGridView2.SelectedRows[0];
                    txtProductID.Text = dr.Cells[0].Value.ToString();
                    txtProductCode.Text = dr.Cells[1].Value.ToString();
                    txtProductName.Text = dr.Cells[2].Value.ToString();
                    txtBarcode.Text = dr.Cells[3].Value.ToString();
                    txtQty.Text = dr.Cells[4].Value.ToString();
                    txtPricePerQty.Text = dr.Cells[5].Value.ToString();
                    txtReturnQty.Focus();
                    WName.Text=dr.Cells[8].Value.ToString();
                    WID.Text=dr.Cells[7].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public double SubTotal()
        {
            double sum = 0d;
            try
            {
                foreach (DataGridViewRow r in this.DataGridView1.Rows)
                    sum = sum + Convert.ToDouble(r.Cells[11].Value);
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
            }
            return sum;
        }

        public void Clear()
        {
            txtProductName.Text = "";
            txtBarcode.Text = "";
            txtProductCode.Text = "";
            txtQty.Text = "";
            txtPricePerQty.Text = "";
            txtTotalAmount.Text = "";
            txtReturnQty.Text = "";
            btnAdd.Enabled = true;
            btnRemove.Enabled = false;
            WName.Text = "";
            WID.Text = "";
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtProductName.Text == "")
                {
                    MessageBox.Show("الرجاء إدراج اسم الصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                if (txtPricePerQty.Text == "")
                {
                    MessageBox.Show("الرجاء كتابة سعر الوحدة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPricePerQty.Focus();
                    return;
                }

                if (Val(txtPricePerQty.Text) == 0)
                {
                    MessageBox.Show("سعر الوحدة يجب أن يكون أكبر من صفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPricePerQty.Focus();
                    return;
                }
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
                    MessageBox.Show("الكمية المرتجعة لا يجب أن تكون أكبر من كمية الشراء", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtReturnQty.Text = "";
                    txtReturnQty.Focus();
                    return;
                }
                if (DataGridView1.Rows.Count == 0)
                {
                    double tp = (Convert.ToDouble(txtQty.Text) - Convert.ToDouble(txtReturnQty.Text)) * Convert.ToDouble(txtPricePerQty.Text);
                    DataGridView1.Rows.Add(Val(txtProductID.Text), txtProductCode.Text, txtProductName.Text, txtBarcode.Text, Val(txtQty.Text), Val(txtPricePerQty.Text), Val(txtReturnQty.Text), Val(txtTotalAmount.Text),WName.Text, (WID.Text), Val(tp.ToString()),Val(textBox3.Text) );
                    double k = 0d;
                    k = SubTotal();
                    k = Math.Round(k, 2);
                    txtSubTotal.Text = k.ToString();
                    Compute();
                    Clear();
                    return;
                }
              /*  if (Val(textBox1.Text) <= 0)
                {
                    MessageBox.Show("لا يمكن ارجاع صنف في فاتوره اجله", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtReturnQty.Focus();
                    return;
                }*/
                foreach (DataGridViewRow row in DataGridView1.Rows)
                {
                    if (txtBarcode.Text == row.Cells[3].Value)
                    {
                        MessageBox.Show("الباركود هذا مدرج بالفعل في شبكة البيانات", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtBarcode.Focus();
                        return;
                    }
                    if (txtBarcode.Text == row.Cells[3].Value & txtProductID.Text == row.Cells[0].Value.ToString())
                    {
                        MessageBox.Show("هذا السجل مضاف بالفعل في شبكة البيانات", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtBarcode.Focus();
                        return;
                    }
                }
                double tr = (Convert.ToDouble(txtQty.Text) - Convert.ToDouble(txtReturnQty.Text)) * Convert.ToDouble(txtPricePerQty.Text);
                DataGridView1.Rows.Add(Val(txtProductID.Text), txtProductCode.Text, txtProductName.Text, txtBarcode.Text, Val(txtQty.Text), Val(txtPricePerQty.Text), Val(txtReturnQty.Text), Val(txtTotalAmount.Text), WName.Text, (WID.Text), Val(tr.ToString()), Val(textBox3.Text));
                double k1 = 0d;
                k1 = SubTotal();
                k1 = Math.Round(k1, 2);
                txtSubTotal.Text = k1.ToString();
                Compute();
                Clear();
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
                k = SubTotal();
                k = Math.Round(k, 2);
                txtSubTotal.Text = k.ToString();
                Compute();
                btnRemove.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void txtAddVATAmt_TextChanged(object sender, EventArgs e)
        {
            Compute();
        }

        private void txtAddVatPer_TextChanged(object sender, EventArgs e)
        {
            Compute();
        }

        private void txtVATAmt_TextChanged(object sender, EventArgs e)
        {
            Compute();
        }

        private void txtVatPer_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtDisc_TextChanged(object sender, EventArgs e)
        {
            Compute();
        }

        private void txtDiscPer_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtSubTotal_TextChanged(object sender, EventArgs e)
        {
            Compute();
        }

        private void txtTotal_TextChanged(object sender, EventArgs e)
        {
            Compute();
        }

        private void txtRoundOff_TextChanged(object sender, EventArgs e)
        {
            Compute();
        }

        private void txtRetuenQty_TextChanged(object sender, EventArgs e)
        {
            double i = 0d;
            i = (double)(Val(txtReturnQty.Text) * Val(txtPricePerQty.Text));
            i = Math.Round(i, 2);
            txtTotalAmount.Text = i.ToString();
        }



        private void btnSave_Click(object sender, EventArgs e) // PurchaseReturn
        {
            try
            {
                // Input Validation
                if (string.IsNullOrWhiteSpace(txtPurchaseInvoiceNo.Text))
                {
                    MessageBox.Show("الرجاء إدراج بيانات فاتورة الشراء", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPurchaseInvoiceNo.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtSupplierID.Text))
                {
                    MessageBox.Show("الرجاء استرداد رقم المورد", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSupplierID.Focus();
                    return;
                }

                if (DataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("الرجاء إدراج بيانات الأصناف المرتجعة في شبكة البيانات", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDiscPer.Text) || !decimal.TryParse(txtDiscPer.Text, out _))
                {
                    MessageBox.Show("الرجاء إدراج نسبة الخصم % بشكل صحيح", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDiscPer.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtRoundOff.Text) || !decimal.TryParse(txtRoundOff.Text, out _))
                {
                    MessageBox.Show("الرجاء كتابة التقريب بشكل صحيح", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtRoundOff.Focus();
                    return;
                }

                // Insert into PurchaseReturn table
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "INSERT INTO PurchaseReturn(PR_ID, PRNo, Date, PurchaseID, SubTotal, DiscPer, Discount, VATPer, VATAmt, Total, RoundOff, GrandTotal, Currencies, CPrice) " +
                                   "VALUES (@d1, @d2, @d3, @d5, @d6, @d7, @d8, @d9, @d10, @d13, @d14, @d15, @d16, @d17)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (!int.TryParse(txtPRID.Text, out int prId) || !decimal.TryParse(txtSubTotal.Text, out decimal subTotal) ||
                            !decimal.TryParse(txtDiscPer.Text, out decimal discPer) || !decimal.TryParse(txtDisc.Text, out decimal disc) ||
                            !decimal.TryParse(txtVatPer.Text, out decimal vatPer) || !decimal.TryParse(txtVATAmt.Text, out decimal vatAmt) ||
                            !decimal.TryParse(txtTotal.Text, out decimal total) || !decimal.TryParse(txtRoundOff.Text, out decimal roundOff) ||
                            !decimal.TryParse(txtGrandTotal.Text, out decimal grandTotal) || !decimal.TryParse(Cprice.Text, out decimal cPrice))
                        {
                            MessageBox.Show("إدخال غير صالح لبعض الحقول العددية.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        cmd.Parameters.AddWithValue("@d1", prId);
                        cmd.Parameters.AddWithValue("@d2", txtPRNO.Text);
                        cmd.Parameters.AddWithValue("@d3", dtpPRDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d5", Convert.ToInt32(txtPurchaseID.Text));
                        cmd.Parameters.AddWithValue("@d6", subTotal);
                        cmd.Parameters.AddWithValue("@d7", discPer);
                        cmd.Parameters.AddWithValue("@d8", disc);
                        cmd.Parameters.AddWithValue("@d9", vatPer);
                        cmd.Parameters.AddWithValue("@d10", vatAmt);
                        cmd.Parameters.AddWithValue("@d13", total);
                        cmd.Parameters.AddWithValue("@d14", roundOff);
                        cmd.Parameters.AddWithValue("@d15", grandTotal);
                        cmd.Parameters.AddWithValue("@d16", comboBox1.Text);
                        cmd.Parameters.AddWithValue("@d17", cPrice);

                        cmd.ExecuteNonQuery();
                    }
                }

                // Update Stock Table
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // Fetch current totals from Stock table
                    string fetchQuery = "SELECT TotalPayment, PaymentDue FROM Stock WHERE ST_ID = @StockID";
                    decimal currentTotalPayment = 0;
                    decimal currentPaymentDue = 0;

                    using (SqlCommand fetchCmd = new SqlCommand(fetchQuery, con))
                    {
                        fetchCmd.Parameters.AddWithValue("@StockID", Convert.ToInt32(txtPurchaseID.Text));

                        using (SqlDataReader reader = fetchCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                currentTotalPayment = Convert.ToDecimal(reader["TotalPayment"]);
                                currentPaymentDue = Convert.ToDecimal(reader["PaymentDue"]);
                            }
                        }
                    }

                    // Calculate new values
                    decimal amountToDeduct = decimal.TryParse(txtSubTotal.Text, out decimal subtotal) ? subtotal : 0;
                    decimal remainingAmount = amountToDeduct;

                    decimal newPaymentDue = currentPaymentDue - remainingAmount;
                    if ( newPaymentDue < 0 )
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

                    // Update Stock_Product table
                    string updateProductQuery = "UPDATE Stock_Product SET Qty = @qtyDifference, TotalAmount = @total, TotalCurrency = TotalCurrency - @TotalCurrency " +
                                                "WHERE ProductID = @productId AND StockID = @stockId";

                    using (SqlCommand cmdUpdate = new SqlCommand(updateProductQuery, con))
                    {
                        foreach (DataGridViewRow row in DataGridView1.Rows)
                        {
                            if (!row.IsNewRow && row.Cells[0].Value != null && row.Cells[4].Value != null)
                            {
                                int productId = Convert.ToInt32(row.Cells[0].Value); // Assuming ProductID is in the first column
                                double newQty = Convert.ToDouble(row.Cells[4].Value); // Assuming Qty is in the fifth column
                                double oldQty = Convert.ToDouble(row.Cells[6].Value);
                                double totalc = Convert.ToDouble(row.Cells[10].Value);
                                double totalCurrency = Convert.ToDouble(row.Cells[7].Value);
                                double qtyDifference = newQty - oldQty; // Calculate the difference

                                cmdUpdate.Parameters.Clear();
                                
                                cmdUpdate.Parameters.AddWithValue("@qtyDifference", qtyDifference); // Quantity difference
                                
                                cmdUpdate.Parameters.AddWithValue("@productId", productId); // ProductID
                                
                                cmdUpdate.Parameters.AddWithValue("@total", totalc); // Total amount
                                
                                cmdUpdate.Parameters.AddWithValue("@TotalCurrency", totalCurrency); // Total currency
                                
                                cmdUpdate.Parameters.AddWithValue("@stockId", Convert.ToInt32(txtPurchaseID.Text)); // StockID

                                cmdUpdate.ExecuteNonQuery();
                            }
                        }
                    }

                    // Update Stock table
                    string updateStockQuery = "UPDATE Stock " +
                                              "SET Total = Total - @Total, " +
                                                  "RoundOff = RoundOff - @RoundOff, " +
                                                  "GrandTotal = GrandTotal - @GrandTotal, " +
                                                  "TotalPayment = @NewTotalPayment, " +
                                                  "PaymentDue = @NewPaymentDue, " +
                                                  "SubTotal = SubTotal - @Total " +
                                              "WHERE ST_ID = @StockID";

                    using (SqlCommand updateCmd = new SqlCommand(updateStockQuery, con))
                    {
                        updateCmd.Parameters.AddWithValue("@Total", decimal.TryParse(txtTotal.Text, out decimal totalVal) ? totalVal : 0);
                        updateCmd.Parameters.AddWithValue("@RoundOff", decimal.TryParse(txtRoundOff.Text, out decimal roundOffVal) ? roundOffVal : 0);
                        updateCmd.Parameters.AddWithValue("@GrandTotal", decimal.TryParse(txtGrandTotal.Text, out decimal grandTotalVal) ? grandTotalVal : 0);
                        updateCmd.Parameters.AddWithValue("@NewTotalPayment", newTotalPayment);
                        updateCmd.Parameters.AddWithValue("@NewPaymentDue", newPaymentDue);
                        updateCmd.Parameters.AddWithValue("@StockID", Convert.ToInt32(txtPurchaseID.Text));

                        int rowsAffected = updateCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            if (remainingAmount > 0)
                            {
                                
                                decimal deff = Math.Abs(Convert.ToDecimal(totalpay.Text) - Convert.ToDecimal(txtGrandTotal.Text));
                                //if ((Convert.ToDecimal(totalpay.Text) - Convert.ToDecimal(txtGrandTotal.Text)) > 0)
                                //{

                                //}
                                LedgerSave(dtpPRDate.Value.Date, "نقدا", txtPRNO.Text, "مردودات مشتريات",
                                    deff, deff,
                                    txtSupplierID.Text, "");

                                SupplierLedgerSave(dtpPRDate.Value.Date, "نقدا", txtPRNO.Text, "مردودات مشتريات",
                                    deff, deff,
                                    txtSupplierID.Text, comboBox1.Text, Convert.ToDecimal(Cprice.Text));

                            }
                            else
                            {
                                if (!decimal.TryParse(Cprice.Text, out decimal currencyPrice))
                                {
                                    MessageBox.Show("سعر العملة غير صالح");
                                    return;
                                }

                                if (!decimal.TryParse(totalpay.Text, out decimal totalPay) ||
                                    !decimal.TryParse(txtGrandTotal.Text, out decimal grandTotal) ||
                                    !decimal.TryParse(textBox2.Text, out decimal GeneralT) ||
                                    !decimal.TryParse(textBox1.Text, out decimal Balance))
                                {
                                    MessageBox.Show("قيمة غير صالحة في حقل المبلغ");
                                    return;
                                }

                                decimal defff = Math.Abs(GeneralT - grandTotal);

                                // Fixed: Use InvoiceNo (txtPRNO.Text) instead of SupplierID
                                SupplierLedgerUpdate(
                                    dtpPRDate.Value.Date,
                                    defff ,
                                    Balance,
                                    txtPurchaseInvoiceNo.Text, // <-- Corrected parameter
                                    "فاتورة مشتريات",
                                    comboBox1.Text,
                                    currencyPrice
                                );
                            }
                        }
                    }
                }

                // Insert into PurchaseReturn_Join table
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "INSERT INTO PurchaseReturn_Join(PurchaseReturnID, ProductID, Barcode, Qty, Price, ReturnQty, TotalAmount, WName, WID) " +
                                   "VALUES (@PRID, @d1, @d2, @d3, @d4, @d5, @d6, @d8, @d9)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Prepare();

                        foreach (DataGridViewRow row in DataGridView1.Rows)
                        {
                            if (!row.IsNewRow && row.Cells[0].Value != null && row.Cells[3].Value != null && row.Cells[4].Value != null)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@PRID", Convert.ToInt32(txtPRID.Text));
                                cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(row.Cells[0].Value));
                                cmd.Parameters.AddWithValue("@d2", row.Cells[3].Value.ToString());
                                cmd.Parameters.AddWithValue("@d3", Convert.ToDouble(row.Cells[4].Value));
                                cmd.Parameters.AddWithValue("@d4", Convert.ToDouble(row.Cells[5].Value));
                                cmd.Parameters.AddWithValue("@d5", Convert.ToDouble(row.Cells[6].Value));
                                cmd.Parameters.AddWithValue("@d6", Convert.ToDouble(row.Cells[7].Value));
                                cmd.Parameters.AddWithValue("@d8", row.Cells[8].Value.ToString());
                                cmd.Parameters.AddWithValue("@d9", Convert.ToInt32(row.Cells[9].Value));

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                // Update Temp_Stock table
                foreach (DataGridViewRow row in DataGridView1.Rows)
                {
                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();
                        string query = "UPDATE Temp_Stock SET Qty = Qty - @Qty WHERE ProductID = @ProductID AND Barcode = @Barcode AND WID = @WID";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            if (!row.IsNewRow && row.Cells[0].Value != null && row.Cells[3].Value != null && row.Cells[6].Value != null && row.Cells[9].Value != null)
                            {
                                cmd.Parameters.AddWithValue("@Qty", Convert.ToDouble(row.Cells[6].Value));
                                cmd.Parameters.AddWithValue("@ProductID", Convert.ToInt32(row.Cells[0].Value));
                                cmd.Parameters.AddWithValue("@Barcode", row.Cells[3].Value.ToString());
                                cmd.Parameters.AddWithValue("@WID", row.Cells[9].Value.ToString());

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                // Log and notify success
                LogFunc(lblUser.Text, "added the new Purchase return record having PR No. '" + txtPRNO.Text + "'");
                MessageBox.Show("تم الحفظ بنجاح", "السجلات", MessageBoxButtons.OK, MessageBoxIcon.Information);

                btnSave.Enabled = false;
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public static void SupplierLedgerUpdate(DateTime date,
      decimal debit, decimal credit, string ledgerNo, string label,
      string currency, decimal currencyPrice)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = @"
                UPDATE SupplierLedgerBook 
                SET 
                    Date = @date,
                    Debit = @debit,
                    Credit =  @credit,
                    Currencies = @currency,
                    CPrice = @currencyPrice
                WHERE 
                    LedgerNo = @ledgerNo 
                   ";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Add parameters with explicit data types
                        cmd.Parameters.Add("@date", SqlDbType.DateTime).Value = date;
                        cmd.Parameters.Add("@debit", SqlDbType.Decimal).Value = debit;
                        cmd.Parameters.Add("@credit", SqlDbType.Decimal).Value = credit;
                        cmd.Parameters.Add("@ledgerNo", SqlDbType.NVarChar, 250).Value = ledgerNo;
                        cmd.Parameters.Add("@label", SqlDbType.NVarChar, 255).Value = label;
                        cmd.Parameters.Add("@currency", SqlDbType.NVarChar, 50).Value = currency;
                        cmd.Parameters.Add("@currencyPrice", SqlDbType.Decimal).Value = currencyPrice;

                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Optional: Add validation for rows affected
                        if (rowsAffected == 0)
                        {
                            throw new InvalidOperationException("لم يتم العثور على السجل المطلوب للتحديث");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        public void SupplierLedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g, string q, decimal v)
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con())) // Use your existing connection method
            {
                con.Open();
                string cb = "INSERT INTO SupplierLedgerBook(Date, Name, LedgerNo, Label, Debit, Credit, PartyID, Currencies, CPrice) " +
                            "VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9)";

                using (SqlCommand cmd = new SqlCommand(cb, con))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.Parameters.AddWithValue("@d2", b);
                    cmd.Parameters.AddWithValue("@d3", c);
                    cmd.Parameters.AddWithValue("@d4", d);
                    cmd.Parameters.AddWithValue("@d5", e);
                    cmd.Parameters.AddWithValue("@d6", f);
                    cmd.Parameters.AddWithValue("@d7", g);
                    cmd.Parameters.AddWithValue("@d8", q);
                    cmd.Parameters.AddWithValue("@d9", v);

                    cmd.ExecuteNonQuery(); // Use ExecuteNonQuery instead of ExecuteReader
                }
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("هل أنت متأكد أنك تريد بالفعل حذف هذا السجل?", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                {
                    DeleteRecord();
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
                int RowsAffected = 0;

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();

                    try
                    {
                        // Delete from PurchaseReturn
                        string cq = "DELETE FROM PurchaseReturn WHERE PR_ID=@d1";
                        using (SqlCommand cmd = new SqlCommand(cq, con, transaction))
                        {
                            cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtPRID.Text));
                            RowsAffected = cmd.ExecuteNonQuery();
                        }

                        // Fetch current totals from Stock table
                        string fetchQuery = "SELECT TotalPayment, PaymentDue FROM Stock WHERE ST_ID = @StockID";
                        decimal currentTotalPayment = 0;
                        decimal currentPaymentDue = 0;

                        using (SqlCommand fetchCmd = new SqlCommand(fetchQuery, con, transaction))
                        {
                            fetchCmd.Parameters.AddWithValue("@StockID", Convert.ToInt32(txtPurchaseID.Text));

                            using (SqlDataReader reader = fetchCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    currentTotalPayment = Convert.ToDecimal(reader["TotalPayment"]);
                                    currentPaymentDue = Convert.ToDecimal(reader["PaymentDue"]);
                                }
                            }
                        }

                        // Calculate new values
                        decimal amountToDeduct = decimal.TryParse(txtSubTotal.Text, out decimal subtotal) ? subtotal : 0;
                        decimal newPaymentDue = currentPaymentDue + amountToDeduct;
                        decimal newTotalPayment = currentTotalPayment + amountToDeduct;

                        // Update Stock_Product table
                        string updateProductQuery = "UPDATE Stock_Product " +
                                                    "SET Qty = Qty + @qtyDifference, " +
                                                        "TotalAmount = TotalAmount + @total, " +
                                                        "TotalCurrency = TotalCurrency + @TotalCurrency " +
                                                    "WHERE ProductID = @productId AND StockID = @stockId";

                        using (SqlCommand cmdUpdate = new SqlCommand(updateProductQuery, con, transaction))
                        {
                            foreach (DataGridViewRow row in DataGridView1.Rows)
                            {
                                if (!row.IsNewRow && row.Cells[0].Value != null && row.Cells[4].Value != null)
                                {
                                    cmdUpdate.Parameters.Clear();
                                    cmdUpdate.Parameters.AddWithValue("@qtyDifference", Convert.ToDouble(row.Cells[6].Value));
                                    cmdUpdate.Parameters.AddWithValue("@productId", Convert.ToInt32(row.Cells[0].Value));
                                    cmdUpdate.Parameters.AddWithValue("@total", Convert.ToDouble(row.Cells[10].Value));
                                    cmdUpdate.Parameters.AddWithValue("@TotalCurrency", Convert.ToDouble(row.Cells[7].Value));
                                    cmdUpdate.Parameters.AddWithValue("@stockId", Convert.ToInt32(txtPurchaseID.Text));

                                    cmdUpdate.ExecuteNonQuery();
                                }
                            }
                        }

                        // Update Stock table
                        string updateStockQuery = "UPDATE Stock " +
                                                  "SET Total = Total + @Total, " +
                                                      "RoundOff = RoundOff + @RoundOff, " +
                                                      "GrandTotal = GrandTotal + @GrandTotal, " +
                                                      "TotalPayment = @NewTotalPayment, " +
                                                      "PaymentDue = @NewPaymentDue, " +
                                                      "SubTotal = SubTotal + @Total " +
                                                  "WHERE ST_ID = @StockID";

                        using (SqlCommand updateCmd = new SqlCommand(updateStockQuery, con, transaction))
                        {
                            updateCmd.Parameters.AddWithValue("@Total", subtotal);
                            updateCmd.Parameters.AddWithValue("@RoundOff", decimal.TryParse(txtRoundOff.Text, out decimal roundOffVal) ? roundOffVal : 0);
                            updateCmd.Parameters.AddWithValue("@GrandTotal", decimal.TryParse(txtGrandTotal.Text, out decimal grandTotalVal) ? grandTotalVal : 0);
                            updateCmd.Parameters.AddWithValue("@NewTotalPayment", newTotalPayment);
                            updateCmd.Parameters.AddWithValue("@NewPaymentDue", newPaymentDue);
                            updateCmd.Parameters.AddWithValue("@StockID", Convert.ToInt32(txtPurchaseID.Text));

                            updateCmd.ExecuteNonQuery();
                        }

                        if (RowsAffected > 0)
                        {
                            // Update Temp_Stock
                            foreach (DataGridViewRow row in DataGridView1.Rows)
                            {
                                string cb2 = "UPDATE Temp_Stock SET Qty = Qty + @Qty WHERE ProductID = @d1 AND Barcode = @d2 AND WID = @d3";
                                using (SqlCommand cmd = new SqlCommand(cb2, con, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@Qty", Convert.ToDouble(row.Cells[6].Value));
                                    cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(row.Cells[0].Value));
                                    cmd.Parameters.AddWithValue("@d2", row.Cells[3].Value.ToString());
                                    cmd.Parameters.AddWithValue("@d3", row.Cells[9].Value.ToString());

                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // Perform Ledger operations
                            LedgerDelete(txtPRNO.Text, "مردودات مشتريات من " + txtSupplierName.Text);
                            LedgerDelete(txtPRNO.Text, "مردودات مشتريات");
                            SupplierLedgerDelete(txtPRNO.Text);

                            // Log the deletion
                            string st = "Deleted the Purchase Return record having PR No. '" + txtPRNO.Text + "'";
                            LogFunc(lblUser.Text, st);

                            MessageBox.Show("تم الحذف بنجاح", "السجلات", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            transaction.Commit();
                            Reset();
                        }
                        else
                        {
                            MessageBox.Show("لا يوجد سجلات", "عذرًا", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            transaction.Rollback();
                            Reset();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("خطأ أثناء الحذف: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

      


        private void txtQty_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            var keyChar = e.KeyChar;

            if (char.IsControl(keyChar))
            {
            }
            // Allow all control characters.
            else if (char.IsDigit(keyChar) || keyChar == '.')
            {
                var text = this.txtQty.Text;
                var selectionStart = this.txtQty.SelectionStart;
                var selectionLength = this.txtQty.SelectionLength;

                text = text.Substring(0, selectionStart) + keyChar + text.Substring(selectionStart + selectionLength);

                if (int.TryParse(text, out int intValue) && text.Length > 16)
                {
                    // Reject an integer that is longer than 16 digits.
                    e.Handled = true;
                }
                else if (double.TryParse(text, out double doubleValue))
                {
                    int decimalIndex = text.IndexOf('.');
                    if (decimalIndex != -1 && text.Length - decimalIndex - 1 > 2)
                    {
                        // Reject a real number with too many decimal places.
                        e.Handled = true;
                    }
                    else
                    {
                        e.Handled = false;
                    }
                }
                else
                {
                    e.Handled = false;
                }

            }
            else
            {
                // Reject all other characters.
                e.Handled = true;
            }
        }

       
        private void txtReturnQty_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' | e.KeyChar > '9') & e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }



        private void btnNew_Click(object sender, EventArgs e)
        {
            Reset();

        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
           
            PurchaseReturnScreen frmPurchaseRecord = new PurchaseReturnScreen();
            frmPurchaseRecord.lblSet.Text = "PR";
            frmPurchaseRecord.Reset();
            frmPurchaseRecord.ShowDialog();
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
                    try
                    {
                        if (DataGridView1.Rows.Count > 0)
                        {
                            btnUpdate.Enabled = true;

                            Clear();
                            DataGridViewRow dr = DataGridView1.SelectedRows[0];
                            txtProductID.Text = dr.Cells[0].Value.ToString();
                            txtProductCode.Text = dr.Cells[1].Value.ToString();
                            txtProductName.Text = dr.Cells[2].Value.ToString();
                            txtBarcode.Text = dr.Cells[3].Value.ToString();
                            txtQty.Text = dr.Cells[4].Value.ToString();
                            txtPricePerQty.Text = dr.Cells[5].Value.ToString();
                            WID.Text = dr.Cells[9].Value.ToString();
                            WName.Text = dr.Cells[8].Value.ToString();
                            txtReturnQty.Focus();
                            DataGridView1.Rows.Remove(dr);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

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

        private void DataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void PurchaseReturn_Load(object sender, EventArgs e)
        {
                
        }

        private void DataGridView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (DataGridView2.Rows.Count > 0)
                {
                    Clear();
                    DataGridViewRow dr = DataGridView2.SelectedRows[0];
                    txtProductID.Text = dr.Cells[0].Value.ToString();
                    txtProductCode.Text = dr.Cells[1].Value.ToString();
                    txtProductName.Text = dr.Cells[2].Value.ToString();
                    txtBarcode.Text = dr.Cells[3].Value.ToString();
                    txtQty.Text = dr.Cells[4].Value.ToString();
                    txtPricePerQty.Text = dr.Cells[5].Value.ToString();
                    txtReturnQty.Focus();
                    WName.Text = dr.Cells[7].Value.ToString();
                    WID.Text = dr.Cells[8].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtTotalAmount_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void txtReturnQty_TextChanged(object sender, EventArgs e)
        {
            double i = 0d,j=0d;
            i = (double)(Val(txtReturnQty.Text) * Val(txtPricePerQty.Text) * Val(Cprice.Text));
            i = Math.Round(i, 2);
            j = (double)(Val(txtReturnQty.Text) * Val(txtPricePerQty.Text) );
            j = Math.Round(j, 2);
            txtTotalAmount.Text = i.ToString();
            textBox3.Text = j.ToString();
        }

        private void Label2_Click(object sender, EventArgs e)
        {

        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtPurchaseInvoiceNo.Text))
                {
                    MessageBox.Show("الرجاء إدراج بيانات فاتورة الشراء", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPurchaseInvoiceNo.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtSupplierID.Text))
                {
                    MessageBox.Show("الرجاء استرداد رقم المورد", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSupplierID.Focus();
                    return;
                }

                if (DataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("الرجاء إدراج بيانات الأصناف المرتجعة في شبكة البيانات", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtDiscPer.Text))
                {
                    MessageBox.Show("الرجاء إدراج نسبة الخصم %", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDiscPer.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtRoundOff.Text))
                {
                    MessageBox.Show("الرجاء كتابة التقريب", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtRoundOff.Focus();
                    return;
                }

                // Update the PurchaseReturn table
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string updateQuery = "UPDATE PurchaseReturn SET PRNo=@d2, Date=@d3, SubTotal=@d6, DiscPer=@d7, Discount=@d8, VATPer=@d9, VATAmt=@d10, Total=@d13, RoundOff=@d14, GrandTotal=@d15,Currencies=@d16,CPrice=@d17  WHERE PurchaseID=@d5";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@d2", txtPRNO.Text);
                        cmd.Parameters.AddWithValue("@d3", dtpPRDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d5", Convert.ToInt32(txtPurchaseID.Text));
                        cmd.Parameters.AddWithValue("@d6", Convert.ToDouble(txtSubTotal.Text));
                        cmd.Parameters.AddWithValue("@d7", Convert.ToDouble(txtDiscPer.Text));
                        cmd.Parameters.AddWithValue("@d8", Convert.ToDouble(txtDisc.Text));
                        cmd.Parameters.AddWithValue("@d9", Convert.ToDouble(txtVatPer.Text));
                        cmd.Parameters.AddWithValue("@d10", Convert.ToDouble(txtVATAmt.Text));
                        cmd.Parameters.AddWithValue("@d13", Convert.ToDouble(txtTotal.Text));
                        cmd.Parameters.AddWithValue("@d14", Convert.ToDouble(txtRoundOff.Text));
                        cmd.Parameters.AddWithValue("@d15", Convert.ToDouble(txtGrandTotal.Text));
                        cmd.Parameters.AddWithValue("@d16", comboBox1.Text);
                        cmd.Parameters.AddWithValue("@d17", decimal.TryParse(Cprice.Text, out decimal currencyPrice) ? currencyPrice : 0);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Update the PurchaseReturn_Join table
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    foreach (DataGridViewRow row in DataGridView1.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            string updateJoinQuery = "UPDATE PurchaseReturn_Join SET ProductID=@d1, Barcode=@d2, Qty=@d3, Price=@d4, ReturnQty=@d5, TotalAmount=@d6 WHERE PurchaseReturnID=" + Convert.ToInt32(txtPRID.Text) + " AND ProductID=@d1";
                            using (SqlCommand cmd = new SqlCommand(updateJoinQuery, con))
                            {
                                cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(row.Cells[0].Value));
                                cmd.Parameters.AddWithValue("@d2", row.Cells[3].Value.ToString());
                                cmd.Parameters.AddWithValue("@d3", Convert.ToDouble(row.Cells[4].Value));
                                cmd.Parameters.AddWithValue("@d4", Convert.ToDouble(row.Cells[5].Value));
                                cmd.Parameters.AddWithValue("@d5", Convert.ToDouble(row.Cells[6].Value));
                                cmd.Parameters.AddWithValue("@d6", Convert.ToDouble(row.Cells[7].Value));
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                        }
                    }
                }

                foreach (DataGridViewRow row in DataGridView1.Rows)
                {
                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();
                        string updateStockQuery = "Update Temp_Stock set Qty=Qty + @qty where ProductID=@d1 and Barcode=@d2 and WID=@d3";
                        using (SqlCommand cmd = new SqlCommand(updateStockQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@qty", Convert.ToDouble(row.Cells[6].Value));
                            cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(row.Cells[0].Value));
                            cmd.Parameters.AddWithValue("@d2", row.Cells[3].Value.ToString());
                            cmd.Parameters.AddWithValue("@d3", row.Cells[9].Value.ToString());
                            cmd.ExecuteReader();
                        }
                    }
                }

                // Update Ledger
                SupplierLedgerSave(dtpPRDate.Value.Date, txtSupplierName.Text, txtPRNO.Text, "مردودات مشتريات", Convert.ToDecimal(txtGrandTotal.Text), 0, txtSupplierID.Text, comboBox1.Text, Convert.ToDecimal(Cprice.Text));
                LedgerSave(dtpPRDate.Value.Date, txtSupplierName.Text, txtPRNO.Text, "مردودات مشتريات", Convert.ToDecimal(txtGrandTotal.Text), 0, txtSupplierID.Text, "");
                LedgerSave(dtpPRDate.Value.Date, "نقدا", txtPRNO.Text, "مردودات مشتريات من " + txtSupplierName.Text + "", 0, Convert.ToDecimal(txtGrandTotal.Text), txtSupplierID.Text, "");
                
                // Log the update
                LogFunc(lblUser.Text, "updated the Purchase return record having PR No. '" + txtPRNO.Text + "'");
                MessageBox.Show("تم التحديث بنجاح", "السجلات", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnUpdate.Enabled = false;
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtVatPer_TextChanged_1(object sender, EventArgs e)
        {
            Compute();

        }

        private void txtDisc_TextChanged_1(object sender, EventArgs e)
        {
            Compute();

        }

        private void txtVATAmt_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void txtDiscPer_TextChanged_1(object sender, EventArgs e)
        {
            Compute();

        }

        private void txtRoundOff_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GenerateCurrencies();
        }
        public void GenerateCurrencies()
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
                                    double price = rdr["Price"] != DBNull.Value ? Convert.ToDouble(rdr["Price"]) : 0;

                                    // Step 8: Assign the retrieved values to the respective TextBoxes
                                    CUID.Text = currencyId.ToString();
                                    Cprice.Text = price.ToString();
                                }

                            }
                        }


                    }
                    catch (SqlException sqlEx)
                    {
                        // Step 9: Hand le SQL-related exceptions specifically
                        MessageBox.Show($"A database error occurred: {sqlEx.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        // Step 10: Handle any other general exceptions
                        MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                  

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"A database error occurred: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }


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

        private void txtReturnQty_KeyPress_1(object sender, KeyPressEventArgs e)
        {


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
        public static void SMS(string st1)
        {
            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    cn.Open();
                    string query = "INSERT INTO SMS(Message, Date) VALUES (@d1, @d2)";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", st1);
                        cmd.Parameters.AddWithValue("@d2", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static void LogFunc(string st1, string st2)
        {
            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    cn.Open();
                    string query = "INSERT INTO Logs(UserID, Date, Operation) VALUES (@d1, @d2, @d3)";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", st1);
                        cmd.Parameters.AddWithValue("@d2", DateTime.Now);
                        cmd.Parameters.AddWithValue("@d3", st2);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

        public static void SMSFunc(string st1, string st2, string st3)
        {
            st3 = st3.Replace("@MobileNo", st1).Replace("@Message", st2);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(st3));
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // Handle response if needed
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static string Encrypt(string password)
        {
            byte[] encode = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(encode);
        }

        public static string Decrypt(string encryptpwd)
        {
            byte[] todecode_byte = Convert.FromBase64String(encryptpwd);
            return Encoding.UTF8.GetString(todecode_byte);
        }

        public static void LedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g, string h)
        {
            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    cn.Open();
                    string query = "INSERT INTO LedgerBook(Date, Name, LedgerNo, Label, Debit, Credit, PartyID, Manual_Inv) VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8)";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", a);
                        cmd.Parameters.AddWithValue("@d2", b);
                        cmd.Parameters.AddWithValue("@d3", c);
                        cmd.Parameters.AddWithValue("@d4", d);
                        cmd.Parameters.AddWithValue("@d5", e);
                        cmd.Parameters.AddWithValue("@d6", f);
                        cmd.Parameters.AddWithValue("@d7", g);
                        cmd.Parameters.AddWithValue("@d8", h);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static void LedgerDelete(string a, string b)
        {
            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    cn.Open();
                    string query = "DELETE FROM LedgerBook WHERE LedgerNo = @d1 AND Label = @d2";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", a);
                        cmd.Parameters.AddWithValue("@d2", b);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static void LedgerUpdate(DateTime a, string b, decimal e, decimal f, string g, string h, string i)
        {
            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    cn.Open();
                    string query = "UPDATE LedgerBook SET Date = @d1, Name = @d2, Debit = @d3, Credit = @d4, PartyID = @d5 WHERE LedgerNo = @d6 AND Label = @d7";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", a);
                        cmd.Parameters.AddWithValue("@d2", b);
                        cmd.Parameters.AddWithValue("@d3", e);
                        cmd.Parameters.AddWithValue("@d4", f);
                        cmd.Parameters.AddWithValue("@d5", g);
                        cmd.Parameters.AddWithValue("@d6", h);
                        cmd.Parameters.AddWithValue("@d7", i);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
}
