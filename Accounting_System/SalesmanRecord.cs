using Microsoft.SqlServer.Server;
using Pharmacy.DL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Accounting_System
{
    public partial class SalesmanRecord : Form
    {
        public static SalesmanRecord _instance;
        public static SalesmanRecord instance;
        public static SalesmanRecord Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new SalesmanRecord();
                }
                return _instance;
            }

        }
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public SalesmanRecord()
        {
            InitializeComponent();
            Getdata();
            txtSalesmanName.TextChanged += new EventHandler(txtSalesmanName_TextChanged);
            txtCity.TextChanged += new EventHandler(txtCity_TextChanged);
            txtContactNo.TextChanged += new EventHandler(txtContactNo_TextChanged);
            dgw.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dgw_RowPostPaint);
            dgw.MouseDoubleClick += new MouseEventHandler(dgw_MouseDoubleClick);
            instance = this;
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        public void Getdata()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT SM_ID, RTRIM(Salesman_ID), RTRIM([Name]), RTRIM(Address), RTRIM(City), RTRIM(State), RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID), CommissionPer, RTRIM(Remarks), Photo FROM Salesman ORDER BY [Name]";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11]);
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
        private void txtSalesmanName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT SM_ID, RTRIM(Salesman_ID), RTRIM([Name]), RTRIM(Address), RTRIM(City), RTRIM(State), RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID), CommissionPer, RTRIM(Remarks), Photo " +
                                   "FROM Salesman " +
                                   "WHERE [Name] LIKE @name " +
                                   "ORDER BY [Name]";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {

                        cmd.Parameters.AddWithValue("@name", "%" + txtSalesmanName.Text + "%");

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11]);
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
        public void Reset()
        {
            txtSalesmanName.Text = "";
            txtContactNo.Text = "";
            txtCity.Text = "";
            Getdata();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Reset();
        }
        private void txtCity_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT SM_ID, RTRIM(Salesman_ID), RTRIM([Name]), RTRIM(Address), RTRIM(City), RTRIM(State), RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID), CommissionPer, RTRIM(Remarks), Photo " +
                                   "FROM Salesman " +
                                   "WHERE City LIKE @city " +
                                   "ORDER BY [Name]";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Use parameterized queries to prevent SQL injection
                        cmd.Parameters.AddWithValue("@city", "%" + txtCity.Text + "%");

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11]);
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
        private void txtContactNo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT SM_ID, RTRIM(Salesman_ID), RTRIM([Name]), RTRIM(Address), RTRIM(City), RTRIM(State), RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID), CommissionPer, RTRIM(Remarks), Photo " +
                                   "FROM Salesman " +
                                   "WHERE ContactNo LIKE @contactNo " +
                                   "ORDER BY [Name]";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Use parameterized queries to prevent SQL injection
                        cmd.Parameters.AddWithValue("@contactNo", "%" + txtContactNo.Text + "%");

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11]);
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
        private void dgw_RowPostPaint(object s, DataGridViewRowPostPaintEventArgs e)
        {
/*            try
            {
                // Create a string for the row number
                string rowNumber = (e.RowIndex + 1).ToString();

                // Measure the size of the string
                SizeF size = e.Graphics.MeasureString(rowNumber, this.Font);

                // Adjust the width of the row header if necessary
                if (dgw.RowHeadersWidth < Convert.ToInt32(size.Width + 20))
                {
                    dgw.RowHeadersWidth = Convert.ToInt32(size.Width + 20);
                }

                // Draw the string in the row header
                using (Brush brush = SystemBrushes.ControlText)
                {
                    e.Graphics.DrawString(rowNumber, this.Font, brush, e.RowBounds.Location.X + 15,
                                          e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }
        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (dgw.Rows.Count > 0)
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];

                    /*if (lblSet.Text == "Salesman Entry")
                    {
                        frmSalesman.Show();
                        this.Close();
                        frmSalesman.txtID.Text = dr.Cells[0].Value.ToString();
                        frmSalesman.txtSalesmanID.Text = dr.Cells[1].Value.ToString();
                        frmSalesman.txtSalesmanName.Text = dr.Cells[2].Value.ToString();
                        frmSalesman.txtAddress.Text = dr.Cells[3].Value.ToString();
                        frmSalesman.txtCity.Text = dr.Cells[4].Value.ToString();
                        frmSalesman.cmbState.Text = dr.Cells[5].Value.ToString();
                        frmSalesman.txtZipCode.Text = dr.Cells[6].Value.ToString();
                        frmSalesman.txtContactNo.Text = dr.Cells[7].Value.ToString();
                        frmSalesman.txtEmailID.Text = dr.Cells[8].Value.ToString();
                        frmSalesman.txtCommissionPer.Text = dr.Cells[9].Value.ToString();
                        frmSalesman.txtRemarks.Text = dr.Cells[10].Value.ToString();

                        byte[] data = (byte[])dr.Cells[11].Value;
                        using (MemoryStream ms = new MemoryStream(data))
                        {
                            frmSalesman.Picture.Image = Image.FromStream(ms);
                        }

                        frmSalesman.btnUpdate.Enabled = true;
                        frmSalesman.btnDelete.Enabled = true;
                        frmSalesman.btnSave.Enabled = false;
                        lblSet.Text = string.Empty;
                    }*/
                    /*else if (lblSet.Text == "Billing")
                    {
                        frmPOS.Show();
                        this.Close();
                        frmPOS.txtSM_ID.Text = dr.Cells[0].Value.ToString();
                        frmPOS.txtSalesmanID.Text = dr.Cells[1].Value.ToString();
                        frmPOS.txtSalesman.Text = dr.Cells[2].Value.ToString();
                        frmPOS.txtCommissionPer.Text = dr.Cells[9].Value.ToString();
                        lblSet.Text = string.Empty;
                    }*/
                    if (lblSet.Text == "txtvenduerID")
                    {
                        // Code commented out in original, handle as needed
                        this.Close();
                        // Uncomment and implement if necessary
                        // frmVendeur.cmbSalesman.Text = dr.Cells[2].Value.ToString();
                        lblSet.Text = string.Empty;
                    }
                    else if (lblSet.Text == "voucher")
                    {
                        Voucher frmVoucher = new Voucher();
                        frmVoucher.Reset();
                        this.Close();
                        frmVoucher.txtName.Text = dr.Cells[2].Value.ToString();
                        frmVoucher.txtName.ReadOnly = true;
                        lblSet.Text = string.Empty;
                        frmVoucher.ShowDialog();
                    }
                    else if (lblSet.Text == "voucher1")
                    {
                        VoucherReport frmVoucherReport = new VoucherReport();
                        this.Close();
                        frmVoucherReport.TextBox1.Text = dr.Cells[2].Value.ToString();
                        frmVoucherReport.TextBox1.ReadOnly = true;
                        lblSet.Text = string.Empty;
                        frmVoucherReport.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {     
            SalesMan frmSalesman = new SalesMan();
            frmSalesman.Show();
            frmSalesman.Reset();
        }

        private void SalesmanRecord_Load(object sender, EventArgs e)
        {

        }

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
