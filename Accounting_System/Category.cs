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

namespace Accounting_System
{
    public partial class Category : Form
    {
        private string connectionString = DataAccessLayer.Con();

        public Category()
        {
            InitializeComponent();
        }

        public void Reset()
        {
            txtCategory.Text = "";
            btnSave.Enabled = true;
            btnDelete.Enabled = false;
            btnUpdate.Enabled = false;
            txtCategory.Focus();
        }

        private void DeleteRecord()
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                try
                {
                    cn.Open();
                    string checkQuery = "SELECT CategoryName FROM Category INNER JOIN SubCategory ON Category.CategoryName = SubCategory.Category WHERE CategoryName = @d1";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, cn))
                    {
                        checkCmd.Parameters.AddWithValue("@d1", txtCategoryName.Text);
                        using (SqlDataReader rdr = checkCmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                MessageBox.Show("Cannot delete. The category is in use in the SubCategory entries.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }

                    string deleteQuery = "DELETE FROM Category WHERE CategoryName = @d1";
                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, cn))
                    {
                        deleteCmd.Parameters.AddWithValue("@d1", txtCategoryName.Text);
                        int rowsAffected = deleteCmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            LogFunc(lblUser.Text, $"deleted the category '{txtCategory.Text}'");
                            MessageBox.Show("Deleted successfully.", "Records", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Getdata();
                            Reset();
                        }
                        else
                        {
                            MessageBox.Show("No records found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Reset();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgw_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (dgw.Rows.Count > 0)
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];
                    txtCategoryName.Text = dr.Cells[0].Value.ToString();
                    txtCategory.Text = dr.Cells[0].Value.ToString();
                    btnUpdate.Enabled = true;
                    btnDelete.Enabled = true;
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgw_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            string rowNumber = (e.RowIndex + 1).ToString();
            SizeF size = e.Graphics.MeasureString(rowNumber, this.Font);
            if (dgw.RowHeadersWidth < Convert.ToInt32(size.Width + 20))
            {
                dgw.RowHeadersWidth = Convert.ToInt32(size.Width + 20);
            }
            Brush brush = SystemBrushes.ControlText;
            e.Graphics.DrawString(rowNumber, this.Font, brush, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + (e.RowBounds.Height - size.Height) / 2);
        }

        public void Getdata()
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                try
                {
                    cn.Open();
                    string query = "SELECT RTRIM(categoryname) FROM category ORDER BY categoryname";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnNew_Click_1(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCategory.Text))
            {
                MessageBox.Show("Please enter the category name.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategory.Focus();
                return;
            }

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                try
                {
                    cn.Open();
                    string checkQuery = "SELECT categoryname FROM category WHERE categoryname = @d1";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, cn))
                    {
                        checkCmd.Parameters.AddWithValue("@d1", txtCategory.Text);
                        using (SqlDataReader rdr = checkCmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                MessageBox.Show("The category already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtCategory.Text = "";
                                txtCategory.Focus();
                                return;
                            }
                        }
                    }

                    string insertQuery = "INSERT INTO category(categoryName) VALUES (@d1)";
                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, cn))
                    {
                        insertCmd.Parameters.AddWithValue("@d1", txtCategory.Text);
                        insertCmd.ExecuteNonQuery();
                    }

                    LogFunc(lblUser.Text, $"added the new category '{txtCategory.Text}'");
                    MessageBox.Show("Saved successfully.", "Records", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnSave.Enabled = false;
                    Reset();
                    Getdata();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Category_Load(object sender, EventArgs e)
        {
            Getdata();
        }

        private void btnUpdate_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCategory.Text))
            {
                MessageBox.Show("Please enter the category name.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategory.Focus();
                return;
            }

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                try
                {
                    cn.Open();
                    string updateQuery = "UPDATE category SET categoryname = @d1 WHERE categoryname = @d2";
                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, cn))
                    {
                        updateCmd.Parameters.AddWithValue("@d1", txtCategory.Text);
                        updateCmd.Parameters.AddWithValue("@d2", txtCategoryName.Text);
                        updateCmd.ExecuteNonQuery();
                    }

                    LogFunc(lblUser.Text, $"updated the category '{txtCategory.Text}'");
                    MessageBox.Show("Updated successfully.", "Records", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnUpdate.Enabled = false;
                    Getdata();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to delete this record?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    DeleteRecord();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
