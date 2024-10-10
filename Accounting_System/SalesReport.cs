using Microsoft.Office.Interop.Excel;
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
using System.Data;


namespace Accounting_System
{
    public partial class SalesReport : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());

        public SalesReport()
        {
            InitializeComponent();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            dtpDateFrom.Value = DateTime.Today;
            dtpDateTo.Value = DateTime.Today;
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            frmReport frmReport = new frmReport();
            using (con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                string ctn = "select InvoiceNo from InvoiceInfo where InvoiceDate between @d1 and @d2";
                using (SqlCommand cmd = new SqlCommand(ctn, con))
                {
                    cmd.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                    cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (!rdr.Read())
                        {
                            MessageBox.Show("عفوا..لا يوجد سجلات", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                }



                // Create a new report instance (assuming rptSales2 is a report class)
                var rpt = new rptSales2();

                string query = @"
                    SELECT 
                        CONVERT(varchar(10), YEAR(InvoiceDate)) AS Year,
                        InvoiceInfo.Inv_ID,
                        InvoiceInfo.InvoiceNo,
                        InvoiceInfo.InvoiceDate,
                        InvoiceInfo.CustomerID,
                        InvoiceInfo.GrandTotal,
                        InvoiceInfo.TotalPaid,
                        InvoiceInfo.Balance,
                        InvoiceInfo.Remarks,
                        Customer.Name 
                    FROM 
                        InvoiceInfo 
                    INNER JOIN 
                        Customer ON InvoiceInfo.CustomerID = Customer.ID 
                    WHERE 
                        InvoiceInfo.InvoiceDate >= @d3 AND InvoiceInfo.InvoiceDate < @d4 
                    ORDER BY 
                        InvoiceInfo.InvoiceDate";
                System.Data.DataTable dtable = new System.Data.DataTable();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@d3", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                    cmd.Parameters.Add("@d4", SqlDbType.DateTime).Value = dtpDateTo.Value.Date.AddDays(1);

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dtable);

                }


                DataSet myDS1 = new DataSet();
                myDS1.Tables.Add(dtable);
                myDS1.WriteXmlSchema("TotalSales.xml");

                rpt.SetDataSource(myDS1);
                rpt.SetParameterValue("p1", dtpDateFrom.Value.Date);
                rpt.SetParameterValue("p2", dtpDateTo.Value.Date);
                rpt.SetParameterValue("p7", DateTime.Today);

                frmReport.crystalReportViewer1.ReportSource = rpt;
                frmReport.Show();

                con.Close();
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                decimal a = 0;
                decimal b = 0;
                decimal c = 0;
                decimal d = 0;
                frmReport frmReport = new frmReport();
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string ctn = "select InvoiceNo from InvoiceInfo where InvoiceDate between @d1 and @d2";
                    using (SqlCommand cmd = new SqlCommand(ctn, con))
                    {
                        cmd.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                        cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;
                        SqlDataReader rdr = cmd.ExecuteReader();

                        if (!rdr.Read())
                        {
                            MessageBox.Show("عذراً..لا يوجد سجلات", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            rdr.Close();
                            return;
                        }


                        rptSales1 rpt = new rptSales1();
                        SqlCommand MyCommand = new SqlCommand();
                        SqlCommand MyCommand1 = new SqlCommand();
                        SqlDataAdapter myDA = new SqlDataAdapter();
                        SqlDataAdapter myDA1 = new SqlDataAdapter();
                        DataSet myDS = new DataSet();

                        using (SqlConnection myConnection = new SqlConnection(DataAccessLayer.Con()))
                        {
                            MyCommand.Connection = myConnection;
                            MyCommand1.Connection = myConnection;
                            MyCommand.CommandText = "SELECT Customer.ID, Customer.Name, Customer.Gender, Customer.Address, Customer.City, Customer.State, Customer.ZipCode, Customer.ContactNo, Customer.EmailID, Customer.Remarks, Customer.Photo, InvoiceInfo.Inv_ID, InvoiceInfo.InvoiceNo, InvoiceInfo.InvoiceDate, InvoiceInfo.CustomerID , InvoiceInfo.GrandTotal, InvoiceInfo.TotalPaid, InvoiceInfo.Balance, Invoice_Product.IPo_ID, Invoice_Product.InvoiceID, Invoice_Product.ProductID, Invoice_Product.CostPrice, Invoice_Product.SellingPrice, Invoice_Product.Margin, Invoice_Product.Qty, Invoice_Product.Amount, Invoice_Product.DiscountPer, Invoice_Product.Discount, Invoice_Product.VATPer, Invoice_Product.VAT, Invoice_Product.TotalAmount, Product.PID, Product.ProductCode, Product.ProductName FROM Customer INNER JOIN InvoiceInfo ON Customer.ID = InvoiceInfo.CustomerID INNER JOIN Invoice_Product ON InvoiceInfo.Inv_ID = Invoice_Product.InvoiceID INNER JOIN Product ON Invoice_Product.ProductID = Product.PID where InvoiceDate between @d1 and @d2 order by InvoiceDate";
                            MyCommand.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                            MyCommand.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;
                            MyCommand1.CommandText = "SELECT * from Company";
                            MyCommand.CommandType = CommandType.Text;
                            MyCommand1.CommandType = CommandType.Text;

                            myDA.SelectCommand = MyCommand;
                            myDA1.SelectCommand = MyCommand1;
                            myDA.Fill(myDS, "InvoiceInfo");
                            myDA.Fill(myDS, "Invoice_Product");
                            myDA.Fill(myDS, "Customer");
                            myDA.Fill(myDS, "Product");
                            myDA1.Fill(myDS, "Company");
                        }

                        using (SqlConnection con2 = new SqlConnection(DataAccessLayer.Con()))
                        {
                            con2.Open();
                            string ct = "select ISNULL(sum(GrandTotal),0),ISNULL(sum(TotalPaid),0),ISNULL(sum(Balance),0) from InvoiceInfo where InvoiceDate between @d1 and @d2";
                            using (SqlCommand cmd2 = new SqlCommand(ct, con2))
                            {
                                cmd2.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                                cmd2.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;
                                SqlDataReader rdr2 = cmd2.ExecuteReader();
                                if (rdr2.Read())
                                {
                                    a = Convert.ToDecimal(rdr2.GetValue(0));
                                    b = Convert.ToDecimal(rdr2.GetValue(1));
                                    c = Convert.ToDecimal(rdr2.GetValue(2));
                                }
                                else
                                {
                                    a = 0;
                                    b = 0;
                                    c = 0;
                                }
                            }
                        }

                        using (SqlConnection con3 = new SqlConnection(DataAccessLayer.Con()))
                        {
                            con3.Open();
                            string ct1 = "select ISNULL(sum(Margin),0) from InvoiceInfo,Invoice_Product where InvoiceInfo.Inv_ID=Invoice_Product.InvoiceID and InvoiceDate between @d1 and @d2";
                            using (SqlCommand cmd3 = new SqlCommand(ct1, con3))
                            {
                                cmd3.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                                cmd3.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;
                                SqlDataReader rdr3 = cmd3.ExecuteReader();
                                if (rdr3.Read())
                                {
                                    d = Convert.ToDecimal(rdr3.GetValue(0));
                                }
                                else
                                {
                                    d = 0;
                                }
                            }
                        }

                        rpt.SetDataSource(myDS);
                        rpt.SetParameterValue("p1", dtpDateFrom.Value.Date);
                        rpt.SetParameterValue("p2", dtpDateTo.Value.Date);
                        rpt.SetParameterValue("p3", a);
                        rpt.SetParameterValue("p4", b);
                        rpt.SetParameterValue("p5", c);
                        rpt.SetParameterValue("p6", d);
                        rpt.SetParameterValue("p7", DateTime.Today);

                        frmReport.crystalReportViewer1.ReportSource = rpt;
                        frmReport.Show();
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
