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
    public partial class SupplierCompanyLedger : Form
    {
        public SupplierCompanyLedger()
        {
            InitializeComponent();
            fillSupplier();
            cmbSupplierName.SelectedIndexChanged += new EventHandler(cmbSupplierName_SelectedIndexChanged);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }
        public void Reset()
        {
            dtpDateFrom.Text = DateTime.Today.ToString("d");
            dtpDateTo.Text = DateTime.Today.ToString("d");
            cmbSupplierName.Text = string.Empty;
            txtSupplierID.Text = string.Empty;
        }

        private void GroupBox2_Enter(object sender, EventArgs e)
        {

        }
        private void fillSupplier()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    SqlDataAdapter adp = new SqlDataAdapter();
                    adp.SelectCommand = new SqlCommand("SELECT RTRIM(ShappingName) FROM [ShippingCom]", con);
                    DataSet ds = new DataSet("ds");
                    adp.Fill(ds);
                    DataTable dtable = ds.Tables[0];
                    cmbSupplierName.Items.Clear();
                    foreach (DataRow drow in dtable.Rows)
                    {
                        cmbSupplierName.Items.Add(drow[0].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void cmbSupplierName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string a = string.Empty;
                string b = string.Empty;
                string c = string.Empty;
                txtSupplierID.Text = string.Empty;

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT RTRIM(ShappingCode), RTRIM(Location), RTRIM([Email]) FROM ShippingCom WHERE ShappingName = @d1";
                        cmd.Parameters.AddWithValue("@d1", cmbSupplierName.Text);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                txtSupplierID.Text = rdr.GetValue(0).ToString();
                                a = rdr.GetValue(1).ToString();
                                b = rdr.GetValue(2).ToString();
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

        private void Button1_Click(object sender, EventArgs e)
        {

                if (string.IsNullOrWhiteSpace(cmbSupplierName.Text))
                {
                    MessageBox.Show("الرجاء اختيار اسم شركة الشحن", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbSupplierName.Focus();
                    return;
                }

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // Single query to join Stock and CompanyLedgerBook on ST_ID
                    string query = @"
            SELECT distinct 
                clb.Id, clb.Date , clb.Name, clb.LedgerNo, clb.Label, clb.Debit, clb.Credit, clb.PartyID, clb.ST_ID,
                s.ST_ID AS StockID, s.InvoiceNo, s.Date , s.PurchaseType, s.TotalPayment, s.CurrencieName, s.Total_By_Pound
            FROM
                CompanyLedgerBook clb
            INNER JOIN 
                Stock s ON clb.ST_ID = s.ST_ID
            WHERE 
                clb.Date >= @DateFrom AND clb.Date < @DateTo AND clb.PartyID = @PartyID
            ORDER BY 
                clb.Date";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@DateFrom", dtpDateFrom.Value.Date);
                        cmd.Parameters.AddWithValue("@DateTo", dtpDateTo.Value.Date.AddDays(1));
                        cmd.Parameters.AddWithValue("@PartyID", txtSupplierID.Text);

                        using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet("Company_Ledger1");
                            adp.Fill(ds, "CompanyLedgerBook");
                            adp.Fill(ds, "Stock");

                            // Split data into CompanyLedgerBook and Stock tables




                            rptCompanyLedger rpt = new rptCompanyLedger();
                            rpt.SetDataSource(ds);

                            rpt.SetParameterValue("p1", dtpDateFrom.Value.Date);
                            rpt.SetParameterValue("p2", dtpDateTo.Value.Date);
                            rpt.SetParameterValue("p3", txtSupplierID.Text);
                            rpt.SetParameterValue("p4", cmbSupplierName.Text);

                            frmReport frmReport = new frmReport();
                            frmReport.crystalReportViewer1.ReportSource = rpt;
                            frmReport.ShowDialog();
                        }
                    }
                }



        }
    }
}
