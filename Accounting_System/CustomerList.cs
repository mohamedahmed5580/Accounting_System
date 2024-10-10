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
using Excel = Microsoft.Office.Interop.Excel;
namespace Accounting_System
{
    public partial class CustomerList : Form
    {
        public CustomerList()
        {
            InitializeComponent();
            dgw.RowPostPaint += Dgw_RowPostPaint;
        }

        private void basic_Load(object sender, EventArgs e)
        {
            GetData();

        }

        private void gunaContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        internal partial class SurroundingClass : Form
        {
            // Add your ExportExcel method implementation here
        }




        public void GetData()
        {

            try
            {
                string query = "SELECT RTRIM(ID), RTRIM(CustomerID), RTRIM([Name]), RTRIM(Gender), RTRIM(Address), RTRIM(City), RTRIM(State), RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID), RTRIM(Remarks), OpeningBalance, OpeningBalanceType, Photo FROM Customer WHERE CustomerType='Regular' order by ID";
                DataTable dt = DataAccessLayer.ExecuteTable(query, CommandType.Text);
                dgw.Rows.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    DataGridViewRow dgvRow = new DataGridViewRow();
                    dgvRow.CreateCells(dgw);

                    dgvRow.Cells[0].Value = row[0].ToString();
                    dgvRow.Cells[1].Value = row[1].ToString();
                    dgvRow.Cells[2].Value = row[2].ToString();
                    dgvRow.Cells[3].Value = row[3].ToString();
                    dgvRow.Cells[4].Value = row[4].ToString();
                    dgvRow.Cells[5].Value = row[5].ToString();
                    dgvRow.Cells[6].Value = row[6].ToString();
                    dgvRow.Cells[7].Value = row[7].ToString();
                    dgvRow.Cells[8].Value = row[8].ToString();
                    dgvRow.Cells[9].Value = row[9].ToString();
                    dgvRow.Cells[10].Value = row[10].ToString();
                    dgvRow.Cells[11].Value = row[11].ToString();
                    dgvRow.Cells[12].Value = row[12].ToString();

                    // Convert byte array to image for Photo column
                    if (row[13] != DBNull.Value)
                    {
                        byte[] photoArray = (byte[])row[13];
                        using (MemoryStream ms = new MemoryStream(photoArray))
                        {
                            dgvRow.Cells[13].Value = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        dgvRow.Cells[13].Value = Properties.Resources.if_icons_user;
                    }

                    dgw.Rows.Add(dgvRow);
                }

                dgw.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Dgw_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            string strRowNumber = (e.RowIndex + 1).ToString();
            SizeF size = e.Graphics.MeasureString(strRowNumber, this.Font);
            if (dgw.RowHeadersWidth < Convert.ToInt32(size.Width + 20))
            {
                dgw.RowHeadersWidth = Convert.ToInt32(size.Width + 20);
            }
            Brush b = SystemBrushes.ControlText;
            e.Graphics.DrawString(strRowNumber, this.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + (e.RowBounds.Height - size.Height) / 2);
        }






        private void SearchData(string column, string searchText)
        {
            try
            {
                string query = $"SELECT RTRIM(ID), RTRIM(CustomerID), RTRIM([Name]), RTRIM(Gender), RTRIM(Address), RTRIM(City), RTRIM(State), RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID), RTRIM(Remarks) FROM Customer WHERE CustomerType='Regular' AND {column} LIKE @searchText ORDER BY Name";
                SqlParameter[] parameters = { DataAccessLayer.CreateParameter("@searchText", SqlDbType.VarChar, $"%{searchText}%") };
                DataTable dt = DataAccessLayer.ExecuteTable(query, CommandType.Text, parameters);
                dgw.Rows.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    dgw.Rows.Add(row.ItemArray);
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
            GetData();
        }



        private void Button2_Click(object sender, EventArgs e)
        {
            // Implement your ExportExcel method here
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            // Implementation here if needed
        }
        private void txtCustomerName_TextChanged(object sender, EventArgs e)
        {
            SearchData("Name", txtCustomerName.Text);

        }

        private void txtCity_TextChanged(object sender, EventArgs e)
        {
            SearchData("City", txtCity.Text);

        }

        private void txtContactNo_TextChanged_1(object sender, EventArgs e)
        {
            SearchData("ContactNo", txtContactNo.Text);

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Reset();

        }

        private void Button2_Click_1(object sender, EventArgs e)
        {
            ExportExcel(dgw);
        }
        public void ExportExcel(DataGridView dataGridView)
        {
            int rowsTotal, colsTotal;
            int I, j, iC;
            Cursor.Current = Cursors.WaitCursor;
            Excel.Application xlApp = new Excel.Application();

            try
            {
                Excel.Workbook excelBook = xlApp.Workbooks.Add();
                Excel.Worksheet excelWorksheet = (Excel.Worksheet)excelBook.Worksheets[1];
                xlApp.Visible = true;

                rowsTotal = dataGridView.RowCount;
                colsTotal = dataGridView.Columns.Count - 3;

                excelWorksheet.Cells.Select();
                excelWorksheet.Cells.Delete();

                for (iC = 0; iC <= colsTotal; iC++)
                {
                    excelWorksheet.Cells[1, iC + 1].Value = dataGridView.Columns[iC].HeaderText;
                }

                for (I = 0; I < rowsTotal; I++)
                {
                    for (j = 0; j <= colsTotal; j++)
                    {
                        excelWorksheet.Cells[I + 2, j + 1].Value = dataGridView.Rows[I].Cells[j].Value;
                    }
                }

               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                xlApp = null;
            }
        }

        private void Panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
           

        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure at least one row is selected
                if (dgw.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a customer to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get the selected row
                DataGridViewRow selectedRow = dgw.SelectedRows[0];

                // Ensure the column "رقم العميل" exists
                if (selectedRow.Cells["Column2"] == null)
                {
                    MessageBox.Show("The selected row does not have a 'رقم العميل' column.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Retrieve CustomerID from the selected row
                string customerID = selectedRow.Cells["Column2"].Value.ToString();

                // Prepare SQL DELETE command
                string deleteQuery = "DELETE FROM Customer WHERE CustomerID = @CustomerID";

                using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, cn))
                    {
                        // Add parameter with proper data type
                        cmd.Parameters.AddWithValue("@CustomerID", customerID);

                        // Execute the delete command
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            MessageBox.Show("No customer was deleted. Please check the CustomerID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }

                // Notify user of success
                MessageBox.Show("Customer deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the customer list
                CustomerList customers = new CustomerList();
                customers.GetData();

                // Close the form
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"SQL error occurred: {sqlEx.Message}", "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                GetData();
            }
        }


        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                
                if (dgw.SelectedRows.Count > 0 && lblSet.Text == "Customer Entry")
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];


                    // Populate fields in the AddCustomer form
                    AddCustomer.instance.txtID.Text = dr.Cells[0].Value?.ToString() ?? "";
                    AddCustomer.instance.txtCustomerID.Text = dr.Cells[1].Value?.ToString() ?? "";
                    AddCustomer.instance.txtCustomerName.Text = dr.Cells[2].Value?.ToString() ?? "";
                    AddCustomer.instance.txtCustName.Text = dr.Cells[2].Value?.ToString() ?? "";

                    // Set gender radio buttons
                    string gender = dr.Cells[3].Value?.ToString();
                    if (gender == "ذكر")
                    {
                        AddCustomer.instance.rbMale.Checked = true;
                    }
                    else if (gender == "أنثي")
                    {
                        AddCustomer.instance.rbFemale.Checked = true;
                    }

                    AddCustomer.instance.txtAddress.Text = dr.Cells[4].Value?.ToString() ?? "";
                    AddCustomer.instance.txtCity.Text = dr.Cells[5].Value?.ToString() ?? "";
                    AddCustomer.instance.cmbState.Text = dr.Cells[6].Value?.ToString() ?? "";
                    AddCustomer.instance.txtZipCode.Text = dr.Cells[7].Value?.ToString() ?? "";
                    AddCustomer.instance.txtContactNo.Text = dr.Cells[8].Value?.ToString() ?? "";
                    AddCustomer.instance.txtEmailID.Text = dr.Cells[9].Value?.ToString() ?? "";
                    AddCustomer.instance.txtRemarks.Text = dr.Cells[10].Value?.ToString() ?? "";
                    AddCustomer.instance.txtOpeningBalance.Text = dr.Cells[11].Value?.ToString() ?? "";
                    AddCustomer.instance.cmbOpeningBalanceType.Text = dr.Cells[12].Value?.ToString() ?? "";

                    // Load customer image if available
                    if (dr.Cells[11].Value != null && dr.Cells[13].Value is byte[] data)
                    {
                        using (var ms = new MemoryStream(data))
                        {
                            AddCustomer.instance.pictureBox1.Image = Image.FromStream(ms);
                        }
                    }

                    // Set button states
                    AddCustomer.instance.button11.Enabled = true;
                    AddCustomer.instance.button10.Enabled = true;
                    AddCustomer.instance.button8.Enabled = false;
                    this.Close();

                    // Clear lblSet text and show the AddCustomer form
                }
                else if (dgw.Rows.Count > 0 && dgw.SelectedRows.Count > 0 && lblSet.Text == "Billing")
                {
                   /* POS frmPOS = new POS();
                    frmPOS.txtCID.Text = dr.Cells[0].Value.ToString();
                    frmPOS.txtCustomerID.Text = dr.Cells[1].Value.ToString();
                    frmPOS.txtCustomerName.Text = dr.Cells[2].Value.ToString();
                    frmPOS.txtContactNo.Text = dr.Cells[8].Value.ToString();
                    frmPOS.txtCustomerName.ReadOnly = true;
                    frmPOS.txtContactNo.ReadOnly = true;
                    frmPOS.Show();
                    this.Hide();*/
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void Panel5_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
