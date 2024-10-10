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
    public partial class SupplierLedger : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public SupplierLedger()
        {
            InitializeComponent();
            fillSupplier();
            cmbSupplierName.SelectedIndexChanged += new EventHandler(cmbSupplierName_SelectedIndexChanged);
        }

        private void SupplierLedger_Load(object sender, EventArgs e)
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
                    adp.SelectCommand = new SqlCommand("SELECT RTRIM(Name) FROM Supplier", con);
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
                        cmd.CommandText = "SELECT RTRIM(SupplierID), RTRIM(Address), RTRIM(City), RTRIM(ContactNo) FROM Supplier WHERE Name = @d1";
                        cmd.Parameters.AddWithValue("@d1", cmbSupplierName.Text);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                txtSupplierID.Text = rdr.GetValue(0).ToString();
                                a = rdr.GetValue(1).ToString();
                                b = rdr.GetValue(2).ToString();
                                c = rdr.GetValue(3).ToString();
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
            try
            {
                if (string.IsNullOrWhiteSpace(cmbSupplierName.Text))
                {
                    MessageBox.Show("الرجاء اختيار اسم المورد", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbSupplierName.Focus();
                    return;
                }

                string a = "";
                string b = "";
                string c = "";

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // Fetch the supplier details
                    string fetchSupplierQuery = "SELECT RTRIM(SupplierID), RTRIM(Address), RTRIM(City), RTRIM(ContactNo) FROM Supplier WHERE Name = @d1";
                    using (SqlCommand cmd = new SqlCommand(fetchSupplierQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", cmbSupplierName.Text);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                txtSupplierID.Text = rdr.IsDBNull(0) ? "" : rdr.GetString(0); // SupplierID
                                a = rdr.IsDBNull(1) ? "" : rdr.GetString(1); // Address
                                b = rdr.IsDBNull(2) ? "" : rdr.GetString(2); // City
                                c = rdr.IsDBNull(3) ? "" : rdr.GetString(3); // ContactNo
                            }
                            else
                            {
                                // Handle case where no data was returned
                                txtSupplierID.Text = "";
                                a = "";
                                b = "";
                                c = "";
                            }
                        }
                    }

                    // Check if there are records
                    string checkQuery = "SELECT PartyID FROM SupplierLedgerBook WHERE PartyID = @d1 AND Date >= @d2 AND Date < @d3";
                    using (SqlCommand cmd = new SqlCommand(checkQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtSupplierID.Text);
                        cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                        cmd.Parameters.Add("@d3", SqlDbType.DateTime).Value = dtpDateTo.Value.Date.AddDays(1);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (!rdr.Read())
                            {
                                MessageBox.Show("عذرًا...لا يوجد سجلات", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }
                    }

                    // Fetch the records for the report
                    string fetchQuery = "SELECT Date, Name, LedgerNo, Label, Credit, Debit FROM SupplierLedgerBook WHERE Date >= @d1 AND Date < @d2 AND PartyID = @d3 ORDER BY Date, LedgerNo";
                    using (SqlCommand cmd = new SqlCommand(fetchQuery, con))
                    {
                        cmd.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                        cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date.AddDays(1);
                        cmd.Parameters.AddWithValue("@d3", txtSupplierID.Text);

                        using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                        {
                            DataTable dtable = new DataTable();
                            adp.Fill(dtable);

                            DataSet ds = new DataSet();
                            ds.Tables.Add(dtable);
                            ds.WriteXmlSchema("SupplierLedger.xml");

                            rptSupplierLedger rpt = new rptSupplierLedger();
                            rpt.SetDataSource(ds);
                            rpt.SetParameterValue("p1", dtpDateFrom.Value.Date);
                            rpt.SetParameterValue("p2", dtpDateTo.Value.Date);
                            rpt.SetParameterValue("p3", txtSupplierID.Text);
                            rpt.SetParameterValue("p4", cmbSupplierName.Text);
                            rpt.SetParameterValue("p5", a); // Address
                            rpt.SetParameterValue("p6", b); // City
                            rpt.SetParameterValue("p7", c); // ContactNo

                            frmReport frmReport = new frmReport();
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


        }

        private void GroupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}
