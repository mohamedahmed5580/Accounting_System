using Microsoft.Office.Interop.Excel;
using Pharmacy.DL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace Accounting_System
{
    public partial class PymentinvoiceScreen : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        private static PymentinvoiceScreen _instance;
        public static PymentinvoiceScreen Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new PymentinvoiceScreen();
                }
                return _instance;
            }
        }
        public PymentinvoiceScreen()
        {
            InitializeComponent();
            Getdata();
        }
        public void Getdata()
        {
            dgw.Rows.Clear();
            try
            {
                using (con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    string query = "  SELECT ST_ID, RTRIM(InvoiceNo), Date, RTRIM(PurchaseType), Supplier.ID, RTRIM(Supplier.SupplierID), RTRIM(Supplier.Name), SubTotal, DiscountPer, Discount, VATPer, VATAmt, FreightCharges, OtherCharges, PreviousDue, Total, RoundOff, GrandTotal, TotalPayment, PaymentDue, RTRIM(Stock.Remarks) ,Stock.CurrencieName  FROM Supplier, Stock  WHERE Supplier.ID = Stock.SupplierID  ORDER BY [Date]\r\n";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            dgw.Rows.Clear();

                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], rdr[16], rdr[17], rdr[18], rdr[19], rdr[20], rdr[21]);
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

        private void btnGetData_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT ST_ID, RTRIM(InvoiceNo), Date, RTRIM(PurchaseType), Supplier.ID, RTRIM(Supplier.SupplierID), RTRIM(Name), SubTotal, DiscountPer, Discount, VATPer, VATAmt, FreightCharges, OtherCharges, PreviousDue, Total, RoundOff, GrandTotal, TotalPayment, PaymentDue, RTRIM(Stock.Remarks) FROM Supplier, Stock WHERE Supplier.ID = Stock.SupplierID AND [Date] BETWEEN @d1 AND @d2 ORDER BY [Date]", con);

                cmd.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    dgw.Rows.Clear();

                    while (rdr.Read())
                    {
                        dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], rdr[16], rdr[17], rdr[18], rdr[19], rdr[20]);
                    }
                }

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSupplierName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    string query = "SELECT ST_ID, RTRIM(InvoiceNo), Date, RTRIM(PurchaseType), Supplier.ID, RTRIM(Supplier.SupplierID), RTRIM(Name), SubTotal, DiscountPer, Discount, VATPer, VATAmt, FreightCharges, OtherCharges, PreviousDue, Total, RoundOff, GrandTotal, TotalPayment, PaymentDue, RTRIM(Stock.Remarks) FROM Supplier, Stock WHERE Supplier.ID = Stock.SupplierID AND [Name] LIKE @SupplierName ORDER BY [Date]";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SupplierName", "%" + txtSupplierName.Text + "%");

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            dgw.Rows.Clear();

                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], rdr[16], rdr[17], rdr[18], rdr[19], rdr[20]);
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
        public void Reset()
        {
            txtSupplierName.Text = "";
            dtpDateFrom.Text = DateTime.Today.ToString();
            dtpDateTo.Text = DateTime.Today.ToString();
            Getdata();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnReset_Click_1(object sender, EventArgs e)
        {
            Reset();
        }

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        // Assuming 'con' is a SqlConnection object defined at the class level

        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            DataGridViewRow dr = dgw.SelectedRows[0];

            if (lblSet.Text == "Purchase")
            {

                string connectionString = DataAccessLayer.Con();
                if (string.IsNullOrEmpty(connectionString))
                {
                    MessageBox.Show("The connection string is not initialized. Please check the configuration.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        con.Open();
                        string sql = "SELECT PID, RTRIM(Product.ProductCode), RTRIM(Productname), RTRIM(Stock_Product.Barcode), " +
                                     "Qty, Price, TotalAmount, Stock_Product.CurrencieName " +
                                     "FROM Stock, Stock_Product, Product " +
                                     "WHERE Product.PID = Stock_Product.ProductID " +
                                     "AND Stock.ST_ID = Stock_Product.StockID " +
                                     "AND ST_ID = " + dr.Cells[0].Value + "";

                        SqlCommand cmd = new SqlCommand(sql, con);
                        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        Pymentinvoice.instance.DataGridView1.Rows.Clear();
                        while (rdr.Read())
                        {
                            Pymentinvoice.instance.DataGridView1.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7]);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                    finally
                    {
                        con.Close();
                    }

                    // Set the fields in frmPurchaseEntry based on the selected row
                    Pymentinvoice.instance.txtST_ID.Text = dr.Cells[0].Value.ToString();
                    Pymentinvoice.instance.txtInvoiceNo.Text = dr.Cells[1].Value.ToString();
                    Pymentinvoice.instance.dtpDate.Text = dr.Cells[2].Value.ToString();
                    Pymentinvoice.instance.cmbPurchaseType.Text = dr.Cells[3].Value.ToString();
                    Pymentinvoice.instance.txtSup_ID.Text = dr.Cells[4].Value.ToString();
                    Pymentinvoice.instance.txtSupplierID.Text = dr.Cells[5].Value.ToString();
                    Pymentinvoice.instance.txtSupplierName.Text = dr.Cells[6].Value.ToString();
                    Pymentinvoice.instance.txtSubTotal.Text = dr.Cells[7].Value.ToString();
                    Pymentinvoice.instance.txtDiscPer.Text = dr.Cells[8].Value.ToString();
                    Pymentinvoice.instance.txtDisc.Text = dr.Cells[9].Value.ToString();
                    Pymentinvoice.instance.txtVATPer.Text = dr.Cells[10].Value.ToString();
                    Pymentinvoice.instance.txtVATAmt.Text = dr.Cells[11].Value.ToString();
                    Pymentinvoice.instance.txtFreightCharges.Text = dr.Cells[12].Value.ToString();
                    Pymentinvoice.instance.txtOtherCharges.Text = dr.Cells[13].Value.ToString();
                    Pymentinvoice.instance.txtPreviousDue.Text = dr.Cells[14].Value.ToString();
                    Pymentinvoice.instance.txtTotal.Text = dr.Cells[15].Value.ToString();
                    Pymentinvoice.instance.txtRoundOff.Text = dr.Cells[16].Value.ToString();
                    Pymentinvoice.instance.txtGrandTotal.Text = dr.Cells[17].Value.ToString();
                    Pymentinvoice.instance.txtTotalPaid.Text = dr.Cells[18].Value.ToString();
                    Pymentinvoice.instance.txtBalance.Text = dr.Cells[19].Value.ToString();
                    Pymentinvoice.instance.txtRemarks.Text = dr.Cells[20].Value.ToString();

                    // Set other properties and call methods on frmPurchaseEntry as needed
                    Pymentinvoice.instance.btnSave.Enabled = false;
                    Pymentinvoice.instance.DataGridView1.Enabled = true;
                    Pymentinvoice.instance.btnAdd.Enabled = true;
                    Pymentinvoice.instance.GetSupplierBalance1();
                    Pymentinvoice.instance.btnDelete.Enabled = true;
                    Pymentinvoice.instance.GetSupplierInfo();
                    Pymentinvoice.instance.btnSelection.Enabled = false;

                    this.Close();
                }
            }
            else if (lblSet.Text == "PR")
            {
                // Handling Purchase Return
                PurchaseReturn frmPurchaseReturn = PurchaseReturn.instance;

                frmPurchaseReturn.txtPurchaseID.Text = dr.Cells[0].Value.ToString();
                frmPurchaseReturn.txtPurchaseInvoiceNo.Text = dr.Cells[1].Value.ToString();
                frmPurchaseReturn.dtpPurchaseDate.Text = dr.Cells[2].Value.ToString();
                frmPurchaseReturn.txtSup_ID.Text = dr.Cells[4].Value.ToString();
                frmPurchaseReturn.txtSupplierID.Text = dr.Cells[5].Value.ToString();
                frmPurchaseReturn.txtSupplierName.Text = dr.Cells[6].Value.ToString();
                frmPurchaseReturn.txtDiscPer.Text = dr.Cells[8].Value.ToString();
                frmPurchaseReturn.txtDisc.Text = dr.Cells[9].Value.ToString();
                frmPurchaseReturn.txtVatPer.Text = dr.Cells[10].Value.ToString();
                frmPurchaseReturn.txtVATAmt.Text = dr.Cells[11].Value.ToString();
                frmPurchaseReturn.auto();
                frmPurchaseReturn.btnSelection.Enabled = false;


                string connectionString = DataAccessLayer.Con();
                if (string.IsNullOrEmpty(connectionString))
                {
                    MessageBox.Show("The connection string is not initialized. Please check the configuration.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        con.Open();
                        string sql = "SELECT ProductID, RTRIM(ProductCode), RTRIM(ProductName), RTRIM(Stock_Product.Barcode), " +
                                     "Qty, Price, TotalAmount " +
                                     "FROM Stock, Stock_Product, Product " +
                                     "WHERE Stock.ST_ID = Stock_Product.StockID " +
                                     "AND Stock_Product.ProductID = Product.PID " +
                                     "AND ST_ID = " + dr.Cells[0].Value + "";

                        SqlCommand cmd = new SqlCommand(sql, con);
                        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        while (rdr.Read())
                        {
                            frmPurchaseReturn.DataGridView2.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6]);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                    finally
                    {
                        con.Close();
                    }

                    this.Close();
                }
            }

        }

        private void PymentinvoiceScreen_Load(object sender, EventArgs e)
        {
            Getdata();

        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            ExportExcel(dgw);
        }
        public static void ExportExcel(object obj)
        {
            short rowsTotal, colsTotal;
            short I, j, iC;
            Cursor.Current = Cursors.WaitCursor;
            var xlApp = new Excel.Application();
            try
            {
                var excelBook = xlApp.Workbooks.Add();
                var excelWorksheet = (Excel.Worksheet)excelBook.Worksheets[1];
                xlApp.Visible = true;

                rowsTotal = (short)((DataGridView)obj).RowCount;
                colsTotal = (short)(((DataGridView)obj).Columns.Count - 1);
                excelWorksheet.Cells.Select();
                excelWorksheet.Cells.Delete();
                for (iC = 0; iC <= colsTotal; iC++)
                {
                    excelWorksheet.Cells[1, iC + 1].Value = ((DataGridView)obj).Columns[iC].HeaderText;
                }
                for (I = 0; I < rowsTotal; I++)
                {
                    for (j = 0; j <= colsTotal; j++)
                    {
                        excelWorksheet.Cells[I + 2, j + 1].Value = ((DataGridView)obj).Rows[I].Cells[j].Value;
                    }
                }
                excelWorksheet.Rows["1:1"].Font.FontStyle = "Bold";
                excelWorksheet.Rows["1:1"].Font.Size = 12;

                excelWorksheet.Cells.Columns.AutoFit();
                excelWorksheet.Cells.Select();
                excelWorksheet.Cells.EntireColumn.AutoFit();
                excelWorksheet.Cells[1, 1].Select();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                xlApp = null;
            }
        }
    }
}
