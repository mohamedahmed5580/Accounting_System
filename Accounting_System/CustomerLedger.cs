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
    public partial class CustomerLedger : Form
    {
        public static CustomerLedger _instance;
        public static CustomerLedger instance;
        public static CustomerLedger Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new CustomerLedger();
                }
                return _instance;
            }

        }
        String a = "";
        String b = "";
        String c = "";
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public CustomerLedger()
        {
            InitializeComponent();
            fillCustomer();
            cmbCustomerName.SelectedIndexChanged += new EventHandler(cmbSupplierName_SelectedIndexChanged);
            instance = this;
        }

        private void GroupBox2_Enter(object sender, EventArgs e)
        {

        }
        private void fillCustomer()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlDataAdapter adp = new SqlDataAdapter())
                    {
                        adp.SelectCommand = new SqlCommand("SELECT RTRIM(Name) FROM Customer WHERE CustomerType='Regular'", con);
                        DataSet ds = new DataSet("ds");
                        adp.Fill(ds);
                        DataTable dtable = ds.Tables[0];
                        cmbCustomerName.Items.Clear();
                        foreach (DataRow drow in dtable.Rows)
                        {
                            cmbCustomerName.Items.Add(drow[0].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Reset()
        {
            dtpDateFrom.Value = DateTime.Today;
            dtpDateTo.Value = DateTime.Today;
            cmbCustomerName.Text = string.Empty;
            txtCustomerID.Text = string.Empty;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cmbCustomerName.Text))
                {
                    MessageBox.Show("الرجاء اختيار اسم العميل", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbCustomerName.Focus();
                    return;
                }

                string a = "";
                string b = "";
                string c = "";

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // Check if there are records for the customer
                    string ct = "SELECT PartyID FROM LedgerBook WHERE PartyID = @d1 AND Date >= @d2 AND Date < @d3";
                    using (SqlCommand cmd = new SqlCommand(ct, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtCustomerID.Text);
                        cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                        cmd.Parameters.Add("@d3", SqlDbType.DateTime).Value = dtpDateTo.Value.Date.AddDays(1);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (!rdr.Read())
                            {
                                MessageBox.Show("عذراً...لا يوجد سجلات", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }
                    }

                    // Populate a, b, and c with data from Customer table
                    string customerQuery = "SELECT RTRIM(Address), RTRIM(City), RTRIM(ContactNo) FROM Customer WHERE CustomerID = @d1";
                    using (SqlCommand cmd = new SqlCommand(customerQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtCustomerID.Text);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                a = rdr.GetString(0);
                                b = rdr.GetString(1);
                                c = rdr.GetString(2);
                            }
                        }
                    }


                    // Retrieve data for the report
                    string query1 = "SELECT Date, Name, LedgerNo, Label, Credit, Debit, Manual_Inv FROM LedgerBook WHERE Date >= @d1 AND Date < @d2 AND PartyID = @d3 ORDER BY Date, LedgerNo ASC";
                    using (SqlCommand cmd = new SqlCommand(query1, con))
                    {
                        cmd.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                        cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date.AddDays(1);
                        cmd.Parameters.AddWithValue("@d3", txtCustomerID.Text);

                        using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                        {
                            DataTable dtable = new DataTable();
                            adp.Fill(dtable);

                            // Retrieve company data for the report
                            string query2 = "SELECT * FROM Company";
                            using (SqlCommand cmd2 = new SqlCommand(query2, con))
                            {
                                using (SqlDataAdapter adp2 = new SqlDataAdapter(cmd2))
                                {
                                    DataTable dtable2 = new DataTable();
                                    adp2.Fill(dtable2);

                                    DataSet ds = new DataSet();
                                    ds.Tables.Add(dtable);
                                    ds.Tables.Add(dtable2);
                                    ds.WriteXmlSchema("CustomerLedger.xml");

                                    rptCustomerLedger rpt = new rptCustomerLedger();
                                    rpt.SetDataSource(ds);
                                    rpt.SetParameterValue("p1", dtpDateFrom.Value.Date);
                                    rpt.SetParameterValue("p2", dtpDateTo.Value.Date);
                                    rpt.SetParameterValue("p3", txtCustomerID.Text);
                                    rpt.SetParameterValue("p4", cmbCustomerName.Text);
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Button2_Click(object sender, EventArgs e)
        {

            this.Close();

            CustomerRecord5 frmCustomerRecord5 = new CustomerRecord5();


            frmCustomerRecord5.lblSet.Text = "";


            frmCustomerRecord5.Reset();


            frmCustomerRecord5.ShowDialog();

        }
        private void cmbSupplierName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string a = "";
                string b = "";
                string c = "";
                txtCustomerID.Text = "";

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT RTRIM(CustomerID),RTRIM(Address),RTRIM(City),RTRIM(ContactNo) FROM Customer WHERE Name=@d1";
                        cmd.Parameters.AddWithValue("@d1", cmbCustomerName.Text);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                txtCustomerID.Text = rdr.GetString(0);
                                a = rdr.GetString(1);
                                b = rdr.GetString(2);
                                c = rdr.GetString(3);
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
    }
}
