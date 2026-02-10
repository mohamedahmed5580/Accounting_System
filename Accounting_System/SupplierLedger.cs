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

                string a = "", b = "", c = "";
                double f = 0, g = 0;

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // Fetch supplier details
                    string fetchSupplierQuery = @"SELECT RTRIM(SupplierID), RTRIM(Address), RTRIM(City), RTRIM(ContactNo) 
                                          FROM Supplier WHERE Name = @d1";
                    using (SqlCommand cmd = new SqlCommand(fetchSupplierQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", cmbSupplierName.Text);
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                txtSupplierID.Text = rdr[0]?.ToString() ?? "";
                                a = rdr[1]?.ToString() ?? "";
                                b = rdr[2]?.ToString() ?? "";
                                c = rdr[3]?.ToString() ?? "";
                            }
                        }
                    }

                    // Query 1: Fetch raw data for display
                    string fetchQuery1 = @"
                SELECT Date, Name, LedgerNo, Label, Credit, Debit, Currencies, CPrice 
                FROM SupplierLedgerBook 
                WHERE Date >= @d1 
                AND Date < @d2 
                AND PartyID = @d3 
                ORDER BY Date;";

                    DataTable dtableDisplay = new DataTable();
                    using (SqlCommand cmd1 = new SqlCommand(fetchQuery1, con))
                    {
                        cmd1.Parameters.AddWithValue("@d1", dtpDateFrom.Value.Date);
                        cmd1.Parameters.AddWithValue("@d2", dtpDateTo.Value.Date.AddDays(1).AddTicks(-1));
                        cmd1.Parameters.AddWithValue("@d3", txtSupplierID.Text);

                        using (SqlDataAdapter adp1 = new SqlDataAdapter(cmd1))
                        {
                            adp1.Fill(dtableDisplay);
                        }
                    }

                    // Check if there are records to display
                    if (dtableDisplay.Rows.Count == 0)
                    {
                        MessageBox.Show("عذرًا...لا يوجد سجلات", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Query 2: Fetch calculated Credit/Debit for f and g
                    string fetchQuery = @"
    SELECT   Credit AS CreditCalculated, Debit AS DebitCalculated ,Label
        
    FROM SupplierLedgerBook 
    WHERE PartyID = @d3 
    AND Date Between @DateFrom  AND @DateTo
   ;";

                    DataTable dtableCalc = new DataTable();
                    using (SqlCommand cmd2 = new SqlCommand(fetchQuery, con))
                    {
                        cmd2.Parameters.AddWithValue("@DateFrom", dtpDateFrom.Value.Date);
                        cmd2.Parameters.AddWithValue("@DateTo", dtpDateTo.Value.Date.AddDays(1).AddTicks(-1));
                        cmd2.Parameters.AddWithValue("@d3", txtSupplierID.Text);

                        using (SqlDataAdapter adp2 = new SqlDataAdapter(cmd2))
                        {
                            adp2.Fill(dtableCalc);
                        }
                    }

                    // Calculate f (Debit Total) and g (Credit Total)
                    foreach (DataRow row in dtableCalc.Rows)
                    {

                       
                        double debit = row["DebitCalculated"] == DBNull.Value ? 0 : Convert.ToDouble(row["DebitCalculated"]);
                        double credit = row["CreditCalculated"] == DBNull.Value ? 0 : Convert.ToDouble(row["CreditCalculated"]);

                        f += debit;
                        g += credit;
                    }
                    // Generate XML and report
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dtableDisplay); // Use raw data for display
                    ds.WriteXmlSchema("SupplierLedger.xml");

                    rptSupplierLedger rpt = new rptSupplierLedger();
                    rpt.SetDataSource(ds);
                    rpt.SetParameterValue("p1", dtpDateFrom.Value.Date);
                    rpt.SetParameterValue("p2", dtpDateTo.Value.Date);
                    rpt.SetParameterValue("p3", txtSupplierID.Text);
                    rpt.SetParameterValue("p4", cmbSupplierName.Text);
                    rpt.SetParameterValue("p5", a);
                    rpt.SetParameterValue("p6", b);
                    rpt.SetParameterValue("p7", c);
                    rpt.SetParameterValue("f", f);
                    rpt.SetParameterValue("g", g);

                    if (f > g)
                    {
                        rpt.SetParameterValue("i", f - g);
                        rpt.SetParameterValue("j", 0);
                    }
                    else
                    {
                        rpt.SetParameterValue("j", g - f);
                        rpt.SetParameterValue("i", 0);
                    }

                    frmReport frmReport = new frmReport();
                    frmReport.crystalReportViewer1.ReportSource = rpt;
                    frmReport.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void GroupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cmbSupplierName.Text))
                {
                    MessageBox.Show("الرجاء اختيار اسم المورد", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbSupplierName.Focus();
                    return;
                }

                string a = "", b = "", c = "";
                double f = 0, g = 0;

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // Fetch supplier details
                    string fetchSupplierQuery = @"SELECT RTRIM(SupplierID), RTRIM(Address), RTRIM(City), RTRIM(ContactNo) 
                                      FROM Supplier WHERE Name = @d1";
                    using (SqlCommand cmd = new SqlCommand(fetchSupplierQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", cmbSupplierName.Text);
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                txtSupplierID.Text = rdr[0]?.ToString() ?? "";
                                a = rdr[1]?.ToString() ?? "";
                                b = rdr[2]?.ToString() ?? "";
                                c = rdr[3]?.ToString() ?? "";
                            }
                        }
                    }

                    /*    // Check records in SupplierLedgerBook
                        string checkQuery = @"SELECT PartyID FROM SupplierLedgerBook 
                                  WHERE PartyID = @d1 
                                  AND Date >= @d2 
                                  AND Date < @d3";
                        using (SqlCommand cmd = new SqlCommand(checkQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@d1", string.IsNullOrEmpty(txtSupplierID.Text) ? DBNull.Value.ToString() : txtSupplierID.Text);
                            cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                            cmd.Parameters.Add("@d3", SqlDbType.DateTime).Value = dtpDateTo.Value.Date.AddDays(1).AddTicks(-1);

                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                if (!rdr.HasRows)
                                {
                                    MessageBox.Show("عذرًا...لا يوجد سجلات", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return;
                                }
                            }
                        }*/

                    string fetchQuery = @"
                                SELECT 
                                    Date, 
                                    Name, 
                                    LedgerNo, 
                                    Label, 

                                    CASE 
                                        WHEN Label = 'مردودات مشتريات' THEN  (
                                            CASE 
                                                WHEN CPrice IS NOT NULL AND CPrice <> 0 
                                                THEN CAST(Credit AS DECIMAL(18,2)) / CPrice 
                                                ELSE CAST(Credit AS DECIMAL(18,2)) 
                                            END
                                        )
                                        ELSE 
                                            CASE 
                                                WHEN CPrice IS NOT NULL AND CPrice <> 0 
                                                THEN CAST(Credit AS DECIMAL(18,2)) / CPrice 
                                                ELSE CAST(Credit AS DECIMAL(18,2)) 
                                            END
                                    END AS Credit, 

                                    CASE 
                                        WHEN Label = 'مردودات مشتريات' THEN  (
                                            CASE 
                                                WHEN CPrice IS NOT NULL AND CPrice <> 0 
                                                THEN CAST(Debit AS DECIMAL(18,2)) / CPrice 
                                                ELSE CAST(Debit AS DECIMAL(18,2)) 
                                            END
                                        )
                                        ELSE 
                                            CASE 
                                                WHEN CPrice IS NOT NULL AND CPrice <> 0 
                                                THEN CAST(Debit AS DECIMAL(18,2)) / CPrice 
                                                ELSE CAST(Debit AS DECIMAL(18,2)) 
                                            END
                                    END AS Debit, 

                                    Currencies, 
                                    CPrice 
                                FROM SupplierLedgerBook 
                                WHERE PartyID = @d3 
                                ORDER BY Date;
                            ";

                    using (SqlCommand cmd = new SqlCommand(fetchQuery, con))
                    {
                        cmd.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                        cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date.AddDays(1).AddTicks(-1);
                        cmd.Parameters.AddWithValue("@d3", txtSupplierID.Text);

                        using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                        {
                            DataTable dtable = new DataTable();
                            adp.Fill(dtable);

                            // Calculate f and g
                            foreach (DataRow row in dtable.Rows)
                            {
                                double costPrice, vat, costPrice1, vat1;
                                double.TryParse(row["Debit"]?.ToString(), out costPrice);
                                double.TryParse(row["CPrice"]?.ToString(), out vat);
                                f += costPrice * vat;

                                double.TryParse(row["Credit"]?.ToString(), out costPrice1);
                                double.TryParse(row["CPrice"]?.ToString(), out vat1);
                                g += costPrice1 * vat1;
                            }

                            // Generate XML

                            // Generate XML
                            DataSet ds = new DataSet();
                            ds.Tables.Add(dtable);
                            ds.WriteXmlSchema("SupplierLedger.xml");

                            // Create and display report
                            rptSupplierLedger rpt = new rptSupplierLedger();
                            rpt.SetDataSource(ds);
                            rpt.SetParameterValue("p1", dtpDateFrom.Value.Date);
                            rpt.SetParameterValue("p2", dtpDateTo.Value.Date);
                            rpt.SetParameterValue("p3", txtSupplierID.Text);
                            rpt.SetParameterValue("p4", cmbSupplierName.Text);
                            rpt.SetParameterValue("p5", a);
                            rpt.SetParameterValue("p6", b);
                            rpt.SetParameterValue("p7", c);
                            rpt.SetParameterValue("f", f);
                            rpt.SetParameterValue("g", g);
                            if (f > g)
                            {
                                rpt.SetParameterValue("i", f - g);
                                rpt.SetParameterValue("j", 0);

                            }
                            else
                            {
                                rpt.SetParameterValue("j", g - f);
                                rpt.SetParameterValue("i", 0);

                            }

                            frmReport frmReport = new frmReport();
                            frmReport.crystalReportViewer1.ReportSource = rpt;
                            frmReport.ShowDialog();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
