using Pharmacy.DL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Accounting_System
{
    public partial class Payment_2 : Form
    {
        public static Payment_2 _instance;
        public static Payment_2 instance;
        public static Payment_2 Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new Payment_2();
                }
                return _instance;
            }

        }
        int i = 0;
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public Payment_2()
        {
            InitializeComponent();
            txtTransactionAmount.KeyPress += new KeyPressEventHandler(txtTotalPaid_KeyPress);
            cmbPaymentMode.SelectedIndexChanged += new EventHandler(cmbPaymentMode_SelectedIndexChanged);
            txtCommissionPer.KeyPress += new KeyPressEventHandler(txtCommissionPer_KeyPress);
            instance = this ;
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        public void GetSupplierBalance()
        {
            try
            {
                try
                {
                    decimal num1 = 0;
                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();
                        string sql = "SELECT ISNULL(SUM(Credit), 0) - ISNULL(SUM(Debit), 0) FROM LedgerBook WHERE PartyID = @d1 GROUP BY PartyID";
                        using (SqlCommand cmd = new SqlCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@d1", txtSupplierID.Text);
                            using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                            {
                                if (rdr.Read())
                                {
                                    num1 = rdr.GetDecimal(0);
                                }
                            }
                        }
                    }

                    lblBalance.Text = num1.ToString();
                    string str;
                    if (Convert.ToDecimal(lblBalance.Text) >= 0)
                    {
                        str = "دائن";
                    }
                    else
                    {
                        str = "مدين";
                    }

                    lblBalance.Text = Math.Abs(Convert.ToDecimal(lblBalance.Text)).ToString();
                    lblBalance.Text = $"{lblBalance.Text} {str}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                    using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 TC_ID FROM Payment_2 ORDER BY TC_ID DESC", con))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                value = rdr["TC_ID"].ToString();
                            }
                        }
                    }

                    // Increase the ID by 1
                    value = (int.Parse(value) + 1).ToString();

                    // Adjust the value with leading zeros
                    if (int.Parse(value) <= 9)
                    {
                        value = "000" + value;
                    }
                    else if (int.Parse(value) <= 99)
                    {
                        value = "00" + value;
                    }
                    else if (int.Parse(value) <= 999)
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
            }

            return value;
        }

        private void CountValue()
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                string sql = "SELECT COUNT(TC_ID) FROM Payment_2 WHERE Amount = 0";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    object result = cmd.ExecuteScalar();
                    i = result == DBNull.Value ? 0 : Convert.ToInt32(result);
                }
            }
        }
        private void Auto()
        {
            try
            {
                CountValue();
                txtT_ID.Text = GenerateID();
                txtTransactionNo.Text = "TC-" + (Convert.ToInt32(GenerateID()) - i).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void Reset()
        {
            txtAddress.Text = "";
            txtCity.Text = "";
            txtContactNo.Text = "";
            txtRemarks.Text = "";
            txtSupplierID.Text = "";
            txtCheck.Text = "";
            txtBank.Text = "";
            // txtSalesman.Text = "";
            // txtSalesmanID.Text = "";
            // txtSM_ID.Text = "";
            // txtCommissionPer.Text = "";
            txtSupplierName.Text = "";
            txtTransactionAmount.Text = "";
            cmbPaymentMode.SelectedIndex = 0;
            dtpTranactionDate.Value = DateTime.Today;
            dtpCheck.Value = DateTime.Today;
            lblBalance.Text = "0.000";
            btnSave.Enabled = true;
            btnDelete.Enabled = false;
            btnUpdate.Enabled = false;
            btnSelection.Enabled = true;
            Auto();
        }
        public void GetSupplierInfo()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string sql = "SELECT CustomerID, Name, Address, City, ContactNo FROM Customer WHERE ID = @d1";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtSup_ID.Text));
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.Read())
                            {
                                txtSupplierID.Text = rdr.GetValue(0).ToString();
                                txtSupplierName.Text = rdr.GetValue(1).ToString();
                                txtAddress.Text = rdr.GetValue(2).ToString();
                                txtCity.Text = rdr.GetValue(3).ToString();
                                txtContactNo.Text = rdr.GetValue(4).ToString();
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
        private void DeleteRecord()
        {
            try
            {
                int rowsAffected = 0;
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string cq = "DELETE FROM Payment_2 WHERE TC_ID = @d1";
                    using (SqlCommand cmd = new SqlCommand(cq, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtT_ID.Text));
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                }

                if (rowsAffected > 0)
                {
                    SupplierLedgerDelete(txtTransactionNo.Text);
                    LedgerDelete(txtTransactionNo.Text, "سند قبض");
                    LogFunc(lblUser.Text, "deleted the Customer payment record having transaction No. '" + txtTransactionNo.Text + "'");
                    MessageBox.Show("تم الحذف بنجاح", "سندات قبض العملاء", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset();
                }
                else
                {
                    MessageBox.Show("لا يوجد سجلات", "عذرًا", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
        public static void LedgerDelete(string a, string b)
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                string cq = "DELETE FROM LedgerBook WHERE LedgerNo=@d1 AND Label=@d2";
                using (var cmd = new SqlCommand(cq, con))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.Parameters.AddWithValue("@d2", b);
                    cmd.ExecuteReader();
                }
            }
        }
        public static void SupplierLedgerDelete(string a)
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                string cq = "DELETE FROM SupplierLedgerBook WHERE LedgerNo=@d1";
                using (var cmd = new SqlCommand(cq, con))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.ExecuteReader();
                }
            }
        }
        private void txtTotalPaid_KeyPress(object sender, KeyPressEventArgs e) {
            if (int.TryParse(Text, out int _) && Text.Length > 16)
            {
                // Reject an integer that is longer than 16 digits.
                e.Handled = true;
            }
            else if (double.TryParse(Text, out double _) && Text.IndexOf('.') < Text.Length - 3)
            {
                // Reject a real number with too many decimal places.
                e.Handled = true;
            }

        }

        private void btnSelection_Click(object sender, EventArgs e)
        {
            this.Close();
            Reset();
            CustomerRecord3 frmCustomerRecord3 = new CustomerRecord3();
            frmCustomerRecord3.lblSet.Text = "سندات قبض العملاء";
            frmCustomerRecord3.ShowDialog();

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtSupplierID.Text.Trim().Length == 0)
            {
                MessageBox.Show("الرجاء إدراج رقم العميل", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSupplierID.Focus();
                return;
            }
            if (txtTransactionAmount.Text.Trim().Length == 0)
            {
                MessageBox.Show("الرجاء كتابة المبلغ المدفوع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTransactionAmount.Focus();
                return;
            }
            if (Convert.ToDecimal(txtTransactionAmount.Text) == 0)
            {
                MessageBox.Show("مبلغ السند يجب أن يكون أكبر من صفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTransactionAmount.Focus();
                return;
            }
            if (txtSalesmanID.Text.Trim().Length == 0)
            {
                MessageBox.Show("الرجاء اختيار المندوب الذي حصل الشيك", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSalesman.Focus();
                return;
            }
            /*
            if (Convert.ToDecimal(txtCommissionPer.Text) == 0)
            {
                MessageBox.Show("مبلغ نسبة المندوب يجب أن يكون أكبر من صفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCommissionPer.Focus();
                return;
            }
            if (Convert.ToDecimal(txtTransactionAmount.Text) > Convert.ToDecimal(lblBalance.Text))
            {
                MessageBox.Show("مبلغ السند يجب أن لا يكون أكبر من رصيد العميل", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTransactionAmount.Focus();
                return;
            }
            */

            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                string cb = "INSERT INTO Payment_2(TC_ID, TransactionID, Date, PaymentMode, CustomerID, Amount, Remarks, Check_ID, Check_Date, Bank, SalesMan_ID, SalesMan_Name, SalesMan_Comession, SalesMan_ID_2) " +
                            "VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9, @d10, @d11, @d12, @d13, @d14)";
                using (SqlCommand cmd = new SqlCommand(cb, con))
                {
                    cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtT_ID.Text));
                    cmd.Parameters.AddWithValue("@d2", txtTransactionNo.Text);
                    cmd.Parameters.AddWithValue("@d3", dtpTranactionDate.Value.Date);
                    cmd.Parameters.AddWithValue("@d4", cmbPaymentMode.Text);
                    cmd.Parameters.AddWithValue("@d5", txtSupplierID.Text);
                    cmd.Parameters.AddWithValue("@d6", Convert.ToDecimal(txtTransactionAmount.Text));
                    cmd.Parameters.AddWithValue("@d7", txtRemarks.Text);
                    cmd.Parameters.AddWithValue("@d8", txtCheck.Text);
                    cmd.Parameters.AddWithValue("@d9", dtpCheck.Value.Date);
                    cmd.Parameters.AddWithValue("@d10", txtBank.Text);
                    cmd.Parameters.AddWithValue("@d11", txtSM_ID.Text);
                    cmd.Parameters.AddWithValue("@d12", txtSalesman.Text);
                    cmd.Parameters.AddWithValue("@d13", txtCommissionPer.Text);
                    cmd.Parameters.AddWithValue("@d14", txtSalesmanID.Text);

                    cmd.ExecuteNonQuery();
                }
            }
            try {

                if (cmbPaymentMode.SelectedIndex == 0)
                {
                    if (Convert.ToDecimal(txtTransactionAmount.Text) < 0)
                    {
                        LedgerSave(dtpTranactionDate.Value.Date, "نقدا", txtTransactionNo.Text, "سند قبض", Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text)), 0, txtSupplierID.Text, "");
                    }
                    else
                    {
                        LedgerSave(dtpTranactionDate.Value.Date, "نقدا", txtTransactionNo.Text, "سند قبض", 0, Convert.ToDecimal(txtTransactionAmount.Text), txtSupplierID.Text, "");
                    }
                }
                if (cmbPaymentMode.SelectedIndex == 1)
                {
                    if (Convert.ToDecimal(txtTransactionAmount.Text) < 0)
                    {
                        LedgerSave(dtpTranactionDate.Value.Date, "شيك رقم" + txtCheck.Text.Trim(), txtTransactionNo.Text, "سند قبض", Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text)), 0, txtSupplierID.Text, "");
                    }
                    else
                    {
                        LedgerSave(dtpTranactionDate.Value.Date, "شيك رقم" + txtCheck.Text.Trim(), txtTransactionNo.Text, "سند قبض", 0, Convert.ToDecimal(txtTransactionAmount.Text), txtSupplierID.Text, "");
                    }
                }
                if (cmbPaymentMode.SelectedIndex == 2)
                {
                    if (Convert.ToDecimal(txtTransactionAmount.Text) < 0)
                    {
                        LedgerSave(dtpTranactionDate.Value.Date, "حوالة بنكية رقم" + txtCheck.Text.Trim(), txtTransactionNo.Text, "سند قبض", Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text)), 0, txtSupplierID.Text, "");
                    }
                    else
                    {
                        LedgerSave(dtpTranactionDate.Value.Date, "حوالة بنكية رقم" + txtCheck.Text.Trim(), txtTransactionNo.Text, "سند قبض", 0, Convert.ToDecimal(txtTransactionAmount.Text), txtSupplierID.Text, "");
                    }
                }

                /*
                if (cmbPaymentMode.SelectedIndex == 0)
                {
                    SupplierLedgerSave(dtpTranactionDate.Value.Date, "نقدا", txtTransactionNo.Text, "سند قبض", Convert.ToDecimal(txtTransactionAmount.Text), 0, txtSupplierID.Text);
                }
                if (cmbPaymentMode.SelectedIndex == 1)
                {
                    SupplierLedgerSave(dtpTranactionDate.Value.Date, "شيك", txtTransactionNo.Text, "سند قبض", Convert.ToDecimal(txtTransactionAmount.Text), 0, txtSupplierID.Text);
                }
                */
                LogFunc(lblUser.Text, "added the new payment having transaction No. '" + txtTransactionNo.Text + "'");
            MessageBox.Show("تم الحفظ بنجاح", "سندات قبض العملاء", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnSave.Enabled = false;
            Reset();
        }
            
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static void LedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g, string h)
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                string cb = "INSERT INTO LedgerBook(Date, Name, LedgerNo, Label, Debit, Credit, PartyID, Manual_Inv) VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8)";
                using (var cmd = new SqlCommand(cb, con))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.Parameters.AddWithValue("@d2", b);
                    cmd.Parameters.AddWithValue("@d3", c);
                    cmd.Parameters.AddWithValue("@d4", d);
                    cmd.Parameters.AddWithValue("@d5", e);
                    cmd.Parameters.AddWithValue("@d6", f);
                    cmd.Parameters.AddWithValue("@d7", g);
                    cmd.Parameters.AddWithValue("@d8", h);
                    cmd.ExecuteReader();
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtSupplierID.Text.Trim().Length == 0)
            {
                MessageBox.Show("الرجاء إدراج رقم العميل", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSupplierID.Focus();
                return;
            }
            if (txtTransactionAmount.Text.Trim().Length == 0)
            {
                MessageBox.Show("الرجاء كتابة المبلغ المدفوع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTransactionAmount.Focus();
                return;
            }
            if (Convert.ToDecimal(txtTransactionAmount.Text) == 0)
            {
                MessageBox.Show("مبلغ السند يجب أن يكون أكبر من صفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTransactionAmount.Focus();
                return;
            }
            /*
            if (Convert.ToDecimal(txtTransactionAmount.Text) > Convert.ToDecimal(lblBalance.Text))
            {
                MessageBox.Show("مبلغ السند يجب أن لا يكون أكبر من رصيد العميل", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTransactionAmount.Focus();
                return;
            }
            */
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string cb = "UPDATE Payment_2 SET TransactionID = @d2, Date = @d3, PaymentMode = @d4, CustomerID = @d5, Amount = @d6, Remarks = @d7, Check_ID = @d8, Check_Date = @d9, Bank = @d10, SalesMan_ID = @d11, SalesMan_Name = @d12, SalesMan_Comession = @d13, SalesMan_ID_2 = @d14 WHERE TC_ID = @d1";
                    using (SqlCommand cmd = new SqlCommand(cb, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtT_ID.Text));
                        cmd.Parameters.AddWithValue("@d2", txtTransactionNo.Text);
                        cmd.Parameters.AddWithValue("@d3", dtpTranactionDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d4", cmbPaymentMode.Text);
                        cmd.Parameters.AddWithValue("@d5", txtSupplierID.Text);
                        cmd.Parameters.AddWithValue("@d6", Convert.ToDecimal(txtTransactionAmount.Text));
                        cmd.Parameters.AddWithValue("@d7", txtRemarks.Text);
                        cmd.Parameters.AddWithValue("@d8", txtCheck.Text);
                        cmd.Parameters.AddWithValue("@d9", dtpCheck.Value.Date);
                        cmd.Parameters.AddWithValue("@d10", txtBank.Text);
                        cmd.Parameters.AddWithValue("@d11", txtSM_ID.Text);
                        cmd.Parameters.AddWithValue("@d12", txtSalesman.Text);
                        cmd.Parameters.AddWithValue("@d13", txtCommissionPer.Text);
                        cmd.Parameters.AddWithValue("@d14", txtSalesmanID.Text);

                        cmd.ExecuteNonQuery();
                    }
                }

                if (cmbPaymentMode.SelectedIndex == 0)
                {
                    if (Convert.ToDecimal(txtTransactionAmount.Text) < 0)
                    {
                        LedgerUpdate(dtpTranactionDate.Value.Date, "نقدا", Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text)), 0, txtSupplierID.Text, txtTransactionNo.Text, "سند قبض");
                    }
                    else
                    {
                        LedgerUpdate(dtpTranactionDate.Value.Date, "نقدا", 0, Convert.ToDecimal(txtTransactionAmount.Text), txtSupplierID.Text, txtTransactionNo.Text, "سند قبض");
                    }
                }
                if (cmbPaymentMode.SelectedIndex == 1)
                {
                    if (Convert.ToDecimal(txtTransactionAmount.Text) < 0)
                    {
                        LedgerUpdate(dtpTranactionDate.Value.Date, "شيك رقم" + txtCheck.Text.Trim(), Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text)), 0, txtSupplierID.Text, txtTransactionNo.Text, "سند قبض");
                    }
                    else
                    {
                        LedgerUpdate(dtpTranactionDate.Value.Date, "شيك رقم" + txtCheck.Text.Trim(), 0, Convert.ToDecimal(txtTransactionAmount.Text), txtSupplierID.Text, txtTransactionNo.Text, "سند قبض");
                    }
                }
                if (cmbPaymentMode.SelectedIndex == 2)
                {
                    if (Convert.ToDecimal(txtTransactionAmount.Text) < 0)
                    {
                        LedgerUpdate(dtpTranactionDate.Value.Date, "حوالة بنكية رقم" + txtCheck.Text.Trim(), Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text)), 0, txtSupplierID.Text, txtTransactionNo.Text, "سند قبض");
                    }
                    else
                    {
                        LedgerUpdate(dtpTranactionDate.Value.Date, "حوالة بنكية رقم" + txtCheck.Text.Trim(), 0, Convert.ToDecimal(txtTransactionAmount.Text), txtSupplierID.Text, txtTransactionNo.Text, "سند قبض");
                    }
                }

                LogFunc(lblUser.Text, "updated Customer payment record having transaction No. '" + txtTransactionNo.Text + "'");
                MessageBox.Show("تم التعديل بنجاح", "سندات قبض العملاء", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnUpdate.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static void LedgerUpdate(DateTime a, string b, decimal e, decimal f, string g, string h, string i)
        {
            using (SqlConnection con = DataAccessLayer.cn)
            {
                con.Open();
                string cb = "UPDATE LedgerBook SET Date=@d1, Name=@d2, Debit=@d3, Credit=@d4, PartyID=@d5 WHERE LedgerNo=@d6 AND Label=@d7";
                using (var cmd = new SqlCommand(cb, con))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.Parameters.AddWithValue("@d2", b);
                    cmd.Parameters.AddWithValue("@d3", e);
                    cmd.Parameters.AddWithValue("@d4", f);
                    cmd.Parameters.AddWithValue("@d5", g);
                    cmd.Parameters.AddWithValue("@d6", h);
                    cmd.Parameters.AddWithValue("@d7", i);
                    cmd.ExecuteReader();
                }
            }
        }
        private void cmbPaymentMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPaymentMode.SelectedIndex == 0)
            {
                txtCheck.Enabled = false;
                dtpCheck.Enabled = false;
                txtBank.Enabled = false;
            }
            else
            {
                txtCheck.Enabled = true;
                dtpCheck.Enabled = true;
                txtBank.Enabled = true;
            }

        }

        private void Button1_Click(object sender, EventArgs e)
        {

            SalesmanRecord_2 frmSalesmanRecord_2 = new SalesmanRecord_2();
            frmSalesmanRecord_2.lblSet.Text = "";
            frmSalesmanRecord_2.Reset();
            frmSalesmanRecord_2.ShowDialog();

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("هل أنت متأكد أنك تريد حذف هذا السند من السجلات?", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    DeleteRecord();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            this.Close(); // Hide the current form

            PaymentRecord_2 frmPaymentRecord_2 = new PaymentRecord_2();
            frmPaymentRecord_2.lblSet.Text = "سندات قبض العملاء";
            frmPaymentRecord_2.Reset();
            frmPaymentRecord_2.ShowDialog();

           
            this.Show(); 

        }
        private void txtCommissionPer_KeyPress(object sender, KeyPressEventArgs e)
        {
            char keyChar = e.KeyChar;

            if (char.IsControl(keyChar))
            {
                // Allow all control characters.
            }
            else if (char.IsDigit(keyChar) || keyChar == '.')
            {
                string text = txtCommissionPer.Text;
                int selectionStart = txtCommissionPer.SelectionStart;
                int selectionLength = txtCommissionPer.SelectionLength;

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
            }
        }

        private void GroupBox3_Enter(object sender, EventArgs e)
        {

        }
    }
}
