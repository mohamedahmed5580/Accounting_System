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
    public partial class SalesmanRecord_2 : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public SalesmanRecord_2()
        {
            InitializeComponent();
            Getdata();
            txtSalesmanName.TextChanged += new EventHandler(txtSalesmanName_TextChanged);
            txtCity.TextChanged += new EventHandler(txtCity_TextChanged);
            txtContactNo.TextChanged += new EventHandler(txtContactNo_TextChanged);
            dgw.MouseDoubleClick += new MouseEventHandler(dgw_MouseClick);
            dgw.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dgw_RowPostPaint);
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
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr.GetValue(0), rdr.GetValue(1), rdr.GetValue(2), rdr.GetValue(3), rdr.GetValue(4), rdr.GetValue(5), rdr.GetValue(6), rdr.GetValue(7), rdr.GetValue(8), rdr.GetValue(9), rdr.GetValue(10), rdr.GetValue(11));
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
        private void txtSalesmanName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT SM_ID, RTRIM(Salesman_ID), RTRIM([Name]), RTRIM(Address), RTRIM(City), RTRIM(State), RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID), CommissionPer, RTRIM(Remarks), Photo " +
                                   "FROM Salesman " +
                                   "WHERE Name LIKE '%' + @Name + '%' " +
                                   "ORDER BY Name";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Name", "%" + txtSalesmanName.Text + "%");
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
                    string query = "SELECT SM_ID, RTRIM(Salesman_ID), RTRIM([Name]), RTRIM(Address), RTRIM(City), RTRIM(State), RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID), CommissionPer, RTRIM(Remarks), Photo " +
                                   "FROM Salesman " +
                                   "WHERE City LIKE '%' + @City + '%' " +
                                   "ORDER BY Name";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@City", "%" + txtCity.Text + "%");
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
                    string query = "SELECT SM_ID, RTRIM(Salesman_ID), RTRIM([Name]), RTRIM(Address), RTRIM(City), RTRIM(State), RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID), CommissionPer, RTRIM(Remarks), Photo " +
                                   "FROM Salesman " +
                                   "WHERE ContactNo LIKE '%' + @ContactNo + '%' " +
                                   "ORDER BY Name";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ContactNo", "%"+ txtContactNo.Text + "%");
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
        private void dgw_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (dgw.Rows.Count > 0)
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];
                    // Uncomment if lblSet is used
                    // if (string.IsNullOrEmpty(lblSet.Text))
                    {
                        Payment_2 frmPayment_2 = Payment_2.Instance;
                        frmPayment_2.Show();
                        this.Hide();
                        frmPayment_2.txtSM_ID.Text = dr.Cells[0].Value.ToString();
                        frmPayment_2.txtSalesmanID.Text = dr.Cells[1].Value.ToString();
                        frmPayment_2.txtSalesman.Text = dr.Cells[2].Value.ToString();
                        frmPayment_2.txtCommissionPer.Text = dr.Cells[9].Value.ToString();
                        lblSet.Text = "";
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
                // Measure the width of the row number
                SizeF size = g.MeasureString(strRowNumber, this.Font);

                // Adjust the width of the row header if necessary
                if (dgw.RowHeadersWidth < (int)(size.Width + 20))
                {
                    dgw.RowHeadersWidth = (int)(size.Width + 20);
                }

                // Draw the row number in the row header
                // Note: SystemBrushes.ControlText should not be disposed of
                Brush b = SystemBrushes.ControlText;
                g.DrawString(strRowNumber, this.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2));
            }*/
        }

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
