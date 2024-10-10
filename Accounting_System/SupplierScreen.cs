using Microsoft.Office.Interop.Excel;
using Pharmacy.DL;
using System;
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
    public partial class SupplierScreen : Form
    {
        SqlConnection cs = new SqlConnection(DataAccessLayer.Con());
        private Pymentinvoice frmPurchaseEntry;
        public SupplierScreen()
        {
            InitializeComponent();
            base.Load += SupplierScreen_Load;

        }

        private void SupplierScreen_Load(object sender, EventArgs e)
        {
            GetData();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

       
        public void GetData()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT RTRIM(ID),RTRIM(SupplierID),RTRIM([Name]), RTRIM(Address),RTRIM(City),RTRIM(State),RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID),RTRIM(TIN),RTRIM(STNo),RTRIM(CST),RTRIM(PAN),RTRIM(AccountName),RTRIM(AccountNumber),RTRIM(Bank),RTRIM(Branch),RTRIM(IFSCCode),OpeningBalance,RTRIM(OpeningBalanceType),RTRIM(Remarks) FROM Supplier ORDER BY Name", cn);
                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    dgw.Rows.Clear();
                    while (rdr.Read())
                    {
                        dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], rdr[16], rdr[17], rdr[18], rdr[19], rdr[20]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Reset()
        {
            txtSupplierName.Text = "";
            txtContactNo.Text = "";
            txtCity.Text = "";
            GetData();
        }

        private void txtSupplierName_TextChanged(object sender, EventArgs e)
        {
            FilterData("Name", txtSupplierName.Text);
        }

        private void txtCity_TextChanged(object sender, EventArgs e)
        {
            FilterData("City", txtCity.Text);
        }

        private void txtContactNo_TextChanged(object sender, EventArgs e)
        {
            FilterData("ContactNo", txtContactNo.Text);
        }

        private void FilterData(string column, string filterText)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
                {
                    cn.Open();
                    string query = $"SELECT RTRIM(ID),RTRIM(SupplierID),RTRIM([Name]), RTRIM(Address),RTRIM(City),RTRIM(State),RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID),RTRIM(TIN),RTRIM(STNo),RTRIM(CST),RTRIM(PAN),RTRIM(AccountName),RTRIM(AccountNumber),RTRIM(Bank),RTRIM(Branch),RTRIM(IFSCCode),OpeningBalance,RTRIM(OpeningBalanceType),RTRIM(Remarks) FROM Supplier WHERE {column} LIKE @filterText ORDER BY {column}";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@filterText", "%" + filterText + "%");
                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    dgw.Rows.Clear();
                    while (rdr.Read())
                    {
                        dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], rdr[16], rdr[17], rdr[18], rdr[19], rdr[20]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgw_MouseClick(object sender, MouseEventArgs e)
        {
            if (dgw.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgw.SelectedRows[0];
                try
                {

                    
                    if (lblSet.Text == "Supplier Entry")
                    {
                        Supplier sup = new Supplier();
                        sup.txtID.Text = row.Cells[0].Value.ToString();
                        sup.txtSupplierID.Text = row.Cells[1].Value.ToString();
                        sup.txtSupplierName.Text = row.Cells[2].Value.ToString();
                        sup.txtSupName.Text = row.Cells[2].Value.ToString();
                        sup.txtAddress.Text = row.Cells[3].Value.ToString();
                        sup.txtCity.Text = row.Cells[4].Value.ToString();
                        sup.cmbState.Text = row.Cells[5].Value.ToString();
                        sup.txtZipCode.Text = row.Cells[6].Value.ToString();
                        sup.txtContactNo.Text = row.Cells[7].Value.ToString();
                        sup.txtEmailID.Text = row.Cells[8].Value.ToString();
                        sup.txtTIN.Text = row.Cells[9].Value.ToString();
                        sup.txtSTNo.Text = row.Cells[10].Value.ToString();
                        sup.txtCSTNo.Text = row.Cells[11].Value.ToString();
                        sup.txtPAN.Text = row.Cells[12].Value.ToString();
                        sup.txtAccountName.Text = row.Cells[13].Value.ToString();
                        sup.txtAccountNo.Text = row.Cells[14].Value.ToString();
                        sup.txtBank.Text = row.Cells[15].Value.ToString();
                        sup.txtBranch.Text = row.Cells[16].Value.ToString();
                        sup.txtIFSCcode.Text = row.Cells[17].Value.ToString();
                        sup.txtOpeningBalance.Text = row.Cells[18].Value.ToString();
                        sup.cmbOpeningBalanceType.Text = row.Cells[19].Value.ToString();
                        sup.txtRemarks.Text = row.Cells[20].Value.ToString();

                        sup.btnUpdate.Enabled = true;
                        sup.btnDelete.Enabled = true;
                        sup.btnSave.Enabled = false;
                        sup.cmbOpeningBalanceType.Enabled = false;
                        sup.txtOpeningBalance.ReadOnly = true;

                        sup.ShowDialog();
                        this.Close();
                    }
                    else if (lblSet.Text == "Purchase")
                    {
                        try
                        {

                            if (row != null)
                            {
                                Pymentinvoice.instance.txtSup_ID.Text = row.Cells[0]?.Value?.ToString() ?? string.Empty;
                                Pymentinvoice.instance.txtSupplierID.Text = row.Cells[1]?.Value?.ToString() ?? string.Empty;
                                Pymentinvoice.instance.txtSupplierName.Text = row.Cells[2]?.Value?.ToString() ?? string.Empty;
                                Pymentinvoice.instance.txtAddress.Text = row.Cells[3]?.Value?.ToString() ?? string.Empty;
                                Pymentinvoice.instance.txtCity.Text = row.Cells[4]?.Value?.ToString() ?? string.Empty;
                                Pymentinvoice.instance.txtContactNo.Text = row.Cells[7]?.Value?.ToString() ?? string.Empty;
                                Pymentinvoice.instance.GetSupplierBalance();
                                lblSet.Text = "";
                                this.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

        private void OpenSupplierForm(DataGridViewRow row)
        {
            try
            {
                if (lblSet.Text== "Supplier Entry")
                {
                    Supplier sup = new Supplier();
                    sup.txtID.Text = row.Cells[0].Value.ToString();
                    sup.txtSupplierID.Text = row.Cells[1].Value.ToString();
                    sup.txtSupplierName.Text = row.Cells[2].Value.ToString();
                    sup.txtSupName.Text = row.Cells[2].Value.ToString();
                    sup.txtAddress.Text = row.Cells[3].Value.ToString();
                    sup.txtCity.Text = row.Cells[4].Value.ToString();
                    sup.cmbState.Text = row.Cells[5].Value.ToString();
                    sup.txtZipCode.Text = row.Cells[6].Value.ToString();
                    sup.txtContactNo.Text = row.Cells[7].Value.ToString();
                    sup.txtEmailID.Text = row.Cells[8].Value.ToString();
                    sup.txtTIN.Text = row.Cells[9].Value.ToString();
                    sup.txtSTNo.Text = row.Cells[10].Value.ToString();
                    sup.txtCSTNo.Text = row.Cells[11].Value.ToString();
                    sup.txtPAN.Text = row.Cells[12].Value.ToString();
                    sup.txtAccountName.Text = row.Cells[13].Value.ToString();
                    sup.txtAccountNo.Text = row.Cells[14].Value.ToString();
                    sup.txtBank.Text = row.Cells[15].Value.ToString();
                    sup.txtBranch.Text = row.Cells[16].Value.ToString();
                    sup.txtIFSCcode.Text = row.Cells[17].Value.ToString();
                    sup.txtOpeningBalance.Text = row.Cells[18].Value.ToString();
                    sup.cmbOpeningBalanceType.Text = row.Cells[19].Value.ToString();
                    sup.txtRemarks.Text = row.Cells[20].Value.ToString();

                    sup.btnUpdate.Enabled = true;
                    sup.btnDelete.Enabled = true;
                    sup.btnSave.Enabled = false;
                    sup.cmbOpeningBalanceType.Enabled = false;
                    sup.txtOpeningBalance.ReadOnly = true;

                    sup.Show();
                    this.Close();
                    this.FormClosed += (s, args) => this.Close();
                }else if (lblSet.Text == "Purchase")
                {

                    Pymentinvoice.instance.txtSup_ID.Text = row.Cells[0].Value.ToString();
                    Pymentinvoice.instance.txtSupplierID.Text = row.Cells[1].Value.ToString();
                    Pymentinvoice.instance.txtSupplierName.Text = row.Cells[2].Value.ToString();
                    Pymentinvoice.instance.txtAddress.Text = row.Cells[3].Value.ToString();
                    Pymentinvoice.instance.txtCity.Text = row.Cells[4].Value.ToString();
                    Pymentinvoice.instance.txtContactNo.Text = row.Cells[7].Value.ToString();
                    lblSet.Text = "";
                    this.Close();

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
    }
}
