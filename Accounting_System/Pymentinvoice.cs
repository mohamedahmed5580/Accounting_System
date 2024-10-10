using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;
using Pharmacy.DL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Accounting_System.Pymentinvoice;

namespace Accounting_System
{
    public partial class Pymentinvoice : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        private static Pymentinvoice _instance;
        public static Pymentinvoice instance;
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
            instance = this;
        }

        private void Pymentinvoice_Load(object sender, EventArgs e)
        {
            cmbPurchaseType.SelectedIndex = 0;
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
            num1 = num7 + Val(txtFreightCharges.Text) + Val(txtOtherCharges.Text) + Val(txtPreviousDue.Text) + Val(txtVATAmt.Text);
            num1 = Math.Round(num1, 2);
            txtTotal.Text = num1.ToString();
            num2 = Math.Round(num1, 1);
            num3 = num2 - num1;
            num3 = Math.Round(num3, 2);
            txtRoundOff.Text = num3.ToString();
            num4 = Val(txtTotal.Text) + Val(txtRoundOff.Text);
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
                txtPreviousDue.Text = num1.ToString();
                lblBalance.Text = Math.Abs(Convert.ToDecimal(lblBalance.Text)).ToString();
                lblBalance.Text = lblBalance.Text + " " + Conversion.str;
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

                // Insert into Stock table
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string cb = "INSERT INTO Stock(ST_ID, InvoiceNo, Date, PurchaseType, SupplierID, SubTotal, DiscountPer, Discount, PreviousDue, FreightCharges, OtherCharges, Total, RoundOff, GrandTotal, TotalPayment, PaymentDue, Remarks, VATPer, VATAmt,CurrencieName) " +
                                "VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d9, @d10, @d11, @d12, @d13, @d14, @d15, @d16, @d17, @d18, @d19, @d20)";
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
                        cmd.Parameters.AddWithValue("@d15",(txtTotalPaid.Text));
                        cmd.Parameters.AddWithValue("@d16", (txtBalance.Text));
                        cmd.Parameters.AddWithValue("@d17", txtRemarks.Text);
                        cmd.Parameters.AddWithValue("@d18", (txtVATPer.Text));
                        cmd.Parameters.AddWithValue("@d19", (txtVATAmt.Text));
                        cmd.Parameters.AddWithValue("@d20", (Cprice.Text));
                        cmd.ExecuteNonQuery();
                    }
                }

                // Insert into Stock_Product table
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string cb1 = "INSERT INTO Stock_Product(StockID, ProductID, Qty, Price, TotalAmount, Barcode,CurrencieName) VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d8)";
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
                                cmd.Parameters.AddWithValue("@d8", row.Cells[7].Value);
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
                                        string cb2 = "UPDATE Temp_Stock SET Qty = Qty + @qty WHERE ProductID = @d1 AND Barcode = @d2";
                                        using (SqlCommand cmdUpdate = new SqlCommand(cb2, con))
                                        {
                                            cmdUpdate.Parameters.AddWithValue("@qty", Convert.ToDouble(row.Cells[4].Value));
                                            cmdUpdate.Parameters.AddWithValue("@d1", Convert.ToInt32(row.Cells[0].Value));
                                            cmdUpdate.Parameters.AddWithValue("@d2", row.Cells[3].Value.ToString());
                                            cmdUpdate.ExecuteNonQuery();
                                        }
                                    }
                                    else
                                    {
                                        // Insert a new record
                                        rdr.Close();
                                        string cb3 = "INSERT INTO Temp_Stock(ProductID, Qty, Barcode) VALUES (@d1, @d2, @d3)";
                                        using (SqlCommand cmdInsert = new SqlCommand(cb3, con))
                                        {
                                            cmdInsert.Parameters.AddWithValue("@d1", Convert.ToInt32(row.Cells[0].Value));
                                            cmdInsert.Parameters.AddWithValue("@d2", Convert.ToDouble(row.Cells[4].Value));
                                            cmdInsert.Parameters.AddWithValue("@d3", row.Cells[3].Value.ToString());
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
                    SupplierLedgerSave(dtpDate.Value.Date, txtSupplierID.Text, txtInvoiceNo.Text, "مورد فاتورة الشراء", Convert.ToDecimal(txtGrandTotal.Text), 0, txtBalance.Text);
                }
                if (cmbPurchaseType.SelectedIndex == 0)
                {
                    SupplierLedgerSave(dtpDate.Value.Date, txtSupplierID.Text, txtInvoiceNo.Text, "مورد فاتورة الشراء",Convert.ToDecimal(txtGrandTotal.Text), Convert.ToDecimal(txtTotalPaid.Text), txtBalance.Text);
                    
                }

                MessageBox.Show("تم الحفظ بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        public void SupplierLedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g)
        {
            con.Open();
            string cb = "insert into SupplierLedgerBook(Date, Name, LedgerNo, Label,Debit,Credit,PartyID) Values (@d1,@d2,@d3,@d4,@d5,@d6,@d7)";
            SqlCommand cmd = new SqlCommand(cb);
            cmd.Parameters.AddWithValue("@d1", a);
            cmd.Parameters.AddWithValue("@d2", b);
            cmd.Parameters.AddWithValue("@d3", c);
            cmd.Parameters.AddWithValue("@d4", d);
            cmd.Parameters.AddWithValue("@d5", e);
            cmd.Parameters.AddWithValue("@d6", f);
            cmd.Parameters.AddWithValue("@d7", g);
            cmd.Connection = con;
            cmd.ExecuteReader();
            con.Close();
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
            currencies.Text = "";
        }

        public void Reset()
        {
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
            currencies.Text = "";
            cmbPurchaseType.SelectedIndex = 1;
            dtpDate.Text = DateTime.Today.ToString();
            btnSave.Enabled = true;
            btnDelete.Enabled = false;
            DataGridView1.Enabled = true;
            btnAdd.Enabled = true;
            pnlCalc.Enabled = true;
            lblBalance.Text = "0";
            txtTotalPaid.ReadOnly = true;
            txtTotalPaid.Enabled = false;
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
                // Use a single connection for the entire operation
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // Check if there are related purchase returns
                    string checkQuery = "SELECT ST_ID FROM Stock INNER JOIN PurchaseReturn ON PurchaseReturn.PurchaseID = Stock.ST_ID WHERE ST_ID = @d1";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@d1", Val(txtST_ID.Text));
                        using (SqlDataReader rdr = checkCmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                MessageBox.Show("لا يمكن الحذف حيث يوجد مرتجعات شراء ذات علاقة", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                    string updateQuery = "UPDATE Temp_Stock SET Qty = Qty - @Qty WHERE ProductID = @d1 AND Barcode = @d2";
                                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                                    {
                                        updateCmd.Parameters.AddWithValue("@Qty", Val(row.Cells[4].Value.ToString()));
                                        updateCmd.Parameters.AddWithValue("@d1", Val(row.Cells[0].Value.ToString()));
                                        updateCmd.Parameters.AddWithValue("@d2", row.Cells[3].Value.ToString());
                                        updateCmd.ExecuteNonQuery();
                                    }
                                }
                            }

                            // Perform ledger operations
                            // LedgerDelete(txtInvoiceNo.Text, "دفع لــ " + txtSupplierName.Text + "");
                            // LedgerDelete(txtInvoiceNo.Text, "فاتورة مشتريات");
                            // SupplierLedgerDelete(txtInvoiceNo.Text);
                            // LogFunc(lblUser.Text, "deleted the purchase record having Invoice No. '" + txtInvoiceNo.Text + "'");

                            MessageBox.Show("تم الحذف بنجاح", "السجلات", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            FileSystem.Reset();
                        }
                        else
                        {
                            MessageBox.Show("لا يوجد سجلات", "عذراً", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            FileSystem.Reset();
                        }
                    }
                    Reset();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                txtTotalAmount.Text = dr.Cells[6]?.Value?.ToString() ?? string.Empty;
                currencies.Text = dr.Cells[7]?.Value?.ToString() ?? string.Empty;
                Cprice.Text = dr.Cells[7]?.Value?.ToString() ?? string.Empty;
                btnAdd.Enabled = true;
                btnRemove.Enabled = true;
                btnDelete.Enabled = true;
                



            }
        }


        private void txtPricePerQty_TextChanged(object sender, EventArgs e)
        {
            double i = 0d;
            i = (double)(Val(txtQty.Text) * Val(txtPricePerQty.Text));
            i = Math.Round(i, 2);
            txtTotalAmount.Text = i.ToString();
        }


        private void txtQty_TextChanged(object sender, EventArgs e)
        {
            double i = 0d;
            i = (double)(Val(txtQty.Text) * Val(txtPricePerQty.Text));
            i = Math.Round(i, 2);
            txtTotalAmount.Text = i.ToString();
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
                    DataGridView1.Rows.Add(txtProductID.Text, txtProductCode.Text, txtProductName.Text, txtBarcode.Text, txtQty.Text, txtPricePerQty.Text, txtTotalAmount.Text, currencies.Text);
                    txtSubTotal.Text = Math.Round(SubTotal(), 2).ToString();
                    Clear();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtQty.Text))
                {
                    btnAdd.Enabled = true;
                    return;
                }
                // Check if the product already exists in the DataGridView
                bool rowUpdated = false;

                foreach (DataGridViewRow r in DataGridView1.Rows)
                {
                    // Check by ProductCode and Barcode for an existing product
                    if (r.Cells[1].Value.ToString() == txtProductCode.Text && r.Cells[3].Value.ToString() == txtBarcode.Text)
                    {
                        // Update the existing row
                        r.Cells[4].Value = Convert.ToDouble(txtQty.Text);       // Quantity
                        r.Cells[6].Value =  Convert.ToDouble(txtTotalAmount.Text); // Total Amount
                        r.Cells[7].Value = currencies.Text; // Currency Name

                        // Update other fields if necessary
                        r.Cells[2].Value = txtProductName.Text;   // Product Name
                        r.Cells[5].Value = Convert.ToDouble(txtPricePerQty.Text);  // Price per Qty

                        rowUpdated = true;
                        break;
                    }
                }

                if (!rowUpdated)
                {
                    DataGridView1.Rows.Add(
                          txtProductID.Text,        // Product ID
                          txtProductCode.Text,      // Product Code
                          txtProductName.Text,      // Product Name
                          txtBarcode.Text,          // Barcode
                          txtQty.Text,              // Quantity
                          txtPricePerQty.Text,      // Price per Quantity
                          txtTotalAmount.Text,      // Total Amount
                          currencies.Text           // Currency Name
                    );
                }


                //DataGridView1.Rows.Add(txtProductID.Text, txtProductCode.Text, txtProductName.Text, txtBarcode.Text, txtQty.Text, txtPricePerQty.Text, txtTotalAmount.Text,currencies.Text);
                txtSubTotal.Text = Math.Round(SubTotal(), 2).ToString();
                Cprice.Text = currencies.Text;
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
                // Validation checks (similar to btnSave_Click)
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
                }

                // Update Stock table
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "UPDATE Stock SET ST_ID=@d1, Date=@d3, PurchaseType=@d4, SupplierID=@d5, SubTotal=@d6, DiscountPer=@d7, Discount=@d8, PreviousDue=@d9, " +
                                   "FreightCharges=@d10, OtherCharges=@d11, Total=@d12, RoundOff=@d13, GrandTotal=@d14, TotalPayment=@d15, PaymentDue=@d16, Remarks=@d17, " +
                                   "VATPer=@d18, VATAmt=@d19 ,CurrencieName=@d20 WHERE InvoiceNo=@d2";
                    using (SqlCommand cmd = new SqlCommand(query, con))
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
                        cmd.Parameters.AddWithValue("@d20", (Cprice.Text));
                        cmd.ExecuteNonQuery();
                    }
                }

                // Update Stock_Product table and adjust quantities in Temp_Stock
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string deleteQuery = "DELETE FROM Stock_Product WHERE StockID=@d1";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtST_ID.Text));
                        cmd.ExecuteNonQuery();
                    }

                    string insertQuery = "INSERT INTO Stock_Product(StockID, ProductID, Qty, Price, TotalAmount, Barcode ,CurrencieName) VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7)";
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
                                // Insert the updated row into Stock_Product
                                cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtST_ID.Text));
                                cmd.Parameters.AddWithValue("@d2", productId);
                                cmd.Parameters.AddWithValue("@d3", newQty);
                                cmd.Parameters.AddWithValue("@d4", Convert.ToDouble(row.Cells[5].Value));
                                cmd.Parameters.AddWithValue("@d5", Convert.ToDouble(row.Cells[6].Value));
                                cmd.Parameters.AddWithValue("@d6", row.Cells[3].Value.ToString());
                                cmd.Parameters.AddWithValue("@d7", row.Cells[7].Value.ToString());
                                cmd.ExecuteNonQuery();

                                // Update the quantity in Temp_Stock
                                string updateTempStockQuery = qtyDifference > 0
                                    ? "UPDATE Temp_Stock SET Qty = Qty + @qtyDiff WHERE ProductID = @d1 AND Barcode = @d2"
                                    : "UPDATE Temp_Stock SET Qty = Qty - @qtyDiff WHERE ProductID = @d1 AND Barcode = @d2";

                                using (SqlCommand cmdUpdate = new SqlCommand(updateTempStockQuery, con))
                                {
                                    cmdUpdate.Parameters.AddWithValue("@qtyDiff", Math.Abs(qtyDifference));
                                    cmdUpdate.Parameters.AddWithValue("@d1", productId);
                                    cmdUpdate.Parameters.AddWithValue("@d2", row.Cells[3].Value.ToString());
                                    cmdUpdate.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }

                MessageBox.Show("تم التحديث بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Products p = new Products();
            p.lblSet.Text = "TOPYMENT";
            p.Reset();
            p.Show();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
         
        }

     

        private void GroupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void currencies_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clear();
        }
    }
}
