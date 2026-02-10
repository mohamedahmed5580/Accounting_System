using SixLabors.ImageSharp.Drawing;
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
    public partial class ShippingCom_pyment : Form
    {
        SqlConnection cn = new SqlConnection(DataAccessLayer.Con());

        public string connectionString = DataAccessLayer.Con();
        private decimal num1, num2, num3, num4;
        private string str;
        public static ShippingCom_pyment instance;
        public ShippingCom_pyment()
        {
            InitializeComponent();
            instance = this;
        }


        private void btnSelection_Click(object sender, EventArgs e)
        {
            ShippingCom shippingCom = new ShippingCom();
            shippingCom.lblUser.Text = "ShippingPyment";
            shippingCom.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PymentinvoiceScreen pymentinvoiceScreen = new PymentinvoiceScreen();
            pymentinvoiceScreen.lblSet.Text = "Shipping";
            pymentinvoiceScreen.Show();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void ShippingCom_pyment_Load(object sender, EventArgs e)
        {
            Reset();
        }
        private void Reset()
        {
            txtRemarks.Text = "";
            txtSupplierID.Text = "";
            txtSupplierName.Text = "";
            txtTransactionAmount.Text = "";
            cmbPaymentMode.SelectedIndex = 0;
            dtpTranactionDate.Value = DateTime.Today;
            lblBalance.Text = "0.000";
            txtSupplierID.Clear();
            txtSupplierName.Clear();
            txtRemarks.Clear();
            txtTransactionAmount.Clear();
            txtSupplierID.Clear();
            textBox4.Clear();
            textBox5.Clear();
            txtSupplierName.Clear();
            cmbPaymentMode.SelectedIndex = 0;
            lblSet.Text = "";
            btnSave.Enabled = true;
            btnDelete.Enabled = false;
            btnUpdate.Enabled = false;
            btnSelection.Enabled = true;
            auto();
        }
        public void auto()
        {
            try
            {
                txtTransactionNo.Text = "SHP-" + GenerateSHID();
                TextBox3.Text = GenerateSHID();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private string GenerateSHID()
        {
            string value = "0000";
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT TOP 1 SHPID FROM Shipping_Pyment ORDER BY SHPID DESC", con);
                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    if (rdr.HasRows)
                    {
                        rdr.Read();
                        value = rdr["SHPID"].ToString();
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
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Check if ST_ID exists in Stock table
                    string checkStockQuery = "SELECT COUNT(1) FROM Stock WHERE ST_ID = @ST_ID";
                    using (SqlCommand checkCmd = new SqlCommand(checkStockQuery, con))
                    {
                        if (int.TryParse(textBox4.Text, out int st_id))
                        {
                            checkCmd.Parameters.AddWithValue("@ST_ID", st_id);
                            int exists = (int)checkCmd.ExecuteScalar();
                            if (exists == 0)
                            {
                                MessageBox.Show("The specified ST_ID does not exist in the Stock table.");
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid ST_ID. Please enter a valid number.");
                            return;
                        }
                    }

                    // Proceed with inserting into Shipping_Pyment
                    string query = "INSERT INTO Shipping_Pyment (SHID, ST_ID, PymentMethod, Comments, TotalPrice,SHPCode) " +
                                   "VALUES (@SHID, @ST_ID, @PymentMethod, @Comments, @TotalPrice,@SHPCode)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Parse SHID
                        if (int.TryParse(TextBox2.Text, out int shid))
                            cmd.Parameters.AddWithValue("@SHID", shid);
                        else
                        {
                            MessageBox.Show("Invalid SHID. Please enter a valid number.");
                            return;
                        }

                        // Set other parameters
                        cmd.Parameters.AddWithValue("@ST_ID", int.Parse(textBox4.Text));
                        cmd.Parameters.AddWithValue("@PymentMethod", string.IsNullOrEmpty(cmbPaymentMode.Text) ? (object)DBNull.Value : cmbPaymentMode.Text);
                        cmd.Parameters.AddWithValue("@Comments", string.IsNullOrEmpty(txtRemarks.Text) ? (object)DBNull.Value : txtRemarks.Text);
                        cmd.Parameters.AddWithValue("@SHPCode", txtTransactionNo.Text);

                        // Parse TotalPrice
                        if (int.TryParse(txtTransactionAmount.Text, out int totalPrice))
                            cmd.Parameters.AddWithValue("@TotalPrice", totalPrice);
                        else
                        {
                            MessageBox.Show("Invalid Total Price. Please enter a valid number.");
                            return;
                        }

                        // Execute the command
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("تم الحفظ بنجاح", "سندات دفع شركات الشحن", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    cn.Open();

                    // Check if a record with the given ST_ID already exists
                    string checkQuery = "SELECT COUNT(*) FROM CompanyLedgerBook WHERE ST_ID = @stId ";
                    using (var checkCmd = new SqlCommand(checkQuery, cn))
                    {
                        checkCmd.Parameters.AddWithValue("@stId", textBox4.Text);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        


                        if (cmbPaymentMode.SelectedIndex == 0)
                        {
                            if (Convert.ToDecimal(txtTransactionAmount.Text) > 0)
                            {
                                LedgerSave(dtpTranactionDate.Value.Date, "نقدا" + "/" + txtSupplierName.Text, txtTransactionNo.Text, "سند دفع شركة شحن", Convert.ToDecimal(txtTransactionAmount.Text), 0, txtSupplierID.Text, "");
                            }
                            else
                            {
                                LedgerSave(dtpTranactionDate.Value.Date, "نقدا" + "/" + txtSupplierName.Text, txtTransactionNo.Text, "سند دفع شركة شحن", 0, Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text)), txtSupplierID.Text, "");
                            }
                        }
                        if (cmbPaymentMode.SelectedIndex == 1)
                        {
                            if (Convert.ToDecimal(txtTransactionAmount.Text) > 0)
                            {
                                LedgerSave(dtpTranactionDate.Value.Date, "شيك" + "/" + txtSupplierName.Text, txtTransactionNo.Text, "سند دفع شركة شحن", Convert.ToDecimal(txtTransactionAmount.Text), 0, txtSupplierID.Text, "");
                            }
                            else
                            {
                                LedgerSave(dtpTranactionDate.Value.Date, "شيك" + "/" + txtSupplierName.Text, txtTransactionNo.Text, "سند دفع شركة شحن", 0, Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text)), txtSupplierID.Text, "");
                            }
                        }
                        if (cmbPaymentMode.SelectedIndex == 0)
                        {
                            if (Convert.ToDecimal(txtTransactionAmount.Text) > 0)
                            {
                                if (count > 0)
                                {
                                    string creditQuery = "SELECT Debit FROM CompanyLedgerBook WHERE ST_ID = @stId";
                                    using (var creditCmd = new SqlCommand(creditQuery, cn))
                                    {
                                        creditCmd.Parameters.AddWithValue("@stId", Convert.ToInt32(textBox4.Text));
                                        decimal currentCredit = Convert.ToDecimal(creditCmd.ExecuteScalar());

                                        // Update with the new Credit amount
                                        decimal newCredit = currentCredit + Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text));

                                        CompanyLedgerUpdate1(dtpTranactionDate.Value.Date, "نقدا", newCredit, Convert.ToInt32(textBox4.Text));
                                    }
                                }
                                else
                                {
                                    CompanyLedgerSave(dtpTranactionDate.Value.Date, "نقدا", txtTransactionNo.Text, "سند دفع شركة شحن", Convert.ToDecimal(txtTransactionAmount.Text), 0, txtSupplierID.Text, Convert.ToInt32(textBox4.Text));
                                }
                            }
                            else
                            {
                                if (count > 0)
                                {
                                    string creditQuery = "SELECT Credit FROM CompanyLedgerBook WHERE ST_ID = @stId";
                                    using (var creditCmd = new SqlCommand(creditQuery, cn))
                                    {
                                        creditCmd.Parameters.AddWithValue("@stId", Convert.ToInt32(textBox4.Text));
                                        decimal currentCredit = Convert.ToDecimal(creditCmd.ExecuteScalar());

                                        // Update with the new Credit amount
                                        decimal newCredit = currentCredit + Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text));

                                        CompanyLedgerUpdate(dtpTranactionDate.Value.Date, "نقدا", newCredit, Convert.ToInt32(textBox4.Text));
                                    }
                                }
                                else
                                {
                                    CompanyLedgerSave(dtpTranactionDate.Value.Date, "نقدا", txtTransactionNo.Text, "سند دفع شركة شحن", 0, Convert.ToDecimal(txtTransactionAmount.Text), txtSupplierID.Text, Convert.ToInt32(textBox4.Text));
                                }
                            }
                        }
                        if (cmbPaymentMode.SelectedIndex == 1)
                        {
                            if (Convert.ToDecimal(txtTransactionAmount.Text) > 0)
                            {
                                CompanyLedgerSave(dtpTranactionDate.Value.Date, "شيك", txtTransactionNo.Text, "سند دفع شركة شحن", Convert.ToDecimal(txtTransactionAmount.Text), 0, txtSupplierID.Text, Convert.ToInt32(textBox4.Text));
                            }
                            else
                            {
                                if (count > 0)
                                {
                                    string creditQuery = "SELECT Credit FROM CompanyLedgerBook WHERE ST_ID = @stId";
                                    using (var creditCmd = new SqlCommand(creditQuery, cn))
                                    {
                                        creditCmd.Parameters.AddWithValue("@stId", Convert.ToInt32(textBox4.Text));
                                        decimal currentCredit = Convert.ToDecimal(creditCmd.ExecuteScalar());

                                        // Update with the new Credit amount
                                        decimal newCredit = currentCredit + Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text));

                                        CompanyLedgerUpdate(dtpTranactionDate.Value.Date, "شيك", newCredit, Convert.ToInt32(textBox4.Text));
                                    }
                                }
                                else
                                {
                                    CompanyLedgerUpdate(dtpTranactionDate.Value.Date, "شيك", Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text)), Convert.ToInt32(textBox4.Text));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            Reset();
        }
        public static void CompanyLedgerUpdate(DateTime a, string b, decimal f, int g)
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                string cb = "UPDATE CompanyLedgerBook SET Date=@d1, Name=@d2, Credit=@d3 WHERE ST_ID=@d4";
                using (var cmd = new SqlCommand(cb, con))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.Parameters.AddWithValue("@d2", b);
                    cmd.Parameters.AddWithValue("@d3", f);
                    cmd.Parameters.AddWithValue("@d4", g);
                    cmd.ExecuteReader();
                }
            }
        }
        public static void CompanyLedgerUpdate1(DateTime a, string b, decimal f, int g)
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                string cb = "UPDATE CompanyLedgerBook SET Date=@d1, Name=@d2, Debit=@d3 WHERE ST_ID=@d4";
                using (var cmd = new SqlCommand(cb, con))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.Parameters.AddWithValue("@d2", b);
                    cmd.Parameters.AddWithValue("@d3", f);
                    cmd.Parameters.AddWithValue("@d4", g);
                    cmd.ExecuteReader();
                }
            }
        }
        public void CompanyLedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g, int h)
        {

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                string cb = "INSERT INTO CompanyLedgerBook(Date, Name, LedgerNo, Label, Debit, Credit, PartyID, ST_ID) VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8)";
                using (var cmd = new SqlCommand(cb, cn))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.Parameters.AddWithValue("@d2", b);
                    cmd.Parameters.AddWithValue("@d3", c);
                    cmd.Parameters.AddWithValue("@d4", d);
                    cmd.Parameters.AddWithValue("@d5", e);
                    cmd.Parameters.AddWithValue("@d6", f);
                    cmd.Parameters.AddWithValue("@d7", g);
                    cmd.Parameters.AddWithValue("@d8", h);
                    cmd.ExecuteReader();
                }
                cn.Close();
            }

        }
        public void LedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g, string h)
        {

            using (SqlConnection cn = new SqlConnection(connectionString)) {
                cn.Open();
                string cb = "INSERT INTO LedgerBook(Date, Name, LedgerNo, Label, Debit, Credit, PartyID, Manual_Inv) VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8)";
                using (var cmd = new SqlCommand(cb, cn))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.Parameters.AddWithValue("@d2", b);
                    cmd.Parameters.AddWithValue("@d3", c);
                    cmd.Parameters.AddWithValue("@d4", d);
                    cmd.Parameters.AddWithValue("@d5", e);
                    cmd.Parameters.AddWithValue("@d6", f);
                    cmd.Parameters.AddWithValue("@d7", g);
                    cmd.Parameters.AddWithValue("@d8", h);
                    cmd.ExecuteReader();
                }
                cn.Close();
            }

        }
        public void GetCompanyBalance()
        {
            try
            {
                try
                {
                    num1 = 0;

                    {
                        cn.Open();
                        string sql = "SELECT ISNULL(SUM(Credit), 0) - ISNULL(SUM(Debit), 0) FROM CompanyLedgerBook WHERE PartyID=@d1 GROUP BY PartyID";
                        using (SqlCommand cmd = new SqlCommand(sql, cn))
                        {
                            cmd.Parameters.AddWithValue("@d1", txtSupplierID.Text);
                            using (SqlDataReader rdr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection))
                            {
                                if (rdr.Read())
                                {
                                    num1 = rdr.GetDecimal(0);
                                }
                            }
                        }
                    }
                    lblBalance.Text = num1.ToString();
                    if (decimal.Parse(lblBalance.Text) >= 0)
                    {
                        str = "دائن";
                    }
                    else if (decimal.Parse(lblBalance.Text) < 0)
                    {
                        str = "مدين";
                    }
                    lblBalance.Text = Math.Abs(decimal.Parse(lblBalance.Text)).ToString();
                    lblBalance.Text = (lblBalance.Text + " " + str).ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                cn.Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnGetData_Click(object sender, EventArgs e)
        {
            ShippingComPymentScreen shippingComPymentScreen = new ShippingComPymentScreen();
            shippingComPymentScreen.Show();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteRecord();
        }
        private void DeleteData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "DELETE FROM Shipping_Pyment WHERE SHPID = @SHPID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SHPID", int.Parse(TextBox3.Text));  // Assuming txtSupplierName holds the SHPID for deletion

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("تم حذف البيانات بنجاح", "شركات الشحن", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Reset();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void DeleteRecord()
        {
            try
            {
                int RowsAffected = 0;


                cn.Open();
                string cq = "DELETE FROM Shipping_Pyment WHERE SHPID = @SHPID";
                using (SqlCommand cmd = new SqlCommand(cq, cn))
                {
                    cmd.Parameters.AddWithValue("@SHPID", int.Parse(TextBox3.Text));
                    RowsAffected = cmd.ExecuteNonQuery();
                }
                cn.Close();

                if (RowsAffected > 0)
                {
                    CompanyLedgerDelete(txtTransactionNo.Text);
                    LedgerDelete(txtTransactionNo.Text, "سند دفع");
                    LogFunc(lblUser.Text, "deleted the payment record having transaction No. '" + txtTransactionNo.Text + "'");
                    MessageBox.Show("تم الحذف بنجاح", "سندات الدفع", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset();
                }

                else
                {
                    MessageBox.Show("لا يوجد سجلات", "عذرًا", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset();
                }
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GroupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void gbPartyInfo_Enter(object sender, EventArgs e)
        {

        }



        public void CompanyLedgerDelete(string a)
        {
            try
            {
                cn.Open();

                string cq = "DELETE FROM CompanyLedgerBook WHERE LedgerNo=@d1";
                using (var cmd = new SqlCommand(cq, cn))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.ExecuteNonQuery(); // Use ExecuteNonQuery for DELETE
                }

                string cqq = "DELETE FROM LedgerBook WHERE LedgerNo=@d2";
                using (var cmd = new SqlCommand(cqq, cn))
                {
                    cmd.Parameters.AddWithValue("@d2", a);
                    cmd.ExecuteNonQuery(); // Use ExecuteNonQuery for DELETE
                }
            }
            catch (Exception ex)
            {
                // Handle or log exception
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close(); // Ensure connection is closed
                }
            }
        }

        public void LedgerDelete(string a, string b)
        {


            cn.Open();
            string cq = "DELETE FROM LedgerBook WHERE LedgerNo=@d1 AND Label=@d2";
            using (var cmd = new SqlCommand(cq, cn))
            {
                cmd.Parameters.AddWithValue("@d1", a);
                cmd.Parameters.AddWithValue("@d2", b);
                cmd.ExecuteReader();
            }
            cn.Close();

        }
        public void LogFunc(string st1, string st2)
        {


            cn.Open();
            string cb = "INSERT INTO Logs(UserID, Date, Operation) VALUES (@d1, @d2, @d3)";
            using (var cmd = new SqlCommand(cb, cn))
            {
                cmd.Parameters.AddWithValue("@d1", st1);
                cmd.Parameters.AddWithValue("@d2", DateTime.Now);
                cmd.Parameters.AddWithValue("@d3", st2);
                cmd.ExecuteReader();
            }
            cn.Close();

        }
        private void LogError(Exception ex)
        {
            // Implement logging logic here (e.g., write to a file, event log, etc.)
            // Example:
            System.IO.File.AppendAllText("error_log.txt", $"{DateTime.Now}: {ex.Message}{Environment.NewLine}");
        }

    }
}
