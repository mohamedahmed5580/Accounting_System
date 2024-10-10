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
    public partial class PurchaseReturnScreen : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public PurchaseReturnScreen()
        {
            InitializeComponent();
        }
        private void PurchaseReturnScreen_Load(object sender, EventArgs e)
        {
        }

        public void Getdata()
        {
            try
            {
                con.Open();
                string query = @"
            SELECT PR_ID, RTRIM(PRNo), PurchaseReturn.Date, RTRIM(PurchaseID), 
                   RTRIM(InvoiceNo), Stock.Date, RTRIM(Supplier.SupplierID), 
                   RTRIM(Name), RTRIM(PurchaseReturn.SubTotal), 
                   RTRIM(PurchaseReturn.DiscPer), RTRIM(PurchaseReturn.Discount), 
                   RTRIM(PurchaseReturn.VATPer), RTRIM(PurchaseReturn.VATAmt), 
                   RTRIM(PurchaseReturn.Total), RTRIM(PurchaseReturn.RoundOff), 
                   RTRIM(PurchaseReturn.GrandTotal) 
            FROM Stock 
            INNER JOIN PurchaseReturn ON Stock.ST_ID = PurchaseReturn.PurchaseID 
            INNER JOIN Supplier ON Supplier.ID = Stock.SupplierID 
            ORDER BY PurchaseReturn.Date";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    dgw.Rows.Clear();
                    while (rdr.Read())
                    {
                        dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7],
                                     rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                string query = @"
            SELECT PR_ID, RTRIM(PRNo), PurchaseReturn.Date, RTRIM(PurchaseID), 
                   RTRIM(InvoiceNo), Stock.Date, RTRIM(Supplier.SupplierID), 
                   RTRIM(Name), RTRIM(PurchaseReturn.SubTotal), 
                   RTRIM(PurchaseReturn.DiscPer), RTRIM(PurchaseReturn.Discount), 
                   RTRIM(PurchaseReturn.VATPer), RTRIM(PurchaseReturn.VATAmt), 
                   RTRIM(PurchaseReturn.Total), RTRIM(PurchaseReturn.RoundOff), 
                   RTRIM(PurchaseReturn.GrandTotal) 
            FROM Stock 
            INNER JOIN PurchaseReturn ON Stock.ST_ID = PurchaseReturn.PurchaseID 
            INNER JOIN Supplier ON Supplier.ID = Stock.SupplierID 
            WHERE PurchaseReturn.Date BETWEEN @d1 AND @d2 
            ORDER BY PurchaseReturn.Date";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                    cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;

                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        dgw.Rows.Clear();
                        while (rdr.Read())
                        {
                            dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7],
                                         rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void txtSupplierName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                string query = @"
            SELECT PR_ID, RTRIM(PRNo), PurchaseReturn.Date, RTRIM(PurchaseID), 
                   RTRIM(InvoiceNo), Stock.Date, RTRIM(Supplier.SupplierID), 
                   RTRIM(Name), RTRIM(PurchaseReturn.SubTotal), 
                   RTRIM(PurchaseReturn.DiscPer), RTRIM(PurchaseReturn.Discount), 
                   RTRIM(PurchaseReturn.VATPer), RTRIM(PurchaseReturn.VATAmt), 
                   RTRIM(PurchaseReturn.Total), RTRIM(PurchaseReturn.RoundOff), 
                   RTRIM(PurchaseReturn.GrandTotal) 
            FROM Stock 
            INNER JOIN PurchaseReturn ON Stock.ST_ID = PurchaseReturn.PurchaseID 
            INNER JOIN Supplier ON Supplier.ID = Stock.SupplierID 
            WHERE Name LIKE @SupplierName 
            ORDER BY PurchaseReturn.Date";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SupplierName", "%" + txtSupplierName.Text + "%");

                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        dgw.Rows.Clear();
                        while (rdr.Read())
                        {
                            dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7],
                                         rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
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
            e.Graphics.DrawString(strRowNumber, this.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + (e.RowBounds.Height - size.Height) / 2);
        }

        public void Reset()
        {
            txtSupplierName.Clear();
            dtpDateFrom.Value = DateTime.Today;
            dtpDateTo.Value = DateTime.Today;
            Getdata();
        }

        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (dgw.SelectedRows.Count > 0)
                {
                    if (lblSet.Text == "PR")
                    {
                        DataGridViewRow dr = dgw.SelectedRows[0];
                        PurchaseReturn frmPurchaseReturn = PurchaseReturn.instance;

                        frmPurchaseReturn.txtPRID.Text = dr.Cells[0].Value.ToString();
                        frmPurchaseReturn.txtPRNO.Text = dr.Cells[1].Value.ToString();
                        frmPurchaseReturn.dtpPRDate.Text = dr.Cells[2].Value.ToString();
                        frmPurchaseReturn.txtPurchaseID.Text = dr.Cells[3].Value.ToString();
                        frmPurchaseReturn.txtPurchaseInvoiceNo.Text = dr.Cells[4].Value.ToString();
                        frmPurchaseReturn.dtpPurchaseDate.Text = dr.Cells[5].Value.ToString();
                        frmPurchaseReturn.txtSupplierID.Text = dr.Cells[6].Value.ToString();
                        frmPurchaseReturn.txtSupplierName.Text = dr.Cells[7].Value.ToString();
                        frmPurchaseReturn.txtSubTotal.Text = dr.Cells[8].Value.ToString();
                        frmPurchaseReturn.txtDiscPer.Text = dr.Cells[9].Value.ToString();
                        frmPurchaseReturn.txtDisc.Text = dr.Cells[10].Value.ToString();
                        frmPurchaseReturn.txtVatPer.Text = dr.Cells[11].Value.ToString();
                        frmPurchaseReturn.txtVATAmt.Text = dr.Cells[12].Value.ToString();
                        frmPurchaseReturn.txtTotal.Text = dr.Cells[13].Value.ToString();
                        frmPurchaseReturn.txtRoundOff.Text = dr.Cells[14].Value.ToString();
                        frmPurchaseReturn.txtGrandTotal.Text = dr.Cells[15].Value.ToString();

                        frmPurchaseReturn.btnSave.Enabled = false;
                        frmPurchaseReturn.DataGridView1.Enabled = true;
                        frmPurchaseReturn.btnAdd.Enabled = false;
                        frmPurchaseReturn.btnRemove.Enabled = false;
                        frmPurchaseReturn.lblSet.Text = "Not Allowed";
                        frmPurchaseReturn.btnDelete.Enabled = true;
                        frmPurchaseReturn.btnSelection.Enabled = false;

                        con.Open();
                        string query = @"
                    SELECT PurchaseReturn_Join.ProductID, RTRIM(Product.ProductCode), 
                           RTRIM(Product.ProductName), RTRIM(PurchaseReturn_Join.Barcode), 
                           PurchaseReturn_Join.Qty, PurchaseReturn_Join.Price, 
                           PurchaseReturn_Join.ReturnQty, PurchaseReturn_Join.TotalAmount 
                    FROM PurchaseReturn_Join 
                    INNER JOIN PurchaseReturn ON PurchaseReturn_Join.PurchaseReturnID = PurchaseReturn.PR_ID 
                    INNER JOIN Product ON Product.PID = PurchaseReturn_Join.ProductID 
                    WHERE PR_ID = @PR_ID";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@PR_ID", dr.Cells[0].Value);

                            using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                            {
                                frmPurchaseReturn.DataGridView1.Rows.Clear();
                                while (rdr.Read())
                                {
                                    frmPurchaseReturn.DataGridView1.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3],
                                                                              rdr[4], rdr[5], rdr[6], rdr[7]);
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                this.Close();
            }
        }


        private void Panel2_Paint(object sender, PaintEventArgs e)
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

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
