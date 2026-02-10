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

        private void fillSalesman()
        {
            try
            {
                string query = "SELECT RTRIM(Name) FROM Salesman ORDER BY 1";
                DataTable dtable = DataAccessLayer.ExecuteTable(query, CommandType.Text);
                cmbSalesman.Items.Clear();
                foreach (DataRow drow in dtable.Rows)
                {
                    cmbSalesman.Items.Add(drow[0].ToString());
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
                txtSalesmanID.Text = string.Empty;
                string query = "SELECT RTRIM(Salesman_ID), RTRIM(Address), RTRIM(City), RTRIM(ContactNo) FROM Salesman WHERE Name = @d1";
                SqlParameter[] parameters = { new SqlParameter("@d1", cmbSalesman.Text) };

                using (SqlDataReader rdr = DataAccessLayer.ExecuteReader(query, CommandType.Text, parameters))
                {
                    if (rdr.Read())
                    {
                        txtSalesmanID.Text = rdr.GetValue(0).ToString();
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
            Button1.Enabled = false;
            try
            {
                if (string.IsNullOrWhiteSpace(cmbSalesman.Text))
                {
                    MessageBox.Show("الرجاء اختيار المندوب", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbSalesman.Focus();
                    return;
                }

                string queryCheck = "SELECT * FROM InvoiceInfo " +
                                  "INNER JOIN SalesMan ON InvoiceInfo.SalesmanID = SalesMan.SM_ID " +
                                  "INNER JOIN Salesman_Commission ON InvoiceInfo.Inv_ID = Salesman_Commission.InvoiceID " +
                                  "WHERE InvoiceDate BETWEEN @d2 AND @d3 AND Salesman_ID = @d1";

                SqlParameter[] parameters = {
                    new SqlParameter("@d1", txtSalesmanID.Text),
                    new SqlParameter("@d2", dtpDateFrom.Value.Date),
                    new SqlParameter("@d3", dtpDateTo.Value.Date)
                };

                using (SqlDataReader rdr = DataAccessLayer.ExecuteReader(queryCheck, CommandType.Text, parameters))
                {
                    if (!rdr.Read())
                    {
                        MessageBox.Show("عفوا...لا يوجد سجلات", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                rptSalesmanLedger rpt = new rptSalesmanLedger();
                DataSet myDS = new DataSet();

                string queryReport = "SELECT InvoiceInfo.Inv_ID, InvoiceInfo.InvoiceNo, InvoiceInfo.InvoiceDate, InvoiceInfo.CustomerID, InvoiceInfo.SalesmanID, " +
                                    "InvoiceInfo.GrandTotal, InvoiceInfo.TotalPaid, InvoiceInfo.Balance, InvoiceInfo.Remarks, SalesMan.SM_ID, SalesMan.SalesMan_ID, " +
                                    "SalesMan.Name, SalesMan.Address, SalesMan.City, SalesMan.State, SalesMan.ZipCode, SalesMan.ContactNo, SalesMan.EmailID, " +
                                    "SalesMan.Remarks AS Expr1, SalesMan.Photo, SalesMan.CommissionPer, Salesman_Commission.ID, Salesman_Commission.InvoiceID, " +
                                    "Salesman_Commission.CommissionPer AS Expr2, Salesman_Commission.Commission " +
                                    "FROM InvoiceInfo " +
                                    "INNER JOIN SalesMan ON InvoiceInfo.SalesmanID = SalesMan.SM_ID " +
                                    "INNER JOIN Salesman_Commission ON InvoiceInfo.Inv_ID = Salesman_Commission.InvoiceID " +
                                    "WHERE InvoiceDate BETWEEN @d2 AND @d3 AND Salesman_ID = @d1 ORDER BY Inv_ID";

                SqlParameter[] reportParams = {
                    new SqlParameter("@d1", txtSalesmanID.Text),
                    new SqlParameter("@d2", dtpDateFrom.Value.Date),
                    new SqlParameter("@d3", dtpDateTo.Value.Date)
                };

                // Re-using same connection or letting FillDataSet handle it
                using (SqlConnection connection = new SqlConnection(DataAccessLayer.Con()))
                {
                    using (SqlCommand cmd = new SqlCommand(queryReport, connection))
                    {
                        cmd.Parameters.AddRange(reportParams);
                        SqlDataAdapter myDA = new SqlDataAdapter(cmd);
                        myDA.Fill(myDS, "InvoiceInfo");
                        myDA.Fill(myDS, "Salesman");
                        myDA.Fill(myDS, "Salesman_Commission");
                    }
                }

                rpt.SetDataSource(myDS);
                rpt.SetParameterValue("p1", dtpDateFrom.Value.Date);
                rpt.SetParameterValue("p2", dtpDateTo.Value.Date);

                frmReport frmReport = new frmReport();
                frmReport.crystalReportViewer1.ReportSource = rpt;
                frmReport.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Button1.Enabled = true;
            }
        }

        private void Venduer_Load(object sender, EventArgs e)
        {
        }
    }
}
