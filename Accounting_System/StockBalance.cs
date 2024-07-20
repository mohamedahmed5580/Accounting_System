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
    public partial class StockBalance : Form
    {
        public StockBalance()
        {
            InitializeComponent();
        }

        private void SlideButton1_Click(object sender, EventArgs e)
        {
            if (SlideButton1.IsOn)
            {
                TextBox2.Visible = true;
                Label2.Visible = true;
                Label3.Visible = true;
                Label5.Visible = true;
                TextBox3.Visible = true;
                TextBox4.Visible = true;
            }
            else
            {
                TextBox2.Visible = false;
                Label2.Visible = false;
                Label3.Visible = false;
                Label5.Visible = false;
                TextBox3.Visible = false;
                TextBox4.Visible = false;

            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            
        }
    }
}
