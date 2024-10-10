using Pharmacy.DL;
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
    public partial class Voucher : Form
    {
     
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public Voucher()
        {
            
            InitializeComponent();
            txtParticulars.Text = "0";
            DataGridView1.MouseClick += new MouseEventHandler(DataGridView1_MouseClick);
            txtAmount.KeyPress += new KeyPressEventHandler(txtAmount_KeyPress);
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        public void Reset()
        {
            txtVoucherID.Text = string.Empty;
            txtName.Text = string.Empty;
            txtDetails.Text = string.Empty;
          //  txtParticulars.Text = string.Empty;
            txtNotes.Text = string.Empty;
            txtVoucherNo.Text = string.Empty;
            txtAmount.Text = string.Empty;
            txtGrandTotal.Text = string.Empty;
            dtpDate.Text = DateTime.Today.ToString("d");
            DataGridView1.Rows.Clear();
            btnPrint.Enabled = false;
            btnSave.Enabled = true;
            btnDelete.Enabled = false;
            btnUpdate.Enabled = false;
            btnAdd.Enabled = true;
            btnRemove.Enabled = false;
            Clear();
            auto();
        }
        private void Clear()
        {
          //  txtParticulars.Text = string.Empty;
            txtAmount.Text = string.Empty;
            txtNotes.Text = string.Empty;
            btnAdd.Enabled = true;
            btnRemove.Enabled = false;
        }
        public double GrandTotal()
        {
            double sum = 0;
            try
            {
                foreach (DataGridViewRow r in this.DataGridView1.Rows)
                {
                    sum += Convert.ToDouble(r.Cells[1].Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return sum;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Reset();
            Reset();
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
                    using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 ID FROM Voucher ORDER BY ID DESC", con))
                    {
                        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            value = rdr["ID"].ToString();
                        }
                        rdr.Close();
                    }

                    // Increase the ID by 1
                    value = (int.Parse(value) + 1).ToString();

                    // Ensure the ID is 4 digits long
                    value = value.PadLeft(4, '0');
                }
                catch (Exception ex)
                {
                    // Handle any errors and reset the ID to "0000"
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                    value = "0000";
                }
            }
            return value;
        }
        private void auto()
        {
            try
            {
                txtVoucherID.Text = GenerateID();
                txtVoucherNo.Text = "V-" + GenerateID();
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


                rptVoucher rpt = new rptVoucher(); // The report you created.
                SqlConnection myConnection = new SqlConnection(DataAccessLayer.Con());
                SqlCommand MyCommand = new SqlCommand();
                SqlCommand MyCommand1 = new SqlCommand();
                SqlDataAdapter myDA = new SqlDataAdapter();
                SqlDataAdapter myDA1 = new SqlDataAdapter();
                DataSet myDS = new DataSet(); // The DataSet you created.

                MyCommand.Connection = myConnection;
                MyCommand1.Connection = myConnection;

                MyCommand.CommandText = "SELECT Voucher.ID, Voucher.VoucherNo, Voucher.Date, Voucher.Name, Voucher.Details, Voucher.GrandTotal, " +
                                        "Voucher_OtherDetails.VD_ID, Voucher_OtherDetails.VoucherID, Voucher_OtherDetails.Particulars, " +
                                        "Voucher_OtherDetails.Amount, Voucher_OtherDetails.Note " +
                                        "FROM Voucher " +
                                        "INNER JOIN Voucher_OtherDetails ON Voucher.ID = Voucher_OtherDetails.VoucherID " +
                                        "WHERE VoucherNo = @VoucherNo";

                MyCommand1.CommandText = "SELECT * FROM Company";

                MyCommand.CommandType = CommandType.Text;
                MyCommand1.CommandType = CommandType.Text;

                MyCommand.Parameters.AddWithValue("@VoucherNo", txtVoucherNo.Text);

                myDA.SelectCommand = MyCommand;
                myDA1.SelectCommand = MyCommand1;

                myDA.Fill(myDS, "Voucher");
                myDA.Fill(myDS, "Voucher_OtherDetails");
                myDA1.Fill(myDS, "Company");

                rpt.SetDataSource(myDS);
                frmReport frmReport = new frmReport();
                frmReport.crystalReportViewer1.ReportSource = rpt;
                frmReport.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void DeleteRecord()
        {
            try
            {
                int RowsAffected = 0;
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string ct = "DELETE FROM Voucher WHERE ID = @ID";
                    using (SqlCommand cmd = new SqlCommand(ct, con))
                    {
                        cmd.Parameters.AddWithValue("@ID", txtVoucherID.Text);
                        RowsAffected = cmd.ExecuteNonQuery();
                    }
                }

                if (RowsAffected > 0)
                {
                    LedgerDelete(txtVoucherNo.Text, "مصروفات");
                    string st = "deleted the voucher having voucher no. '" + txtVoucherNo.Text + "'";
                    LogFunc(lblUser.Text, st);
                    MessageBox.Show("تم الحذف بنجاح", "السجلات", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset();
                }
                else
                {
                    MessageBox.Show("لا يوجد سجلات", "عذراً", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset();
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
                    cmd.ExecuteReader();
                }
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Print();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtParticulars.Text))
                {
                    MessageBox.Show("الرجاء كتابة البيان", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtParticulars.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtAmount.Text))
                {
                    MessageBox.Show("الرجاء كتابة المبلغ", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtAmount.Focus();
                    return;
                }

                if (DataGridView1.Rows.Count == 0)
                {
                    DataGridView1.Rows.Add(txtParticulars.Text, Convert.ToDouble(txtAmount.Text), txtNotes.Text);
                    double k = GrandTotal();
                    k = Math.Round(k, 2);
                    txtGrandTotal.Text = k.ToString();
                    Clear();
                    return;
                }

                DataGridView1.Rows.Add(txtParticulars.Text, Convert.ToDouble(txtAmount.Text), txtNotes.Text);
                double j = GrandTotal();
                j = Math.Round(j, 2);
                txtGrandTotal.Text = j.ToString();
                Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("الرجاء كتابة اسم السند", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtName.Focus();
                    return;
                }

                if (DataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("عذراً لا يوجد بيانات مضافة في شبكة البيانات", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int voucherId;
                decimal grandTotal;

                // Validate and parse values
                if (!int.TryParse(txtVoucherID.Text, out voucherId))
                {
                    MessageBox.Show("Invalid Voucher ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!decimal.TryParse(txtGrandTotal.Text, out grandTotal))
                {
                    MessageBox.Show("Invalid Grand Total.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // Insert into Voucher
                    string cb = "INSERT INTO Voucher(Id, VoucherNo, Date, Name, Details, GrandTotal) VALUES (@d1, @d2, @d3, @d4, @d5, @d7)";
                    using (SqlCommand cmd = new SqlCommand(cb, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", voucherId);
                        cmd.Parameters.AddWithValue("@d2", txtVoucherNo.Text);
                        cmd.Parameters.AddWithValue("@d3", dtpDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d4", txtName.Text);
                        cmd.Parameters.AddWithValue("@d5", txtDetails.Text);
                        cmd.Parameters.AddWithValue("@d7", grandTotal);
                        cmd.ExecuteNonQuery();
                    }

                    // Insert into Voucher_OtherDetails
                    string cb1 = "INSERT INTO Voucher_OtherDetails(VoucherID, Particulars, Amount, Note) VALUES (@d1, @d2, @d3, @d4)";
                    using (SqlCommand cmd = new SqlCommand(cb1, con))
                    {
                        foreach (DataGridViewRow row in DataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                int particulars;
                                decimal amount;
                                string note = row.Cells[2].Value?.ToString(); // Assuming note is a string

                                string particularsStr = row.Cells[0].Value?.ToString();
                                string amountStr = row.Cells[1].Value?.ToString();

                                // Log the values for debugging
                                Console.WriteLine($"Particulars: {particularsStr}, Amount: {amountStr}");

                                if (!int.TryParse(particularsStr, out particulars))
                                {
                                    MessageBox.Show($"Invalid particulars value in row: {particularsStr}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    continue; // Skip this row
                                }

                                if (!decimal.TryParse(amountStr, out amount))
                                {
                                    MessageBox.Show($"Invalid amount value in row: {amountStr}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    continue; // Skip this row
                                }

                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@d1", voucherId);
                                cmd.Parameters.AddWithValue("@d2", particulars);
                                cmd.Parameters.AddWithValue("@d3", amount);
                                cmd.Parameters.AddWithValue("@d4", note ?? (object)DBNull.Value);

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                string st = "added the new voucher having voucher no. '" + txtVoucherNo.Text + "'";
                LogFunc(lblUser.Text, st);
                LedgerSave(dtpDate.Value.Date, txtName.Text, txtVoucherNo.Text, "مصروفات", grandTotal, 0, "", "");
                btnSave.Enabled = false;
                MessageBox.Show("تم الحفظ بنجاح", "سندات الصرف", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("هل أنت متأكد أنك تريد حذف سجل السند?", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    DeleteRecord();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("الرجاء كتابة اسم السند", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtName.Focus();
                    return;
                }

                if (DataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("عذرًا لا يوجد بيانات مضافة في شبكة البيانات", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int voucherId;
                decimal grandTotal;

                // Validate and parse values
                if (!int.TryParse(txtVoucherID.Text, out voucherId))
                {
                    MessageBox.Show("Invalid Voucher ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!decimal.TryParse(txtGrandTotal.Text, out grandTotal))
                {
                    MessageBox.Show("Invalid Grand Total.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // Update Voucher
                    string cb = "UPDATE Voucher SET VoucherNo = @d2, Date = @d3, Name = @d4, Details = @d5, GrandTotal = @d7 WHERE ID = @d1";
                    using (SqlCommand cmd = new SqlCommand(cb, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", voucherId);
                        cmd.Parameters.AddWithValue("@d2", txtVoucherNo.Text);
                        cmd.Parameters.AddWithValue("@d3", dtpDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d4", txtName.Text);
                        cmd.Parameters.AddWithValue("@d5", txtDetails.Text);
                        cmd.Parameters.AddWithValue("@d7", grandTotal);
                        cmd.ExecuteNonQuery();
                    }

                    // Delete existing Voucher_OtherDetails
                    string deleteDetails = "DELETE FROM Voucher_OtherDetails WHERE VoucherID = @VoucherID";
                    using (SqlCommand cmd = new SqlCommand(deleteDetails, con))
                    {
                        cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                        cmd.ExecuteNonQuery();
                    }

                    // Insert new Voucher_OtherDetails
                    string insertDetails = "INSERT INTO Voucher_OtherDetails(VoucherID, Particulars, Amount, Note) VALUES (@VoucherID, @d1, @d2, @d3)";
                    using (SqlCommand cmd = new SqlCommand(insertDetails, con))
                    {
                        foreach (DataGridViewRow row in DataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@VoucherID", voucherId);
                                cmd.Parameters.AddWithValue("@d1", row.Cells[0].Value);
                                cmd.Parameters.AddWithValue("@d2", Convert.ToDouble(row.Cells[1].Value));
                                cmd.Parameters.AddWithValue("@d3", row.Cells[2].Value ?? (object)DBNull.Value);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                string st = "updated the voucher having voucher no. '" + txtVoucherNo.Text + "'";
                LogFunc(lblUser.Text, st);
                LedgerUpdate(dtpDate.Value.Date, txtName.Text, Convert.ToDecimal(txtGrandTotal.Text), 0, txtVoucherNo.Text, "", "Expenses");
                btnUpdate.Enabled = false;
                MessageBox.Show("تم التعديل بنجاح", "سندات الصرف", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void DataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            btnRemove.Enabled = true ;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in DataGridView1.SelectedRows)
                {
                    if (!row.IsNewRow)
                    {
                        DataGridView1.Rows.Remove(row);
                    }
                }

                double k = GrandTotal();
                k = Math.Round(k, 2);
                txtGrandTotal.Text = k.ToString();

                btnRemove.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
/*            char keyChar = e.KeyChar;

            if (char.IsControl(keyChar))
            {
                // Allow all control characters.
                return;
            }
            else if (char.IsDigit(keyChar) || keyChar == '.')
            {
                string text = txtAmount.Text;
                int selectionStart = txtAmount.SelectionStart;
                int selectionLength = txtAmount.SelectionLength;

                text = text.Substring(0, selectionStart) + keyChar + text.Substring(selectionStart + selectionLength);

                if (int.TryParse(text, out _) && text.Length > 16)
                {
                    // Reject an integer that is longer than 16 digits.
                    e.Handled = true;
                }
                else if (double.TryParse(text, out _) && text.IndexOf('.') < text.Length - 3)
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

        private void btnGetData_Click(object sender, EventArgs e)
        {
            VoucherRecord frm = new VoucherRecord();
            frm.Reset();
            frm.ShowDialog();

        }

        private void Button2_Click(object sender, EventArgs e)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("الرجاء كتابة اسم السند", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtName.Focus();
                    return;
                }

                if (DataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("عذراً لا يوجد بيانات مضافة في شبكة البيانات", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int voucherId;
                decimal grandTotal;

                // Validate and parse values
                if (!int.TryParse(txtVoucherID.Text, out voucherId))
                {
                    MessageBox.Show("Invalid Voucher ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!decimal.TryParse(txtGrandTotal.Text, out grandTotal))
                {
                    MessageBox.Show("Invalid Grand Total.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // Insert into Voucher
                    string cb = "INSERT INTO Voucher(Id, VoucherNo, Date, Name, Details, GrandTotal) VALUES (@d1, @d2, @d3, @d4, @d5, @d7)";
                    using (SqlCommand cmd = new SqlCommand(cb, con))
                    {
                        cmd.Parameters.AddWithValue("@d1", voucherId);
                        cmd.Parameters.AddWithValue("@d2", txtVoucherNo.Text);
                        cmd.Parameters.AddWithValue("@d3", dtpDate.Value.Date);
                        cmd.Parameters.AddWithValue("@d4", txtName.Text);
                        cmd.Parameters.AddWithValue("@d5", txtDetails.Text);
                        cmd.Parameters.AddWithValue("@d7", grandTotal);
                        cmd.ExecuteNonQuery();
                    }

                    // Insert into Voucher_OtherDetails
                    string cb1 = "INSERT INTO Voucher_OtherDetails(VoucherID, Particulars, Amount, Note) VALUES (@d1, @d2, @d3, @d4)";
                    using (SqlCommand cmd = new SqlCommand(cb1, con))
                    {
                        foreach (DataGridViewRow row in DataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                int particulars;
                                decimal amount;
                                string note = row.Cells[2].Value?.ToString(); // Assuming note is a string

                                string particularsStr = row.Cells[0].Value?.ToString();
                                string amountStr = row.Cells[1].Value?.ToString();

                                // Log the values for debugging
                                Console.WriteLine($"Particulars: {particularsStr}, Amount: {amountStr}");

                                if (!int.TryParse(particularsStr, out particulars))
                                {
                                    MessageBox.Show($"Invalid particulars value in row: {particularsStr}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    continue; // Skip this row
                                }

                                if (!decimal.TryParse(amountStr, out amount))
                                {
                                    MessageBox.Show($"Invalid amount value in row: {amountStr}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    continue; // Skip this row
                                }

                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@d1", voucherId);
                                cmd.Parameters.AddWithValue("@d2", particulars);
                                cmd.Parameters.AddWithValue("@d3", amount);
                                cmd.Parameters.AddWithValue("@d4", note ?? (object)DBNull.Value);

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                string st = "added the new voucher having voucher no. '" + txtVoucherNo.Text + "'";
                LogFunc(lblUser.Text, st);
                LedgerSave(dtpDate.Value.Date, txtName.Text, txtVoucherNo.Text, "مصروفات", grandTotal, 0, "", "");
                btnSave.Enabled = false;
                MessageBox.Show("تم الحفظ بنجاح", "سندات الصرف", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SalesmanRecord frm = new SalesmanRecord();
            frm.lblSet.Text = "voucher";
            frm.Reset();
            frm.ShowDialog();

        }
    }
}
