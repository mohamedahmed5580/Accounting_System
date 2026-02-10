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
using Excel = Microsoft.Office.Interop.Excel;

namespace Accounting_System
{
    public partial class Supplier : Form
    {
        public static Supplier instance;

        public Supplier()
        {
            InitializeComponent();
            instance = this;


        }
        private void Panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        public void Reset()
        {
            txtSupplierName.Text = "";
            txtAddress.Text = "";
            txtRemarks.Text = "";
            txtSupplierName.Text = "";
            txtSupplierID.Text = "";
            txtContactNo.Text = "";
            txtEmailID.Text = "";
            cmbState.Text = "";
            txtZipCode.Text = "";
            txtCity.Text = "";
            txtSupplierName.Focus();
            txtTIN.Text = "";
            txtPAN.Text = "";
            txtCSTNo.Text = "";
            txtSTNo.Text = "";
            txtAccountName.Text = "";
            txtAccountNo.Text = "";
            txtBank.Text = "";
            txtBranch.Text = "";
            txtIFSCcode.Text = "";
            cmbOpeningBalanceType.SelectedIndex = 0;
            txtOpeningBalance.Text = "";
            cmbOpeningBalanceType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbOpeningBalanceType.Enabled = true;
            txtOpeningBalance.ReadOnly = false;
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            auto();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private string GenerateID()
        {
            string query = "SELECT TOP 1 ID FROM Supplier ORDER BY ID DESC";
            string value = "0000";

            try
            {
                using (var con = new SqlConnection(DataAccessLayer.Con()))
                using (var cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        value = result.ToString();
                    }

                    int numericValue = int.Parse(value) + 1;
                    value = numericValue.ToString("D4");
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return value;
        }
        public void auto()
        {
            try
            {
                txtID.Text = GenerateID();
                txtSupplierID.Text = "S-" + GenerateID();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
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

                    string cl = "SELECT Supplier.ID FROM Supplier INNER JOIN Stock ON Supplier.ID = Stock.SupplierID WHERE Supplier.ID=@d1";
                    using (SqlCommand cmd = new SqlCommand(cl, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                MessageBox.Show("لا يمكن حذف المورد لأنه مستخدم بالفعل في فواتير الشراء", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }

                    con.Close();
                    con.Open();

                    string cl1 = "SELECT Supplier.ID FROM Supplier INNER JOIN Payment ON Supplier.ID = Payment.SupplierID WHERE Supplier.ID=@d1 AND Amount > 0";
                    using (SqlCommand cmd = new SqlCommand(cl1, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                MessageBox.Show("لا يمكن حذف المورد لأنه مستخدم بالفعل في دفعات الموردين", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }

                    con.Close();
                    con.Open();

                    string cq = "DELETE FROM Supplier WHERE ID = @d1";
                    using (SqlCommand cmd = new SqlCommand(cq, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                        RowsAffected = cmd.ExecuteNonQuery();
                    }

                    if (RowsAffected > 0)
                    {
                        LedgerDelete(txtSupplierID.Text, "الرصيد الافتتاحي");
                        SupplierLedgerDelete(txtSupplierID.Text);
                        LogFunc(lblUser.Text, "deleted the supplier record having supplier id '" + txtSupplierID.Text + "'");
                        MessageBox.Show("تم الحذف بنجاح", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Reset();
                        fillState();
                    }
                    else
                    {
                        MessageBox.Show("لا يوجد سجلات", "عذراً", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Reset();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }


        }


        public void fillState()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
                {
                    cn.Open();
                    SqlDataAdapter adp = new SqlDataAdapter();
                    adp.SelectCommand = new SqlCommand("SELECT DISTINCT RTRIM(State) FROM Supplier ORDER BY 1", cn);
                    DataSet ds = new DataSet("ds");
                    adp.Fill(ds);

                    // Specify System.Data.DataTable explicitly to resolve ambiguity
                    System.Data.DataTable dtable = ds.Tables[0];
                    cmbState.Items.Clear();

                    foreach (System.Data.DataRow drow in dtable.Rows)
                    {
                        cmbState.Items.Add(drow[0].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        private void cmbState_Format(object sender, System.Windows.Forms.ListControlConvertEventArgs e)
        {
            if (object.ReferenceEquals(e.DesiredType, typeof(string)))
            {
                e.Value = e.Value.ToString();
            }
        }


        private void btnNew_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSupplierName.Text))
            {
                MessageBox.Show("الرجاء كتابة اسم المورد", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSupplierName.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("الرجاء كتابة العنوان", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAddress.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtContactNo.Text))
            {
                MessageBox.Show("الرجاء كتابة رقم الاتصال.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContactNo.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtOpeningBalance.Text))
            {
                txtOpeningBalance.Text = "0";
            }

            btnSave.Enabled = false;
            try
            {
                using (var connection = new SqlConnection(DataAccessLayer.Con()))
                {
                    connection.Open();

                    // Check if contact number already exists
                    string checkQuery = "SELECT COUNT(*) FROM Supplier WHERE ContactNo = @ContactNo";
                    using (var checkCmd = new SqlCommand(checkQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@ContactNo", txtContactNo.Text);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("لم يتم إدخال جهة اتصال , أنه مسجل مسبقا", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Insert new supplier record
                    string insertQuery = "INSERT INTO Supplier (ID, SupplierID, [Name], Address, City, ContactNo, EmailID, Remarks, State, ZipCode, TIN, STNo, CST, PAN, AccountName, AccountNumber, Bank, Branch, IFSCCode, OpeningBalance, OpeningBalanceType) " +
                                         "VALUES (@ID, @SupplierID, @Name, @Address, @City, @ContactNo, @EmailID, @Remarks, @State, @ZipCode, @TIN, @STNo, @CST, @PAN, @AccountName, @AccountNumber, @Bank, @Branch, @IFSCCode, @OpeningBalance, @OpeningBalanceType)";
                    using (var insertCmd = new SqlCommand(insertQuery, connection))
                    {
                        insertCmd.Parameters.AddWithValue("@ID", Convert.ToInt32(txtID.Text));
                        insertCmd.Parameters.AddWithValue("@SupplierID", txtSupplierID.Text);
                        insertCmd.Parameters.AddWithValue("@Name", txtSupplierName.Text);
                        insertCmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                        insertCmd.Parameters.AddWithValue("@City", txtCity.Text);
                        insertCmd.Parameters.AddWithValue("@ContactNo", txtContactNo.Text);
                        insertCmd.Parameters.AddWithValue("@EmailID", txtEmailID.Text);
                        insertCmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text);
                        insertCmd.Parameters.AddWithValue("@State", cmbState.Text);
                        insertCmd.Parameters.AddWithValue("@ZipCode", txtZipCode.Text);
                        insertCmd.Parameters.AddWithValue("@TIN", txtTIN.Text);
                        insertCmd.Parameters.AddWithValue("@STNo", txtSTNo.Text);
                        insertCmd.Parameters.AddWithValue("@CST", txtCSTNo.Text);
                        insertCmd.Parameters.AddWithValue("@PAN", txtPAN.Text);
                        insertCmd.Parameters.AddWithValue("@AccountName", txtAccountName.Text);
                        insertCmd.Parameters.AddWithValue("@AccountNumber", txtAccountNo.Text);
                        insertCmd.Parameters.AddWithValue("@Bank", txtBank.Text);
                        insertCmd.Parameters.AddWithValue("@Branch", txtBranch.Text);
                        insertCmd.Parameters.AddWithValue("@IFSCCode", txtIFSCcode.Text);
                        insertCmd.Parameters.AddWithValue("@OpeningBalance", Convert.ToDecimal(txtOpeningBalance.Text));
                        insertCmd.Parameters.AddWithValue("@OpeningBalanceType", cmbOpeningBalanceType.Text);

                        insertCmd.ExecuteNonQuery();
                    }
                    
                    if (Convert.ToDecimal(txtOpeningBalance.Text) > 0)
                    {
                        if (cmbOpeningBalanceType.Text == "Credit")
                        {
                            LedgerSave(DateTime.Now, txtSupplierName.Text, txtSupplierID.Text, "الرصيد الافتتاحي", 0, Convert.ToDecimal(txtOpeningBalance.Text), txtSupplierID.Text, "");
                        }
                        if (cmbOpeningBalanceType.Text == "Debit")
                        {
                            LedgerSave(DateTime.Now, txtSupplierName.Text, txtSupplierID.Text, "الرصيد الافتتاحي", Convert.ToDecimal(txtOpeningBalance.Text), 0, txtSupplierID.Text, "");
                        }
                    }
                }

                LogFunc(lblUser.Text, "added the new supplier having supplier id '" + txtSupplierID.Text + "'");
                MessageBox.Show("تم الحفظ بنجاح", "سجلات الموردين", MessageBoxButtons.OK, MessageBoxIcon.Information);
                fillState();
                Reset();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            finally
            {
                btnSave.Enabled = true;
            }
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSupplierName.Text.Trim()))
            {
                MessageBox.Show("الرجاء كتابة اسم المورد", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSupplierName.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtAddress.Text.Trim()))
            {
                MessageBox.Show("الرجاء كتابة العنوان", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAddress.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtContactNo.Text.Trim()))
            {
                MessageBox.Show("الرجاء كتابة رقم الاتصال.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContactNo.Focus();
                return;
            }

            btnUpdate.Enabled = false;
            try
            {
                using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
                {
                    cn.Open();

                    // Update LedgerBook
                    string cb1 = "UPDATE LedgerBook SET [Name] = @d3 WHERE PartyID = @d1 AND Name = @d2";
                    using (SqlCommand cmd = new SqlCommand(cb1, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtSupplierID.Text);
                        cmd.Parameters.AddWithValue("@d2", txtSupName.Text);
                        cmd.Parameters.AddWithValue("@d3", txtSupplierName.Text);
                        cmd.ExecuteNonQuery();
                    }

                    // Update SupplierLedgerBook
                    string cb3 = "UPDATE SupplierLedgerBook SET [Name] = @d3 WHERE PartyID = @d1 AND Name = @d2";
                    using (SqlCommand cmd = new SqlCommand(cb3, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtSupplierID.Text);
                        cmd.Parameters.AddWithValue("@d2", txtSupName.Text);
                        cmd.Parameters.AddWithValue("@d3", txtSupplierName.Text);
                        cmd.ExecuteNonQuery();
                    }

                    // Update Supplier
                    string cb = "UPDATE supplier SET SupplierID = @d2, [Name] = @d3, Address = @d5, City = @d6, ContactNo = @d7, EmailID = @d8, Remarks = @d9, State = @d10, ZipCode = @d11, TIN = @d12, STNo = @d13, CST = @d14, PAN = @d15, AccountName = @d16, AccountNumber = @d17, Bank = @d18, Branch = @d19, IFSCCode = @d20 WHERE ID = @d1";
                    using (SqlCommand cmd = new SqlCommand(cb, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                        cmd.Parameters.AddWithValue("@d2", txtSupplierID.Text);
                        cmd.Parameters.AddWithValue("@d3", txtSupplierName.Text);
                        cmd.Parameters.AddWithValue("@d5", txtAddress.Text);
                        cmd.Parameters.AddWithValue("@d6", txtCity.Text);
                        cmd.Parameters.AddWithValue("@d7", txtContactNo.Text);
                        cmd.Parameters.AddWithValue("@d8", txtEmailID.Text);
                        cmd.Parameters.AddWithValue("@d9", txtRemarks.Text);
                        cmd.Parameters.AddWithValue("@d10", cmbState.Text);
                        cmd.Parameters.AddWithValue("@d11", txtZipCode.Text);
                        cmd.Parameters.AddWithValue("@d12", txtTIN.Text);
                        cmd.Parameters.AddWithValue("@d13", txtSTNo.Text);
                        cmd.Parameters.AddWithValue("@d14", txtCSTNo.Text);
                        cmd.Parameters.AddWithValue("@d15", txtPAN.Text);
                        cmd.Parameters.AddWithValue("@d16", txtAccountName.Text);
                        cmd.Parameters.AddWithValue("@d17", txtAccountNo.Text);
                        cmd.Parameters.AddWithValue("@d18", txtBank.Text);
                        cmd.Parameters.AddWithValue("@d19", txtBranch.Text);
                        cmd.Parameters.AddWithValue("@d20", txtIFSCcode.Text);
                        cmd.ExecuteNonQuery();
                    }

                    // Log the update operation
                    LogFunc(lblUser.Text, "updated the supplier having supplier id '" + txtSupplierID.Text + "'");

                    MessageBox.Show("تم التعديل بنجاح", "سجلات الموردين", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    fillState();
                    Reset();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnUpdate.Enabled = true;
            }
            finally
            {
                // btnUpdate will be false usually due to Reset, but let's be safe
                if (btnSave.Enabled) btnUpdate.Enabled = false;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("هل أنت متأكد أنك تريد حذف سجل هذا المورد?", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
            {
                btnDelete.Enabled = false;
                try
                {
                    DeleteRecord();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnDelete.Enabled = true;
                }
            }
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            btnGetData.Enabled = false;
            try
            {
                var frm = new SupplierScreen();
                frm.lblSet.Text = "Supplier Entry";
                frm.ShowDialog();
            }
            finally
            {
                btnGetData.Enabled = true;
            }
        }


        private void txtOpeningBalance_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            var keyChar = e.KeyChar;

            if (char.IsControl(keyChar))
            {
            }
            // Allow all control characters.
            else if (char.IsDigit(keyChar) || keyChar == '.')
            {
                var text = this.txtOpeningBalance.Text;
                var selectionStart = this.txtOpeningBalance.SelectionStart;
                var selectionLength = this.txtOpeningBalance.SelectionLength;

                text = text.Substring(0, selectionStart) + keyChar + text.Substring(selectionStart + selectionLength);

                int parsedInt;
                double parsedDouble;

                if (int.TryParse(text, out parsedInt) && text.Length > 16)
                {
                    // Reject an integer that is longer than 16 digits.
                    e.Handled = true;
                }
                else if (double.TryParse(text, out parsedDouble) && text.IndexOf('.') < text.Length - 3)
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
                // Reject all other characters.
                e.Handled = true;
            }
        }
        public void SMS(string st1)
        {
            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
            {
                cn.Open();
                string cb = "insert into SMS(Message,Date) VALUES (@d1,@d2)";
                using (SqlCommand cmd = new SqlCommand(cb, cn))
                {
                    cmd.Parameters.AddWithValue("@d1", st1);
                    cmd.Parameters.AddWithValue("@d2", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void LogFunc(string st1, string st2)
        {
            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
            {
                cn.Open();
                string cb = "insert into Logs(UserID,Date,Operation) VALUES (@d1,@d2,@d3)";
                using (SqlCommand cmd = new SqlCommand(cb, cn))
                {
                    cmd.Parameters.AddWithValue("@d1", st1);
                    cmd.Parameters.AddWithValue("@d2", DateTime.Now);
                    cmd.Parameters.AddWithValue("@d3", st2);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void LedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g, string h)
        {
            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
            {
                cn.Open();
                string cb = "insert into LedgerBook(Date, Name, LedgerNo, Label,Debit,Credit,PartyID,Manual_Inv) Values (@d1,@d2,@d3,@d4,@d5,@d6,@d7,@d8)";
                using (SqlCommand cmd = new SqlCommand(cb, cn))
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
        }
        public void LedgerDelete(string a, string b)
        {
            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
            {
                cn.Open();
                string cq = "delete from LedgerBook where LedgerNo=@d1 and Label=@d2";
                using (SqlCommand cmd = new SqlCommand(cq, cn))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.Parameters.AddWithValue("@d2", b);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void LedgerUpdate(DateTime a, string b, decimal e, decimal f, string g, string h, string i)
        {
            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
            {
                cn.Open();
                string cb = "Update LedgerBook set Date=@d1, Name=@d2,Debit=@d3,Credit=@d4,PartyID=@d5 where LedgerNo=@d6 and Label=@d7";
                using (SqlCommand cmd = new SqlCommand(cb, cn))
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
        }
        public void SupplierLedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g)
        {
            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
            {
                cn.Open();
                string cb = "insert into SupplierLedgerBook(Date, Name, LedgerNo, Label,Debit,Credit,PartyID) Values (@d1,@d2,@d3,@d4,@d5,@d6,@d7)";
                using (SqlCommand cmd = new SqlCommand(cb, cn))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.Parameters.AddWithValue("@d2", b);
                    cmd.Parameters.AddWithValue("@d3", c);
                    cmd.Parameters.AddWithValue("@d4", d);
                    cmd.Parameters.AddWithValue("@d5", e);
                    cmd.Parameters.AddWithValue("@d6", f);
                    cmd.Parameters.AddWithValue("@d7", g);
                    cmd.ExecuteNonQuery();
                }
            }
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
            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
            {
                cn.Open();
                string cq = "delete from SupplierLedgerBook where LedgerNo=@d1";
                SqlCommand cmd = new SqlCommand(cq);
                cmd.Parameters.AddWithValue("@d1", a);
                cmd.Connection = cn;
                cmd.ExecuteReader();
            }
        }
        public void SupplierLedgerUpdate(DateTime a, string b, decimal e, decimal f, string g, string h)
        {
            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
            {
                cn.Open();
                string cb = "Update SupplierLedgerBook set Date=@d1, Name=@d2,Debit=@d3,Credit=@d4 where LedgerNo=@d5 and Label=@d6";
                SqlCommand cmd = new SqlCommand(cb);
                cmd.Parameters.AddWithValue("@d1", a);
                cmd.Parameters.AddWithValue("@d2", b);
                cmd.Parameters.AddWithValue("@d3", e);
                cmd.Parameters.AddWithValue("@d4", f);
                cmd.Parameters.AddWithValue("@d5", g);
                cmd.Parameters.AddWithValue("@d6", h);
                cmd.Connection = cn;
                cmd.ExecuteReader();
            }
        }

        private void Supplier_Load(object sender, EventArgs e)
        {
            
            fillState();
        }

        private void txtSupplierID_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
