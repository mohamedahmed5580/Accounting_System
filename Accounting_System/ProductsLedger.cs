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
    public partial class ProductsLedger : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public ProductsLedger()
        {
            InitializeComponent();
            Getdata();
            dgw.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dgw_RowPostPaint);
            txtProductName.KeyDown += new KeyEventHandler(txtProductName_KeyDown);
            txtProductName.TextChanged += new EventHandler(txtProductName_TextChanged);
            TextBox3.TextChanged += new EventHandler(TextBox3_TextChanged);
        }

        private void ProductsLedger_Load(object sender, EventArgs e)
        {

        }
        private void Reset()
        {
            txtProductName.Text = "";
            TextBox3.Text = "";
            TextBox1.Text = "";
            TextBox2.Text = "";
            dtpDateFrom.Value = DateTime.Today;
            dtpDateTo.Value = DateTime.Today;
            dgw.Rows.Clear();
            Getdata(); // Ensure GetData() is defined elsewhere in your code
        }
        private void Getdata()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT Invoice_Product.InvoiceID, Invoice_Product.ProductID, Product.ProductName, Invoice_Product.Barcode, Invoice_Product.Qty, Invoice_Product.TotalAmount, Invoice_Payment.PaymentDate " +
                                   "FROM Invoice_Product " +
                                   "JOIN Invoice_Payment ON Invoice_Product.InvoiceID = Invoice_Payment.InvoiceID " +
                                   "JOIN Product ON Invoice_Product.ProductID = Product.PID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(
                                    rdr[0],
                                    rdr[1],
                                    rdr[2],
                                    rdr[3],
                                    rdr[4],
                                    rdr[5],
                                    rdr[6]
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

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }
        private void dgw_RowPostPaint(object sender,  DataGridViewRowPostPaintEventArgs e)
        {
            string strRowNumber = (e.RowIndex + 1).ToString();
            SizeF size = e.Graphics.MeasureString(strRowNumber, this.Font);

            if (dgw.RowHeadersWidth < (int)(size.Width + 20))
            {
                dgw.RowHeadersWidth = (int)(size.Width + 20);
            }

            Brush b = SystemBrushes.ControlText;
            e.Graphics.DrawString(
                strRowNumber,
                this.Font,
                b,
                e.RowBounds.Left + 15,
                e.RowBounds.Top + ((e.RowBounds.Height - size.Height) / 2)
            );
        }
        private void txtProductName_KeyDown(object sender, KeyEventArgs e) {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    // Your code here
                }
            }
            catch (Exception ex)
            {
                // Handle exception here
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnSalesLedger_Click(object sender, EventArgs e)
        {
            try
            {


                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Invoice_Product.InvoiceID, Invoice_Product.ProductID, Product.ProductName, Invoice_Product.Barcode, Invoice_Product.Qty, Invoice_Product.TotalAmount, Invoice_Payment.PaymentDate FROM Invoice_Product, Invoice_Payment, Product WHERE Invoice_Product.InvoiceID = Invoice_Payment.InvoiceID AND Invoice_Product.ProductID = Product.PID AND Invoice_Payment.PaymentDate >= @d1 AND Invoice_Payment.PaymentDate < @d2", con))
                    {
                        cmd.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                        cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date.AddDays(1);
                        cmd.CommandTimeout = 0;

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6]);
                            }
                        }
                    }
                }

                double total1 = 0;
                double total2 = 0;

                foreach (DataGridViewRow row in dgw.Rows)
                {
                    if (row.Cells[4].Value != null && double.TryParse(row.Cells[4].Value.ToString(), out double cellValue))
                    {
                        total1 += cellValue;
                    }

                    if (row.Cells[5].Value != null && double.TryParse(row.Cells[5].Value.ToString(), out double cellValue1))
                    {
                        total2 += cellValue1;
                    }
                }

                TextBox1.Text = total1.ToString();
                TextBox2.Text = total2.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void txtProductName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT Invoice_Product.InvoiceID, Invoice_Product.ProductID, Product.ProductName, Invoice_Product.Barcode, Invoice_Product.Qty, Invoice_Product.TotalAmount, Invoice_Payment.PaymentDate " +
                                   "FROM Invoice_Product, Invoice_Payment, Product " +
                                   "WHERE Invoice_Product.InvoiceID = Invoice_Payment.InvoiceID AND Invoice_Product.ProductID = Product.PID " +
                                   "AND Invoice_Payment.PaymentDate >= @d1 AND Invoice_Payment.PaymentDate < @d2 " +
                                   "AND Invoice_Product.Barcode LIKE '%' + @productName + '%' " +
                                   "ORDER BY Invoice_Product.Barcode";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                        cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date.AddDays(1);
                        cmd.Parameters.Add("@productName", SqlDbType.VarChar).Value = txtProductName.Text;
                        cmd.CommandTimeout = 0;

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6]);
                            }
                        }
                    }
                }

                double total1 = 0;
                double total2 = 0;

                foreach (DataGridViewRow row in dgw.Rows)
                {
                    if (row.Cells[4].Value != null && double.TryParse(row.Cells[4].Value.ToString(), out double cellValue))
                    {
                        total1 += cellValue;
                    }

                    if (row.Cells[5].Value != null && double.TryParse(row.Cells[5].Value.ToString(), out double cellValue1))
                    {
                        total2 += cellValue1;
                    }
                }

                TextBox1.Text = total1.ToString();
                TextBox2.Text = total2.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void TextBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT Invoice_Product.InvoiceID, Invoice_Product.ProductID, Product.ProductName, Invoice_Product.Barcode, Invoice_Product.Qty, Invoice_Product.TotalAmount, Invoice_Payment.PaymentDate " +
                                   "FROM Invoice_Product, Invoice_Payment, Product " +
                                   "WHERE Invoice_Product.InvoiceID = Invoice_Payment.InvoiceID " +
                                   "AND Invoice_Product.ProductID = Product.PID " +
                                   "AND Invoice_Payment.PaymentDate >= @d1 " +
                                   "AND Invoice_Payment.PaymentDate < @d2 " +
                                   "AND Product.ProductName LIKE '%' + @productName + '%' " +
                                   "ORDER BY Product.ProductName";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                        cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date.AddDays(1);
                        cmd.Parameters.Add("@productName", SqlDbType.VarChar).Value = TextBox3.Text;
                        cmd.CommandTimeout = 0;

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6]);
                            }
                        }
                    }
                }

                double total1 = 0;
                double total2 = 0;

                foreach (DataGridViewRow row in dgw.Rows)
                {
                    if (row.Cells[4].Value != null && double.TryParse(row.Cells[4].Value.ToString(), out double cellValue))
                    {
                        total1 += cellValue;
                    }

                    if (row.Cells[5].Value != null && double.TryParse(row.Cells[5].Value.ToString(), out double cellValue1))
                    {
                        total2 += cellValue1;
                    }
                }

                TextBox1.Text = total1.ToString();
                TextBox2.Text = total2.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
