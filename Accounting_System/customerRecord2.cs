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
    public partial class customerRecord2 : Form
    {
        public static customerRecord2 _instance;
        public static customerRecord2 instance;
        public static customerRecord2 Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new customerRecord2();
                }
                return _instance;
            }

        }


        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public customerRecord2()
        {
          
            InitializeComponent();
            Getdata();
            dgw.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dgw_RowPostPaint);
            txtCustomerName.TextChanged += new EventHandler(txtCustomerName_TextChanged);
            txtCity.TextChanged += new EventHandler(txtCity_TextChanged);
            txtContactNo.TextChanged += new EventHandler(txtContactNo_TextChanged);
            dgw.CellContentClick += new DataGridViewCellEventHandler(dgw_CellContentClick);
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
                    using (SqlCommand cmd = new SqlCommand("SELECT RTRIM(ID), RTRIM(CustomerID), RTRIM([Name]), RTRIM(Gender), RTRIM(Address), RTRIM(City), RTRIM(State), RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID), RTRIM(Remarks), Photo FROM Customer WHERE CustomerType='Regular' ORDER BY ID", con))
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
        private void dgw_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {

            
        string strRowNumber = (e.RowIndex + 1).ToString();
        SizeF size = e.Graphics.MeasureString(strRowNumber, this.Font);

        if (dgw.RowHeadersWidth < Convert.ToInt32(size.Width + 20))
            {
                    dgw.RowHeadersWidth = Convert.ToInt32(size.Width + 20);
                    dgw.RowHeadersDefaultCellStyle.ForeColor = Color.White;
            }

        Brush brush = SystemBrushes.Window; // Equivalent to SystemBrushes.ControlText
        e.Graphics.DrawString(strRowNumber, this.Font, brush, e.RowBounds.Location.X + dgw.Width - 43, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2));
            

        }
        private void txtCustomerName_TextChanged(object sender, EventArgs e) {

            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT RTRIM(ID), RTRIM(CustomerID), RTRIM([Name]), RTRIM(Gender), RTRIM(Address), RTRIM(City), RTRIM(State), RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID), RTRIM(Remarks), Photo " +
                                   "FROM Customer " +
                                   "WHERE CustomerType = 'Regular' AND Name LIKE '%' + @customerName + '%' " +
                                   "ORDER BY ID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@customerName", txtCustomerName.Text);

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
        private void txtCity_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT RTRIM(ID), RTRIM(CustomerID), RTRIM([Name]), RTRIM(Gender), RTRIM(Address), RTRIM(City), RTRIM(State), RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID), RTRIM(Remarks), Photo " +
                                   "FROM Customer " +
                                   "WHERE CustomerType = 'Regular' AND City LIKE '%' + @city + '%' " +
                                   "ORDER BY ID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@city", txtCity.Text);

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
        public void Reset() {
            txtCustomerName.Text = "";
            txtContactNo.Text = "";
            txtCity.Text = "";
            Getdata();
                }

        private void Button2_Click(object sender, EventArgs e)
        {
            AddCustomer frmCustomer = new AddCustomer();
            frmCustomer.lblUser.Text = lblUser.Text;
            frmCustomer.Reset();
            frmCustomer.Reset();
            frmCustomer.ShowDialog();
            
        }

        private void lblSet_Click(object sender, EventArgs e)
        {

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
                                   "WHERE CustomerType = 'Regular' AND ContactNo LIKE '%' + @contactNo + '%' " +
                                   "ORDER BY ID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@contactNo", txtContactNo.Text);

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
        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e) {

/*            if (frmPOS.txtCustomerID.Text == "C-0001")
            {
                frmPOS.cmbPaymentMode.SelectedIndex = 0;
                frmPOS.txtPayment.ReadOnly = true;
                frmPOS.txtPayment.Text = Convert.ToString(decimal.Parse(frmPOS.txtGrandTotal.Text));
            }
            else
            {
                frmPOS.txtPayment.ReadOnly = false;
            }
*/
        }
        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e) {

            try
            {
                if (dgw.Rows.Count > 0)
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];

/*                    if (lblSet.Text == "Billing")
                    {
                        frmPOS.Show();
                        this.Hide();
                        frmPOS.txtCID.Text = dr.Cells[0].Value.ToString();
                        frmPOS.txtCustomerID.Text = dr.Cells[1].Value.ToString();
                        frmPOS.txtCustomerName.Text = dr.Cells[2].Value.ToString();
                        frmPOS.txtContactNo.Text = dr.Cells[8].Value.ToString();
                        frmPOS.txtCustomerName.ReadOnly = true;
                        frmPOS.txtContactNo.ReadOnly = true;
                        lblSet.Text = "";
                    }
                    else */if (lblSet.Text == "Quotation")
                    {
                        Quotation frmQuotation = Quotation.Instance;
                        frmQuotation.Reset();
                        frmQuotation.Show();
                        frmQuotation.txtCID.Text = dr.Cells[0].Value.ToString();
                        frmQuotation.txtCustomerID.Text = dr.Cells[1].Value.ToString();
                        frmQuotation.txtCustomerName.Text = dr.Cells[2].Value.ToString();
                        frmQuotation.txtContactNo.Text = dr.Cells[8].Value.ToString();
                        frmQuotation.txtCustomerName.ReadOnly = true;
                        frmQuotation.txtContactNo.ReadOnly = true;
                        lblSet.Text = "";
                        this.Hide();


                    }
                    else
                    {
/*
                        services frmServices = new services();
                        frmServices.Show();
                        this.Hide();
                        frmServices.txtCID.Text = dr.Cells[0].Value.ToString();
                        frmServices.txtCustomerID.Text = dr.Cells[1].Value.ToString();
                        frmServices.txtCustomerName.Text = dr.Cells[2].Value.ToString();
                        frmServices.txtContactNo.Text = dr.Cells[8].Value.ToString();
                        lblSet.Text = "";*/

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
