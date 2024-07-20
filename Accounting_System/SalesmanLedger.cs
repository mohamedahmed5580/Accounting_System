using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace Accounting_System
{
    public partial class SalesmanLedger : Form
    {
        public SalesmanLedger()
        {
            InitializeComponent();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            dtpDateFrom.Value = DateTime.Today ;
            dtpDateTo.Value = DateTime.Today;
            cmbSalesman.Text = "";
            txtSalesmanID.Text = "";
        }
    }
}
