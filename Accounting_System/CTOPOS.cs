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
using static Accounting_System.POS;

namespace Accounting_System
{
    public partial class CTOPOS : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());

        public CTOPOS()
        {
            InitializeComponent();
        }
        private void CTOPOS_Load(object sender, EventArgs e)
        {
            Getdata();
        }
        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void dgw_RowPostPaint(object sender, System.Windows.Forms.DataGridViewRowPostPaintEventArgs e)
        {
            string strRowNumber = (e.RowIndex + 1).ToString();
            SizeF size = e.Graphics.MeasureString(strRowNumber, this.Font);
            if (dgw.RowHeadersWidth < Convert.ToInt32(size.Width + 20))
            {
                dgw.RowHeadersWidth = Convert.ToInt32(size.Width + 20);
                dgw.RowHeadersDefaultCellStyle.ForeColor = Color.White;
            }
            Brush b = SystemBrushes.Window; // ControlText
            e.Graphics.DrawString(strRowNumber, this.Font, b, e.RowBounds.Location.X + dgw.Width - 43, e.RowBounds.Location.Y + (e.RowBounds.Height - size.Height) / 2);

        }
        private void txtCustomerName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT RTRIM(ID),RTRIM(CustomerID),RTRIM([Name]),RTRIM(Gender), RTRIM(Address),RTRIM(City),RTRIM(State),RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID),RTRIM(Remarks),Photo from Customer where CustomerType='Regular' and name like '%" + txtCustomerName.Text + "%' order by ID", con);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dgw.Rows.Clear();
                while (rdr.Read() == true)
                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11]);
                con.Close();
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
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT RTRIM(ID),RTRIM(CustomerID),RTRIM([Name]),RTRIM(Gender), RTRIM(Address),RTRIM(City),RTRIM(State),RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID),RTRIM(Remarks),Photo from Customer where CustomerType='Regular' and City like '%" + txtCity.Text + "%' order by ID", con);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dgw.Rows.Clear();
                while (rdr.Read() == true)
                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11]);
                con.Close();
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
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT RTRIM(ID),RTRIM(CustomerID),RTRIM([Name]),RTRIM(Gender), RTRIM(Address),RTRIM(City),RTRIM(State),RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID),RTRIM(Remarks),Photo from Customer where CustomerType='Regular' and ContactNo like '%" + txtContactNo.Text + "%' order by ID", con);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dgw.Rows.Clear();
                while (rdr.Read() == true)
                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11]);
                con.Close();
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


        public void Getdata()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT RTRIM(ID),RTRIM(CustomerID),RTRIM([Name]),RTRIM(Gender), RTRIM(Address),RTRIM(City),RTRIM(State),RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID),RTRIM(Remarks),Photo from Customer where CustomerType='Regular' order by ID", con);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dgw.Rows.Clear();
                while (rdr.Read() == true)
                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11]);
                con.Close();
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

                    if (lblSet.Text == "Billing")
                    {

                        POS.instance.txtCID.Text = dr.Cells[0].Value.ToString();
                        POS.instance.txtCustomerID.Text = dr.Cells[1].Value.ToString();
                        POS.instance.txtCustomerName.Text = dr.Cells[2].Value.ToString();
                        POS.instance.txtContactNo.Text = dr.Cells[8].Value.ToString();
                        POS.instance.txtCustomerName.ReadOnly = true;
                        POS.instance.txtContactNo.ReadOnly = true;

                        this.Hide();


                    }
                    /*if (lblSet.Text == "Quotation")
                    {
                        frmQuotation.Show();
                        this.Hide();
                        frmQuotation.txtCID.Text = dr.Cells(0).Value.ToString();
                        frmQuotation.txtCustomerID.Text = dr.Cells(1).Value.ToString();
                        frmQuotation.txtCustomerName.Text = dr.Cells(2).Value.ToString();
                        frmQuotation.txtContactNo.Text = dr.Cells(8).Value.ToString();
                        frmQuotation.txtCustomerName.ReadOnly = true;
                        frmQuotation.txtContactNo.ReadOnly = true;
                        lblSet.Text = "";
                    }
                    if (lblSet.Text == "Services")
                    {
                        frmServices.Show();
                        this.Hide();
                        frmServices.txtCID.Text = dr.Cells(0).Value.ToString();
                        frmServices.txtCustomerID.Text = dr.Cells(1).Value.ToString();
                        frmServices.txtCustomerName.Text = dr.Cells(2).Value.ToString();
                        frmServices.txtContactNo.Text = dr.Cells(8).Value.ToString();
                        lblSet.Text = "";
                    }*/

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            AddCustomer addCustomer = new AddCustomer();
            addCustomer.lblSet.Text = "TOPOS";
            addCustomer.Show();
        }

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
