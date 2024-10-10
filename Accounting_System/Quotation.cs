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

namespace Accounting_System
{
    public partial class Quotation : Form
    {
        private static Quotation _instance;
        public static Quotation Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new Quotation();
                }
                return _instance;
            }
        }
        public Quotation()
        {
            InitializeComponent();
            txtQty.TextChanged += new EventHandler(txtQty_TextChanged);
            txtQty.KeyPress += new KeyPressEventHandler(txtQty_KeyPress);
            DataGridView1.MouseClick += new MouseEventHandler(DataGridView1_MouseClick);
            DataGridView1.RowPostPaint += new DataGridViewRowPostPaintEventHandler(DataGridView1_RowPostPaint);
            txtSellingPrice.KeyPress += new KeyPressEventHandler(txtSellingPrice_KeyPress);
            txtDiscountPer.KeyPress += new KeyPressEventHandler(txtDiscountPer_KeyPress);
            txtVAT.KeyPress += new KeyPressEventHandler(txtVAT_KeyPress);
            txtSellingPrice.TextChanged += new EventHandler(txtSellingPrice_TextChanged);
            txtDiscountPer.TextChanged += new EventHandler(txtDiscountPer_TextChanged);
            txtVAT.TextChanged += new EventHandler(txtVAT_TextChanged);

        }

        private void Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
        public void Reset()
        {
            txtCID.Text = "";
            txtRemarks.Text = "";
            txtCustomerName.Text = "";
            txtAmount.Text = "";
            txtSellingPrice.Text = "";
            txtCustomerID.Text = "";
            txtDiscountAmount.Text = "";
            txtDiscountPer.Text = "";
            txtQuotationNo.Text = "";
            txtProductCode.Text = "";
            txtProductName.Text = "";
            txtQty.Text = "";
            txtSellingPrice.Text = "";
            txtTotalAmount.Text = "";
            txtTotalQty.Text = "";
            txtVAT.Text = "";
            txtVATAmount.Text = "";
            txtGrandTotal.Text = "";
            dtpQuotationDate.Text = DateTime.Today.ToString("d");
            btnDelete.Enabled = false;
            //btnUpdate.Enabled = false;
            btnSave.Enabled = true;
            btnRemove.Enabled = false;
            btnAdd.Enabled = true;
            btnPrint.Enabled = false;
            txtContactNo.Text = "";
            txtCustomerType.Text = "";
            Auto(); // Assuming you have a method called Auto()
            lblSet.Text = "Allowed";
            DataGridView1.Rows.Clear();
            Clear(); // Assuming you have a method called Clear()
        }
        private string GenerateID()
        {
            string value = "0000";
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    // Fetch the latest ID from the database
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 Q_ID FROM Quotation ORDER BY Q_ID DESC", con))
                    {
                        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            value = rdr["Q_ID"].ToString();
                        }
                        rdr.Close();
                    }

                    // Increase the ID by 1
                    int numericValue = int.Parse(value) + 1;

                    // Format the value with leading zeros
                    value = numericValue.ToString("D4");
                }
                catch (Exception ex)
                {
                    // If an error occurs, close the connection if necessary
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                    value = "0000";
                }
            }
            return value;
        }
        private void Auto()
        {
            try
            {
                txtID.Text = GenerateID();
                txtQuotationNo.Text = "Q-" + GenerateID();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSelectionInv_Click(object sender, EventArgs e)
        {

            productRecord frmProductRecord = new productRecord();
            frmProductRecord.lblSet.Text = "Quotation";
            frmProductRecord.Reset();
            frmProductRecord.ShowDialog();
        }
        private void Compute()
        {
            double qty, sellingPrice, amount, discountPer, discountAmount, vat, vatAmount, totalAmount;

            // Initialize values to zero in case parsing fails
            qty = sellingPrice = amount = discountPer = discountAmount = vat = vatAmount = totalAmount = 0;

            // Try parsing values from text boxes
            bool isQtyValid = double.TryParse(txtQty.Text, out qty);
            bool isSellingPriceValid = double.TryParse(txtSellingPrice.Text, out sellingPrice);
            bool isDiscountPerValid = double.TryParse(txtDiscountPer.Text, out discountPer);
            bool isVATValid = double.TryParse(txtVAT.Text, out vat);

            // Check if all required fields are valid
            if (isQtyValid && isSellingPriceValid && isDiscountPerValid && isVATValid)
            {
                // Compute amount
                amount = Math.Round(qty * sellingPrice, 2);
                txtAmount.Text = amount.ToString("F2");

                // Compute discount amount
                discountAmount = Math.Round((amount * discountPer) / 100, 2);
                txtDiscountAmount.Text = discountAmount.ToString("F2");

                // Compute VAT amount
                double netAmount = amount - discountAmount;
                vatAmount = Math.Round((vat * netAmount) / 100, 2);
                txtVATAmount.Text = vatAmount.ToString("F2");

                // Compute total amount
                totalAmount = Math.Round(amount + vatAmount - discountAmount, 2);
                txtTotalAmount.Text = totalAmount.ToString("F2");
            }

        }

        private void txtQty_TextChanged(object sender, EventArgs e)
        {
            Compute();
        }
        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < (char)48 || e.KeyChar > (char)57) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }
        public double GrandTotal()
        {
            double sum = 0;
            try
            {
                foreach (DataGridViewRow r in this.DataGridView1.Rows)
                {
                    sum += Convert.ToDouble(r.Cells[9].Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return sum;
        }
        private void Print()
        {
            try
            {


                DataSet myDS = new DataSet(); // The DataSet you created.
                frmReport frmReport = new frmReport();
                using (SqlConnection myConnection = new SqlConnection(DataAccessLayer.Con()))
                {
                    SqlCommand myCommand = new SqlCommand
                    {
                        Connection = myConnection,
                        CommandText = "SELECT Customer.ID, Customer.Name, Customer.Gender, Customer.Address, Customer.City, Customer.State, Customer.ZipCode, Customer.ContactNo, Customer.EmailID, Customer.Photo, Quotation.Q_ID, Quotation.QuotationNo, Quotation.Date, Quotation.GrandTotal, Quotation_Join.QJ_ID, Quotation_Join.QuotationID, Quotation_Join.ProductID, Quotation_Join.Cost, Quotation_Join.Qty, Quotation_Join.Amount, Quotation_Join.DiscountPer, Quotation_Join.Discount, Quotation_Join.VATPer, Quotation_Join.VAT, Quotation_Join.TotalAmount, Product.PID, Product.ProductCode, Product.ProductName FROM Customer INNER JOIN Quotation ON Customer.ID = Quotation.CustomerID INNER JOIN Quotation_Join ON Quotation.Q_ID = Quotation_Join.QuotationID INNER JOIN Product ON Quotation_Join.ProductID = Product.PID WHERE QuotationNo=@d1",
                        CommandType = CommandType.Text
                    };
                    myCommand.Parameters.AddWithValue("@d1", txtQuotationNo.Text);

                    SqlCommand myCommand1 = new SqlCommand
                    {
                        Connection = myConnection,
                        CommandText = "SELECT * FROM Company",
                        CommandType = CommandType.Text
                    };

                    SqlDataAdapter myDA = new SqlDataAdapter(myCommand);
                    SqlDataAdapter myDA1 = new SqlDataAdapter(myCommand1);

                    myDA.Fill(myDS, "Quotation");
                    myDA.Fill(myDS, "Quotation_Join");
                    myDA.Fill(myDS, "Customer");
                    myDA.Fill(myDS, "Product");
                    myDA1.Fill(myDS, "Company");

                    if (txtCustomerType.Text != "Non Regular")
                    {
                        rptQuotation rpt = new rptQuotation(); // The report you created.
                        rpt.SetDataSource(myDS);
                        rpt.SetParameterValue("p1", txtCustomerID.Text);
                        rpt.SetParameterValue("p2", DateTime.Today);

                        frmReport.crystalReportViewer1.ReportSource = rpt;
                        frmReport.ShowDialog();
                    }
                    else
                    {
                        rptQuotation1 rpt = new rptQuotation1(); // The report you created.
                        rpt.SetDataSource(myDS);
                        rpt.SetParameterValue("p1", txtCustomerID.Text);
                        rpt.SetParameterValue("p2", DateTime.Today);

                        frmReport.crystalReportViewer1.ReportSource = rpt;
                        frmReport.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Clear()
        {
            txtProductCode.Text = "";
            txtProductName.Text = "";
            txtSellingPrice.Text = "";
            txtQty.Text = "";
            txtAmount.Text = "";
            txtDiscountPer.Text = "";
            txtDiscountAmount.Text = "";
            txtVAT.Text = "";
            txtVATAmount.Text = "";
            txtTotalAmount.Text = "";

            btnAdd.Enabled = true;
            btnRemove.Enabled = false;
            btnListUpdate.Enabled = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {


                // Check if Customer Name is empty
                if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
                {
                    MessageBox.Show("الرجاء إدراج بيانات العميل", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check if DataGridView1 has rows
                if (DataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("الرجاء إدراج معلومات الأصناف في شبكة البيانات", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // Check if Company data exists
                    string checkCompanyQuery = "SELECT * FROM Company";
                    using (SqlCommand cmd = new SqlCommand(checkCompanyQuery, con))
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (!rdr.Read())
                        {
                            MessageBox.Show("الرجاء إدراج بيانات الشركة أولا", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }

                    if (!txtCustomerName.ReadOnly)
                    {
                        Auto1();
                        string insertCustomerQuery = "INSERT INTO Customer(ID, CustomerID, [Name], Gender, Address, City, ContactNo, EmailID, Remarks, State, ZipCode, Photo, CustomerType) " +
                                                     "VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9, @d10, @d11, @d12, 'Non Regular')";
                        using (SqlCommand cmd = new SqlCommand(insertCustomerQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@d1", int.Parse(txtCID.Text));
                            cmd.Parameters.AddWithValue("@d2", txtCustomerID.Text);
                            cmd.Parameters.AddWithValue("@d3", txtCustomerName.Text);
                            cmd.Parameters.AddWithValue("@d4", "");
                            cmd.Parameters.AddWithValue("@d5", "");
                            cmd.Parameters.AddWithValue("@d6", "");
                            cmd.Parameters.AddWithValue("@d7", txtContactNo.Text);
                            cmd.Parameters.AddWithValue("@d8", "");
                            cmd.Parameters.AddWithValue("@d9", "");
                            cmd.Parameters.AddWithValue("@d10", "");
                            cmd.Parameters.AddWithValue("@d11", "");

                            // Handling image data
                            using (MemoryStream ms = new MemoryStream())
                            {

                                byte[] data = ms.ToArray();
                                cmd.Parameters.Add(new SqlParameter("@d12", SqlDbType.Image) { Value = data });
                                cmd.ExecuteNonQuery();
                            }
                        }

                        txtCustomerType.Text = "Non Regular";
                    }

                    // Insert Quotation
                    string insertQuotationQuery = "INSERT INTO Quotation(Q_ID, QuotationNo, Date, CustomerID, GrandTotal, Remarks) " +
                                                   "VALUES (@d1, @d2, @d3, @d4, @d5, @d6)";
                    using (SqlCommand cmd = new SqlCommand(insertQuotationQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", int.Parse(txtID.Text));
                        cmd.Parameters.AddWithValue("@d2", txtQuotationNo.Text);
                        cmd.Parameters.AddWithValue("@d3", dtpQuotationDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d4", int.Parse(txtCID.Text));
                        cmd.Parameters.AddWithValue("@d5", double.Parse(txtGrandTotal.Text));
                        cmd.Parameters.AddWithValue("@d6", txtRemarks.Text);
                        cmd.ExecuteNonQuery();
                    }

                    // Insert Quotation_Join
                    string insertQuotationJoinQuery = "INSERT INTO Quotation_Join(QuotationID, Cost, Qty, Amount, DiscountPer, Discount, VATPer, VAT, TotalAmount, ProductID) " +
                                                       "VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9, @d10)";
                    using (SqlCommand cmd = new SqlCommand(insertQuotationJoinQuery, con))
                    {
                        foreach (DataGridViewRow row in DataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                cmd.Parameters.AddWithValue("@d1", int.Parse(txtID.Text));
                                cmd.Parameters.AddWithValue("@d2", double.Parse(row.Cells[2].Value.ToString()));
                                cmd.Parameters.AddWithValue("@d3", double.Parse(row.Cells[3].Value.ToString()));
                                cmd.Parameters.AddWithValue("@d4", double.Parse(row.Cells[4].Value.ToString()));
                                cmd.Parameters.AddWithValue("@d5", double.Parse(row.Cells[5].Value.ToString()));
                                cmd.Parameters.AddWithValue("@d6", double.Parse(row.Cells[6].Value.ToString()));
                                cmd.Parameters.AddWithValue("@d7", double.Parse(row.Cells[7].Value.ToString()));
                                cmd.Parameters.AddWithValue("@d8", double.Parse(row.Cells[8].Value.ToString()));
                                cmd.Parameters.AddWithValue("@d9", double.Parse(row.Cells[9].Value.ToString()));
                                cmd.Parameters.AddWithValue("@d10", row.Cells[10].Value.ToString());
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                        }
                    }

                    string logMessage = $"added the new quotation having quotation no. '{txtQuotationNo.Text}'";
                    LogFunc(lblUser.Text, logMessage);
                    btnSave.Enabled = false;

                    // Optional: Uncomment if you want to send an SMS
                    /*
                    if (CheckForInternetConnection())
                    {
                        string smsQuery = "SELECT RTRIM(APIURL) FROM SMSSetting WHERE IsDefault='Yes' AND IsEnabled='Yes'";
                        using (SqlCommand cmd = new SqlCommand(smsQuery, con))
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                string apiUrl = rdr.GetString(0);
                                string message = $"Hello, {txtCustomerName.Text} you have successfully applied for quotation having quotation no. {txtQuotationNo.Text}";
                                SMSFunc(txtContactNo.Text, message, apiUrl);
                                SMS(message);
                            }
                        }
                    }
                    */

                    RefreshRecords();
                    MessageBox.Show("تم الحفظ بنجاح", "السجلات", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Print();
                }
            

            

        }
        public static void RefreshRecords()
        {
/*            var obj = (StockBalance)Application.OpenForms["StockBalance"];
            obj.Getdata();
            obj.dataGridView1.Refresh();
            obj.dataGridView1.Update();*/
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
                try
                {
                    if (string.IsNullOrWhiteSpace(txtProductCode.Text))
                    {
                        MessageBox.Show("الرجاء إدراج كود الصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtProductCode.Focus();
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtSellingPrice.Text))
                    {
                        MessageBox.Show("الرجاء كتابة السعر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtSellingPrice.Focus();
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtDiscountPer.Text))
                    {
                        MessageBox.Show("الرجاء تحديد الخصم %", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtDiscountPer.Focus();
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtVAT.Text))
                    {
                        MessageBox.Show("الرجاء تحديد الضريبة %", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtVAT.Focus();
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtQty.Text))
                    {
                        MessageBox.Show("الرجاء كتابة الكمية", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtQty.Focus();
                        return;
                    }

                    if (double.TryParse(txtQty.Text, out double qty) && qty <= 0)
                    {
                        MessageBox.Show("الكمية يجب أن تكون أكبر من صفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtQty.Focus();
                        return;
                    }

                    if (DataGridView1.Rows.Count == 0)
                    {
                        DataGridView1.Rows.Add(txtProductCode.Text, txtProductName.Text, txtSellingPrice.Text, txtQty.Text, txtAmount.Text, txtDiscountPer.Text, txtDiscountAmount.Text, txtVAT.Text, txtVATAmount.Text, txtTotalAmount.Text, txtProductID.Text);
                        double grandTotal = GrandTotal();
                        grandTotal = Math.Round(grandTotal, 2);
                        txtGrandTotal.Text = grandTotal.ToString();
                        Clear();
                        return;
                    }

                    foreach (DataGridViewRow row in DataGridView1.Rows)
                    {
                        if (row.Cells[0].Value.ToString() == txtProductCode.Text)
                        {
                            row.Cells[0].Value = txtProductCode.Text;
                            row.Cells[1].Value = txtProductName.Text;
                            row.Cells[2].Value = txtSellingPrice.Text;
                            row.Cells[3].Value = (double.Parse(row.Cells[3].Value.ToString()) + double.Parse(txtQty.Text)).ToString();
                            row.Cells[4].Value = (double.Parse(row.Cells[4].Value.ToString()) + double.Parse(txtAmount.Text)).ToString();
                            row.Cells[5].Value = txtDiscountPer.Text;
                            row.Cells[6].Value = (double.Parse(row.Cells[6].Value.ToString()) + double.Parse(txtDiscountAmount.Text)).ToString();
                            row.Cells[7].Value = txtVAT.Text;
                            row.Cells[8].Value = (double.Parse(row.Cells[8].Value.ToString()) + double.Parse(txtVATAmount.Text)).ToString();
                            row.Cells[9].Value = (double.Parse(row.Cells[9].Value.ToString()) + double.Parse(txtTotalAmount.Text)).ToString();
                            row.Cells[10].Value = txtProductID.Text;

                            double grandTotal = GrandTotal();
                            grandTotal = Math.Round(grandTotal, 3);
                            txtGrandTotal.Text = grandTotal.ToString();
                            Clear();
                            return;
                        }
                    }

                    DataGridView1.Rows.Add(txtProductCode.Text, txtProductName.Text, txtSellingPrice.Text, txtQty.Text, txtAmount.Text, txtDiscountPer.Text, txtDiscountAmount.Text, txtVAT.Text, txtVATAmount.Text, txtTotalAmount.Text, txtProductID.Text);
                    double grandTotalNew = GrandTotal();
                    grandTotalNew = Math.Round(grandTotalNew, 3);
                    txtGrandTotal.Text = grandTotalNew.ToString();
                    Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            

        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                // Iterate through selected rows and remove them
                foreach (DataGridViewRow row in DataGridView1.SelectedRows)
                {
                    DataGridView1.Rows.Remove(row);
                }

                // Calculate and display the grand total
                double grandTotal = GrandTotal();
                grandTotal = Math.Round(grandTotal, 2);
                txtGrandTotal.Text = grandTotal.ToString();

                // Perform computations and clear fields
                Compute();
                Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void DataGridView1_MouseClick(object sender, MouseEventArgs e)
        {

            
                if (DataGridView1.Rows.Count > 0)
                {
                    if (lblSet.Text == "Not Allowed")
                    {
                        btnRemove.Enabled = false;
                        btnListUpdate.Enabled = false;
                    }
                    else
                    {
                        btnRemove.Enabled = true;
                        btnListUpdate.Enabled = true;
                    }

                    btnAdd.Enabled = false;

                    if (DataGridView1.SelectedRows.Count > 0)
                    {
                        DataGridViewRow row = DataGridView1.SelectedRows[0];
                        txtProductCode.Text = row.Cells[0].Value.ToString();
                        txtProductName.Text = row.Cells[1].Value.ToString();
                        txtSellingPrice.Text = row.Cells[2].Value.ToString();
                        txtQty.Text = row.Cells[3].Value.ToString();
                        txtAmount.Text = row.Cells[4].Value.ToString();
                        txtDiscountPer.Text = row.Cells[5].Value.ToString();
                        txtDiscountAmount.Text = row.Cells[6].Value.ToString();
                        txtVAT.Text = row.Cells[7].Value.ToString();
                        txtVATAmount.Text = row.Cells[8].Value.ToString();
                        txtTotalAmount.Text = row.Cells[9].Value.ToString();
                        txtProductID.Text = row.Cells[10].Value.ToString();
                    }
                }
        }
        private void DataGridView1_RowPostPaint(object sender,DataGridViewRowPostPaintEventArgs e)
        {



        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

                try
                {
                    DialogResult result = MessageBox.Show("هل أنت متأكد بالفعل أنك تريد حذف هذا السجل?", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        DeleteRecord();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            

        }
        private void DeleteRecord()
        {
            try
            {
                int rowsAffected = 0;
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "DELETE FROM Quotation WHERE Q_ID = @d1";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                        rowsAffected = cmd.ExecuteNonQuery();
                    }

                    if (rowsAffected > 0)
                    {
                        string logMessage = $"deleted the invoice no. '{txtQuotationNo.Text}'";
                        LogFunc(lblUser.Text, logMessage);
                        MessageBox.Show("تم الحذف بنجاح", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Reset();
                    }
                    else
                    {
                        MessageBox.Show("لا يوجد سجلات", "عذرًا", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Reset();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static void LogFunc(string st1, string st2)
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
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
        private string GenerateID1()
        {
            string value = "0000";
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    // Fetch the latest ID from the database
                    con.Open();
                    string query = "SELECT TOP 1 ID FROM Customer ORDER BY ID DESC";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                value = rdr["ID"].ToString();
                            }
                        }
                    }

                    // Increase the ID by 1
                    int numericValue = Convert.ToInt32(value);
                    numericValue += 1;

                    // Format the new ID with leading zeros
                    value = numericValue.ToString("D4");
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    value = "0000";
                }
            }
            return value;
        }
        private void Auto1()
        {
            try
            {
                txtCID.Text = GenerateID1();
                txtCustomerID.Text = "C-" + GenerateID1();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

                if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
                {
                    MessageBox.Show("الرجاء إدراج معلومات العميل", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (DataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("الرجاء إدراج معلومات الأصناف في شبكة البيانات", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();
                        string cb = "UPDATE Quotation SET QuotationNo = @d2, Date = @d3, CustomerID = @d4, GrandTotal = @d5, Remarks = @d6 WHERE Q_ID = @d1";

                        using (SqlCommand cmd = new SqlCommand(cb, con))
                        {
                            cmd.Parameters.AddWithValue("@d1", int.Parse(txtID.Text));
                            cmd.Parameters.AddWithValue("@d2", txtQuotationNo.Text);
                            cmd.Parameters.AddWithValue("@d3", dtpQuotationDate.Value.Date);
                            cmd.Parameters.AddWithValue("@d4", int.Parse(txtCID.Text));
                            cmd.Parameters.AddWithValue("@d5", decimal.Parse(txtGrandTotal.Text));
                            cmd.Parameters.AddWithValue("@d6", txtRemarks.Text);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    string st = "Updated the quotation having invoice no. '" + txtQuotationNo.Text + "'";
                    LogFunc(lblUser.Text, st);
                    //btnUpdate.Enabled = false;
                    MessageBox.Show("تم التعديل بنجاح", "السجلات", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            

        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            this.Hide();
            Reset();
            customerRecord2 frmCustomerRecord2 = new customerRecord2();
            frmCustomerRecord2.lblSet.Text = "Quotation";
            frmCustomerRecord2.lblUser.Text = lblUser.Text;
            frmCustomerRecord2.Show();

        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            this.Hide();
            QuotationRecord frmQuotationRecord = new QuotationRecord();
            frmQuotationRecord.Reset();
            frmQuotationRecord.ShowDialog();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Print();
        }

        private void btnListReset_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnListUpdate_Click(object sender, EventArgs e)
        {

                try
                {
                    if (string.IsNullOrWhiteSpace(txtProductCode.Text))
                    {
                        MessageBox.Show("الرجاء إدراج كود الصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtProductCode.Focus();
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtSellingPrice.Text))
                    {
                        MessageBox.Show("الرجاء كتابة سعر الصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtSellingPrice.Focus();
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtDiscountPer.Text))
                    {
                        MessageBox.Show("الرجاء تحديد الخصم %", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtDiscountPer.Focus();
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtVAT.Text))
                    {
                        MessageBox.Show("الرجاء تحديد الضريبة %", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtVAT.Focus();
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtQty.Text))
                    {
                        MessageBox.Show("الرجاء إدخال الكمية", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtQty.Focus();
                        return;
                    }

                    if (int.TryParse(txtQty.Text, out int quantity) && quantity <= 0)
                    {
                        MessageBox.Show("الكمية يجب أن تكون أكبر من صفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtQty.Focus();
                        return;
                    }

                    // Remove selected rows from DataGridView
                    foreach (DataGridViewRow row in DataGridView1.SelectedRows)
                    {
                        DataGridView1.Rows.Remove(row);
                    }

                    // Add new row to DataGridView
                    DataGridView1.Rows.Add(txtProductCode.Text, txtProductName.Text, txtSellingPrice.Text, txtQty.Text, txtAmount.Text, txtDiscountPer.Text, txtDiscountAmount.Text, txtVAT.Text, txtVATAmount.Text, txtTotalAmount.Text, txtProductID.Text);

                    // Calculate and display grand total
                    double grandTotal = GrandTotal();
                    grandTotal = Math.Round(grandTotal, 2);
                    txtGrandTotal.Text = grandTotal.ToString();

                    // Clear input fields
                    Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            

        }
        private void txtSellingPrice_KeyPress(object sender, KeyPressEventArgs e)
        {

/*                char keyChar = e.KeyChar;

                if (char.IsControl(keyChar))
                {
                    // Allow all control characters.
                }
                else if (char.IsDigit(keyChar) || keyChar == '.')
                {
                    string text = this.txtSellingPrice.Text;
                    int selectionStart = this.txtSellingPrice.SelectionStart;
                    int selectionLength = this.txtSellingPrice.SelectionLength;

                    text = text.Substring(0, selectionStart) + keyChar + text.Substring(selectionStart + selectionLength);

                    if (int.TryParse(text, out int _) && text.Length > 16)
                    {
                        // Reject an integer that is longer than 16 digits.
                        e.Handled = true;
                    }
                    else if (double.TryParse(text, out double _) && text.IndexOf('.') < text.Length - 3)
                    {
                        // Reject a real number with too many decimal places.
                        e.Handled = true;
                    }
                }
                else
                {
                    // Reject all other characters.
                    e.Handled = true;
                }*/
            

        }
        private void txtDiscountPer_KeyPress(object sender, KeyPressEventArgs e)
        {

                char keyChar = e.KeyChar;

                if (char.IsControl(keyChar))
                {
                    // Allow all control characters.
                }
                else if (char.IsDigit(keyChar) || keyChar == '.')
                {
                    string text = this.txtDiscountPer.Text;
                    int selectionStart = this.txtDiscountPer.SelectionStart;
                    int selectionLength = this.txtDiscountPer.SelectionLength;

                    // Insert the character at the cursor position
                    text = text.Substring(0, selectionStart) + keyChar + text.Substring(selectionStart + selectionLength);

                    if (int.TryParse(text, out int _) && text.Length > 16)
                    {
                        // Reject an integer that is longer than 16 digits.
                        e.Handled = true;
                    }
                    else if (double.TryParse(text, out double _) && text.IndexOf('.') >= 0 && text.IndexOf('.') < text.Length - 3)
                    {
                        // Reject a real number with too many decimal places.
                        e.Handled = true;
                    }
                }
                else
                {
                    // Reject all other characters.
                    e.Handled = true;
                }
            

        }
        private void txtVAT_KeyPress(object sender, KeyPressEventArgs e)
        {

                char keyChar = e.KeyChar;

                if (char.IsControl(keyChar))
                {
                    // Allow all control characters.
                }
                else if (char.IsDigit(keyChar) || keyChar == '.')
                {
                    string text = this.txtVAT.Text;
                    int selectionStart = this.txtVAT.SelectionStart;
                    int selectionLength = this.txtVAT.SelectionLength;

                    // Insert the character at the cursor position
                    text = text.Substring(0, selectionStart) + keyChar + text.Substring(selectionStart + selectionLength);

                    if (int.TryParse(text, out int _) && text.Length > 16)
                    {
                        // Reject an integer that is longer than 16 digits.
                        e.Handled = true;
                    }
                    else if (double.TryParse(text, out double _) && text.IndexOf('.') >= 0 && text.IndexOf('.') < text.Length - 3)
                    {
                        // Reject a real number with too many decimal places.
                        e.Handled = true;
                    }
                }
                else
                {
                    // Reject all other characters.
                    e.Handled = true;
                }
            

        }
        private void txtSellingPrice_TextChanged(object sender, EventArgs e)
        {
            Compute();
        }
        private void txtDiscountPer_TextChanged(object sender, EventArgs e)
        {
            Compute();
        }
        private void txtVAT_TextChanged(object sender, EventArgs e)
        {
            Compute();

        }

        private void Quotation_Load(object sender, EventArgs e)
        {

        }
        protected override void WndProc(ref Message m)
        {
            const int WM_NCLBUTTONDOWN = 0x00A1;
            const int HTCAPTION = 0x0002;

            if (m.Msg == WM_NCLBUTTONDOWN && (int)m.LParam == HTCAPTION)
            {
                // Prevent dragging
                return;
            }

            base.WndProc(ref m);
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
