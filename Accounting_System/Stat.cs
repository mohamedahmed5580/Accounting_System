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
    public partial class Stat : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public Stat()
        {
            InitializeComponent();
            Getdata();
            dgw.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dgw_RowPostPaint);

        }

        private void Stat_Load(object sender, EventArgs e)
        {

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
        private void Reset()
        {
            dtpDateFrom.Value = DateTime.Today;
            dtpDateTo.Value = DateTime.Today;
            dgw.Rows.Clear();
            txtcost.Text = string.Empty;
            TextBox1.Text = string.Empty;
            TextBox2.Text = string.Empty;
            TextBox5.Text = string.Empty;

            Getdata();
        }
        private void Getdata()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Invoice_Product.InvoiceID, Invoice_Product.ProductID, Product.ProductName, Invoice_Product.Barcode, Invoice_Product.Qty, Invoice_Product.CostPrice, Invoice_Product.TotalAmount, Invoice_Payment.PaymentDate FROM Invoice_Product INNER JOIN Invoice_Payment ON Invoice_Product.InvoiceID = Invoice_Payment.InvoiceID INNER JOIN Product ON Invoice_Product.ProductID = Product.PID", con))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], (decimal)rdr[5] * (decimal)rdr[4], rdr[6], rdr[7]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }


        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            ExportExcel(dgw);
        }
        private void dgw_RowPostPaint(object sender,DataGridViewRowPostPaintEventArgs e)
        {
            try
            {
                string strRowNumber = (e.RowIndex + 1).ToString();
                SizeF size = e.Graphics.MeasureString(strRowNumber, dgw.Font);

                if (dgw.RowHeadersWidth < Convert.ToInt32(size.Width + 20))
                {
                    dgw.RowHeadersWidth = Convert.ToInt32(size.Width + 20);
                }

                using (Brush b = new SolidBrush(SystemColors.ControlText)) // Use SolidBrush with SystemColors.ControlText
                {
                    e.Graphics.DrawString(strRowNumber, dgw.Font, b, e.RowBounds.Left + 15, e.RowBounds.Top + (e.RowBounds.Height - size.Height) / 2);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void txtProductName_KeyDown(object sender, KeyEventArgs e) {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                   
                }
            }
            catch (Exception ex)
            {
                
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
                    using (SqlCommand cmd = new SqlCommand(
                        "Select Invoice_Product.InvoiceID, Invoice_Product.ProductID, Product.ProductName, " +
                        "Invoice_Product.Barcode, Invoice_Product.Qty, Invoice_Product.CostPrice, " +
                        "Invoice_Product.TotalAmount, Invoice_Payment.PaymentDate " +
                        "from Invoice_Product, Invoice_Payment, Product " +
                        "where Invoice_Product.InvoiceID = Invoice_Payment.InvoiceID " +
                        "and Invoice_Product.ProductID = Product.PID " +
                        "and Invoice_Payment.PaymentDate >= @d1 " +
                        "and Invoice_Payment.PaymentDate < @d2", con))
                    {
                        cmd.Parameters.AddWithValue("@d1", dtpDateFrom.Value.Date);
                        cmd.Parameters.AddWithValue("@d2", dtpDateTo.Value.Date.AddDays(1));
                        cmd.CommandTimeout = 0;

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], (double)rdr[5] * (double)rdr[4], rdr[6], rdr[7]);
                            }
                        }
                    }
                }

                double total1 = 0;
                double total2 = 0;
                double total3 = 0;

                foreach (DataGridViewRow row in dgw.Rows)
                {
                    if (row.Cells[4].Value != null && double.TryParse(row.Cells[4].Value.ToString(), out double celv) &&
                        double.TryParse(row.Cells[5].Value.ToString(), out double celv0) &&
                        double.TryParse(row.Cells[6].Value.ToString(), out double celv1))
                    {
                        total1 += celv;
                        total2 += celv1;
                        total3 += celv0;
                    }
                }

                TextBox1.Text = total1.ToString();
                TextBox2.Text = total2.ToString();
                txtcost.Text = total3.ToString();
                TextBox5.Text = (total2 - total3).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
