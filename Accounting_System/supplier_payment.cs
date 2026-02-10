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
using System.Text.RegularExpressions;


namespace Accounting_System
{
    public partial class supplier_payment : Form
    {

        SqlConnection cn = new SqlConnection(DataAccessLayer.Con());

        public static supplier_payment instance;
        private string str;
        private string OBType;
        private decimal num1, num2, num3, num4;
        private int i = 0;
        public supplier_payment()
        {
            InitializeComponent();
            LoadCurrenciesToComboBox();
            comboBox1.SelectedIndex = 0;
            cmbPaymentMode.SelectedIndex = 0;
            Auto();
            fillSupplers();
            fillSupplersPhone();
        }
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

        public void SupplierLedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g, string q, decimal v)
        {
            
            
                cn.Open();
                string cb = "INSERT INTO SupplierLedgerBook(Date, Name, LedgerNo, Label, Debit, Credit, PartyID ,Currencies,CPrice) VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9)";
                using (var cmd = new SqlCommand(cb, cn))
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

                    
                    cn.Open();
                    string sql = "SELECT ISNULL(SUM(Credit), 0) - ISNULL(SUM(Debit), 0)   FROM SupplierLedgerBook WHERE PartyID=@d1 GROUP BY PartyID";
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
            txtBank.Text = "";
            txtCheck.Text = "";
            label14.Text = "";
            txtTransactionAmount.Text = "";
            cmbPaymentMode.SelectedIndex = 0;
            dtpTranactionDate.Value = DateTime.Today;
            dtpCheck.Value = DateTime.Today;
            LoadCurrenciesToComboBox();
            comboBox1.SelectedIndex = 0;
            lblBalance.Text = "0.000";
            btnSave.Enabled = true;
            btnDelete.Enabled = false;
            btnUpdate1.Enabled = false;
            btnSelection.Enabled = true;
            Auto();
            comboBox1.SelectedIndex = comboBox1.Items.Count > 0 ? 0 : -1;
            comboBox2.SelectedIndex = comboBox2.Items.Count > 0 ? 0 : -1;
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
            instance = this;
            Reset();
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
            if (comboBox1.Text.Trim().Length == 0)
            {
                MessageBox.Show("الرجاء كتابة عملة المبلغ المدفوع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox1.Focus();
                return;
            }

            if (comboBox2.Text.Trim().Length == 0)
            {
                MessageBox.Show("الرجاء كتابة عملة السند", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox2.Focus();
                return;
            }

            btnSave.Enabled = false;
            try
            {
                using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
                {
                    cn.Open();
                    string cb = "insert into Payment(T_ID, TransactionID, Date, PaymentMode, SupplierID, Amount, Remarks,Check_ID,Check_Date,Bank) VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9, @d10)";
                    using (SqlCommand cmd = new SqlCommand(cb, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtT_ID.Text));
                        cmd.Parameters.AddWithValue("@d2", txtTransactionNo.Text);
                        cmd.Parameters.AddWithValue("@d3", dtpTranactionDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d4", cmbPaymentMode.Text);
                        cmd.Parameters.AddWithValue("@d5", Convert.ToInt32(txtSup_ID.Text));
                        cmd.Parameters.AddWithValue("@d6", Convert.ToDecimal(txtTransactionAmount.Text));
                        cmd.Parameters.AddWithValue("@d7", txtRemarks.Text);
                        cmd.Parameters.AddWithValue("@d8", txtCheck.Text);
                        cmd.Parameters.AddWithValue("@d9", dtpCheck.Value.Date);
                        cmd.Parameters.AddWithValue("@d10", txtBank.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                if (cmbPaymentMode.SelectedIndex == 0)
                {
                    if (Convert.ToDecimal(txtTransactionAmount.Text) > 0)
                    {
                        LedgerSave(dtpTranactionDate.Value.Date, "نقدا" + "/" + txtSupplierName.Text, txtTransactionNo.Text, "سند دفع", Convert.ToDecimal(txtTransactionAmount.Text)*Convert.ToDecimal(Cprice.Text), 0, txtSupplierID.Text, "");
                    }
                    else
                    {
                        LedgerSave(dtpTranactionDate.Value.Date, "نقدا" + "/" + txtSupplierName.Text, txtTransactionNo.Text, "سند دفع", 0, Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text)) * Convert.ToDecimal(Cprice.Text), txtSupplierID.Text, "");
                    }
                }
                if (cmbPaymentMode.SelectedIndex == 1)
                {
                    if (Convert.ToDecimal(txtTransactionAmount.Text) > 0)
                    {
                        LedgerSave(dtpTranactionDate.Value.Date, "شيك رقم" + txtCheck.Text.Trim() + "/" + txtSupplierName.Text, txtTransactionNo.Text, "سند دفع", Convert.ToDecimal(txtTransactionAmount.Text) * Convert.ToDecimal(Cprice.Text), 0, txtSupplierID.Text, "");
                    }
                    else
                    {
                        LedgerSave(dtpTranactionDate.Value.Date, "شيك رقم" + txtCheck.Text.Trim() + "/" + txtSupplierName.Text, txtTransactionNo.Text, "سند دفع", 0, Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text)) * Convert.ToDecimal(Cprice.Text), txtSupplierID.Text, "");
                    }
                }
                if (cmbPaymentMode.SelectedIndex == 2)
                {
                    if (Convert.ToDecimal(txtTransactionAmount.Text) > 0)
                    {
                        LedgerSave(dtpTranactionDate.Value.Date, "حوالة بنكية رقم" + txtCheck.Text.Trim() + "/" + txtSupplierName.Text, txtTransactionNo.Text, "سند دفع", Convert.ToDecimal(txtTransactionAmount.Text) * Convert.ToDecimal(Cprice.Text), 0, txtSupplierID.Text, "");
                    }
                    else
                    {
                        LedgerSave(dtpTranactionDate.Value.Date, "حوالة بنكية رقم" + txtCheck.Text.Trim() + "/" + txtSupplierName.Text, txtTransactionNo.Text, "سند دفع", 0, Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text)) * Convert.ToDecimal(Cprice.Text), txtSupplierID.Text, "");
                    }
                }
                if (cmbPaymentMode.SelectedIndex == 0)
                {
                    if (Convert.ToDecimal(txtTransactionAmount.Text) > 0)
                    {
                        SupplierLedgerSave(dtpTranactionDate.Value.Date, "نقدا" + "/" + txtSupplierName.Text, txtTransactionNo.Text, "سند دفع", Convert.ToDecimal(txtTransactionAmount.Text) * Convert.ToDecimal(Cprice.Text), 0, txtSupplierID.Text, comboBox1.Text, Convert.ToDecimal(Cprice.Text));
                    }
                    else
                    {
                        SupplierLedgerSave(dtpTranactionDate.Value.Date, "نقدا" + "/" + txtSupplierName.Text, txtTransactionNo.Text, "سند دفع", 0, Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text)) * Convert.ToDecimal(Cprice.Text), txtSupplierID.Text, comboBox1.Text, Convert.ToDecimal(Cpricee.Text));
                    }
                }
                if (cmbPaymentMode.SelectedIndex == 1)
                {
                    if (Convert.ToDecimal(txtTransactionAmount.Text) > 0)
                    {
                        SupplierLedgerSave(dtpTranactionDate.Value.Date, "شيك رقم" + txtCheck.Text.Trim() + "/" + txtSupplierName.Text, txtTransactionNo.Text, "سند دفع", Convert.ToDecimal(txtTransactionAmount.Text) * Convert.ToDecimal(Cprice.Text), 0, txtSupplierID.Text, comboBox1.Text, Convert.ToDecimal(Cpricee.Text));
                    }
                    else
                    {
                        SupplierLedgerSave(dtpTranactionDate.Value.Date, "شيك رقم" + txtCheck.Text.Trim() + "/" + txtSupplierName.Text, txtTransactionNo.Text, "سند دفع", 0, Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text)) * Convert.ToDecimal(Cprice.Text), txtSupplierID.Text, comboBox1.Text, Convert.ToDecimal(Cpricee.Text));
                    }
                }
                if (cmbPaymentMode.SelectedIndex == 2)
                {
                    if (Convert.ToDecimal(txtTransactionAmount.Text) > 0)
                    {
                        SupplierLedgerSave(dtpTranactionDate.Value.Date, "حوالة بنكية رقم" + txtCheck.Text.Trim() + "/" + txtSupplierName.Text, txtTransactionNo.Text, "سند دفع", Convert.ToDecimal(txtTransactionAmount.Text) * Convert.ToDecimal(Cprice.Text), 0, txtSupplierID.Text, comboBox1.Text, Convert.ToDecimal(Cpricee.Text));
                    }
                    else
                    {
                        SupplierLedgerSave(dtpTranactionDate.Value.Date, "حوالة بنكية رقم" + txtCheck.Text.Trim() + "/" + txtSupplierName.Text, txtTransactionNo.Text, "سند دفع", 0, Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text)) * Convert.ToDecimal(Cprice.Text), txtSupplierID.Text, comboBox1.Text, Convert.ToDecimal(Cpricee.Text));
                    }
                }



                LogFunc(lblUser.Text, "added the new payment having transaction No. '" + txtTransactionNo.Text + "'");
                MessageBox.Show("تم الحفظ بنجاح", "سندات دفع الموردين", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSave.Enabled = true;
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
            btnGetData.Enabled = false;
            try
            {
                PaymentRecord record = new PaymentRecord();
                record.Show();
            }
            finally
            {
                btnGetData.Enabled = true;
            }
        }

        private void btnSelection_Click_1(object sender, EventArgs e)
        {
           
            SuppliersList suppliersList = new SuppliersList();
            suppliersList.Show();

        }

        private void GroupBox1_Enter(object sender, EventArgs e)
        {

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

        private void lblBalance_Click(object sender, EventArgs e)
        {

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("هل تريد الحذف بالتأكيد؟", "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DeleteRecord();
            }
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
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
            catch (Exception ex)
            {
                MessageBox.Show($"A database error occurred: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void dtpCheck_ValueChanged(object sender, EventArgs e)
        {

        }

        private void gbPartyInfo_Enter(object sender, EventArgs e)
        {

        }

        private void txtSupplierName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string a = "";
                string b = "";
                string c = "";
                txtSup_ID.Text = "";

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT  RTRIM(ID),RTRIM(SupplierID),RTRIM([Name]), RTRIM(Address),RTRIM(City),RTRIM(State),RTRIM(ZipCode), RTRIM(ContactNo) FROM Supplier WHERE Name=@d1";
                        cmd.Parameters.AddWithValue("@d1", txtSupplierName.Text);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                txtSup_ID.Text = rdr.GetString(0);
                                txtSupplierID.Text = rdr.GetString(1) ?? string.Empty;
                                txtSupplierName.Text = rdr.GetString(2) ?? string.Empty;
                                txtAddress.Text = rdr.GetString(3) ?? string.Empty;
                                txtCity.Text = rdr.GetString(4) ?? string.Empty;
                                txtContactNo.Text = rdr.GetString(7) ?? string.Empty;
                                GetSupplierBalance();

                                a = rdr.GetString(1);
                                b = rdr.GetString(2);
                                c = rdr.GetString(3);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            try
            {
                label14.Text = (Convert.ToDecimal(ExtractDecimal(lblBalance.Text)) / Convert.ToDecimal(ExtractDecimal(Cpricee.Text))).ToString("F2");

            }
            catch { }
        }

        private void txtContactNo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtContactNo_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            try
            {
                string a = "";
                string b = "";
                string c = "";
                txtSup_ID.Text = "";

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT RTRIM(ID),RTRIM(SupplierID),RTRIM([Name]), RTRIM(Address),RTRIM(City),RTRIM(State),RTRIM(ZipCode), RTRIM(ContactNo) FROM Supplier WHERE ContactNo=@d1";
                        cmd.Parameters.AddWithValue("@d1", txtContactNo.Text);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                txtSup_ID.Text = rdr.GetString(0);
                                txtSupplierID.Text = rdr.GetString(1) ?? string.Empty;
                                txtSupplierName.Text = rdr.GetString(2) ?? string.Empty;
                                txtAddress.Text = rdr.GetString(3) ?? string.Empty;
                                txtCity.Text = rdr.GetString(4) ?? string.Empty;
                                txtContactNo.Text = rdr.GetString(7) ?? string.Empty;
                                GetSupplierBalance();
                                a = rdr.GetString(1);
                                b = rdr.GetString(2);
                                c = rdr.GetString(3);
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
        private void fillSupplersPhone()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlDataAdapter adp = new SqlDataAdapter())
                    {
                        adp.SelectCommand = new SqlCommand("SELECT RTRIM(ContactNo) FROM Supplier ", con);
                        DataSet ds = new DataSet("ds");
                        adp.Fill(ds);
                        System.Data.DataTable dtable = ds.Tables[0];
                        txtContactNo.Items.Clear();
                        foreach (DataRow drow in dtable.Rows)
                        {
                            txtContactNo.Items.Add(drow[0].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void fillSupplers()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlDataAdapter adp = new SqlDataAdapter())
                    {
                        adp.SelectCommand = new SqlCommand("SELECT RTRIM(Name) FROM Supplier ", con);
                        DataSet ds = new DataSet("ds");
                        adp.Fill(ds);
                        System.Data.DataTable dtable = ds.Tables[0];
                        txtSupplierName.Items.Clear();
                        foreach (DataRow drow in dtable.Rows)
                        {
                            txtSupplierName.Items.Add(drow[0].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void btnUpdate_Click_1(object sender, EventArgs e)
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

            btnUpdate.Enabled = false;
            try
            {
                using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
                {
                    cn.Open();
                    string cb = "UPDATE Payment SET Date = @d3, PaymentMode = @d4, SupplierID = @d5, Amount = @d6, Remarks = @d7, Check_ID = @d8, Check_Date = @d9, Bank = @d10 WHERE TransactionID = @d2";

                    using (SqlCommand cmd = new SqlCommand(cb, cn))
                    {
                        cmd.Parameters.AddWithValue("@d2", txtTransactionNo.Text); // Ensure this field uniquely identifies the record
                        cmd.Parameters.AddWithValue("@d3", dtpTranactionDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d4", cmbPaymentMode.Text);
                        cmd.Parameters.AddWithValue("@d5", Convert.ToInt32(txtSup_ID.Text));
                        cmd.Parameters.AddWithValue("@d6", Convert.ToDecimal(txtTransactionAmount.Text));
                        cmd.Parameters.AddWithValue("@d7", txtRemarks.Text);
                        cmd.Parameters.AddWithValue("@d8", txtCheck.Text);
                        cmd.Parameters.AddWithValue("@d9", dtpCheck.Value.Date);
                        cmd.Parameters.AddWithValue("@d10", txtBank.Text);
                        cmd.ExecuteNonQuery();
                    }

                    // Update SupplierLedgerBook if necessary
                    string ledgerUpdateQuery = "UPDATE SupplierLedgerBook SET Date = @d1, Name = @d2, Label = @d3, Debit = @d4, Credit = @d5, Currencies = @d6, CPrice = @d7 WHERE LedgerNo = @d8";

                    using (SqlCommand cmd = new SqlCommand(ledgerUpdateQuery, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", dtpTranactionDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d2", cmbPaymentMode.Text + "/" + txtSupplierName.Text);
                        cmd.Parameters.AddWithValue("@d3", "سند دفع");
                        if (Convert.ToDecimal(txtTransactionAmount.Text) < 0)
                        {
                            cmd.Parameters.AddWithValue("@d4", 0);
                            cmd.Parameters.AddWithValue("@d5", Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text) * Convert.ToDecimal(Cprice.Text)));
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@d4", Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text) * Convert.ToDecimal(Cprice.Text)));
                            cmd.Parameters.AddWithValue("@d5", 0);
                        }

                        cmd.Parameters.AddWithValue("@d6", comboBox1.Text);
                        cmd.Parameters.AddWithValue("@d7", Convert.ToDecimal(Cprice.Text));
                        cmd.Parameters.AddWithValue("@d8", txtTransactionNo.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Determine Payment Mode & Update LedgerBook
                string ledgerName = "";
                decimal debit = 0, credit = 0;

                if (cmbPaymentMode.SelectedIndex == 0) // نقدا
                {
                    ledgerName = "نقدا" + "/" + txtSupplierName.Text;
                }
                else if (cmbPaymentMode.SelectedIndex == 1) // شيك
                {
                    ledgerName = "شيك رقم " + txtCheck.Text.Trim() + "/" + txtSupplierName.Text;
                }
                else if (cmbPaymentMode.SelectedIndex == 2) // حوالة بنكية
                {
                    ledgerName = "حوالة بنكية رقم " + txtCheck.Text.Trim() + "/" + txtSupplierName.Text;
                }

                if (Convert.ToDecimal(txtTransactionAmount.Text) > 0)
                {
                    debit = Convert.ToDecimal(txtTransactionAmount.Text) * Convert.ToDecimal(Cprice.Text);
                    credit = 0;
                }
                else
                {
                    debit = 0;
                    credit = Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text)) * Convert.ToDecimal(Cprice.Text);
                }

                LedgerUpdate(dtpTranactionDate.Value.Date,
                             ledgerName,
                             debit,
                             credit,
                             txtSupplierID.Text,
                             txtTransactionNo.Text,  // LedgerNo
                             "سند دفع");  // Label
                LogFunc(lblUser.Text, "Updated the payment with transaction No. '" + txtTransactionNo.Text + "'");
                MessageBox.Show("تم التحديث بنجاح", "سندات دفع الموردين", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnUpdate.Enabled = true;
            }
        }

        private void btnUpdate1_Click(object sender, EventArgs e)
        {

        }

        private decimal ExtractDecimal(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0m;

            // Remove any non-numeric characters except decimal point and commas
            string numericPart = Regex.Replace(input, @"[^0-9.,-]", "");

            // Ensure proper decimal format conversion
            return decimal.TryParse(numericPart, out decimal result) ? result : 0m;
        }

    private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {


                // Step 2: Get the selected currency name from the ComboBox
                string selectedCurrency = comboBox2.SelectedItem.ToString().Trim();

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
                                    Cpricee.Text = price.ToString();
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



                label14.Text = (Convert.ToDecimal(ExtractDecimal(lblBalance.Text)) / Convert.ToDecimal(ExtractDecimal(Cpricee.Text))).ToString("F2");


            }
            catch (Exception ex)
            {
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
                                comboBox2.Items.Clear();

                                // Step 5: Loop through the result set and add each currency to the ComboBox
                                while (reader.Read())
                                {
                                    comboBox1.Items.Add(reader["Name"].ToString());
                                    comboBox2.Items.Add(reader["Name"].ToString());
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
    }
}

