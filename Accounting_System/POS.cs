using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;
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
using static DevExpress.Data.Helpers.ExpressiveSortInfo;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Accounting_System
{

    public partial class POS : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());

        public static POS _instance;
        public static POS instance;
        public static POS Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new POS();
                }
                return _instance;
            }

        }

        public POS()
        {
            InitializeComponent();

            ComboBox1.SelectedIndex = 0;
            txtDiscountPer.TextChanged += new EventHandler(txtDiscountPer_TextChanged);
            txtVAT.TextChanged += new EventHandler(txtVAT_TextChanged);
            DataGridView1.MouseDoubleClick += new MouseEventHandler(DataGridView1_MouseDoubleClick);

            txtCustomerID.Text = "C-0001";
            txtTotalPayment.Text = "0.00";
            txtPaymentDue.Text = "0.00";
            instance = this;
        }
        public static class SessionData
        {

            public static string CustomerName { get; set; }
            public static string SalesManName { get; set; }
        }
        public static class ProductData
        {
            public static string ProductCode { get; set; }
            public static string ProductName { get; set; }
            public static string ProductBrecode { get; set; }
            public static string Qty { get; set; }
            public static string CostPrice { get; set; }
            public static string SellingPrice { get; set; }
            public static string DiscountPer { get; set; }
            public static string DiscountAmount { get; set; }
            public static string VAT { get; set; }
            public static string VATAmount { get; set; }
            public static string Margin { get; set; }
            public static string TotalAmount { get; set; }
            public static string GrandTotal { get; set; }
            public static string TotalPayment { get; set; }
            public static string PaymentDue { get; set; }
        }

        // Nested static class for transaction-related session data
        public static class TransactionData
        {
            public static string InvoiceNo { get; set; }
            public static DateTime InvoiceDate { get; set; } = DateTime.Today;
            public static string Remarks { get; set; }
            public static string PaymentMode { get; set; }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            SalesManScreen salesMan = new SalesManScreen();
            salesMan.lblSet.Text = "POS Entry";
            salesMan.Show();
        }
        public void Getdata1()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT PID, RTRIM(Product.ProductCode),RTRIM(ProductName),RTRIM(Temp_Stock.Barcode),(CostPrice),(SellingPrice),(Discount),(VAT),Qty, RTRIM(Product.SellingPrice2),Plimit from Temp_Stock,Product where Product.PID=Temp_Stock.ProductID and Qty > 0  order by ProductCode", con);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dgw.Rows.Clear();
                while (rdr.Read() == true)
                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10]);
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            auto();
        }

        private void POS_Load(object sender, EventArgs e)
        {
            // Set default values
            cmbPaymentMode.SelectedIndex = 0;
            txtCustomerID.Text = "C-0001";
            txtCustomerName.Text = "عميل نقدي";
            txtCID.Text = "1";
            txtContactNo.Text = "00000000";
            txtBarcode.Focus();
            txtSM_ID.Text = "1";
            txtSalesmanID.Text = "SM-0001";
            txtSalesman.Text = "مندوب 1";
            txtCommissionPer.Text = "0.000";

            // Check customer ID and update fields accordingly
            if (txtCustomerID.Text == "C-0001")
            {
                cmbPaymentMode.SelectedIndex = 0;
                /*                txtPayment.ReadOnly = true;
                */
                txtPayment.Text = txtGrandTotal.Text;
            }
            else
            {
                /*                txtPayment.ReadOnly = false;
                */
            }

         
            Getdata1();
            Reset();    

        }

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void txtDiscountPer_TextChanged(object sender, EventArgs e)
        {
            Compute();
        }

        private void Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Button4_Click(object sender, EventArgs e)
        {

        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            SalesManScreen salesManScreen = new SalesManScreen();
            salesManScreen.lblSet.Text = "Billing";
            salesManScreen.Show();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {

            CTOPOS ctopos = new CTOPOS();
            ctopos.lblSet.Text = "Billing";
            ctopos.Show();

        }


        private void button5_Click(object sender, EventArgs e)
        {
            AddCustomer addCustomer = new AddCustomer();
            addCustomer.Reset();
            addCustomer.lblSet.Text = "TOPOS";
            addCustomer.Show();
        }

        private void txtCustomerName_TextChanged(object sender, EventArgs e)
        {

        }


        public void auto()
        {
            try
            {
                txtID.Text = GenerateID();
                txtInvoiceNo.Text = "Inv-" + GenerateID();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public void Reset()
        {
        // Reset text fields and combo boxes
            ComboBox1.SelectedIndex = 0;
            txtRemarks.Text = "";
            TextBox1.Text = "";
            txtAmount.Text = "";
            txtCostPrice.Text = "";
            txtDiscountAmount.Text = "";
            txtDiscountPer.Text = "";
            txtMargin.Text = "";
            txtInvoiceNo.Text = "";
            txtProductCode.Text = "";
            txtProductName.Text = "";
            txtQty.Text = "";
            txtSellingPrice.Text = "";
            txtTotalAmount.Text = "";
            txtTotalQty.Text = "";
            txtVAT.Text = "";
            txtVATAmount.Text = "";
            txtGrandTotal.Text = "";
            txtTotalPayment.Text = "";
            txtPayment.Text = "0.00";
            txtTotalPayment.Text = "0.00";
            txtPaymentDue.Text = "0.00";

            // Reset date to today
            dtpInvoiceDate.Value = DateTime.Today;

            // Reset DataGridView visibility and clear rows
            dgw.Visible = false;
            DataGridView1.Rows.Clear();
            DataGridView2.Rows.Clear();

            // Reset button states
            btnDelete.Enabled = false;
            btnUpdate.Enabled = false;
            btnSave.Enabled = true;
            btnRemove.Enabled = false;
            btnAdd.Enabled = true;
            btnRemove1.Enabled = false;
            btnAdd1.Enabled = true;
            btnPrint.Enabled = false;
            Button2.Enabled = true;
            Button3.Enabled = false;
            button4.Enabled = false;    
            Button1.Enabled = true;

            // Perform additional resets
            auto();
            lblSet.Text = "Allowed";
            Clear1();
            Clear();

            // Set default payment mode
            cmbPaymentMode.SelectedIndex = 0;

            txtCustomerID.Text = "C-0001";
        }

        public void Clear1()
        {
            cmbPaymentMode.SelectedIndex = 0;
            dtpPaymentDate.Text = DateTime.Today.ToString();
            btnAdd1.Enabled = true;
            btnRemove1.Enabled = false;
            btnListUpdate1.Enabled = false;
        }

        public void Clear()
        {

            txtBarcode.Text = "";
            txtProductCode.Text = "";
            txtProductName.Text = "";
            txtCostPrice.Text = "";
            txtSellingPrice.Text = "";
            txtMargin.Text = "";
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
            dgw.Visible = false;
            txtProductName.Enabled = true; 
            txtBarcode.Focus();
        }
        public void ClearAddProdact()
        {

            // txtDiscountPer.Text = ""
            txtBarcode.Text = "";
            txtProductCode.Text = "";
            txtProductName.Text = "";
            txtCostPrice.Text = "";
            txtSellingPrice.Text = "";
            txtMargin.Text = "";
            txtQty.Text = "";
            txtAmount.Text = "";
            txtDiscountPer.Text = "";
            txtDiscountAmount.Text = "";
            txtVAT.Text = "";
            txtVATAmount.Text = "";
            btnAdd.Enabled = true;
            btnRemove.Enabled = false;
            btnListUpdate.Enabled = false;
            dgw.Visible = false;
            txtBarcode.Focus();
            cmbPaymentMode.SelectedIndex = 0;
            dtpPaymentDate.Text = DateTime.Today.ToString();
            btnAdd1.Enabled = true;
            btnRemove1.Enabled = false;
            btnListUpdate1.Enabled = false;
        }
        private string GenerateID()
        {
            string value = "0001"; // Default value in case there's no record in the database
            try
            {
                // Open the database connection
                con.Open();

                // Fetch the latest ID from the database
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 Inv_ID FROM InvoiceInfo ORDER BY Inv_ID DESC", con);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                if (rdr.HasRows)
                {
                    rdr.Read();
                    value = rdr["Inv_ID"].ToString(); // Read the current ID from the database
                }

                rdr.Close();

                // Increment the ID by 1
                int numericValue = int.Parse(value);
                numericValue++;

                // Format the ID with leading zeros
                value = numericValue.ToString("D4"); // Ensures the ID is 4 digits long with leading zeros
            }
            catch (Exception ex)
            {
                // Handle the exception and log it if needed

                // Close the connection if it's still open
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                value = "0001"; // Fallback value in case of an error
            }
            finally
            {
                // Ensure the connection is closed
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }

            return value;
        }


        private void Button2_Click_1(object sender, EventArgs e)
        {
            string cs = DataAccessLayer.Con();
            try
            {
                if (string.IsNullOrWhiteSpace(txtSalesmanID.Text))
                {
                    MessageBox.Show("الرجاء تحديد رقم البائع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSalesmanID.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
                {
                    MessageBox.Show("الرجاء تحديد العميل", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCustomerName.Focus();
                    return;
                }
                if (DataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("الرجاء اضافة اصناف للشبكة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (DataGridView2.Rows.Count == 0)
                {
                    DataGridView2.Rows.Add(cmbPaymentMode.Text, txtPayment.Text, dtpPaymentDate.Value.Date);
                    double totalPayment = Math.Round(TotalPayment(), 2);
                    txtTotalPayment.Text = totalPayment.ToString();
                    Compute1();
                }
                if (Convert.ToDouble(txtTotalPayment.Text) > Convert.ToDouble(txtGrandTotal.Text))
                {
                    MessageBox.Show("المبلغ المدفوع لا يكون اكبر من قيمة الفاتورة", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    string ctn1 = "select * from Company";
                    using (SqlCommand cmd = new SqlCommand(ctn1, con))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (!rdr.Read())
                            {
                                MessageBox.Show("الرجاء اضافة ملف تعريفي للشركة في القائمة الرئيسية ", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }
                    }

                    foreach (DataGridViewRow row in DataGridView1.Rows)
                    {
                        using (SqlConnection con2 = new SqlConnection(cs))
                        {
                            con2.Open();
                            string query = "SELECT Qty from Temp_Stock where ProductID=@d1 and Barcode=@d2";
                            using (SqlCommand cmd = new SqlCommand(query, con2))
                            {
                                cmd.Parameters.AddWithValue("@d1", row.Cells[13].Value);
                                cmd.Parameters.AddWithValue("@d2", row.Cells[2].Value);

                                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                                {
                                    DataSet ds = new DataSet();
                                    da.Fill(ds);
                                    if (ds.Tables[0].Rows.Count > 0)
                                    {
                                        int totalQty = Convert.ToInt32(ds.Tables[0].Rows[0]["Qty"]);
                                        if (Convert.ToInt32(row.Cells[6].Value) > totalQty)
                                        {
                                            MessageBox.Show($"الكمية المضافة إلى سلة البيع أكثر من الكمية المتوفرة من المنتج '{row.Cells[0].Value}' واسم المنتج ='{row.Cells[1].Value}' وجود الباركود ='{row.Cells[2].Value}'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (!txtCustomerName.ReadOnly)
                    {
                        auto1();
                        string cbn = "insert into Customer(ID, CustomerID, [Name], Gender, Address, City, ContactNo, EmailID,Remarks,State,ZipCode,Photo,CustomerType) Values (@d1,@d2,@d3,@d4,@d5,@d6,@d7,@d8,@d9,@d10,@d11,@d12,'Non Regular')";
                        using (SqlCommand cmd = new SqlCommand(cbn, con))
                        {
                            cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtCID.Text));
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

                            using (MemoryStream ms = new MemoryStream())
                            {
                                Bitmap bmpImage = new Bitmap(Properties.Resources.if_icons_user);
                                bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                byte[] data = ms.ToArray();

                                SqlParameter p = new SqlParameter("@d12", SqlDbType.Image)
                                {
                                    Value = data
                                };
                                cmd.Parameters.Add(p);
                            }

                            cmd.ExecuteNonQuery();
                            txtCustomerType.Text = "Non Regular";
                        }
                    }

                    string invoiceQuery = "insert into InvoiceInfo(Inv_ID, InvoiceNo, InvoiceDate, CustomerID, GrandTotal, TotalPaid, Balance, Remarks, SalesmanID) Values (@d1,@d2,@d3,@d4,@d5,@d6,@d7,@d8,@d9)";
                    using (SqlCommand cmd = new SqlCommand(invoiceQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                        cmd.Parameters.AddWithValue("@d2", txtInvoiceNo.Text);
                        cmd.Parameters.AddWithValue("@d3", dtpInvoiceDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d4", Convert.ToInt32(txtCID.Text));
                        cmd.Parameters.AddWithValue("@d5", Convert.ToDouble(txtGrandTotal.Text));
                        cmd.Parameters.AddWithValue("@d6", Convert.ToDouble(txtTotalPayment.Text));
                        cmd.Parameters.AddWithValue("@d7", Convert.ToDouble(txtPaymentDue.Text));
                        cmd.Parameters.AddWithValue("@d8", txtRemarks.Text);
                        cmd.Parameters.AddWithValue("@d9", Convert.ToInt32(txtSM_ID.Text));
                        cmd.ExecuteNonQuery();
                    }

                    decimal sum = 0m;
                    decimal commission;
                    foreach (DataGridViewRow row in DataGridView1.Rows)
                    {
                        sum += Convert.ToDecimal(row.Cells[12].Value);
                    }
                    commission = sum * Convert.ToDecimal(txtCommissionPer.Text) / 100;

                    string commissionQuery = "insert into Salesman_Commission(InvoiceID, CommissionPer, Commission) VALUES (@T1,@T2,@T3)";
                    using (SqlCommand cmd = new SqlCommand(commissionQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@T1", txtID.Text);
                        cmd.Parameters.AddWithValue("@T2", Convert.ToDouble(txtCommissionPer.Text));
                        cmd.Parameters.AddWithValue("@T3", commission);
                        cmd.ExecuteNonQuery();
                    }

                    string invoiceProductQuery = "insert into Invoice_Product(InvoiceID, Barcode, CostPrice, SellingPrice, Margin, Qty, Amount, DiscountPer, Discount, VATPer, VAT, TotalAmount, ProductID) VALUES (@d1,@d2,@d3,@d4,@d5,@d6,@d7,@d8,@d9,@d10,@d11,@d12,@d13)";
                    using (SqlCommand cmd = new SqlCommand(invoiceProductQuery, con))
                    {
                        foreach (DataGridViewRow row in DataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@d1", txtID.Text);
                                cmd.Parameters.AddWithValue("@d2", row.Cells[2].Value);
                                cmd.Parameters.AddWithValue("@d3", Convert.ToDouble(row.Cells[3].Value));
                                cmd.Parameters.AddWithValue("@d4", Convert.ToDouble(row.Cells[4].Value));
                                cmd.Parameters.AddWithValue("@d5", Convert.ToDouble(row.Cells[5].Value));
                                cmd.Parameters.AddWithValue("@d6", Convert.ToInt32(row.Cells[6].Value));
                                cmd.Parameters.AddWithValue("@d7", Convert.ToDouble(row.Cells[7].Value));
                                cmd.Parameters.AddWithValue("@d8", Convert.ToDouble(row.Cells[8].Value));
                                cmd.Parameters.AddWithValue("@d9", Convert.ToDouble(row.Cells[9].Value));
                                cmd.Parameters.AddWithValue("@d10", Convert.ToDouble(row.Cells[10].Value));
                                cmd.Parameters.AddWithValue("@d11", Convert.ToDouble(row.Cells[11].Value));
                                cmd.Parameters.AddWithValue("@d12", Convert.ToDouble(row.Cells[12].Value));
                                cmd.Parameters.AddWithValue("@d13", Convert.ToInt32(row.Cells[13].Value));
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    string invoicePaymentQuery = "insert into Invoice_Payment(InvoiceID,PaymentMode,TotalPaid,PaymentDate) VALUES (@d1,@d2,@d3,@d4)";
                    using (SqlCommand cmd = new SqlCommand(invoicePaymentQuery, con))
                    {
                        foreach (DataGridViewRow row in DataGridView2.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@d1", txtID.Text);
                                cmd.Parameters.AddWithValue("@d2", row.Cells[0].Value);
                                cmd.Parameters.AddWithValue("@d3", Convert.ToDouble(row.Cells[1].Value));
                                cmd.Parameters.AddWithValue("@d4", Convert.ToDateTime(row.Cells[2].Value));
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    string stockUpdateQuery = "Update Temp_Stock set Qty = Qty - @Qty where ProductID=@d1";
                    using (SqlCommand cmd = new SqlCommand(stockUpdateQuery, con))
                    {
                        foreach (DataGridViewRow row in DataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
                            {

                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@Qty", Convert.ToInt32(row.Cells[6].Value));
                                cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(row.Cells[13].Value));
                                cmd.Parameters.AddWithValue("@d2", row.Cells[2].Value);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    string logsQuery = "insert into Logs(Date,Operation,UserID) VALUES (@d1,@d2,@d3)";
                    using (SqlCommand cmd = new SqlCommand(logsQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", DateTime.UtcNow.ToLocalTime());
                        cmd.Parameters.AddWithValue("@d2", $"تم انشاء الفاتورة ذات الرقم '{txtInvoiceNo.Text}'");
                        cmd.Parameters.AddWithValue("@d3", lblUser.Text);
                        cmd.ExecuteNonQuery();
                    }

                    btnSave.Enabled = false;
                    btnPrint.Enabled = true;
                    MessageBox.Show("تمت عملية الحفظ بنجاح", "الفاتورة", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private double GrandTotal()
        {
            double sum = 0d;
            try
            {
                foreach (DataGridViewRow r in this.DataGridView1.Rows)
                    // txtPayment.Text = sum
                    sum = sum + Convert.ToDouble( r.Cells[12].Value.ToString());
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
            }
            return sum;
        }


        public void SMS(string st1)
        {
            con.Open();
            string cb = "insert into SMS(Message,Date) VALUES (@d1,@d2)";
            SqlCommand cmd = new SqlCommand(cb);
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@d1", st1);
            cmd.Parameters.AddWithValue("@d2", DateTime.Now);
            cmd.ExecuteReader();
            con.Close();
        }

        public void LogFunc(string st1, string st2)
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                string cb = "insert into Logs(UserID,Date,Operation) VALUES (@d1,@d2,@d3)";
                SqlCommand cmd = new SqlCommand(cb);
                cmd.Connection = con;
                cmd.Parameters.AddWithValue("@d1", st1);
                cmd.Parameters.AddWithValue("@d2", DateTime.Now);
                cmd.Parameters.AddWithValue("@d3", st2);
                cmd.ExecuteReader();
                con.Close();
            }
        }

        public void LedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g, string h)
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                string cb = "insert into LedgerBook(Date, Name, LedgerNo, Label,Debit,Credit,PartyID,Manual_Inv) Values (@d1,@d2,@d3,@d4,@d5,@d6,@d7,@d8)";
                SqlCommand cmd = new SqlCommand(cb);
                cmd.Parameters.AddWithValue("@d1", a);
                cmd.Parameters.AddWithValue("@d2", b);
                cmd.Parameters.AddWithValue("@d3", c);
                cmd.Parameters.AddWithValue("@d4", d);
                cmd.Parameters.AddWithValue("@d5", e);
                cmd.Parameters.AddWithValue("@d6", f);
                cmd.Parameters.AddWithValue("@d7", g);
                cmd.Parameters.AddWithValue("@d8", h);
                cmd.Connection = con;
                cmd.ExecuteReader();
                con.Close();
            }
        }
        public void LedgerDelete(string a, string b)
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                string cq = "delete from LedgerBook where LedgerNo=@d1 and Label=@d2";
                SqlCommand cmd = new SqlCommand(cq);
                cmd.Parameters.AddWithValue("@d1", a);
                cmd.Parameters.AddWithValue("@d2", b);
                cmd.Connection = con;
                cmd.ExecuteReader();
                con.Close();
            }
        }
        public void LedgerUpdate(DateTime a, string b, decimal e, decimal f, string g, string h, string i)
        {
            con.Open();
            string cb = "Update LedgerBook set Date=@d1, Name=@d2,Debit=@d3,Credit=@d4,PartyID=@d5 where LedgerNo=@d6 and Label=@d7";
            SqlCommand cmd = new SqlCommand(cb);
            cmd.Parameters.AddWithValue("@d1", a);
            cmd.Parameters.AddWithValue("@d2", b);
            cmd.Parameters.AddWithValue("@d3", e);
            cmd.Parameters.AddWithValue("@d4", f);
            cmd.Parameters.AddWithValue("@d5", g);
            cmd.Parameters.AddWithValue("@d6", h);
            cmd.Parameters.AddWithValue("@d7", i);
            cmd.Connection = con;
            cmd.ExecuteReader();
            con.Close();
        }

        private void btnSelectionInv_Click(object sender, EventArgs e)
        {
            Stock stock = new Stock();
            stock.lblSet.Text = "Billing";
            stock.ShowDialog();
        }

        private void txtSellingPrice_TextChanged(object sender, EventArgs e)
        {

        }
        private void txtVAT_TextChanged(object sender, EventArgs e)
        {
            Compute();
        }

        private void txtDiscountAmount_TextChanged(object sender, EventArgs e)
        {
            txtTotalAmount.Text = (
            (string.IsNullOrWhiteSpace(txtAmount.Text) ? 0 : Convert.ToDouble(txtAmount.Text)) +
            (string.IsNullOrWhiteSpace(txtVATAmount.Text) ? 0 : Convert.ToDouble(txtVATAmount.Text)) -
            (string.IsNullOrWhiteSpace(txtDiscountAmount.Text) ? 0 : Convert.ToDouble(txtDiscountAmount.Text))
        ).ToString("F2");

        }
        public void Compute()
        {
            double num1, num2, num3, num4, num5;

            // Use helper method to parse or return 0 if empty/invalid
            double sellingPrice = ParseOrDefault(txtSellingPrice.Text);
            double costPrice = ParseOrDefault(txtCostPrice.Text);
            double qty = ParseOrDefault(txtQty.Text);
            double discountPer = ParseOrDefault(txtDiscountPer.Text);
            double vat = ParseOrDefault(txtVAT.Text);

            // Calculate Margin
            txtMargin.Text = ((sellingPrice - costPrice) * qty).ToString("F2");

            // Calculate Amount
            num1 = qty * sellingPrice;
            num1 = Math.Round(num1, 2);
            txtAmount.Text = num1.ToString("F2");

            // Calculate Discount Amount
            num2 = num1 * discountPer / 100;
            num2 = Math.Round(num2, 2);
            txtDiscountAmount.Text = num2.ToString("F2");

            // Calculate VAT Amount
            num3 = num1 - num2;
            num4 = vat * num3 / 100;
            num4 = Math.Round(num4, 2);
            txtVATAmount.Text = num4.ToString("F2");

            // Calculate Total Amount
            num5 = num1 + num4 - num2;
            num5 = Math.Round(num5, 2);
            txtTotalAmount.Text = num5.ToString("F2");
        }

        // Helper method to parse a string to double or return 0 if invalid/empty
        private double ParseOrDefault(string input)
        {
            if (double.TryParse(input, out double result))
            {
                return result;
            }
            return 0;
        }



        private void txtQty_TextChanged(object sender, EventArgs e)
        {
            Compute();
        }


        public void Compute1()
        {
            double i = 0d;
            i = ParseOrDefault(txtGrandTotal.Text) - ParseOrDefault(txtTotalPayment.Text);
            i = Math.Round(i, 2);
            txtPaymentDue.Text = i.ToString();
        }
        private void Compute2()
        {
            double totalPayment = 0;
            foreach (DataGridViewRow row in DataGridView2.Rows)
            {
                if (!row.IsNewRow)
                {
                    if (double.TryParse(row.Cells[1].Value.ToString(), out double paymentAmount))
                    {
                        totalPayment += paymentAmount;
                    }
                }
            }

            txtTotalPayment.Text = totalPayment.ToString("F2");

            if (double.TryParse(txtGrandTotal.Text, out double grandTotal) &&
                double.TryParse(txtTotalPayment.Text, out totalPayment))
            {
                /*                txtChange.Text = (totalPayment - grandTotal).ToString("F2");
                */
            }
        }

        private double TotalPayment()
        {
            double totalPayment = 0;
            foreach (DataGridViewRow row in DataGridView2.Rows)
            {
                if (!row.IsNewRow)
                {
                    if (double.TryParse(row.Cells[1].Value.ToString(), out double paymentAmount))
                    {
                        totalPayment += paymentAmount;
                    }
                }
            }
            return totalPayment;
        }
        /*
                public double TotalPayment()
                {
                    double sum = 0d;
                    try
                    {
                        if (DataGridView1.Rows.Count == 0)
                        {
                            sum = Convert.ToDouble(txtGrandTotal.Text);
                        }
                        else
                        {
                            foreach (DataGridViewRow r in this.DataGridView2.Rows)
                            {
                                if (r.Cells[1].Value != null)
                                {
                                    sum += Convert.ToDouble(r.Cells[1].Value);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return sum;
                }
        */
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Set default values for fields if empty
                txtQty.Text = string.IsNullOrWhiteSpace(txtQty.Text) ? "1" : txtQty.Text;
                txtDiscountPer.Text = string.IsNullOrWhiteSpace(txtDiscountPer.Text) ? "0" : txtDiscountPer.Text;
                txtVAT.Text = string.IsNullOrWhiteSpace(txtVAT.Text) ? "0" : txtVAT.Text;

                // Validation checks for required fields
                if (string.IsNullOrWhiteSpace(txtProductCode.Text))
                {
                    MessageBox.Show("الرجاء إدراج رقم الصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProductCode.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtBarcode.Text))
                {
                    MessageBox.Show("الرجاء إدراج الباركود للصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBarcode.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtSellingPrice.Text))
                {
                    MessageBox.Show("الرجاء كتلبة السعر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    MessageBox.Show("الرجاء تحديد الكمية", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQty.Focus();
                    return;
                }
                if (txtQty.Text == "0")
                {
                    MessageBox.Show("الكمية يجب ان تكون اكبر من الصفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQty.Focus();
                    return;
                }
                if (Convert.ToDecimal(txtSellingPrice.Text) < Convert.ToDecimal(Plimit.Text))
                {
                    MessageBox.Show("لا يمكن تخطي حد السعر "+ Plimit.Text, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                // Check if the product already exists in the DataGridView
                bool rowUpdated = false;

                foreach (DataGridViewRow r in DataGridView1.Rows)
                {
                    if (r.Cells[0].Value.ToString() == txtProductCode.Text && r.Cells[2].Value.ToString() == txtBarcode.Text)
                    {
                        // Update existing row
                        r.Cells[0].Value = txtProductCode.Text;
                        r.Cells[1].Value = txtProductName.Text;
                        r.Cells[2].Value = txtBarcode.Text;
                        r.Cells[3].Value = Convert.ToDouble(txtCostPrice.Text);
                        r.Cells[4].Value = Convert.ToDouble(txtSellingPrice.Text);
                        r.Cells[5].Value = Convert.ToDouble(txtMargin.Text);
                        r.Cells[6].Value = Convert.ToDouble(r.Cells[6].Value) + Convert.ToDouble(txtQty.Text);
                        r.Cells[7].Value = Convert.ToDouble(r.Cells[7].Value) + Convert.ToDouble(txtAmount.Text);
                        r.Cells[8].Value = Convert.ToDouble(txtDiscountPer.Text);
                        r.Cells[9].Value = Convert.ToDouble(r.Cells[9].Value) + Convert.ToDouble(txtDiscountAmount.Text);
                        r.Cells[10].Value = Convert.ToDouble(txtVAT.Text);
                        r.Cells[11].Value = Convert.ToDouble(r.Cells[11].Value) + Convert.ToDouble(txtVATAmount.Text);
                        r.Cells[12].Value = Convert.ToDouble(r.Cells[12].Value) + Convert.ToDouble(txtTotalAmount.Text);
                        r.Cells[13].Value = Convert.ToDouble(txtProductID.Text);

                        rowUpdated = true;
                        break;
                    }
                }

                if (!rowUpdated)
                {
                    // Add new row
                    DataGridView1.Rows.Add(
                        txtProductCode.Text, txtProductName.Text, txtBarcode.Text,
                        Convert.ToDouble(txtCostPrice.Text), Convert.ToDouble(txtSellingPrice.Text),
                        Convert.ToDouble(txtMargin.Text), Convert.ToDouble(txtQty.Text),
                        Convert.ToDouble(txtAmount.Text), Convert.ToDouble(txtDiscountPer.Text),
                        Convert.ToDouble(txtDiscountAmount.Text), Convert.ToDouble(txtVAT.Text),
                        Convert.ToDouble(txtVATAmount.Text), Convert.ToDouble(txtTotalAmount.Text),
                        Convert.ToDouble(txtProductID.Text)
                    );
                }

                // Update totals
                double grandTotal = GrandTotal();
                grandTotal = Math.Round(grandTotal, 2);
                txtGrandTotal.Text = grandTotal.ToString("F2");
                txtTotalPayment.Text = grandTotal.ToString("F2");

                // Recompute necessary values
                Compute1();

                // Reset specific fields
                txtPayment.Text = txtGrandTotal.Text.ToString();
                txtDiscountPer.Text = "";
                txtAmount.Text = "";
                txtDiscountAmount.Text = "";
                Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string GenerateID1()
        {
            string value = "0000";
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 ID FROM Customer ORDER BY ID DESC", con))
                    {
                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.Read())
                        {
                            value = rdr["ID"].ToString();
                        }
                    }
                }

                // Convert the ID to an integer, increment it, and then convert back to a string
                if (int.TryParse(value, out int idValue))
                {
                    idValue++;
                    // Pad the number with leading zeros
                    value = idValue.ToString("D4");
                }
                else
                {
                    value = "0001"; // Default ID if the parsing fails
                }
            }
            catch (Exception ex)
            {
                // Log the error (ex.Message) if necessary and handle any specific errors
                value = "0001"; // Default ID in case of an error
            }

            return value;
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {

            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtSalesmanID.Text))
            {
                MessageBox.Show("الرجاء تحديد رقم البائع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Button1.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("الرجاء تحديد العميل", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (DataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("الرجاء اضافة اصناف للشبكة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (DataGridView2.Rows.Count == 0)
            {
                DataGridView2.Rows.Add(cmbPaymentMode.Text, txtPayment.Text, dtpPaymentDate.Value.Date);
                double j = Math.Round(TotalPayment(), 2);
                txtTotalPayment.Text = j.ToString();
                Compute1();
            }
            if (cmbPaymentMode.SelectedIndex == 0)
            {
                DataGridView2.Rows.Clear(); 
                DataGridView2.Rows.Add(cmbPaymentMode.Text, txtPayment.Text, dtpPaymentDate.Value.Date);
                double j = Math.Round(TotalPayment(), 2);
                txtTotalPayment.Text = j.ToString();
                Compute1();
            }
            if (Convert.ToDouble(txtTotalPayment.Text) > Convert.ToDouble(txtGrandTotal.Text))
            {
                MessageBox.Show("المبلغ المدفوع لا يكون اكبر من قيمة الفاتورة", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();

                // Update InvoiceInfo
                string cb = "Update InvoiceInfo set InvoiceNo=@d2, CustomerID=@d4, GrandTotal=@d5, TotalPaid=@d6, Balance=@d7, Remarks=@d8, SalesmanID=@d9 where INV_ID=@d1";
                using (SqlCommand cmd = new SqlCommand(cb, con))
                {

                    cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                    cmd.Parameters.AddWithValue("@d2", txtInvoiceNo.Text);
                    cmd.Parameters.AddWithValue("@d4", Convert.ToInt32(txtCID.Text));
                    cmd.Parameters.AddWithValue("@d5", Convert.ToDouble(txtGrandTotal.Text));
                    cmd.Parameters.AddWithValue("@d6", Convert.ToDouble(txtTotalPayment.Text));
                    cmd.Parameters.AddWithValue("@d7", Convert.ToDouble(txtPaymentDue.Text));
                    cmd.Parameters.AddWithValue("@d8", txtRemarks.Text);
                    cmd.Parameters.AddWithValue("@d9", Convert.ToInt32(txtSM_ID.Text));
                    cmd.ExecuteNonQuery();
                }

                // Delete existing Invoice_Product entries
                using (SqlCommand cmd = new SqlCommand("delete from Invoice_Product where InvoiceID=@d1", con))
                {
                    cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                    cmd.ExecuteNonQuery();
                }

                // Insert new Invoice_Product entries
                string cb1 = "insert into Invoice_Product(InvoiceID, Barcode, CostPrice, SellingPrice, Margin, Qty, Amount, DiscountPer, Discount, VATPer, VAT, TotalAmount, ProductID) VALUES (@d1, @d4, @d5, @d6, @d7, @d8, @d9, @d10, @d11, @d12, @d13, @d14, @d15)";
                using (SqlCommand cmd = new SqlCommand(cb1, con))
                {
                    foreach (DataGridViewRow row in DataGridView1.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                            cmd.Parameters.AddWithValue("@d4", row.Cells[2].Value);
                            cmd.Parameters.AddWithValue("@d5", Convert.ToDecimal(row.Cells[3].Value));
                            cmd.Parameters.AddWithValue("@d6", Convert.ToDecimal(row.Cells[4].Value));
                            cmd.Parameters.AddWithValue("@d7", Convert.ToDecimal(row.Cells[5].Value));
                            cmd.Parameters.AddWithValue("@d8", Convert.ToDecimal(row.Cells[6].Value));
                            cmd.Parameters.AddWithValue("@d9", Convert.ToDecimal(row.Cells[7].Value));
                            cmd.Parameters.AddWithValue("@d10", Convert.ToDecimal(row.Cells[8].Value));
                            cmd.Parameters.AddWithValue("@d11", Convert.ToDecimal(row.Cells[9].Value));
                            cmd.Parameters.AddWithValue("@d12", Convert.ToDecimal(row.Cells[10].Value));
                            cmd.Parameters.AddWithValue("@d13", Convert.ToDecimal(row.Cells[11].Value));
                            cmd.Parameters.AddWithValue("@d14", Convert.ToDecimal(row.Cells[12].Value));
                            cmd.Parameters.AddWithValue("@d15", Convert.ToInt32(row.Cells[13].Value));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                // Delete existing Invoice_Payment entries
                using (SqlCommand cmd = new SqlCommand("delete from Invoice_Payment where InvoiceID=@d1", con))
                {
                    cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                    cmd.ExecuteNonQuery();
                }

                // Insert new Invoice_Payment entries
                string cb2 = "insert into Invoice_Payment(InvoiceID, PaymentMode, TotalPaid, PaymentDate) VALUES (@d1, @d4, @d5, @d6)";
                using (SqlCommand cmd = new SqlCommand(cb2, con))
                {
                    foreach (DataGridViewRow row in DataGridView2.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtID.Text));
                            cmd.Parameters.AddWithValue("@d4", row.Cells[0].Value);
                            cmd.Parameters.AddWithValue("@d5", Convert.ToDecimal(row.Cells[1].Value));
                            cmd.Parameters.AddWithValue("@d6", Convert.ToDateTime(row.Cells[2].Value));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open(); // Open the connection once outside the loop

                foreach (DataGridViewRow row1 in DataGridView1.Rows)
                {
                    bool matchFound = false;
                    foreach (DataGridViewRow row3 in dataGridView3.Rows)
                    {
                        // Check if Barcode matches (assuming Cells[2] is Barcode in both grids)
                        if (row1.Cells[2].Value != null && row3.Cells[2].Value != null &&
                            row1.Cells[2].Value.ToString() == row3.Cells[2].Value.ToString())
                        {
                            matchFound = true;

                            // Get the Qty values from both DataGridViews
                            double qty1 = Convert.ToDouble(row1.Cells[6].Value); // Qty from DataGridView1
                            double qty3 = Convert.ToDouble(row3.Cells[6].Value); // Qty from DataGridView3

                            // Calculate the quantity difference
                            double qtyDifference = qty1 - qty3;

                            // Only update if there is a difference
                            if (qtyDifference != 0)
                            {
                                string stockUpdateQuery = "Update Temp_Stock set Qty = Qty - @Qty where ProductID=@d1 and Barcode=@d2";
                                if (qtyDifference < 0) // If the qty3 is greater than qty1, we add the difference
                                {
                                    stockUpdateQuery = "Update Temp_Stock set Qty = Qty + @Qty where ProductID=@d1 and Barcode=@d2";
                                    qtyDifference = Math.Abs(qtyDifference); // Convert the difference to a positive value for addition
                                }

                                using (SqlCommand cmd = new SqlCommand(stockUpdateQuery, con))
                                {
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.AddWithValue("@Qty", qtyDifference);
                                    cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(row1.Cells[13].Value)); // ProductID from DataGridView1
                                    cmd.Parameters.AddWithValue("@d2", row1.Cells[2].Value); // Barcode from DataGridView1
                                    cmd.ExecuteNonQuery();
                                }

                            }

                            // Exit the inner loop once a match is found
                            break;
                        }
                    }

                    // If no match was found, update Qty for row1 as a standalone entry
                    if (!matchFound)
                    {
                        string stockUpdateQuery = "Update Temp_Stock set Qty = Qty - @Qty where ProductID=@d1";
                        using (SqlCommand cmd = new SqlCommand(stockUpdateQuery, con))
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@Qty", Convert.ToInt32(row1.Cells[6].Value)); // Qty from DataGridView1
                            cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(row1.Cells[13].Value)); // ProductID from DataGridView1
                            cmd.Parameters.AddWithValue("@d2", row1.Cells[2].Value); // Barcode from DataGridView1
                            cmd.ExecuteNonQuery();
                        }

                    }
                }

                con.Close(); // Close the connection after the loop
            }

           
            LedgerDelete(txtInvoiceNo.Text, "فاتورة مبيعات");
            LedgerDelete(txtInvoiceNo.Text, "دفعة فورية");
            LedgerSave(dtpInvoiceDate.Value.Date, txtCustomerName.Text, txtInvoiceNo.Text, "فاتورة مبيعات", Convert.ToDecimal(txtGrandTotal.Text), Convert.ToDecimal(txtTotalPayment.Text), txtCustomerID.Text, txtRemarks.Text);

            // Log the update action
            string st = $"updated the bill (Products) having invoice no. '{txtInvoiceNo.Text}'";
            LogFunc(lblUser.Text, st);

            // Disable the update button after successful update
            btnUpdate.Enabled = false;
            MessageBox.Show("تم التعديل بنجاح", "السجلات", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Reset();
        }



        private void btnGetData_Click(object sender, EventArgs e)
        {
            SalesInvoiceScreen frmSalesInvoiceRecord = new SalesInvoiceScreen();
            frmSalesInvoiceRecord.lblSet.Text = "Sales Invoice";
            frmSalesInvoiceRecord.Reset();
            frmSalesInvoiceRecord.ShowDialog();

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Reset();  // Replace FileSystem.Reset with appropriate form reset method
            cmbPaymentMode.SelectedIndex = 0;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            Timer1.Enabled = false;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            /*            PrintDocument();  // Replace FileSystem.Print with appropriate printing logic
            */
        }

        private void btnAdd1_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("الرجاء اضافة اصناف للشبكة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(cmbPaymentMode.Text))
                {
                    MessageBox.Show("الرجاء اختيار طريقة دفع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbPaymentMode.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txtPayment.Text))
                {
                    MessageBox.Show("الرجاء كتابة المبلغ المدفوع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPayment.Focus();
                    return;
                }

                DataGridView2.Rows.Add(cmbPaymentMode.Text, txtPayment.Text, dtpPaymentDate.Value.Date);
                double j = TotalPayment();
                txtTotalPayment.Text = Math.Round(j, 2).ToString();
                Compute1();
                Clear1();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);  // Replace Interaction.MsgBox with MessageBox
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("هل أنت متأكد من أنك تريد حذف سجل الفاتورة؟", "تاكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    DeleteRecord();

                    Reset();  // Replace FileSystem.Reset with appropriate form reset method
                    cmbPaymentMode.SelectedIndex = 0;
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

                    string queryCheck = "SELECT Inv_ID FROM InvoiceInfo INNER JOIN SalesReturn ON SalesReturn.SalesID = InvoiceInfo.Inv_ID WHERE Inv_ID = @d1";
                    using (SqlCommand cmdCheck = new SqlCommand(queryCheck, con))
                    {
                        cmdCheck.Parameters.AddWithValue("@d1", int.Parse(txtID.Text));
                        using (SqlDataReader rdr = cmdCheck.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                MessageBox.Show("غير قادر على الحذف .. مستخدمة مسبقًا في إرجاع المبيعات", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }

                    string queryDelete = "DELETE FROM InvoiceInfo WHERE Inv_ID = @d1";
                    using (SqlCommand cmdDelete = new SqlCommand(queryDelete, con))
                    {
                        cmdDelete.Parameters.AddWithValue("@d1", int.Parse(txtID.Text));
                        rowsAffected = cmdDelete.ExecuteNonQuery();
                    }

                    if (rowsAffected > 0)
                    {
                        foreach (DataGridViewRow row in DataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                string updateStockQuery = "UPDATE Temp_Stock SET qty = qty + @qty WHERE ProductID = @d1 AND Barcode = @d2";
                                using (SqlCommand cmdUpdateStock = new SqlCommand(updateStockQuery, con))
                                {
                                    cmdUpdateStock.Parameters.AddWithValue("@qty", decimal.Parse(row.Cells[6].Value.ToString()));
                                    cmdUpdateStock.Parameters.AddWithValue("@d1", int.Parse(row.Cells[13].Value.ToString()));
                                    cmdUpdateStock.Parameters.AddWithValue("@d2", row.Cells[2].Value.ToString());
                                    cmdUpdateStock.ExecuteNonQuery();
                                }
                            }
                        }

                        LedgerDelete(txtInvoiceNo.Text, "فاتورة مبيعات");
                        LedgerDelete(txtInvoiceNo.Text, "دفعة فورية");

                        string logMessage = $"deleted the bill (Products) having invoice no. '{txtInvoiceNo.Text}'";
                        LogFunc(lblUser.Text, logMessage);

                        MessageBox.Show("تم الحذف بنجاح", "سجل", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FileSystem.Reset();
                    }
                    else
                    {
                        MessageBox.Show("لايوجد سجل", "عذرا", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FileSystem.Reset();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnRemove1_Click(object sender, EventArgs e)
        {

        }

        private void txtPayment_KeyPress(object sender, KeyPressEventArgs e)
        {

        }



        private void DataGridView2_MouseClick(object sender, MouseEventArgs e)
        {
            if (DataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow row = DataGridView2.SelectedRows[0];
                cmbPaymentMode.Text = row.Cells[0].Value.ToString();
                txtPayment.Text = row.Cells[1].Value.ToString();
                dtpPaymentDate.Value = Convert.ToDateTime(row.Cells[2].Value);

                btnRemove1.Enabled = true;
                btnListUpdate1.Enabled = true;
                btnAdd1.Enabled = false;
            }
        }

        private void btnListReset1_Click(object sender, EventArgs e)
        {
            Clear1();
        }

        private void btnListReset_Click(object sender, EventArgs e)
        {
            Clear();  // Assuming you have a Clear method for other purposes
        }

        private void DataGridView2_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            string strRowNumber = (e.RowIndex + 1).ToString();
            SizeF size = e.Graphics.MeasureString(strRowNumber, this.Font);
            if (DataGridView2.RowHeadersWidth < Convert.ToInt32(size.Width + 20))
            {
                DataGridView2.RowHeadersWidth = Convert.ToInt32(size.Width + 20);
            }
            Brush b = SystemBrushes.ControlText;
            e.Graphics.DrawString(strRowNumber, this.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + (e.RowBounds.Height - size.Height) / 2);
        }
        public void auto1()
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            string cs = DataAccessLayer.Con();
            try
            {
                // Validation checks
                if (string.IsNullOrWhiteSpace(txtSalesmanID.Text))
                {
                    MessageBox.Show("الرجاء تحديد رقم البائع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
                {
                    MessageBox.Show("الرجاء تحديد العميل", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (DataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("الرجاء اضافة اصناف للشبكة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (DataGridView2.Rows.Count == 0)
                {
                    DataGridView2.Rows.Add(cmbPaymentMode.Text, txtPayment.Text, dtpPaymentDate.Value.Date);
                    txtTotalPayment.Text = Math.Round(TotalPayment(), 2).ToString();
                    Compute1();
                }

                if (double.Parse(txtTotalPayment.Text) > double.Parse(txtGrandTotal.Text))
                {
                    MessageBox.Show("المبلغ المدفوع لا يكون اكبر من قيمة الفاتورة", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Company profile check
                using (var con = new SqlConnection(cs))
                {
                    con.Open();
                    using (var cmd = new SqlCommand("SELECT * FROM Company", con))
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (!rdr.Read())
                        {
                            MessageBox.Show("الرجاء اضافة ملف تعريفي للشركة في القائمة الرئيسية", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }

                    // Product quantity check
                    foreach (DataGridViewRow row in DataGridView1.Rows)
                    {
                        using (var cmd = new SqlCommand("SELECT Qty FROM Temp_Stock WHERE ProductID=@d1 AND Barcode=@d2", con))
                        {
                            cmd.Parameters.AddWithValue("@d1", row.Cells[13].Value);
                            cmd.Parameters.AddWithValue("@d2", row.Cells[2].Value);
                            using (var da = new SqlDataAdapter(cmd))
                            {
                                var ds = new DataSet();
                                da.Fill(ds);

                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    txtTotalQty.Text = ds.Tables[0].Rows[0]["Qty"].ToString();
                                    if (int.Parse(row.Cells[6].Value.ToString()) > int.Parse(txtTotalQty.Text))
                                    {
                                        MessageBox.Show("الكمية المضافة. إلى سلة البيع أكثر من" + Environment.NewLine +
                                                        "الكمية المتوفرة. من المنتج '" + row.Cells[0].Value + "' واسم المنتج ='" +
                                                        row.Cells[1].Value + "' وجود الباركود ='" + row.Cells[2].Value + "'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }
                                }
                            }
                        }
                    }

                    // Insert Customer
                    if (!txtCustomerName.ReadOnly)
                    {
                        auto1();
                        using (var cmd = new SqlCommand("INSERT INTO Customer(ID, CustomerID, [Name], Gender, Address, City, ContactNo, EmailID, Remarks, State, ZipCode, Photo, CustomerType) " +
                                                        "VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9, @d10, @d11, @d12, 'Non Regular')", con))
                        {
                            cmd.Parameters.AddWithValue("@d1", txtCID.Text);
                            cmd.Parameters.AddWithValue("@d2", txtCustomerID.Text);
                            cmd.Parameters.AddWithValue("@d3", txtCustomerName.Text);
                            cmd.Parameters.AddWithValue("@d4", string.Empty);
                            cmd.Parameters.AddWithValue("@d5", string.Empty);
                            cmd.Parameters.AddWithValue("@d6", string.Empty);
                            cmd.Parameters.AddWithValue("@d7", txtContactNo.Text);
                            cmd.Parameters.AddWithValue("@d8", string.Empty);
                            cmd.Parameters.AddWithValue("@d9", string.Empty);
                            cmd.Parameters.AddWithValue("@d10", string.Empty);
                            cmd.Parameters.AddWithValue("@d11", string.Empty);

                            using (var ms = new MemoryStream())
                            {
                                var bmpImage = new Bitmap(Properties.Resources.if_icons_user1);
                                bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                cmd.Parameters.AddWithValue("@d12", ms.ToArray());
                            }

                            cmd.ExecuteNonQuery();
                            txtCustomerType.Text = "Non Regular";
                        }
                    }

                    // Insert InvoiceInfo
                    using (var cmd = new SqlCommand("INSERT INTO InvoiceInfo(Inv_ID, InvoiceNo, InvoiceDate, CustomerID, GrandTotal, TotalPaid, Balance, Remarks, SalesmanID) " +
                                                    "VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9)", con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtID.Text);
                        cmd.Parameters.AddWithValue("@d2", txtInvoiceNo.Text);
                        cmd.Parameters.AddWithValue("@d3", dtpInvoiceDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d4", txtCID.Text);
                        cmd.Parameters.AddWithValue("@d5", txtGrandTotal.Text);
                        cmd.Parameters.AddWithValue("@d6", txtTotalPayment.Text);
                        cmd.Parameters.AddWithValue("@d7", txtPaymentDue.Text);
                        cmd.Parameters.AddWithValue("@d8", txtRemarks.Text);
                        cmd.Parameters.AddWithValue("@d9", txtSM_ID.Text);
                        cmd.ExecuteNonQuery();
                    }

                    // Insert Salesman_Commission
                    decimal sum = DataGridView1.Rows.Cast<DataGridViewRow>().Sum(r => decimal.Parse(r.Cells[12].Value.ToString()));
                    decimal commission = sum * decimal.Parse(txtCommissionPer.Text) / 100;

                    using (var cmd = new SqlCommand("INSERT INTO Salesman_Commission(InvoiceID, CommissionPer, Commission) VALUES (@T1, @T2, @T3)", con))
                    {
                        cmd.Parameters.AddWithValue("@T1", txtID.Text);
                        cmd.Parameters.AddWithValue("@T2", txtCommissionPer.Text);
                        cmd.Parameters.AddWithValue("@T3", commission);
                        cmd.ExecuteNonQuery();
                    }

                    // Insert Invoice_Product
                    using (var cmd = new SqlCommand("INSERT INTO Invoice_Product(InvoiceID, Barcode, CostPrice, SellingPrice, Margin, Qty, Amount, DiscountPer, Discount, VATPer, VAT, TotalAmount, ProductID) " +
                                                    "VALUES (@d1, @d4, @d5, @d6, @d7, @d8, @d9, @d10, @d11, @d12, @d13, @d14, @d15)", con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtID.Text);
                        foreach (DataGridViewRow row in DataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                cmd.Parameters.AddWithValue("@d4", row.Cells[2].Value);
                                cmd.Parameters.AddWithValue("@d5", row.Cells[3].Value);
                                cmd.Parameters.AddWithValue("@d6", row.Cells[4].Value);
                                cmd.Parameters.AddWithValue("@d7", row.Cells[5].Value);
                                cmd.Parameters.AddWithValue("@d8", row.Cells[6].Value);
                                cmd.Parameters.AddWithValue("@d9", row.Cells[7].Value);
                                cmd.Parameters.AddWithValue("@d10", row.Cells[8].Value);
                                cmd.Parameters.AddWithValue("@d11", row.Cells[9].Value);
                                cmd.Parameters.AddWithValue("@d12", row.Cells[10].Value);
                                cmd.Parameters.AddWithValue("@d13", row.Cells[11].Value);
                                cmd.Parameters.AddWithValue("@d14", row.Cells[12].Value);
                                cmd.Parameters.AddWithValue("@d15", row.Cells[13].Value);
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@d1", txtID.Text); // Re-add txtID.Text after clearing parameters
                            }
                        }
                    }

                    // Insert Invoice_Payment
                    using (var cmd = new SqlCommand("INSERT INTO Invoice_Payment(InvoiceID, PaymentMode, TotalPaid, PaymentDate) VALUES (@d1, @d4, @d5, @d6)", con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtID.Text);
                        foreach (DataGridViewRow row in DataGridView2.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                cmd.Parameters.AddWithValue("@d4", row.Cells[0].Value);
                                cmd.Parameters.AddWithValue("@d5", row.Cells[1].Value);
                                cmd.Parameters.AddWithValue("@d6", row.Cells[2].Value);
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@d1", txtID.Text); // Re-add txtID.Text after clearing parameters
                            }
                        }
                    }

                    // Update Temp_Stock
                    foreach (DataGridViewRow row in DataGridView1.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            using (var cmd = new SqlCommand("UPDATE Temp_stock SET Qty = Qty - @d3 WHERE ProductID=@d1", con))
                            {

                                cmd.Parameters.AddWithValue("@d1", row.Cells[13].Value);
                                cmd.Parameters.AddWithValue("@d2", row.Cells[2].Value);
                                cmd.Parameters.AddWithValue("@d3", row.Cells[6].Value);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                // Final confirmation message
                MessageBox.Show("تم الحفظ بنجاح", "الفاتورة", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
                btnPrint.Enabled = true;
                txtCustomerType.Text = "Regular";
                Print();
                Reset();
            }
            catch (Exception ex)
            {

            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in DataGridView1.SelectedRows)
                {
                    DataGridView1.Rows.Remove(row);
                }

                double k = 0;
                k = GrandTotal();
                k = Math.Round(k, 2);
                txtGrandTotal.Text = k.ToString();
                Compute();
                Compute1();
                Clear();
                txtPayment.Text = txtGrandTotal.Text.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnListUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(txtProductCode.Text))
                {
                    MessageBox.Show("الرجاء ادخال المنتج", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProductCode.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtBarcode.Text))
                {
                    MessageBox.Show("الرجاء إدراج الباركود للصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBarcode.Focus();
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
                    MessageBox.Show("الرجاء تحديد الكمية", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQty.Focus();
                    return;
                }
                if (Convert.ToDecimal(txtSellingPrice.Text) < Convert.ToDecimal(Plimit.Text))
                {
                    MessageBox.Show("لا يمكن تخطي حد السعر " + Plimit.Text, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (Convert.ToDecimal(txtQty.Text) <= 0)
                {
                    MessageBox.Show("الكمية يجب ان تكون اكبر من الصفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQty.Focus();
                    return;
                }
                if (Convert.ToDecimal(txtSellingPrice.Text) < Convert.ToDecimal(Plimit.Text))
                {
                    MessageBox.Show("لا يمكن تخطي حد السعر " + Plimit.Text, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Remove selected rows before adding the new one
                foreach (DataGridViewRow row in DataGridView1.SelectedRows)
                {
                    DataGridView1.Rows.Remove(row);
                }

                // Add new row to DataGridView1
                DataGridView1.Rows.Add(
                    txtProductCode.Text,
                    txtProductName.Text,
                    txtBarcode.Text,
                    Convert.ToDecimal(txtCostPrice.Text),
                    Convert.ToDecimal(txtSellingPrice.Text),
                    Convert.ToDecimal(txtMargin.Text),
                    Convert.ToDecimal(txtQty.Text),
                    Convert.ToDecimal(txtAmount.Text),
                    Convert.ToDecimal(txtDiscountPer.Text),
                    Convert.ToDecimal(txtDiscountAmount.Text),
                    Convert.ToDecimal(txtVAT.Text),
                    Convert.ToDecimal(txtVATAmount.Text),
                    Convert.ToDecimal(txtTotalAmount.Text),
                    Convert.ToInt32(txtProductID.Text) // Assuming ProductID is an integer
                );

                // Calculate Grand Total and update the textbox
                double grandTotal = Math.Round(GrandTotal(), 2);
                cmbPaymentMode.SelectedIndex = 0;
                txtGrandTotal.Text = grandTotal.ToString();
                txtPayment.Text = txtGrandTotal.Text.ToString();
                // Set payment amount to the total amount
                // Recompute other values if necessary
                Compute1();

                // Clear input fields
                Clear();
                txtProductName.Enabled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnListReset_Click_1(object sender, EventArgs e)
        {
            Clear();

        }

        private void txtPayment_TextChanged(object sender, EventArgs e)
        {
/*
            if (txtCustomerID.Text == "C-0001")
            {
                cmbPaymentMode.SelectedIndex = 0;
                txtPayment.ReadOnly = true;
                txtPayment.Text = Val(txtGrandTotal.Text);
                txtTotalPayment.Text = Val(txtGrandTotal.Text);
                Compute1();
            }
            else
            {
                txtPayment.ReadOnly = false;

            }*/

            if (txtCustomerID.Text == "C-0001")
            {
/*                cmbPaymentMode.SelectedIndex = 0;
 *                
*//*                txtPayment.Text = (txtGrandTotal.Text.ToString());
                txtTotalPayment.Text = txtGrandTotal.Text.ToString();*/
                /*                txtPayment.ReadOnly = true;*/
                /*                txtPayment.Text = txtGrandTotal.Text;
                *//*                txtTotalPayment.Text = txtGrandTotal.Text;
                */
                Compute1();
            }
            else
            {
                /*                txtPayment.ReadOnly = false;
                */
            }

            /*
                        if (cmbPaymentMode.SelectedIndex == 4)
                        {
                            txtPayment.ReadOnly = true;

                        }*/

        }
        private void txtCheck_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnAdd1_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Check if there are rows in DataGridView1
                if (DataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("الرجاء اضافة اصناف للشبكة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check if a payment mode is selected
                if (string.IsNullOrWhiteSpace(cmbPaymentMode.Text))
                {
                    MessageBox.Show("الرجاء اختيار طريقة دفع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbPaymentMode.Focus();
                    return;
                }

                // Check if payment amount is provided
                if (string.IsNullOrWhiteSpace(txtPayment.Text) || !decimal.TryParse(txtPayment.Text, out _))
                {
                    MessageBox.Show("الرجاء كتابة المبلغ المدفوع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPayment.Focus();
                    return;
                }

                // Add payment information to DataGridView2
                DataGridView2.Rows.Add(cmbPaymentMode.Text, txtPayment.Text, dtpPaymentDate.Value.Date);

                // Calculate and round the total payment
                double totalPayment = TotalPayment();
                totalPayment = Math.Round(totalPayment, 2);
                txtTotalPayment.Text = totalPayment.ToString();

                // Perform additional computations
                Compute1();

                // Clear any additional data or reset fields if needed
                Clear1();
            }
            catch (Exception ex)
            {
                // Show detailed error message
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnListReset1_Click_1(object sender, EventArgs e)
        {
            Clear1();
        }

        private void btnListUpdate1_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("الرجاء اضافة اصناف ", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cmbPaymentMode.Text == "")
                {
                    MessageBox.Show("الرجاء اختيار طريقة دفع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbPaymentMode.Focus();
                    return;
                }
                if (txtPayment.Text == "")
                {
                    MessageBox.Show("الرجاء كتابة المبلغ المدفوع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPayment.Focus();
                    return;
                }
                foreach (DataGridViewRow row in DataGridView2.SelectedRows)
                    DataGridView2.Rows.Remove(row);
                DataGridView2.Rows.Add(cmbPaymentMode.Text, Convert.ToDecimal(txtPayment.Text), dtpPaymentDate.Value.Date);
                double j = 0d;
                j = TotalPayment();
                j = Math.Round(j, 2);
                txtTotalPayment.Text = j.ToString();
                Compute1();
                Clear1();
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
            }
        }

        private void btnRemove1_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Remove selected rows from DataGridView2
                foreach (DataGridViewRow row in DataGridView2.SelectedRows)
                {
                    // Ensure the row is not a new row before attempting to remove
                    if (!row.IsNewRow)
                    {
                        DataGridView2.Rows.Remove(row);
                    }
                }

                // Calculate and round the total payment
                double totalPayment = TotalPayment();
                totalPayment = Math.Round(totalPayment, 2);
                txtTotalPayment.Text = totalPayment.ToString();

                // Perform additional computations
                Compute1();
                Compute();

                // Clear any additional data or reset fields if needed
                Clear1();
            }
            catch (Exception ex)
            {
                // Show detailed error message
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void txtTotalPayment_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtQty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnAdd_Click(this, EventArgs.Empty);
            }

        }

        private void txtBarcode_TextChanged(object sender, EventArgs e)
        {

            try
            {
                // Make the DataGridView visible
                dgw.Visible = true;

                // Establish the SQL connection
                con.Open();

                // Define the SQL query with parameterized barcode search
                SqlCommand cmd = new SqlCommand(@"SELECT PID, RTRIM(Product.ProductCode), RTRIM(ProductName), 
                                          RTRIM(Temp_Stock.Barcode), CostPrice, SellingPrice, Discount, VAT, Qty 
                                          FROM Temp_Stock, Product 
                                          WHERE Product.PID = Temp_Stock.ProductID AND Qty > 0 
                                          AND Temp_Stock.Barcode LIKE @barcode 
                                          ORDER BY ProductCode", con);

                // Add the parameter for the barcode search
                cmd.Parameters.AddWithValue("@barcode", "%" + txtBarcode.Text + "%");

                // Execute the query and read the results
                SqlDataReader rdr = cmd.ExecuteReader();

                // Clear existing rows in the DataGridView
                dgw.Rows.Clear();

                // Populate the DataGridView with the results from the query
                while (rdr.Read())
                {
                    dgw.Rows.Add(rdr["PID"], rdr["ProductCode"], rdr["ProductName"], rdr["Barcode"],
                                 rdr["CostPrice"], rdr["SellingPrice"], rdr["Discount"], rdr["VAT"], rdr["Qty"]);
                }
            }
            catch (Exception ex)
            {
                // Display error message if an exception occurs
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Ensure the SqlDataReader and SqlConnection are closed properly
                if (con != null && con.State == ConnectionState.Open) con.Close();
            }

            // Update the payment text and other controls based on the selected payment mode
            if (cmbPaymentMode.SelectedIndex == 2 || cmbPaymentMode.SelectedIndex == 3 ||
                cmbPaymentMode.SelectedIndex == 4 || cmbPaymentMode.SelectedIndex == 5 ||
                cmbPaymentMode.SelectedIndex == 6 || cmbPaymentMode.SelectedIndex == 7 ||
                cmbPaymentMode.SelectedIndex == 8 || cmbPaymentMode.SelectedIndex == 9)
            {
                /*                txtPayment.ReadOnly = true;
                */
                txtPayment.Text = "0";
                txtQty.Focus();
            }
            else
            {
                /*                txtPayment.ReadOnly = false;
                */
                txtPayment.Text = Convert.ToString(Convert.ToDecimal(txtGrandTotal.Text));
            }
        }

        private void cmbPaymentMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPaymentMode.SelectedIndex == 2 || cmbPaymentMode.SelectedIndex == 3 ||
                    cmbPaymentMode.SelectedIndex == 4 || cmbPaymentMode.SelectedIndex == 5 ||
                    cmbPaymentMode.SelectedIndex == 6 || cmbPaymentMode.SelectedIndex == 7 ||
                    cmbPaymentMode.SelectedIndex == 8 || cmbPaymentMode.SelectedIndex == 9)
            {
                txtPayment.Text = "0";
                /*                txtPayment.ReadOnly = true;
                */
                if (txtCustomerID.Text == "C-0001")
                {
                    txtPayment.Text = Convert.ToString(Convert.ToDecimal(txtGrandTotal.Text));
                    /*                   
                    */                    /*                    txtPayment.ReadOnly = true;
                                        */
                }
            }
            else
            {
                /*                txtPayment.ReadOnly = false;
                */
                /*                txtPayment.Text = txtGrandTotal.Text;
                */
            }

            if (cmbPaymentMode.SelectedIndex == 4)
            {
                txtPayment.Text = "0.00";

            }

        }

        private void txtGrandTotal_TextChanged(object sender, EventArgs e)
        {

        }

        private void ToolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm:ss tt");
        }

        private void Timer1_Tick_1(object sender, EventArgs e)
        {

            Cursor = Cursors.Default;
            Timer1.Enabled = false;
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            txtCustomerType.Text = "Non Regular";
            Print();
            Reset();
        }


        private void DataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void DataGridView1_ControlAdded(object sender, ControlEventArgs e)
        {
            txtPayment.Text = (txtGrandTotal.Text);

            // List of payment mode indices that require a specific condition
            int[] specialPaymentModes = { 2, 3, 4, 5, 6, 7, 8, 9 };

            if (specialPaymentModes.Contains(cmbPaymentMode.SelectedIndex))
            {
                txtPayment.Text = "0";
                /*                txtPayment.ReadOnly = true;
                */
            }
            else
            {
                /*                txtPayment.ReadOnly = false;
                */
                txtPayment.Text = (txtGrandTotal.Text);
            }
        }

        private decimal Val(string text)
        {
            // Convert the text to a decimal, handle cases where conversion might fail
            if (decimal.TryParse(text, out decimal result))
            {
                return result;
            }
            return 0;
        }

        private void txtSalesman_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtAmount_TextChanged(object sender, EventArgs e)
        {

        }

        private void DataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            if (DataGridView1.Rows.Count > 0)
            {
          

                btnAdd.Enabled = false;

                DataGridViewRow row = DataGridView1.SelectedRows[0];

                txtProductCode.Text = row.Cells[0].Value.ToString();
                txtProductName.Text = row.Cells[1].Value.ToString();
                txtBarcode.Text = row.Cells[2].Value.ToString();
                txtCostPrice.Text = row.Cells[3].Value.ToString();
                txtSellingPrice.Text = row.Cells[4].Value.ToString();
                txtMargin.Text = row.Cells[5].Value.ToString();
                txtQty.Text = row.Cells[6].Value.ToString();
                txtAmount.Text = row.Cells[7].Value.ToString();
                txtDiscountPer.Text = row.Cells[8].Value.ToString();
                txtDiscountAmount.Text = row.Cells[9].Value.ToString();
                txtVAT.Text = row.Cells[10].Value.ToString();
                txtVATAmount.Text = row.Cells[11].Value.ToString();
                txtTotalAmount.Text = row.Cells[12].Value.ToString();
                txtProductID.Text = row.Cells[13].Value.ToString();
                txtProductName.Enabled = false;
                txtQty.Focus();
                dgw.Visible = false;
                btnRemove.Enabled = true;
                btnListUpdate.Enabled = true;
            }
            txtPayment.Text = (txtGrandTotal.Text.ToString());

        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void DataGridView1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void GroupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void btnPrint_Click_1(object sender, EventArgs e)
        {
            Print();
        }
        public void Print()
        {
            try
            {
                if (txtCustomerType.Text != "Non Regular")
                {
                    Cursor = Cursors.WaitCursor;
                    Timer1.Enabled = true;
                    rptInvoice rpt = new rptInvoice(); // The report you created.
                    SqlConnection myConnection = new SqlConnection(DataAccessLayer.Con());
                    SqlCommand MyCommand = new SqlCommand();
                    SqlCommand MyCommand1 = new SqlCommand();
                    SqlDataAdapter myDA = new SqlDataAdapter();
                    SqlDataAdapter myDA1 = new SqlDataAdapter();
                    DataSet myDS = new DataSet(); // The DataSet you created.
                    MyCommand.Connection = myConnection;
                    MyCommand1.Connection = myConnection;

                    MyCommand.CommandText = "Select Customer.ID, Customer.Name, Customer.Gender, Customer.Address, Customer.City, Customer.State, Customer.ZipCode, Customer.ContactNo, Customer.EmailID, InvoiceInfo.Remarks, Customer.Photo, InvoiceInfo.Inv_ID, InvoiceInfo.InvoiceNo, InvoiceInfo.InvoiceDate, InvoiceInfo.CustomerID, InvoiceInfo.GrandTotal, InvoiceInfo.TotalPaid, InvoiceInfo.Balance, Invoice_Product.IPo_ID, Invoice_Product.InvoiceID, Invoice_Product.ProductID, Invoice_Product.CostPrice, Invoice_Product.SellingPrice, Invoice_Product.Margin, Invoice_Product.Qty, Invoice_Product.Amount, Invoice_Product.DiscountPer, Invoice_Product.Discount, Invoice_Product.VATPer, Invoice_Product.VAT, Invoice_Product.TotalAmount, Invoice_Product.Barcode, Product.PID, Product.ProductCode, Product.ProductName FROM Customer INNER JOIN InvoiceInfo On Customer.ID = InvoiceInfo.CustomerID INNER JOIN Invoice_Product On InvoiceInfo.Inv_ID = Invoice_Product.InvoiceID INNER JOIN Product On Invoice_Product.ProductID = Product.PID where InvoiceInfo.Invoiceno=@d1";
                    MyCommand.Parameters.AddWithValue("@d1", txtInvoiceNo.Text);

                    MyCommand1.CommandText = "Select * from Company";
                    MyCommand.CommandType = CommandType.Text;
                    MyCommand1.CommandType = CommandType.Text;

                    myDA.SelectCommand = MyCommand;
                    myDA1.SelectCommand = MyCommand1;

                    myDA.Fill(myDS, "InvoiceInfo");
                    myDA.Fill(myDS, "Invoice_Product");
                    myDA.Fill(myDS, "Customer");
                    myDA.Fill(myDS, "Product");
                    myDA1.Fill(myDS, "Company");

                    rpt.SetDataSource(myDS);
                    rpt.SetParameterValue("p1", txtCustomerID.Text);
                    rpt.SetParameterValue("p2", DateTime.Today);
                    frmReport frmReport = new frmReport();

                    frmReport.crystalReportViewer1.ReportSource = rpt;
                    frmReport.ShowDialog();
                }
                else if (txtCustomerType.Text == "Non Regular")
                {
                    Cursor = Cursors.WaitCursor;
                    Timer1.Enabled = true;
                    rptInvoice3 rpt = new rptInvoice3(); // The report you created.
                    SqlConnection myConnection = new SqlConnection(DataAccessLayer.Con());
                    SqlCommand MyCommand = new SqlCommand();
                    SqlCommand MyCommand1 = new SqlCommand();
                    SqlDataAdapter myDA = new SqlDataAdapter();
                    SqlDataAdapter myDA1 = new SqlDataAdapter();
                    DataSet myDS = new DataSet(); // The DataSet you created.
                    MyCommand.Connection = myConnection;
                    MyCommand1.Connection = myConnection;

                    MyCommand.CommandText = "Select Customer.ID, Customer.Name, Customer.Gender, Customer.Address, Customer.City, Customer.State, Customer.ZipCode, Customer.ContactNo, Customer.EmailID, InvoiceInfo.Remarks, Customer.Photo, InvoiceInfo.Inv_ID, InvoiceInfo.InvoiceNo, InvoiceInfo.InvoiceDate, InvoiceInfo.CustomerID, InvoiceInfo.GrandTotal, InvoiceInfo.TotalPaid, InvoiceInfo.Balance, Invoice_Product.IPo_ID, Invoice_Product.InvoiceID, Invoice_Product.ProductID, Invoice_Product.CostPrice, Invoice_Product.SellingPrice, Invoice_Product.Margin, Invoice_Product.Qty, Invoice_Product.Amount, Invoice_Product.DiscountPer, Invoice_Product.Discount, Invoice_Product.VATPer, Invoice_Product.VAT, Invoice_Product.TotalAmount, Invoice_Product.Barcode, Product.PID, Product.ProductCode, Product.ProductName FROM Customer INNER JOIN InvoiceInfo On Customer.ID = InvoiceInfo.CustomerID INNER JOIN Invoice_Product On InvoiceInfo.Inv_ID = Invoice_Product.InvoiceID INNER JOIN Product On Invoice_Product.ProductID = Product.PID where InvoiceInfo.Invoiceno=@d1";
                    MyCommand.Parameters.AddWithValue("@d1", txtInvoiceNo.Text);

                    MyCommand1.CommandText = "Select * from Company";
                    MyCommand.CommandType = CommandType.Text;
                    MyCommand1.CommandType = CommandType.Text;

                    myDA.SelectCommand = MyCommand;
                    myDA1.SelectCommand = MyCommand1;

                    myDA.Fill(myDS, "InvoiceInfo");
                    myDA.Fill(myDS, "Invoice_Product");
                    myDA.Fill(myDS, "Customer");
                    myDA.Fill(myDS, "Product");
                    myDA1.Fill(myDS, "Company");

                    rpt.SetDataSource(myDS);
                    rpt.SetParameterValue("p1", txtCustomerID.Text);
                    rpt.SetParameterValue("p2", DateTime.Today);
                    frmReport frmReport = new frmReport();

                    frmReport.crystalReportViewer1.ReportSource = rpt;
                    frmReport.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtProductCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSalesmanID_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCustomerID_TextChanged(object sender, EventArgs e)
        {
            if (txtCustomerID.Text == "C-0001")
            {
                cmbPaymentMode.SelectedIndex = 0;
                txtPayment.Text = Convert.ToString(Val(txtGrandTotal.Text));
                /*                txtPayment.ReadOnly = true;
                */
            }
            else
            {
                txtPayment.Text = Convert.ToString(Val(txtGrandTotal.Text));
                /*                txtPayment.ReadOnly = false;
                */
            }

        }

        private void dgw_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (DataGridView1.Rows.Count > 0)
            {
                if (lblSet.Text == "Not Allowed")
                {
                    btnRemove.Enabled = true;
                    btnListUpdate.Enabled = true;
                }
                else
                {
                    btnRemove.Enabled = true;
                    btnListUpdate.Enabled = true;
                }

                btnAdd.Enabled = false;

                DataGridViewRow row = DataGridView1.SelectedRows[0];

                txtProductCode.Text = row.Cells[0].Value.ToString();
                txtProductName.Text = row.Cells[1].Value.ToString();
                txtBarcode.Text = row.Cells[2].Value.ToString();
                txtCostPrice.Text = row.Cells[3].Value.ToString();
                txtSellingPrice.Text = row.Cells[4].Value.ToString();
                txtMargin.Text = row.Cells[5].Value.ToString();
                txtQty.Text = row.Cells[6].Value.ToString();
                txtAmount.Text = row.Cells[7].Value.ToString();
                txtDiscountPer.Text = row.Cells[8].Value.ToString();
                txtDiscountAmount.Text = row.Cells[9].Value.ToString();
                txtVAT.Text = row.Cells[10].Value.ToString();
                txtVATAmount.Text = row.Cells[11].Value.ToString();
                txtTotalAmount.Text = row.Cells[12].Value.ToString();
                txtProductID.Text = row.Cells[13].Value.ToString();

                txtQty.Focus();
                dgw.Visible = false;
            }

        }

        private void dgw_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (dgw.Rows.Count > 0)
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];
                    txtProductID.Text = dr.Cells[0].Value.ToString();
                    txtProductCode.Text = dr.Cells[1].Value.ToString();
                    txtProductName.Text = dr.Cells[2].Value.ToString();
                    txtBarcode.Text = dr.Cells[3].Value.ToString();
                    txtCostPrice.Text = dr.Cells[4].Value.ToString();

                    if (ComboBox1.SelectedIndex == 0)
                    {
                        txtSellingPrice.Text = dr.Cells[5].Value.ToString();
                    }
                    else
                    {
                        txtSellingPrice.Text = dr.Cells[9].Value.ToString();
                    }

                    txtVAT.Text = dr.Cells[7].Value.ToString();

                    double num = Convert.ToDouble(dr.Cells[5].Value) - Convert.ToDouble(dr.Cells[4].Value);
                    num = Math.Round(num, 2);
                    txtMargin.Text = num.ToString();

                    txtDiscountPer.Text = dr.Cells[6].Value.ToString();
                    txtVAT.Text = dr.Cells[7].Value.ToString();

                    txtQty.Focus();

                    lblSet.Text = "";
                    dgw.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT RTRIM(ID),RTRIM(CustomerID),RTRIM([Name]),RTRIM(Gender), RTRIM(Address),RTRIM(City),RTRIM(State),RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID),RTRIM(Remarks),Photo from Customer where CustomerType='Regular' order by ID", con);
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

        private void txtProductName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtProductName_TextChanged_1(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtProductName.Text))
                {
                    dgw.Visible = true;
                    // txtDiscountPer.Enabled = true;
                    // txtDiscountAmount.Enabled = true;
                    string productName = txtProductName.Text.Trim();
                    string query = @"SELECT PID, RTRIM(Product.ProductCode), RTRIM(ProductName), RTRIM(Temp_Stock.Barcode), 
                        CostPrice, SellingPrice, Discount, VAT, Qty, RTRIM(Product.SellingPrice2) 
                        FROM Temp_Stock, Product 
                        WHERE Product.PID = Temp_Stock.ProductID 
                        AND Qty > 0 
                        AND ProductName LIKE @ProductName 
                        ORDER BY ProductCode";

                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@ProductName", "%" + productName + "%");
                            using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                            {
                                dgw.Rows.Clear();
                                while (rdr.Read())
                                {
                                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9]);
                                }
                            }
                        }
                    }


                }
                else
                {
                    dgw.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GroupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void txtBarcode_TextChanged_1(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBarcode.Text))
            {
                // Make the DataGridView visible
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    dgw.Visible = true;
                    // Establish the SQL connection
                    con.Open();

                    // Define the SQL query with an exact match for the barcode
                    SqlCommand cmd = new SqlCommand(@"SELECT PID, RTRIM(Product.ProductCode), RTRIM(ProductName), 
                                          RTRIM(Temp_Stock.Barcode), CostPrice, SellingPrice, Discount, VAT, Qty 
                                          FROM Temp_Stock, Product 
                                          WHERE Product.PID = Temp_Stock.ProductID 
                                          AND Qty > 0 
                                          AND Temp_Stock.Barcode = @barcode 
                                          ORDER BY ProductCode", con);

                    // Add the parameter for the barcode search
                    cmd.Parameters.AddWithValue("@barcode", txtBarcode.Text.Trim());

                    // Execute the query and read the results

                    // Clear existing rows in the DataGridView
                    dgw.Rows.Clear();

                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                // Since there are 9 columns, ensure you only access indices 0-8
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8]);
                            }
                            if (dgw.Rows.Count > 0)
                            {
                                DataGridViewRow dr = dgw.SelectedRows[0];
                                txtProductID.Text = dr.Cells[0].Value.ToString();
                                txtProductCode.Text = dr.Cells[1].Value.ToString();
                                txtProductName.Text = dr.Cells[2].Value.ToString();
                                txtBarcode.Text = dr.Cells[3].Value.ToString();
                                txtCostPrice.Text = dr.Cells[4].Value.ToString();

                                if (ComboBox1.SelectedIndex == 0)
                                {
                                    txtSellingPrice.Text = dr.Cells[5].Value.ToString();
                                }
                                else
                                {
                                    txtSellingPrice.Text = dr.Cells[9].Value.ToString();
                                }

                                txtVAT.Text = dr.Cells[7].Value.ToString();

                                double num = Convert.ToDouble(dr.Cells[5].Value) - Convert.ToDouble(dr.Cells[4].Value);
                                num = Math.Round(num, 2);
                                txtMargin.Text = num.ToString();

                                txtDiscountPer.Text = dr.Cells[6].Value.ToString();
                                txtVAT.Text = dr.Cells[7].Value.ToString();
                                txtPayment.Text = txtTotalAmount.Text.ToString();
                                txtQty.Text = "1";

                                lblSet.Text = "";
                                dgw.Visible = false;
                            }
                        }
                        else
                        {
                            // If no rows are found, optionally notify the user or take another action
                            dgw.Visible = false; // Optionally hide the DataGridView if no matches are found
                        }
                    }
                }
            }
            else
            {
                dgw.Visible = false;
            }
        }

        private void txtPaymentDue_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            txtCustomerType.Text = "Regular";
            Print();
            Reset();    
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Products p = new Products();
            p.Reset();
            p.Show();
        }

        private void txtSellingPrice_TextChanged_1(object sender, EventArgs e)
        {
            Compute();
        }

        private void dataGridView3_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void DataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void DataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            string strRowNumber = (e.RowIndex + 1).ToString();
            SizeF size = e.Graphics.MeasureString(strRowNumber, this.Font);
            if (DataGridView1.RowHeadersWidth < Convert.ToInt32(size.Width + 20))
            {
                DataGridView1.RowHeadersWidth = Convert.ToInt32(size.Width + 20);
            }
            Brush b = SystemBrushes.Window; // .ControlText
            e.Graphics.DrawString(strRowNumber, this.Font, b, e.RowBounds.Location.X + DataGridView1.Width - 25, e.RowBounds.Location.Y + (e.RowBounds.Height - size.Height) / 2);

        }

        private void DataGridView1_ControlAdded_1(object sender, ControlEventArgs e)
        {
            txtPayment.Text = (txtGrandTotal.Text.ToString());
            if (cmbPaymentMode.SelectedIndex == 2 | cmbPaymentMode.SelectedIndex == 3 | cmbPaymentMode.SelectedIndex == 2 | cmbPaymentMode.SelectedIndex == 4 | cmbPaymentMode.SelectedIndex == 5 | cmbPaymentMode.SelectedIndex == 6 | cmbPaymentMode.SelectedIndex == 7 | cmbPaymentMode.SelectedIndex == 8 | cmbPaymentMode.SelectedIndex == 9)
            {
                txtPayment.Text = "0";

            }
            else
            {
                txtPayment.Text = (txtGrandTotal.Text.ToString());
            }
        }

        private void DataGridView1_CellContentClick_2(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgw_CellContentClick_2(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            string cs = DataAccessLayer.Con();
            try
            {
                // Validation checks
                if (string.IsNullOrWhiteSpace(txtSalesmanID.Text))
                {
                    MessageBox.Show("الرجاء تحديد رقم البائع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
                {
                    MessageBox.Show("الرجاء تحديد العميل", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (DataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("الرجاء اضافة اصناف للشبكة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (DataGridView2.Rows.Count == 0)
                {
                    DataGridView2.Rows.Add(cmbPaymentMode.Text, txtPayment.Text, dtpPaymentDate.Value.Date);
                    txtTotalPayment.Text = Math.Round(TotalPayment(), 2).ToString();
                    Compute1();
                }

                if (double.Parse(txtTotalPayment.Text) > double.Parse(txtGrandTotal.Text))
                {
                    MessageBox.Show("المبلغ المدفوع لا يكون اكبر من قيمة الفاتورة", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Company profile check
                using (var con = new SqlConnection(cs))
                {
                    con.Open();
                    using (var cmd = new SqlCommand("SELECT * FROM Company", con))
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (!rdr.Read())
                        {
                            MessageBox.Show("الرجاء اضافة ملف تعريفي للشركة في القائمة الرئيسية", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }

                    // Product quantity check
                    foreach (DataGridViewRow row in DataGridView1.Rows)
                    {
                        using (var cmd = new SqlCommand("SELECT Qty FROM Temp_Stock WHERE ProductID=@d1", con))
                        {
                            cmd.Parameters.AddWithValue("@d1", row.Cells[13].Value);
                            cmd.Parameters.AddWithValue("@d2", row.Cells[2].Value);
                            using (var da = new SqlDataAdapter(cmd))
                            {
                                var ds = new DataSet();
                                da.Fill(ds);

                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    txtTotalQty.Text = ds.Tables[0].Rows[0]["Qty"].ToString();
                                    if (int.Parse(row.Cells[6].Value.ToString()) > int.Parse(txtTotalQty.Text))
                                    {
                                        MessageBox.Show("الكمية المضافة. إلى سلة البيع أكثر من" + Environment.NewLine +
                                                        "الكمية المتوفرة. من المنتج '" + row.Cells[0].Value + "' واسم المنتج ='" +
                                                        row.Cells[1].Value + "' وجود الباركود ='" + row.Cells[2].Value + "'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }
                                }
                            }
                        }
                    }

                    // Insert Customer
                    if (!txtCustomerName.ReadOnly)
                    {
                        auto1();
                        using (var cmd = new SqlCommand("INSERT INTO Customer(ID, CustomerID, [Name], Gender, Address, City, ContactNo, EmailID, Remarks, State, ZipCode, Photo, CustomerType) " +
                                                        "VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9, @d10, @d11, @d12, 'Non Regular')", con))
                        {
                            cmd.Parameters.AddWithValue("@d1", txtCID.Text);
                            cmd.Parameters.AddWithValue("@d2", txtCustomerID.Text);
                            cmd.Parameters.AddWithValue("@d3", txtCustomerName.Text);
                            cmd.Parameters.AddWithValue("@d4", string.Empty);
                            cmd.Parameters.AddWithValue("@d5", string.Empty);
                            cmd.Parameters.AddWithValue("@d6", string.Empty);
                            cmd.Parameters.AddWithValue("@d7", txtContactNo.Text);
                            cmd.Parameters.AddWithValue("@d8", string.Empty);
                            cmd.Parameters.AddWithValue("@d9", string.Empty);
                            cmd.Parameters.AddWithValue("@d10", string.Empty);
                            cmd.Parameters.AddWithValue("@d11", string.Empty);

                            using (var ms = new MemoryStream())
                            {
                                var bmpImage = new Bitmap(Properties.Resources.if_icons_user1);
                                bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                cmd.Parameters.AddWithValue("@d12", ms.ToArray());
                            }

                            cmd.ExecuteNonQuery();
                            txtCustomerType.Text = "Non Regular";
                        }
                    }

                    // Insert InvoiceInfo
                    using (var cmd = new SqlCommand("INSERT INTO InvoiceInfo(Inv_ID, InvoiceNo, InvoiceDate, CustomerID, GrandTotal, TotalPaid, Balance, Remarks, SalesmanID) " +
                                                    "VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9)", con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtID.Text);
                        cmd.Parameters.AddWithValue("@d2", txtInvoiceNo.Text);
                        cmd.Parameters.AddWithValue("@d3", dtpInvoiceDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d4", txtCID.Text);
                        cmd.Parameters.AddWithValue("@d5", txtGrandTotal.Text);
                        cmd.Parameters.AddWithValue("@d6", txtTotalPayment.Text);
                        cmd.Parameters.AddWithValue("@d7", txtPaymentDue.Text);
                        cmd.Parameters.AddWithValue("@d8", txtRemarks.Text);
                        cmd.Parameters.AddWithValue("@d9", txtSM_ID.Text);
                        cmd.ExecuteNonQuery();
                    }

                    // Insert Salesman_Commission
                    decimal sum = DataGridView1.Rows.Cast<DataGridViewRow>().Sum(r => decimal.Parse(r.Cells[12].Value.ToString()));
                    decimal commission = sum * decimal.Parse(txtCommissionPer.Text) / 100;

                    using (var cmd = new SqlCommand("INSERT INTO Salesman_Commission(InvoiceID, CommissionPer, Commission) VALUES (@T1, @T2, @T3)", con))
                    {
                        cmd.Parameters.AddWithValue("@T1", txtID.Text);
                        cmd.Parameters.AddWithValue("@T2", txtCommissionPer.Text);
                        cmd.Parameters.AddWithValue("@T3", commission);
                        cmd.ExecuteNonQuery();
                    }

                    // Insert Invoice_Product
                    using (var cmd = new SqlCommand("INSERT INTO Invoice_Product(InvoiceID, Barcode, CostPrice, SellingPrice, Margin, Qty, Amount, DiscountPer, Discount, VATPer, VAT, TotalAmount, ProductID) " +
                                                    "VALUES (@d1, @d4, @d5, @d6, @d7, @d8, @d9, @d10, @d11, @d12, @d13, @d14, @d15)", con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtID.Text);
                        foreach (DataGridViewRow row in DataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                cmd.Parameters.AddWithValue("@d4", row.Cells[2].Value);
                                cmd.Parameters.AddWithValue("@d5", row.Cells[3].Value);
                                cmd.Parameters.AddWithValue("@d6", row.Cells[4].Value);
                                cmd.Parameters.AddWithValue("@d7", row.Cells[5].Value);
                                cmd.Parameters.AddWithValue("@d8", row.Cells[6].Value);
                                cmd.Parameters.AddWithValue("@d9", row.Cells[7].Value);
                                cmd.Parameters.AddWithValue("@d10", row.Cells[8].Value);
                                cmd.Parameters.AddWithValue("@d11", row.Cells[9].Value);
                                cmd.Parameters.AddWithValue("@d12", row.Cells[10].Value);
                                cmd.Parameters.AddWithValue("@d13", row.Cells[11].Value);
                                cmd.Parameters.AddWithValue("@d14", row.Cells[12].Value);
                                cmd.Parameters.AddWithValue("@d15", row.Cells[13].Value);
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@d1", txtID.Text); // Re-add txtID.Text after clearing parameters
                            }
                        }
                    }

                    // Insert Invoice_Payment
                    using (var cmd = new SqlCommand("INSERT INTO Invoice_Payment(InvoiceID, PaymentMode, TotalPaid, PaymentDate) VALUES (@d1, @d4, @d5, @d6)", con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtID.Text);
                        foreach (DataGridViewRow row in DataGridView2.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                cmd.Parameters.AddWithValue("@d4", row.Cells[0].Value);
                                cmd.Parameters.AddWithValue("@d5", row.Cells[1].Value);
                                cmd.Parameters.AddWithValue("@d6", row.Cells[2].Value);
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@d1", txtID.Text); // Re-add txtID.Text after clearing parameters
                            }
                        }
                    }

                    // Update Temp_Stock
                    foreach (DataGridViewRow row in DataGridView1.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            using (var cmd = new SqlCommand("UPDATE Temp_stock SET Qty = Qty - @d3 WHERE ProductID=@d1", con))
                            {

                                cmd.Parameters.AddWithValue("@d1", row.Cells[13].Value);
                                cmd.Parameters.AddWithValue("@d2", row.Cells[2].Value);
                                cmd.Parameters.AddWithValue("@d3", row.Cells[6].Value);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                // Final confirmation message
                MessageBox.Show("تم الحفظ بنجاح", "الفاتورة", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
                btnPrint.Enabled = true;
                txtCustomerType.Text = "Non Regular";
                Print();
                Reset();
            }
            catch (Exception ex)
            {

            }
        }
    }
}

