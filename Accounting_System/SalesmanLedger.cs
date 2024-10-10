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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace Accounting_System
{
    public partial class SalesmanLedger : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public SalesmanLedger()
        {
            InitializeComponent();
            fillSalesman();
            cmbSalesman.SelectedIndexChanged += new EventHandler(cmbSupplierName_SelectedIndexChanged);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            dtpDateFrom.Value = DateTime.Today ;
            dtpDateTo.Value = DateTime.Today;
            cmbSalesman.Text = "";
            txtSalesmanID.Text = "";
        }
        private void fillSalesman()
        {
            con.Open();
            SqlDataAdapter adp = new SqlDataAdapter("SELECT RTRIM(Name) FROM Salesman ORDER BY 1", con);
            DataSet ds = new DataSet();
            adp.Fill(ds);
            DataTable dtable = ds.Tables[0];
            cmbSalesman.Items.Clear();

            foreach (DataRow drow in dtable.Rows)
            {
                cmbSalesman.Items.Add(drow[0].ToString());
            }
            con.Close();

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    if (string.IsNullOrWhiteSpace(cmbSalesman.Text))
                    {
                        MessageBox.Show("الرجاء اختيار المندوب", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cmbSalesman.Focus();
                        return;
                    }
                    frmReport report_frm = new frmReport();

                    con.Open();
                    string ct = @"SELECT * FROM InvoiceInfo 
                      INNER JOIN SalesMan ON InvoiceInfo.SalesmanID = SalesMan.SM_ID 
                      INNER JOIN Salesman_Commission ON InvoiceInfo.Inv_ID = Salesman_Commission.InvoiceID 
                      WHERE InvoiceDate BETWEEN @d2 AND @d3 AND Salesman_ID = @d1";

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



                    rptSalesmanLedger3 rpt = new rptSalesmanLedger3(); // The report you created.
                    DataSet myDS = new DataSet(); // The DataSet you created.


                    using (SqlCommand MyCommand = new SqlCommand())
                    {
                        MyCommand.Connection = con;
                        MyCommand.CommandText = @"SELECT InvoiceInfo.Inv_ID, InvoiceInfo.InvoiceNo, InvoiceInfo.InvoiceDate, 
                                          InvoiceInfo.CustomerID, InvoiceInfo.SalesmanID, InvoiceInfo.GrandTotal, 
                                          InvoiceInfo.TotalPaid, InvoiceInfo.Balance, InvoiceInfo.Remarks, 
                                          SalesMan.SM_ID, SalesMan.SalesMan_ID, SalesMan.Name, SalesMan.Address, 
                                          SalesMan.City, SalesMan.State, SalesMan.ZipCode, SalesMan.ContactNo, 
                                          SalesMan.EmailID, SalesMan.Remarks AS Expr1, SalesMan.Photo, 
                                          SalesMan.CommissionPer, Salesman_Commission.ID, Salesman_Commission.InvoiceID, 
                                          Salesman_Commission.CommissionPer AS Expr2, Salesman_Commission.Commission 
                                          FROM InvoiceInfo 
                                          INNER JOIN SalesMan ON InvoiceInfo.SalesmanID = SalesMan.SM_ID 
                                          INNER JOIN Salesman_Commission ON InvoiceInfo.Inv_ID = Salesman_Commission.InvoiceID 
                                          WHERE InvoiceDate BETWEEN @d2 AND @d3 AND Salesman_ID = @d1 
                                          ORDER BY Inv_ID";

                        MyCommand.Parameters.AddWithValue("@d1", txtSalesmanID.Text);
                        MyCommand.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                        MyCommand.Parameters.Add("@d3", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;
                        MyCommand.CommandType = CommandType.Text;

                        using (SqlDataAdapter myDA = new SqlDataAdapter(MyCommand))
                        {
                            myDA.Fill(myDS, "InvoiceInfo");
                            myDA.Fill(myDS, "Salesman");
                            myDA.Fill(myDS, "Salesman_Commission");
                        }
                    }
                    

                    rpt.SetDataSource(myDS);
                    rpt.SetParameterValue("p1", dtpDateFrom.Value.Date);
                    rpt.SetParameterValue("p2", dtpDateTo.Value.Date);
                    report_frm.crystalReportViewer1.ReportSource = rpt;
                    report_frm.ShowDialog();
                    
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void cmbSupplierName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string a = "";
            string b = "";
            string c = "";
            txtSalesmanID.Text = "";

            
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

        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    if (string.IsNullOrWhiteSpace(cmbSalesman.Text))
                    {
                        MessageBox.Show("الرجاء اختيار المندوب", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cmbSalesman.Focus();
                        return;
                    }
                    frmReport report_frm = new frmReport();


                    con.Open();
                    string ct = "SELECT * FROM SalesManComession WHERE Date BETWEEN @d2 AND @d3 AND Salesman_ID = @d1";

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


                    rptSalesmanLedger_2 rpt = new rptSalesmanLedger_2(); // The report you created.
                    DataSet myDS = new DataSet(); // The DataSet you created.



                    using (SqlCommand MyCommand = new SqlCommand())
                    {
                        MyCommand.Connection = con;
                        MyCommand.CommandText = "SELECT * FROM SalesManComession WHERE Date BETWEEN @d2 AND @d3 AND Salesman_ID = @d1 ORDER BY TC_ID";
                        MyCommand.Parameters.AddWithValue("@d1", txtSalesmanID.Text);
                        MyCommand.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                        MyCommand.Parameters.Add("@d3", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;
                        MyCommand.CommandType = CommandType.Text;

                        using (SqlDataAdapter myDA = new SqlDataAdapter(MyCommand))
                        {
                            myDA.Fill(myDS, "SalesManComession");
                        }
                    }


                    rpt.SetDataSource(myDS);
                    rpt.SetParameterValue("p1", dtpDateFrom.Value.Date);
                    rpt.SetParameterValue("p2", dtpDateTo.Value.Date);
                    report_frm.crystalReportViewer1.ReportSource = rpt;
                    report_frm.ShowDialog();
                    
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void SalesmanLedger_Load(object sender, EventArgs e)
        {

        }
    }
}
