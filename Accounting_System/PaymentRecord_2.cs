using DevExpress.Data.Browsing.Design;
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
    public partial class PaymentRecord_2 : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public PaymentRecord_2()
        {
            InitializeComponent();
            Getdata();
            dgw.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dgw_RowPostPaint);
            txtSupplierName.TextChanged += new EventHandler(txtSupplierName_TextChanged);
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
                    string query = @"SELECT TC_ID, RTRIM(TransactionID) AS TransactionID, Date, RTRIM(PaymentMode) AS PaymentMode,
                                     Customer.ID AS CustomerID, RTRIM(Customer.CustomerID) AS CustomerCustomerID,
                                     RTRIM(Name) AS CustomerName, Amount, RTRIM(Payment_2.Remarks) AS Remarks,
                                     RTRIM(Check_ID) AS CheckID, Check_Date, RTRIM(Bank) AS Bank, SalesMan_ID, 
                                     SalesMan_Name, SalesMan_Comession, SalesMan_ID_2
                              FROM Customer
                              INNER JOIN Payment_2 ON Customer.CustomerID = Payment_2.CustomerID
                              ORDER BY [Date]";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        dgw.Rows.Clear();
                        while (rdr.Read())
                        {
                            dgw.Rows.Add(
                                rdr["TC_ID"],
                                rdr["TransactionID"],
                                rdr["Date"],
                                rdr["PaymentMode"],
                                rdr["CustomerID"],
                                rdr["CustomerCustomerID"],
                                rdr["CustomerName"],
                                rdr["Amount"],
                                rdr["Remarks"],
                                rdr["CheckID"],
                                rdr["Check_Date"],
                                rdr["Bank"],
                                rdr["SalesMan_ID"],
                                rdr["SalesMan_Name"],
                                rdr["SalesMan_Comession"],
                                rdr["SalesMan_ID_2"]
                            );
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
            txtSupplierName.Text = string.Empty;
            dtpDateFrom.Value = DateTime.Today;
            dtpDateTo.Value = DateTime.Today;
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
                        string query = "SELECT TC_ID, RTRIM(TransactionID), Date, RTRIM(PaymentMode), Customer.ID, RTRIM(Customer.CustomerID), RTRIM(Name), Amount, RTRIM(Payment_2.Remarks), RTRIM(Check_ID), Check_Date, RTRIM(Bank), SalesMan_ID, SalesMan_Name, SalesMan_Comession, SalesMan_ID_2 " +
                                       "FROM Customer, Payment_2 " +
                                       "WHERE Customer.CustomerID = Payment_2.CustomerID " +
                                       "AND [Date] BETWEEN @d1 AND @d2 " +
                                       "ORDER BY [Date]";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                            cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value;

                            using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                            {
                                dgw.Rows.Clear();
                                while (rdr.Read())
                                {
                                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15]);
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
        private void dgw_RowPostPaint(object sender , DataGridViewRowPostPaintEventArgs e)
        {
/*            try
            {
                string strRowNumber = (e.RowIndex + 1).ToString();
                using (Graphics g = e.Graphics)
                {
                    // Measure the width of the row number
                    SizeF size = g.MeasureString(strRowNumber, this.Font);

                    // Adjust the width of the row header if necessary
                    if (dgw.RowHeadersWidth < (int)(size.Width + 20))
                    {
                        dgw.RowHeadersWidth = (int)(size.Width + 20);
                    }

                    // Draw the row number in the row header
                    using (Brush b = new SolidBrush(SystemColors.ControlText))
                    {
                        g.DrawString(strRowNumber, this.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }
        private void txtSupplierName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT TC_ID, RTRIM(TransactionID), Date, RTRIM(PaymentMode), Customer.ID, " +
                                   "RTRIM(Customer.CustomerID), RTRIM(Name), Amount, RTRIM(Payment_2.Remarks), " +
                                   "RTRIM(Check_ID), Check_Date, RTRIM(Bank), SalesMan_ID, SalesMan_Name, " +
                                   "SalesMan_Comession, SalesMan_ID_2 " +
                                   "FROM Customer, Payment_2 " +
                                   "WHERE Customer.CustomerID = Payment_2.CustomerID " +
                                   "AND [Name] LIKE '%' + @supplierName + '%' " +
                                   "ORDER BY [Date]";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@supplierName", "%"+ txtSupplierName.Text + "%");

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
                    if (lblSet.Text == "سندات قبض العملاء")
                    {
                        this.Close();
                        DataGridViewRow dr = dgw.SelectedRows[0];
                        Payment_2 frmPayment_2 = new Payment_2();
                        
                        
                        frmPayment_2.txtT_ID.Text = dr.Cells[0].Value.ToString();
                        frmPayment_2.txtTransactionNo.Text = dr.Cells[1].Value.ToString();
                        frmPayment_2.dtpTranactionDate.Value = Convert.ToDateTime(dr.Cells[2].Value);
                        frmPayment_2.cmbPaymentMode.Text = dr.Cells[3].Value.ToString();
                        frmPayment_2.txtSup_ID.Text = dr.Cells[4].Value.ToString();
                        frmPayment_2.txtSupplierID.Text = dr.Cells[5].Value.ToString();
                        frmPayment_2.txtSupplierName.Text = dr.Cells[6].Value.ToString();
                        frmPayment_2.txtTransactionAmount.Text = dr.Cells[7].Value.ToString();
                        frmPayment_2.txtRemarks.Text = dr.Cells[8].Value.ToString();
                        frmPayment_2.txtCheck.Text = dr.Cells[9].Value.ToString();
                        frmPayment_2.dtpCheck.Value = Convert.ToDateTime(dr.Cells[10].Value);
                        frmPayment_2.txtBank.Text = dr.Cells[11].Value.ToString();
                        frmPayment_2.txtSM_ID.Text = dr.Cells[12].Value.ToString();
                        frmPayment_2.txtSalesman.Text = dr.Cells[13].Value.ToString();
                        frmPayment_2.txtCommissionPer.Text = dr.Cells[14].Value.ToString();
                        frmPayment_2.txtSalesmanID.Text = dr.Cells[15].Value.ToString();

                        frmPayment_2.btnSave.Enabled = false;
                        frmPayment_2.GetSupplierBalance();
                        frmPayment_2.btnUpdate.Enabled = true;
                        frmPayment_2.btnDelete.Enabled = true;
                        frmPayment_2.GetSupplierInfo();
                        frmPayment_2.btnSelection.Enabled = false;
                        frmPayment_2.Button1.Enabled = false;
                        frmPayment_2.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PaymentRecord_2_Load(object sender, EventArgs e)
        {

        }

        private void txtSupplierName_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}
