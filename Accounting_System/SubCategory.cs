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

namespace Accounting_System
{
    public partial class SubCategory : Form
    {
        SqlConnection cn = new SqlConnection(DataAccessLayer.Con());

        public SubCategory()
        {
            InitializeComponent();
        }
        public void fillCombo()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
                {
                    cn.Open();
                    SqlDataAdapter adp = new SqlDataAdapter("SELECT distinct RTRIM(CategoryName) FROM Category", cn);
                    DataSet ds = new DataSet();
                    adp.Fill(ds);
                    DataTable dtable = ds.Tables[0];
                    cmbCategory.Items.Clear();
                    foreach (DataRow drow in dtable.Rows)
                    {
                        cmbCategory.Items.Add(drow[0].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Reset()
        {
            cmbCategory.SelectedIndex = -1;
            txtSearchByCategory.Text = "";
            txtSearchBySubCategory.Text = "";
            txtSubCategory.Text = "";
            txtSubCategory.Focus();
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            Getdata();
            auto();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSubCategory.Text))
            {
                MessageBox.Show("الرجاء كتابة اسم الفئة الفرعية", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSubCategory.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(cmbCategory.Text))
            {
                MessageBox.Show("الرجاء اختيار الفئة الرئيسية", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCategory.Focus();
                return;
            }
            try
            {
                using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
                {
                    cn.Open();
                    string ct = "select SubCategoryName,Category from SubCategory where SubCategoryName=@d1 and Category=@d2";
                    using (SqlCommand cmd = new SqlCommand(ct, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtSubCategory.Text);
                        cmd.Parameters.AddWithValue("@d2", cmbCategory.Text);
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                MessageBox.Show("هذه الفئة الفرعية موجودة بالفعل", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtSubCategory.Text = "";
                                txtSubCategory.Focus();
                                return;
                            }
                        }
                    }

                    string cb = "insert into SubCategory(SubCategoryName,Category,ID) VALUES (@d1,@d2,@d3)";
                    using (SqlCommand cmd = new SqlCommand(cb, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtSubCategory.Text);
                        cmd.Parameters.AddWithValue("@d2", cmbCategory.Text);
                        cmd.Parameters.AddWithValue("@d3", txtID.Text);
                        cmd.ExecuteNonQuery();
                    }

                    LogFunc(lblUser.Text, "added the new subcategory '" + txtSubCategory.Text + "' having Category '" + cmbCategory.Text + "'");
                    MessageBox.Show("تم الحفظ بنجاح", "السجلات", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnSave.Enabled = false;
                    Getdata();
                    Reset();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("هل أنت متأكد بالفعل أنك تريد حذف هذا السجل?", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
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
                using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
                {
                    cn.Open();
                    int RowsAffected = 0;

                    string cl = "select SubCategoryID from Product,SubCategory where Product.SubCategoryID=SubCategory.ID and SubCategoryID=@d1";
                    using (SqlCommand cmd = new SqlCommand(cl, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                MessageBox.Show("لا يمكن حذف هذه الفئة الفرعية لأنها بالفعل مستخدمة في الأصناف", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }

                    string cq = "delete from SubCategory where ID=@d1";
                    using (SqlCommand cmd = new SqlCommand(cq, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                        RowsAffected = cmd.ExecuteNonQuery();
                    }

                    if (RowsAffected > 0)
                    {
                        LogFunc(lblUser.Text, "deleted the subcategory '" + txtSubCategory.Text + "' having Category '" + cmbCategory.Text + "'");
                        MessageBox.Show("تم الحذف بنجاح", "السجلات", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Getdata();
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSubCategory.Text))
                {
                    MessageBox.Show("الرجاء كتابة اسم الفئة الفرعية", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSubCategory.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(cmbCategory.Text))
                {
                    MessageBox.Show("الرجاء اختيار الفئة الرئيسية", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbCategory.Focus();
                    return;
                }

                using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
                {
                    cn.Open();
                    string cb = "update SubCategory set SubCategoryName=@d1, Category=@d2 where ID=@d3";
                    using (SqlCommand cmd = new SqlCommand(cb, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtSubCategory.Text);
                        cmd.Parameters.AddWithValue("@d2", cmbCategory.Text);
                        cmd.Parameters.AddWithValue("@d3", Convert.ToInt32(txtID.Text));
                        cmd.ExecuteNonQuery();
                    }

                    LogFunc(lblUser.Text, "updated the sub category '" + txtSubCategory.Text + "' having Category '" + cmbCategory.Text + "'");
                    MessageBox.Show("تم التعديل بنجاح", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnUpdate.Enabled = false;
                    Getdata();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Getdata()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT RTRIM(ID),RTRIM(SubCategoryName), RTRIM(Category) from SubCategory order by SubCategoryName", cn);
                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    dgw.Rows.Clear();
                    while (rdr.Read())
                    {
                        dgw.Rows.Add(rdr[0], rdr[1], rdr[2]);
                    }
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

        private void dgw_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            string strRowNumber = (e.RowIndex + 1).ToString();
            SizeF size = e.Graphics.MeasureString(strRowNumber, this.Font);
            if (dgw.RowHeadersWidth < Convert.ToInt32(size.Width + 20))
            {
                dgw.RowHeadersWidth = Convert.ToInt32(size.Width + 20);
            }
            Brush b = SystemBrushes.ControlText;
            e.Graphics.DrawString(strRowNumber, this.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + (e.RowBounds.Height - size.Height) / 2);
        }

        private void auto()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
                {
                    cn.Open();
                    string sql = "SELECT MAX(ID) FROM SubCategory";
                    using (SqlCommand cmd = new SqlCommand(sql, cn))
                    {
                        var result = cmd.ExecuteScalar();
                        txtID.Text = (result == DBNull.Value ? 1 : Convert.ToInt32(result) + 1).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgw_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (dgw.Rows.Count > 0)
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];
                    txtID.Text = dr.Cells[0].Value.ToString();
                    txtSubCategory.Text = dr.Cells[1].Value.ToString();
                    cmbCategory.Text = dr.Cells[2].Value.ToString();
                    btnDelete.Enabled = true;
                    btnUpdate.Enabled = true;
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSearchBySubCategory_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT RTRIM(ID),RTRIM(SubCategoryName), RTRIM(Category) from SubCategory where SubCategoryName like '" + txtSearchBySubCategory.Text + "%' order by SubCategoryName", cn);
                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    dgw.Rows.Clear();
                    while (rdr.Read())
                    {
                        dgw.Rows.Add(rdr[0], rdr[1], rdr[2]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSearchByCategory_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT RTRIM(ID),RTRIM(SubCategoryName), RTRIM(Category) from SubCategory where Category like '" + txtSearchByCategory.Text + "%' order by SubCategoryName", cn);
                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    dgw.Rows.Clear();
                    while (rdr.Read())
                    {
                        dgw.Rows.Add(rdr[0], rdr[1], rdr[2]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void cmbCategory_Format(object sender, System.Windows.Forms.ListControlConvertEventArgs e)
        {
            if (object.ReferenceEquals(e.DesiredType, typeof(string)))
            {
                e.Value = e.Value.ToString();
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

        public static void SupplierLedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g)
        {
            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    cn.Open();
                    string query = "INSERT INTO SupplierLedgerBook(Date, Name, LedgerNo, Label, Debit, Credit, PartyID) VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7)";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static void SendMail(string s1, string s2, string s3, string s5, string s6, int s7, string s8, string s9)
        {
            var msg = new MailMessage();
            try
            {
                msg.From = new MailAddress(s1);
                msg.To.Add(s2);
                msg.Body = s3;
                msg.IsBodyHtml = true;
                msg.Subject = s5;

                var smt = new SmtpClient(s6)
                {
                    Port = s7,
                    Credentials = new NetworkCredential(s8, s9),
                    EnableSsl = true
                };
                smt.Send(msg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SupplierLedgerDelete(string a)
        {
            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    cn.Open();
                    string query = "DELETE FROM SupplierLedgerBook WHERE LedgerNo = @d1";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", a);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static void SupplierLedgerUpdate(DateTime a, string b, decimal e, decimal f, string g, string h)
        {
            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    cn.Open();
                    string query = "UPDATE SupplierLedgerBook SET Date = @d1, Name = @d2, Debit = @d3, Credit = @d4 WHERE LedgerNo = @d5 AND Label = @d6";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", a);
                        cmd.Parameters.AddWithValue("@d2", b);
                        cmd.Parameters.AddWithValue("@d3", e);
                        cmd.Parameters.AddWithValue("@d4", f);
                        cmd.Parameters.AddWithValue("@d5", g);
                        cmd.Parameters.AddWithValue("@d6", h);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void SubCategory_Load(object sender, EventArgs e)
        {
            Getdata();
            fillCombo();
        }
    }
}
