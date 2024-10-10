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
        SqlConnection cn = new SqlConnection(DataAccessLayer.Con());

        public Supplier()
        {
            InitializeComponent();
            
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
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }


        public void fillState()
        {
            try
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
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            try
            {
                using (var connection = new SqlConnection(DataAccessLayer.Con()))
                {
                    connection.Open();

                    // Check if contact number already exists
                    string checkQuery = "SELECT RTRIM(ContactNo) FROM Supplier WHERE ContactNo = @ContactNo";
                    using (var checkCmd = new SqlCommand(checkQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@ContactNo", txtContactNo.Text);
                        using (var reader = checkCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                MessageBox.Show("لم يتم إدخال جهة اتصال , أنه مسجل مسبقا", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
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
                        insertCmd.Parameters.AddWithValue("@OpeningBalance", Convert.ToInt32(txtOpeningBalance.Text));
                        insertCmd.Parameters.AddWithValue("@OpeningBalanceType", cmbOpeningBalanceType.Text);

                        insertCmd.ExecuteNonQuery();
                    }
                }

                // Additional logic for ledger and logs...
                MessageBox.Show("تم الحفظ بنجاح", "سجلات الموردين", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
                fillState();
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    btnUpdate.Enabled = false;
                    fillState();
                    Reset();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("هل أنت متأكد أنك تريد حذف سجل هذا المورد?", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
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
            var frm = new SupplierScreen();
            frm.lblSet.Text = "Supplier Entry";
            frm.ShowDialog();
            this.Close();
            this.FormClosed += (s, args) => this.Close();

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
            
            cn.Open();
            string cb = "insert into SMS(Message,Date) VALUES (@d1,@d2)";
            SqlCommand cmd = new SqlCommand(cb);
            cmd.Connection = cn;
            cmd.Parameters.AddWithValue("@d1", st1);
            cmd.Parameters.AddWithValue("@d2", DateTime.Now);
            cmd.ExecuteReader();
            cn.Close();
        }
        public void LogFunc(string st1, string st2)
        {
            
            cn.Open();
            string cb = "insert into Logs(UserID,Date,Operation) VALUES (@d1,@d2,@d3)";
            SqlCommand cmd = new SqlCommand(cb);
            cmd.Connection = cn;
            cmd.Parameters.AddWithValue("@d1", st1);
            cmd.Parameters.AddWithValue("@d2", DateTime.Now);
            cmd.Parameters.AddWithValue("@d3", st2);
            cmd.ExecuteReader();
            cn.Close();
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
/*            frmStockBalance obj = (frmStockBalance)Application.OpenForms("frmStockBalance");
            obj.Getdata();
            obj.DataGridView1.Refresh();
            obj.DataGridView1.Update();*/
        }
        
        public void LedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g, string h)
        {
            
            cn.Open();
            string cb = "insert into LedgerBook(Date, Name, LedgerNo, Label,Debit,Credit,PartyID,Manual_Inv) Values (@d1,@d2,@d3,@d4,@d5,@d6,@d7,@d8)";
            SqlCommand cmd = new SqlCommand(cb);
            cmd.Parameters.AddWithValue("@d1", a);
            cmd.Parameters.AddWithValue("@d2", b);
            cmd.Parameters.AddWithValue("@d3", c);
            cmd.Parameters.AddWithValue("@d4", d);
            cmd.Parameters.AddWithValue("@d5", e);
            cmd.Parameters.AddWithValue("@d6", f);
            cmd.Parameters.AddWithValue("@d7", g);
            cmd.Parameters.AddWithValue("@d8", h);
            cmd.Connection = cn;
            cmd.ExecuteReader();
            cn.Close();
        }
        public void LedgerDelete(string a, string b)
        {
            
            cn.Open();
            string cq = "delete from LedgerBook where LedgerNo=@d1 and Label=@d2";
            SqlCommand cmd = new SqlCommand(cq);
            cmd.Parameters.AddWithValue("@d1", a);
            cmd.Parameters.AddWithValue("@d2", b);
            cmd.Connection = cn;
            cmd.ExecuteReader();
            cn.Close();
        }
        public void LedgerUpdate(DateTime a, string b, decimal e, decimal f, string g, string h, string i)
        {
            
            cn.Open();
            string cb = "Update LedgerBook set Date=@d1, Name=@d2,Debit=@d3,Credit=@d4,PartyID=@d5 where LedgerNo=@d6 and Label=@d7";
            SqlCommand cmd = new SqlCommand(cb);
            cmd.Parameters.AddWithValue("@d1", a);
            cmd.Parameters.AddWithValue("@d2", b);
            cmd.Parameters.AddWithValue("@d3", e);
            cmd.Parameters.AddWithValue("@d4", f);
            cmd.Parameters.AddWithValue("@d5", g);
            cmd.Parameters.AddWithValue("@d6", h);
            cmd.Parameters.AddWithValue("@d7", i);
            cmd.Connection = cn;
            cmd.ExecuteReader();
            cn.Close();
        }
        public void SupplierLedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g)
        {
            
            cn.Open();
            string cb = "insert into SupplierLedgerBook(Date, Name, LedgerNo, Label,Debit,Credit,PartyID) Values (@d1,@d2,@d3,@d4,@d5,@d6,@d7)";
            SqlCommand cmd = new SqlCommand(cb);
            cmd.Parameters.AddWithValue("@d1", a);
            cmd.Parameters.AddWithValue("@d2", b);
            cmd.Parameters.AddWithValue("@d3", c);
            cmd.Parameters.AddWithValue("@d4", d);
            cmd.Parameters.AddWithValue("@d5", e);
            cmd.Parameters.AddWithValue("@d6", f);
            cmd.Parameters.AddWithValue("@d7", g);
            cmd.Connection = cn;
            cmd.ExecuteReader();
            cn.Close();
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
            
            cn.Open();
            string cq = "delete from SupplierLedgerBook where LedgerNo=@d1";
            SqlCommand cmd = new SqlCommand(cq);
            cmd.Parameters.AddWithValue("@d1", a);
            cmd.Connection = cn;
            cmd.ExecuteReader();
            cn.Close();
        }
        public void SupplierLedgerUpdate(DateTime a, string b, decimal e, decimal f, string g, string h)
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
            cn.Close();
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
