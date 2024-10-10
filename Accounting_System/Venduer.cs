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
    public partial class Venduer : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public Venduer()
        {
            InitializeComponent();
            fillSalesman();
            cmbSalesman.SelectedIndexChanged += new EventHandler(cmbSupplierName_SelectedIndexChanged);
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void fillSalesman()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlDataAdapter adp = new SqlDataAdapter("SELECT RTRIM(Name) FROM Salesman ORDER BY 1", con))
                    {
                        DataSet ds = new DataSet();
                        adp.Fill(ds);
                        DataTable dtable = ds.Tables[0];

                        cmbSalesman.Items.Clear();
                        foreach (DataRow drow in dtable.Rows)
                        {
                            cmbSalesman.Items.Add(drow[0].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void Reset()
        {
            dtpDateFrom.Value = DateTime.Today;
            dtpDateTo.Value = DateTime.Today;
            cmbSalesman.Text = string.Empty;
            txtSalesmanID.Text = string.Empty;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }
        private void cmbSupplierName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string a = string.Empty;
                string b = string.Empty;
                string c = string.Empty;
                txtSalesmanID.Text = string.Empty;

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT RTRIM(Salesman_ID), RTRIM(Address), RTRIM(City), RTRIM(ContactNo) FROM Salesman WHERE Name = @d1";
                        cmd.Parameters.AddWithValue("@d1", cmbSalesman.Text);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                txtSalesmanID.Text = rdr.GetValue(0).ToString();
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
                if (string.IsNullOrWhiteSpace(cmbSalesman.Text))
                {
                    MessageBox.Show("الرجاء اختيار المندوب", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbSalesman.Focus();
                    return;
                }

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string ct = "SELECT * FROM InvoiceInfo " +
                                "INNER JOIN SalesMan ON InvoiceInfo.SalesmanID = SalesMan.SM_ID " +
                                "INNER JOIN Salesman_Commission ON InvoiceInfo.Inv_ID = Salesman_Commission.InvoiceID " +
                                "WHERE InvoiceDate BETWEEN @d2 AND @d3 AND Salesman_ID = @d1";
                    using (SqlCommand cmd = new SqlCommand(ct, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtSalesmanID.Text);
                        cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                        cmd.Parameters.Add("@d3", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (!rdr.Read())
                            {
                                MessageBox.Show("عفوا...لا يوجد سجلات", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }
                    }


                    rptSalesmanLedger rpt = new rptSalesmanLedger(); // The report you created.
                    SqlConnection myConnection = new SqlConnection(DataAccessLayer.Con());
                    SqlCommand MyCommand = new SqlCommand();
                    SqlDataAdapter myDA = new SqlDataAdapter();
                    DataSet myDS = new DataSet(); // The DataSet you created.

                    MyCommand.Connection = myConnection;
                    MyCommand.CommandText = "SELECT InvoiceInfo.Inv_ID, InvoiceInfo.InvoiceNo, InvoiceInfo.InvoiceDate, InvoiceInfo.CustomerID, InvoiceInfo.SalesmanID, " +
                                            "InvoiceInfo.GrandTotal, InvoiceInfo.TotalPaid, InvoiceInfo.Balance, InvoiceInfo.Remarks, SalesMan.SM_ID, SalesMan.SalesMan_ID, " +
                                            "SalesMan.Name, SalesMan.Address, SalesMan.City, SalesMan.State, SalesMan.ZipCode, SalesMan.ContactNo, SalesMan.EmailID, " +
                                            "SalesMan.Remarks AS Expr1, SalesMan.Photo, SalesMan.CommissionPer, Salesman_Commission.ID, Salesman_Commission.InvoiceID, " +
                                            "Salesman_Commission.CommissionPer AS Expr2, Salesman_Commission.Commission " +
                                            "FROM InvoiceInfo " +
                                            "INNER JOIN SalesMan ON InvoiceInfo.SalesmanID = SalesMan.SM_ID " +
                                            "INNER JOIN Salesman_Commission ON InvoiceInfo.Inv_ID = Salesman_Commission.InvoiceID " +
                                            "WHERE InvoiceDate BETWEEN @d2 AND @d3 AND Salesman_ID = @d1 ORDER BY Inv_ID";
                    MyCommand.Parameters.AddWithValue("@d1", txtSalesmanID.Text);
                    MyCommand.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                    MyCommand.Parameters.Add("@d3", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;
                    MyCommand.CommandType = CommandType.Text;

                    myDA.SelectCommand = MyCommand;
                    myDA.Fill(myDS, "InvoiceInfo");
                    myDA.Fill(myDS, "Salesman");
                    myDA.Fill(myDS, "Salesman_Commission");

                    rpt.SetDataSource(myDS);
                    rpt.SetParameterValue("p1", dtpDateFrom.Value.Date);
                    rpt.SetParameterValue("p2", dtpDateTo.Value.Date);

                    frmReport frmReport = new frmReport();
                    frmReport.crystalReportViewer1.ReportSource = rpt;
                    frmReport.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Venduer_Load(object sender, EventArgs e)
        {

        }
    }
}
