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
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Accounting_System.Pymentinvoice;

namespace Accounting_System
{
    public partial class SalesReturn : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());


        private static SalesReturn _instance;
        public static SalesReturn instance;
        public static SalesReturn Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new SalesReturn();
                }
                return _instance;
            }
        }

        public SalesReturn()
        {
            InitializeComponent();
            txtReturnQty.TextChanged += new EventHandler(txtRetuenQty_TextChanged);
            instance = this;
        }
        private void SalesReturn_Load(object sender, EventArgs e)
        {
        }
        private void txtSRNO_TextChanged(object sender, EventArgs e)
        {
        }
        // Install-Package Microsoft.VisualBasic
        private string str;
        private string st;

        private string GenerateID()
        {
            string value = "0000";
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    // Fetch the latest ID from the database
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 SR_ID FROM SalesReturn ORDER BY SR_ID DESC", con))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                value = rdr["SR_ID"].ToString();
                            }
                        }
                    }

                    // Increase the ID by 1
                    int intValue = int.Parse(value) + 1;
                    value = intValue.ToString("D4"); // Format the integer as a 4-digit number, padded with zeros
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
            }
            return value;
        }


        public void Reset()
        {
            txtSRNO.Text = "";
            txtSRID.Text = "";
            dtpSRDate.Text = DateTime.Today.ToString("d");
            dtpSalesDate.Text = DateTime.Today.ToString("d");
            txtSalesID.Text = "";
            txtSalesInvoiceNo.Text = "";
            txtCustomerID.Text = "";
            txtCustomerName.Text = "";
            txtcust_ID.Text = "";
            txtGrandTotal.Text = "";
            btnSave.Enabled = true;
            btnDelete.Enabled = false;
            DataGridView1.Enabled = true;
            btnAdd.Enabled = true;
            pnlCalc.Enabled = true;
            btnRemove.Enabled = false;
            DataGridView1.Rows.Clear();
            DataGridView2.Rows.Clear();
            Clear();
            btnSelection.Enabled = true;
            lblSet.Text = "";
            auto();
        }

        public void auto()
        {
            try
            {
                txtSRID.Text = GenerateID();
                txtSRNO.Text = "SR-" + GenerateID();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnSelection_Click(object sender, EventArgs e)
        {
            /*frmSalesInvoiceRecord.lblSet.Text = "Sales Return";
            frmSalesInvoiceRecord.Reset();
            frmSalesInvoiceRecord.ShowDialog();*/
        }


        public double GrandTotal()
        {

    
                double sum = 0;
                try
                {
                    foreach (DataGridViewRow r in DataGridView1.Rows)
                    {
                        if (r.Cells[10].Value != null)
                        {
                            sum += Convert.ToDouble(r.Cells[10].Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return sum;

        }

        public void Clear()
        {
            txtBarcode.Text = "";
            txtProductCode.Text = "";
            txtProductName.Text = "";
            txtQty.Text = "";
            txtPrice.Text = "";
            txtDiscountPer.Text = "";
            txtDiscountAmount.Text = "";
            txtVATPer.Text = "";
            txtVATAmount.Text = "";
            txtReturnQty.Text = "";
            txtTotalAmount.Text = "";
            txtCostPrice.Text = "";
            txtMargin.Text = "";
            btnAdd.Enabled = true;
            btnRemove.Enabled = false;
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtProductName.Text == "")
                {
                    MessageBox.Show("الرجاء إدراج كود الصنف", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProductName.Focus();
                    return;
                }
                if (txtBarcode.Text == "")
                {
                    MessageBox.Show("الرجاء كتابة الباركود", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBarcode.Focus();
                    return;
                }
                if (txtQty.Text == "")
                {
                    MessageBox.Show("الرجاء كتابة الكمية", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQty.Focus();
                    return;
                }

                if (Val(txtQty.Text) == 0)
                {
                    MessageBox.Show("الكمية يجب أن تكون أكبر من صفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQty.Focus();
                    return;
                }
                // txtReturnQty.Text = 1
                if (txtReturnQty.Text == "")
                {
                    MessageBox.Show("الرجاء كتابة الكمية المرتجعة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtReturnQty.Focus();
                    return;
                }
                if (Val(txtReturnQty.Text) == 0)
                {
                    MessageBox.Show("الكمية المرتجعة يجب أن تكون أكبر من صفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtReturnQty.Focus();
                    return;
                }
                if (Val(txtReturnQty.Text) > Val(txtQty.Text))
                {
                    MessageBox.Show("الكمية المرتجعة لا يجب أن تكون أكبر من الكمية المباعة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtReturnQty.Text = "";
                    txtReturnQty.Focus();
                    return;
                }
                if (DataGridView1.Rows.Count == 0)
                {
                    DataGridView1.Rows.Add(txtProductCode.Text, txtProductName.Text, txtBarcode.Text, Val(txtPrice.Text), Val(txtQty.Text), Val(txtDiscountPer.Text), Val(txtDiscountAmount.Text), Val(txtVATPer.Text), Val(txtVATAmount.Text), Val(txtReturnQty.Text), Val(txtTotalAmount.Text), Val(txtProductID.Text), Val(txtCostPrice.Text), Val(txtMargin.Text) * Val(txtReturnQty.Text));
                    double k = 0d;
                    k = GrandTotal();
                    k = Math.Round(k, 2);
                    txtGrandTotal.Text = k.ToString();
                    Clear();
                    return;
                }
                foreach (DataGridViewRow row in DataGridView1.Rows)
                {
                    if (txtBarcode.Text == row.Cells[2].Value.ToString())
                    {
                        MessageBox.Show("هذا الباركود مضاف مسبقا في شبكة البيانات", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtBarcode.Focus();
                        return;
                    }
                    if (txtBarcode.Text == row.Cells[2].Value & txtProductID.Text == row.Cells[11].Value)
                    {
                        MessageBox.Show("هذا الصنف مضاف مسبقا في شبكة البيانات", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtBarcode.Focus();
                        return;
                    }
                }
                DataGridView1.Rows.Add(txtProductCode.Text, txtProductName.Text, txtBarcode.Text, Val(txtPrice.Text), Val(txtQty.Text), Val(txtDiscountPer.Text), Val(txtDiscountAmount.Text), Val(txtVATPer.Text), Val(txtVATAmount.Text), Val(txtReturnQty.Text), Val(txtTotalAmount.Text), Val(txtProductID.Text), Val(txtCostPrice.Text), Val(txtMargin.Text) * Val(txtReturnQty.Text));
                double k1 = 0d;
                k1 = GrandTotal();
                k1 = Math.Round(k1, 2);
                txtGrandTotal.Text = k1.ToString();
                Clear();
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
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
                txtGrandTotal.Text = k.ToString("F2");
                btnRemove.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void txtRetuenQty_TextChanged(object sender, EventArgs e)
        {
            double i = 0d;
            i = (double)(Val(txtReturnQty.Text) * Val(txtPrice.Text));
            i = Math.Round(i, 2);
            txtTotalAmount.Text = i.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtSalesInvoiceNo.Text.Trim().Length == 0)
                {
                    MessageBox.Show("الرجاء اختيار فاتورة المبيعات", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSalesInvoiceNo.Focus();
                    return;
                }

                if (DataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("عذراً لا يوجد أصناف مرتجعة في شبكة البيانات", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // Check if the SalesID already exists in SalesReturn
                    string checkQuery = "select SalesID from SalesReturn where SalesID=@d1";
                    using (SqlCommand cmd = new SqlCommand(checkQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtSalesID.Text));
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                MessageBox.Show("هذا الصنف في فاتورة المبيعات هذه تم إرجاعه بالفعل", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }

                    // Insert into SalesReturn
                    string insertSalesReturnQuery = "insert into SalesReturn(SR_ID, SRNo, Date, SalesID, GrandTotal) VALUES (@d1, @d2, @d3, @d4, @d5)";
                    using (SqlCommand cmd = new SqlCommand(insertSalesReturnQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtSRID.Text));
                        cmd.Parameters.AddWithValue("@d2", txtSRNO.Text);
                        cmd.Parameters.AddWithValue("@d3", dtpSRDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d4", Convert.ToInt32(txtSalesID.Text));
                        cmd.Parameters.AddWithValue("@d5", Convert.ToDecimal(txtGrandTotal.Text));
                        cmd.ExecuteNonQuery();
                    }

                    // Insert into SalesReturn_Join for each row in DataGridView
                    string insertSalesReturnJoinQuery = "insert into SalesReturn_Join(SalesReturnID, Barcode, Price, Qty, DiscountPer, Discount, VATPer, VAT, ReturnQty, TotalAmount, ProductID, CostPrice, Margin) VALUES (@SRID, @Barcode, @Price, @Qty, @DiscountPer, @Discount, @VATPer, @VAT, @ReturnQty, @TotalAmount, @ProductID, @CostPrice, @Margin)";
                    using (SqlCommand cmd = new SqlCommand(insertSalesReturnJoinQuery, con))
                    {
                        foreach (DataGridViewRow row in DataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                cmd.Parameters.AddWithValue("@SRID", Convert.ToInt32(txtSRID.Text));
                                cmd.Parameters.AddWithValue("@Barcode", row.Cells[2].Value);
                                cmd.Parameters.AddWithValue("@Price", row.Cells[3].Value);
                                cmd.Parameters.AddWithValue("@Qty", row.Cells[4].Value);
                                cmd.Parameters.AddWithValue("@DiscountPer", row.Cells[5].Value);
                                cmd.Parameters.AddWithValue("@Discount", row.Cells[6].Value);
                                cmd.Parameters.AddWithValue("@VATPer", row.Cells[7].Value);
                                cmd.Parameters.AddWithValue("@VAT", row.Cells[8].Value);
                                cmd.Parameters.AddWithValue("@ReturnQty", row.Cells[9].Value);
                                cmd.Parameters.AddWithValue("@TotalAmount", row.Cells[10].Value);
                                cmd.Parameters.AddWithValue("@ProductID", row.Cells[11].Value);
                                cmd.Parameters.AddWithValue("@CostPrice", row.Cells[12].Value);
                                cmd.Parameters.AddWithValue("@Margin", row.Cells[13].Value);
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                        }
                    }

                    // Update Temp_Stock for each row in DataGridView
                    string updateStockQuery = "Update Temp_Stock set Qty = Qty + @Qty where ProductID = @ProductID and Barcode = @Barcode";
                    foreach (DataGridViewRow row in DataGridView1.Rows)
                    {
                        using (SqlCommand cmd = new SqlCommand(updateStockQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@Qty", row.Cells[9].Value);
                            cmd.Parameters.AddWithValue("@ProductID", row.Cells[11].Value);
                            cmd.Parameters.AddWithValue("@Barcode", row.Cells[2].Value);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                // Ledger Save
                LedgerSave(dtpSRDate.Value.Date, txtCustomerName.Text, txtSRNO.Text, "مردودات مبيعات", 0, Convert.ToDecimal(txtGrandTotal.Text), txtCustomerID.Text, "");

                // Log Activity
                LogFunc(lblUser.Text, "added the new Sales return record having SR No. '" + txtSRNO.Text + "'");

                // Confirmation Message
                MessageBox.Show("تم الحفظ بنجاح", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);

                btnSave.Enabled = false;

                // Reset form fields
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private double Val(string text)
        {
            double.TryParse(text, out double result);
            return result;
        }
        private void DeleteRecord()
        {
            try
            {
                int rowsAffected = 0;
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string cq = "DELETE FROM SalesReturn WHERE SR_ID = @d1";
                    using (SqlCommand cmd = new SqlCommand(cq, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Convert.ToInt32(txtSRID.Text));
                        rowsAffected = cmd.ExecuteNonQuery();
                    }

                    if (rowsAffected > 0)
                    {
                        foreach (DataGridViewRow row in DataGridView1.Rows)
                        {
                            using (SqlConnection conUpdate = new SqlConnection(DataAccessLayer.Con()))
                            {
                                conUpdate.Open();
                                string cb2 = "UPDATE Temp_Stock SET Qty = Qty - @qty WHERE ProductID = @d1 AND Barcode = @d2";
                                using (SqlCommand cmdUpdate = new SqlCommand(cb2, conUpdate))
                                {
                                    cmdUpdate.Parameters.AddWithValue("@d1", Convert.ToInt32(row.Cells[11].Value));
                                    cmdUpdate.Parameters.AddWithValue("@d2", row.Cells[2].Value);
                                    cmdUpdate.Parameters.AddWithValue("@qty", Convert.ToDouble(row.Cells[9].Value));
                                    cmdUpdate.ExecuteNonQuery();
                                }
                            }
                        }

                        LedgerDelete(txtSRNO.Text, "مردودات مبيعات من " + txtCustomerName.Text);
                        LedgerDelete(txtSRNO.Text, "مردودات مبيعات");

                        string st = "deleted the Sales Return record having SR No. '" + txtSRNO.Text + "'";
                        LogFunc(lblUser.Text, st);

                        MessageBox.Show("تم الحذف بنجاح", "السجلات", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Reset();
                        // RefreshRecords(); // Uncomment if needed
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

        private void txtReturnQty_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' | e.KeyChar > '9') & e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("هل أنت متأكد أنك تريد حذف هذا السجل?", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    DeleteRecord();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Reset();

        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            SalesReturnRecord frmSalesReturnRecord = new SalesReturnRecord();

            frmSalesReturnRecord.lblSet.Text = "SR";
            frmSalesReturnRecord.Reset();
            frmSalesReturnRecord.Show();
            this.Close();
        }

        private void DataGridView2_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                if (DataGridView2.Rows.Count > 0)
                {
                    // Ensure Clear() is defined and called correctly
                    Clear();

                    if (DataGridView2.SelectedRows.Count > 0)
                    {
                        DataGridViewRow dr = DataGridView2.SelectedRows[0];
                        txtProductID.Text = dr.Cells[11].Value.ToString();
                        txtProductCode.Text = dr.Cells[0].Value.ToString();
                        txtProductName.Text = dr.Cells[1].Value.ToString();
                        txtBarcode.Text = dr.Cells[2].Value.ToString();
                        txtPrice.Text = dr.Cells[3].Value.ToString();
                        txtQty.Text = dr.Cells[4].Value.ToString();
                        txtDiscountPer.Text = dr.Cells[6].Value.ToString();
                        txtDiscountAmount.Text = dr.Cells[7].Value.ToString();
                        txtVATPer.Text = dr.Cells[8].Value.ToString();
                        txtVATAmount.Text = dr.Cells[9].Value.ToString();
                        txtCostPrice.Text = dr.Cells[12].Value.ToString();

                        // Convert cell values to numeric types before division
                        decimal margin = Convert.ToDecimal(dr.Cells[13].Value) / Convert.ToDecimal(dr.Cells[4].Value);
                        txtMargin.Text = margin.ToString();

                        txtReturnQty.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Please select a row first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataGridView2_RowPostPaint(object sender, System.Windows.Forms.DataGridViewRowPostPaintEventArgs e)
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

        private void DataGridView1_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (DataGridView1.Rows.Count > 0)
            {
                if (lblSet.Text == "Not allowed")
                {
                    btnRemove.Enabled = false;
                }
                else
                {
                    btnRemove.Enabled = true;
                }
            }
        }

        private void DataGridView1_RowPostPaint(object sender, System.Windows.Forms.DataGridViewRowPostPaintEventArgs e)
        {
            string strRowNumber = (e.RowIndex + 1).ToString();
            SizeF size = e.Graphics.MeasureString(strRowNumber, this.Font);
            if (DataGridView1.RowHeadersWidth < Convert.ToInt32(size.Width + 20))
            {
                DataGridView1.RowHeadersWidth = Convert.ToInt32(size.Width + 20);
            }
            Brush b = SystemBrushes.ControlText;
            e.Graphics.DrawString(strRowNumber, this.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + (e.RowBounds.Height - size.Height) / 2);

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
        public void SMSFunc(string st1, string st2, string st3)
        {
            st3 = st3.Replace("@MobileNo", st1).Replace("@Message", st2);
            HttpWebRequest request;
            HttpWebResponse response = default;
            var myUri = new Uri(st3);
            request = (HttpWebRequest)WebRequest.Create(myUri);
            response = (HttpWebResponse)request.GetResponse();
        }
        public string Encrypt(string password)
        {
            string strmsg = string.Empty;
            byte[] encode = new byte[password.Length];
            encode = Encoding.UTF8.GetBytes(password);
            strmsg = Convert.ToBase64String(encode);
            return strmsg;
        }

        public string Decrypt(string encryptpwd)
        {
            string decryptpwd = string.Empty;
            var encodepwd = new UTF8Encoding();
            var Decode = encodepwd.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encryptpwd);
            int charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            decryptpwd = new string(decoded_char);
            return decryptpwd;
        }
        public void RefreshRecords()
        {
  /*          frmStockBalance obj = (frmStockBalance)Application.OpenForms("frmStockBalance");
            obj.Getdata();
            obj.DataGridView1.Refresh();
            obj.DataGridView1.Update();*/
        }
    
        public void LedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g, string h)
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
        public void LedgerDelete(string a, string b)
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
        public void SendMail(string s1, string s2, string s3, string s5, string s6, int s7, string s8, string s9)
        {
            var msg = new MailMessage();
            try
            {
                msg.From = new MailAddress(s1);
                msg.To.Add(s2);
                msg.Body = s3;
                msg.IsBodyHtml = true;
                msg.Subject = s5;
                var smt = new SmtpClient(s6);
                smt.Port = s7;
                smt.Credentials = new NetworkCredential(s8, s9);
                smt.EnableSsl = true;
                smt.Send(msg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void SupplierLedgerDelete(string a)
        {

            con.Open();
            string cq = "delete from SupplierLedgerBook where LedgerNo=@d1";
            SqlCommand cmd = new SqlCommand(cq);
            cmd.Parameters.AddWithValue("@d1", a);
            cmd.Connection = con;
            cmd.ExecuteReader();
            con.Close();
        }
        public void SupplierLedgerUpdate(DateTime a, string b, decimal e, decimal f, string g, string h)
        {

            con.Open();
            string cb = "Update SupplierLedgerBook set Date=@d1, Name=@d2,Debit=@d3,Credit=@d4 where LedgerNo=@d5 and Label=@d6";
            SqlCommand cmd = new SqlCommand(cb);
            cmd.Parameters.AddWithValue("@d1", a);
            cmd.Parameters.AddWithValue("@d2", b);
            cmd.Parameters.AddWithValue("@d3", e);
            cmd.Parameters.AddWithValue("@d4", f);
            cmd.Parameters.AddWithValue("@d5", g);
            cmd.Parameters.AddWithValue("@d6", h);
            cmd.Connection = con;
            cmd.ExecuteReader();
            con.Close();
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnSelection_BackgroundImageChanged(object sender, EventArgs e)
        {

        }

        private void txtGrandTotal_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSelection_Click_1(object sender, EventArgs e)
        {
            SalesInvoiceScreen frmSalesInvoiceRecord = new SalesInvoiceScreen();
            frmSalesInvoiceRecord.lblSet.Text = "SR";
            frmSalesInvoiceRecord.Reset();
            frmSalesInvoiceRecord.ShowDialog();
        }

      

        private void txtSalesInvoiceNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    } 
 }

