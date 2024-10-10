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
using Excel = Microsoft.Office.Interop.Excel;

namespace Accounting_System
{
    public partial class SalesReturnRecord : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public SalesReturnRecord()
        {
            InitializeComponent();
            txtCustomerName.TextChanged += new EventHandler(txtSupplierName_TextChanged);
            dgw.MouseClick += new MouseEventHandler(dgw_MouseDoubleClick);
        }
        public void Getdata()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT SR_ID, RTRIM(SRNo), SalesReturn.Date, SalesID, RTRIM(InvoiceNo), InvoiceDate, RTRIM(Customer.CustomerID), RTRIM(Name), RTRIM(SalesReturn.GrandTotal) " +
                                   "FROM InvoiceInfo, SalesReturn, Customer " +
                                   "WHERE InvoiceInfo.Inv_ID = SalesReturn.SalesID AND Customer.ID = InvoiceInfo.CustomerID " +
                                   "ORDER BY SalesReturn.Date";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8]);
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

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void SalesReturnRecord_Load(object sender, EventArgs e)
        {
            Getdata();
        }

        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (dgw.Rows.Count > 0)
                {
                    if (lblSet.Text == "SR")
                    {
                        DataGridViewRow dr = dgw.SelectedRows[0];
                        // frmSalesReturn.Reset();



                        SalesReturn.Instance.txtSRID.Text = dr.Cells[0].Value.ToString();
                        SalesReturn.Instance.txtSRNO.Text = dr.Cells[1].Value.ToString();
                        SalesReturn.Instance.dtpSRDate.Text = dr.Cells[2].Value.ToString();
                        SalesReturn.Instance.txtSalesID.Text = dr.Cells[3].Value.ToString();
                        SalesReturn.Instance.txtSalesInvoiceNo.Text = dr.Cells[4].Value.ToString();
                        SalesReturn.Instance.dtpSalesDate.Text = dr.Cells[5].Value.ToString();
                        SalesReturn.Instance.txtCustomerID.Text = dr.Cells[6].Value.ToString();
                        SalesReturn.Instance.txtCustomerName.Text = dr.Cells[7].Value.ToString();
                        SalesReturn.Instance.txtGrandTotal.Text = dr.Cells[8].Value.ToString();

                        SalesReturn.Instance.btnSave.Enabled = false;
                        SalesReturn.Instance.DataGridView1.Enabled = true;
                        SalesReturn.Instance.btnAdd.Enabled = false;
                        SalesReturn.Instance.btnRemove.Enabled = false;
                        SalesReturn.Instance.lblSet.Text = "Not Allowed";
                        SalesReturn.Instance.pnlCalc.Enabled = false;
                        SalesReturn.Instance.btnDelete.Enabled = true;
                        SalesReturn.Instance.btnSelection.Enabled = false;
                        

                        using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                        {
                            con.Open();
                            string sql = "SELECT RTRIM(ProductCode), RTRIM(ProductName), RTRIM(SalesReturn_Join.Barcode), Price, Qty, DiscountPer, SalesReturn_Join.Discount, VATPer, SalesReturn_Join.VAT, ReturnQty, TotalAmount, ProductID, SalesReturn_Join.CostPrice, Margin FROM SalesReturn_Join INNER JOIN SalesReturn ON SalesReturn_Join.SalesReturnID = SalesReturn.SR_ID INNER JOIN Product ON Product.PID = SalesReturn_Join.ProductID WHERE SR_ID = @sr_id";
                            using (SqlCommand cmd = new SqlCommand(sql, con))
                            {
                                cmd.Parameters.AddWithValue("@sr_id", dr.Cells[0].Value);
                                using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                                {
                                    SalesReturn.Instance.DataGridView1.Rows.Clear();
                                    while (rdr.Read())
                                    {
                                        SalesReturn.Instance.DataGridView1.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13]);
                                    }
                                }
                            }
                        }
                        SalesReturn.Instance.Show();
                        this.Close();

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
                SqlCommand cmd = new SqlCommand("SELECT SR_ID, RTRIM(SRNo),SalesReturn.Date,SalesID,RTRIM(InvoiceNo),InvoiceDate, RTRIM(Customer.CustomerID),RTRIM(Name),RTRIM(SalesReturn.GrandTotal) FROM InvoiceInfo,SalesReturn,Customer where InvoiceInfo.Inv_ID=SalesReturn.SalesID and Customer.ID=InvoiceInfo.CustomerID and SalesReturn.Date between @d1 and @d2 order by SalesReturn.Date", con);
                cmd.Parameters.Add("@d1", SqlDbType.DateTime, 30, "Date").Value = dtpDateFrom.Value.Date;
                cmd.Parameters.Add("@d2", SqlDbType.DateTime, 30, "Date").Value = dtpDateTo.Value;
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dgw.Rows.Clear();
                while (rdr.Read() == true)
                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8]);
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void Reset()
        {
            txtCustomerName.Text = "";
            dtpDateFrom.Text = DateTime.Today.ToString();
            dtpDateTo.Text = DateTime.Today.ToString();
            Getdata();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

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
        private void txtSupplierName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT SR_ID, RTRIM(SRNo), SalesReturn.Date, SalesID, RTRIM(InvoiceNo), InvoiceDate, RTRIM(Customer.CustomerID), RTRIM(Name), RTRIM(SalesReturn.GrandTotal) " +
                                   "FROM InvoiceInfo, SalesReturn, Customer " +
                                   "WHERE InvoiceInfo.Inv_ID = SalesReturn.SalesID " +
                                   "AND Customer.ID = InvoiceInfo.CustomerID " +
                                   "AND Name LIKE @customerName " +
                                   "ORDER BY SalesReturn.Date";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@customerName", "%" + txtCustomerName.Text + "%");
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8]);
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
