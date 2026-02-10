
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
    public partial class CustomerSuppReport : Form
    {
        public CustomerSuppReport()
        {
            InitializeComponent();
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {

            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    Cursor = Cursors.WaitCursor;
                    Timer1.Enabled = true;
                    frmReport frmReport = new frmReport();

                    con.Open();
                    string ct = @"SELECT 
	                                c.ID,
                                    c.CustomerID,    
                                    c.Name AS CustomerName,
	                                SUM(i.Debit),
                                    SUM(i.Balance) AS Balance
                                FROM 
                                    [dbo].[Debitors] i
                                INNER JOIN 
                                    [dbo].[Customer] c ON i.CustomerID = c.CustomerID 
                                INNER JOIN 
                                    [dbo].[InvoiceInfo] inv ON inv.CustomerID = c.ID 
                           
                                GROUP BY 
                                    c.ID,c.CustomerID, c.Name	 ";
                    using (SqlCommand cmd = new SqlCommand(ct, con))
                    {
                      
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

                    string query = @"SELECT 
                                    c.CustomerID AS EntityCode,
                                    c.Name AS EntityName,
                                    SUM(CAST(i.Debit AS FLOAT)) AS TotalDebit, -- Convert Debit to FLOAT
                                    SUM(CASE 
                                        WHEN TRY_CAST(i.Balance AS FLOAT) IS NOT NULL THEN TRY_CAST(i.Balance AS FLOAT) 
                                        ELSE 0 
                                    END)
                                 AS TotalBalance,
                                    N'عميل' AS EntityType
                                FROM 
                                    [dbo].[Debitors] i
                                INNER JOIN 
                                    [dbo].[Customer] c ON i.CustomerID = c.CustomerID 
                                GROUP BY 
                                    c.CustomerID, c.Name

                                UNION ALL

                                SELECT 
                                    s.SupplierID AS EntityCode,
                                    s.Name AS EntityName,
                                    SUM(CAST(s.Debit AS FLOAT)) AS TotalDebit, -- Convert Debit to FLOAT
                                    SUM(CASE 
                                            WHEN CAST(s.Balance AS FLOAT) > 0 THEN -CAST(s.Balance AS FLOAT) -- Convert positive balance to negative
                                            ELSE CAST(s.Balance AS FLOAT) -- Keep negative balance
                                        END) AS TotalBalance,
                                    N'مورد' AS EntityType
                                FROM 
                                   [dbo].[Debitors_supplier] s
                                GROUP BY 
                                    s.SupplierID, s.Name;

                                    ";
                    string totalBalanceQuery = @"
                                                SELECT 
                                                    SUM(CustomerBalance + SupplierBalance) AS TotalBalance
                                                FROM (
                                                    SELECT 
                                                        SUM(TRY_CAST(i.Balance AS FLOAT)) AS CustomerBalance,
                                                        0 AS SupplierBalance
                                                    FROM 
                                                        [dbo].[Debitors] i

                                                    UNION ALL

                                                    SELECT 
                                                        0 AS CustomerBalance,
                                                        SUM(CASE 
                                                                WHEN TRY_CAST(s.Balance AS FLOAT) > 0 THEN -TRY_CAST(s.Balance AS FLOAT)
                                                                ELSE TRY_CAST(s.Balance AS FLOAT)
                                                            END) AS SupplierBalance
                                                    FROM 
                                                        [dbo].[Debitors_supplier] s
                                                ) AS CombinedBalances;";

                    // Fetch total balance
                    double totalBalance = 0;
                    using (SqlCommand cmdTotal = new SqlCommand(totalBalanceQuery, con))
                    {
                        object result = cmdTotal.ExecuteScalar();
                        totalBalance = result != DBNull.Value ? Convert.ToDouble(result) : 0;
                    }
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            dt.Columns.Add("EntityID", typeof(int));
                            dt.Columns.Add("EntityCode", typeof(string));
                            dt.Columns.Add("EntityName", typeof(string));
                            dt.Columns.Add("TotalDebit", typeof(double));  // TotalDebit is numeric
                            dt.Columns.Add("TotalBalance", typeof(string));  // TotalBalance as string (formatted with + for customers)
                            dt.Columns.Add("EntityType", typeof(string));

                            // Fill the DataTable with data from the SQL query
                            adp.Fill(dt);

                            rptGeneralCSReport rpt = new rptGeneralCSReport();
                            rpt.SetDataSource(dt);
                            // Pass total balance as a parameter
                            rpt.SetParameterValue("p1", totalBalance); // Ensure "p1" matches the parameter name in your report

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

        private void Label1_Click(object sender, EventArgs e)
        {

        }
    }
}
