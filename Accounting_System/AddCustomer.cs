using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;
using Pharmacy.DL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Accounting_System.POS;

namespace Accounting_System
{
    public partial class AddCustomer : Form
    {
        SqlConnection cn = new SqlConnection(DataAccessLayer.Con());
        public static AddCustomer _instance;
        public static AddCustomer instance;
        public static AddCustomer Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new AddCustomer();
                }
                return _instance;
            }

        }

        public AddCustomer()
        {
            InitializeComponent();
            instance=this;
        }
        public static class SessionData
        {
            public static string CustomerName { get; set; }
            public static string SalesManName { get; set; }
        }
        public void LoadCustomerData(string id)
        {
            // Fetch customer data by ID and populate fields
            try
            {
                string query = "SELECT * FROM Customer WHERE ID = @ID";
                SqlParameter[] parameters = { new SqlParameter("@ID", SqlDbType.Int) { Value = id } };

                if (DataAccessLayer.ExecuteTable(query, CommandType.Text, parameters).Rows.Count > 0)
                {
                    DataRow row = DataAccessLayer.ExecuteTable(query, CommandType.Text, parameters).Rows[0];

                    txtCustomerID.Text = row["رقم العميل"].ToString();
                    txtCustomerName.Text = row["اسم العميل"].ToString();
                    rbMale.Checked = row["Gender"].ToString() == "Male";
                    rbFemale.Checked = row["Gender"].ToString() == "Female";
                    txtAddress.Text = row["Address"].ToString();
                    txtCity.Text = row["City"].ToString();
                    cmbState.Text = row["State"].ToString();
                    txtZipCode.Text = row["ZipCode"].ToString();
                    txtContactNo.Text = row["ContactNo"].ToString();
                    txtEmailID.Text = row["EmailID"].ToString();
                    txtRemarks.Text = row["Remarks"].ToString();
                    // Handle image loading if required
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
         public void UpdatePictureBox(Bitmap image)
        {
            // Assuming pictureBox1 is the PictureBox you want to update
            pictureBox1.Image = image;
        }
        public void LogFunc(string st1, string st2)
        {
            try
            {
                DataAccessLayer.cn.Open();

                string cb = "INSERT INTO Logs(UserID, Date, Operation) VALUES (@d1, @d2, @d3)";
                using (SqlCommand cmd = new SqlCommand(cb, DataAccessLayer.cn))
                {
                    cmd.Parameters.AddWithValue("@d1", st1);
                    cmd.Parameters.AddWithValue("@d2", DateTime.Now);
                    cmd.Parameters.AddWithValue("@d3", st2);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                DataAccessLayer.cn.Close();
            }

        }

        public void LedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g, string h)
        {
            try
            {
                DataAccessLayer.cn.Open();
                string cb = "INSERT INTO LedgerBook(Date, Name, LedgerNo, Label, Debit, Credit, PartyID, Manual_Inv) VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8)";
                using (SqlCommand cmd = new SqlCommand(cb, DataAccessLayer.cn))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.Parameters.AddWithValue("@d2", b);
                    cmd.Parameters.AddWithValue("@d3", c);
                    cmd.Parameters.AddWithValue("@d4", d);
                    cmd.Parameters.AddWithValue("@d5", e);
                    cmd.Parameters.AddWithValue("@d6", f);
                    cmd.Parameters.AddWithValue("@d7", g);
                    cmd.Parameters.AddWithValue("@d8", h);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                DataAccessLayer.cn.Close();
            }
        }

        public void FillState()
        {
            try
            {
                DataAccessLayer.cn.Open();
                SqlDataAdapter adp = new SqlDataAdapter();
                adp.SelectCommand = new SqlCommand("SELECT DISTINCT RTRIM(State) FROM Customer ORDER BY 1", DataAccessLayer.cn);
                DataSet ds = new DataSet("ds");
                adp.Fill(ds);
                System.Data.DataTable dtable = ds.Tables[0];
                cmbState.Items.Clear();
                foreach (DataRow drow in dtable.Rows)
                {
                    cmbState.Items.Add(drow[0].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                DataAccessLayer.cn.Close();
            }
        }
        public void Reset()
        {
            rbMale.Checked = true;

            txtID.Text = GenerateID();
            txtCustomerID.Text = "C-" + GenerateID();
            txtCustomerName.Text = "";
            txtAddress.Text = "";
            txtRemarks.Text = "";
            txtCustomerID.Text = "";
            txtContactNo.Text = "";
            txtOpeningBalance.Text = "0";
            cmbOpeningBalanceType.SelectedIndex = 0;
            txtEmailID.Text = "";
            txtZipCode.Text = "";
            txtCity.Text = "";
            txtCustomerName.Focus();

            auto();
            cmbState.Text = "";
            pictureBox1.Image = Properties.Resources.if_icons_user1;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string GenerateID()
        {
            string value = "000";
            try
            {
                DataAccessLayer.cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 ID FROM Customer ORDER BY ID DESC", DataAccessLayer.cn);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (rdr.HasRows)
                {
                    rdr.Read();
                    value = rdr["ID"].ToString();
                }
                rdr.Close();
                value = (int.Parse(value) + 1).ToString();
                if (int.Parse(value) <= 9)
                {
                    value = "000" + value;
                }
                else if (int.Parse(value) <= 99)
                {
                    value = "00" + value;
                }
                else if (int.Parse(value) <= 999)
                {
                    value = "0" + value;
                }
            }
            catch (Exception ex)
            {
                if (DataAccessLayer.cn.State == ConnectionState.Open)
                {
                    DataAccessLayer.cn.Close();
                }
                value = "000";
            }
            return value;
        }

        private void auto()
        {
            try
            {
                txtID.Text = GenerateID();
                txtCustomerID.Text = "C-" + GenerateID();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SetupSampleData()
        {
            // Use an OpenFileDialog to select the image
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        pictureBox1.Image = new Bitmap(openFileDialog.FileName); // Ensure the image path is correct
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            lblUser.Text = "admin"; // Example user, adjust as needed


        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            lblUser.Text = "admin"; // Example user, adjust as needed

            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("الرجاء كتابة اسم العميل", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCustomerName.Focus();
                return;
            }

            if (!rbMale.Checked && !rbFemale.Checked)
            {
                rbMale.Checked = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("الرجاء كتابة العنوان", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAddress.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtContactNo.Text))
            {
                MessageBox.Show("الرجاء كتابة رقم الهاتف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContactNo.Focus();
                return;
            }

            try
            {
                DataAccessLayer.cn.Open();
                string selectQuery = "SELECT RTRIM(Name) FROM Customer WHERE Name=@d1";
                using (SqlCommand selectCmd = new SqlCommand(selectQuery, DataAccessLayer.cn))
                {
                    selectCmd.Parameters.AddWithValue("@d1", txtCustomerName.Text);
                    using (SqlDataReader rdr = selectCmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            MessageBox.Show("بيانات هذا العميل تم تسجيلها من قبل", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                string gender = rbMale.Checked ? rbMale.Text : rbFemale.Text;

                string insertQuery = "INSERT INTO Customer(ID, CustomerID, [Name], Gender, Address, City, ContactNo, EmailID, Remarks, State, ZipCode, Photo, OpeningBalance, OpeningBalanceType, CustomerType) VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9, @d10, @d11, @d12, @d13, @d14, 'Regular')";
                using (SqlCommand insertCmd = new SqlCommand(insertQuery, DataAccessLayer.cn))
                {
                    insertCmd.Parameters.AddWithValue("@d1", txtID.Text);
                    insertCmd.Parameters.AddWithValue("@d2", txtCustomerID.Text);
                    insertCmd.Parameters.AddWithValue("@d3", txtCustomerName.Text);
                    insertCmd.Parameters.AddWithValue("@d4", gender);
                    insertCmd.Parameters.AddWithValue("@d5", txtAddress.Text);
                    insertCmd.Parameters.AddWithValue("@d6", txtCity.Text);
                    insertCmd.Parameters.AddWithValue("@d7", txtContactNo.Text);
                    insertCmd.Parameters.AddWithValue("@d8", txtEmailID.Text);
                    insertCmd.Parameters.AddWithValue("@d9", txtRemarks.Text);
                    insertCmd.Parameters.AddWithValue("@d10", cmbState.Text);
                    insertCmd.Parameters.AddWithValue("@d11", txtZipCode.Text);

                    MemoryStream ms = new MemoryStream();

                    if (pictureBox1.Image != null)
                    {

                        Bitmap bmpImage = new Bitmap(pictureBox1.Image);
                        bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        byte[] data = ms.GetBuffer();
                        SqlParameter p = new SqlParameter("@d12", SqlDbType.Image);
                        p.Value = data;
                        insertCmd.Parameters.Add(p);

                    }
                    else
                    {
                        // Set default image if no image is selected
                        Bitmap defaultBmpImage = new Bitmap(Properties.Resources.if_icons_user);
                        defaultBmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        byte[] data = ms.GetBuffer();
                        SqlParameter p = new SqlParameter("@d12", SqlDbType.Image);
                        p.Value = data;
                        insertCmd.Parameters.Add(p);
                    }


                    insertCmd.Parameters.AddWithValue("@d13", int.Parse(txtOpeningBalance.Text));
                    insertCmd.Parameters.AddWithValue("@d14", cmbOpeningBalanceType.Text);

                    insertCmd.ExecuteNonQuery();
                    
                }

                // Rest of the code...

                MessageBox.Show("تم الحفظ بنجاح", "سجلات العملاء", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Reset();
                if (lblSet.Text == "TOPOS")
                {
                    POS pOS = new POS();
                    SessionData.CustomerName = txtCustomerName.Text;
                    MessageBox.Show("SessionData.CustomerName: " + SessionData.CustomerName); // Debugging line

                    pOS.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (DataAccessLayer.cn.State == ConnectionState.Open)
                {
                    DataAccessLayer.cn.Close(); // Ensure the connection is closed
                }
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {

            SetupSampleData();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.if_icons_user1;

        }

        private void txtCustomerName_TextChanged(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnClose_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
        public void LedgerUpdate(DateTime a, string b, decimal e, decimal f, string g, string h, string i)
        {
            cn.Open();
            string cb = "Update LedgerBook set Date=@d1, Name=@d2,Debit=@d3,Credit=@d4,PartyID=@d5 where LedgerNo=@d6 and Label=@d7";
            SqlCommand cmd = new SqlCommand(cb);
            cmd.Parameters.AddWithValue("@d1", a);
            cmd.Parameters.AddWithValue("@d2", b);
            cmd.Parameters.AddWithValue("@d3", e);
            cmd.Parameters.AddWithValue("@d4", f);
            cmd.Parameters.AddWithValue("@d5", g);
            cmd.Parameters.AddWithValue("@d6", h);
            cmd.Parameters.AddWithValue("@d7", i);
            cmd.Connection = cn;
            cmd.ExecuteReader();
            cn.Close();
        }
        private void button11_Click(object sender, EventArgs e)
        {
            string gender = "";

            // Validation checks
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("الرجاء كتابة اسم العميل", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCustomerName.Focus();
                return;
            }

            if (!rbMale.Checked && !rbFemale.Checked)
            {
                MessageBox.Show("الرجاء اختيار النوع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("الرجاء كتابة العنوان", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAddress.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtContactNo.Text))
            {
                MessageBox.Show("الرجاء كتابة رقم الهاتف.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContactNo.Focus();
                return;
            }

            // Validate and parse ZipCode and OpeningBalance
            if (!decimal.TryParse(txtZipCode.Text, out decimal zipCode))
            {
                MessageBox.Show("الرجاء إدخال رمز بريدي صحيح", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtZipCode.Focus();
                return;
            }

            if (!decimal.TryParse(txtOpeningBalance.Text, out decimal openingBalance))
            {
                MessageBox.Show("الرجاء إدخال رصيد افتتاحي صحيح", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOpeningBalance.Focus();
                return;
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con())) // Ensure your actual connection string method is correct
                {
                    cn.Open();

                    // Update LedgerBook
                    string ledgerUpdateQuery = "UPDATE LedgerBook SET [Name] = @d3 WHERE PartyID = @d1 AND Name = @d2";
                    using (SqlCommand cmd = new SqlCommand(ledgerUpdateQuery, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtCustomerID.Text);
                        cmd.Parameters.AddWithValue("@d2", txtCustName.Text);
                        cmd.Parameters.AddWithValue("@d3", txtCustomerName.Text);
                        cmd.ExecuteNonQuery();
                    }

                    // Determine gender
                    gender = rbMale.Checked ? rbMale.Text : rbFemale.Text;

                    // Update Customer
                    string customerUpdateQuery = "UPDATE Customer SET CustomerID = @d2, [Name] = @d3, Gender = @d4, Address = @d5, City = @d6, ContactNo = @d7, EmailID = @d8, Remarks = @d9, State = @d10, ZipCode = @d11, Photo = @d12, CustomerType = 'Regular', OpeningBalance = @d13, OpeningBalanceType = @d14 WHERE ID = @d1";
                    using (SqlCommand cmd = new SqlCommand(customerUpdateQuery, cn))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                        cmd.Parameters.AddWithValue("@d2", txtCustomerID.Text);
                        cmd.Parameters.AddWithValue("@d3", txtCustomerName.Text);
                        cmd.Parameters.AddWithValue("@d4", gender);
                        cmd.Parameters.AddWithValue("@d5", txtAddress.Text);
                        cmd.Parameters.AddWithValue("@d6", txtCity.Text);
                        cmd.Parameters.AddWithValue("@d7", txtContactNo.Text);
                        cmd.Parameters.AddWithValue("@d8", txtEmailID.Text);
                        cmd.Parameters.AddWithValue("@d9", txtRemarks.Text);
                        cmd.Parameters.AddWithValue("@d10", cmbState.Text);
                        cmd.Parameters.AddWithValue("@d11", zipCode);  // Use parsed decimal value
                        cmd.Parameters.AddWithValue("@d13", openingBalance);  // Use parsed decimal value
                        cmd.Parameters.AddWithValue("@d14", cmbOpeningBalanceType.Text);

                        // Handle Photo
                        byte[] fileData1 = null;

                        if (pictureBox1.Image != null)
                        {
                            try
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    pictureBox1.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                    fileData1 = ms.ToArray(); // Convert to byte array
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error saving the image: {ex.Message}");
                                return; // Exit if there is an error with the image
                            }
                        }

                        if (fileData1 != null && fileData1.Length > 0)
                        {
                            cmd.Parameters.Add("@d12", SqlDbType.VarBinary).Value = fileData1;
                        }
                        else
                        {
                            cmd.Parameters.Add("@d12", SqlDbType.VarBinary).Value = DBNull.Value;
                        }

                        int rowsAffected = cmd.ExecuteNonQuery();
                        Debug.WriteLine("Rows affected: " + rowsAffected);
                    }

                    // Handle ledger update based on the opening balance type
                    if (cmbOpeningBalanceType.SelectedIndex == 0)
                    {
                        LedgerUpdate(DateTime.Today, "الرصيد الافتتاحي", openingBalance, 0m, txtCustomerID.Text, txtCustomerID.Text, "الرصيد الافتتاحي");
                    }
                    else if (cmbOpeningBalanceType.SelectedIndex == 1)
                    {
                        LedgerUpdate(DateTime.Today, "الرصيد الافتتاحي", 0m, openingBalance, txtCustomerID.Text, txtCustomerID.Text, "الرصيد الافتتاحي");
                    }

                    LogFunc(lblUser.Text, "updated the Customer having Customer ID '" + txtCustomerID.Text + "'");
                    MessageBox.Show("تم التعديل بنجاح", "سجلات العملاء", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FillState();
                    Reset();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"Exception: {ex.Message}\n{ex.StackTrace}");
            }


        }


        private void txtOpeningBalance_TextChanged(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                string deleteQuery = "DELETE FROM Customer WHERE CustomerID = @CustomerID";
                using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, cn))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", txtCustomerID.Text);
                        cmd.ExecuteNonQuery();
                    }
                    cn.Close();
                }

                MessageBox.Show("Customer deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                CustomerList customers = new CustomerList();
                customers.GetData();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label37_Click(object sender, EventArgs e)
        {

        }

        private void BStartCapture_Click(object sender, EventArgs e)
        {
           

        }

        private void AddCustomer_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CustomerList customers = new CustomerList();
            customers.GetData();
            customers.lblSet.Text = "Customer Entry";
            customers.ShowDialog();
        }

        private void AddCustomer_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (lblSet.Text== "TOPOS")
            {
                POS pos = new POS();
                pos.ShowDialog();

            }
        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
