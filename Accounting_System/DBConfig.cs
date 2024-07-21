using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace Pharmacy.PL
{
    public partial class DBConfig : MetroFramework.Forms.MetroForm
    {
        public DBConfig()
        {
            InitializeComponent();
            // Example of accessing a setting called "ConnectionString" in Properties.Settings

            if (Accounting_System.Properties.Settings.Default.Mode == true)
                rbWindows.Checked = true;
            else
                rdSQL.Checked = true;
               
            tbServer.Text= Accounting_System.Properties.Settings.Default.Server;
            tbDb.Text= Accounting_System.Properties.Settings.Default.Database;
            tbUser.Text = Accounting_System.Properties.Settings.Default.Name;
            tbPass.Text = Accounting_System.Properties.Settings.Default.Pass;
        }

        private void rbWindows_CheckedChanged(object sender, EventArgs e)
        {
            if (rbWindows.Checked == true)
                tbUser.Enabled = tbPass.Enabled = false;
            else
                tbUser.Enabled = tbPass.Enabled = true;

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            Accounting_System.Properties.Settings.Default.Mode = (rbWindows.Checked == true)? true: false;
            Accounting_System.Properties.Settings.Default.Server = tbServer.Text;
            Accounting_System.Properties.Settings.Default.Database = tbDb.Text;
            Accounting_System.Properties.Settings.Default.Name = tbUser.Text;
            Accounting_System.Properties.Settings.Default.Pass = tbPass.Text;
            Accounting_System.Properties.Settings.Default.Save();

            MetroFramework.MetroMessageBox.Show(this, "", "Success", MessageBoxButtons.OK, MessageBoxIcon.Question);
            this.Close();

        }

        private void rdSQL_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void DBConfig_Load(object sender, EventArgs e)
        {

        }

        private void tbServer_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
