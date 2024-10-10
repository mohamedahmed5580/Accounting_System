using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;
using Pharmacy.DL;
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
                    sum = sum + Convert.ToDouble(r.Cells[7].Value);
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
                    DataGridView1.Rows.Add(Val(txtProductID.Text), txtProductCode.Text, txtProductName.Text, txtBarcode.Text, Val(txtQty.Text), Val(txtPricePerQty.Text), Val(txtReturnQty.Text), Val(txtTotalAmount.Text));
                    double k = 0d;
                    k = SubTotal();
                    k = Math.Round(k, 2);
                    txtSubTotal.Text = k.ToString();
                    Compute();
                    Clear();
                    return;
                }
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
                DataGridView1.Rows.Add(Val(txtProductID.Text), txtProductCode.Text, txtProductName.Text, txtBarcode.Text, Val(txtQty.Text), Val(txtPricePerQty.Text), Val(txtReturnQty.Text), Val(txtTotalAmount.Text));
                double k1 = 0d;
                k1 = SubTotal();
                k1 = Math.Round(k1, 2);
                txtSubTotal.Text = k1.ToString();
                Clear();
                Compute();
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

       

        private void btnSave_Click(object sender, EventArgs e)
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

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string ct = "select PurchaseID from PurchaseReturn where PurchaseID=@d1";
                    using (SqlCommand cmd = new SqlCommand(ct, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtPurchaseID.Text));
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                MessageBox.Show("هذا الصنف في فاتورة الشراء هذه بالفعل تم إرجاعه", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }
                }

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string cb = "insert into PurchaseReturn(PR_ID, PRNo, Date, PurchaseID, SubTotal, DiscPer, Discount, VATPer, VATAmt, Total, RoundOff, GrandTotal) " +
                                "VALUES (@d1, @d2, @d3, @d5, @d6, @d7, @d8, @d9, @d10, @d13, @d14, @d15)";
                    using (SqlCommand cmd = new SqlCommand(cb, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtPRID.Text));
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
                        cmd.ExecuteNonQuery();
                    }
                }

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string cb1 = "insert into PurchaseReturn_Join(PurchaseReturnID, ProductID, Barcode, Qty, Price, ReturnQty, TotalAmount) " +
                                 "VALUES (" + Convert.ToInt32(txtPRID.Text) + ", @d1, @d2, @d3, @d4, @d5, @d6)";
                    using (SqlCommand cmd = new SqlCommand(cb1, con))
                    {
                        cmd.Prepare();
                        foreach (DataGridViewRow row in DataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
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
                        string cb2 = "Update Temp_Stock set Qty=Qty - " + Convert.ToDouble(row.Cells[6].Value) + " where ProductID=@d1 and Barcode=@d2";
                        using (SqlCommand cmd = new SqlCommand(cb2, con))
                        {
                            cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(row.Cells[0].Value));
                            cmd.Parameters.AddWithValue("@d2", row.Cells[3].Value.ToString());
                            cmd.ExecuteReader();
                        }
                    }
                }

                SupplierLedgerSave(dtpPRDate.Value.Date, txtSupplierName.Text, txtPRNO.Text, "مردودات مشتريات", Convert.ToDecimal(txtGrandTotal.Text), 0, txtSupplierID.Text);
                LedgerSave(dtpPRDate.Value.Date, txtSupplierName.Text, txtPRNO.Text, "مردودات مشتريات", Convert.ToDecimal(txtGrandTotal.Text), 0, txtSupplierID.Text, "");
                LedgerSave(dtpPRDate.Value.Date, "نقدا", txtPRNO.Text, "مردودات مشتريات من " + txtSupplierName.Text + "", 0, Convert.ToDecimal(txtGrandTotal.Text), txtSupplierID.Text,"");
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

        private void DeleteRecord()
        {
            try
            {
                int RowsAffected = 0;

                // Using statement to manage the SqlConnection
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // Delete from PurchaseReturn
                    string cq = "delete from PurchaseReturn where PR_ID=@d1";
                    using (SqlCommand cmd = new SqlCommand(cq, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtPRID.Text));
                        RowsAffected = cmd.ExecuteNonQuery();
                    }

                    if (RowsAffected > 0)
                    {
                        // Update Temp_Stock
                        foreach (DataGridViewRow row in DataGridView1.Rows)
                        {
                            string cb2 = "Update Temp_Stock set Qty=Qty + @Qty where ProductID=@d1 and Barcode=@d2";
                            using (SqlCommand cmd = new SqlCommand(cb2, con))
                            {
                                cmd.Parameters.AddWithValue("@Qty", Convert.ToDouble(row.Cells[6].Value));
                                cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(row.Cells[0].Value));
                                cmd.Parameters.AddWithValue("@d2", row.Cells[3].Value.ToString());
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // Perform Ledger operations
                        LedgerDelete(txtPRNO.Text, "مردودات مشتريات من " + txtSupplierName.Text + "");
                        LedgerDelete(txtPRNO.Text, "مردودات مشتريات");
                        SupplierLedgerDelete(txtPRNO.Text);

                        // Log the deletion
                        string st = "deleted the Purchase Return record having PR No. '" + txtPRNO.Text + "'";
                        LogFunc(lblUser.Text, st);

                        MessageBox.Show("تم الحذف بنجاح", "السجلات", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Reset();
                    }
                    else
                    {
                        MessageBox.Show("لا يوجد سجلات", "عذرًا", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Reset();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            double i = 0d;
            i = (double)(Val(txtReturnQty.Text) * Val(txtPricePerQty.Text));
            i = Math.Round(i, 2);
            txtTotalAmount.Text = i.ToString();
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
                    string updateQuery = "UPDATE PurchaseReturn SET PRNo=@d2, Date=@d3, SubTotal=@d6, DiscPer=@d7, Discount=@d8, VATPer=@d9, VATAmt=@d10, Total=@d13, RoundOff=@d14, GrandTotal=@d15 WHERE PurchaseID=@d5";
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
                        string updateStockQuery = "Update Temp_Stock set Qty=Qty + @qty where ProductID=@d1 and Barcode=@d2";
                        using (SqlCommand cmd = new SqlCommand(updateStockQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@qty", Convert.ToDouble(row.Cells[6].Value));
                            cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(row.Cells[0].Value));
                            cmd.Parameters.AddWithValue("@d2", row.Cells[3].Value.ToString());
                            cmd.ExecuteReader();
                        }
                    }
                }

                // Update Ledger
                SupplierLedgerSave(dtpPRDate.Value.Date, txtSupplierName.Text, txtPRNO.Text, "مردودات مشتريات", Convert.ToDecimal(txtGrandTotal.Text), 0, txtSupplierID.Text);
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
