using Pharmacy.PL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Accounting_System
{
    public partial class loading : Form
    {
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
                    OpenNewForm(new basic());
                }
            }
            catch
            {
                return;
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
