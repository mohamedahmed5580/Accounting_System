using Microsoft.Office.Interop.Excel;
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
using static Accounting_System.POS;

namespace Accounting_System
{
    public partial class SalesManScreen : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public SalesManScreen()
        {
            InitializeComponent();
        }

        private void Panel2_Paint(object sender, PaintEventArgs e)
        {

        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void Getdata()
        {
            try
            {

                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT SM_ID,RTRIM(Salesman_ID),RTRIM([Name]), RTRIM(Address),RTRIM(City),RTRIM(State),RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID),CommissionPer,RTRIM(Remarks),Photo from Salesman order by name", con);
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

        private void txtSalesmanName_TextChanged(object sender, EventArgs e)
        {
            try
            {

                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT SM_ID,RTRIM(Salesman_ID),RTRIM([Name]), RTRIM(Address),RTRIM(City),RTRIM(State),RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID),CommissionPer,RTRIM(Remarks),Photo from Salesman where name like '%" + txtSalesmanName.Text + "%' order by name", con);
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
                SqlCommand cmd = new SqlCommand("SELECT SM_ID,RTRIM(Salesman_ID),RTRIM([Name]), RTRIM(Address),RTRIM(City),RTRIM(State),RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID),CommissionPer,RTRIM(Remarks),Photo from Salesman where City like '%" + txtCity.Text + "%' order by Name", con);
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
            txtSalesmanName.Text = "";
            txtContactNo.Text = "";
            txtCity.Text = "";
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

                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT SM_ID,RTRIM(Salesman_ID),RTRIM([Name]), RTRIM(Address),RTRIM(City),RTRIM(State),RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID),CommissionPer,RTRIM(Remarks),Photo from Salesman where ContactNo like '%" + txtContactNo.Text + "%' order by Name", con);
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


        private void SalesManScreen_Load(object sender, EventArgs e)
        {
            Getdata();
        }

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (dgw.Rows.Count > 0 && lblSet.Text == "Salesman Entry")
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];

                    // Create a new instance each time
                    SalesMan frmSalesman = System.Windows.Forms.Application.OpenForms.OfType<SalesMan>().FirstOrDefault() ?? new SalesMan();


                    frmSalesman.txtID.Text = dr.Cells[0].Value?.ToString();
                    frmSalesman.txtSalesmanID.Text = dr.Cells[1].Value?.ToString();
                    frmSalesman.txtSalesmanName.Text = dr.Cells[2].Value?.ToString();
                    frmSalesman.txtAddress.Text = dr.Cells[3].Value?.ToString();
                    frmSalesman.txtCity.Text = dr.Cells[4].Value?.ToString();
                    frmSalesman.cmbState.Text = dr.Cells[5].Value?.ToString();
                    frmSalesman.txtZipCode.Text = dr.Cells[6].Value?.ToString();
                    frmSalesman.txtContactNo.Text = dr.Cells[7].Value?.ToString();
                    frmSalesman.txtEmailID.Text = dr.Cells[8].Value?.ToString();
                    frmSalesman.txtCommissionPer.Text = dr.Cells[9].Value?.ToString();
                    frmSalesman.txtRemarks.Text = dr.Cells[10].Value?.ToString();
                    byte[] data = (byte[])dr.Cells[11].Value;
                    var ms = new MemoryStream(data);
                    frmSalesman.Picture.Image = Image.FromStream(ms);
                    frmSalesman.btnUpdate.Enabled = true;
                    frmSalesman.btnDelete.Enabled = true;
                    frmSalesman.btnSave.Enabled = false;

                    // Show the new form instance
                    frmSalesman.Show();
                    this.Close();
                }
                else if (dgw.Rows.Count > 0 && lblSet.Text == "Billing")
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];

                    // Create a new instance each time

                    POS.instance.Show();
                    POS.instance.txtSM_ID.Text = dr.Cells[0].Value.ToString();
                    POS.instance.txtSalesmanID.Text = dr.Cells[1].Value.ToString();
                    POS.instance.txtSalesman.Text = dr.Cells[2].Value.ToString();
                    POS.instance.txtCommissionPer.Text = dr.Cells[9].Value.ToString();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgw_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dgw_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void Panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            SalesMan sm = new SalesMan();
            sm.Show();
        }
    }
}
