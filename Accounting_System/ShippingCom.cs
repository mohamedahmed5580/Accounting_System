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
    public partial class ShippingCom : Form
    {
        string connectionString = DataAccessLayer.Con();
        public ShippingCom()
        {
            InitializeComponent();
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnSelection_Click(object sender, EventArgs e)
        {
                
        }

        private void Panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ShippingCom_Load(object sender, EventArgs e)
        {
            LoadDataGrid();
            Reset();
        }

        private string GenerateSHID()
        {
            string value = "0000";
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT TOP 1 SHID FROM ShippingCom ORDER BY SHID DESC", con);
                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    if (rdr.HasRows)
                    {
                        rdr.Read();
                        value = rdr["SHID"].ToString();
                    }
                    rdr.Close();

                    // Increment the ID by 1
                    int numericValue = int.Parse(value);
                    numericValue++;
                    value = numericValue.ToString("D4"); // Format as a 4-digit number with leading zeros
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                value = "0000";
            }
            return value;
        }

        public void auto()
        {
            try
            {
                txtSupplierID.Text = "SH-" + GenerateSHID();
                txtID.Text = GenerateSHID();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "INSERT INTO ShippingCom (ShappingCode, ShappingName, Location, Phone, Email) VALUES (@ShappingCode,@ShappingName, @Location, @Phone, @Email)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ShappingCode", txtSupplierID.Text);
                        cmd.Parameters.AddWithValue("@ShappingName", txtSupplierName.Text);
                        cmd.Parameters.AddWithValue("@Location", txtAddress.Text);
                        cmd.Parameters.AddWithValue("@Phone", txtContactNo.Text);
                        cmd.Parameters.AddWithValue("@Email", txtEmailID.Text);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("تم الحفظ بنجاح", "شركات الشحن", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDataGrid();
                        Reset();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "UPDATE ShippingCom SET ShappingName = @ShappingName, Location = @Location, Phone = @Phone, Email = @Email WHERE SHID = @SHID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SHID", Convert.ToInt32(txtID.Text));
                        cmd.Parameters.AddWithValue("@ShappingName", txtSupplierName.Text);
                        cmd.Parameters.AddWithValue("@Location", txtAddress.Text);
                        cmd.Parameters.AddWithValue("@Phone", txtContactNo.Text);
                        cmd.Parameters.AddWithValue("@Email", txtEmailID.Text);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("تم تعديل البيانات بنجاح", "شركات الشحن", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDataGrid();
                        Reset();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "DELETE FROM ShippingCom WHERE SHID = @SHID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SHID", Convert.ToInt32(txtID.Text));
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("تم حذف البيانات بنجاح", "شركات الشحن", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDataGrid();
                    }
                }
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void LoadDataGrid()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT SHID,ShappingCode, ShappingName , Location , Phone, Email FROM ShippingCom";
                    using (SqlCommand da = new SqlCommand(query, con))
                    {
                        using (SqlDataReader rdr = da.ExecuteReader())
                        {
                            dgw.Rows.Clear(); // Clear any existing rows in the DataGridView

                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Reset()
        {
            txtSupplierID.Clear();
            txtSupplierName.Clear();
            txtAddress.Clear();
            txtContactNo.Clear();
            txtEmailID.Clear();
            txtCustomerName.Clear();
            textBox1.Clear();
            auto();
            LoadDataGrid();
        }

        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dgw.CurrentRow != null && lblUser.Text=="user" ) // Ensure a row is selected
            {

                txtID.Text = dgw.CurrentRow.Cells[0].Value.ToString();
                txtSupplierID.Text = dgw.CurrentRow.Cells[1].Value.ToString();
                txtSupplierName.Text = dgw.CurrentRow.Cells[2].Value.ToString();
                txtAddress.Text = dgw.CurrentRow.Cells[3].Value.ToString();
                txtContactNo.Text = dgw.CurrentRow.Cells[4].Value.ToString();
                txtEmailID.Text = dgw.CurrentRow.Cells[5].Value.ToString();
                btnDelete.Enabled = true;
                btnUpdate.Enabled = true;
            }
            else if (dgw.CurrentRow != null && lblUser.Text == "ShippingPyment")
            {
                DataGridViewRow ds = dgw.SelectedRows[0];
                ShippingCom_pyment.instance.TextBox2.Text = ds.Cells[0].Value.ToString();
                ShippingCom_pyment.instance.txtSupplierID.Text = ds.Cells[1].Value.ToString();
                ShippingCom_pyment.instance.txtSupplierName.Text = ds.Cells[2].Value.ToString();
                ShippingCom_pyment.instance.GetCompanyBalance();
                this.Hide();
            }
            else if (dgw.CurrentRow != null && lblUser.Text == "Pymentinv")
            {
                DataGridViewRow ds = dgw.SelectedRows[0];
                Pymentinvoice.instance.textBox2.Text = ds.Cells[2].Value.ToString();
                this.Hide();
            }

        }

        private void GroupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void txtCustomerName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT SHID,ShappingCode, ShappingName, Location, Phone, Email FROM ShippingCom WHERE ShappingName LIKE @Name";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Name", "%" + txtCustomerName.Text + "%");

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            dgw.Rows.Clear(); // Clear any existing rows in the DataGridView

                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT SHID,ShappingCode, ShappingName, Location, Phone, Email FROM ShippingCom WHERE Phone LIKE @Phone OR SHID = @StockID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Phone", "%" + textBox1.Text + "%");

                        // Check if txtID has a valid integer value for STOCK_ID
                        if (int.TryParse(txtID.Text, out int stockID))
                        {
                            cmd.Parameters.AddWithValue("@StockID", stockID);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@StockID", DBNull.Value);
                        }

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            dgw.Rows.Clear(); // Clear any existing rows in the DataGridView

                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
