using Microsoft.Office.Interop.Excel;
using Pharmacy.DL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Accounting_System
{
    public partial class Company : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public Company()
        {
            InitializeComponent();
            Getdata();
            dgw.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dgw_RowPostPaint);
            dgw.MouseClick += new MouseEventHandler(dgw_MouseClick);
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
        private void Reset()
        {
            txtSTNo.Text = string.Empty;
            txtTIN.Text = string.Empty;
            txtEmailID.Text = string.Empty;
            txtContactNo.Text = string.Empty;
            txtCompanyName.Text = string.Empty;
            txtCIN.Text = string.Empty;
            txtAddress.Text = string.Empty;
            PictureBox1.Image = Properties.Resources.Company11; // Assuming 'Company1' is a resource in your project
            txtCompanyName.Focus();
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(txtCompanyName.Text))
                {
                    MessageBox.Show("الرجاء كتابة اسم الشركة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCompanyName.Focus();
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
                    MessageBox.Show("الرجاء كتابة رقم الهاتف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtContactNo.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtEmailID.Text))
                {
                    MessageBox.Show("الرجاء كتابة إيميل الشركة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmailID.Focus();
                    return;
                }


                string connectionString = DataAccessLayer.Con(); // Replace with your actual connection string

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Check if a record already exists
                    string checkQuery = "SELECT COUNT(*) FROM Company";
                    using (SqlCommand cmd = new SqlCommand(checkQuery, con))
                    {
                        int count = (int)cmd.ExecuteScalar(); // Use ExecuteScalar for single value queries

                        if (count >= 1)
                        {
                            MessageBox.Show("هذا السجل موجود مسبقا" + Environment.NewLine + "الرجاء تحديث معلومات الشركة", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Insert the new record
                    string insertQuery = "INSERT INTO Company(CompanyName, Address, ContactNo, EmailID, TIN, STNo, CIN, Logo) VALUES (@d1,@d2,@d3,@d4,@d5,@d6,@d7,@d8)";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtCompanyName.Text);
                        cmd.Parameters.AddWithValue("@d2", txtAddress.Text);
                        cmd.Parameters.AddWithValue("@d3", txtContactNo.Text);
                        cmd.Parameters.AddWithValue("@d4", txtEmailID.Text);
                        cmd.Parameters.AddWithValue("@d5", txtTIN.Text);
                        cmd.Parameters.AddWithValue("@d6", txtSTNo.Text);
                        cmd.Parameters.AddWithValue("@d7", txtCIN.Text);

                        // Convert image to byte array
                        using (MemoryStream ms = new MemoryStream())
                        {
                            if (ms.Length > 0 && PictureBox1.Image != null)
                            {
                                Bitmap bmpImage = new Bitmap(PictureBox1.Image);
                                bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                byte[] data = ms.ToArray();

                                cmd.Parameters.Add("@d8", SqlDbType.Image).Value = data;
                            }
                            else
                            {
                                Bitmap bmpImage = new Bitmap(Properties.Resources.noThing);
                                bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                byte[] data = ms.ToArray();

                                cmd.Parameters.Add("@d8", SqlDbType.Image).Value = data;
                            }

                        }

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("تم الحفظ بنجاح", "معلومات الشركة", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnSave.Enabled = false;
                    Getdata();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void Getdata()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT RTRIM(ID), RTRIM(CompanyName), RTRIM(Address), RTRIM(ContactNo), RTRIM(EmailID), RTRIM(TIN), RTRIM(STNo), RTRIM(CIN), Logo FROM Company", con))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0].ToString(), rdr[1].ToString(), rdr[2].ToString(), rdr[3].ToString(), rdr[4].ToString(), rdr[5].ToString(), rdr[6].ToString(), rdr[7].ToString(), rdr[8]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCompanyName.Text))
            {
                MessageBox.Show("الرجاء كتابة اسم الشركة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCompanyName.Focus();
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
                MessageBox.Show("الرجاء كتابة رقم الهاتف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContactNo.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtEmailID.Text))
            {
                MessageBox.Show("الرجاء كتابة إيميل الشركة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmailID.Focus();
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "UPDATE Company SET CompanyName = @d1, Address = @d2, ContactNo = @d3, EmailID = @d4, TIN = @d5, STNo = @d6, CIN = @d7, Logo = @d8 WHERE ID = @id";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtCompanyName.Text);
                        cmd.Parameters.AddWithValue("@d2", txtAddress.Text);
                        cmd.Parameters.AddWithValue("@d3", txtContactNo.Text);
                        cmd.Parameters.AddWithValue("@d4", txtEmailID.Text);
                        cmd.Parameters.AddWithValue("@d5", txtTIN.Text);
                        cmd.Parameters.AddWithValue("@d6", txtSTNo.Text);
                        cmd.Parameters.AddWithValue("@d7", txtCIN.Text);

                        // Convert image to byte array
                        using (MemoryStream ms = new MemoryStream())
                        {
                            Bitmap bmpImage = new Bitmap(PictureBox1.Image);
                            bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                            byte[] data = ms.ToArray();

                            cmd.Parameters.Add("@d8", SqlDbType.VarBinary).Value = data;
                        }

                        cmd.Parameters.AddWithValue("@id", txtID.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                string logMessage = $"updated the company '{txtCompanyName.Text}' info";
                LogFunc(lblUser.Text, logMessage);
                MessageBox.Show("تم التعديل بنجاح", "معلومات الشركة", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnUpdate.Enabled = false;
                Getdata();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static void LogFunc(string st1, string st2)
        {
            using (SqlConnection con = DataAccessLayer.cn)
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("هل أنت متأكد أنك تريد حذف هذا السجل?", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
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
                int rowsAffected = 0;
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "DELETE FROM company WHERE ID = @d1";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtID.Text);
                        rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            string logMessage = $"deleted the company '{txtCompanyName.Text}'";
                            LogFunc(lblUser.Text, logMessage);
                            MessageBox.Show("تم الحذف بنجاح", "السجلات", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Getdata();
                            Reset();
                        }
                        else
                        {
                            MessageBox.Show("لا يوجد سجلات", "عفوا", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Images | *.png; *.bmp; *.jpg; *.jpeg; *.gif; *.ico";
                    openFileDialog.FilterIndex = 4;
                    openFileDialog.FileName = ""; // Clear the file name

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        PictureBox1.Image = Image.FromFile(openFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dgw_RowPostPaint(object sender , DataGridViewRowPostPaintEventArgs e)
        {
/*            try
            {
                string rowNumber = (e.RowIndex + 1).ToString();

                // Measure the size of the row number string
                SizeF size = e.Graphics.MeasureString(rowNumber, this.Font);

                // Adjust the row header width if necessary
                if (dgw.RowHeadersWidth < Convert.ToInt32(size.Width + 20))
                {
                    dgw.RowHeadersWidth = Convert.ToInt32(size.Width + 20);
                }

                // Define the brush and draw the row number
                using (Brush b = SystemBrushes.ControlText)
                {
                    e.Graphics.DrawString(rowNumber, this.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }
        private void dgw_MouseClick(object sender, MouseEventArgs e) {
            try
            {
                if (dgw.Rows.Count > 0)
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];
                    txtID.Text = dr.Cells[0].Value.ToString();
                    txtCompanyName.Text = dr.Cells[1].Value.ToString();
                    txtAddress.Text = dr.Cells[2].Value.ToString();
                    txtContactNo.Text = dr.Cells[3].Value.ToString();
                    txtEmailID.Text = dr.Cells[4].Value.ToString();
                    txtTIN.Text = dr.Cells[5].Value.ToString();
                    txtSTNo.Text = dr.Cells[6].Value.ToString();
                    txtCIN.Text = dr.Cells[7].Value.ToString();

                    byte[] data = (byte[])dr.Cells[8].Value;
                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        PictureBox1.Image = Image.FromStream(ms);
                    }

                    btnUpdate.Enabled = true;
                    btnSave.Enabled = false;
                    btnDelete.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Company_Load(object sender, EventArgs e)
        {

        }
    }
}
