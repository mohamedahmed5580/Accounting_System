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
    public partial class DebtorsReport : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public DebtorsReport()
        {
            InitializeComponent();
            fillcity();
        }
        private void fillcity()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    SqlDataAdapter adp = new SqlDataAdapter();
                    adp.SelectCommand = new SqlCommand("SELECT DISTINCT RTRIM(City) FROM Supplier UNION SELECT DISTINCT RTRIM(City) FROM Customer", con);

                    DataSet ds = new DataSet("ds");
                    adp.Fill(ds);
                    DataTable dtable = ds.Tables[0];

                    cmbCity.Items.Clear();
                    foreach (DataRow drow in dtable.Rows)
                    {
                        cmbCity.Items.Add(drow[0].ToString());
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

        private void btnDebtors_Click(object sender, EventArgs e)
        {


            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                // SqlCommand cmd = new SqlCommand("Select CustomerID,Name,City,ContactNo,Sum(Balance) as [Balance] From (Select Customer.CustomerID,Customer.Name,Customer.City,Customer.ContactNo,Sum(Balance) as [Balance] from Customer,InvoiceInfo where Customer.ID=InvoiceInfo.CustomerID group by Customer.CustomerID,Customer.Name,Customer.City,Customer.ContactNo having (Sum(Balance) > 0)  Union All Select Customer.CustomerID,Customer.Name,Customer.City,Customer.ContactNo,Sum(Balance) as [Balance] from Customer,InvoiceInfo1,Service where Customer.ID=Service.CustomerID and Service.S_ID=InvoiceInfo1.ServiceID group by Customer.CustomerID,Customer.Name,Customer.City,Customer.ContactNo having (Sum(Balance) > 0)) As Customer Group By CustomerID,Name,City,ContactNo Order by Name", con);
                SqlCommand cmd = new SqlCommand("Select * from Debitors", con);

                // SqlCommand cmd = new SqlCommand("Select CustomerID,Name,Sum(Debit) as [SDebit],Sum(Credit) as [SCredit],PartyID From (Select Customer.CustomerID,Customer.Name,Sum(Debit) as [SDebit],Sum(Credit) as [SCredit] from Customer,LedgerBook where Customer.CustomerID=LedgerBook.PartyID ", con);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dtable = new DataTable();
                adp.Fill(dtable);
                con.Close();

                DataSet ds = new DataSet();
                ds.Tables.Add(dtable);
                ds.WriteXmlSchema("Debtors.xml");

                rptDebtors rpt = new rptDebtors();
                rpt.SetDataSource(ds);
                rpt.SetParameterValue("p1", DateTime.Today);

                frmReport frm = new frmReport();
                frm.crystalReportViewer1.ReportSource = rpt;
                frm.ShowDialog();
            }

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            cmbCity.Text = "";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cmbCity.Text))
                {
                    MessageBox.Show("الرجاء اختيار المدينة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbCity.Focus();
                    return;
                }


                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("Select CustomerID, Name, City, ContactNo, Sum(Balance) As [Balance] From (Select Customer.CustomerID, Customer.Name, Customer.City, Customer.ContactNo, Sum(Balance) As [Balance] from Customer, InvoiceInfo where Customer.ID=InvoiceInfo.CustomerID And City=@d1 group by Customer.CustomerID, Customer.Name, Customer.City, Customer.ContactNo having (Sum(Balance) > 0)  Union All Select Customer.CustomerID,Customer.Name,Customer.City,Customer.ContactNo,Sum(Balance) As [Balance] from Customer,InvoiceInfo1,Service where Customer.ID=Service.CustomerID And Service.S_ID=InvoiceInfo1.ServiceID group by Customer.CustomerID,Customer.Name,Customer.City,Customer.ContactNo having (Sum(Balance) > 0)) As Customer Group By CustomerID,Name,City,ContactNo order by Name", con);

                    cmd.Parameters.AddWithValue("@d1", cmbCity.Text);

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    DataTable dtable = new DataTable();
                    adp.Fill(dtable);
                    con.Close();

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dtable);
                    ds.WriteXmlSchema("Debtors.xml");

                    rptDebtors rpt = new rptDebtors();
                    rpt.SetDataSource(ds);
                    rpt.SetParameterValue("p1", DateTime.Today);

                    frmReport frm = new frmReport();
                    frm.crystalReportViewer1.ReportSource = rpt;
                    frm.ShowDialog();
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
    }
}
