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

namespace Accounting_System
{
    public partial class CustomerRecord5 : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public CustomerRecord5()
        {
            InitializeComponent();
            Getdata();
            dgw.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dgw_RowPostPaint);
            txtCustomerName.TextChanged += new EventHandler(txtCustomerName_TextChanged);
            txtCity.TextChanged += new EventHandler(txtCity_TextChanged);
            txtContactNo.TextChanged += new EventHandler(txtContactNo_TextChanged);
            dgw.MouseDoubleClick += new MouseEventHandler(dgw_MouseDoubleClick);
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
                    using (SqlCommand cmd = new SqlCommand("SELECT RTRIM(ID), RTRIM(CustomerID), RTRIM([Name]), RTRIM(Gender), RTRIM(Address), RTRIM(City), RTRIM(State), RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID), RTRIM(Remarks), Photo, OpeningBalance, OpeningBalanceType FROM Customer WHERE CustomerType = 'Regular' ORDER BY Name", con))
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

        private void dgw_RowPostPaint(object sender , DataGridViewRowPostPaintEventArgs e)
        {
/*            string strRowNumber = (e.RowIndex + 1).ToString();
            using (Graphics g = e.Graphics)
            {
                // Measure the size of the row number text
                SizeF size = g.MeasureString(strRowNumber, this.Font);

                // Adjust the RowHeadersWidth if needed
                if (dgw.RowHeadersWidth < (int)(size.Width + 20))
                {
                    dgw.RowHeadersWidth = (int)(size.Width + 20);
                }

                // Draw the row number
                Brush brush = SystemBrushes.ControlText;
                g.DrawString(strRowNumber, this.Font, brush, e.RowBounds.Left + 15, e.RowBounds.Top + ((e.RowBounds.Height - size.Height) / 2));
            }*/
        }
        private void txtCustomerName_TextChanged(object sender, EventArgs e) {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT RTRIM(ID), RTRIM(CustomerID), RTRIM([Name]), RTRIM(Gender), RTRIM(Address), RTRIM(City), RTRIM(State), RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID), RTRIM(Remarks), Photo " +
                                   "FROM Customer " +
                                   "WHERE CustomerType = 'Regular' AND [Name] LIKE @CustomerName " +
                                   "ORDER BY [Name]";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CustomerName", "%" + txtCustomerName.Text + "%");

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
        private void txtCity_TextChanged(object sender, EventArgs args) {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT RTRIM(ID), RTRIM(CustomerID), RTRIM([Name]), RTRIM(Gender), RTRIM(Address), RTRIM(City), RTRIM(State), RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID), RTRIM(Remarks), Photo " +
                                   "FROM Customer " +
                                   "WHERE CustomerType = 'Regular' AND City LIKE @City " +
                                   "ORDER BY [Name]";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@City", "%" + txtCity.Text + "%");

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
            txtCustomerName.Text = string.Empty;
            txtContactNo.Text = string.Empty;
            txtCity.Text = string.Empty;
            Getdata();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Reset();
        }
        private void txtContactNo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT RTRIM(ID), RTRIM(CustomerID), RTRIM([Name]), RTRIM(Gender), RTRIM(Address), RTRIM(City), RTRIM(State), RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID), RTRIM(Remarks), Photo " +
                                   "FROM Customer " +
                                   "WHERE CustomerType = 'Regular' AND ContactNo LIKE @ContactNo " +
                                   "ORDER BY [Name]";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ContactNo", "%" + txtContactNo.Text + "%");

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

        private void Button2_Click(object sender, EventArgs e)
        {
            AddCustomer frmCustomer = new AddCustomer();
            frmCustomer.lblUser.Text = lblUser.Text;
            frmCustomer.Reset();
            frmCustomer.Reset();
            frmCustomer.ShowDialog();
        }
        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (dgw.Rows.Count > 0)
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];
                    this.Close();
                    CustomerLedger customerLedgerForm = new CustomerLedger();
                    CustomerLedger.instance.txtCustomerID.Text = dr.Cells[1].Value.ToString();
                    CustomerLedger.instance.cmbCustomerName.Text = dr.Cells[2].Value.ToString();
                    CustomerLedger.instance.ShowDialog();
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
    }
}
