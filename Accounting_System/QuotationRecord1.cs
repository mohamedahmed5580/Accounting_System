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
using Excel = Microsoft.Office.Interop.Excel;

namespace Accounting_System
{
    public partial class QuotationRecord1 : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public QuotationRecord1()
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
        public void Getdata()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "Select RTRIM(QuotationNo), Date,RTRIM(Customer.CustomerID),RTRIM(Name),RTRIM(Product.ProductCode),RTRIM(ProductName),Cost, Qty, DiscountPer, Quotation_Join.Discount, VATPer, Quotation_Join.VAT, TotalAmount, GrandTotal, RTRIM(quotation.Remarks) from Customer,quotation,quotation_Join,Product where Customer.ID=quotation.CustomerID and Quotation.Q_ID=Quotation_Join.QuotationID and Product.PID=Quotation_Join.ProductID order by Date";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(
                                    rdr[0].ToString(),
                                    rdr[1],
                                    rdr[2].ToString(),
                                    rdr[3].ToString(),
                                    rdr[4].ToString(),
                                    rdr[5].ToString(),
                                    rdr[6],
                                    rdr[7],
                                    rdr[8],
                                    rdr[9],
                                    rdr[10],
                                    rdr[11],
                                    rdr[12],
                                    rdr[13],
                                    rdr[14].ToString()
                                );
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


        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        public void fillQuotationNo()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlDataAdapter adp = new SqlDataAdapter())
                    {
                        adp.SelectCommand = new SqlCommand("SELECT DISTINCT RTRIM(QuotationNo) FROM quotation", con);
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dgw_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            string strRowNumber = (e.RowIndex + 1).ToString();
            SizeF size = e.Graphics.MeasureString(strRowNumber, this.Font);

            if (dgw.RowHeadersWidth < Convert.ToInt32(size.Width + 20))
            {
                dgw.RowHeadersWidth = Convert.ToInt32(size.Width + 20);
            }

            Brush b = SystemBrushes.ControlText;
            e.Graphics.DrawString(
                strRowNumber,
                this.Font,
                b,
                e.RowBounds.Location.X + 15,
                e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2)
            );
        }
        public void Reset()
        {
            cmbQuotationNo.Text = "";
            txtCustomerName.Text = "";
            fillQuotationNo();
            dtpDateFrom.Text = DateTime.Today.ToString();
            dtpDateTo.Text = DateTime.Today.ToString();
            Getdata();
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
                    string query = "SELECT RTRIM(QuotationNo), Date, RTRIM(Customer.CustomerID), RTRIM(Name), RTRIM(Product.ProductCode), RTRIM(ProductName), Cost, Qty, DiscountPer, Quotation_Join.Discount, VATPer, Quotation_Join.VAT, TotalAmount, GrandTotal, RTRIM(quotation.Remarks) " +
                                   "FROM Customer, quotation, quotation_Join, Product " +
                                   "WHERE Customer.ID = quotation.CustomerID " +
                                   "AND quotation.Q_ID = Quotation_Join.QuotationID " +
                                   "AND Product.PID = Quotation_Join.ProductID " +
                                   "AND Date BETWEEN @d1 AND @d2 " +
                                   "ORDER BY Date";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                        cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(
                                    rdr[0].ToString(),
                                    rdr[1],
                                    rdr[2].ToString(),
                                    rdr[3].ToString(),
                                    rdr[4].ToString(),
                                    rdr[5].ToString(),
                                    rdr[6],
                                    rdr[7],
                                    rdr[8],
                                    rdr[9],
                                    rdr[10],
                                    rdr[11],
                                    rdr[12],
                                    rdr[13],
                                    rdr[14].ToString()
                                );
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
        private void cmbOrderNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT RTRIM(QuotationNo), Date, RTRIM(Customer.CustomerID), RTRIM(Name), RTRIM(Product.ProductCode), RTRIM(ProductName), Cost, Qty, DiscountPer, Quotation_Join.Discount, VATPer, Quotation_Join.VAT, TotalAmount, GrandTotal, RTRIM(quotation.Remarks) " +
                                   "FROM Customer, quotation, quotation_Join, Product " +
                                   "WHERE Customer.ID = quotation.CustomerID " +
                                   "AND quotation.Q_ID = Quotation_Join.QuotationID " +
                                   "AND Product.PID = Quotation_Join.ProductID " +
                                   "AND QuotationNo = @QuotationNo " +
                                   "ORDER BY Date";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@QuotationNo", cmbQuotationNo.Text);

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(
                                    rdr[0].ToString(),
                                    rdr[1],
                                    rdr[2].ToString(),
                                    rdr[3].ToString(),
                                    rdr[4].ToString(),
                                    rdr[5].ToString(),
                                    rdr[6],
                                    rdr[7],
                                    rdr[8],
                                    rdr[9],
                                    rdr[10],
                                    rdr[11],
                                    rdr[12],
                                    rdr[13],
                                    rdr[14].ToString()
                                );
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
        private void txtCustomerName_TextChanged(object sender, EventArgs e) {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT RTRIM(QuotationNo), Date, RTRIM(Customer.CustomerID), RTRIM(Name), RTRIM(Product.ProductCode), RTRIM(ProductName), Cost, Qty, DiscountPer, Quotation_Join.Discount, VATPer, Quotation_Join.VAT, TotalAmount, GrandTotal, RTRIM(quotation.Remarks) " +
                                   "FROM Customer, quotation, quotation_Join, Product " +
                                   "WHERE Customer.ID = quotation.CustomerID " +
                                   "AND quotation.Q_ID = Quotation_Join.QuotationID " +
                                   "AND Product.PID = Quotation_Join.ProductID " +
                                   "AND Name LIKE @CustomerName " +
                                   "ORDER BY Date";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CustomerName", "%" + txtCustomerName.Text + "%");

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(
                                    rdr[0].ToString(),
                                    rdr[1],
                                    rdr[2].ToString(),
                                    rdr[3].ToString(),
                                    rdr[4].ToString(),
                                    rdr[5].ToString(),
                                    rdr[6],
                                    rdr[7],
                                    rdr[8],
                                    rdr[9],
                                    rdr[10],
                                    rdr[11],
                                    rdr[12],
                                    rdr[13],
                                    rdr[14].ToString()
                                );
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
        private void cmbQuotationNo_Format(object sender, ListControlConvertEventArgs e) {
            if (e.DesiredType == typeof(string))
            {
                e.Value = e.Value.ToString().Trim();
            }

        }
        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
/*            try
            {
                if (dgw.Rows.Count > 0)
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];
                    Quotation frm = new Quotation();
                    frm.Show();

                    frm.txtID.Text = dr.Cells[0].Value.ToString();
                    frm.txtQuotationNo.Text = dr.Cells[1].Value.ToString();
                    frm.dtpQuotationDate.Text = dr.Cells[2].Value.ToString();
                    frm.txtCustomerID.Text = dr.Cells[4].Value.ToString();
                    frm.txtCID.Text = dr.Cells[3].Value.ToString();
                    frm.txtCustomerID.Text = dr.Cells[4].Value.ToString();
                    frm.txtCustomerName.Text = dr.Cells[5].Value.ToString();
                    frm.txtContactNo.Text = dr.Cells[6].Value.ToString();
                    frm.txtGrandTotal.Text = dr.Cells[7].Value.ToString();
                    frm.txtRemarks.Text = dr.Cells[8].Value.ToString();
                    frm.btnSave.Enabled = false;
                    // frm.btnUpdate.Enabled = true;
                    frm.btnPrint.Enabled = true;
                    frm.btnDelete.Enabled = true;
                    frm.lblSet.Text = "Not Allowed";
                    frm.btnAdd.Enabled = false;

                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();
                        string sql = "SELECT RTRIM(ProductCode), RTRIM(ProductName), Quotation_Join.Cost, Quotation_Join.Qty, Quotation_Join.Amount, Quotation_Join.DiscountPer, Quotation_Join.Discount, Quotation_Join.VATPer, Quotation_Join.VAT, Quotation_Join.TotalAmount, Product.PID " +
                                     "FROM quotation, Quotation_Join, Product " +
                                     "WHERE quotation.Q_ID = Quotation_Join.QuotationID AND Product.PID = Quotation_Join.ProductID AND quotation.Q_ID = @d1";
                        using (SqlCommand cmd = new SqlCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@d1", dr.Cells[0].Value.ToString());
                            using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                            {
                                frm.DataGridView1.Rows.Clear();
                                while (rdr.Read())
                                {
                                    frm.DataGridView1.Rows.Add(
                                        rdr[0].ToString(),
                                        rdr[1].ToString(),
                                        rdr[2].ToString(),
                                        rdr[3].ToString(),
                                        rdr[4].ToString(),
                                        rdr[5].ToString(),
                                        rdr[6].ToString(),
                                        rdr[7].ToString(),
                                        rdr[8].ToString(),
                                        rdr[9].ToString(),
                                        rdr[10].ToString()
                                    );
                                }
                            }
                        }
                    }

                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();
                        string ct = "SELECT RTRIM(CustomerType) FROM Customer WHERE ID = @CustomerID";
                        using (SqlCommand cmd = new SqlCommand(ct, con))
                        {
                            cmd.Parameters.AddWithValue("@CustomerID", dr.Cells[3].Value);
                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                if (rdr.Read())
                                {
                                    frm.txtCustomerType.Text = rdr.GetValue(0).ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/

        }

        private void QuotationRecord1_Load(object sender, EventArgs e)
        {

        }
    }
}
