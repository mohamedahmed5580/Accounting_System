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
    public partial class CustomerRecord3 : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public CustomerRecord3()
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
                    using (SqlCommand cmd = new SqlCommand("SELECT RTRIM(ID), RTRIM(CustomerID), RTRIM([Name]), RTRIM(Gender), RTRIM(Address), RTRIM(City), RTRIM(State), RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID), RTRIM(Remarks), Photo FROM Customer WHERE CustomerType='Regular' ORDER BY Name", con))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(
                                    rdr[0], rdr[1], rdr[2], rdr[3], rdr[4],
                                    rdr[5], rdr[6], rdr[7], rdr[8], rdr[9],
                                    rdr[10], rdr[11]
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
        public void Reset()
        {
            txtCustomerName.Text = "";
            txtContactNo.Text = "";
            txtCity.Text = "";
            Getdata(); 
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void Button2_Click(object sender, EventArgs e)
        {

            AddCustomer frmCustomer = new AddCustomer();
            frmCustomer.lblUser.Text = lblUser.Text;
            frmCustomer.Reset();
            frmCustomer.ShowDialog();

        }
        private void dgw_RowPostPaint(object sender,DataGridViewRowPostPaintEventArgs e) 
        {
            try
            {
                string strRowNumber = (e.RowIndex + 1).ToString();

                // Measure the width of the row number
                SizeF size = e.Graphics.MeasureString(strRowNumber, this.Font);

                // Adjust the width of the row header if necessary
                if (dgw.RowHeadersWidth < (int)(size.Width + 20))
                {
                    dgw.RowHeadersWidth = (int)(size.Width + 20);
                }

                // Draw the row number in the row header
                Brush brush = SystemBrushes.ControlText; // Use the brush directly without `using`
                e.Graphics.DrawString(strRowNumber, this.Font, brush, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2));
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
                    string query = "SELECT RTRIM(ID), RTRIM(CustomerID), RTRIM([Name]), RTRIM(Gender), RTRIM(Address), RTRIM(City), RTRIM(State), RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID), RTRIM(Remarks), Photo " +
                                   "FROM Customer " +
                                   "WHERE CustomerType = 'Regular' AND Name LIKE @name " +
                                   "ORDER BY Name";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@name", "%" + txtCustomerName.Text + "%");

                        using (SqlDataReader rdr = cmd.ExecuteReader())
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
        private void txtCity_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT RTRIM(ID), RTRIM(CustomerID), RTRIM([Name]), RTRIM(Gender), RTRIM(Address), RTRIM(City), RTRIM(State), RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID), RTRIM(Remarks), Photo " +
                                   "FROM Customer " +
                                   "WHERE CustomerType = 'Regular' AND City LIKE @city " +
                                   "ORDER BY Name";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@city", "%" + txtCity.Text + "%");

                        using (SqlDataReader rdr = cmd.ExecuteReader())
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
                    string query = "SELECT RTRIM(ID), RTRIM(CustomerID), RTRIM([Name]), RTRIM(Gender), RTRIM(Address), RTRIM(City), RTRIM(State), RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID), RTRIM(Remarks), Photo " +
                                   "FROM Customer " +
                                   "WHERE CustomerType = 'Regular' AND ContactNo LIKE @contactNo " +
                                   "ORDER BY Name";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@contactNo", "%" + txtContactNo.Text + "%");

                        using (SqlDataReader rdr = cmd.ExecuteReader())
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
        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {

                if (dgw.Rows.Count > 0)
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];

                    Payment_2 frmPayment_2 = Payment_2.Instance;
                    frmPayment_2.Reset();

                    frmPayment_2.txtSupplierID.Text = dr.Cells[1].Value.ToString();
                    frmPayment_2.txtSupplierName.Text = dr.Cells[2].Value.ToString();
                    frmPayment_2.txtAddress.Text = dr.Cells[4].Value.ToString();
                    frmPayment_2.txtCity.Text = dr.Cells[5].Value.ToString();
                    frmPayment_2.txtContactNo.Text = dr.Cells[8].Value.ToString();
                    frmPayment_2.GetSupplierBalance();
                    frmPayment_2.lblSet.Text = "سندات قبض العملاء";
                    this.Close();
                    frmPayment_2.ShowDialog();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
