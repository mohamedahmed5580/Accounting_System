using Microsoft.Office.Interop.Excel;
using Pharmacy.DL;
using Pharmacy.PL;
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
    public partial class loading : Form
    {
        SqlConnection cn = new SqlConnection(DataAccessLayer.Con());

        public loading()
        {
            InitializeComponent();
        }

        private void loading_Load(object sender, EventArgs e)
        {
            
        }

        private void pbDBConfig_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            DBConfig c = new DBConfig();
            c.ShowDialog();
            timer1.Enabled = true;
        }

        private void Prbar_Click(object sender, EventArgs e)
        {

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                Prbar.Value += 5;
                if (Prbar.Value == 100)
                {
                    timer1.Enabled = false;
                    CheckTrial();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        public void CheckTrial()
        {

            // Check if it's the first time the application is opened
            if (Properties.Settings.Default.IsFirstOpen)
            {
                // Initialize trial period
                DateTime startDemo = DateTime.Now;
                Properties.Settings.Default.StartDate = startDemo;
                Properties.Settings.Default.EndDate = startDemo.AddMinutes(1); // Set trial period to 10 minutes
                Properties.Settings.Default.IsActive = true;
                Properties.Settings.Default.IsFirstOpen = false;
                Properties.Settings.Default.Save();

                MessageBox.Show("لقد بدأت النسخة التجريبية الخاصة بك وستنتهي في " + Properties.Settings.Default.EndDate.ToLongDateString());
                OpenNewForm(new basic());
            }
            else
            {
                // Check if the trial period has ended
                if (DateTime.Now > Properties.Settings.Default.EndDate)
                {
                    // Trial period has ended
                    MessageBox.Show("الرجاء ادخال كود النسخة الخاص بك");
                    Properties.Settings.Default.IsActive = false;
                    OpenNewForm(new Actives()); // Redirect to activation form
                }
                else
                {

                    // Check if the software is active
                    if (Properties.Settings.Default.IsActive)
                    {
                        OpenNewForm(new LoginForm());
                    }
                }
            }
        }
        private void OpenNewForm(Form newForm)
        {
            newForm.FormClosed += (s, args) => this.Close();
            newForm.Show();
            this.Hide();

        }
    }
}
