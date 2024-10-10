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

namespace Accounting_System
{
    public partial class ChangePassword : Form
    {
        public ChangePassword()
        {
            SqlConnection con = new SqlConnection(DataAccessLayer.Con());
            InitializeComponent();
            this.FormClosed += new FormClosedEventHandler(formclosed);
        }
        private void formclosed(object sender, FormClosedEventArgs e)
        { 
            LoginForm frmLogin = new LoginForm();
            frmLogin.Show();
            frmLogin.UserID.Text = "";
            frmLogin.Password.Text = "";
            frmLogin.Password.Focus();
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            
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

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserID.Text))
                {
                    MessageBox.Show("الرجاء كتابة اسم المستخدم", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UserID.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(OldPassword.Text))
                {
                    MessageBox.Show("الرجاء كتابة كلمة السر القديمة", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    OldPassword.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewPassword.Text))
                {
                    MessageBox.Show("الرجاء كتابة كلمة السر الجديدة", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    NewPassword.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(ConfirmPassword.Text))
                {
                    MessageBox.Show("الرجاء تأكيد كلمة السر الجديدة", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ConfirmPassword.Focus();
                    return;
                }

                if (NewPassword.Text.Length < 5)
                {
                    MessageBox.Show("كلمة السر يجب الا تقل عن خمسة حروف أو أرقام", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    NewPassword.Text = "";
                    ConfirmPassword.Text = "";
                    NewPassword.Focus();
                    return;
                }
                else if (NewPassword.Text != ConfirmPassword.Text)
                {
                    MessageBox.Show("كلمة السر غير مطابقة", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    NewPassword.Text = "";
                    OldPassword.Text = "";
                    ConfirmPassword.Text = "";
                    OldPassword.Focus();
                    return;
                }
                else if (OldPassword.Text == NewPassword.Text)
                {
                    MessageBox.Show("كلمة السر الجديدة يجب أن تكون مختلفة عن القديمة", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    NewPassword.Text = "";
                    ConfirmPassword.Text = "";
                    NewPassword.Focus();
                    return;
                }

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string co = "UPDATE Registration SET password = @d1 WHERE userid = @d2 AND password = @d3";
                    using (SqlCommand cmd = new SqlCommand(co, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", (NewPassword.Text));
                        cmd.Parameters.AddWithValue("@d2", UserID.Text);
                        cmd.Parameters.AddWithValue("@d3", (OldPassword.Text));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            string st = "تم تغيير كلمة السر بنجاح";
                            LogFunc(UserID.Text, st);
                            MessageBox.Show(st);
                            this.Close();

                        }
                        else
                        {
                            MessageBox.Show("خطأ في اسم المستخدم أو كلمة السر", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            UserID.Text = "";
                            NewPassword.Text = "";
                            OldPassword.Text = "";
                            ConfirmPassword.Text = "";
                            UserID.Focus();
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
}
