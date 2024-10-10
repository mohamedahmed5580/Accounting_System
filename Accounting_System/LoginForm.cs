using Emgu.CV.CvEnum;
using Pharmacy.DL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Accounting_System
{
    public partial class LoginForm : Form
    {
        public static LoginForm _instance;
        public static LoginForm instance;
        public static LoginForm Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new LoginForm();
                }
                return _instance;
            }

        }
        public LoginForm()
        {
            InitializeComponent();
            this.FormClosed += new FormClosedEventHandler(formclosed);
            instance = this;
        }
        private void formclosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            label3.Parent = pictureBox1;
            label3.BackColor = Color.Transparent;



            label5.Parent = pictureBox1;
            label5.BackColor = Color.Transparent;

            fillUsers();
            UserID.SelectedIndex = 0;

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void gunaButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserID.Text))
                {
                    MessageBox.Show("Please enter user id", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    UserID.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(Password.Text))
                {
                    MessageBox.Show("Please enter password", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Password.Focus();
                    return;
                }

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT RTRIM(UserID), RTRIM(Password) FROM Registration WHERE UserID = @d1 AND Password = @d2 AND Active = 'Yes'";
                        cmd.Parameters.AddWithValue("@d1", UserID.Text);
                        cmd.Parameters.AddWithValue("@d2", (Password.Text));

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                // Close the previous reader
                                rdr.Close();

                                // Fetch user type
                                cmd.CommandText = "SELECT UserType FROM Registration WHERE UserID = @d3 AND Password = @d4";
                                cmd.Parameters.AddWithValue("@d3", UserID.Text);
                                cmd.Parameters.AddWithValue("@d4", (Password.Text));

                                using (SqlDataReader userTypeReader = cmd.ExecuteReader())
                                {
                                    if (userTypeReader.Read())
                                    {
                                        UserType.Text = userTypeReader.GetValue(0).ToString().Trim();
                                    }
                                }

                                // Set permissions based on user type
                                if (UserType.Text == "Admin")
                                {
                                    this.Hide();
                                    basic frm = new basic();
                                   /* frm.اToolStripMenuItem.Enabled = true;
                                    frm.المستخدمينToolStripMenuItem.Enabled = true;
                                    frm.toolStripMenuItem5.Enabled = true;
                                    frm.نسخToolStripMenuItem.Enabled = true;
                                    frm.toolStripMenuItem17.Enabled = true;
                                    frm.toolStripMenuItem23.Enabled = true;
                                    frm.toolStripMenuItem33.Enabled = true;
                                    frm.toolStripMenuItem19.Enabled = true;
                                    frm.toolStripMenuItem25.Enabled = true;
                                    frm.toolStripMenuItem9.Enabled = true;
                                    frm.toolStripMenuItem7.Enabled = true;
                                    frm.toolStripMenuItem29.Enabled = true;
                            //        frm.SalesmanToolStripMenuItem.Enabled = true;
                             //       frm.SendSMSToolStripMenuItem.Enabled = true;
                                    frm.مرتجعمبيعاتToolStripMenuItem.Enabled = true;
                                    frm.toolStripMenuItem21.Enabled = true;
                                //    frm.ToolStripMenuItemSetting.Enabled = true;
                                    frm.toolStripMenuItem15.Enabled = true;
                                    frm.lblUser.Text = UserID.Text;
                                    frm.lblUserType.Text = UserType.Text;*/
                                    LogFunc(UserID.Text, "Successfully logged in");
                                    frm.Show();
                                }
                                else if (UserType.Text == "Sales Person")
                                {
                                    this.Hide();
                                    basic frm = new basic();
                                  /*  frm.اToolStripMenuItem.Enabled = false;
                                    frm.المستخدمينToolStripMenuItem.Enabled = false;
                                    frm.toolStripMenuItem5.Enabled = false;
                                    frm.نسخToolStripMenuItem.Enabled = false;
                                    frm.toolStripMenuItem17.Enabled = true;
                                    frm.toolStripMenuItem23.Enabled = false;
                                    frm.toolStripMenuItem33.Enabled = true;
                                   // frm.StockToolStripMenuItem.Enabled = false;
                                    //frm.ServiceToolStripMenuItem.Enabled = true;
                                   // frm.جردالمخزونToolStripMenuItem.Enabled = true;
                                    frm.toolStripMenuItem19.Enabled = true;
                                    frm.toolStripMenuItem25.Enabled = true;
                                    frm.toolStripMenuItem9.Enabled = false;
                                    frm.toolStripMenuItem7.Enabled = false;
                                    frm.toolStripMenuItem29.Enabled = false;
                                   // frm.SalesmanToolStripMenuItem.Enabled = false;
                                   // frm.SendSMSToolStripMenuItem.Enabled = false;
                                    frm.مرتجعمبيعاتToolStripMenuItem.Enabled = true;
                                    frm.toolStripMenuItem21.Enabled = false;*/
                                    frm.btnProductMaster.Enabled = false;
                                    frm.btnSalesmanMaster.Enabled = false;
                                    frm.btnStockAdjustment.Enabled = false;
                                    frm.btnPayment.Enabled = false;
                                    frm.btnAccountingReports.Enabled = false;
                                    frm.btnPOSReport.Enabled = false;
                                    frm.btnPurchaseOrder.Enabled = false;
                                    frm.btnWorkPeriod.Enabled = false;
                                    frm.btnPOSRecord.Enabled = false;
                                    frm.lblUser.Text = UserID.Text;
                                    frm.lblUserType.Text = UserType.Text;
                                    LogFunc(UserID.Text, "Successfully logged in");
                                    frm.Show();
                                }
                                else if (UserType.Text == "Inventory Manager")
                                {
                                    this.Hide();
                                    basic frm = new basic();
                                    frm.اToolStripMenuItem.Enabled = false;
                                    frm.المستخدمينToolStripMenuItem.Enabled = false;
                                   /* frm.toolStripMenuItem5.Enabled = false;
                                    frm.نسخToolStripMenuItem.Enabled = false;
                                    frm.toolStripMenuItem17.Enabled = false;
                                    frm.toolStripMenuItem23.Enabled = false;
                                    frm.toolStripMenuItem33.Enabled = true;
                                  //  frm.StockToolStripMenuItem.Enabled = true;
                                  //  frm.ServiceToolStripMenuItem.Enabled = false;
                                  //  frm.جردالمخزونToolStripMenuItem.Enabled = true;
                                    frm.toolStripMenuItem19.Enabled = false;
                                    frm.toolStripMenuItem25.Enabled = false;
                                    frm.toolStripMenuItem9.Enabled = false;
                                    frm.toolStripMenuItem7.Enabled = false;
                                    frm.toolStripMenuItem29.Enabled = false;
                                   // frm.SalesmanToolStripMenuItem.Enabled = false;
                                  //  frm.SendSMSToolStripMenuItem.Enabled = false;
                                    frm.toolStripMenuItem21.Enabled = false;*/
                                    frm.btnProductMaster.Enabled = false;
                                    frm.btnSalesmanMaster.Enabled = false;
                                    frm.btnStockAdjustment.Enabled = true;
                                    frm.btnPayment.Enabled = false;
                                    frm.btnAccountingReports.Enabled = false;
                                    frm.btnPOSReport.Enabled = false;
                                    frm.btnPurchaseOrder.Enabled = false;
                                    frm.btnWorkPeriod.Enabled = false;
                                    frm.btnPOSRecord.Enabled = false;
                                    frm.lblUser.Text = UserID.Text;
                                    frm.lblUserType.Text = UserType.Text;
                                    LogFunc(UserID.Text, "Successfully logged in");
                                    frm.Show();
                                }
                                else if (UserType.Text == "accountant")
                                {
                                    this.Hide();
                                    basic frm = new basic();
                                    frm.اToolStripMenuItem.Enabled = false;
                                    frm.المستخدمينToolStripMenuItem.Enabled = false;
                                /*    frm.toolStripMenuItem5.Enabled = false;
                                    frm.نسخToolStripMenuItem.Enabled = false;
                                    frm.toolStripMenuItem17.Enabled = false;
                                    frm.toolStripMenuItem23.Enabled = false;
                                    frm.toolStripMenuItem33.Enabled = false;
                                  //  frm.StockToolStripMenuItem.Enabled = false;
                                   // frm.ServiceToolStripMenuItem.Enabled = true;
                                   // frm.جردالمخزونToolStripMenuItem.Enabled = true;
                                    frm.toolStripMenuItem19.Enabled = false;
                                    frm.toolStripMenuItem25.Enabled = true;
                                    frm.toolStripMenuItem9.Enabled = true;
                                    frm.toolStripMenuItem7.Enabled = true;
                                    frm.toolStripMenuItem29.Enabled = false;
                                 //   frm.SalesmanToolStripMenuItem.Enabled = false;
                                  //  frm.SendSMSToolStripMenuItem.Enabled = false;
                                    frm.toolStripMenuItem21.Enabled = true;*/
                                    frm.btnProductMaster.Enabled = false;
                                    frm.btnSalesmanMaster.Enabled = false;
                                    frm.btnStockAdjustment.Enabled = true;
                                    frm.btnPayment.Enabled = false;
                                    frm.btnAccountingReports.Enabled = false;
                                    frm.btnPOSReport.Enabled = false;
                                    frm.btnPurchaseOrder.Enabled = false;
                                    frm.btnWorkPeriod.Enabled = false;
                                    frm.btnPOSRecord.Enabled = false;
                                    frm.lblUser.Text = UserID.Text;
                                    frm.lblUserType.Text = UserType.Text;
                                    LogFunc(UserID.Text, "Successfully logged in");
                                    frm.Show();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Login is Failed...Try again!", "Login Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                UserID.Text = "";
                                Password.Text = "";
                                UserID.Focus();
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

        private void label5_Click(object sender, EventArgs e)
        {
            OpenLink("https://WWW.URTECH-EGY.com/");
        }
        private void OpenLink(string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to open the link: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("The URL is invalid or empty.");
            }
        }

        private void label5_MouseHover(object sender, EventArgs e)
        {
            
        }

        private void label5_FontChanged(object sender, EventArgs e)
        {
            label5.ForeColor = Color.Blue;
        }

        private void gunaButton2_Click(object sender, EventArgs e)
        {
            this.Hide();
            ChangePassword changePassword = new ChangePassword();
            changePassword.UserID.Focus();
            changePassword.Show();
        }
        private void fillUsers()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlDataAdapter adp = new SqlDataAdapter())
                    {
                        adp.SelectCommand = new SqlCommand("SELECT DISTINCT UserID FROM Registration WHERE Active='Yes'", con);
                        DataSet ds = new DataSet("ds");
                        adp.Fill(ds);
                        DataTable dtable = ds.Tables[0];
                        UserID.Items.Clear();
                        foreach (DataRow drow in dtable.Rows)
                        {
                            UserID.Items.Add(drow[0].ToString());
                        }
                    }
                }
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
    }
}
