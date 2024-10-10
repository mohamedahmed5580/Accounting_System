using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Pharmacy.DL;

namespace Accounting_System
{
    public partial class GeneralLedger : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public GeneralLedger()
        {
            InitializeComponent();
            Timer1.Tick += new EventHandler(Timer1_Tick);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            dtpDateFrom.Value = DateTime.Today;
            dtpDateTo.Value = DateTime.Today;
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            Timer1.Enabled = false;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    Cursor = Cursors.WaitCursor;
                    Timer1.Enabled = true;
                    frmReport frmReport = new frmReport();

                    con.Open();
                    string ct = "select * from LedgerBook where Date >=@d2 and Date < @d3";
                    using (SqlCommand cmd = new SqlCommand(ct, con))
                    {
                        cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                        cmd.Parameters.Add("@d3", SqlDbType.DateTime).Value = dtpDateTo.Value.Date.AddDays(1);
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (!rdr.Read())
                            {
                                MessageBox.Show("عذرًا...لا يوجد سجلات", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                if (rdr != null)
                                {
                                    rdr.Close();
                                }
                                return;
                            }
                        }
                    }
                   
                }
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    frmReport frmReport = new frmReport();
                    con.Open();
                    string query = "Select Date, Name, LedgerNo, Label, Credit, Debit from LedgerBook where Date >=@d1 and Date < @d2 order by Date, LedgerNo";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                        cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date.AddDays(1);

                        using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                        {
                            DataTable dtable = new DataTable();
                            adp.Fill(dtable);

                            DataSet ds = new DataSet();
                            ds.Tables.Add(dtable);
                            ds.WriteXmlSchema("GeneralLedger.xml");

                            rptGeneralLedger rpt = new rptGeneralLedger();
                            rpt.SetDataSource(ds);
                            rpt.SetParameterValue("p1", dtpDateFrom.Value.Date);
                            rpt.SetParameterValue("p2", dtpDateTo.Value.Date);

                            frmReport.crystalReportViewer1.ReportSource = rpt;
                            frmReport.ShowDialog();
                        }
                    }
                   
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                Timer1.Enabled = false;
            }


        }
    }
}
