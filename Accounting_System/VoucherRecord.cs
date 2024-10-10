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
    public partial class VoucherRecord : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public VoucherRecord()
        {
            InitializeComponent();
            dgw.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dgw_RowPostPaint);
            cmbVoucherNo.SelectedIndexChanged += new EventHandler(cmbBillNo_SelectedIndexChanged);
            dgw.MouseDoubleClick += new MouseEventHandler(dgw_MouseDoubleClick);

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void fillVoucherNo()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT DISTINCT RTRIM(VoucherNo) FROM Voucher";
                    using (SqlDataAdapter adp = new SqlDataAdapter(query, con))
                    {
                        DataSet ds = new DataSet("ds");
                        adp.Fill(ds);
                        DataTable dtable = ds.Tables[0];

                        cmbVoucherNo.Items.Clear();
                        foreach (DataRow drow in dtable.Rows)
                        {
                            cmbVoucherNo.Items.Add(drow[0].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void GetData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT RTRIM(Voucher.Id) AS [Voucher ID], RTRIM(VoucherNo) AS [Voucher No.], " +
                                   "CONVERT(DateTime, Date, 103) AS [Voucher Date], RTRIM(Name) AS [Name], " +
                                   "RTRIM(Details) AS [Details], RTRIM(Voucher.GrandTotal) AS [Grand Total] " +
                                   "FROM Voucher ORDER BY Date";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataAdapter myDA = new SqlDataAdapter(cmd);
                        DataSet myDataSet = new DataSet();
                        myDA.Fill(myDataSet, "Voucher");
                        dgw.DataSource = myDataSet.Tables["Voucher"].DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void VoucherRecord_Load(object sender, EventArgs e)
        {
            GetData();
            fillVoucherNo();
        }
        public void Reset()
        {
            cmbVoucherNo.Text = string.Empty;
            dtpDateFrom.Value = DateTime.Today;
            dtpDateTo.Value = DateTime.Today;
            GetData();
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

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            ExportExcel(dgw);
        }
        private void dgw_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            // Calculate the row number
            string strRowNumber = (e.RowIndex + 1).ToString();

            // Measure the width of the row number text
            SizeF size = e.Graphics.MeasureString(strRowNumber, this.Font);

            // Adjust the row header width if necessary
            if (dgw.RowHeadersWidth < (int)(size.Width + 20))
            {
                dgw.RowHeadersWidth = (int)(size.Width + 20);
            }

            // Create a brush for drawing the text
            Brush b = SystemBrushes.ControlText;

            // Draw the row number text
            e.Graphics.DrawString(strRowNumber, this.Font, b,
                e.RowBounds.Location.X + 15,
                e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2));
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT RTRIM(Voucher.Id) AS [Voucher ID], RTRIM(VoucherNo) AS [Voucher No.], " +
                                   "CONVERT(DateTime, Date, 103) AS [Voucher Date], RTRIM(Name) AS [Name], " +
                                   "RTRIM(Details) AS [Details], RTRIM(Voucher.GrandTotal) AS [Grand Total] " +
                                   "FROM Voucher WHERE Date BETWEEN @d1 AND @d2 ORDER BY Date";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                        cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value;

                        SqlDataAdapter myDA = new SqlDataAdapter(cmd);
                        DataSet myDataSet = new DataSet();
                        myDA.Fill(myDataSet, "Voucher");

                        dgw.DataSource = myDataSet.Tables["Voucher"].DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void cmbBillNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT RTRIM(Voucher.Id) AS [Voucher ID], RTRIM(VoucherNo) AS [Voucher No.], " +
                                   "CONVERT(DateTime, Date, 103) AS [Voucher Date], RTRIM(Name) AS [Name], " +
                                   "RTRIM(Details) AS [Details], RTRIM(Voucher.GrandTotal) AS [Grand Total] " +
                                   "FROM Voucher WHERE VoucherNo = @voucherNo ORDER BY Date";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@voucherNo", cmbVoucherNo.Text);

                        SqlDataAdapter myDA = new SqlDataAdapter(cmd);
                        DataSet myDataSet = new DataSet();
                        myDA.Fill(myDataSet, "Voucher");

                        dgw.DataSource = myDataSet.Tables["Voucher"].DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (dgw.Rows.Count > 0)
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];
                    Voucher frmVoucher = new Voucher();
                    this.Close();
                    

                    // Use column names or indices to retrieve cell values
                    frmVoucher.txtVoucherID.Text = dr.Cells[0].Value.ToString();
                    frmVoucher.txtVoucherNo.Text = dr.Cells[1].Value.ToString();
                    frmVoucher.dtpDate.Text = dr.Cells[2].Value.ToString();
                    frmVoucher.txtName.Text = dr.Cells[3].Value.ToString();
                    frmVoucher.txtDetails.Text = dr.Cells[4].Value.ToString();
                    frmVoucher.txtGrandTotal.Text = dr.Cells[5].Value.ToString();

                    frmVoucher.btnSave.Enabled = false;
                    frmVoucher.btnDelete.Enabled = true;
                    frmVoucher.btnUpdate.Enabled = true;
                    frmVoucher.btnPrint.Enabled = true;
                    frmVoucher.btnRemove.Enabled = false;

                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();
                        string sql = "SELECT RTRIM(Particulars), RTRIM(Amount), RTRIM(Note) " +
                                     "FROM Voucher INNER JOIN Voucher_OtherDetails ON Voucher.Id = Voucher_OtherDetails.VoucherID " +
                                     "WHERE Voucher.ID = @voucherID";

                        using (SqlCommand cmd = new SqlCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@voucherID", dr.Cells[0].Value);

                            using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                            {
                                frmVoucher.DataGridView1.Rows.Clear();
                                while (rdr.Read())
                                {
                                    frmVoucher.DataGridView1.Rows.Add(rdr[0], rdr[1], rdr[2]);
                                }
                            }
                        }
                    }
                    frmVoucher.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
