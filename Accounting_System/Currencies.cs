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
using System.Xml.Linq;

namespace Accounting_System
{
    public partial class Currencies : Form
    {
        public Currencies()
        {
            InitializeComponent();
        }

        private void Panel4_Paint(object sender, PaintEventArgs e)
        {

        }
        private void GetData()
        {

            // Step 2: SQL query to fetch data from Currencies table
            string query = "SELECT * FROM Currencies";

            // Step 3: Create a DataTable to hold the data

            // Step 4: Create a connection to the database
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            dgw.Rows.Clear(); // Clear any existing rows in the DataGridView

                            while (rdr.Read())
                            {
                                // Add rows to the DataGridView with the retrieved data
                                dgw.Rows.Add(
                                    rdr["id"].ToString(),         // First column: ID
                                    rdr["Name"].ToString(),       // Second column: Name
                                    rdr["Price"].ToString()       // Third  column: hours
                                );
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Step 7: Handle any errors
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
            
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            // Step 1: Get the values from the textboxes
            string name = txtName.Text;
            int price;

            // Validate if price is a valid integer
            if (!int.TryParse(txtPrice.Text, out price))
            {
                MessageBox.Show("Please enter a valid price.");
                return;
            }


            // Step 3: SQL query for inserting the data into Currencies table
            string query = "INSERT INTO Currencies (Name, Price) VALUES (@Name, @Price)";

            // Step 4: Create a connection to the database and execute the query
            using (SqlConnection conn = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Step 5: Define parameters and assign values
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Price", price);

                        // Step 6: Execute the query
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Step 7: Display a success message if data is inserted
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("تمت اضافة العملة بنجاح.");
                            // Optionally, you can clear the textboxes here
                            txtName.Clear();
                            txtPrice.Clear();
                        }
                        else
                        {
                            MessageBox.Show("خطا في اضافة العمله.");
                        }
                    }
                    GetData();
                }
                catch (Exception ex)
                {
                    // Step 8: Handle any errors
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }

        }

        private void Currencies_Load(object sender, EventArgs e)
        {
            GetData();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteSelectedItem();
        }
        private void DeleteSelectedItem()
        {
            // Step 1: Check if a row is selected
            if (dgw.SelectedRows.Count > 0)
            {
                // Step 2: Get the ID of the selected item
                int selectedId = Convert.ToInt32(dgw.SelectedRows[0].Cells[0].Value);

                // Step 3: Define the SQL DELETE query
                string query = "DELETE FROM Currencies WHERE id = @id";

                // Step 4: Create a connection to the database
                using (SqlConnection conn = new SqlConnection(DataAccessLayer.Con()))
                {
                    try
                    {
                        conn.Open();

                        // Step 5: Create a SqlCommand to execute the DELETE query
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", selectedId);

                            // Step 6: Execute the query
                            int rowsAffected = cmd.ExecuteNonQuery();

                            // Step 7: Provide feedback and refresh data if deletion was successful
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("تم الحذف بنجاح.");
                                GetData(); // Refresh the DataGridView to show updated data
                            }
                            else
                            {
                                MessageBox.Show("لم يتم الحذف.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Step 8: Handle any errors
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            GetData();
            txtName.Clear();
            txtPrice.Clear();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateData();
        }

        private void UpdateData()
        {
            // Ensure a row is selected in the DataGridView
            if (dgw.CurrentRow != null)
            {
                // Get the selected row's ID
                string id = dgw.CurrentRow.Cells[0].Value.ToString();

                // Get the updated values from the textboxes
                string name = txtName.Text.Trim();
                string price = txtPrice.Text.Trim();

                // SQL query to update the selected row in the Currencies table
                string query = "UPDATE Currencies SET Name = @Name, Price = @Price WHERE id = @id";

                // Create a connection to the database
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    try
                    {
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            // Add parameters to the query to prevent SQL injection
                            cmd.Parameters.AddWithValue("@Name", name);
                            cmd.Parameters.AddWithValue("@Price", price);
                            cmd.Parameters.AddWithValue("@id", id);

                            // Execute the update query
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("تم التعديل بنجاح.");
                                GetData(); // Refresh the DataGridView with updated data
                            }
                            else
                            {
                                MessageBox.Show("No data was updated.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle any errors
                        MessageBox.Show("An error occurred while updating: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a row to update.");
            }
        }

        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (dgw.Rows.Count > 0)
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];
                    txtID.Text = dr.Cells[0].Value.ToString();
                    // Get the updated values from the textboxes
                    txtName.Text = dr.Cells[1].Value.ToString();
                    txtPrice.Text = dr.Cells[2].Value.ToString();

                    btnUpdate.Enabled = true;
                    btnDelete.Enabled = true;
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
