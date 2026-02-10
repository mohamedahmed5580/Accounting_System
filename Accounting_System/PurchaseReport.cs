
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
    public partial class PurchaseReport : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public PurchaseReport()
        {
            InitializeComponent();
            fillSupplier();

        }
        private void fillSupplier()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con())) ;
                {
                    con.Open();
                    using (SqlDataAdapter adp = new SqlDataAdapter())
                    {
                        adp.SelectCommand = new SqlCommand("SELECT RTRIM(Name) FROM Supplier ORDER BY Name", con);
                        DataSet ds = new DataSet("ds");
                        adp.Fill(ds);
                        DataTable dtable = ds.Tables[0];
                        cmbSupplier.Items.Clear();
                        foreach (DataRow drow in dtable.Rows)
                        {
                            cmbSupplier.Items.Add(drow[0].ToString());
                        }
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
            cmbSupplier.Text = "";
            dtpDateFrom.Value = DateTime.Today;
            dtpDateTo.Value = DateTime.Today;

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        /*  private void btnViewReport_Click(object sender, EventArgs e)
          {
              try
              {
                  if (string.IsNullOrWhiteSpace(cmbSupplier.Text))
                  {
                      MessageBox.Show("الرجاء اختيار المورد", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                      cmbSupplier.Focus();
                      return;
                  }

                  frmReport frmReport = new frmReport();
                  rptPurchase rpt = new rptPurchase(); // The report

                  DataSet myDS = new DataSet();
                  DataSet myDS1 = new DataSet();

                  // Declare variables outside of the `using` block
                  decimal totalGrand = 0, totalPaid = 0, totalDue = 0;
                  double totalCredit = 0, totalDebit = 0;

                  using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                  {
                      con.Open();

                      // Fetch Purchase Summary
                      string summaryQuery = @"
                  SELECT ISNULL(CAST(SUM(GrandTotal) AS DECIMAL(18,2)),0), 
                         ISNULL(CAST(SUM(TotalPayment) AS DECIMAL(18,2)),0), 
                         ISNULL(CAST(SUM(PaymentDue) AS DECIMAL(18,2)),0) 
                  FROM Stock 
                  INNER JOIN Supplier ON Supplier.ID = Stock.SupplierID 
                  WHERE Supplier.Name = @d1";

                      using (SqlCommand cmd = new SqlCommand(summaryQuery, con))
                      {
                          cmd.Parameters.AddWithValue("@d1", cmbSupplier.Text);
                          using (SqlDataReader rdr = cmd.ExecuteReader())
                          {
                              if (rdr.Read())
                              {
                                  totalGrand = rdr.IsDBNull(0) ? 0 : rdr.GetDecimal(0);
                                  totalPaid = rdr.IsDBNull(1) ? 0 : rdr.GetDecimal(1);
                                  totalDue = rdr.IsDBNull(2) ? 0 : rdr.GetDecimal(2);
                              }
                          }
                      }
  *//*
                      // Fetch Ledger Book Entries
                      string fetchQuery = @"
                  SELECT Date, Name, LedgerNo, Label, 
                         ISNULL(CAST(Credit AS FLOAT),0) AS Credit, 
                         ISNULL(CAST(Debit AS FLOAT),0) AS Debit, 
                         Currencies, 
                         ISNULL(CAST(CPrice AS FLOAT),0) AS CPrice 
                  FROM SupplierLedgerBook 
                  WHERE Date >= @d1 
                  AND Date < @d2 
                  AND PartyID = @d3 
                  ORDER BY Date";

                      DataTable ledgerTable = new DataTable();

                      using (SqlCommand cmdd = new SqlCommand(fetchQuery, con))
                      {
                          cmdd.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                          cmdd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date.AddDays(1).AddTicks(-1);
                          cmdd.Parameters.AddWithValue("@d3", cmbSupplier.Text);

                          using (SqlDataAdapter adp = new SqlDataAdapter(cmdd))
                          {
                              adp.Fill(ledgerTable);
                          }
                      }

                      // Calculate total credit and debit
                      foreach (DataRow row in ledgerTable.Rows)
                      {
                          double credit = Convert.ToDouble(row["Credit"]);
                          double debit = Convert.ToDouble(row["Debit"]);
                          double cPrice = Convert.ToDouble(row["CPrice"]);

                          totalCredit += credit * cPrice;
                          totalDebit += debit * cPrice;
                      }*//*
                  }

                  // Set up report
                  rpt.SetParameterValue("p1", dtpDateFrom.Value.Date);
                  rpt.SetParameterValue("p2", dtpDateTo.Value.Date);
                  rpt.SetParameterValue("p3", totalGrand);
                  rpt.SetParameterValue("p4", totalPaid);
                  rpt.SetParameterValue("p5", totalDue);
                  rpt.SetParameterValue("p6", DateTime.Today);
   *//*               rpt.SetParameterValue("f", totalCredit);
                  rpt.SetParameterValue("g", totalDebit);
  *//*
                  frmReport.crystalReportViewer1.ReportSource = rpt;
                  frmReport.ShowDialog();
              }
              catch (Exception ex)
              {
                  MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
              }
          }
  */
        private void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cmbSupplier.Text))
                {
                    MessageBox.Show("الرجاء اختيار المورد", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbSupplier.Focus();
                    return;
                }
                frmReport frmReport = new frmReport();

                rptPurchase rpt = new rptPurchase(); // The report you created
                SqlConnection myConnection;
                SqlCommand MyCommand = new SqlCommand();
                SqlDataAdapter myDA = new SqlDataAdapter();
                DataSet myDS = new DataSet(); // The DataSet you created
                DataSet myDS1 = new DataSet(); // Another DataSet

                myConnection = new SqlConnection(DataAccessLayer.Con());
                MyCommand.Connection = myConnection;
                MyCommand.CommandText = @"SELECT Distinct Stock.ST_ID, Stock.InvoiceNo, Stock.Date, Stock.SupplierID, 
                              Stock.GrandTotal, Stock.TotalPayment, Stock.PaymentDue, Stock.Remarks, Stock.CurrencieName,
                              Stock_Product.SP_ID, Stock_Product.StockID, Stock_Product.ProductID, 
                              Stock_Product.Qty, Stock_Product.Price, Stock_Product.TotalAmount, Supplier.ID, 
                              Supplier.SupplierID AS Expr1, Supplier.Name, Supplier.Address, Supplier.City, 
                              Supplier.State, Supplier.ZipCode, Supplier.ContactNo, Supplier.EmailID, 
                              Supplier.Remarks AS Expr2, Product.PID, Product.ProductCode, Product.ProductName, 
                              Product.SubCategoryID, Product.Description, Product.CostPrice, Product.SellingPrice, 
                              Product.Discount, Product.VAT, Product.ReorderPoint, Currency_price 
                              FROM Stock 
                              INNER JOIN Stock_Product ON Stock.ST_ID = Stock_Product.StockID 
                              INNER JOIN Supplier ON Stock.SupplierID = Supplier.ID 
                              INNER JOIN Product ON Stock_Product.ProductID = Product.PID 
                              WHERE Supplier.Name = @d1 AND Stock.Date BETWEEN @d2 AND @d3 ";
                MyCommand.Parameters.AddWithValue("@d1", cmbSupplier.Text);
                MyCommand.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                MyCommand.Parameters.Add("@d3", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;

                MyCommand.CommandType = CommandType.Text;
                myDA.SelectCommand = MyCommand;
                myDA.Fill(myDS, "Stock");
                myDA.Fill(myDS, "Stock_Product");
                myDA.Fill(myDS, "Product");
                myDA.Fill(myDS, "Supplier");

                con = new SqlConnection(DataAccessLayer.Con());
                con.Open();
                string ct = "SELECT ISNULL(SUM(GrandTotal * Currency_price),0), ISNULL(SUM(TotalPayment * Currency_price),0), ISNULL(SUM(PaymentDue * Currency_price),0) " +
                            "FROM Stock, Supplier WHERE Supplier.ID = Stock.SupplierID AND Name = @d1  AND Stock.Date BETWEEN @d2 AND @d3 ";
                SqlCommand cmd = new SqlCommand(ct, con);
                cmd.Parameters.AddWithValue("@d1", cmbSupplier.Text);
                cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                cmd.Parameters.Add("@d3", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;

                SqlDataReader rdr = cmd.ExecuteReader();

                decimal a = 0, b = 0, c = 0;

                while (rdr.Read())
                {
                    if (!rdr.IsDBNull(0))
                        a = Convert.ToDecimal(rdr.GetValue(0));  // Ensure safe conversion to decimal
                    if (!rdr.IsDBNull(1))
                        b = Convert.ToDecimal(rdr.GetValue(1));
                    if (!rdr.IsDBNull(2))
                        c = Convert.ToDecimal(rdr.GetValue(2));
                }

                con.Close();

                con = new SqlConnection(DataAccessLayer.Con());
                con.Open();
                cmd = new SqlCommand("SELECT CONVERT(varchar(10), YEAR(Date)) AS Year, SUM(GrandTotal) AS GrandTotal " +
                                     "FROM Stock, Supplier WHERE Supplier.ID = Stock.SupplierID AND Name = @d1 " +
                                     "GROUP BY YEAR(Date) ORDER BY Year", con);
                cmd.Parameters.AddWithValue("@d1", cmbSupplier.Text);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dtable = new DataTable();
                adp.Fill(dtable);
                con.Close();


                // Declare variables outside of the `using` block
                decimal totalGrand = 0, totalPaid = 0, totalDue = 0;
                double totalCredit = 0, totalDebit = 0;


                string fetchQuery = @"
                  SELECT Date, Name, LedgerNo, Label, 
                         ISNULL(CAST(Credit AS FLOAT),0) AS Credit, 
                         ISNULL(CAST(Debit AS FLOAT),0) AS Debit, 
                         Currencies, 
                         ISNULL(CAST(CPrice AS FLOAT),0) AS CPrice 
                  FROM SupplierLedgerBook 
                  WHERE PartyID = @d3 AND Label = 'سند دفع'
                  ORDER BY Date";

                DataTable ledgerTable = new DataTable();

                using (SqlCommand cmdd = new SqlCommand(fetchQuery, con))
                {
                   
                    cmdd.Parameters.AddWithValue("@d3", txtSup_ID.Text);

                    using (SqlDataAdapter adpp = new SqlDataAdapter(cmdd))
                    {
                        adpp.Fill(ledgerTable);
                    }
                }

                // Calculate total credit and debit
                foreach (DataRow row in ledgerTable.Rows)
                {
                    double credit = Convert.ToDouble(row["Credit"]);
                    double debit = Convert.ToDouble(row["Debit"]);
                    double cPrice = Convert.ToDouble(row["CPrice"]);

                    totalCredit += credit ;
                    totalDebit += debit;

                }





                myDS1.Tables.Add(dtable);
                myDS1.WriteXmlSchema("TotalPurchase.xml");

                rpt.Subreports[0].SetDataSource(myDS1);
                rpt.SetDataSource(myDS);
                rpt.SetParameterValue("p1", dtpDateFrom.Value.Date);
                rpt.SetParameterValue("p2", dtpDateTo.Value.Date);
                rpt.SetParameterValue("p3", a);
                rpt.SetParameterValue("p4", b);
                rpt.SetParameterValue("p5", c);
                rpt.SetParameterValue("p6", DateTime.Today);
                rpt.SetParameterValue("f", totalCredit);
                rpt.SetParameterValue("g", Math.Abs( Convert.ToDouble(c) - totalCredit)); // Rounds to 2 decimal places
                rpt.SetParameterValue("i",Math.Abs( Convert.ToDouble(b) - totalCredit) ); // Rounds to 2 decimal places


                frmReport.crystalReportViewer1.ReportSource = rpt;
                frmReport.ShowDialog();
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
                frmReport frmReport = new frmReport();

                rptPurchase rpt = new rptPurchase(); // The report you created
                SqlConnection myConnection;
                SqlCommand MyCommand = new SqlCommand();
                SqlDataAdapter myDA = new SqlDataAdapter();
                DataSet myDS = new DataSet(); // The DataSet you created
                DataSet myDS1 = new DataSet(); // Another DataSet

                myConnection = new SqlConnection(DataAccessLayer.Con());
                MyCommand.Connection = myConnection;
                MyCommand.CommandText = @"SELECT DISTINCT Stock.ST_ID, Stock.InvoiceNo, Stock.Date, Stock.SupplierID, 
                              Stock.GrandTotal, Stock.TotalPayment, Stock.PaymentDue, Stock.Remarks, Stock.CurrencieName,
                              Stock_Product.SP_ID, Stock_Product.StockID, Stock_Product.ProductID, 
                              Stock_Product.Qty, Stock_Product.Price, Stock_Product.TotalAmount, Supplier.ID, 
                              Supplier.SupplierID AS Expr1, Supplier.Name, Supplier.Address, Supplier.City, 
                              Supplier.State, Supplier.ZipCode, Supplier.ContactNo, Supplier.EmailID, 
                              Supplier.Remarks AS Expr2, Product.PID, Product.ProductCode, Product.ProductName, 
                              Product.SubCategoryID, Product.Description, Product.CostPrice, Product.SellingPrice, 
                              Product.Discount, Product.VAT, Product.ReorderPoint, Currency_price
                              FROM Stock 
                              INNER JOIN Stock_Product ON Stock.ST_ID = Stock_Product.StockID 
                              INNER JOIN Supplier ON Stock.SupplierID = Supplier.ID 
                              INNER JOIN Product ON Stock_Product.ProductID = Product.PID 
                             
                              ORDER BY Stock.Date";
                MyCommand.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                MyCommand.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;
                MyCommand.CommandType = CommandType.Text;
                myDA.SelectCommand = MyCommand;
                myDA.Fill(myDS, "Stock");
                myDA.Fill(myDS, "Stock_Product");
                myDA.Fill(myDS, "Product");
                myDA.Fill(myDS, "Supplier");

                con = new SqlConnection(DataAccessLayer.Con());
                con.Open();
                string ct = @"SELECT ISNULL(SUM(GrandTotal*Currency_price), 0), ISNULL(SUM(TotalPayment*Currency_price), 0), ISNULL(SUM(PaymentDue*Currency_price), 0) 
                 FROM Stock, Supplier 
                 WHERE Supplier.ID = Stock.SupplierID 
                ";
                SqlCommand cmd = new SqlCommand(ct, con);
                cmd.Parameters.Add("@d3", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                cmd.Parameters.Add("@d4", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;
                SqlDataReader rdr = cmd.ExecuteReader();

                decimal a = 0, b = 0, c = 0;

                while (rdr.Read())
                {
                    if (!rdr.IsDBNull(0))
                        a = Convert.ToDecimal(rdr.GetValue(0));  // Ensure safe conversion to decimal
                    if (!rdr.IsDBNull(1))
                        b = Convert.ToDecimal(rdr.GetValue(1));
                    if (!rdr.IsDBNull(2))
                        c = Convert.ToDecimal(rdr.GetValue(2));
                }

                con.Close();

                con = new SqlConnection(DataAccessLayer.Con());
                con.Open();
                cmd = new SqlCommand(@"SELECT CONVERT(varchar(10), YEAR(Date)) AS Year, SUM(GrandTotal) AS GrandTotal 
                           FROM Stock 
                           WHERE Date BETWEEN @d3 AND @d4 
                           GROUP BY YEAR(Date) 
                           ORDER BY Year", con);
                cmd.Parameters.Add("@d3", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                cmd.Parameters.Add("@d4", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dtable = new DataTable();
                adp.Fill(dtable);
                con.Close();






                // Declare variables outside of the `using` block
                decimal totalGrand = 0, totalPaid = 0, totalDue = 0;
                double totalCredit = 0, totalDebit = 0;


                string fetchQuery = @"
                  SELECT Date, Name, LedgerNo, Label, 
                         ISNULL(CAST(Credit AS FLOAT),0) AS Credit, 
                         ISNULL(CAST(Debit AS FLOAT),0) AS Debit, 
                         Currencies, 
                         ISNULL(CAST(CPrice AS FLOAT),0) AS CPrice 
                  FROM SupplierLedgerBook 
                  WHERE PartyID = @d3 AND Label = 'سند دفع'
                  ORDER BY Date";

                DataTable ledgerTable = new DataTable();

                using (SqlCommand cmdd = new SqlCommand(fetchQuery, con))
                {

                    cmdd.Parameters.AddWithValue("@d3", txtSup_ID.Text);

                    using (SqlDataAdapter adpp = new SqlDataAdapter(cmdd))
                    {
                        adpp.Fill(ledgerTable);
                    }
                }

                // Calculate total credit and debit
                foreach (DataRow row in ledgerTable.Rows)
                {
                    double credit = Convert.ToDouble(row["Credit"]);
                    double debit = Convert.ToDouble(row["Debit"]);
                    double cPrice = Convert.ToDouble(row["CPrice"]);

                    totalCredit += credit * cPrice;
                    totalDebit += debit * cPrice;

                }









                myDS1.Tables.Add(dtable);
                myDS1.WriteXmlSchema("TotalPurchase.xml");

                rpt.Subreports[0].SetDataSource(myDS1);
                rpt.SetDataSource(myDS);
                rpt.SetParameterValue("p1", dtpDateFrom.Value.Date);
                rpt.SetParameterValue("p2", dtpDateTo.Value.Date);
                rpt.SetParameterValue("p3", a);
                rpt.SetParameterValue("p4", b);
                rpt.SetParameterValue("p5", c);
                rpt.SetParameterValue("p6", DateTime.Today);
                rpt.SetParameterValue("f", totalCredit);
                rpt.SetParameterValue("g", Math.Abs(Convert.ToDouble(c) - totalCredit)); // Rounds to 2 decimal places
                rpt.SetParameterValue("i", Math.Abs(Convert.ToDouble(b) - totalCredit)); // Rounds to 2 decimal places



                frmReport.crystalReportViewer1.ReportSource = rpt;
                frmReport.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default; // Reset cursor to default after operation
            }

        }

        private void PurchaseReport_Load(object sender, EventArgs e)
        {

        }

        private void cmbSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                txtSup_ID.Text = "";

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT RTRIM(SupplierID) FROM Supplier WHERE Name=@d1";
                        cmd.Parameters.AddWithValue("@d1", cmbSupplier.Text);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                txtSup_ID.Text = rdr.GetString(0);

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
    
