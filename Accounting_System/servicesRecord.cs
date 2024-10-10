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
    public partial class servicesRecord : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public servicesRecord()
        {
            InitializeComponent();
            Getdata();
            fillServiceCode();
            dgw.MouseDoubleClick += new MouseEventHandler(dgw_MouseClick);
            dgw.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dgw_RowPostPaint);
            cmbServiceCode.SelectedIndexChanged += new EventHandler(cmbOrderNo_SelectedIndexChanged);
            txtCustomerName.TextChanged += new EventHandler(txtCustomerName_TextChanged);
            cmbServiceCode.Format += new ListControlConvertEventHandler(cmbInvoiceNo_Format);
        }

        private void servicesRecord_Load(object sender, EventArgs e)
        {

        }
        private void Getdata()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT S_ID, RTRIM(ServiceCode), ServiceCreationDate, Customer.ID, RTRIM(Customer.CustomerID), RTRIM(Name), RTRIM(ServiceType), RTRIM(ItemDescription), RTRIM(ProblemDescription), ChargesQuote, AdvanceDeposit, EstimatedRepairDate, RTRIM(Status), RTRIM(Service.Remarks) " +
                                   "FROM Customer, Service WHERE Customer.ID = Service.CustomerID ORDER BY ServiceCreationDate";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13]);
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
        private void fillServiceCode()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlDataAdapter adp = new SqlDataAdapter("SELECT DISTINCT RTRIM(ServiceCode) FROM Service", con))
                    {
                        DataSet ds = new DataSet();
                        adp.Fill(ds);
                        DataTable dtable = ds.Tables[0];
                        cmbServiceCode.Items.Clear();
                        foreach (DataRow drow in dtable.Rows)
                        {
                            cmbServiceCode.Items.Add(drow[0].ToString());
                        }
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
            try
            {
                if (dgw.Rows.Count > 0)
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];
                    if (lblSet.Text == "Services")
                    {
                        services frm = new services();
                        frm.Show();
                        this.Hide();

                        frm.txtID.Text = dr.Cells[0].Value.ToString();
                        frm.txtServiceCode.Text = dr.Cells[1].Value.ToString();
                        frm.dtpServiceCreationDate.Text = dr.Cells[2].Value.ToString();
                        frm.txtCustomerID.Text = dr.Cells[4].Value.ToString();
                        frm.txtCID.Text = dr.Cells[3].Value.ToString();
                        frm.txtCustomerName.Text = dr.Cells[5].Value.ToString();
                        frm.cmbServiceType.Text = dr.Cells[6].Value.ToString();
                        frm.txtItemsDescription.Text = dr.Cells[7].Value.ToString();
                        frm.txtProblemDescription.Text = dr.Cells[8].Value.ToString();
                        frm.txtChargesQuote.Text = dr.Cells[9].Value.ToString();
                        frm.txtUpfront.Text = dr.Cells[10].Value.ToString();
                        frm.dtpEstimatedRepairDate.Text = dr.Cells[11].Value.ToString();
                        frm.cmbStatus.Text = dr.Cells[12].Value.ToString();
                        frm.txtRemarks.Text = dr.Cells[13].Value.ToString();

                        frm.btnSave.Enabled = false;
                        frm.btnUpdate.Enabled = true;
                        frm.btnPrint.Enabled = true;
                        frm.btnDelete.Enabled = true;
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
            e.Graphics.DrawString(strRowNumber, this.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2));

        }
        public void Reset()
        {
            cmbServiceCode.Text = "";
            txtCustomerName.Text = "";
            fillServiceCode();
            dtpDateFrom.Value = DateTime.Today;
            dtpDateTo.Value = DateTime.Today;
            DateTimePicker2.Value = DateTime.Today;
            DateTimePicker1.Value = DateTime.Today;
            cmbStatus.SelectedIndex = -1;
            Getdata();

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cmbStatus.Text))
                {
                    MessageBox.Show("Please select status", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbStatus.Focus();
                    return; // Exit the method
                }

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT S_ID, RTRIM(ServiceCode), ServiceCreationDate, Customer.ID, RTRIM(Customer.CustomerID), RTRIM(Name), RTRIM(ServiceType), RTRIM(ItemDescription), RTRIM(ProblemDescription), ChargesQuote, AdvanceDeposit, EstimatedRepairDate, RTRIM(Status), RTRIM(Service.Remarks) " +
                                   "FROM Customer, Service " +
                                   "WHERE Customer.ID = Service.CustomerID " +
                                   "AND ServiceCreationDate BETWEEN @d1 AND @d2 " +
                                   "AND Status = @status " +
                                   "ORDER BY ServiceCreationDate";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@d1", SqlDbType.DateTime).Value = DateTimePicker2.Value.Date;
                        cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = DateTimePicker1.Value.Date;
                        cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = cmbStatus.Text;

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(
                                    rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5],
                                    rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11],
                                    rdr[12], rdr[13]);
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

        private void btnGetData_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT S_ID, RTRIM(ServiceCode), ServiceCreationDate, Customer.ID, RTRIM(Customer.CustomerID), RTRIM(Name), RTRIM(ServiceType), RTRIM(ItemDescription), RTRIM(ProblemDescription), ChargesQuote, AdvanceDeposit, EstimatedRepairDate, RTRIM(Status), RTRIM(Service.Remarks) " +
                        "FROM Customer, Service " +
                        "WHERE Customer.ID = Service.CustomerID " +
                        "AND ServiceCreationDate BETWEEN @d1 AND @d2 " +
                        "ORDER BY ServiceCreationDate", con))
                    {
                        cmd.Parameters.Add("@d1", SqlDbType.DateTime).Value = dtpDateFrom.Value.Date;
                        cmd.Parameters.Add("@d2", SqlDbType.DateTime).Value = dtpDateTo.Value.Date;

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(
                                    rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5],
                                    rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11],
                                    rdr[12], rdr[13]);
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
                    string query = "SELECT S_ID, RTRIM(ServiceCode), ServiceCreationDate, Customer.ID, RTRIM(Customer.CustomerID), RTRIM(Name), RTRIM(ServiceType), RTRIM(ItemDescription), RTRIM(ProblemDescription), ChargesQuote, AdvanceDeposit, EstimatedRepairDate, RTRIM(Status), RTRIM(Service.Remarks) " +
                                   "FROM Customer, Service " +
                                   "WHERE Customer.ID = Service.CustomerID " +
                                   "AND ServiceCode = @serviceCode " +
                                   "ORDER BY ServiceCreationDate";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@serviceCode", cmbServiceCode.Text);

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(
                                    rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5],
                                    rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11],
                                    rdr[12], rdr[13]);
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
                    string query = "SELECT S_ID, RTRIM(ServiceCode), ServiceCreationDate, Customer.ID, RTRIM(Customer.CustomerID), RTRIM(Name), RTRIM(ServiceType), RTRIM(ItemDescription), RTRIM(ProblemDescription), ChargesQuote, AdvanceDeposit, EstimatedRepairDate, RTRIM(Status), RTRIM(Service.Remarks) " +
                                   "FROM Customer, Service " +
                                   "WHERE Customer.ID = Service.CustomerID " +
                                   "AND Name LIKE @customerName " +
                                   "ORDER BY ServiceCreationDate";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@customerName", "%" + txtCustomerName.Text + "%");

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(
                                    rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5],
                                    rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11],
                                    rdr[12], rdr[13]);
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
        private void cmbInvoiceNo_Format(object sender, ListControlConvertEventArgs e)
        {
            if (e.DesiredType == typeof(string))
            {
                e.Value = e.Value.ToString().Trim();
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
    }
}
