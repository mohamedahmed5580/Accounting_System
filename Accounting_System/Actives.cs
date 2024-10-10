using DeviceId;
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
    public partial class Actives : Form
    {
        public Actives()
        {
            InitializeComponent();
            textBoxMacAddress.Text= new DeviceIdBuilder().AddMacAddress().ToString();
        }

        private void Actives_Load(object sender, EventArgs e)
        {

        }

        private void gunaTextBox1_TextChanged(object sender, EventArgs e)
        {

        }


        private void buttonActivation_Click(object sender, EventArgs e)
        {
         
        }


        private void buttonActivation_Click_1(object sender, EventArgs e)
        {
            string activationCode = null;

            try
            {
                // Retrieve the activation code from the database
                string query1 = "SELECT code FROM Free_Edition";
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open(); // Open the connection

                    using (SqlCommand cmd = new SqlCommand(query1, con))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                activationCode = rdr["code"].ToString();
                            }
                        }
                    }
                }


                // Compare the user-entered code with the retrieved code

                if (!string.IsNullOrEmpty(activationCode) && textBoxKey.Text == activationCode)
                {
                    // Trial activation key
                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open(); // Open the connection again before executing any command
                        string deleteQuery = "DELETE FROM Free_Edition WHERE code = @code";
                        using (SqlCommand sqlCommand = new SqlCommand(deleteQuery, con))
                        {
                            sqlCommand.Parameters.AddWithValue("@code", activationCode);
                            sqlCommand.ExecuteNonQuery(); // Execute the command
                        }
                    }

                    // Update application settings for trial activation
                    Properties.Settings.Default.IsActive = true;
                    Properties.Settings.Default.IsSoftwerAcitve = true;
                    Properties.Settings.Default.IsFirstOpen = false;
                    Properties.Settings.Default.EndDate = DateTime.Now.AddDays(1); // Adjust the logic to set the correct end date
                    Properties.Settings.Default.Save();

                    MessageBox.Show($"لقد بدأت النسخة التجريبية الخاصة بك وستنتهي في {Properties.Settings.Default.EndDate.ToLongDateString()}");

                    OpenNewForm(new LoginForm());
                }
                else if (textBoxKey.Text == "AZ47R-WMK5Z-S54B0-Q30QH-Q42LJ")
                {
                    // Full activation key
                    Properties.Settings.Default.IsActive = true;
                    Properties.Settings.Default.IsSoftwerAcitve = true;
                    Properties.Settings.Default.IsFirstOpen = false;
                    Properties.Settings.Default.EndDate = DateTime.Now.AddYears(1000);
                    Properties.Settings.Default.Save();

                    MessageBox.Show("انت في النسخه المدفوعه الان اهلا بك");
                    OpenNewForm(new LoginForm());
                }
                else
                {
                    // Invalid or reused trial key
                    MessageBox.Show("لا يمكن استخدام الرمز التجريبي مرة اخرى يرجى ادخال الرمز المدفوع");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء عملية التفعيل: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void OpenNewForm(Form newForm)
        {
            // Subscribe to the new form's FormClosed event
            newForm.FormClosed += (s, args) => this.Close();
            newForm.Show();
            this.Hide(); // Optionally hide the current form until the new one is shown
        }

    }
}
