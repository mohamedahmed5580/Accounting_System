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
        public static Voucher instance;
        public Voucher()
        {
            InitializeComponent();
            txtParticulars.Text = "0";
            DataGridView1.MouseClick += new MouseEventHandler(DataGridView1_MouseClick);
            txtAmount.KeyPress += new KeyPressEventHandler(txtAmount_KeyPress);
            instance = this;
            fillSaleMan();
        }

        public void Reset()
        {
            txtVoucherID.Text = string.Empty;
            txtName.Text = string.Empty;
            txtDetails.Text = string.Empty;
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
                    if (r.Cells[1].Value != null)
                        sum += Convert.ToDouble(r.Cells[1].Value);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            return sum;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private string GenerateID()
        {
            string value = "0000";
            try
            {
                string query = "SELECT TOP 1 ID FROM Voucher ORDER BY ID DESC";
                object result = DataAccessLayer.ExecuteScalar(query, CommandType.Text);
                if (result != null && result != DBNull.Value)
                {
                    value = result.ToString();
                }
                value = (int.Parse(value) + 1).ToString().PadLeft(4, '0');
            }
            catch
            {
                value = "0001";
            }
            return value;
        }

        private void auto()
        {
            try
            {
                string id = GenerateID();
                txtVoucherID.Text = id;
                txtVoucherNo.Text = "V-" + id;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void Print()
        {
            try
            {
                rptVoucher rpt = new rptVoucher();
                DataSet myDS = new DataSet();

                string queryVoucher = "SELECT Voucher.ID, Voucher.VoucherNo, Voucher.Date, Voucher.Name, Voucher.Details, Voucher.GrandTotal, " +
                                     "Voucher_OtherDetails.VD_ID, Voucher_OtherDetails.VoucherID, Voucher_OtherDetails.Particulars, " +
                                     "Voucher_OtherDetails.Amount, Voucher_OtherDetails.Note " +
                                     "FROM Voucher " +
                                     "INNER JOIN Voucher_OtherDetails ON Voucher.ID = Voucher_OtherDetails.VoucherID " +
                                     "WHERE VoucherNo = @VoucherNo";

                SqlParameter[] parameters = { new SqlParameter("@VoucherNo", txtVoucherNo.Text) };
                
                using (SqlConnection connection = new SqlConnection(DataAccessLayer.Con()))
                {
                    using (SqlCommand cmd = new SqlCommand(queryVoucher, connection))
                    {
                        cmd.Parameters.AddRange(parameters);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(myDS, "Voucher");
                        da.Fill(myDS, "Voucher_OtherDetails");
                    }
                    
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Company", connection))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(myDS, "Company");
                    }
                }

                rpt.SetDataSource(myDS);
                frmReport frmReport = new frmReport();
                frmReport.crystalReportViewer1.ReportSource = rpt;
                frmReport.ShowDialog();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public void DeleteRecord()
        {
            try
            {
                string query = "DELETE FROM Voucher WHERE ID = @ID";
                SqlParameter[] parameters = { new SqlParameter("@ID", txtVoucherID.Text) };
                int RowsAffected = DataAccessLayer.ExecuteNonQuery(query, CommandType.Text, parameters);

                if (RowsAffected > 0)
                {
                    LedgerDelete(txtVoucherNo.Text, "مصروفات");
                    LogFunc(lblUser.Text, "deleted the voucher having voucher no. '" + txtVoucherNo.Text + "'");
                    System.Windows.Forms.MessageBox.Show("تم الحذف بنجاح", "السجلات", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    Reset();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("لا يوجد سجلات", "عذراً", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    Reset();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void LogFunc(string st1, string st2)
        {
            try
            {
                string query = "INSERT INTO Logs(UserID, Date, Operation) VALUES (@d1, @d2, @d3)";
                SqlParameter[] parameters = {
                    new SqlParameter("@d1", st1),
                    new SqlParameter("@d2", DateTime.Now),
                    new SqlParameter("@d3", st2)
                };
                DataAccessLayer.ExecuteNonQuery(query, CommandType.Text, parameters);
            }
            catch { }
        }

        public static void LedgerDelete(string a, string b)
        {
            try
            {
                string query = "DELETE FROM LedgerBook WHERE LedgerNo=@d1 AND Label=@d2";
                SqlParameter[] parameters = {
                    new SqlParameter("@d1", a),
                    new SqlParameter("@d2", b)
                };
                DataAccessLayer.ExecuteNonQuery(query, CommandType.Text, parameters);
            }
            catch { }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;
            try { Print(); }
            finally { btnPrint.Enabled = true; }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtParticulars.Text))
                {
                    System.Windows.Forms.MessageBox.Show("الرجاء كتابة البيان", "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                    txtParticulars.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtAmount.Text))
                {
                    System.Windows.Forms.MessageBox.Show("الرجاء كتابة المبلغ", "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                    txtAmount.Focus();
                    return;
                }

                btnAdd.Enabled = false;
                try
                {
                    DataGridView1.Rows.Add(txtParticulars.Text, Convert.ToDouble(txtAmount.Text), txtNotes.Text);
                    double sum = GrandTotal();
                    txtGrandTotal.Text = Math.Round(sum, 2).ToString();
                    Clear();
                }
                finally { btnAdd.Enabled = true; }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                System.Windows.Forms.MessageBox.Show("الرجاء كتابة اسم السند", "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            if (DataGridView1.Rows.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("عذراً لا يوجد بيانات مضافة في شبكة البيانات", "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                return;
            }

            btnSave.Enabled = false;
            try
            {
                int voucherId;
                decimal grandTotal;
                if (!int.TryParse(txtVoucherID.Text, out voucherId)) return;
                if (!decimal.TryParse(txtGrandTotal.Text, out grandTotal)) return;

                string queryVoucher = "INSERT INTO Voucher(Id, VoucherNo, Date, Name, Details, GrandTotal) VALUES (@d1, @d2, @d3, @d4, @d5, @d7)";
                SqlParameter[] voucherParams = {
                    new SqlParameter("@d1", voucherId),
                    new SqlParameter("@d2", txtVoucherNo.Text),
                    new SqlParameter("@d3", dtpDate.Value.Date),
                    new SqlParameter("@d4", txtName.Text),
                    new SqlParameter("@d5", txtDetails.Text),
                    new SqlParameter("@d7", grandTotal)
                };
                
                DataAccessLayer.ExecuteNonQuery(queryVoucher, CommandType.Text, voucherParams);

                foreach (DataGridViewRow row in DataGridView1.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        string queryDetail = "INSERT INTO Voucher_OtherDetails(VoucherID, Particulars, Amount, Note) VALUES (@d1, @d2, @d3, @d4)";
                        SqlParameter[] detailParams = {
                            new SqlParameter("@d1", voucherId),
                            new SqlParameter("@d2", row.Cells[0].Value),
                            new SqlParameter("@d3", Convert.ToDecimal(row.Cells[1].Value)),
                            new SqlParameter("@d4", row.Cells[2].Value ?? (object)DBNull.Value)
                        };
                        DataAccessLayer.ExecuteNonQuery(queryDetail, CommandType.Text, detailParams);
                    }
                }

                LogFunc(lblUser.Text, "added the new voucher having voucher no. '" + txtVoucherNo.Text + "'");
                LedgerSave(dtpDate.Value.Date, txtName.Text, txtVoucherNo.Text, "مصروفات", 0, grandTotal, "", "");
                System.Windows.Forms.MessageBox.Show("تم الحفظ بنجاح", "سندات الصرف", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                Print();
                Reset();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            finally { btnSave.Enabled = true; }
        }

        public static void LedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g, string h)
        {
            try
            {
                string query = "INSERT INTO LedgerBook(Date, Name, LedgerNo, Label, Debit, Credit, PartyID, Manual_Inv) VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8)";
                SqlParameter[] parameters = {
                    new SqlParameter("@d1", a),
                    new SqlParameter("@d2", b),
                    new SqlParameter("@d3", c),
                    new SqlParameter("@d4", d),
                    new SqlParameter("@d5", e),
                    new SqlParameter("@d6", f),
                    new SqlParameter("@d7", g),
                    new SqlParameter("@d8", h)
                };
                DataAccessLayer.ExecuteNonQuery(query, CommandType.Text, parameters);
            }
            catch { }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("هل أنت متأكد أنك تريد حذف سجل السند?", "تأكيد", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                btnDelete.Enabled = false;
                try { DeleteRecord(); }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
                finally { if (btnSave.Enabled) btnDelete.Enabled = false; }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                System.Windows.Forms.MessageBox.Show("الرجاء كتابة اسم السند", "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            if (DataGridView1.Rows.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("عذرًا لا يوجد بيانات مضافة في شبكة البيانات", "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                return;
            }

            btnUpdate.Enabled = false;
            try
            {
                int voucherId;
                decimal grandTotal;
                if (!int.TryParse(txtVoucherID.Text, out voucherId)) return;
                if (!decimal.TryParse(txtGrandTotal.Text, out grandTotal)) return;

                string queryVoucher = "UPDATE Voucher SET VoucherNo = @d2, Date = @d3, Name = @d4, Details = @d5, GrandTotal = @d7 WHERE ID = @d1";
                SqlParameter[] voucherParams = {
                    new SqlParameter("@d1", voucherId),
                    new SqlParameter("@d2", txtVoucherNo.Text),
                    new SqlParameter("@d3", dtpDate.Value.Date),
                    new SqlParameter("@d4", txtName.Text),
                    new SqlParameter("@d5", txtDetails.Text),
                    new SqlParameter("@d7", grandTotal)
                };
                DataAccessLayer.ExecuteNonQuery(queryVoucher, CommandType.Text, voucherParams);

                DataAccessLayer.ExecuteNonQuery("DELETE FROM Voucher_OtherDetails WHERE VoucherID = @VoucherID", CommandType.Text, new SqlParameter("@VoucherID", voucherId));

                foreach (DataGridViewRow row in DataGridView1.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        string queryInsert = "INSERT INTO Voucher_OtherDetails(VoucherID, Particulars, Amount, Note) VALUES (@VoucherID, @d1, @d2, @d3)";
                        SqlParameter[] parameters = {
                            new SqlParameter("@VoucherID", voucherId),
                            new SqlParameter("@d1", row.Cells[0].Value),
                            new SqlParameter("@d2", Convert.ToDecimal(row.Cells[1].Value)),
                            new SqlParameter("@d3", row.Cells[2].Value ?? (object)DBNull.Value)
                        };
                        DataAccessLayer.ExecuteNonQuery(queryInsert, CommandType.Text, parameters);
                    }
                }

                LogFunc(lblUser.Text, "updated the voucher having voucher no. '" + txtVoucherNo.Text + "'");
                LedgerUpdate(dtpDate.Value.Date, txtName.Text, 0, grandTotal, "", txtVoucherNo.Text, "مصروفات");
                System.Windows.Forms.MessageBox.Show("تم التعديل بنجاح", "سندات الصرف", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                Reset();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            finally { if (btnSave.Enabled) btnUpdate.Enabled = false; }
        }

        public static void LedgerUpdate(DateTime a, string b, decimal e, decimal f, string g, string h, string i)
        {
            try
            {
                string query = "UPDATE LedgerBook SET Date=@d1, Name=@d2, Debit=@d3, Credit=@d4, PartyID=@d5 WHERE LedgerNo=@d6 AND Label=@d7";
                SqlParameter[] parameters = {
                    new SqlParameter("@d1", a),
                    new SqlParameter("@d2", b),
                    new SqlParameter("@d3", e),
                    new SqlParameter("@d4", f),
                    new SqlParameter("@d5", g),
                    new SqlParameter("@d6", h),
                    new SqlParameter("@d7", i)
                };
                DataAccessLayer.ExecuteNonQuery(query, CommandType.Text, parameters);
            }
            catch { }
        }

        private void DataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            btnRemove.Enabled = true;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            btnRemove.Enabled = false;
            try
            {
                foreach (DataGridViewRow row in DataGridView1.SelectedRows)
                {
                    if (!row.IsNewRow) DataGridView1.Rows.Remove(row);
                }
                txtGrandTotal.Text = Math.Round(GrandTotal(), 2).ToString();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            finally { btnRemove.Enabled = false; }
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            btnGetData.Enabled = false;
            try
            {
                VoucherRecord frm = new VoucherRecord();
                frm.Reset();
                frm.Show();
            }
            finally { btnGetData.Enabled = true; }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            btnSave_Click(sender, e);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            btnSelect.Enabled = false;
            try
            {
                SalesmanRecord frm = new SalesmanRecord();
                frm.lblSet.Text = "voucher";
                frm.Reset();
                frm.Show();
            }
            finally { btnSelect.Enabled = true; }
        }

        private void txtName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string query = "SELECT Name FROM Salesman WHERE Name=@d1";
                SqlParameter[] parameters = { new SqlParameter("@d1", txtName.Text) };
                using (SqlDataReader rdr = DataAccessLayer.ExecuteReader(query, CommandType.Text, parameters))
                {
                    if (rdr.Read())
                    {
                        txtName.Text = rdr[0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void fillSaleMan()
        {
            try
            {
                DataTable dtable = DataAccessLayer.ExecuteTable("SELECT RTRIM(Name) FROM Salesman", CommandType.Text);
                txtName.Items.Clear();
                foreach (DataRow drow in dtable.Rows)
                {
                    txtName.Items.Add(drow[0].ToString());
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
    }
}
