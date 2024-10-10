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
    public partial class VoucherReport : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public VoucherReport()
        {
            InitializeComponent();
            fillVoucherNo();
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void fillVoucherNo()
        {
            try
            {
                using (SqlConnection CN = new SqlConnection(DataAccessLayer.Con()))
                {
                    CN.Open();
                    SqlDataAdapter adp = new SqlDataAdapter();
                    adp.SelectCommand = new SqlCommand("SELECT DISTINCT RTRIM(VoucherNo) FROM Voucher", CN);
                    DataSet ds = new DataSet("ds");
                    adp.Fill(ds);
                    DataTable dtable = ds.Tables[0];
                    cmbVoucherNo.Items.Clear();
                    foreach (DataRow drow in dtable.Rows)
                    {
                        cmbVoucherNo.Items.Add(drow[0].ToString());
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
            cmbVoucherNo.Text = string.Empty;
            TextBox1.Text = string.Empty;
            dtpDateFrom.Text = DateTime.Today.ToString();
            dtpDateTo.Text = DateTime.Today.ToString();
            fillVoucherNo();
            // FillSalesman();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbVoucherNo.Text))
            {
                MessageBox.Show("الرجاء اختيار رقم السند.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbVoucherNo.Focus();
                return;
            }

            try
            {


                rptVoucher rpt = new rptVoucher(); // The report you created.
                using (SqlConnection myConnection = new SqlConnection(DataAccessLayer.Con()))
                {
                    SqlCommand MyCommand = new SqlCommand();
                    SqlCommand MyCommand1 = new SqlCommand();
                    SqlDataAdapter myDA = new SqlDataAdapter();
                    SqlDataAdapter myDA1 = new SqlDataAdapter();
                    DataSet myDS = new DataSet(); // The DataSet you created.

                    MyCommand.Connection = myConnection;
                    MyCommand1.Connection = myConnection;

                    MyCommand.CommandText = "SELECT Voucher.ID, Voucher.VoucherNo, Voucher.Date, Voucher.Name, Voucher.Details, Voucher.GrandTotal, Voucher_OtherDetails.VD_ID, Voucher_OtherDetails.VoucherID, Voucher_OtherDetails.Particulars, Voucher_OtherDetails.Amount, Voucher_OtherDetails.Note FROM Voucher INNER JOIN Voucher_OtherDetails ON Voucher.ID = Voucher_OtherDetails.VoucherID WHERE VoucherNo = @VoucherNo";
                    MyCommand1.CommandText = "SELECT * FROM Company";

                    MyCommand.CommandType = CommandType.Text;
                    MyCommand1.CommandType = CommandType.Text;

                    MyCommand.Parameters.AddWithValue("@VoucherNo", cmbVoucherNo.Text);

                    myDA.SelectCommand = MyCommand;
                    myDA1.SelectCommand = MyCommand1;

                    myConnection.Open();
                    myDA.Fill(myDS, "Voucher");
                    myDA.Fill(myDS, "Voucher_OtherDetails");
                    myDA1.Fill(myDS, "Company");

                    rpt.SetDataSource(myDS);

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

        private void Button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox1.Text))
            {
                MessageBox.Show("الرجاء اختيار المندوب.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbVoucherNo.Focus();
                return;
            }

            try
            {


                rptVoucher2 rpt = new rptVoucher2(); // The report you created.
                using (SqlConnection myConnection = new SqlConnection(DataAccessLayer.Con()))
                {
                    SqlCommand MyCommand = new SqlCommand();
                    SqlCommand MyCommand1 = new SqlCommand();
                    SqlDataAdapter myDA = new SqlDataAdapter();
                    SqlDataAdapter myDA1 = new SqlDataAdapter();
                    DataSet myDS = new DataSet(); // The DataSet you created.

                    MyCommand.Connection = myConnection;
                    MyCommand1.Connection = myConnection;

                    MyCommand.CommandText = "SELECT Voucher.ID, Voucher.VoucherNo, Voucher.Date, Voucher.Name, Voucher.Details, Voucher.GrandTotal, Voucher_OtherDetails.VD_ID, Voucher_OtherDetails.VoucherID, Voucher_OtherDetails.Particulars, Voucher_OtherDetails.Amount, Voucher_OtherDetails.Note FROM Voucher INNER JOIN Voucher_OtherDetails ON Voucher.ID = Voucher_OtherDetails.VoucherID WHERE Name = @Name";
                    MyCommand1.CommandText = "SELECT * FROM Company";

                    MyCommand.CommandType = CommandType.Text;
                    MyCommand1.CommandType = CommandType.Text;

                    MyCommand.Parameters.AddWithValue("@Name", TextBox1.Text);

                    myDA.SelectCommand = MyCommand;
                    myDA1.SelectCommand = MyCommand1;

                    myConnection.Open();
                    myDA.Fill(myDS, "Voucher");
                    myDA.Fill(myDS, "Voucher_OtherDetails");
                    myDA1.Fill(myDS, "Company");

                    rpt.SetDataSource(myDS);

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

        private void btnGetData_Click(object sender, EventArgs e)
        {
            try
            {


                rptExpenses rpt = new rptExpenses(); // The report you created.
                using (SqlConnection myConnection = new SqlConnection(DataAccessLayer.Con()))
                {
                    SqlCommand MyCommand = new SqlCommand();
                    SqlDataAdapter myDA = new SqlDataAdapter();
                    DataSet myDS = new DataSet(); // The DataSet you created.

                    MyCommand.Connection = myConnection;
                    MyCommand.CommandText = "SELECT Voucher.ID, Voucher.VoucherNo, Voucher.Date, Voucher.Name, Voucher.Details, Voucher.GrandTotal, Voucher_OtherDetails.VD_ID, Voucher_OtherDetails.VoucherID, Voucher_OtherDetails.Particulars, Voucher_OtherDetails.Amount, Voucher_OtherDetails.Note FROM Voucher INNER JOIN Voucher_OtherDetails ON Voucher.ID = Voucher_OtherDetails.VoucherID WHERE Date BETWEEN @d1 AND @d2 ORDER BY Date";
                    MyCommand.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                    MyCommand.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;
                    MyCommand.CommandType = CommandType.Text;

                    myDA.SelectCommand = MyCommand;
                    myDA.Fill(myDS, "Voucher");
                    myDA.Fill(myDS, "Voucher_OtherDetails");

                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();
                        string ct = "SELECT ISNULL(SUM(GrandTotal), 0) FROM Voucher WHERE Date BETWEEN @d1 AND @d2";
                        SqlCommand cmd = new SqlCommand(ct, con);
                        cmd.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                        cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;

                        SqlDataReader rdr = cmd.ExecuteReader();
                        decimal a = 0;
                        if (rdr.Read())
                        {
                            a = rdr.GetDecimal(0);
                        }

                        rpt.SetDataSource(myDS);
                        rpt.SetParameterValue("p1", dtpDateFrom.Value.Date);
                        rpt.SetParameterValue("p2", dtpDateTo.Value.Date);
                        rpt.SetParameterValue("p3", a);
                        rpt.SetParameterValue("p4", DateTime.Today);

                        frmReport reportForm = new frmReport();
                        reportForm.crystalReportViewer1.ReportSource = rpt;
                        reportForm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
             this.Close();
             SalesmanRecord frmSalesmanRecord = new SalesmanRecord();
             frmSalesmanRecord.lblSet.Text = "voucher1";
             frmSalesmanRecord.Reset();
             frmSalesmanRecord.ShowDialog();
        }
    }
}
