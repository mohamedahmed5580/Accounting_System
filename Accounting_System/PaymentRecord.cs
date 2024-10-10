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
    public partial class PaymentRecord : Form
    {
        SqlConnection cn = new SqlConnection(DataAccessLayer.Con());
        public PaymentRecord()
        {
            InitializeComponent();
            GetData();
            txtSupplierName.TextChanged += new EventHandler(txtSupplierName_TextChanged);
            dgw.MouseDoubleClick += new MouseEventHandler(dgw_MouseDoubleClick);
        }
        public void GetData()
        {
            
            
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT T_ID, RTRIM(TransactionID), Date, RTRIM(PaymentMode), Supplier.ID, RTRIM(Supplier.SupplierID), RTRIM(Name), Amount, RTRIM(Payment.Remarks) from Supplier, Payment where Supplier.ID = Payment.SupplierID and Amount > 0 order by [Date]", cn))
                {
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        dgw.Rows.Clear();
                        while (rdr.Read())
                        {
                            dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8]);
                        }
                    }
                }
                cn.Close();
            
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            
            
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT T_ID, RTRIM(TransactionID), Date, RTRIM(PaymentMode), Supplier.ID, RTRIM(Supplier.SupplierID), RTRIM(Name), Amount, RTRIM(Payment.Remarks) from Supplier, Payment where Supplier.ID = Payment.SupplierID and Amount > 0 and [Date] between @d1 and @d2 order by [Date]", cn))
                {
                    cmd.Parameters.Add(new SqlParameter("@d1", SqlDbType.DateTime) { Value = dtpDateFrom.Value.Date });
                    cmd.Parameters.Add(new SqlParameter("@d2", SqlDbType.DateTime) { Value = dtpDateTo.Value });

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
        private void txtSupplierName_TextChanged(object sender, EventArgs e)
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT T_ID, RTRIM(TransactionID), Date, RTRIM(PaymentMode), Supplier.ID, RTRIM(Supplier.SupplierID), RTRIM(Name), Amount, RTRIM(Payment.Remarks) from Supplier, Payment where Supplier.ID = Payment.SupplierID and Amount > 0 and [Name] like @name order by [Date]", cn))
            {
                cmd.Parameters.AddWithValue("@name", "%" + txtSupplierName.Text + "%");

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
        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dgw.Rows.Count > 0)
            {
                
                
                    DataGridViewRow dr = dgw.SelectedRows[0];
                    supplier_payment frmPayment = new supplier_payment();
                    frmPayment.Show();
                    this.Hide();
                    frmPayment.txtT_ID.Text = dr.Cells[0].Value.ToString();
                    frmPayment.txtTransactionNo.Text = dr.Cells[1].Value.ToString();
                    frmPayment.dtpTranactionDate.Text = dr.Cells[2].Value.ToString();
                    frmPayment.cmbPaymentMode.Text = dr.Cells[3].Value.ToString();
                    frmPayment.txtSup_ID.Text = dr.Cells[4].Value.ToString();
                    frmPayment.txtSupplierID.Text = dr.Cells[5].Value.ToString();
                    frmPayment.txtSupplierName.Text = dr.Cells[6].Value.ToString();
                    frmPayment.txtTransactionAmount.Text = dr.Cells[7].Value.ToString();
                    frmPayment.txtRemarks.Text = dr.Cells[8].Value.ToString();
                    frmPayment.btnSave.Enabled = false;
                    frmPayment.GetSupplierBalance();
                    frmPayment.btnUpdate.Enabled = true;
                    frmPayment.btnDelete.Enabled = true;
                    frmPayment.GetSupplierInfo();
                    frmPayment.btnSelection.Enabled = false;
                
            }

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSupplierName.Text = "";
            dtpDateFrom.Value = DateTime.Today;
            dtpDateTo.Value = DateTime.Today;

        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            ExportExcel(dgw);
        }
        public void ExportExcel(object obj)
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

        private void txtSupplierName_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void PaymentRecord_Load(object sender, EventArgs e)
        {

        }
    }
}
