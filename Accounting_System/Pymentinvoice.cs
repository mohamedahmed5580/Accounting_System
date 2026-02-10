using DevExpress.Xpo.DB.Helpers;
using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Accounting_System.Pymentinvoice;
using static DevExpress.Data.Helpers.ExpressiveSortInfo;
using Point = System.Drawing.Point;

namespace Accounting_System
{
    public partial class Pymentinvoice : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        private static Pymentinvoice _instance;
        public static Pymentinvoice instance;
        public bool isSupplierSelecting = true;

        public static Pymentinvoice Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new Pymentinvoice();
                }
                return _instance;
            }
        }   
        public Pymentinvoice()
        {
            InitializeComponent();
            LoadCurrenciesToComboBox();
            instance = this;
        }

        private void Pymentinvoice_Load(object sender, EventArgs e)
        {
            Reset();
            fillSupplers();
            fillSupplersPhone();
        }


        private void LoadSupplierNamesAutoComplete()
        {
            // Create a collection to hold supplier names (with IDs)
            AutoCompleteStringCollection supplierNames = new AutoCompleteStringCollection();

            // Retrieve supplier names and IDs from the database
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                string query = "SELECT RTRIM([Name]) FROM Supplier ORDER BY [Name]";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            // Combine ID and Name into one string, e.g., "123 - SupplierName"
                            supplierNames.Add(rdr[0].ToString());
                        }
                    }
                }
            }

            // Configure the TextBox to use AutoComplete
            txtSupplierName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtSupplierName.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtSupplierName.AutoCompleteCustomSource = supplierNames;
        }


        public static class Conversion
            {
                public static string str { get; set; }
            }
   
        
        private string GenerateID()
        {
            string value = "0000";
            try
            {
                // Fetch the latest ID from the database
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 ST_ID FROM Stock ORDER BY ST_ID DESC", con);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (rdr.HasRows)
                {
                    rdr.Read();
                    value = rdr["ST_ID"].ToString();
                }
                rdr.Close();

                // Increase the ID by 1
                int numericValue = int.Parse(value);
                numericValue++;
                value = numericValue.ToString("D4"); // Format as a 4-digit number with leading zeros
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                Console.WriteLine("An error occurred: " + ex.Message);
                value = "0000";
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
        public void auto()
        {
            try
            {
                txtST_ID.Text = GenerateID();
                txtInvoiceNo.Text = "ST-" + GenerateID();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void Compute()
        {
            double num6, num7, num8, num1, num2, num3, num4, num5;
            num6 = Val(txtSubTotal.Text) * Val(txtDiscPer.Text) / 100;
            num6 = Math.Round(num6, 2);
            txtDisc.Text = num6.ToString();
            num7 = Val(txtSubTotal.Text) - num6;
            num8 = num7 * Val(txtVATPer.Text) / 100;
            num8 = Math.Round(num8, 2);
            txtVATAmt.Text = num8.ToString();
            num1 = num7 + Val(txtFreightCharges.Text) + Val(txtOtherCharges.Text) + Val(txtVATAmt.Text);
            num1 = Math.Round(num1, 2);
            txtTotal.Text = num1.ToString();
            num2 = Math.Round(num1, 1);
            num3 = num2 - num1;
            num3 = Math.Round(num3, 2);
            txtRoundOff.Text = num3.ToString();
            num4 = Val(txtTotal.Text) ;
            num4 = Math.Round(num4, 2);
            txtGrandTotal.Text = num4.ToString();
            num5 = Val(txtGrandTotal.Text) - Val(txtTotalPaid.Text);
            num5 = Math.Round(num5, 2);
            txtBalance.Text = num5.ToString();
        }


        private double Val(string text)
        {
            double.TryParse(text, out double result);
            return result;
        }


       



        public void GetSupplierBalance()
        {
            try
            {
                decimal num1 = 0;
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string sql = "SELECT ISNULL(SUM(Credit), 0) - ISNULL(SUM(Debit), 0)  FROM SupplierLedgerBook WHERE PartyID = @d1 GROUP BY PartyID";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtSupplierID.Text);
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.Read())
                            {
                                num1 = Convert.ToDecimal(rdr.GetValue(0));
                            }
                        }
                    }
                }
                lblBalance.Text = num1.ToString();
                lblBalanceCC.Text = num1.ToString();
                if (Convert.ToDecimal(lblBalance.Text) >= 0)
                {
                    Conversion.str = "دائن";
                }
                else
                {
                    Conversion.str = "مدين";
                }
                if (Conversion.str == "دائن")
                {
                    txtPreviousDue.Text = (-num1).ToString();
                    lblBalance.Text = Math.Abs(Convert.ToDecimal(lblBalance.Text)).ToString();
                    lblBalanceByC.Text = (Math.Abs(Convert.ToDecimal(lblBalanceCC.Text)) / Convert.ToDecimal(Cprice.Text)).ToString("F2");
                    lblBalance.Text = lblBalance.Text + " " + Conversion.str ;
                }
                else
                {
                    txtPreviousDue.Text = Math.Abs(num1).ToString();
                    lblBalance.Text = Math.Abs(Convert.ToDecimal(lblBalance.Text)).ToString();
                    lblBalanceByC.Text = (Math.Abs(Convert.ToDecimal(lblBalanceCC.Text)) / Convert.ToDecimal(Cprice.Text)).ToString("F2");
                    lblBalance.Text = lblBalance.Text + " " + Conversion.str;
                }
                Compute();
            auto();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void GetSupplierInfo()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string sql = "SELECT SupplierID, Name, Address, City, ContactNo FROM Supplier WHERE ID = @d1";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtSup_ID.Text));
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.Read())
                            {
                                txtSupplierID.Text = rdr.GetValue(0).ToString();
                                txtSupplierName.Text = rdr.GetValue(1).ToString();
                                txtAddress.Text = rdr.GetValue(2).ToString();
                                txtCity.Text = rdr.GetValue(3).ToString();
                                txtContactNo.Text = rdr.GetValue(4).ToString();
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
        public void GetSupplierBalance1()
        {
            try
            {
                decimal num1 = 0;
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string sql = "SELECT ISNULL(SUM(Credit), 0) - ISNULL(SUM(Debit), 0) FROM SupplierLedgerBook WHERE PartyID = @d1 GROUP BY PartyID";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtSupplierID.Text);
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.Read())
                            {
                                num1 = Convert.ToDecimal(rdr.GetValue(0));
                            }
                        }
                    }
                }
                lblBalance.Text = num1.ToString();
                if (Convert.ToDecimal(lblBalance.Text) >= 0)
                {
                    Conversion.str = "دائن";
                }
                else
                {
                    Conversion.str = "مدين";
                }
                lblBalance.Text = Math.Abs(Convert.ToDecimal(lblBalance.Text)).ToString();
                lblBalance.Text = lblBalance.Text + " " + Conversion.str;
                Compute();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Print();
        }
        public void Print()
        {
            decimal a = 0, b = 0, c = 0;
            try
            {
                Cursor = Cursors.WaitCursor;
                Timer1.Enabled = true;

                var rpt = new rptPurchase(); // The report you created.
                DataSet myDS = new DataSet();
                DataSet myDS1 = new DataSet();

                using (SqlConnection myConnection = new SqlConnection(DataAccessLayer.Con()))
                {
                    using (SqlCommand MyCommand = new SqlCommand())
                    {
                        MyCommand.Connection = myConnection;
                        MyCommand.CommandText = @"
                    SELECT Distinct 
                        Stock.ST_ID, Stock.InvoiceNo, Stock.SupplierID, Stock.GrandTotal, Stock.TotalPayment, 
                        Stock.PaymentDue, Stock.Remarks, Stock_Product.SP_ID, Stock_Product.StockID, 
                        Stock_Product.ProductID, Stock_Product.Qty, Stock_Product.Price, 
                        Stock_Product.TotalAmount, Supplier.ID, Supplier.SupplierID AS Expr1, 
                        Supplier.Name, Supplier.Address, Supplier.City, Supplier.State, Supplier.ZipCode, 
                        Supplier.ContactNo, Supplier.EmailID, Supplier.Remarks AS Expr2, 
                        Product.PID, Product.ProductCode, Product.ProductName, Product.SubCategoryID, 
                        Product.Description, Product.CostPrice, Product.SellingPrice, Product.Discount, 
                        Product.VAT, Product.ReorderPoint 
                    FROM Stock 
                    INNER JOIN Stock_Product ON Stock.ST_ID = Stock_Product.StockID 
                    INNER JOIN Supplier ON Stock.SupplierID = Supplier.ID 
                    INNER JOIN Product ON Stock_Product.ProductID = Product.PID 
                    WHERE Stock.InvoiceNo = @d1";

                        MyCommand.Parameters.AddWithValue("@d1", txtInvoiceNo.Text);
                        MyCommand.CommandType = CommandType.Text;

                        using (SqlDataAdapter myDA = new SqlDataAdapter(MyCommand))
                        {
                            myDA.Fill(myDS, "Stock");
                            myDA.Fill(myDS, "Stock_Product");
                            myDA.Fill(myDS, "Product");
                            myDA.Fill(myDS, "Supplier");
                        }
                    }
                }

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string ct = @"
                SELECT 
                    ISNULL(SUM(GrandTotal), 0), 
                    ISNULL(SUM(TotalPayment), 0), 
                    ISNULL(SUM(PaymentDue), 0) 
                FROM Stock 
                INNER JOIN Supplier ON Supplier.ID = Stock.SupplierID 
                WHERE Stock.InvoiceNo = @d3";

                    using (SqlCommand cmd = new SqlCommand(ct, con))
                    {
                        cmd.Parameters.AddWithValue("@d3", txtInvoiceNo.Text);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                a = rdr.GetDecimal(0);
                                b = rdr.GetDecimal(1);
                                c = rdr.GetDecimal(2);
                            }
                        }
                    }
                }

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = @"
                SELECT 
                    CONVERT(varchar(10), YEAR(Date)) AS Year, 
                    SUM(GrandTotal) AS GrandTotal 
                FROM Stock 
                WHERE Stock.InvoiceNo = @d3";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@d3", txtInvoiceNo.Text);

                        using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                        {
                            System.Data.DataTable dtable = new System.Data.DataTable();

                            adp.Fill(dtable);
                            myDS1.Tables.Add(dtable);
                        }
                    }
                }

                myDS1.WriteXmlSchema("TotalPurchase.xml");
                rpt.Subreports[0].SetDataSource(myDS1);
                rpt.SetDataSource(myDS);

                rpt.SetParameterValue("p1", DateTime.Now);
                rpt.SetParameterValue("p2", DateTime.Now);
                rpt.SetParameterValue("p3", a);
                rpt.SetParameterValue("p4", b);
                rpt.SetParameterValue("p5", c);
                rpt.SetParameterValue("p6", DateTime.Today);
                frmReport frReport= new frmReport();
                frReport.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                Timer1.Enabled = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation checks
                if (!ValidateInputFields())
                    return;

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            if (InvoiceExists(con, transaction))
                                return;

                            decimal totalByPound = CalculateTotalByPound();

                            InsertStockRecord(con, transaction, totalByPound);
                            InsertStockProductRecords(con, transaction);
                            UpdateTempStock(con, transaction);
                            UpdateLedgers(con, transaction);

                            transaction.Commit();
                            MessageBox.Show("تم الحفظ بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            btnSave.Enabled = false;
                            Reset();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("حدث خطأ أثناء الحفظ", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Consider logging the exception here
            }
        }

        private bool ValidateInputFields()
        {
            if (string.IsNullOrWhiteSpace(txtSupplierID.Text))
                return ShowValidationError("الرجاء إدراج رقم المورد", txtSupplierID);

            if (DataGridView1.Rows.Count == 0)
                return ShowValidationError("يجب إضافة أصناف أولا");

            if (!ValidateDecimalField(txtDiscPer, "الرجاء كتابة الخصم %"))
                return false;

            if (!ValidateDecimalField(txtVATPer, "الرجاء كتابة مبلغ الضريبة %"))
                return false;

            if (!ValidateDecimalField(txtFreightCharges, "الرجاء كتابة مصاريف الشحن"))
                return false;

            if (!ValidateDecimalField(txtOtherCharges, "الرجاء كتابة المصروفات الأخرى"))
                return false;

            if (!ValidateDecimalField(txtRoundOff, "الرجاء استخدام التقريب"))
                return false;

            if (cmbPurchaseType.SelectedIndex == 0)
            {
                if (!ValidateDecimalField(txtTotalPaid, "الرجاء كتابة إجمالي المدفوع"))
                    return false;

                if (GetDecimalValue(txtTotalPaid) <= 0)
                    return ShowValidationError("المبلغ المدفوع يجب أن يكون أكبر من صفر", txtTotalPaid);
            }

            if (string.IsNullOrWhiteSpace(comboBox1.Text))
                return ShowValidationError("الرجاء اختيار العملة", comboBox1);

            return true;
        }

        private bool ValidateDecimalField(System.Windows.Forms.TextBox textBox, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text) || !decimal.TryParse(textBox.Text, out _))
                return ShowValidationError(errorMessage, textBox);
            return true;
        }

        private bool ShowValidationError(string message, Control control = null)
        {
            MessageBox.Show(message, "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            control?.Focus();
            return false;
        }

        private decimal GetDecimalValue(System.Windows.Forms.TextBox textBox)
        {
            return decimal.TryParse(textBox.Text, out decimal value) ? value : 0;
        }

        private bool InvoiceExists(SqlConnection con, SqlTransaction transaction)
        {
            string query = "SELECT 1 FROM Stock WHERE InvoiceNo = @InvoiceNo";
            using (SqlCommand cmd = new SqlCommand(query, con, transaction))
            {
                cmd.Parameters.AddWithValue("@InvoiceNo", txtInvoiceNo.Text);
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    MessageBox.Show("رقم الفاتورة موجود بالفعل", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtInvoiceNo.Clear();
                    txtInvoiceNo.Focus();
                    return true;
                }
            }
            return false;
        }

        private decimal CalculateTotalByPound()
        {
            if (cmbPurchaseType.SelectedIndex != 0)
                return 0;

            decimal currencyPrice = Convert.ToDecimal(Cprice.Text);
            decimal totalPaid = GetDecimalValue(txtTotalPaid);
            return currencyPrice * totalPaid;
        }

        private void InsertStockRecord(SqlConnection con, SqlTransaction transaction, decimal totalByPound)
        {
            string query = @"
        INSERT INTO Stock (
            ST_ID, InvoiceNo, Date, PurchaseType, SupplierID, SubTotal, 
            DiscountPer, Discount, PreviousDue, FreightCharges, OtherCharges, 
            Total, RoundOff, GrandTotal, TotalPayment, PaymentDue, Remarks, 
            VATPer, VATAmt, CurrencieName, Currency_price, Total_By_Pound, WID
        ) VALUES (
            @ST_ID, @InvoiceNo, @Date, @PurchaseType, @SupplierID, @SubTotal,
            @DiscountPer, @Discount, @PreviousDue, @FreightCharges, @OtherCharges,
            @Total, @RoundOff, @GrandTotal, @TotalPayment, @PaymentDue, @Remarks,
            @VATPer, @VATAmt, @CurrencieName, @Currency_price, @Total_By_Pound, @WID
        )";

            using (SqlCommand cmd = new SqlCommand(query, con, transaction))
            {
                AddCommonParameters(cmd);
                cmd.Parameters.AddWithValue("@Total_By_Pound", totalByPound);
                cmd.ExecuteNonQuery();
            }
        }

        private void AddCommonParameters(SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@ST_ID", txtST_ID.Text);
            cmd.Parameters.AddWithValue("@InvoiceNo", txtInvoiceNo.Text);
            cmd.Parameters.AddWithValue("@Date", dtpDate.Value.Date);
            cmd.Parameters.AddWithValue("@PurchaseType", cmbPurchaseType.Text);
            cmd.Parameters.AddWithValue("@SupplierID", txtSup_ID.Text);
            cmd.Parameters.AddWithValue("@SubTotal", GetDecimalValue(txtSubTotal));
            cmd.Parameters.AddWithValue("@DiscountPer", GetDecimalValue(txtDiscPer));
            cmd.Parameters.AddWithValue("@Discount", GetDecimalValue(txtDisc));
            cmd.Parameters.AddWithValue("@PreviousDue", GetDecimalValue(txtPreviousDue));
            cmd.Parameters.AddWithValue("@FreightCharges", GetDecimalValue(txtFreightCharges));
            cmd.Parameters.AddWithValue("@OtherCharges", GetDecimalValue(txtOtherCharges));
            cmd.Parameters.AddWithValue("@Total", GetDecimalValue(txtTotal));
            cmd.Parameters.AddWithValue("@RoundOff", GetDecimalValue(txtRoundOff));
            cmd.Parameters.AddWithValue("@GrandTotal", GetDecimalValue(txtGrandTotal));
            cmd.Parameters.AddWithValue("@TotalPayment", GetDecimalValue(txtTotalPaid));
            cmd.Parameters.AddWithValue("@PaymentDue", GetDecimalValue(txtBalance));
            cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text);
            cmd.Parameters.AddWithValue("@VATPer", GetDecimalValue(txtVATPer));
            cmd.Parameters.AddWithValue("@VATAmt", GetDecimalValue(txtVATAmt));
            cmd.Parameters.AddWithValue("@CurrencieName", comboBox1.Text);
            cmd.Parameters.AddWithValue("@Currency_price", Convert.ToDecimal(Cprice.Text));
            cmd.Parameters.AddWithValue("@WID", int.TryParse(WID.Text, out int wid) ? wid : 0);
        }

        private void InsertStockProductRecords(SqlConnection con, SqlTransaction transaction)
        {
            string query = @"
        INSERT INTO Stock_Product (
            StockID, ProductID, Qty, Price, TotalAmount, Barcode, WID, TotalCurrency
        ) VALUES (
            @StockID, @ProductID, @Qty, @Price, @TotalAmount, @Barcode, @WID, @TotalCurrency
        )";

            using (SqlCommand cmd = new SqlCommand(query, con, transaction))
            {
                foreach (DataGridViewRow row in DataGridView1.Rows)
                {
                    if (row.IsNewRow) continue;

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@StockID", int.Parse(txtST_ID.Text));
                    cmd.Parameters.AddWithValue("@ProductID", int.Parse(row.Cells[0].Value.ToString()));
                    cmd.Parameters.AddWithValue("@Qty", double.Parse(row.Cells[4].Value.ToString()));
                    cmd.Parameters.AddWithValue("@Price", double.Parse(row.Cells[5].Value.ToString()));
                    cmd.Parameters.AddWithValue("@TotalAmount", double.Parse(row.Cells[6].Value.ToString()));
                    cmd.Parameters.AddWithValue("@Barcode", row.Cells[3].Value.ToString());
                    cmd.Parameters.AddWithValue("@WID", int.Parse(WID.Text));
                    cmd.Parameters.AddWithValue("@TotalCurrency", double.Parse(row.Cells[7].Value.ToString()));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void UpdateTempStock(SqlConnection con, SqlTransaction transaction)
        {
            foreach (DataGridViewRow row in DataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                int productID = int.Parse(row.Cells[0].Value.ToString());
                string barcode = row.Cells[3].Value.ToString();
                double qty = double.Parse(row.Cells[4].Value.ToString());

                if (TempStockExists(con, transaction, productID, barcode))
                    UpdateExistingTempStock(con, transaction, productID, barcode, qty);
                else
                    InsertNewTempStock(con, transaction, productID, barcode, qty);
            }
        }

        private bool TempStockExists(SqlConnection con, SqlTransaction transaction, int productID, string barcode)
        {
            string query = "SELECT 1 FROM Temp_Stock WHERE ProductID = @ProductID AND Barcode = @Barcode";
            using (SqlCommand cmd = new SqlCommand(query, con, transaction))
            {
                cmd.Parameters.AddWithValue("@ProductID", productID);
                cmd.Parameters.AddWithValue("@Barcode", barcode);
                return cmd.ExecuteScalar() != null;
            }
        }

        private void UpdateExistingTempStock(SqlConnection con, SqlTransaction transaction, int productID, string barcode, double qty)
        {
            string query = @"
        UPDATE Temp_Stock 
        SET Qty = Qty + @Qty, 
            QtyP = QtyP + @Qty, 
            SupplierID = @SupplierID 
        WHERE ProductID = @ProductID 
            AND Barcode = @Barcode 
            AND WID = @WID";

            using (SqlCommand cmd = new SqlCommand(query, con, transaction))
            {
                cmd.Parameters.AddWithValue("@Qty", qty);
                cmd.Parameters.AddWithValue("@ProductID", productID);
                cmd.Parameters.AddWithValue("@Barcode", barcode);
                cmd.Parameters.AddWithValue("@WID", WID.Text);
                cmd.Parameters.AddWithValue("@SupplierID", int.Parse(txtSup_ID.Text));
                cmd.ExecuteNonQuery();
            }
        }

        private void InsertNewTempStock(SqlConnection con, SqlTransaction transaction, int productID, string barcode, double qty)
        {
            string query = @"
        INSERT INTO Temp_Stock (
            ProductID, Qty, Barcode, WID, BarcodeImage, SupplierID, QTYP
        ) VALUES (
            @ProductID, @Qty, @Barcode, @WID, @BarcodeImage, @SupplierID, @QTYP
        )";

            using (SqlCommand cmd = new SqlCommand(query, con, transaction))
            {
                cmd.Parameters.AddWithValue("@ProductID", productID);
                cmd.Parameters.AddWithValue("@Qty", qty);
                cmd.Parameters.AddWithValue("@Barcode", barcode);
                cmd.Parameters.AddWithValue("@WID", WID.Text);
                cmd.Parameters.AddWithValue("@SupplierID", int.Parse(txtSup_ID.Text));
                cmd.Parameters.AddWithValue("@QTYP", qty);

                if (pictureBox1.Image != null)
                {
                    byte[] imageData = ImageToByteArray(pictureBox1.Image);
                    cmd.Parameters.AddWithValue("@BarcodeImage", imageData);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@BarcodeImage", DBNull.Value);
                }

                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateLedgers(SqlConnection con, SqlTransaction transaction)
        {
            if (cmbPurchaseType.SelectedIndex == 1)
            {
                SupplierLedgerSave(dtpDate.Value.Date, "اجل", txtInvoiceNo.Text, "فاتورة مشتريات", Convert.ToDecimal(txtSubTotal.Text) * Convert.ToDecimal(Cprice.Text), 0, txtSupplierID.Text, comboBox1.Text, Convert.ToDecimal(Cprice.Text));
            }
            else
            {
                SupplierLedgerSave(dtpDate.Value.Date, "كاش", txtInvoiceNo.Text, "فاتورة مشتريات", Convert.ToDecimal(txtSubTotal.Text) * Convert.ToDecimal(Cprice.Text), Convert.ToDecimal(txtTotalPaid.Text) * Convert.ToDecimal(Cprice.Text), txtSupplierID.Text, comboBox1.Text, Convert.ToDecimal(Cprice.Text));

            }

        }

        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }
        private void SaveLedger()
        {
            string transactionType = cmbPurchaseType.SelectedIndex == 1 ? "اجل" : "كاش";
            decimal amount = Convert.ToDecimal(txtSubTotal.Text) * Convert.ToDecimal(Cprice.Text);
            decimal payment = cmbPurchaseType.SelectedIndex == 0 ? Convert.ToDecimal(txtTotalPaid.Text) * Convert.ToDecimal(Cprice.Text) : 0;
            LedgerSave(dtpDate.Value.Date, transactionType + "/" + txtSupplierName.Text, txtInvoiceNo.Text, "فاتورة مشتريات", amount, payment, txtSupplierID.Text);
        }





        private decimal SafeConvertToDecimal(string textBox)
        {
            return decimal.TryParse(textBox, out decimal result) ? result : 0m;
        }

        private void SaveSupplierLedger()
        {
            if (cmbPurchaseType.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a purchase type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime date = dtpDate.Value.Date;
            string name = cmbPurchaseType.SelectedIndex == 1 ? "اجل" : "كاش";
            string ledgerNo = txtInvoiceNo.Text;
            string label = "فاتورة مشتريات";
            decimal totalPaid = SafeConvertToDecimal(txtSubTotal.Text.ToString());
            decimal cPrice = SafeConvertToDecimal(Cprice.Text.ToString());
            decimal cPricee = cPrice != 0 ? totalPaid : 0;

            decimal subTotal = SafeConvertToDecimal(txtSubTotal.Text.ToString()) * cPrice;
            string supplierID = txtSupplierID.Text;
            string currency = comboBox1.Text;

            decimal debit = subTotal;
            decimal credit = cmbPurchaseType.SelectedIndex == 1 ? 0 : totalPaid;

            SupplierLedgerSave(date, name, ledgerNo, label, debit, credit, supplierID, currency, cPrice);
        }
      
        public void LedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g)
        {


            con.Open();
            string cb = "INSERT INTO LedgerBook(Date, Name, LedgerNo, Label, Debit, Credit, PartyID) VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7)";
            using (var cmd = new SqlCommand(cb, con))
            {
                cmd.Parameters.AddWithValue("@d1", a);
                cmd.Parameters.AddWithValue("@d2", b);
                cmd.Parameters.AddWithValue("@d3", c);
                cmd.Parameters.AddWithValue("@d4", d);
                cmd.Parameters.AddWithValue("@d5", e);
                cmd.Parameters.AddWithValue("@d6", f);
                cmd.Parameters.AddWithValue("@d7", g);

                cmd.ExecuteReader();
            }
            con.Close();

        }
        public void print()
        {
            try
            {
                frmReport frmReport = new frmReport();

                rptPurchase_1 rpt = new rptPurchase_1(); // The main report
                SqlConnection myConnection;
                SqlCommand MyCommand = new SqlCommand();
                SqlDataAdapter myDA = new SqlDataAdapter();
                DataSet myDS = new DataSet(); // The DataSet for the main report
                DataSet myDS1 = new DataSet(); // Another DataSet

                myConnection = new SqlConnection(DataAccessLayer.Con());
                MyCommand.Connection = myConnection;
                MyCommand.CommandText = @"SELECT DISTINCT Stock.ST_ID, Stock.InvoiceNo, Stock.Date, Stock.SupplierID, 
                              Stock.GrandTotal, Stock.TotalPayment, Stock.PaymentDue, Stock.Remarks, 
                              Stock_Product.SP_ID, Stock_Product.StockID, Stock_Product.ProductID, 
                              Stock_Product.Qty, Stock_Product.Price, Stock_Product.TotalAmount, Supplier.ID, 
                              Supplier.SupplierID AS Expr1, Supplier.Name, Supplier.Address, Supplier.City, 
                              Supplier.State, Supplier.ZipCode, Supplier.ContactNo, Supplier.EmailID, 
                              Supplier.Remarks AS Expr2, Product.PID, Product.ProductCode, Product.ProductName, 
                              Product.SubCategoryID, Product.Description, Product.CostPrice, Product.SellingPrice, 
                              Product.Discount, Product.VAT, Product.ReorderPoint 
                              FROM Stock 
                              INNER JOIN Stock_Product ON Stock.ST_ID = Stock_Product.StockID 
                              INNER JOIN Supplier ON Stock.SupplierID = Supplier.ID 
                              INNER JOIN Product ON Stock_Product.ProductID = Product.PID 
                              WHERE InvoiceNo = @d1 
                              ORDER BY Stock.Date";
                MyCommand.Parameters.AddWithValue("@d1", txtInvoiceNo.Text);
                MyCommand.CommandType = CommandType.Text;
                myDA.SelectCommand = MyCommand;
                myDA.Fill(myDS, "Stock");
                myDA.Fill(myDS, "Stock_Product");
                myDA.Fill(myDS, "Product");
                myDA.Fill(myDS, "Supplier");

                con = new SqlConnection(DataAccessLayer.Con());
                con.Open();
                string ct = @"SELECT ISNULL(SUM(GrandTotal), 0), ISNULL(SUM(TotalPayment), 0), ISNULL(SUM(PaymentDue), 0) 
                 FROM Stock, Supplier 
                 WHERE Supplier.ID = Stock.SupplierID 
                 AND InvoiceNo = @d1";
                SqlCommand cmd = new SqlCommand(ct, con);
                cmd.Parameters.AddWithValue("@d1", txtInvoiceNo.Text);

                SqlDataReader rdr = cmd.ExecuteReader();

                decimal a = 0, b = 0, c = 0;

                while (rdr.Read())
                {
                    if (!rdr.IsDBNull(0))
                        a = Convert.ToDecimal(rdr.GetValue(0));  // Ensure safe conversion to decimal
                    if (!rdr.IsDBNull(1))
                        b = Convert.ToDecimal(rdr.GetValue(1));
                    if (!rdr.IsDBNull(2))
                        c = Convert.ToDecimal(rdr.GetValue(2));
                }

                con.Close();


                string currencyName = string.Empty;

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    SqlCommand cmd1 = new SqlCommand("SELECT CurrencieName FROM Stock WHERE InvoiceNo = @d1", con);
                    cmd1.Parameters.AddWithValue("@d1", txtInvoiceNo.Text);

                    SqlDataReader reader = cmd1.ExecuteReader();
                    if (reader.Read())
                    {
                        currencyName = reader["CurrencieName"].ToString();
                    }
                    con.Close();
                }
                double Currency_price = 0;
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    SqlCommand cmd1 = new SqlCommand("SELECT Currency_price FROM Stock WHERE InvoiceNo = @d1", con);
                    cmd1.Parameters.AddWithValue("@d1", txtInvoiceNo.Text);

                    SqlDataReader reader = cmd1.ExecuteReader();
                    if (reader.Read())
                    {
                        Currency_price = Convert.ToDouble(reader["Currency_price"].ToString());
                    }
                    con.Close();
                }

                myDS1.WriteXmlSchema("TotalPurchase.xml");

                // Set the main report data source
                rpt.SetDataSource((DataSet)myDS);  // Explicitly cast as DataSet

                // Create an empty DataTable to clear the subreport's data source
                

                rpt.SetParameterValue("p3", a);
                rpt.SetParameterValue("p4", b);
                rpt.SetParameterValue("p5", c);
                rpt.SetParameterValue("p6", DateTime.Today);
                rpt.SetParameterValue("currency_name", currencyName);
                rpt.SetParameterValue("Currency_price", Currency_price);

                frmReport.crystalReportViewer1.ReportSource = rpt;
                frmReport.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default; // Reset cursor to default after operation
            }


        }
        public void SupplierLedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g, string q, decimal v)
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con())) // Use your existing connection method
            {
                con.Open();
                string cb = "INSERT INTO SupplierLedgerBook(Date, Name, LedgerNo, Label, Debit, Credit, PartyID, Currencies, CPrice) " +
                            "VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9)";

                using (SqlCommand cmd = new SqlCommand(cb, con))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.Parameters.AddWithValue("@d2", b);
                    cmd.Parameters.AddWithValue("@d3", c);
                    cmd.Parameters.AddWithValue("@d4", d);
                    cmd.Parameters.AddWithValue("@d5", e);
                    cmd.Parameters.AddWithValue("@d6", f);
                    cmd.Parameters.AddWithValue("@d7", g);
                    cmd.Parameters.AddWithValue("@d8", q);
                    cmd.Parameters.AddWithValue("@d9", v);

                    cmd.ExecuteNonQuery(); // Use ExecuteNonQuery instead of ExecuteReader
                }
            }
        }


        private void txtRemarks_TextChanged(object sender, EventArgs e)
        {

        }
        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in DataGridView1.SelectedRows)
                    DataGridView1.Rows.Remove(row);
                double k = 0d;
                k = SubTotal();
                k = Math.Round(k, 2);
                txtSubTotal.Text = k.ToString();
                Compute();
                btnRemove.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public double SubTotal()
        {
            double sum = 0d;
            try
            {
                foreach (DataGridViewRow r in this.DataGridView1.Rows)
                {
                    // Skip new rows
                    if (!r.IsNewRow)
                    {
                        // Safely parse the cell value as double, defaulting to 0 if it's null or not a number
                        double value = Convert.ToDouble(r.Cells[6].Value);
                        sum += value;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return sum;
        }



        private void cmbPurchaseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPurchaseType.SelectedIndex == 1)
            {
                txtTotalPaid.Text = "";
                txtTotalPaid.ReadOnly = true;
                txtTotalPaid.Enabled = false;
            }
            else
            {
                txtTotalPaid.Text = "";
                txtTotalPaid.ReadOnly = false;
                txtTotalPaid.Enabled = true;
            }
        }

        private void btnSelection_Click(object sender, EventArgs e)
        {
            SupplierScreen supplierScreen = new SupplierScreen();
            supplierScreen.lblSet.Text = "Purchase";
            supplierScreen.Show();
/*            this.Hide();
*/
        }


        public void Clear()
        {
            txtProductCode.Text = "";
            txtProductName.Text = "";
            txtQty.Text = "";
            txtPricePerQty.Text = "";
            txtTotalAmount.Text = "";
            txtBarcode.Text = "";
            textBox2.Text = "";
            comboBox2.SelectedIndex = comboBox2.Items.Count > 0 ? 0 : -1;
        }

        public void Reset()
        {
            txtInvoiceNo.Clear();
            txtST_ID.Clear();
            txtAddress.Text = "";
            txtBalance.Text = "";
            txtCity.Text = "";
            txtContactNo.Text = "";
            txtDiscPer.Text = "0";
            txtDisc.Text = "0";
            txtSubTotal.Text = "";
            txtTotal.Text = "";
            txtSupplierID.Text = "";
            txtSupplierName.Text = "";
            txtSup_ID.Text = "";
            txtVATPer.Text = "0";
            txtVATAmt.Text = "0";
            txtFreightCharges.Text = "0";
            txtGrandTotal.Text = "";
            txtInvoiceNo.Text = "";
            txtOtherCharges.Text = "0";
            txtPreviousDue.Text = "0";
            txtRemarks.Text = "";
            txtRoundOff.Text = "0";
            txtTotalPaid.Text = "";
            cmbPurchaseType.SelectedIndex = cmbPurchaseType.Items.Count > 0 ? 0 : -1;
            comboBox2.SelectedIndex = comboBox2.Items.Count > 0 ? 0 : -1;
            comboBoxGenerate();

            dtpDate.Text = DateTime.Today.ToString();
            btnSave.Enabled = true;
            btnDelete.Enabled = false;
            DataGridView1.Enabled = true;
            btnAdd.Enabled = true;
            pnlCalc.Enabled = true;
            lblBalance.Text = "0";
            txtTotalPaid.ReadOnly = false;
            txtTotalPaid.Enabled = true;
            DataGridView1.Rows.Clear();
            btnSelection.Enabled = true;
            txtQty.Focus();
            Clear();
            auto();
            
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            Reset();
        }

        // Install-Package Microsoft.VisualBasic
        private void txtPricePerQty_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            var keyChar = e.KeyChar;

            if (char.IsControl(keyChar))
            {
            }
            // Allow all control characters.
            else if (char.IsDigit(keyChar) || keyChar == '.')
            {
                var text = this.txtPricePerQty.Text;
                var selectionStart = this.txtPricePerQty.SelectionStart;
                var selectionLength = this.txtPricePerQty.SelectionLength;

                text = text.Substring(0, selectionStart) + keyChar + text.Substring(selectionStart + selectionLength);
                int decimalIndex = text.IndexOf('.');
                if (decimalIndex != -1 && text.Length - decimalIndex - 1 > 2)
                {
                    // Reject an integer that is longer than 16 digits.
                    e.Handled = true;
                }
                int decimalIndexx = text.IndexOf('.');
                if (decimalIndexx != -1 && text.Length - decimalIndex - 1 > 2)
                {
                    // Reject a real number with two many decimal places.
                    e.Handled = false;
                }
            }
            else
            {
                // Reject all other characters.
                e.Handled = true;
            }
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("هل أنت متأكد أنك تريد حذف هذا السجل?", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
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
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // Log the start of the operation
                    Console.WriteLine("Starting DeleteRecord...");

                    // Check for related purchase returns
                    string checkQuery = "SELECT ST_ID FROM Stock INNER JOIN PurchaseReturn ON PurchaseReturn.PurchaseID = Stock.ST_ID WHERE ST_ID = @d1";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@d1", Val(txtST_ID.Text));
                        using (SqlDataReader rdr = checkCmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                MessageBox.Show("لا يمكن الحذف حيث يوجد مرتجعات شراء ذات علاقة", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }

                    // Delete from Stock
                    string deleteQuery = "DELETE FROM Stock WHERE ST_ID = @d1";
                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, con))
                    {
                        deleteCmd.Parameters.AddWithValue("@d1", Val(txtST_ID.Text));
                        int rowsAffected = deleteCmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {

                            // Update Temp_Stock
                            foreach (DataGridViewRow row in DataGridView1.Rows)
                            {
                                if (!row.IsNewRow)
                                {
                                    string updateQuery = "UPDATE Temp_Stock SET Qty = Qty - @Qty WHERE ProductID = @d1 AND Barcode = @d2 AND WID=@d3 ";
                                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                                    {
                                        updateCmd.Parameters.AddWithValue("@Qty", Val(row.Cells[4].Value.ToString()));
                                        updateCmd.Parameters.AddWithValue("@d1", Val(row.Cells[0].Value.ToString()));
                                        updateCmd.Parameters.AddWithValue("@d2", row.Cells[3].Value.ToString());
                                        updateCmd.Parameters.AddWithValue("@d3", WID.Text);
                                        updateCmd.ExecuteNonQuery();
                                    }
                                }
                            }

                            // Perform ledger operations
                            LedgerDelete(txtInvoiceNo.Text, "دفع لــ " + txtSupplierName.Text + "");
                            LedgerDelete(txtInvoiceNo.Text, "فاتورة مشتريات");

                            SupplierLedgerDelete(txtInvoiceNo.Text, "فاتورة مشتريات");

                            MessageBox.Show("تم الحذف بنجاح", "السجلات", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            FileSystem.Reset();
                            Reset();    
                        }
                        else
                        {
                            MessageBox.Show("لا يوجد سجلات", "عذراً", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            FileSystem.Reset();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"هناك عمليات اخري للغاتورة يرجي ازالتها اولا يرجي مراجعة شركات الشحن.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        public static void SupplierLedgerDelete(string a, string b)
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                string cq = "DELETE FROM SupplierLedgerBook WHERE LedgerNo=@d1 and Label=@d2";
                using (var cmd = new SqlCommand(cq, con))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.Parameters.AddWithValue("@d2", b);
                    cmd.ExecuteReader();
                }
            }
        }

        public static void LedgerDelete(string a, string b)
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                string cq = "DELETE FROM LedgerBook WHERE LedgerNo=@d1 AND Label=@d2";
                using (var cmd = new SqlCommand(cq, con))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.Parameters.AddWithValue("@d2", b);
                    cmd.ExecuteNonQuery(); // Correct method for DELETE operations
                }
            }
        }

        private void DataGridView1_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (DataGridView1.Rows.Count > 0 && DataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow dr = DataGridView1.SelectedRows[0];
                txtProductID.Text = dr.Cells[0]?.Value?.ToString() ?? string.Empty;
                txtProductCode.Text = dr.Cells[1]?.Value?.ToString() ?? string.Empty;
                txtProductName.Text = dr.Cells[2]?.Value?.ToString() ?? string.Empty;
                txtBarcode.Text = dr.Cells[3]?.Value?.ToString() ?? string.Empty; // Adjust index if necessary
                txtQty.Text= dr.Cells[4]?.Value?.ToString() ?? string.Empty;
                txtPricePerQty.Text = dr.Cells[5]?.Value?.ToString() ?? string.Empty;
                textBox2.Text = dr.Cells[6]?.Value?.ToString() ?? string.Empty;
                dgw.Visible= false;  
                btnAdd.Enabled = true;
                btnRemove.Enabled = true;
                btnDelete.Enabled = true;
                //if (btnSave.Enabled == false)
                //{
                //    DataGridView1.Rows.Remove(dr);
                //}



            }
        }


        private void txtPricePerQty_TextChanged(object sender, EventArgs e)
        {
            double i = 0d;
            double x = 0d;
            i = (double)(Val(txtQty.Text) * Val(txtPricePerQty.Text));
            x = i / Convert.ToDouble(Cprice.Text);
            i = Math.Round(i, 2);
            x = Math.Round(x, 2);
            txtTotalAmount.Text = i.ToString();
        }


        private void txtQty_TextChanged(object sender, EventArgs e)
        {
            double i = 0d;
            i = (double)(Val(txtQty.Text) * Val(txtPricePerQty.Text));
            i = Math.Round(i, 2);
            txtTotalAmount.Text = i.ToString();
            decimal totalAmount = decimal.Parse(txtTotalAmount.Text);
            decimal cPrice = decimal.Parse(Cprice.Text);

            // Calculate the division result
            decimal result = totalAmount / cPrice;
            decimal result1 = totalAmount * cPrice;
            result = Math.Round(result, 2);
            textBox2.Text = result1.ToString();  

        }

        private void txtTotalAmount_TextChanged(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            ProductsScreen productsScreen = new ProductsScreen();
            productsScreen.lblSet.Text= "payment";
            productsScreen.Show();

        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            PymentinvoiceScreen pymentinvoiceScreen = new PymentinvoiceScreen();
            pymentinvoiceScreen.lblSet.Text = "Purchase";
            pymentinvoiceScreen.Show();
        }

        private void txtSubTotal_TextChanged(object sender, EventArgs e)
        {
            Compute();
        }

        private void txtFreightCharges_TextChanged(object sender, EventArgs e)
        {
            Compute();

        }

        private void txtOtherCharges_TextChanged(object sender, EventArgs e)
        {
            Compute();

        }

        private void txtPreviousDue_TextChanged(object sender, EventArgs e)
        {
            Compute();

        }
        private void txtTotalPaid_TextChanged(object sender, EventArgs e)
        {
            Compute();

        }

        private void txtTotalPayment_TextChanged(object sender, EventArgs e)
        {
            Compute();
        }


        private void txtTotal_TextChanged(object sender, EventArgs e)
        {
            Compute();


        }

        private void txtRoundOff_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtGrandTotal_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtDiscPer_TextChanged(object sender, EventArgs e)
        {
            Compute();

        }

        private void txtVATPer_TextChanged(object sender, EventArgs e)
        {
            Compute();

        }

        private void txtVATPer_KeyPress(object sender, KeyPressEventArgs e)
        {
            var keyChar = e.KeyChar;

            if (char.IsControl(keyChar))
            {
            }
            // Allow all control characters.
            else if (char.IsDigit(keyChar) || keyChar == '.')
            {
                var text = this.txtVATPer.Text;
                var selectionStart = this.txtVATPer.SelectionStart;
                var selectionLength = this.txtVATPer.SelectionLength;

                text = text.Substring(0, selectionStart) + keyChar + text.Substring(selectionStart + selectionLength);
                if (int.TryParse(text, out int intValue) && text.Length > 16)
                {
                    // Reject an integer that is longer than 16 digits.
                    e.Handled = true;
                }
                else if (double.TryParse(text, out double doubleValue))
                {
                    int decimalIndex = text.IndexOf('.');
                    if (decimalIndex != -1 && text.Length - decimalIndex - 1 > 2)
                    {
                        // Reject a real number with too many decimal places.
                        e.Handled = true;
                    }
                    else
                    {
                        e.Handled = false;
                    }
                }
                else
                {
                    e.Handled = false;
                }

            }
            else
            {
                // Reject all other characters.
                e.Handled = true;
            }
        }

        private void txtBarcode_TextChanged(object sender, EventArgs e)
        {

        }
        private void txtBarcode_KeyPress(object sender,System.Windows.Forms.KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' | e.KeyChar > '9') & e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void txtInvoiceNo_TextChanged(object sender, EventArgs e)
        {

        }
        private DataGridViewRow previousRow;

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
               
                if (string.IsNullOrWhiteSpace(txtProductCode.Text))
                {
                    MessageBox.Show("الرجاء إدراج رقم الصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProductCode.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtBarcode.Text))
                {
                    MessageBox.Show("الرجاء إدخال الباركود", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBarcode.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtQty.Text))
                {
                    MessageBox.Show("الرجاء كتابة الكمية", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQty.Focus();
                    return;
                }
                if (txtQty.Text == "0")
                {
                    MessageBox.Show("الكمية يجب أن تكون أكبر من صفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQty.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtPricePerQty.Text))
                {
                    MessageBox.Show("الرجاء إدخال سعر الوحدة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPricePerQty.Focus();
                    return;
                }
                if (DataGridView1.Rows.Count == 0)
                {
                    // Add the values to the DataGridView
                    DataGridView1.Rows.Add(txtProductID.Text, txtProductCode.Text, txtProductName.Text, txtBarcode.Text, txtQty.Text, txtPricePerQty.Text, txtTotalAmount.Text, textBox2.Text); // Use the result here

                    // Update the SubTotal
                    txtSubTotal.Text = Math.Round(SubTotal(), 2).ToString();
                    comboBox2.Visible=true;
                    comboBox2.Visible=true;
                    Clear();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtQty.Text))
                {
                    btnAdd.Enabled = true;
                    return;
                }

            
                DataGridView1.Rows.Add(txtProductID.Text, txtProductCode.Text, txtProductName.Text, txtBarcode.Text, txtQty.Text, txtPricePerQty.Text, txtTotalAmount.Text, textBox2.Text);
                txtSubTotal.Text = Math.Round(SubTotal(), 2).ToString();
                DataGridView1.Enabled = true;

                Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void txtSupplierID_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtAddress_TextChanged(object sender, EventArgs e)
        {

        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
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

            Brush b = SystemBrushes.Window; // You can use SystemBrushes.ControlText for text color if needed
            e.Graphics.DrawString(strRowNumber, this.Font, b, e.RowBounds.Location.X + DataGridView1.Width - 25, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2));
        }

        private void GroupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void txtQty_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void txtPricePerQty_TextChanged_1(object sender, EventArgs e)
        {

        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation checks
                if (string.IsNullOrWhiteSpace(txtSupplierID.Text))
                {
                    MessageBox.Show("الرجاء إدراج رقم المورد", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSupplierID.Focus();
                    return;
                }
                if (DataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("يجب إضافة أصناف أولا", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtDiscPer.Text) || !decimal.TryParse(txtDiscPer.Text, out _))
                {
                    MessageBox.Show("الرجاء كتابة الخصم %", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDiscPer.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtVATPer.Text) || !decimal.TryParse(txtVATPer.Text, out _))
                {
                    MessageBox.Show("الرجاء كتابة مبلغ الضريبة %", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtVATPer.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtFreightCharges.Text) || !decimal.TryParse(txtFreightCharges.Text, out _))
                {
                    MessageBox.Show("الرجاء كتابة مصاريف الشحن", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtFreightCharges.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtOtherCharges.Text) || !decimal.TryParse(txtOtherCharges.Text, out _))
                {
                    MessageBox.Show("الرجاء كتابة المصروفات الأخرى", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtOtherCharges.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtRoundOff.Text) || !decimal.TryParse(txtRoundOff.Text, out _))
                {
                    MessageBox.Show("الرجاء استخدام التقريب", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtRoundOff.Focus();
                    return;
                }
                if (cmbPurchaseType.SelectedIndex == 0)
                {
                    if (string.IsNullOrWhiteSpace(txtTotalPaid.Text) || !decimal.TryParse(txtTotalPaid.Text, out _))
                    {
                        MessageBox.Show("الرجاء كتابة إجمالي المدفوع", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTotalPaid.Focus();
                        return;
                    }
                    if (Convert.ToDecimal(txtTotalPaid.Text) <= 0)
                    {
                        MessageBox.Show("المبلغ المدفوع يجب أن يكون أكبر من صفر", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTotalPaid.Focus();
                        return;
                    }
                }

                // Store old quantities from Stock_Product
                Dictionary<int, double> oldQuantities = new Dictionary<int, double>();
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string selectQuery = "SELECT ProductID, Qty FROM Stock_Product WHERE StockID=@d1";
                    using (SqlCommand cmd = new SqlCommand(selectQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtST_ID.Text));
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                oldQuantities[Convert.ToInt32(rdr["ProductID"])] = Convert.ToDouble(rdr["Qty"]);
                            }
                        }
                    }

                    // Update Stock table
                    string query = "UPDATE Stock SET Date=@d3, PurchaseType=@d4, SupplierID=@d5, SubTotal=@d6, DiscountPer=@d7, Discount=@d8, PreviousDue=@d9, " +
                                   "FreightCharges=@d10, OtherCharges=@d11, Total=@d12, RoundOff=@d13, GrandTotal=@d14, TotalPayment=@d15, PaymentDue=@d16, Remarks=@d17, " +
                                   "VATPer=@d18, VATAmt=@d19, CurrencieName=@d20, Currency_price=@d21, Total_By_Pound=@d22, WID=@d23 WHERE InvoiceNo=@d2 AND WID=@d23";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtST_ID.Text);
                        cmd.Parameters.AddWithValue("@d2", txtInvoiceNo.Text);
                        cmd.Parameters.AddWithValue("@d3", dtpDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d4", cmbPurchaseType.Text);
                        cmd.Parameters.AddWithValue("@d5", txtSup_ID.Text);
                        cmd.Parameters.AddWithValue("@d6", decimal.TryParse(txtSubTotal.Text, out decimal subTotal) ? subTotal : 0);
                        cmd.Parameters.AddWithValue("@d7", decimal.TryParse(txtDiscPer.Text, out decimal discPer) ? discPer : 0);
                        cmd.Parameters.AddWithValue("@d8", decimal.TryParse(txtDisc.Text, out decimal disc) ? disc : 0);
                        cmd.Parameters.AddWithValue("@d9", decimal.TryParse(txtPreviousDue.Text, out decimal previousDue) ? previousDue : 0);
                        cmd.Parameters.AddWithValue("@d10", decimal.TryParse(txtFreightCharges.Text, out decimal freightCharges) ? freightCharges : 0);
                        cmd.Parameters.AddWithValue("@d11", decimal.TryParse(txtOtherCharges.Text, out decimal otherCharges) ? otherCharges : 0);
                        cmd.Parameters.AddWithValue("@d12", decimal.TryParse(txtTotal.Text, out decimal total) ? total : 0);
                        cmd.Parameters.AddWithValue("@d13", decimal.TryParse(txtRoundOff.Text, out decimal roundOff) ? roundOff : 0);
                        cmd.Parameters.AddWithValue("@d14", decimal.TryParse(txtGrandTotal.Text, out decimal grandTotal) ? grandTotal : 0);
                        cmd.Parameters.AddWithValue("@d15", decimal.TryParse(txtTotalPaid.Text, out decimal totalPaid) ? totalPaid : 0);
                        cmd.Parameters.AddWithValue("@d16", decimal.TryParse(txtBalance.Text, out decimal balance) ? balance : 0);
                        cmd.Parameters.AddWithValue("@d17", txtRemarks.Text);
                        cmd.Parameters.AddWithValue("@d18", decimal.TryParse(txtVATPer.Text, out decimal vatPer) ? vatPer : 0);
                        cmd.Parameters.AddWithValue("@d19", decimal.TryParse(txtVATAmt.Text, out decimal vatAmt) ? vatAmt : 0);
                        cmd.Parameters.AddWithValue("@d20", comboBox1.Text);
                        cmd.Parameters.AddWithValue("@d21", decimal.TryParse(Cprice.Text, out decimal currencyPrice) ? currencyPrice : 0);
                        cmd.Parameters.AddWithValue("@d22", cmbPurchaseType.SelectedIndex == 0 ? Convert.ToDecimal(Cprice.Text) * Convert.ToDecimal(txtTotalPaid.Text) : 0);
                        cmd.Parameters.AddWithValue("@d23", WID.Text);
                        cmd.ExecuteNonQuery();
                    }

                    // Update Stock_Product table
                    string deleteQuery = "DELETE FROM Stock_Product WHERE StockID=@d1";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtST_ID.Text));
                        cmd.ExecuteNonQuery();
                    }


                    string insertQuery = @"
                    INSERT INTO Stock_Product(StockID, ProductID, Qty, Price, TotalAmount, Barcode, WID, TotalCurrency) 
                    VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8)";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        foreach (DataGridViewRow row in DataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                cmd.Parameters.Clear();
                                int productId = Convert.ToInt32(row.Cells[0].Value);
                                double newQty = Convert.ToDouble(row.Cells[4].Value);
                                double oldQty = oldQuantities.ContainsKey(productId) ? oldQuantities[productId] : 0;
                                double qtyDifference = newQty - oldQty;


                                cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtST_ID.Text));
                                cmd.Parameters.AddWithValue("@d2", productId);
                                cmd.Parameters.AddWithValue("@d3", newQty);
                                cmd.Parameters.AddWithValue("@d4", Convert.ToDouble(row.Cells[5].Value));
                                cmd.Parameters.AddWithValue("@d5", Convert.ToDouble(row.Cells[6].Value));
                                cmd.Parameters.AddWithValue("@d6", row.Cells[3].Value.ToString());
                                cmd.Parameters.AddWithValue("@d7", WID.Text);
                                cmd.Parameters.AddWithValue("@d8", row.Cells[7].Value == null || string.IsNullOrEmpty(row.Cells[7].Value.ToString()) ? Convert.ToDouble(row.Cells[6].Value) * Convert.ToDouble(Cprice.Text) : Convert.ToDouble(row.Cells[7].Value));

                                cmd.ExecuteNonQuery();

                                // Update the quantity in Temp_Stock
                                string updateTempStockQuery = qtyDifference > 0
                                    ? "UPDATE Temp_Stock SET Qty = Qty + @qtyDiff, QtyP = QtyP + @qtyDifff, SupplierID=@d4 WHERE ProductID = @d1 AND Barcode = @d2 AND WID = @d3"
                                    : "UPDATE Temp_Stock SET Qty = Qty - @qtyDiff, QtyP = QtyP - @qtyDifff, SupplierID=@d4 WHERE ProductID = @d1 AND Barcode = @d2 AND WID = @d3";
                                using (SqlCommand cmdUpdate = new SqlCommand(updateTempStockQuery, con))
                                {
                                    cmdUpdate.Parameters.AddWithValue("@qtyDiff", Math.Abs(qtyDifference));
                                    cmdUpdate.Parameters.AddWithValue("@qtyDifff", Math.Abs(qtyDifference));
                                    cmdUpdate.Parameters.AddWithValue("@d1", productId);
                                    cmdUpdate.Parameters.AddWithValue("@d2", row.Cells[3].Value.ToString());
                                    cmdUpdate.Parameters.AddWithValue("@d3", WID.Text);
                                    cmdUpdate.Parameters.AddWithValue("@d4", Convert.ToInt32(txtSup_ID.Text));
                                    cmdUpdate.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    // Ledger operations based on purchase type
                    if (cmbPurchaseType.SelectedIndex == 1)
                    {
                        LedgerUpdate(dtpDate.Value.Date, "اجل" + "/" + txtSupplierName.Text, Convert.ToDecimal(txtSubTotal.Text) * Convert.ToDecimal(Cprice.Text), 0, txtSupplierID.Text, txtInvoiceNo.Text, "فاتورة مشتريات");
                    }
                    if (cmbPurchaseType.SelectedIndex == 0)
                    {
                        LedgerUpdate(dtpDate.Value.Date, "كاش" + "/" + txtSupplierName.Text, Convert.ToDecimal(txtSubTotal.Text) * Convert.ToDecimal(Cprice.Text), Convert.ToDecimal(txtTotalPaid.Text) * Convert.ToDecimal(Cprice.Text), txtSupplierID.Text, txtInvoiceNo.Text, "فاتورة مشتريات");
                    }

                    // Supplier ledger operations based on purchase type
                    if (cmbPurchaseType.SelectedIndex == 1)
                    {
                        SupplierLedgerUpdate(dtpDate.Value.Date, "اجل", Convert.ToDecimal(txtSubTotal.Text) * Convert.ToDecimal(Cprice.Text), 0, txtInvoiceNo.Text, "فاتورة مشتريات", comboBox1.Text, Convert.ToDecimal(Cprice.Text));
                    }
                    if (cmbPurchaseType.SelectedIndex == 0)
                    {
                        SupplierLedgerUpdate(dtpDate.Value.Date, "كاش", Convert.ToDecimal(txtSubTotal.Text) * Convert.ToDecimal(Cprice.Text), Convert.ToDecimal(txtTotalPaid.Text) * Convert.ToDecimal(Cprice.Text), txtInvoiceNo.Text, "فاتورة مشتريات", comboBox1.Text, Convert.ToDecimal(Cprice.Text));
                    }

                    MessageBox.Show("تم التحديث بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnSave.Enabled = false;
                    Reset();
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                File.AppendAllText("ErrorLog.txt", $"[{DateTime.Now}] {ex.Message}\n{ex.StackTrace}\n");
                MessageBox.Show("خطأ: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }







        /*   private void btnUpdate_Click(object sender, EventArgs e)
           {
               try
               {
                   // Validation checks
                   if (string.IsNullOrWhiteSpace(txtSupplierID.Text))
                   {
                       MessageBox.Show("الرجاء إدراج رقم المورد", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                       txtSupplierID.Focus();
                       return;
                   }
                   if (DataGridView1.Rows.Count == 0)
                   {
                       MessageBox.Show("يجب إضافة أصناف أولا", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                       return;
                   }

                   if (!double.TryParse(txtDiscPer.Text, out double discPer) ||
                       !double.TryParse(txtVATPer.Text, out double vatPer) ||
                       !double.TryParse(txtFreightCharges.Text, out double freightCharges) ||
                       !double.TryParse(txtOtherCharges.Text, out double otherCharges) ||
                       !double.TryParse(txtRoundOff.Text, out double roundOff))
                   {
                       MessageBox.Show("الرجاء إدخال القيم الرقمية بشكل صحيح", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                       return;
                   }

                   if (cmbPurchaseType.SelectedIndex == 0)
                   {
                       if (!double.TryParse(txtTotalPaid.Text, out double totalPaid) || totalPaid <= 0)
                       {
                           MessageBox.Show("المبلغ المدفوع يجب أن يكون أكبر من صفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                           txtTotalPaid.Focus();
                           return;
                       }
                   }

                   // Store old quantities from Stock_Product
                   Dictionary<int, double> oldQuantities = new Dictionary<int, double>();

                   using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                   {
                       con.Open();

                       string selectQuery = "SELECT ProductID, Qty FROM Stock_Product WHERE StockID=@d1";
                       using (SqlCommand cmd = new SqlCommand(selectQuery, con))
                       {
                           cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtST_ID.Text));
                           using (SqlDataReader rdr = cmd.ExecuteReader())
                           {
                               while (rdr.Read())
                               {
                                   if (!rdr.IsDBNull(1))
                                   {
                                       oldQuantities[Convert.ToInt32(rdr["ProductID"])] = Convert.ToDouble(rdr["Qty"]);
                                   }
                               }
                           }
                       }

                       // Update Stock table
                       string query = @"
               UPDATE Stock SET ST_ID=@d1, Date=@d3, PurchaseType=@d4, SupplierID=@d5, SubTotal=@d6, DiscountPer=@d7, Discount=@d8, PreviousDue=@d9,
               FreightCharges=@d10, OtherCharges=@d11, Total=@d12, RoundOff=@d13, GrandTotal=@d14, TotalPayment=@d15, PaymentDue=@d16, Remarks=@d17, 
               VATPer=@d18, VATAmt=@d19, CurrencieName=@d20, Currency_price=@d21, Total_By_Pound=@d22, WID=@d23 WHERE InvoiceNo=@d2 AND WID=@d23";

                       using (SqlCommand cmd = new SqlCommand(query, con))
                       {
                           cmd.Parameters.AddWithValue("@d1", txtST_ID.Text);
                           cmd.Parameters.AddWithValue("@d2", txtInvoiceNo.Text);
                           cmd.Parameters.AddWithValue("@d3", dtpDate.Value.Date);
                           cmd.Parameters.AddWithValue("@d4", cmbPurchaseType.Text);
                           cmd.Parameters.AddWithValue("@d5", txtSup_ID.Text);
                           cmd.Parameters.AddWithValue("@d6", (txtSubTotal.Text));
                           cmd.Parameters.AddWithValue("@d7", discPer);
                           cmd.Parameters.AddWithValue("@d8", (txtDisc.Text));
                           cmd.Parameters.AddWithValue("@d9", (txtPreviousDue.Text));
                           cmd.Parameters.AddWithValue("@d10", freightCharges);
                           cmd.Parameters.AddWithValue("@d11", otherCharges);
                           cmd.Parameters.AddWithValue("@d12", (txtTotal.Text));
                           cmd.Parameters.AddWithValue("@d13", roundOff);
                           cmd.Parameters.AddWithValue("@d14", (txtGrandTotal.Text));
                           cmd.Parameters.AddWithValue("@d15", (txtTotalPaid.Text));
                           cmd.Parameters.AddWithValue("@d16", (txtBalance.Text));
                           cmd.Parameters.AddWithValue("@d17", string.IsNullOrEmpty(txtRemarks.Text) ? (object)DBNull.Value : txtRemarks.Text);
                           cmd.Parameters.AddWithValue("@d18", vatPer);
                           cmd.Parameters.AddWithValue("@d19", (txtVATAmt.Text));
                           cmd.Parameters.AddWithValue("@d20", comboBox1.Text);
                           cmd.Parameters.AddWithValue("@d21", (Cprice.Text));
                           cmd.Parameters.AddWithValue("@d22", Convert.ToDouble(Cprice.Text) * Convert.ToDouble(txtTotalPaid.Text));
                           cmd.Parameters.AddWithValue("@d23", WID.Text);
                           cmd.ExecuteNonQuery();
                       }

                       // Update Stock_Product table
                       using (SqlCommand cmd = new SqlCommand("DELETE FROM Stock_Product WHERE StockID=@d1", con))
                       {
                           cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtST_ID.Text));
                           cmd.ExecuteNonQuery();
                       }

                       string insertQuery = "INSERT INTO Stock_Product(StockID, ProductID, Qty, Price, TotalAmount, Barcode, WID, TotalCurrency) VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8)";
                       using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                       {
                           foreach (DataGridViewRow row in DataGridView1.Rows)
                           {
                               if (!row.IsNewRow)
                               {
                                   cmd.Parameters.Clear();
                                   cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtST_ID.Text));
                                   cmd.Parameters.AddWithValue("@d2", Convert.ToInt32(row.Cells[0].Value));
                                   cmd.Parameters.AddWithValue("@d3", Convert.ToDouble(row.Cells[4].Value));
                                   cmd.Parameters.AddWithValue("@d4", Convert.ToDouble(row.Cells[5].Value));
                                   cmd.Parameters.AddWithValue("@d5", Convert.ToDouble(row.Cells[6].Value));
                                   cmd.Parameters.AddWithValue("@d6", row.Cells[3].Value.ToString());
                                   cmd.Parameters.AddWithValue("@d7", Convert.ToInt32(WID.Text));
                                   cmd.Parameters.AddWithValue("@d8", Convert.ToDouble(row.Cells[7].Value));
                                   cmd.ExecuteNonQuery();
                               }
                           }
                       }
                   }

                   MessageBox.Show("تم التحديث بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                   Reset();
               }
               catch (Exception ex)
               {
                   MessageBox.Show("Error: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
               }
           }
   */
        public static void SupplierLedgerUpdate(DateTime a, string b, decimal e, decimal f, string g, string h, string q, decimal v)
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                string cb = @"UPDATE SupplierLedgerBook 
                      SET Date=@d1, Name=@d2, Debit=@d3, Credit=@d4, 
                          Currencies=@d7, CPrice=@d8 
                      WHERE LedgerNo=@d5 AND Label=@d6";
                using (var cmd = new SqlCommand(cb, con))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.Parameters.AddWithValue("@d2", b);
                    cmd.Parameters.AddWithValue("@d3", e);
                    cmd.Parameters.AddWithValue("@d4", f);
                    cmd.Parameters.AddWithValue("@d5", g);
                    cmd.Parameters.AddWithValue("@d6", h);
                    cmd.Parameters.AddWithValue("@d7", q);
                    cmd.Parameters.AddWithValue("@d8", v);
                    // Use ExecuteNonQuery() for UPDATE operations
                    cmd.ExecuteNonQuery(); // Corrected line
                }
            }
        }

        public static void LedgerUpdate(DateTime a, string b, decimal e, decimal f, string g, string h, string i)
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
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

        private void button2_Click_1(object sender, EventArgs e)
        {
            Products p = new Products();
            p.lblSet.Text = "TOPYMENT";
            p.Reset();
            p.Show();
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblBalance_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GenerateCurrencies();
        }
        public void GenerateCurrencies()
        {
            try
            {


                // Step 2: Get the selected currency name from the ComboBox
                string selectedCurrency = comboBox1.SelectedItem.ToString().Trim();

                // Step 3: Define the SQL query to fetch both id and price based on the selected currency name
                string query = "SELECT id, Price FROM Currencies WHERE Name = @Name";

                // Step 4: Create a connection to the database
                using (SqlConnection conn = new SqlConnection(DataAccessLayer.Con()))
                {
                    try
                    {
                        conn.Open();

                        // Step 5: Create a SqlCommand to execute the query
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            // Step 6: Add the selected currency name as a parameter to prevent SQL injection
                            cmd.Parameters.AddWithValue("@Name", selectedCurrency);

                            // Step 7: Execute the query and use SqlDataReader to fetch both id and price
                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                if (rdr.Read()) // Check if at least one record is returned
                                {
                                    // Retrieve the 'id' and 'Price' from the reader
                                    int currencyId = rdr["id"] != DBNull.Value ? Convert.ToInt32(rdr["id"]) : 0;
                                    double price = rdr["Price"] != DBNull.Value ? Convert.ToDouble(rdr["Price"]) : 0;

                                    // Step 8: Assign the retrieved values to the respective TextBoxes
                                    CUID.Text = currencyId.ToString();
                                    Cprice.Text = price.ToString();
                                }

                            }
                        }
                      

                    }
                    catch (SqlException sqlEx)
                    {
                        // Step 9: Handle SQL-related exceptions specifically
                        MessageBox.Show($"A database error occurred: {sqlEx.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LogError(sqlEx); // Ensure this method logs the error details appropriately
                    }
                    catch (Exception ex)
                    {
                        // Step 10: Handle any other general exceptions
                        MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LogError(ex); // Ensure this method logs the error details appropriately
                    }

                    decimal balance = Math.Abs(Convert.ToDecimal(lblBalanceCC.Text));
                    decimal pricee = Convert.ToDecimal(Cprice.Text);
                    lblBalanceByC.Text = (balance / pricee).ToString("F2");

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"A database error occurred: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }


        }
        private void LogError(Exception ex)
        {
            // Implement logging logic here (e.g., write to a file, event log, etc.)
            // Example:
            System.IO.File.AppendAllText("error_log.txt", $"{DateTime.Now}: {ex.Message}{Environment.NewLine}");
        }
        private void LoadCurrenciesToComboBox()
        {
            // Step 1: SQL query to fetch currency names from the Currencies table
            string query = "SELECT Name FROM Currencies";

            // Step 2: Create a connection to the database
            using (SqlConnection conn = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    conn.Open();

                    // Step 3: Create a SqlCommand to execute the query
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                // Step 4: Clear any existing items in the ComboBox
                                comboBox1.Items.Clear();

                                // Step 5: Loop through the result set and add each currency to the ComboBox
                                while (reader.Read())
                                {
                                    comboBox1.Items.Add(reader["Name"].ToString());
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    // Step 6: Handle any errors
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }

            comboBox1.SelectedIndex = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Currencies currencies = new Currencies();
            currencies.lblSet.Text = "curr";
            currencies.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation checks
                if (string.IsNullOrWhiteSpace(txtSupplierID.Text))
                {
                    MessageBox.Show("الرجاء إدراج رقم المورد", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSupplierID.Focus();
                    return;
                }
                if (DataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("يجب إضافة أصناف أولا", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtDiscPer.Text))
                {
                    MessageBox.Show("الرجاء كتابة الخصم %", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDiscPer.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtVATPer.Text))
                {
                    MessageBox.Show("الرجاء كتابة مبلغ الضريبة %", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtVATPer.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtFreightCharges.Text))
                {
                    MessageBox.Show("الرجاء كتابة مصاريف الشحن", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtFreightCharges.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtOtherCharges.Text))
                {
                    MessageBox.Show("Please enter other charges", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtOtherCharges.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtRoundOff.Text))
                {
                    MessageBox.Show("الرجاء استخدام التقريب", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtRoundOff.Focus();
                    return;
                }
                if (cmbPurchaseType.SelectedIndex == 0)
                {
                    if (string.IsNullOrWhiteSpace(txtTotalPaid.Text))
                    {
                        MessageBox.Show("الرجاء كتابة إجمالي المدفوع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTotalPaid.Focus();
                        return;
                    }
                    if (Convert.ToDouble(txtTotalPaid.Text) == 0)
                    {
                        MessageBox.Show("المبلغ المدفوع يجب أن يكون أكبر من صفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTotalPaid.Focus();
                        return;
                    }
                }

                // Check if InvoiceNo already exists
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string ct = "SELECT InvoiceNo FROM Stock WHERE InvoiceNo = @d1";
                    using (SqlCommand cmd = new SqlCommand(ct, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtInvoiceNo.Text);
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                MessageBox.Show("رقم الفاتورة موجود بالفعل", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtInvoiceNo.Text = "";
                                txtInvoiceNo.Focus();
                                return;
                            }
                        }
                    }
                }

                if (cmbPurchaseType.SelectedIndex == 0)
                {

                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();
                        string cb = "INSERT INTO Stock(ST_ID, InvoiceNo, Date, PurchaseType, SupplierID, SubTotal, DiscountPer, Discount, PreviousDue, FreightCharges, OtherCharges, Total, RoundOff, GrandTotal, TotalPayment, PaymentDue, Remarks, VATPer, VATAmt,CurrencieName,Currency_price,Total_By_Pound,WID) " +
                                    "VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9, @d10, @d11, @d12, @d13, @d14, @d15, @d16, @d17, @d18, @d19, @d20, @d21, @d22, @d23)";
                        using (SqlCommand cmd = new SqlCommand(cb, con))
                        {
                            cmd.Parameters.AddWithValue("@d1", txtST_ID.Text);
                            cmd.Parameters.AddWithValue("@d2", txtInvoiceNo.Text);
                            cmd.Parameters.AddWithValue("@d3", dtpDate.Value.Date);
                            cmd.Parameters.AddWithValue("@d4", cmbPurchaseType.Text);
                            cmd.Parameters.AddWithValue("@d5", txtSup_ID.Text);
                            cmd.Parameters.AddWithValue("@d6", (txtSubTotal.Text));
                            cmd.Parameters.AddWithValue("@d7", txtDiscPer.Text);
                            cmd.Parameters.AddWithValue("@d8", (txtDisc.Text));
                            cmd.Parameters.AddWithValue("@d9", (txtPreviousDue.Text));
                            cmd.Parameters.AddWithValue("@d10", (txtFreightCharges.Text));
                            cmd.Parameters.AddWithValue("@d11", (txtOtherCharges.Text));
                            cmd.Parameters.AddWithValue("@d12", (txtTotal.Text));
                            cmd.Parameters.AddWithValue("@d13", (txtRoundOff.Text));
                            cmd.Parameters.AddWithValue("@d14", (txtGrandTotal.Text));
                            cmd.Parameters.AddWithValue("@d15", (txtTotalPaid.Text));
                            cmd.Parameters.AddWithValue("@d16", (txtBalance.Text));
                            cmd.Parameters.AddWithValue("@d17", txtRemarks.Text);
                            cmd.Parameters.AddWithValue("@d18", (txtVATPer.Text));
                            cmd.Parameters.AddWithValue("@d19", (txtVATAmt.Text));
                            cmd.Parameters.AddWithValue("@d20", (comboBox1.Text));
                            cmd.Parameters.AddWithValue("@d21", (Cprice.Text));
                            cmd.Parameters.AddWithValue("@d22", Convert.ToDouble(Cprice.Text) * Convert.ToDouble(txtTotalPaid.Text));
                            cmd.Parameters.AddWithValue("@d23", Convert.ToInt32(WID.Text));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();
                        string cb = "INSERT INTO Stock(ST_ID, InvoiceNo, Date, PurchaseType, SupplierID, SubTotal, DiscountPer, Discount, PreviousDue, FreightCharges, OtherCharges, Total, RoundOff, GrandTotal, TotalPayment, PaymentDue, Remarks, VATPer, VATAmt,CurrencieName,Currency_price,Total_By_Pound,WID) " +
                                    "VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9, @d10, @d11, @d12, @d13, @d14, @d15, @d16, @d17, @d18, @d19, @d20, @d21, @d22,@d23)";
                        using (SqlCommand cmd = new SqlCommand(cb, con))
                        {
                            cmd.Parameters.AddWithValue("@d1", txtST_ID.Text);
                            cmd.Parameters.AddWithValue("@d2", txtInvoiceNo.Text);
                            cmd.Parameters.AddWithValue("@d3", dtpDate.Value.Date);
                            cmd.Parameters.AddWithValue("@d4", cmbPurchaseType.Text);
                            cmd.Parameters.AddWithValue("@d5", txtSup_ID.Text);
                            cmd.Parameters.AddWithValue("@d6", (txtSubTotal.Text));
                            cmd.Parameters.AddWithValue("@d7", txtDiscPer.Text);
                            cmd.Parameters.AddWithValue("@d8", (txtDisc.Text));
                            cmd.Parameters.AddWithValue("@d9", (txtPreviousDue.Text));
                            cmd.Parameters.AddWithValue("@d10", (txtFreightCharges.Text));
                            cmd.Parameters.AddWithValue("@d11", (txtOtherCharges.Text));
                            cmd.Parameters.AddWithValue("@d12", (txtTotal.Text));
                            cmd.Parameters.AddWithValue("@d13", (txtRoundOff.Text));
                            cmd.Parameters.AddWithValue("@d14", (txtGrandTotal.Text));
                            cmd.Parameters.AddWithValue("@d15", (txtTotalPaid.Text));
                            cmd.Parameters.AddWithValue("@d16", (txtBalance.Text));
                            cmd.Parameters.AddWithValue("@d17", txtRemarks.Text);
                            cmd.Parameters.AddWithValue("@d18", (txtVATPer.Text));
                            cmd.Parameters.AddWithValue("@d19", (txtVATAmt.Text));
                            cmd.Parameters.AddWithValue("@d20", (comboBox1.Text));
                            cmd.Parameters.AddWithValue("@d21", (Cprice.Text));
                            cmd.Parameters.AddWithValue("@d22", 0);
                            cmd.Parameters.AddWithValue("@d23", Convert.ToInt32(WID.Text));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                // Insert into Stock_Product table
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string cb1 = "INSERT INTO Stock_Product(StockID, ProductID, Qty, Price, TotalAmount, Barcode,WID) VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7)";
                    using (SqlCommand cmd = new SqlCommand(cb1, con))
                    {
                        foreach (DataGridViewRow row in DataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtST_ID.Text));
                                cmd.Parameters.AddWithValue("@d2", Convert.ToInt32(row.Cells[0].Value));
                                cmd.Parameters.AddWithValue("@d3", Convert.ToDouble(row.Cells[4].Value));
                                cmd.Parameters.AddWithValue("@d4", Convert.ToDouble(row.Cells[5].Value));
                                cmd.Parameters.AddWithValue("@d5", Convert.ToDouble(row.Cells[6].Value));
                                cmd.Parameters.AddWithValue("@d6", row.Cells[3].Value.ToString());
                                cmd.Parameters.AddWithValue("@d7", Convert.ToInt32(WID.Text));
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                // Update Temp_Stock table or insert new rows
                foreach (DataGridViewRow row in DataGridView1.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                        {
                            con.Open();
                            string ctx = "SELECT ProductID FROM Temp_Stock WHERE ProductID = @d1 AND Barcode = @d2";
                            using (SqlCommand cmd = new SqlCommand(ctx, con))
                            {
                                cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(row.Cells[0].Value));
                                cmd.Parameters.AddWithValue("@d2", row.Cells[3].Value.ToString());
                                using (SqlDataReader rdr = cmd.ExecuteReader())
                                {
                                    if (rdr.Read())
                                    {
                                        // Update the existing record
                                        rdr.Close();
                                        string cb2 = "UPDATE Temp_Stock SET Qty = Qty + @qty WHERE ProductID = @d1 AND Barcode = @d2 AND WID = @d3";
                                        using (SqlCommand cmdUpdate = new SqlCommand(cb2, con))
                                        {
                                            cmdUpdate.Parameters.AddWithValue("@qty", Convert.ToDouble(row.Cells[4].Value));
                                            cmdUpdate.Parameters.AddWithValue("@d1", Convert.ToInt32(row.Cells[0].Value));
                                            cmdUpdate.Parameters.AddWithValue("@d2", row.Cells[3].Value.ToString());
                                            cmdUpdate.Parameters.AddWithValue("@d3",WID.Text);
                                            cmdUpdate.ExecuteNonQuery();
                                        }
                                    }
                                    else
                                    {
                                        // Insert a new record
                                        rdr.Close();
                                        string cb3 = "INSERT INTO Temp_Stock(ProductID, Qty, Barcode,WID) VALUES (@d1, @d2, @d3, @d4)";
                                        using (SqlCommand cmdInsert = new SqlCommand(cb3, con))
                                        {
                                            cmdInsert.Parameters.AddWithValue("@d1", Convert.ToInt32(row.Cells[0].Value));
                                            cmdInsert.Parameters.AddWithValue("@d2", Convert.ToDouble(row.Cells[4].Value));
                                            cmdInsert.Parameters.AddWithValue("@d3", row.Cells[3].Value.ToString());
                                            cmdInsert.Parameters.AddWithValue("@d4",WID.Text);
                                            cmdInsert.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Ledger operations based on purchase type
                if (cmbPurchaseType.SelectedIndex == 1)
                {
                    LedgerSave(dtpDate.Value.Date, "اجل" + "/" + txtSupplierName.Text, txtInvoiceNo.Text, "فاتورة مشتريات", Convert.ToDecimal(txtSubTotal.Text) * Convert.ToDecimal(Cprice.Text), 0, txtSupplierID.Text);
                }
                if (cmbPurchaseType.SelectedIndex == 0)
                {
                    LedgerSave(dtpDate.Value.Date, "كاش" + "/" + txtSupplierName.Text, txtInvoiceNo.Text, "فاتورة مشتريات", Convert.ToDecimal(txtSubTotal.Text) * Convert.ToDecimal(Cprice.Text), Convert.ToDecimal(txtTotalPaid.Text) * Convert.ToDecimal(Cprice.Text), txtSupplierID.Text);

                }
                if (cmbPurchaseType.SelectedIndex == 1)
                {
                    SupplierLedgerSave(dtpDate.Value.Date, "اجل", txtInvoiceNo.Text, "فاتورة مشتريات", Convert.ToDecimal(txtSubTotal.Text), 0, txtSupplierID.Text, comboBox1.Text, Convert.ToDecimal(Cprice.Text));
                }
                if (cmbPurchaseType.SelectedIndex == 0)
                {
                    SupplierLedgerSave(dtpDate.Value.Date, "كاش", txtInvoiceNo.Text, "فاتورة مشتريات", Convert.ToDecimal(txtSubTotal.Text), Convert.ToDecimal(txtTotalPaid.Text), txtSupplierID.Text, comboBox1.Text, Convert.ToDecimal(Cprice.Text));

                }
                /*                if (cmbPurchaseType.SelectedIndex == 0)
                                {
                                    if (Convert.ToDecimal(txtTransactionAmount.Text) > 0)
                                    {
                                        LedgerSave(dtpTranactionDate.Value.Date, "نقدا", txtTransactionNo.Text, "سند دفع", Convert.ToDecimal(txtTransactionAmount.Text), 0, txtSupplierID.Text, "");
                                    }
                                    else
                                    {
                                        LedgerSave(dtpTranactionDate.Value.Date, "نقدا", txtTransactionNo.Text, "سند دفع", 0, Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text)), txtSupplierID.Text, "");
                                    }
                                }
                                if (cmbPurchaseType.SelectedIndex == 1)
                                {
                                    if (Convert.ToDecimal(txtTransactionAmount.Text) > 0)
                                    {
                                        LedgerSave(dtpTranactionDate.Value.Date, "شيك", txtTransactionNo.Text, "سند دفع", Convert.ToDecimal(txtTransactionAmount.Text), 0, txtSupplierID.Text, "");
                                    }
                                    else
                                    {
                                        LedgerSave(dtpTranactionDate.Value.Date, "شيك", txtTransactionNo.Text, "سند دفع", 0, Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text)), txtSupplierID.Text, "");
                                    }
                                }*/
                /*                if (cmbPurchaseType.SelectedIndex == 0)
                                {
                                    if (Convert.ToDecimal(txtTransactionAmount.Text) > 0)
                                    {
                                        SupplierLedgerSave(dtpTranactionDate.Value.Date, "نقدا", txtTransactionNo.Text, "سند دفع", Convert.ToDecimal(txtTransactionAmount.Text), 0, txtSupplierID.Text);
                                    }
                                    else
                                    {
                                        SupplierLedgerSave(dtpTranactionDate.Value.Date, "نقدا", txtTransactionNo.Text, "سند دفع", 0, Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text)), txtSupplierID.Text);
                                    }
                                }
                                if (cmbPurchaseType.SelectedIndex == 1)
                                {
                                    if (Convert.ToDecimal(txtTransactionAmount.Text) > 0)
                                    {
                                        SupplierLedgerSave(dtpTranactionDate.Value.Date, "شيك", txtTransactionNo.Text, "سند دفع", Convert.ToDecimal(txtTransactionAmount.Text), 0, txtSupplierID.Text);
                                    }
                                    else
                                    {
                                        SupplierLedgerSave(dtpTranactionDate.Value.Date, "شيك", txtTransactionNo.Text, "سند دفع", 0, Math.Abs(Convert.ToDecimal(txtTransactionAmount.Text)), txtSupplierID.Text);
                                    }
                                }*/

                MessageBox.Show("تم الحفظ بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
                print();
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void txtProductName_TextChanged(object sender, EventArgs e)
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

        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
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
                    txtPricePerQty.Text = dr.Cells[4].Value.ToString();
                    txtQty.Focus();

                    dgw.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // الخطوة 2: الحصول على اسم المخزن المحدد من الـ ComboBox
                string selectedWarehouse = comboBox2.SelectedItem.ToString();

                // الخطوة 3: تعريف استعلام SQL لجلب WID و WarehouseName بناءً على اسم المخزن المحدد
                string query = "SELECT WID, WarehouseName FROM Warehouses WHERE WarehouseName = @Name";

                // الخطوة 4: إنشاء اتصال بقاعدة البيانات
                using (SqlConnection conn = new SqlConnection(DataAccessLayer.Con()))
                {
                    try
                    {
                        conn.Open();

                        // الخطوة 5: إنشاء SqlCommand لتنفيذ الاستعلام
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            // الخطوة 6: إضافة اسم المخزن المحدد كمعامل لمنع هجمات SQL Injection
                            cmd.Parameters.AddWithValue("@Name", selectedWarehouse);

                            // الخطوة 7: تنفيذ الاستعلام واستخدام SqlDataReader لجلب WID و WarehouseName
                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                if (rdr.Read()) // التحقق من وجود سجل واحد على الأقل
                                {
                                    // استرجاع 'WID' و 'WarehouseName' من القارئ
                                    int wid = rdr["WID"] != DBNull.Value ? Convert.ToInt32(rdr["WID"]) : 0;
                                    string warehouseName = rdr["WarehouseName"] != DBNull.Value ? rdr["WarehouseName"].ToString() : string.Empty;

                                    // الخطوة 8: تعيين القيم المسترجعة إلى الـ TextBoxes الخاصة بك
                                    WID.Text = wid.ToString();
                                    // إذا كان لديك TextBox لاسم المخزن، يمكنك تعيينه هنا
                                    // مثلاً: WarehouseNameTextBox.Text = warehouseName;
                                }
                                else
                                {
                                    MessageBox.Show("لم يتم العثور على المخزن المحدد.", "معلومات", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        // الخطوة 9: معالجة استثناءات SQL الخاصة
                        MessageBox.Show($"حدث خطأ في قاعدة البيانات: {sqlEx.Message}", "خطأ قاعدة بيانات", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        // الخطوة 10: معالجة أي استثناءات عامة أخرى
                        MessageBox.Show($"حدث خطأ غير متوقع: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ في قاعدة البيانات: {ex.Message}", "خطأ قاعدة بيانات", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void comboBoxGenerate()
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT WarehouseName FROM [dbo].[Warehouses]", con);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    comboBox2.Items.Clear();
                    while (rdr.Read()) // Loops through all available rows
                    {
                        comboBox2.Items.Add(rdr["WarehouseName"].ToString());
                    }

                }
            }
        }

        private void txtSupplierName_TextChanged(object sender, EventArgs e)
        {
          
        }

        private void listBoxSuppliers_MouseDoubleClick(object sender, MouseEventArgs e)
        {
        }

        private void listBoxSuppliers_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void metroListView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
           
        }
        private void PositionDataGridView(int x,int y)
        {
           
        }

        private void metroListView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtContactNo_TextChanged(object sender, EventArgs e)
        {
            //54, 290
         
        }

        private void searchtime_Tick(object sender, EventArgs e)
        {
        
        }

        private void txtSupplierName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {

        }

        private void txtSupplierName_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            try
            {
                string a = "";
                string b = "";
                string c = "";
                txtSup_ID.Text = "";

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT  RTRIM(ID),RTRIM(SupplierID),RTRIM([Name]), RTRIM(Address),RTRIM(City),RTRIM(State),RTRIM(ZipCode), RTRIM(ContactNo) FROM Supplier WHERE Name=@d1";
                        cmd.Parameters.AddWithValue("@d1", txtSupplierName.Text);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                txtSup_ID.Text = rdr.GetString(0);
                                txtSupplierID.Text =  rdr.GetString(1) ?? string.Empty;
                                txtSupplierName.Text = rdr.GetString(2) ?? string.Empty;
                                txtAddress.Text = rdr.GetString(3) ?? string.Empty;
                                txtCity.Text = rdr.GetString(4) ?? string.Empty;
                                txtContactNo.Text = rdr.GetString(7) ?? string.Empty;
                                GetSupplierBalance();
                                a = rdr.GetString(1);
                                b = rdr.GetString(2);
                                c = rdr.GetString(3);
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
        private void fillSupplers()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlDataAdapter adp = new SqlDataAdapter())
                    {
                        adp.SelectCommand = new SqlCommand("SELECT RTRIM(Name) FROM Supplier ", con);
                        DataSet ds = new DataSet("ds");
                        adp.Fill(ds);
                        System.Data.DataTable dtable = ds.Tables[0];
                        txtSupplierName.Items.Clear();
                        foreach (DataRow drow in dtable.Rows)
                        {
                            txtSupplierName.Items.Add(drow[0].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtContactNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string a = "";
                string b = "";
                string c = "";
                txtSup_ID.Text = "";

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT RTRIM(ID),RTRIM(SupplierID),RTRIM([Name]), RTRIM(Address),RTRIM(City),RTRIM(State),RTRIM(ZipCode), RTRIM(ContactNo) FROM Supplier WHERE ContactNo=@d1";
                        cmd.Parameters.AddWithValue("@d1", txtContactNo.Text);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                txtSup_ID.Text = rdr.GetString(0);
                                txtSupplierID.Text = rdr.GetString(1) ?? string.Empty;
                                txtSupplierName.Text = rdr.GetString(2) ?? string.Empty;
                                txtAddress.Text = rdr.GetString(3) ?? string.Empty;
                                txtCity.Text = rdr.GetString(4) ?? string.Empty;
                                txtContactNo.Text = rdr.GetString(7) ?? string.Empty;
                                GetSupplierBalance();
                                a = rdr.GetString(1);
                                b = rdr.GetString(2);
                                c = rdr.GetString(3);
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

        private void fillSupplersPhone()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlDataAdapter adp = new SqlDataAdapter())
                    {
                        adp.SelectCommand = new SqlCommand("SELECT RTRIM(ContactNo) FROM Supplier ", con);
                        DataSet ds = new DataSet("ds");
                        adp.Fill(ds);
                        System.Data.DataTable dtable = ds.Tables[0];
                        txtContactNo.Items.Clear();
                        foreach (DataRow drow in dtable.Rows)
                        {
                            txtContactNo.Items.Add(drow[0].ToString());
                        }
                    }
                }
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
                    MessageBox.Show("الرجاء إدخال المنتج", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProductCode.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtBarcode.Text))
                {
                    MessageBox.Show("الرجاء إدراج الباركود للصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBarcode.Focus();
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
                    MessageBox.Show("الكمية يجب أن تكون أكبر من صفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQty.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtPricePerQty.Text))
                {
                    MessageBox.Show("الرجاء إدخال سعر الوحدة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPricePerQty.Focus();
                    return;
                }

                // Remove selected rows from the DataGridView
                foreach (DataGridViewRow row in DataGridView1.SelectedRows)
                {
                    DataGridView1.Rows.Remove(row);
                }

                // Add the updated values to the DataGridView
                DataGridView1.Rows.Add(
                    txtProductID.Text,
                    txtProductCode.Text,
                    txtProductName.Text,
                    txtBarcode.Text,
                    txtQty.Text,
                    txtPricePerQty.Text,
                    txtTotalAmount.Text,
                    textBox2.Text
                );

                // Update the SubTotal
                txtSubTotal.Text = Math.Round(SubTotal(), 2).ToString();

                // Clear input fields
                Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (DataGridView1.Rows.Count==0)
            {
                MessageBox.Show("الرجاء ادخال اصناف اولا . ", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;

            }
            print();
        }

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}

