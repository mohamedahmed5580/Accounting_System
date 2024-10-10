using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Pharmacy.DL;
using System.Net.Mail;
using System.Net;

namespace Accounting_System
{
    public partial class services : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public services()
        {
            InitializeComponent();
            fillServiceType();
            Auto();
            txtUpfront.KeyPress += new KeyPressEventHandler(txtChargesQuote_KeyPress);
            txtChargesQuote.KeyPress += new KeyPressEventHandler(txtChargesQuote_KeyPress_1);
            cmbServiceType.Format += new ListControlConvertEventHandler(cmbServiceType_Format);

        }

        private void GroupBox4_Enter(object sender, EventArgs e)
        {

        }
        private void Reset()
        {
            txtChargesQuote.Text = "";
            txtCID.Text = "";
            txtCustomerID.Text = "";
            txtCustomerName.Text = "";
            txtItemsDescription.Text = "";
            txtProblemDescription.Text = "";
            txtRemarks.Text = "";
            txtUpfront.Text = "";
            cmbServiceType.Text = "";
            cmbStatus.SelectedIndex = 1;
            txtContactNo.Text = "";
            dtpServiceCreationDate.Value = DateTime.Today;
            dtpEstimatedRepairDate.Value = DateTime.Today;
            btnPrint.Enabled = false;
            btnDelete.Enabled = false;
            btnUpdate.Enabled = false;
            btnSave.Enabled = true;
            Auto();
        }
        private string GenerateID()
        {
            string connectionString = DataAccessLayer.Con(); 
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string value = "0000";
                try
                {
                    // Fetch the latest ID from the database
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT TOP 1 S_ID FROM Service ORDER BY S_ID DESC", con);
                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    if (rdr.HasRows)
                    {
                        rdr.Read();
                        value = rdr["S_ID"].ToString();
                    }
                    rdr.Close();

                    // Increase the ID by 1
                    int newValue = int.Parse(value) + 1;
                    value = newValue.ToString();

                    // Because incrementing a string with an integer removes leading zeros,
                    // we need to replace them if necessary.
                    if (newValue <= 9) // Value is between 0 and 10
                    {
                        value = "000" + value;
                    }
                    else if (newValue <= 99) // Value is between 9 and 100
                    {
                        value = "00" + value;
                    }
                    else if (newValue <= 999) // Value is between 999 and 1000
                    {
                        value = "0" + value;
                    }
                }
                catch (Exception ex)
                {
                    // If an error occurs, check the connection state and close it if necessary.
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                    value = "0000";
                }

                return value;
            }
        }


        private void Auto()
        {
            try
            {
                txtID.Text = GenerateID();
                txtServiceCode.Text = "SC-" + GenerateID();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Print()
        {
            try
            {

                frmReport frmReport = new frmReport();
                var rpt = new rptServiceReceipt(); // The report you created.
                using (SqlConnection myConnection = new SqlConnection(DataAccessLayer.Con()))
                {
                    SqlCommand MyCommand = new SqlCommand();
                    SqlCommand MyCommand1 = new SqlCommand();
                    SqlDataAdapter myDA = new SqlDataAdapter();
                    SqlDataAdapter myDA1 = new SqlDataAdapter();
                    DataSet myDS = new DataSet(); // The DataSet you created.

                    MyCommand.Connection = myConnection;
                    MyCommand1.Connection = myConnection;

                    MyCommand.CommandText = @"SELECT Service.S_ID, Service.ServiceCode, Service.ServiceType, 
                                      Service.ServiceCreationDate, Service.ItemDescription, 
                                      Service.ProblemDescription, Service.ChargesQuote, Service.AdvanceDeposit, 
                                      Service.EstimatedRepairDate, Service.Remarks, Service.Status, 
                                      Customer.ID, Customer.Name, Customer.Gender, Customer.Address, 
                                      Customer.City, Customer.State, Customer.ZipCode, Customer.ContactNo, 
                                      Customer.EmailID, Customer.Remarks AS Expr2, Customer.Photo 
                                      FROM Service 
                                      INNER JOIN Customer ON Service.CustomerID = Customer.ID 
                                      WHERE Service.ServiceCode = @d1";

                    MyCommand.Parameters.AddWithValue("@d1", txtServiceCode.Text);

                    MyCommand1.CommandText = "SELECT * FROM Company";

                    MyCommand.CommandType = CommandType.Text;
                    MyCommand1.CommandType = CommandType.Text;

                    myDA.SelectCommand = MyCommand;
                    myDA1.SelectCommand = MyCommand1;

                    myDA.Fill(myDS, "Service");
                    myDA.Fill(myDS, "Customer");
                    myDA1.Fill(myDS, "Company");

                    rpt.SetDataSource(myDS);
                    rpt.SetParameterValue("p1", txtCustomerID.Text);
                    rpt.SetParameterValue("p2", DateTime.Today);

                    frmReport.crystalReportViewer1.ReportSource = rpt;
                    frmReport.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
        private void DeleteRecord()
        {
            try
            {
                int RowsAffected = 0;
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string cl = "SELECT S_ID FROM Service INNER JOIN InvoiceInfo1 ON Service.S_ID = InvoiceInfo1.ServiceID WHERE S_ID = @d1";
                    using (SqlCommand cmd = new SqlCommand(cl, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                MessageBox.Show("Unable to delete..Already in use in Billing", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }
                }

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string cq = "DELETE FROM Service WHERE S_ID = @d1";
                    using (SqlCommand cmd = new SqlCommand(cq, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                        RowsAffected = cmd.ExecuteNonQuery();
                    }
                }

                if (RowsAffected > 0)
                {
                    LedgerDelete(txtServiceCode.Text, "خدمات آخرى");
                    LedgerDelete(txtServiceCode.Text, "سداد العملاء");
                    string st = "deleted the record having service code '" + txtServiceCode.Text + "'";
                    LogFunc(lblUser.Text, st);
                    MessageBox.Show("Successfully deleted", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset();
                    fillServiceType();
                }
                else
                {
                    MessageBox.Show("No Record found", "Sorry", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static void LedgerDelete(string a, string b)
        {
            using (SqlConnection con = DataAccessLayer.cn)
            {
                con.Open();
                string cq = "DELETE FROM LedgerBook WHERE LedgerNo=@d1 AND Label=@d2";
                using (var cmd = new SqlCommand(cq, con))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.Parameters.AddWithValue("@d2", b);
                    cmd.ExecuteReader();
                }
            }
        }
        public static void LogFunc(string st1, string st2)
        {
            using (SqlConnection con = DataAccessLayer.cn)
            {
                con.Open();
                string cb = "INSERT INTO Logs(UserID, Date, Operation) VALUES (@d1, @d2, @d3)";
                using (var cmd = new SqlCommand(cb, con))
                {
                    cmd.Parameters.AddWithValue("@d1", st1);
                    cmd.Parameters.AddWithValue("@d2", DateTime.Now);
                    cmd.Parameters.AddWithValue("@d3", st2);
                    cmd.ExecuteReader();
                }
            }
        }
        private void fillServiceType()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlDataAdapter adp = new SqlDataAdapter("SELECT DISTINCT RTRIM(ServiceType) FROM Service", con))
                    {
                        DataSet ds = new DataSet();
                        adp.Fill(ds);
                        DataTable dtable = ds.Tables[0];
                        cmbServiceType.Items.Clear();
                        foreach (DataRow drow in dtable.Rows)
                        {
                            cmbServiceType.Items.Add(drow[0].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Do you really want to delete this record?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    DeleteRecord();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtItemsDescription.Text))
            {
                MessageBox.Show("Please enter items description", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtItemsDescription.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtChargesQuote.Text))
            {
                MessageBox.Show("Please enter charges quote", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtChargesQuote.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtUpfront.Text))
            {
                MessageBox.Show("Please enter upfront", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUpfront.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("Please retrieve customer details", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string ctn = "SELECT * FROM Company";
                    using (SqlCommand cmd = new SqlCommand(ctn, con))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (!rdr.Read())
                            {
                                MessageBox.Show("Add company profile first in master entry", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }
                    }
                }

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string cb = "INSERT INTO Service(S_ID, ServiceCode, CustomerID, ServiceType, ServiceCreationDate, ItemDescription, ProblemDescription, ChargesQuote, AdvanceDeposit, EstimatedRepairDate, Remarks, Status) " +
                                "VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9, @d10, @d11, @d12)";
                    using (SqlCommand cmd = new SqlCommand(cb, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                        cmd.Parameters.AddWithValue("@d2", txtServiceCode.Text);
                        cmd.Parameters.AddWithValue("@d3", Convert.ToInt32(txtCID.Text));
                        cmd.Parameters.AddWithValue("@d4", cmbServiceType.Text);
                        cmd.Parameters.AddWithValue("@d5", dtpServiceCreationDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d6", txtItemsDescription.Text);
                        cmd.Parameters.AddWithValue("@d7", txtProblemDescription.Text);
                        cmd.Parameters.AddWithValue("@d8", Convert.ToDecimal(txtChargesQuote.Text));
                        cmd.Parameters.AddWithValue("@d9", Convert.ToDecimal(txtUpfront.Text));
                        cmd.Parameters.AddWithValue("@d10", dtpEstimatedRepairDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d11", txtRemarks.Text);
                        cmd.Parameters.AddWithValue("@d12", cmbStatus.Text);
                        cmd.ExecuteReader();
                    }
                }

                LedgerSave(dtpServiceCreationDate.Value.Date, txtCustomerName.Text, txtServiceCode.Text, "خدمات آخرى", 0, Convert.ToDecimal(txtUpfront.Text), txtCustomerID.Text, "");
                LedgerSave(dtpServiceCreationDate.Value.Date, "نقدا", txtServiceCode.Text, "سداد العملاء", Convert.ToDecimal(txtUpfront.Text), 0, txtCustomerID.Text, "");

                string st = "added the new service having service code '" + txtServiceCode.Text + "'";
                LogFunc(lblUser.Text, st);

                // Uncomment and implement SMS functionality if needed
                /*
                if (CheckForInternetConnection())
                {
                    using (SqlConnection con = new SqlConnection(cs))
                    {
                        con.Open();
                        string ctn1 = "SELECT RTRIM(APIURL) FROM SMSSetting WHERE IsDefault='Yes' AND IsEnabled='Yes'";
                        using (SqlCommand cmd = new SqlCommand(ctn1, con))
                        {
                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                if (rdr.Read())
                                {
                                    string st2 = rdr.GetValue(0).ToString();
                                    string st3 = "Hello, " + txtCustomerName.Text + " service has been created successfully having service code " + txtServiceCode.Text;
                                    SMSFunc(txtContactNo.Text, st3, st2);
                                    SMS(st3);
                                }
                            }
                        }
                    }
                }
                */

                btnSave.Enabled = false;
                fillServiceType();
                MessageBox.Show("Successfully created", "Service", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Print();
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        public static void LedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g, string h)
        {
            using (SqlConnection con = DataAccessLayer.cn)
            {
                con.Open();
                string cb = "INSERT INTO LedgerBook(Date, Name, LedgerNo, Label, Debit, Credit, PartyID, Manual_Inv) VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8)";
                using (var cmd = new SqlCommand(cb, con))
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
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtItemsDescription.Text))
            {
                MessageBox.Show("الرجاء إدخال وصف العناصر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtItemsDescription.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtChargesQuote.Text))
            {
                MessageBox.Show("الرجاء إدخال اقتباس الرسوم", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtChargesQuote.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtUpfront.Text))
            {
                MessageBox.Show("الرجاء إدخال مقدما", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUpfront.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("يرجى استرجاع تفاصيل العميل", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string cb = "UPDATE Service SET ServiceCode=@d2, CustomerID=@d3, ServiceType=@d4, ServiceCreationDate=@d5, ItemDescription=@d6, ProblemDescription=@d7, ChargesQuote=@d8, AdvanceDeposit=@d9, EstimatedRepairDate=@d10, Remarks=@d11, Status=@d12 WHERE S_ID=@d1";
                    using (SqlCommand cmd = new SqlCommand(cb, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                        cmd.Parameters.AddWithValue("@d2", txtServiceCode.Text);
                        cmd.Parameters.AddWithValue("@d3", Convert.ToInt32(txtCID.Text));
                        cmd.Parameters.AddWithValue("@d4", cmbServiceType.Text);
                        cmd.Parameters.AddWithValue("@d5", dtpServiceCreationDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d6", txtItemsDescription.Text);
                        cmd.Parameters.AddWithValue("@d7", txtProblemDescription.Text);
                        cmd.Parameters.AddWithValue("@d8", Convert.ToDecimal(txtChargesQuote.Text));
                        cmd.Parameters.AddWithValue("@d9", Convert.ToDecimal(txtUpfront.Text));
                        cmd.Parameters.AddWithValue("@d10", dtpEstimatedRepairDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d11", txtRemarks.Text);
                        cmd.Parameters.AddWithValue("@d12", cmbStatus.Text);
                        cmd.ExecuteReader();
                    }
                }

                LedgerUpdate(dtpServiceCreationDate.Value.Date, txtCustomerName.Text, 0, Convert.ToDecimal(txtUpfront.Text), txtServiceCode.Text, txtCustomerID.Text, "Service Upfront");
                LedgerUpdate(dtpServiceCreationDate.Value.Date, "Cash Account", Convert.ToDecimal(txtUpfront.Text), 0, txtServiceCode.Text, txtCustomerID.Text, "Payment");

                string st = "updated the service having service code '" + txtServiceCode.Text + "'";
                LogFunc(lblUser.Text, st);

                btnUpdate.Enabled = false;
                fillServiceType();
                MessageBox.Show("تم التعديل بنجاح", "خدمات", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        public static void LedgerUpdate(DateTime a, string b, decimal e, decimal f, string g, string h, string i)
        {
            using (SqlConnection con = DataAccessLayer.cn)
            {
                con.Open();
                string cb = "UPDATE LedgerBook SET Date=@d1, Name=@d2, Debit=@d3, Credit=@d4, PartyID=@d5 WHERE LedgerNo=@d6 AND Label=@d7";
                using (var cmd = new SqlCommand(cb, con))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.Parameters.AddWithValue("@d2", b);
                    cmd.Parameters.AddWithValue("@d3", e);
                    cmd.Parameters.AddWithValue("@d4", f);
                    cmd.Parameters.AddWithValue("@d5", g);
                    cmd.Parameters.AddWithValue("@d6", h);
                    cmd.Parameters.AddWithValue("@d7", i);
                    cmd.ExecuteReader();
                }
            }
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            servicesRecord frmServicesRecord = new servicesRecord();
            frmServicesRecord.lblSet.Text = "Services";
            frmServicesRecord.Reset();
            frmServicesRecord.ShowDialog();

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Print();
        }
        private void txtChargesQuote_KeyPress(object sender, KeyPressEventArgs e)
        {
            char keyChar = e.KeyChar;

            if (char.IsControl(keyChar))
            {
                // Allow all control characters.
            }
            else if (char.IsDigit(keyChar) || keyChar == '.')
            {
                string text = this.txtUpfront.Text;
                int selectionStart = this.txtUpfront.SelectionStart;
                int selectionLength = this.txtUpfront.SelectionLength;

                text = text.Substring(0, selectionStart) + keyChar + text.Substring(selectionStart + selectionLength);

                if (int.TryParse(text, out _) && text.Length > 16)
                {
                    // Reject an integer that is longer than 16 digits.
                    e.Handled = true;
                }
                else if (double.TryParse(text, out _) && text.IndexOf('.') < text.Length - 3)
                {
                    // Reject a real number with too many decimal places.
                    e.Handled = false;
                }
            }
            else
            {
                // Reject all other characters.
                e.Handled = true;
            }
            
        }
        private void txtChargesQuote_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            char keyChar = e.KeyChar;

            if (char.IsControl(keyChar))
            {
                // Allow all control characters.
            }
            else if (char.IsDigit(keyChar) || keyChar == '.')
            {
                string text = this.txtChargesQuote.Text;
                int selectionStart = this.txtChargesQuote.SelectionStart;
                int selectionLength = this.txtChargesQuote.SelectionLength;

                text = text.Substring(0, selectionStart) + keyChar + text.Substring(selectionStart + selectionLength);

                if (int.TryParse(text, out _) && text.Length > 16)
                {
                    // Reject an integer that is longer than 16 digits.
                    e.Handled = true;
                }
                else if (double.TryParse(text, out _) && text.IndexOf('.') < text.Length - 3)
                {
                    // Reject a real number with too many decimal places.
                    e.Handled = false;
                }
            }
            else
            {
                // Reject all other characters.
                e.Handled = true;
            }

        }
        private void cmbServiceType_Format(object sender,ListControlConvertEventArgs e)
        {
            if (e.DesiredType == typeof(string))
            {
                e.Value = e.Value.ToString().Trim();
            }

        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            this.Close();
            customerRecord2 customerRecord2 = new customerRecord2();
            customerRecord2.Show();
        }
    }
}
