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
    public partial class PurshaseDaybook : Form
    {
        SqlConnection SqlConnection= new SqlConnection(DataAccessLayer.Con());
        public PurshaseDaybook()
        {
            InitializeComponent();
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
       public void Reset()
       {
            dtpDateFrom.Text = DateTime.Today.ToString();
            dtpDateTo.Text = DateTime.Today.ToString();
       }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {


                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT ST_ID, Date, InvoiceNo, Name, SubTotal, Discount, FreightCharges, OtherCharges, PreviousDue, GrandTotal FROM Supplier, Stock WHERE Supplier.ID = Stock.SupplierID AND PurchaseType = 'Credit' ORDER BY [Date]", con);
                    cmd.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                    cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    DataTable dtable = new DataTable();
                    adp.Fill(dtable);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dtable);
                    ds.WriteXmlSchema("PurchaseDayBook.xml");

                    rptPurchaseDayBook rpt = new rptPurchaseDayBook();
                    rpt.SetDataSource(ds);
                    rpt.SetParameterValue("p1", dtpDateFrom.Value.Date);
                    rpt.SetParameterValue("p2", dtpDateTo.Value.Date);

                    frmReport reportForm = new frmReport();
                    reportForm.crystalReportViewer1.ReportSource = rpt;
                    reportForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
    }
}
