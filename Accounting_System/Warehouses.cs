using Microsoft.Office.Interop.Excel;
using SkiaSharp;
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
    public partial class Warehouses : Form
    {
        public Warehouses()
        {
            InitializeComponent();
        }

        private void Warehouses_Load(object sender, EventArgs e)
        {
            Reset();

        }
        private string GenerateID()
        {
            string value = "000";
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    // Fetch the latest ID from the database
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 WID FROM Warehouses ORDER BY WID DESC", con))
                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            value = rdr["WID"].ToString();
                        }
                    }

                    // Increase the ID by 1
                    int numericValue = int.Parse(value);
                    numericValue += 1;
                    value = numericValue.ToString("D5"); // Ensure the string is padded with leading zeros if necessary
                }
                catch (Exception ex)
                {
                    // If an error occurs, set the value to "0000"
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                    value = "000";
                }
            }
            return value;
        }
        private void auto()
        {
            try
            {
                txtID.Text = GenerateID();
                txtWCode.Text = "W-" + GenerateID();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Getdata();

        }
        public void Reset()
        {
            auto();
            txtName.Text = "";
            txtloc.Text = "";
            btnDelete.Enabled = false;
            btnUpdate.Enabled = false;
            btnSave.Enabled = true;
        }

        public void Getdata()
        {


            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "Select WID,WarehouseCode,WarehouseName,WarehouseLoc from Warehouses";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        dgw.Rows.Clear();
                        while (rdr.Read())
                        {
                            dgw.Rows.Add(
                                rdr[0].ToString(),
                                rdr[1].ToString(),
                                rdr[2].ToString(),
                                rdr[3].ToString()  
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Please enter a warehouse ID.");
                return;
            }
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("Please enter a warehouse Name.");
                return;
            }
            if (string.IsNullOrEmpty(txtWCode.Text))
            {
                MessageBox.Show("Please enter a warehouse Code.");
                return;
            }
            if (string.IsNullOrEmpty(txtloc.Text))
            {
                MessageBox.Show("Please enter a warehouse Location.");
                return;
            }

            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    con.Open();

                    // Enable IDENTITY_INSERT
                    string enableIdentityInsert = "SET IDENTITY_INSERT Warehouses ON;";
                    using (SqlCommand cmdEnable = new SqlCommand(enableIdentityInsert, con))
                    {
                        cmdEnable.ExecuteNonQuery();
                    }

                    string query = "INSERT INTO Warehouses (WID, WarehouseName, WarehouseLoc, WarehouseCode) VALUES (@id, @Name, @Loc, @Code)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", Convert.ToInt16(txtID.Text)); // Explicitly set ID
                        cmd.Parameters.AddWithValue("@Name", txtName.Text);
                        cmd.Parameters.AddWithValue("@Loc", txtloc.Text);
                        cmd.Parameters.AddWithValue("@Code", txtWCode.Text);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("تم الحفظ بنجاح.");
                    }

                    // Disable IDENTITY_INSERT
                    //string disableIdentityInsert = "SET IDENTITY_INSERT Warehouses OFF;";
                    //using (SqlCommand cmdDisable = new SqlCommand(disableIdentityInsert, con))
                    //{
                    //    cmdDisable.ExecuteNonQuery();
                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                Reset();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(txtID.Text))
            {
                // Call the delete method
                MessageBox.Show("Please select a warehouse to delete.");
            }
          
            if (string.IsNullOrEmpty(txtWCode.Text))
            {
                // Call the delete method
                MessageBox.Show("Please select a warehouse Code to delete.");
                return;
            }
            if (string.IsNullOrEmpty(txtName.Text))
            {
                // Call the delete method
                MessageBox.Show("Please select a warehouse Name to delete.");
                return;
            }
            if (string.IsNullOrEmpty(txtloc.Text))
            {
                // Call the delete method
                MessageBox.Show("Please Write a warehouse Loc to delete.");
                return;
            }

            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    con.Open();
                    string query = "DELETE FROM Warehouses WHERE WID = @ID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ID", Convert.ToInt16( txtID.Text) );

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Warehouse deleted successfully!");
                        }
                        else
                        {
                            MessageBox.Show("Warehouse not found.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            Reset();
        }

        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Check if the user double-clicked on a row
            if (dgw.CurrentRow != null )
            {
                // Get the selected row
                DataGridViewRow selectedRow = dgw.SelectedRows[0];

                // Populate the text fields with the selected row data
                txtID.Text = selectedRow.Cells[0].Value.ToString();              // Warehouse ID
                txtName.Text = selectedRow.Cells[2].Value.ToString();  // Warehouse Name
                txtloc.Text = selectedRow.Cells[3].Value.ToString();    // Warehouse Location
                txtWCode.Text = selectedRow.Cells[1].Value.ToString(); // Warehouse Code

                btnDelete.Enabled = true;
                btnUpdate.Enabled = true;
                btnSave.Enabled = false; 
            }
            if (dgw.CurrentRow != null && lbSet.Text == "WTransport")
            {
                DataGridViewRow selectedRow = dgw.SelectedRows[0];
                Warehouses_transportation.instance.TtxtWIDTxt.Text = selectedRow.Cells[0].Value.ToString();
                Warehouses_transportation.instance.autherWarehouse.Text = selectedRow.Cells[1].Value.ToString();
                Warehouses_transportation.instance.WName.Text = selectedRow.Cells[2].Value.ToString();
                this.Close();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                // Call the delete method
                MessageBox.Show("Please select a warehouse to Updatev.");
            }

            if (string.IsNullOrEmpty(txtWCode.Text))
            {
                // Call the delete method
                MessageBox.Show("Please select a warehouse Code to Update.");
                return;
            }
            if (string.IsNullOrEmpty(txtName.Text))
            {
                // Call the delete method
                MessageBox.Show("Please select a warehouse Name to Update.");
                return;
            }
            if (string.IsNullOrEmpty(txtloc.Text))
            {
                // Call the delete method
                MessageBox.Show("Please Write a warehouse Loc to Update.");
                return;
            }
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    con.Open();
                    // Update query to modify the warehouse details based on the WID
                    string query = @"UPDATE Warehouses
                                 SET WarehouseName = @Name,
                                     WarehouseLoc = @Location,
                                     WarehouseCode = @Code
                                 WHERE WID = @ID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Add parameters to avoid SQL injection
                        cmd.Parameters.AddWithValue("@ID",Convert.ToInt16( txtID.Text));
                        cmd.Parameters.AddWithValue("@Name", txtName.Text);
                        cmd.Parameters.AddWithValue("@Location", txtloc.Text);
                        cmd.Parameters.AddWithValue("@Code", txtWCode.Text);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Warehouse updated successfully!");
                        }
                        else
                        {
                            MessageBox.Show("Warehouse not found.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            Reset();
        }

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void lbSet_TextChanged(object sender, EventArgs e)
        {

        }
    }
    }

