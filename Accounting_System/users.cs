using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Pharmacy.DL;
using System.Security.Cryptography;

namespace Accounting_System
{
    public partial class users : Form
    {
        public users()
        {
            SqlConnection con = new SqlConnection(DataAccessLayer.Con());
            InitializeComponent();
            Getdata();
            dgw.MouseClick += new MouseEventHandler(dgw_MouseClick);
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void Reset()
        {
            txtContactNo.Text = "";
            txtEmailID.Text = "";
            txtName.Text = "";
            txtPassword.Text = "";
            txtUserID.Text = "";
            cmbUserType.SelectedIndex = -1;
            chkActive.Checked = true;
            txtUserID.Focus();
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtUserID.Text == "")
            {
                MessageBox.Show("الرجاء كتابة كود المستخدم", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUserID.Focus();
                return;
            }
            if (cmbUserType.Text == "")
            {
                MessageBox.Show("الرجاء اختيار صلاحية المستخدم", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbUserType.Focus();
                return;
            }
            if (txtPassword.Text == "")
            {
                MessageBox.Show("الرجاء كتابة كلمة السر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }
            if (txtName.Text == "")
            {
                MessageBox.Show("الرجاء كتابة اسم المستخدم", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }
            // Uncomment if needed to validate contact number
            /*
            if (txtContactNo.Text == "")
            {
                MessageBox.Show("الرجاء كتابة رقم الاتصال.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContactNo.Focus();
                return;
            }
            */

            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string ct = "select userid from registration where userid=@d1";
                    using (SqlCommand cmd = new SqlCommand(ct, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtUserID.Text);
                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.Read())
                        {
                            MessageBox.Show("كود المستخدم موجود مسبقا", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtUserID.Text = "";
                            txtUserID.Focus();
                            if (rdr != null)
                                rdr.Close();
                            return;
                        }
                        rdr.Close();
                    }
                    con.Close();
                }

                string st1 = chkActive.Checked ? "Yes" : "No";

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string cb = "insert into Registration(userid, UserType, Password, Name, ContactNo, EmailID, JoiningDate, Active) " +
                                "VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8)";
                    using (SqlCommand cmd = new SqlCommand(cb, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtUserID.Text);
                        cmd.Parameters.AddWithValue("@d2", cmbUserType.Text);
                        cmd.Parameters.AddWithValue("@d3", (txtPassword.Text.Trim()));
                        cmd.Parameters.AddWithValue("@d4", txtName.Text);
                        cmd.Parameters.AddWithValue("@d5", txtContactNo.Text);
                        cmd.Parameters.AddWithValue("@d6", txtEmailID.Text);
                        cmd.Parameters.AddWithValue("@d7", DateTime.Now);
                        cmd.Parameters.AddWithValue("@d8", st1);
                        cmd.ExecuteReader();
                    }
                    con.Close();
                }

                string st = "added the new user '" + txtUserID.Text + "'";
                LogFunc(lblUser.Text, st);
                MessageBox.Show("تم التسجيل بنجاح", "المستخدمين", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
                Getdata();
                Reset();
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("هل انت متأكد أنك تريد حذف بيانات هذا المستخدم?", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
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
                // Uncomment this if admin accounts should not be deleted
                /*
                if (txtUserID.Text == "admin" || txtUserID.Text == "Admin")
                {
                    MessageBox.Show("لا يمكن حذف حساب المسؤول", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                */

                int RowsAffected = 0;
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string cq = "delete from Registration where userid=@UserID";
                    using (SqlCommand cmd = new SqlCommand(cq, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", txtUserID.Text);
                        RowsAffected = cmd.ExecuteNonQuery();
                    }
                }

                if (RowsAffected > 0)
                {
                    string st = "deleted the user '" + txtUserID.Text + "'";
                    LogFunc(lblUser.Text, st);
                    MessageBox.Show("تم الحذف بنجاح", "السجلات", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Getdata();
                    Reset();
                }
                else
                {
                    MessageBox.Show("لا يوجد سجلات", "عذرًا ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset();
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
                if (txtUserID.Text == "")
                {
                    MessageBox.Show("الرجاء كتابة كود المستخدم", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUserID.Focus();
                    return;
                }
                if (cmbUserType.Text == "")
                {
                    MessageBox.Show("الرجاء اختيار صلاحية المستخدم", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbUserType.Focus();
                    return;
                }
                if (txtPassword.Text == "")
                {
                    MessageBox.Show("الرجاء كتابة كلمة السر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    return;
                }
                if (txtName.Text == "")
                {
                    MessageBox.Show("الرجاء كتابة اسم المستخدم", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtName.Focus();
                    return;
                }
                if (txtContactNo.Text == "")
                {
                    MessageBox.Show("الرجاء كتابة رقم الاتصال", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtContactNo.Focus();
                    return;
                }

                string st1 = chkActive.Checked ? "Yes" : "No";

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string cb = "update registration set userid=@d1, usertype=@d2, password=@d3, name=@d4, contactno=@d5, emailid=@d6, Active=@d8 where userid=@d7";
                    using (SqlCommand cmd = new SqlCommand(cb, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtUserID.Text);
                        cmd.Parameters.AddWithValue("@d2", cmbUserType.Text);
                        cmd.Parameters.AddWithValue("@d3", (txtPassword.Text.Trim()));
                        cmd.Parameters.AddWithValue("@d4", txtName.Text);
                        cmd.Parameters.AddWithValue("@d5", txtContactNo.Text);
                        cmd.Parameters.AddWithValue("@d6", txtEmailID.Text);
                        cmd.Parameters.AddWithValue("@d7", TextBox1.Text);  // Assuming TextBox1 contains the original userID
                        cmd.Parameters.AddWithValue("@d8", st1);
                        cmd.ExecuteReader();
                    }
                }

                string st = "updated the user '" + txtUserID.Text + "' details";
                LogFunc(lblUser.Text, st);
                MessageBox.Show("تم التعديل بنجاح", "معلومات المستخدم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnUpdate.Enabled = false;
                Reset();
                Getdata();
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
        public void Getdata()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT RTRIM(userid), RTRIM(UserType), RTRIM(Password), RTRIM(Name), RTRIM(EmailID), RTRIM(ContactNo), RTRIM(Active), JoiningDate FROM Registration ORDER BY JoiningDate", con))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], (rdr[2].ToString()), rdr[3], rdr[4], rdr[5], rdr[6], rdr[7]);
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
        private void dgw_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (dgw.Rows.Count > 0)
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];
                    txtUserID.Text = dr.Cells[0].Value.ToString();
                    TextBox1.Text = dr.Cells[0].Value.ToString();
                    cmbUserType.Text = dr.Cells[1].Value.ToString();
                    txtPassword.Text = dr.Cells[2].Value.ToString();
                    txtName.Text = dr.Cells[3].Value.ToString();
                    txtContactNo.Text = dr.Cells[5].Value.ToString();
                    txtEmailID.Text = dr.Cells[4].Value.ToString();

                    if (dr.Cells[6].Value.ToString() == "Yes")
                    {
                        chkActive.Checked = true;
                    }
                    else
                    {
                        chkActive.Checked = false;
                    }

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

        private void btnCheckAvailability_Click(object sender, EventArgs e)
        {
            if (txtUserID.Text == "")
            {
                MessageBox.Show("الرجاء كتابة كود المستخدم", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUserID.Focus();
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string ct = "SELECT userid FROM registration WHERE userid=@d1";
                    using (SqlCommand cmd = new SqlCommand(ct, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtUserID.Text);
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                MessageBox.Show("كود المستخدم غير متاح", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                            else
                            {
                                MessageBox.Show("كود المستخدم متاح", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
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

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
