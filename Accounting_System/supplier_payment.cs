using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pharmacy.DL;


namespace Accounting_System
{
    public partial class supplier_payment : Form
    {
        SqlConnection cn= new SqlConnection(DataAccessLayer.Con());
        private string str;
        private string OBType;
        private decimal num1, num2, num3, num4;
        private int i = 0;
        public void LogFunc(string st1, string st2)
        {
            
            
                cn.Open();
                string cb = "INSERT INTO Logs(UserID, Date, Operation) VALUES (@d1, @d2, @d3)";
                using (var cmd = new SqlCommand(cb, cn))
                {
                    cmd.Parameters.AddWithValue("@d1", st1);
                    cmd.Parameters.AddWithValue("@d2", DateTime.Now);
                    cmd.Parameters.AddWithValue("@d3", st2);
                    cmd.ExecuteReader();
                }
                cn.Close();
            
        }
        public void SupplierLedgerDelete(string a)
        {
            
            
                cn.Open();
                string cq = "DELETE FROM SupplierLedgerBook WHERE LedgerNo=@d1";
                using (var cmd = new SqlCommand(cq, cn))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.ExecuteReader();
                }
                cn.Close();

        }
        public  void LedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g, string h)
        {
            
            
                cn.Open();
                string cb = "INSERT INTO LedgerBook(Date, Name, LedgerNo, Label, Debit, Credit, PartyID, Manual_Inv) VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8)";
                using (var cmd = new SqlCommand(cb, cn))
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
               cn.Close();

        }

        public void LedgerDelete(string a, string b)
        {

            
                cn.Open();
                string cq = "DELETE FROM LedgerBook WHERE LedgerNo=@d1 AND Label=@d2";
                using (var cmd = new SqlCommand(cq, cn))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.Parameters.AddWithValue("@d2", b);
                    cmd.ExecuteReader();
                }
                cn.Close();

        }

        public  void LedgerUpdate(DateTime a, string b, decimal e, decimal f, string g, string h, string i)
        {

            
                cn.Open();
                string cb = "UPDATE LedgerBook SET Date=@d1, Name=@d2, Debit=@d3, Credit=@d4, PartyID=@d5 WHERE LedgerNo=@d6 AND Label=@d7";
                using (var cmd = new SqlCommand(cb, cn))
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
            cn.Close();

        }

        public void SupplierLedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g)
        {
            
            
                cn.Open();
                string cb = "INSERT INTO SupplierLedgerBook(Date, Name, LedgerNo, Label, Debit, Credit, PartyID) VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7)";
                using (var cmd = new SqlCommand(cb, cn))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.Parameters.AddWithValue("@d2", b);
                    cmd.Parameters.AddWithValue("@d3", c);
                    cmd.Parameters.AddWithValue("@d4", d);
                    cmd.Parameters.AddWithValue("@d5", e);
                    cmd.Parameters.AddWithValue("@d6", f);
                    cmd.Parameters.AddWithValue("@d7", g);
                    cmd.ExecuteReader();
                }
            cn.Close();

        }

        public void GetSupplierBalance()
        {
            try
            {
                try
                {
                    num1 = 0;
                    
                    {
                        cn.Open();
                        string sql = "SELECT ISNULL(SUM(Credit), 0) - ISNULL(SUM(Debit), 0) FROM SupplierLedgerBook WHERE PartyID=@d1 GROUP BY PartyID";
                        using (SqlCommand cmd = new SqlCommand(sql, cn))
                        {
                            cmd.Parameters.AddWithValue("@d1", txtSupplierID.Text);
                            using (SqlDataReader rdr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection))
                            {
                                if (rdr.Read())
                                {
                                    num1 = rdr.GetDecimal(0);
                                }
                            }
                        }
                    }
                    lblBalance.Text = num1.ToString();
                    if (decimal.Parse(lblBalance.Text) >= 0)
                    {
                        str = "دائن";
                    }
                    else if (decimal.Parse(lblBalance.Text) < 0)
                    {
                        str = "مدين";
                    }
                    lblBalance.Text = Math.Abs(decimal.Parse(lblBalance.Text)).ToString();
                    lblBalance.Text = (lblBalance.Text + " " + str).ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                cn.Close();
            }
            
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerateID()
        {
            string value = "0000";
            try
            {
                
                
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 T_ID FROM Payment ORDER BY T_ID DESC", cn))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                value = rdr["T_ID"].ToString();
                            }
                        }
                    }
                cn.Close();
                value = (int.Parse(value) + 1).ToString();
                    value = value.PadLeft(4, '0');
                
            }
            catch (Exception ex)
            {
                if (DataAccessLayer.cn.State == System.Data.ConnectionState.Open)
                {
                    DataAccessLayer.cn.Close();
                }
                value = "0000";
            }
            return value;
        }

        private void CountValue()
        {
            
            
                cn.Open();
                string sql = "SELECT COUNT(T_ID) FROM Payment WHERE Amount=0";
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    i = (int)cmd.ExecuteScalar();
                }
            cn.Close();

        }

        private void Auto()
        {
            try
            {
                CountValue();
                txtT_ID.Text = GenerateID();
                txtTransactionNo.Text = "T-" + (int.Parse(GenerateID()) - i).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Reset()
        {
            txtAddress.Text = "";
            txtCity.Text = "";
            txtContactNo.Text = "";
            txtRemarks.Text = "";
            txtSupplierID.Text = "";
            txtSupplierName.Text = "";
            txtTransactionAmount.Text = "";
            cmbPaymentMode.SelectedIndex = 0;
            dtpTranactionDate.Value = DateTime.Today;
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
                
                
                    cn.Open();
                    string sql = "SELECT SupplierID, Name, Address, City, ContactNo FROM Supplier WHERE ID=@d1";
                    using (SqlCommand cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", int.Parse(txtSup_ID.Text));
                        using (SqlDataReader rdr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection))
                        {
                            if (rdr.Read())
                            {
                                txtSupplierID.Text = rdr["SupplierID"].ToString();
                                txtSupplierName.Text = rdr["Name"].ToString();
                                txtAddress.Text = rdr["Address"].ToString();
                                txtCity.Text = rdr["City"].ToString();
                                txtContactNo.Text = rdr["ContactNo"].ToString();
                            }
                        }
                    }
                    cn.Close();
                
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
                
                
                    cn.Open();
                    string cq = "DELETE FROM Payment WHERE T_ID=@d1";
                    using (SqlCommand cmd = new SqlCommand(cq, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", int.Parse(txtT_ID.Text));
                        RowsAffected = cmd.ExecuteNonQuery();
                    }
                    cn.Close() ;
                
                if (RowsAffected > 0)
                {
                    SupplierLedgerDelete(txtTransactionNo.Text);
                    LedgerDelete(txtTransactionNo.Text, "سند دفع");
                    LogFunc(lblUser.Text, "deleted the payment record having transaction No. '" + txtTransactionNo.Text + "'");
                    MessageBox.Show("تم الحذف بنجاح", "سندات الدفع", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset();
                }
                
                else
                {
                    MessageBox.Show("لا يوجد سجلات", "عذرًا", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset();
                }
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtTotalPaid_KeyPress(object sender, KeyPressEventArgs e)
        {
            var keyChar = e.KeyChar;
            var text = txtTransactionAmount.Text;
            var selectionStart = txtTransactionAmount.SelectionStart;
            var selectionLength = txtTransactionAmount.SelectionLength;

            text = text.Substring(0, selectionStart) + keyChar + text.Substring(selectionStart + selectionLength);

            if (int.TryParse(text, out int result) && text.Length > 16)
            {
                e.Handled = true;
            }
            else if (double.TryParse(text, out double result2) && text.IndexOf('.') < text.Length - 3)
            {
                e.Handled = false;
            }
        }

        private void txtTransactionAmount_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Validation logic
        }

        private void supplier_payment_Load(object sender, EventArgs e)
        {
          
        }

        private void btnNew_Click_1(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            if (txtSupplierID.Text.Trim().Length == 0)
            {
                MessageBox.Show("الرجاء إدراج رقم المورد", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            // Uncomment and modify if you need to compare with lblBalance.Text
            // if (Convert.ToDecimal(txtTransactionAmount.Text) > Convert.ToDecimal(lblBalance.Text))
            // {
            //     MessageBox.Show("مبلغ السند يجب أن لا يكون أكبر من رصيد المورد", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //     txtTransactionAmount.Focus();
            //     return;
            // }

            try
            {
                
                
                    cn.Open();
                    string cb = "insert into Payment(T_ID, TransactionID, Date, PaymentMode, SupplierID, Amount, Remarks) VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7)";
                    using (SqlCommand cmd = new SqlCommand(cb, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtT_ID.Text));
                        cmd.Parameters.AddWithValue("@d2", txtTransactionNo.Text);
                        cmd.Parameters.AddWithValue("@d3", dtpTranactionDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d4", cmbPaymentMode.Text);
                        cmd.Parameters.AddWithValue("@d5", Convert.ToInt32(txtSup_ID.Text));
                        cmd.Parameters.AddWithValue("@d6", Convert.ToDecimal(txtTransactionAmount.Text));
                        cmd.Parameters.AddWithValue("@d7", txtRemarks.Text);
                        cmd.ExecuteNonQuery();
                    }
                cn.Close();

                if (cmbPaymentMode.SelectedIndex == 0)
                {
                    LedgerSave(dtpTranactionDate.Value.Date, "نقدا", txtTransactionNo.Text, "سند دفع", Convert.ToDecimal(txtTransactionAmount.Text), 0, txtSupplierID.Text, "");
                }
                if (cmbPaymentMode.SelectedIndex == 1)
                {
                    LedgerSave(dtpTranactionDate.Value.Date, "شيك", txtTransactionNo.Text, "سند دفع", Convert.ToDecimal(txtTransactionAmount.Text), 0, txtSupplierID.Text, "");
                }
                if (cmbPaymentMode.SelectedIndex == 0)
                {
                    SupplierLedgerSave(dtpTranactionDate.Value.Date, "نقدا", txtTransactionNo.Text, "سند دفع", Convert.ToDecimal(txtTransactionAmount.Text), 0, txtSupplierID.Text);
                }
                if (cmbPaymentMode.SelectedIndex == 1)
                {
                    SupplierLedgerSave(dtpTranactionDate.Value.Date, "شيك", txtTransactionNo.Text, "سند دفع", Convert.ToDecimal(txtTransactionAmount.Text), 0, txtSupplierID.Text);
                }

                LogFunc(lblUser.Text, "added the new payment having transaction No. '" + txtTransactionNo.Text + "'");
                MessageBox.Show("تم الحفظ بنجاح", "سندات دفع الموردين", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
                Reset();
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("هل تريد الحذف بالتأكيد؟", "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DeleteRecord();
            }
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            this.Close();
            PaymentRecord record = new PaymentRecord();
            record.Show();
        }

        private void btnSelection_Click_1(object sender, EventArgs e)
        {
            this.Close();
            Reset();
            SuppliersList suppliersList = new SuppliersList();
            suppliersList.Show();

        }

        private void GroupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void cmbPaymentMode_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                
                
                    cn.Open();
                    string cb = "UPDATE Payment SET TransactionID=@d2, Date=@d3, PaymentMode=@d4, SupplierID=@d5, Amount=@d6, Remarks=@d7 WHERE T_ID=@d1";
                    using (SqlCommand cmd = new SqlCommand(cb, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", int.Parse(txtT_ID.Text));
                        cmd.Parameters.AddWithValue("@d2", txtTransactionNo.Text);
                        cmd.Parameters.AddWithValue("@d3", dtpTranactionDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d4", cmbPaymentMode.Text);
                        cmd.Parameters.AddWithValue("@d5", int.Parse(txtSup_ID.Text));
                        cmd.Parameters.AddWithValue("@d6", decimal.Parse(txtTransactionAmount.Text));
                        cmd.Parameters.AddWithValue("@d7", txtRemarks.Text);
                        cmd.ExecuteNonQuery();
                    }
                
                MessageBox.Show("تم التعديل بنجاح", "سندات الدفع", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
                btnSave.Enabled = true;
                GetSupplierBalance();
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("هل تريد الحذف بالتأكيد؟", "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DeleteRecord();
            }
        }

        public supplier_payment()
        {
            InitializeComponent();
            cmbPaymentMode.SelectedIndex = 0;
            Auto();
        }
    }
}

