using Pharmacy.DL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Accounting_System
{
    public partial class SalesMan : Form
    {

        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        private string s;
        private string Photoname = "";
        private bool IsImageChanged = false;

        public SalesMan()
        {
            InitializeComponent();
        }
        private void SalesMan_Load(object sender, EventArgs e)
        {
            Reset();

        }
        public void Reset()
        {
            txtSalesmanName.Text = "";
            txtAddress.Text = "";
            txtRemarks.Text = "";
            txtSalesmanName.Text = "";
            txtSalesmanID.Text = "";
            txtContactNo.Text = "";
            txtEmailID.Text = "";
            txtZipCode.Text = "";
            txtCommissionPer.Text = "0.000";
            txtCity.Text = "";
            txtSalesmanName.Focus();
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            Picture.Image = Properties.Resources.noThing;
            auto();
            cmbState.Text = "";
        }

        private string GenerateID()
        {
            string value = "0000";
            try
            {
                using (var con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (var cmd = new SqlCommand("SELECT TOP 1 SM_ID FROM Salesman ORDER BY SM_ID DESC", con))
                    {
                        using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                value = rdr["SM_ID"].ToString();
                            }
                        }
                    }

                    // Increase the ID by 1
                    value = (int.Parse(value) + 1).ToString();
                    // Format the ID with leading zeros
                    value = value.PadLeft(4, '0');
                }
            }
            catch (Exception ex)
            {
                value = "0000";
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return value;
        }

        public void auto()
        {
            try
            {
                txtID.Text = GenerateID();
                txtSalesmanID.Text = "SM-" + GenerateID();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSalesmanName.Text))
            {
                MessageBox.Show("الرجاء كتابة اسم المندوب", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSalesmanName.Focus();
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

            try
            {
                using (var con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (var cmd = new SqlCommand("SELECT RTRIM(ContactNo) FROM Salesman WHERE ContactNo=@d1", con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtContactNo.Text);
                        using (var rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                MessageBox.Show("لا لقد تم إدخال جهة اتصال , أنه مسجل مسبقا", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }

                    using (var cmd = new SqlCommand("INSERT INTO Salesman(SM_ID, Salesman_ID, [Name], CommissionPer, Address, City, ContactNo, EmailID, Remarks, State, ZipCode, Photo) VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9, @d10, @d11, @d12)", con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtID.Text);
                        cmd.Parameters.AddWithValue("@d2", txtSalesmanID.Text);
                        cmd.Parameters.AddWithValue("@d3", txtSalesmanName.Text);
                        cmd.Parameters.AddWithValue("@d4", double.Parse(txtCommissionPer.Text));
                        cmd.Parameters.AddWithValue("@d5", txtAddress.Text);
                        cmd.Parameters.AddWithValue("@d6", txtCity.Text);
                        cmd.Parameters.AddWithValue("@d7", txtContactNo.Text);
                        cmd.Parameters.AddWithValue("@d8", txtEmailID.Text);
                        cmd.Parameters.AddWithValue("@d9", txtRemarks.Text);
                        cmd.Parameters.AddWithValue("@d10", cmbState.Text);
                        cmd.Parameters.AddWithValue("@d11", txtZipCode.Text);

                        var ms = new MemoryStream();
                        var bmpImage = new Bitmap(Picture.Image);
                        bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        byte[] data = ms.ToArray();
                        cmd.Parameters.Add(new SqlParameter("@d12", SqlDbType.Image) { Value = data });

                        cmd.ExecuteNonQuery();
                    }
                }

                LogFunc(lblUser.Text, "added the new Salesman having Salesman id '" + txtSalesmanID.Text + "'");
                MessageBox.Show("تم الحفظ بنجاح", "سجلات المندوبين", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
                fillState();
                Reset();

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
                if (MessageBox.Show("هل أنت متأكد أنك تريد حذف سجل المندوب?", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
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
                using (var con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (var cmd = new SqlCommand("SELECT Salesman.SM_ID FROM Salesman INNER JOIN InvoiceInfo ON Salesman.SM_ID = InvoiceInfo.SalesmanID WHERE Salesman.SM_ID=@d1", con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtID.Text);
                        using (var rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                MessageBox.Show("لا يمكن حذف المندوب حيث أنه مستخدم في سجل المبيعات", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }

                    using (var cmd = new SqlCommand("DELETE FROM Salesman WHERE SM_ID=@d1", con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtID.Text);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            LogFunc(lblUser.Text, "deleted the Salesman record having Salesman id '" + txtSalesmanID.Text + "'");
                            MessageBox.Show("تم الحذف بنجاح", "السجلات", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSalesmanName.Text))
            {
                MessageBox.Show("الرجاء كتابة اسم المندوب", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSalesmanName.Focus();
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

            try
            {
                using (var con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (var cmd = new SqlCommand("UPDATE Salesman SET Salesman_ID=@d2, [Name]=@d3, CommissionPer=@d4, Address=@d5, City=@d6, ContactNo=@d7, EmailID=@d8, Remarks=@d9, State=@d10, ZipCode=@d11, Photo=@d12 WHERE SM_ID=@d1", con))
                    {
                        cmd.Parameters.AddWithValue("@d2", txtSalesmanID.Text);
                        cmd.Parameters.AddWithValue("@d3", txtSalesmanName.Text);
                        cmd.Parameters.AddWithValue("@d4", double.Parse(txtCommissionPer.Text));
                        cmd.Parameters.AddWithValue("@d5", txtAddress.Text);
                        cmd.Parameters.AddWithValue("@d6", txtCity.Text);
                        cmd.Parameters.AddWithValue("@d7", txtContactNo.Text);
                        cmd.Parameters.AddWithValue("@d8", txtEmailID.Text);
                        cmd.Parameters.AddWithValue("@d9", txtRemarks.Text);
                        cmd.Parameters.AddWithValue("@d10", cmbState.Text);
                        cmd.Parameters.AddWithValue("@d11", txtZipCode.Text);

                        var ms = new MemoryStream();
                        var bmpImage = new Bitmap(Picture.Image);
                        bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        byte[] data = ms.ToArray();
                        cmd.Parameters.Add(new SqlParameter("@d12", SqlDbType.Image) { Value = data });

                        cmd.Parameters.AddWithValue("@d1", txtID.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                LogFunc(lblUser.Text, "updated the Salesman having Salesman id '" + txtSalesmanID.Text + "'");
                MessageBox.Show("تم التعديل بنجاح", "سجلات المندوبين", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnUpdate.Enabled = false;
                fillState();
                Reset();
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

        private void BStartCapture_Click(object sender, EventArgs e)
        {
          
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            SalesManScreen smscreen = new SalesManScreen();
            smscreen.lblSet.Text = "Salesman Entry";
            smscreen.Button2.Enabled= false;
            smscreen.Show();
            this.Hide();
        }


        public void fillState()
        {
            try
            {
                using (var con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (var adp = new SqlDataAdapter("SELECT DISTINCT RTRIM(State) FROM Salesman ORDER BY 1", con))
                    {
                        var ds = new DataSet("ds");
                        adp.Fill(ds);
                        var dtable = ds.Tables[0];
                        cmbState.Items.Clear();
                        foreach (DataRow drow in dtable.Rows)
                            cmbState.Items.Add(drow[0].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog1.Filter = "Images |*.png; *.bmp; *.jpg;*.jpeg; *.gif;";
                OpenFileDialog1.FilterIndex = 4;
                OpenFileDialog1.FileName = "";
                if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Picture.Image = Image.FromFile(OpenFileDialog1.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BRemove_Click(object sender, EventArgs e)
        {
            Picture.Image = Properties.Resources.noThing;
        }

        private void frmSalesman_Load(object sender, EventArgs e)
        {
            fillState();
        }

        private void cmbState_Format(object sender, ListControlConvertEventArgs e)
        {
            if (e.DesiredType == typeof(string))
            {
                e.Value = e.Value.ToString().Trim();
            }
        }

        private void txtCommissionPer_KeyPress(object sender, KeyPressEventArgs e)
        {
            var keyChar = e.KeyChar;

            if (char.IsControl(keyChar))
            {
                // Allow all control characters.
            }
            else if (char.IsDigit(keyChar) || keyChar == '.')
            {
                var text = this.txtCommissionPer.Text;
                var selectionStart = this.txtCommissionPer.SelectionStart;
                var selectionLength = this.txtCommissionPer.SelectionLength;

                text = text.Substring(0, selectionStart) + keyChar + text.Substring(selectionStart + selectionLength);

                if (double.TryParse(text, out double result) && text.Length > 16)
                {
                    // Reject a real number that is too long.
                    e.Handled = true;
                }
                else if (text.IndexOf('.') >= 0 && text.IndexOf('.') < text.Length - 3)
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

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
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

        private void BStartCapture_Click_1(object sender, EventArgs e)
        {

        }
    }
}
