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
    public partial class QuotationRecord : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public QuotationRecord()
        {
            InitializeComponent();
            Getdata();
            fillQuotationNo();
            dgw.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dgw_RowPostPaint);
            cmbQuotationNo.SelectedIndexChanged += new EventHandler(cmbOrderNo_SelectedIndexChanged);
            txtCustomerName.TextChanged += new EventHandler(txtCustomerName_TextChanged);
            cmbQuotationNo.Format += new ListControlConvertEventHandler(cmbQuotationNo_Format);
            dgw.MouseDoubleClick += new MouseEventHandler(dgw_MouseDoubleClick);
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void Getdata()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT Q_ID, RTRIM(QuotationNo), Date, Customer.ID, RTRIM(Customer.CustomerID), RTRIM(Name), RTRIM(ContactNo), GrandTotal, RTRIM(quotation.Remarks) FROM Customer, quotation WHERE Customer.ID = quotation.CustomerID ORDER BY Date";
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
        public void fillQuotationNo()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    SqlDataAdapter adp = new SqlDataAdapter("SELECT DISTINCT RTRIM(QuotationNo) FROM quotation", con);
                    DataSet ds = new DataSet("ds");
                    adp.Fill(ds);
                    DataTable dtable = ds.Tables[0];
                    cmbQuotationNo.Items.Clear();
                    foreach (DataRow drow in dtable.Rows)
                    {
                        cmbQuotationNo.Items.Add(drow[0].ToString());
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
            cmbQuotationNo.Text = string.Empty;
            txtCustomerName.Text = string.Empty;
            fillQuotationNo(); 
            dtpDateFrom.Value = DateTime.Today;
            dtpDateTo.Value = DateTime.Today;
            Getdata();
        }
        private void dgw_RowPostPaint(object sender,DataGridViewRowPostPaintEventArgs e)
        {

/*                string strRowNumber = (e.RowIndex + 1).ToString();
                using (Graphics g = e.Graphics)
                {
                    Font font = this.Font;
                    SizeF size = g.MeasureString(strRowNumber, font);

                    if (dgw.RowHeadersWidth < (int)(size.Width + 20))
                    {
                        dgw.RowHeadersWidth = (int)(size.Width + 20);
                    }

                    Brush b = SystemBrushes.ControlText;
                    g.DrawString(strRowNumber, font, b, e.RowBounds.Left + 15, e.RowBounds.Top + ((e.RowBounds.Height - size.Height) / 2));
                }
            
*/
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
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

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            ExportExcel(dgw);
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
                try
                {
                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT Q_ID, RTRIM(QuotationNo), Date, Customer.ID, RTRIM(Customer.CustomerID), RTRIM(Name), RTRIM(ContactNo), GrandTotal, RTRIM(quotation.Remarks) " +
                            "FROM Customer, quotation " +
                            "WHERE Customer.ID = quotation.CustomerID AND Date BETWEEN @d1 AND @d2 " +
                            "ORDER BY Date", con))
                        {
                            cmd.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                            cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;

                            using (SqlDataReader rdr = cmd.ExecuteReader())
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
        private void cmbOrderNo_SelectedIndexChanged(object sender, EventArgs e) {

            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT Q_ID, RTRIM(QuotationNo), Date, Customer.ID, RTRIM(Customer.CustomerID), RTRIM(Name), RTRIM(ContactNo), GrandTotal, RTRIM(quotation.Remarks) " +
                                   "FROM Customer, quotation " +
                                   "WHERE Customer.ID = quotation.CustomerID AND QuotationNo = @quotationNo " +
                                   "ORDER BY Date";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@quotationNo", cmbQuotationNo.Text);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
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
        private void txtCustomerName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT Q_ID, RTRIM(QuotationNo), Date, Customer.ID, RTRIM(Customer.CustomerID), RTRIM(Name), RTRIM(ContactNo), GrandTotal, RTRIM(quotation.Remarks) " +
                                   "FROM Customer, quotation " +
                                   "WHERE Customer.ID = quotation.CustomerID AND Name LIKE @customerName " +
                                   "ORDER BY Date";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Use parameters to prevent SQL injection
                        cmd.Parameters.AddWithValue("@customerName", "%" + txtCustomerName.Text + "%");

                        using (SqlDataReader rdr = cmd.ExecuteReader())
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
        private void cmbQuotationNo_Format(object sender, ListControlConvertEventArgs e)
        {
            if (e.DesiredType == typeof(string))
            {
                e.Value = e.Value.ToString().Trim();
            }

        }
        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (dgw.Rows.Count > 0)
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];
                    Quotation frmQuotation = new Quotation();   

                    frmQuotation.Show();

                    frmQuotation.txtID.Text = dr.Cells[0].Value.ToString();
                    frmQuotation.txtQuotationNo.Text = dr.Cells[1].Value.ToString();
                    frmQuotation.dtpQuotationDate.Text = dr.Cells[2].Value.ToString();
                    frmQuotation.txtCustomerID.Text = dr.Cells[4].Value.ToString();
                    frmQuotation.txtCID.Text = dr.Cells[3].Value.ToString();
                    frmQuotation.txtCustomerID.Text = dr.Cells[4].Value.ToString();
                    frmQuotation.txtCustomerName.Text = dr.Cells[5].Value.ToString();
                    frmQuotation.txtContactNo.Text = dr.Cells[6].Value.ToString();
                    frmQuotation.txtGrandTotal.Text = dr.Cells[7].Value.ToString();
                    frmQuotation.txtRemarks.Text = dr.Cells[8].Value.ToString();
                    frmQuotation.btnSave.Enabled = false;
                   // frmQuotation.btnUpdate.Enabled = true;
                    frmQuotation.btnPrint.Enabled = true;
                    frmQuotation.btnDelete.Enabled = true;
                    frmQuotation.lblSet.Text = "Not Allowed";
                    frmQuotation.btnAdd.Enabled = false;
                    

                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();
                        string sql = "SELECT RTRIM(ProductCode), RTRIM(ProductName), Quotation_Join.Cost, Quotation_Join.Qty, Quotation_Join.Amount, Quotation_Join.DiscountPer, Quotation_Join.Discount, Quotation_Join.VATPer, Quotation_Join.VAT, Quotation_Join.TotalAmount, Product.PID FROM quotation, Quotation_Join, Product WHERE quotation.Q_ID = Quotation_Join.QuotationID AND Product.PID = Quotation_Join.ProductID AND quotation.Q_ID = @d1";
                        using (SqlCommand cmd = new SqlCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@d1", dr.Cells[0].Value.ToString());
                            using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                            {
                                frmQuotation.DataGridView1.Rows.Clear();
                                while (rdr.Read())
                                {
                                    frmQuotation.DataGridView1.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10]);
                                }
                            }
                        }
                    }

                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();
                        string ct = "SELECT RTRIM(CustomerType) FROM Customer WHERE ID = @id";
                        using (SqlCommand cmd = new SqlCommand(ct, con))
                        {
                            cmd.Parameters.AddWithValue("@id", dr.Cells[3].Value.ToString());
                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                if (rdr.Read())
                                {
                                    frmQuotation.txtCustomerType.Text = rdr.GetValue(0).ToString();
                                }
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

        private void GroupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void QuotationRecord_Load(object sender, EventArgs e)
        {

        }
    }
}
