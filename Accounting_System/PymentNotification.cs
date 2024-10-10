using Microsoft.Office.Interop.Excel;
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
using Excel = Microsoft.Office.Interop.Excel;
using System.Text;
using System.Net.Mail;
using System.IO;
namespace Accounting_System
{
    public partial class PymentNotification : Form
    {
        public PymentNotification()
        {
            InitializeComponent();
        }
        private string str;
        private string OBType;
        private decimal num1, num2, num3, num4;
        private int i = 0;
        string cs = DataAccessLayer.Con();
        public void GetSupplierBalance()
        {
            try
            {
                decimal num1 = 0m;
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string sql = "SELECT ISNULL(SUM(Credit), 0) - ISNULL(SUM(Debit), 0) FROM LedgerBook WHERE PartyID = @d1 GROUP BY PartyID";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", txtSupplierID.Text);
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                num1 = Convert.ToDecimal(rdr.GetValue(0));
                            }
                        }
                    }
                }

                lblBalance.Text = num1.ToString();
                string str = Val(lblBalance.Text) >= 0 ? "دائن" : "مدين";
                lblBalance.Text = Math.Abs(Val(lblBalance.Text)).ToString() + " " + str;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static double Val(string InputStr)
        {
            int num = InputStr?.Length ?? 0;
            checked
            {
                int i;
                for (i = 0; i < num; i++)
                {
                    switch (InputStr[i])
                    {
                        case '\t':
                        case '\n':
                        case '\r':
                        case ' ':
                        case '\u3000':
                            continue;
                    }

                    break;
                }

                if (i >= num)
                {
                    return 0.0;
                }

                char c = InputStr[i];
               

                bool flag = false;
                bool flag2 = false;
                bool flag3 = false;
                double num2 = 0.0;
                c = InputStr[i];
                switch (c)
                {
                    case '-':
                        flag3 = true;
                        i++;
                        break;
                    case '+':
                        i++;
                        break;
                }

                int num4 = default(int);
                double num3 = default(double);
                int num5 = default(int);
                while (i < num)
                {
                    c = InputStr[i];
                    char c2 = c;
                    switch (c2)
                    {
                        case '\t':
                        case '\n':
                        case '\r':
                        case ' ':
                        case '\u3000':
                            i++;
                            continue;
                        case '0':
                            if (num4 != 0 || flag)
                            {
                                num3 = num3 * 10.0 + (double)unchecked((int)c) - 48.0;
                                i++;
                                num4++;
                            }
                            else
                            {
                                i++;
                            }

                            continue;
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            num3 = num3 * 10.0 + (double)unchecked((int)c) - 48.0;
                            i++;
                            num4++;
                            continue;
                    }

                    switch (c2)
                    {
                        case '.':
                            i++;
                            if (!flag)
                            {
                                flag = true;
                                num5 = num4;
                                continue;
                            }

                            break;
                        case 'D':
                        case 'E':
                        case 'd':
                        case 'e':
                            flag2 = true;
                            i++;
                            break;
                    }

                    break;
                }

                int num6 = default(int);
                if (flag)
                {
                    num6 = num4 - num5;
                }

                if (flag2)
                {
                    bool flag4 = false;
                    bool flag5 = false;
                    while (i < num)
                    {
                        c = InputStr[i];
                        char c3 = c;
                        switch (c3)
                        {
                            case '\t':
                            case '\n':
                            case '\r':
                            case ' ':
                            case '\u3000':
                                i++;
                                continue;
                            case '0':
                            case '1':
                            case '2':
                            case '3':
                            case '4':
                            case '5':
                            case '6':
                            case '7':
                            case '8':
                            case '9':
                                num2 = num2 * 10.0 + (double)unchecked((int)c) - 48.0;
                                i++;
                                continue;
                        }

                        switch (c3)
                        {
                            case '+':
                                if (!flag4)
                                {
                                    flag4 = true;
                                    i++;
                                    continue;
                                }

                                break;
                            case '-':
                                if (!flag4)
                                {
                                    flag4 = true;
                                    flag5 = true;
                                    i++;
                                    continue;
                                }

                                break;
                        }

                        break;
                    }

                    if (flag5)
                    {
                        num2 += (double)num6;
                        num3 *= Math.Pow(10.0, 0.0 - num2);
                    }
                    else
                    {
                        num2 -= (double)num6;
                        num3 *= Math.Pow(10.0, num2);
                    }
                }
                else if (flag && num6 != 0)
                {
                    num3 /= Math.Pow(10.0, num6);
                }

                if (double.IsInfinity(num3))
                {
                    throw new Exception("Value is too large or too small.");
                }

                if (flag3)
                {
                    num3 = 0.0 - num3;
                }

                switch (c)
                {
                    case '%':
                        if (num6 > 0)
                        {
                            throw new Exception("Invalid percentage value.");
                        }

                        num3 = (short)Math.Round(num3);
                        break;
                    case '&':
                        if (num6 > 0)
                        {
                            throw new Exception("Invalid value.");
                        }

                        num3 = (int)Math.Round(num3);
                        break;
                    case '!':
                        num3 = (float)num3;
                        break;
                }

                return num3;
            }
        }

        private string GenerateID()
        {
            string value = "0000";
            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    string sql = "SELECT TOP 1 TCD_ID FROM Payment_3 ORDER BY TCD_ID DESC";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            value = rdr["TCD_ID"].ToString();
                        }
                    }
                }

                value = (Convert.ToInt32(value) + 1).ToString("D4");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return value;
        }

        private void CountValue()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    string sql = "SELECT COUNT(TCD_ID) FROM Payment_3 WHERE Amount = 0";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        i = cmd.ExecuteScalar() is DBNull ? 0 : Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void auto()
        {
            try
            {
                CountValue();
                txtT_ID.Text = GenerateID();
                txtTransactionNo.Text = "TCD-" + (Convert.ToInt32(GenerateID()) - i).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Reset()
        {
            txtAddress.Text = "";
            txtCity.Text = "";
            txtContactNo.Text = "";
            txtRemarks.Text = "";
            txtSupplierID.Text = "";
            txtSalesman.Text = "";
            txtSalesmanID.Text = "";
            txtSM_ID.Text = "";
            txtCommissionPer.Text = "";
            txtSupplierName.Text = "";
            txtTransactionAmount.Text = "";
            dtpTranactionDate.Value = DateTime.Today;
            lblBalance.Text = "0.000";
            btnSave.Enabled = true;
            btnDelete.Enabled = false;
            btnUpdate.Enabled = false;
            btnSelection.Enabled = true;
            auto();
        }

        public void GetSupplierInfo()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    string sql = "SELECT CustomerID, Name, Address, City, ContactNo FROM Customer WHERE ID = @d1";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Val(txtSup_ID.Text));
                        using (SqlDataReader rdr = cmd.ExecuteReader())
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
        private void DeleteRecord()
        {
            try
            {
                int RowsAffected = 0;
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    string cq = "delete from Payment_3 where TCD_ID=@d1";

                    using (SqlCommand cmd = new SqlCommand(cq, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Val(txtT_ID.Text));
                        RowsAffected = cmd.ExecuteNonQuery();
                    }
                }

                if (RowsAffected > 0)
                {
                    SupplierLedgerDelete(txtTransactionNo.Text);
                    LedgerDelete(txtTransactionNo.Text, "خصم مسموح به");
                    LogFunc(lblUser.Text, "deleted the Customer payment record having transaction No. '" + txtTransactionNo.Text + "'");
                    MessageBox.Show("تم الحذف بنجاح", "اشعار خصم - دائن", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset();
                }
                else
                {
                    MessageBox.Show("لا يوجد سجلات", "عفوا", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Val((txtSupplierID.Text)) == 0)
            {
                MessageBox.Show("الرجاء إدراج رقم العميل", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSupplierID.Focus();
                return;
            }
            if (Val((txtTransactionAmount.Text)) == 0)
            {
                MessageBox.Show("الرجاء كتابة المبلغ المدفوع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTransactionAmount.Focus();
                return;
            }
            if (Val(txtTransactionAmount.Text) == 0)
            {
                MessageBox.Show("مبلغ الخصم المسموح به يجب أن يكون أكبر من صفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTransactionAmount.Focus();
                return;
            }
            if (Val((txtSalesmanID.Text)) == 0)
            {
                MessageBox.Show("الرجاء اختيار المندوب", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSalesman.Focus();
                return;
            }
            if (Val(txtCommissionPer.Text) == 0)
            {
                MessageBox.Show("مبلغ نسبة المندوب يجب أن يكون أكبر من صفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCommissionPer.Focus();
                return;
            }
            if (Val(txtTransactionAmount.Text) > Val(lblBalance.Text))
            {
                MessageBox.Show("مبلغ الخصم يجب أن لا يكون أكبر من رصيد العميل", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTransactionAmount.Focus();
                return;
            }

            try
            {
                // Use 'using' statement to ensure the connection and command are properly disposed of.
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    string cb = "INSERT INTO Payment_3(TCD_ID, TransactionID, Date, CustomerID, Amount,Remarks,SalesMan_ID,SalesMan_Name,SalesMan_Comession,SalesMan_ID_2) " +
                                "VALUES (@d1,@d2,@d3,@d4,@d5,@d6,@d7,@d8,@d9,@d10)";

                    using (SqlCommand cmd = new SqlCommand(cb, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Val(txtT_ID.Text));
                        cmd.Parameters.AddWithValue("@d2", txtTransactionNo.Text);
                        cmd.Parameters.AddWithValue("@d3", dtpTranactionDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d4", txtSupplierID.Text);
                        cmd.Parameters.AddWithValue("@d5", Val(txtTransactionAmount.Text));
                        cmd.Parameters.AddWithValue("@d6", txtRemarks.Text);
                        cmd.Parameters.AddWithValue("@d7", txtSM_ID.Text);
                        cmd.Parameters.AddWithValue("@d8", txtSalesman.Text);
                        cmd.Parameters.AddWithValue("@d9", txtCommissionPer.Text);
                        cmd.Parameters.AddWithValue("@d10", txtSalesmanID.Text);

                        cmd.ExecuteNonQuery();
                    }
                }

                LedgerSave(dtpTranactionDate.Value.Date, "نقدا", txtTransactionNo.Text, "خصم مسموح به", 0, Convert.ToDecimal(txtTransactionAmount.Text), txtSupplierID.Text, "");
                LogFunc(lblUser.Text, "added the new payment having transaction No. '" + txtTransactionNo.Text + "'");
                MessageBox.Show("تم الحفظ بنجاح", "اشعار خصم - دائن", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Val((txtSupplierID.Text)) == 0)
            {
                MessageBox.Show("الرجاء إدراج رقم العميل", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSupplierID.Focus();
                return;
            }
            if (Val((txtTransactionAmount.Text)) == 0)
            {
                MessageBox.Show("الرجاء كتابة المبلغ المدفوع", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTransactionAmount.Focus();
                return;
            }
            if (Val(txtTransactionAmount.Text) == 0)
            {
                MessageBox.Show("مبلغ الخصم المسموح به يجب أن يكون أكبر من صفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTransactionAmount.Focus();
                return;
            }
            if (Val(txtTransactionAmount.Text) > Val(lblBalance.Text))
            {
                MessageBox.Show("مبلغ الخصم يجب أن لا يكون أكبر من رصيد العميل", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTransactionAmount.Focus();
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    string cb = "UPDATE Payment_3 SET TransactionID=@d2, Date=@d3, CustomerID=@d4, Amount=@d5, Remarks=@d6, SalesMan_ID=@d7, SalesMan_Name=@d8, SalesMan_Comession=@d9, SalesMan_ID_2=@d10 " +
                                "WHERE TCD_ID=@d1";

                    using (SqlCommand cmd = new SqlCommand(cb, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", Val(txtT_ID.Text));
                        cmd.Parameters.AddWithValue("@d2", txtTransactionNo.Text);
                        cmd.Parameters.AddWithValue("@d3", dtpTranactionDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d4", txtSupplierID.Text);
                        cmd.Parameters.AddWithValue("@d5", Val(txtTransactionAmount.Text));
                        cmd.Parameters.AddWithValue("@d6", txtRemarks.Text);
                        cmd.Parameters.AddWithValue("@d7", txtSM_ID.Text);
                        cmd.Parameters.AddWithValue("@d8", txtSalesman.Text);
                        cmd.Parameters.AddWithValue("@d9", txtCommissionPer.Text);
                        cmd.Parameters.AddWithValue("@d10", txtSalesmanID.Text);

                        cmd.ExecuteNonQuery();
                    }
                }

                LedgerUpdate(dtpTranactionDate.Value.Date, "نقدا", 0, Convert.ToDecimal(txtTransactionAmount.Text), txtSupplierID.Text, txtTransactionNo.Text, "خصم مسموح به");
                LogFunc(lblUser.Text, "updated Customer payment record having transaction No. '" + txtTransactionNo.Text + "'");
                MessageBox.Show("تم التعديل بنجاح", "اشعار خصم - دائن", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnUpdate.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        public static void SMS(string st1)
        {
            using (SqlConnection con = DataAccessLayer.cn)
            {
                con.Open();
                string cb = "INSERT INTO SMS(Message, Date) VALUES (@d1, @d2)";
                using (var cmd = new SqlCommand(cb, con))
                {
                    cmd.Parameters.AddWithValue("@d1", st1);
                    cmd.Parameters.AddWithValue("@d2", DateTime.Now);
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

        public static void SMSFunc(string st1, string st2, string st3)
        {
            st3 = st3.Replace("@MobileNo", st1).Replace("@Message", st2);
            var myUri = new Uri(st3);
            var request = (HttpWebRequest)WebRequest.Create(myUri);
            using (var response = (HttpWebResponse)request.GetResponse()) { }
        }

        public static string Encrypt(string password)
        {
            byte[] encode = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(encode);
        }

        public static string Decrypt(string encryptpwd)
        {
            byte[] todecodeByte = Convert.FromBase64String(encryptpwd);
            Decoder decode = Encoding.UTF8.GetDecoder();
            int charCount = decode.GetCharCount(todecodeByte, 0, todecodeByte.Length);
            char[] decodedChar = new char[charCount];
            decode.GetChars(todecodeByte, 0, todecodeByte.Length, decodedChar, 0);
            return new string(decodedChar);
        }

        public static void RefreshRecords()
        {
            StockBalance obj = new StockBalance();
            obj.Getdata();
            obj.dataGridView1.Refresh();
            obj.dataGridView1.Update();
        }

        public static void ExportExcel(object obj)
        {
            short rowsTotal, colsTotal;
            short I, j, iC;
            Cursor.Current = Cursors.WaitCursor;
            var xlApp = new Excel.Application();
            try
            {
                var excelBook = xlApp.Workbooks.Add();
                var excelWorksheet = (Excel.Worksheet)excelBook.Worksheets[1];
                xlApp.Visible = true;

                rowsTotal = (short)((DataGridView)obj).RowCount;
                colsTotal = (short)(((DataGridView)obj).Columns.Count - 1);
                excelWorksheet.Cells.Select();
                excelWorksheet.Cells.Delete();
                for (iC = 0; iC <= colsTotal; iC++)
                {
                    excelWorksheet.Cells[1, iC + 1].Value = ((DataGridView)obj).Columns[iC].HeaderText;
                }
                for (I = 0; I < rowsTotal; I++)
                {
                    for (j = 0; j <= colsTotal; j++)
                    {
                        excelWorksheet.Cells[I + 2, j + 1].Value = ((DataGridView)obj).Rows[I].Cells[j].Value;
                    }
                }
                excelWorksheet.Rows["1:1"].Font.FontStyle = "Bold";
                excelWorksheet.Rows["1:1"].Font.Size = 12;

                excelWorksheet.Cells.Columns.AutoFit();
                excelWorksheet.Cells.Select();
                excelWorksheet.Cells.EntireColumn.AutoFit();
                excelWorksheet.Cells[1, 1].Select();
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

        public static void LedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g, string h)
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
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

        public static void SupplierLedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g)
        {
            using (SqlConnection con = DataAccessLayer.cn)
            {
                con.Open();
                string cb = "INSERT INTO SupplierLedgerBook(Date, Name, LedgerNo, Label, Debit, Credit, PartyID) VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7)";
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
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("هل أنت متأكد أنك تريد حذف هذا الاشعار من السجلات?", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                {
                    DeleteRecord();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {

        }

        public static void SendMail(string s1, string s2, string s3, string s5, string s6, int s7, string s8, string s9)
        {
            var msg = new MailMessage();
            try
            {
                msg.From = new MailAddress(s1);
                msg.To.Add(s2);
                msg.Body = s3;
                msg.IsBodyHtml = true;
                msg.Subject = s5;
                var smt = new SmtpClient(s6)
                {
                    Port = s7,
                    Credentials = new NetworkCredential(s8, s9),
                    EnableSsl = true
                };
                smt.Send(msg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SupplierLedgerDelete(string a)
        {
            using (SqlConnection con = DataAccessLayer.cn)
            {
                con.Open();
                string cq = "DELETE FROM SupplierLedgerBook WHERE LedgerNo=@d1";
                using (var cmd = new SqlCommand(cq, con))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.ExecuteReader();
                }
            }
        }

        public static void SupplierLedgerUpdate(DateTime a, string b, decimal e, decimal f, string g, string h)
        {
            using (SqlConnection con = DataAccessLayer.cn)
            {
                con.Open();
                string cb = "UPDATE SupplierLedgerBook SET Date=@d1, Name=@d2, Debit=@d3, Credit=@d4 WHERE LedgerNo=@d5 AND Label=@d6";
                using (var cmd = new SqlCommand(cb, con))
                {
                    cmd.Parameters.AddWithValue("@d1", a);
                    cmd.Parameters.AddWithValue("@d2", b);
                    cmd.Parameters.AddWithValue("@d3", e);
                    cmd.Parameters.AddWithValue("@d4", f);
                    cmd.Parameters.AddWithValue("@d5", g);
                    cmd.Parameters.AddWithValue("@d6", h);
                    cmd.ExecuteReader();
                }
            }
        }
    }
}
