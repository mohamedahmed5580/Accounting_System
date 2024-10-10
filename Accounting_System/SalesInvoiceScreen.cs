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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Accounting_System
{
    public partial class SalesInvoiceScreen : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        private static SalesInvoiceScreen _instance;
        public static SalesInvoiceScreen Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new SalesInvoiceScreen();
                }
                return _instance;
            }
        }
        public SalesInvoiceScreen()
        {
            InitializeComponent();
        }
        private void SalesInvoiceScreen_Load(object sender, EventArgs e)
        {
            Getdata();
            fillInvoiceNo();
            var total1 = default(double);
            var total2 = default(double);
            var total3 = default(double);
            // Dim Row1 As DataGridViewRow
            foreach (DataGridViewRow Row in dgw.Rows)
            {
                DataGridViewTextBoxCell celv = Row.Cells[10] as DataGridViewTextBoxCell;
                DataGridViewTextBoxCell celv1 = Row.Cells[11] as DataGridViewTextBoxCell;
                DataGridViewTextBoxCell celv2 = Row.Cells[12] as DataGridViewTextBoxCell;


                if (Information.IsNumeric(celv.Value) == true)
                {
                    total1 += Convert.ToDouble(celv.Value);
                    total2 += Convert.ToDouble(celv1.Value);
                    total3 += Convert.ToDouble(celv2.Value);
                }

            }
            TextBox1.Text = total1.ToString();
            TextBox2.Text = total2.ToString();
            TextBox3.Text = total3.ToString();
           

        }
        public void Getdata()
        {
            try
            {
                con.Open();
                SqlCommand  cmd = new SqlCommand("Select Inv_ID, RTRIM(InvoiceNo), InvoiceDate,SM_ID, RTRIM(Salesman_ID),RTRIM(Salesman.Name),Customer.ID,RTRIM(Customer.CustomerID),RTRIM(Customer.Name),RTRIM(Customer.ContactNo), GrandTotal, TotalPaid, Balance, RTRIM(InvoiceInfo.Remarks) from Customer,InvoiceInfo,Salesman where Customer.ID=InvoiceInfo.CustomerID and Salesman.SM_ID=InvoiceInfo.SalesmanID order by InvoiceDate Desc", con);
                SqlDataReader  rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dgw.Rows.Clear();
                while (rdr.Read() == true)
                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13]);
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            foreach (DataGridViewRow row in dgw.Rows)
            {

                if (row.Cells["Column9"].Value.ToString() == "0")
                {
                    row.DefaultCellStyle.BackColor = Color.Green;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                }
            }
        }

        private void gunaAdvenceButton1_Click(object sender, EventArgs e)
        {
            if (dgw.SelectedRows.Count > 0)
            {
                try
                {
                    // Get the selected row from DataGridView
                    DataGridViewRow dr = dgw.SelectedRows[0];

                    // Swap values in column 12 and 11
                    string cell12 = dr.Cells[12].Value.ToString();  // GrandTotal
                /*    dr.Cells[12].Value = 0;  // Set GrandTotal to 0
                    dr.Cells[11].Value = cell12;  // Swap column 11 and 12 values (TotalPaid)
*/
                    // Get the invoice number from the first column (string)
                    string invoiceNo = dr.Cells[1].Value.ToString();

                    // Get the TotalPaid column value from column 11 after swapping
                    string TotalPaid = dr.Cells[10].Value.ToString();

                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();

                        // Prepare the SQL UPDATE command to update Balance and TotalPaid
                        string query = "UPDATE InvoiceInfo SET Balance = @Balance, TotalPaid = @TotalPaid WHERE InvoiceNo = @InvoiceNo";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            // Add parameters to the SQL query
                            cmd.Parameters.AddWithValue("@Balance", 0);  // Setting Balance to 0
                            cmd.Parameters.AddWithValue("@TotalPaid", TotalPaid);  // Setting TotalPaid to the swapped value
                            cmd.Parameters.AddWithValue("@InvoiceNo", invoiceNo);  // InvoiceNo is a string

                            // Execute the query
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("تم دفع الفاتوره بنجاح", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("No rows were updated. Please check the InvoiceNo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        Getdata();
                        con.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a row first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void dgw_RowPostPaint(object sender, System.Windows.Forms.DataGridViewRowPostPaintEventArgs e)
        {
            string strRowNumber = (e.RowIndex + 1).ToString();
            SizeF size = e.Graphics.MeasureString(strRowNumber, this.Font);
            if (dgw.RowHeadersWidth < Convert.ToInt32(size.Width + 20))
            {
                dgw.RowHeadersWidth = Convert.ToInt32(size.Width + 20);
            }
            Brush b = SystemBrushes.ControlText;
            e.Graphics.DrawString(strRowNumber, this.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + (e.RowBounds.Height - size.Height) / 2);

        }


        public void Reset()
        {
            cmbInvoiceNo.Text = "";
            txtCustomerName.Text = "";
            txtSalesman.Text = "";
            fillInvoiceNo();
            dtpDateFrom.Text = DateTime.Today.ToString();
            dtpDateTo.Text = DateTime.Today.ToString();
            DateTimePicker2.Text = DateTime.Today.ToString();
            DateTimePicker1.Text = DateTime.Today.ToString();
            Getdata();
        }

        public void fillInvoiceNo()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    SqlDataAdapter adp = new SqlDataAdapter("SELECT DISTINCT RTRIM(InvoiceNo) FROM InvoiceInfo", con);
                    DataSet ds = new DataSet();
                    adp.Fill(ds);
                    System.Data.DataTable dtable = ds.Tables[0]; // Fully qualified DataTable
                    cmbInvoiceNo.Items.Clear();
                    foreach (System.Data.DataRow drow in dtable.Rows) // Fully qualified DataRow
                    {
                        cmbInvoiceNo.Items.Add(drow[0].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnGetData_Click(object sender, EventArgs e)
        {

            try
            {
                con.Open();
                SqlCommand  cmd = new SqlCommand("Select Inv_ID, RTRIM(InvoiceNo), InvoiceDate,SM_ID, RTRIM(Salesman_ID),RTRIM(Salesman.Name),Customer.ID,RTRIM(Customer.CustomerID),RTRIM(Customer.Name),RTRIM(Customer.ContactNo), GrandTotal, TotalPaid, Balance, RTRIM(InvoiceInfo.Remarks) from Customer,InvoiceInfo,Salesman where Customer.ID=InvoiceInfo.CustomerID and Salesman.SM_ID=InvoiceInfo.SalesmanID and InvoiceDate between @d1 and @d2 order by InvoiceDate Desc", con);
                cmd.Parameters.Add("@d1", SqlDbType.DateTime, 30, "Date").Value = dtpDateFrom.Value.Date;
                cmd.Parameters.Add("@d2", SqlDbType.DateTime, 30, "Date").Value = dtpDateTo.Value.Date;
                SqlDataReader  rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dgw.Rows.Clear();
                while (rdr.Read() == true)
                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13]);
                con.Close();
                var total1 = default(double);
                var total2 = default(double);
                var total3 = default(double);
                // Dim Row1 As DataGridViewRow
                foreach (DataGridViewRow Row in dgw.Rows)
                {
                    DataGridViewTextBoxCell celv = Row.Cells[10] as DataGridViewTextBoxCell;
                    DataGridViewTextBoxCell celv1 = Row.Cells[11] as DataGridViewTextBoxCell;
                    DataGridViewTextBoxCell celv2 = Row.Cells[12] as DataGridViewTextBoxCell;


                    if (Information.IsNumeric(celv.Value) == true)
                    {
                        total1 += Convert.ToDouble(celv.Value);
                        total2 += Convert.ToDouble(celv1.Value);
                        total3 += Convert.ToDouble(celv2.Value);
                    }


                }
                TextBox1.Text = total1.ToString();
                TextBox2.Text = total2.ToString();
                TextBox3.Text = total3.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
            var total1 = default(double);
            var total2 = default(double);
            var total3 = default(double);
            // Dim Row1 As DataGridViewRow
            foreach (DataGridViewRow Row in dgw.Rows)
            {
                DataGridViewTextBoxCell celv = Row.Cells[10] as DataGridViewTextBoxCell;
                DataGridViewTextBoxCell celv1 = Row.Cells[11] as DataGridViewTextBoxCell;
                DataGridViewTextBoxCell celv2 = Row.Cells[12] as DataGridViewTextBoxCell;


                if (Information.IsNumeric(celv.Value) == true)
                {
                    total1 += Convert.ToDouble(celv.Value);
                    total2 += Convert.ToDouble(celv1.Value);
                    total3 += Convert.ToDouble(celv2.Value);
                }


            }
            TextBox1.Text = total1.ToString();
            TextBox2.Text = total2.ToString();
            TextBox3.Text = total3.ToString();
        }

        private void cmbInvoiceNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand  cmd = new SqlCommand("Select Inv_ID, RTRIM(InvoiceNo), InvoiceDate,SM_ID, RTRIM(Salesman_ID),RTRIM(Salesman.Name),Customer.ID,RTRIM(Customer.CustomerID),RTRIM(Customer.Name),RTRIM(Customer.ContactNo), GrandTotal, TotalPaid, Balance, RTRIM(InvoiceInfo.Remarks) from Customer,InvoiceInfo,Salesman where Customer.ID=InvoiceInfo.CustomerID and Salesman.SM_ID=InvoiceInfo.SalesmanID and InvoiceNo='" + cmbInvoiceNo.Text + "' order by InvoiceDate Desc", con);
                SqlDataReader  rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dgw.Rows.Clear();
                while (rdr.Read() == true)
                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13]);
                con.Close();
                var total1 = default(double);
                var total2 = default(double);
                var total3 = default(double);
                // Dim Row1 As DataGridViewRow
                foreach (DataGridViewRow Row in dgw.Rows)
                {
                    DataGridViewTextBoxCell celv = Row.Cells[10] as DataGridViewTextBoxCell;
                    DataGridViewTextBoxCell celv1 = Row.Cells[11] as DataGridViewTextBoxCell;
                    DataGridViewTextBoxCell celv2 = Row.Cells[12] as DataGridViewTextBoxCell;


                    if (Information.IsNumeric(celv.Value) == true)
                    {
                        total1 += Convert.ToDouble(celv.Value);
                        total2 += Convert.ToDouble(celv1.Value);
                        total3 += Convert.ToDouble(celv2.Value);
                    }


                }
                TextBox1.Text = total1.ToString();
                TextBox2.Text = total2.ToString();
                TextBox3.Text = total3.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand  cmd = new SqlCommand("Select Inv_ID, RTRIM(InvoiceNo), InvoiceDate,SM_ID, RTRIM(Salesman_ID),RTRIM(Salesman.Name),Customer.ID,RTRIM(Customer.CustomerID),RTRIM(Customer.Name),RTRIM(Customer.ContactNo), GrandTotal, TotalPaid, Balance, RTRIM(InvoiceInfo.Remarks) from Customer,InvoiceInfo,Salesman where Customer.ID=InvoiceInfo.CustomerID and Salesman.SM_ID=InvoiceInfo.SalesmanID and InvoiceDate between @d1 and @d2 and Balance > 0 order by InvoiceDate Desc", con);
                cmd.Parameters.Add("@d1", SqlDbType.DateTime, 30, "Date").Value = DateTimePicker2.Value.Date;
                cmd.Parameters.Add("@d2", SqlDbType.DateTime, 30, "Date").Value = DateTimePicker1.Value.Date;
                SqlDataReader  rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dgw.Rows.Clear();
                while (rdr.Read() == true)
                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13]);
                con.Close();
                var total1 = default(double);
                var total2 = default(double);
                var total3 = default(double);
                // Dim Row1 As DataGridViewRow
                foreach (DataGridViewRow Row in dgw.Rows)
                {
                    DataGridViewTextBoxCell celv = Row.Cells[10] as DataGridViewTextBoxCell;
                    DataGridViewTextBoxCell celv1 = Row.Cells[11] as DataGridViewTextBoxCell;
                    DataGridViewTextBoxCell celv2 = Row.Cells[12] as DataGridViewTextBoxCell;


                    if (Information.IsNumeric(celv.Value) == true)
                    {
                        total1 += Convert.ToDouble(celv.Value);
                        total2 += Convert.ToDouble(celv1.Value);
                        total3 += Convert.ToDouble(celv2.Value);
                    }


                }
                TextBox1.Text = total1.ToString();
                TextBox2.Text = total2.ToString();
                TextBox3.Text = total3.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtCustomerName_TextChanged(object sender, EventArgs e)
        {
            
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Select Inv_ID, RTRIM(InvoiceNo), InvoiceDate,SM_ID, RTRIM(Salesman_ID),RTRIM(Salesman.Name),Customer.ID,RTRIM(Customer.CustomerID),RTRIM(Customer.Name),RTRIM(Customer.ContactNo), GrandTotal, TotalPaid, Balance, RTRIM(InvoiceInfo.Remarks) from Customer,InvoiceInfo,Salesman where Customer.ID=InvoiceInfo.CustomerID and Salesman.SM_ID=InvoiceInfo.SalesmanID and Customer.Name like '%" + txtCustomerName.Text + "%' order by InvoiceDate Desc", con); cmd.Parameters.Add("@d1", SqlDbType.DateTime, 30, "Date").Value = DateTimePicker2.Value.Date;
                cmd.Parameters.Add("@d2", SqlDbType.DateTime, 30, "Date").Value = DateTimePicker1.Value.Date;
                SqlDataReader  rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dgw.Rows.Clear();
                while (rdr.Read() == true)
                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13]);
                con.Close();
                var total1 = default(double);
                var total2 = default(double);
                var total3 = default(double);
                // Dim Row1 As DataGridViewRow
                foreach (DataGridViewRow Row in dgw.Rows)
                {
                    DataGridViewTextBoxCell celv = Row.Cells[10] as DataGridViewTextBoxCell;
                    DataGridViewTextBoxCell celv1 = Row.Cells[11] as DataGridViewTextBoxCell;
                    DataGridViewTextBoxCell celv2 = Row.Cells[12] as DataGridViewTextBoxCell;


                    if (Information.IsNumeric(celv.Value) == true)
                    {
                        total1 += Convert.ToDouble(celv.Value);
                        total2 += Convert.ToDouble(celv1.Value);
                        total3 += Convert.ToDouble(celv2.Value);
                    }


                }
                TextBox1.Text = total1.ToString();
                TextBox2.Text = total2.ToString();
                TextBox3.Text = total3.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbInvoiceNo_Format(object sender, ListControlConvertEventArgs e)
        {
            if (object.ReferenceEquals(e.DesiredType, typeof(string)))
            {
                e.Value = e.Value.ToString();
            }
        }

        private void txtSalesman_TextChanged(object sender, EventArgs e)
        {
            
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Select Inv_ID, RTRIM(InvoiceNo), InvoiceDate,SM_ID, RTRIM(Salesman_ID),RTRIM(Salesman.Name),Customer.ID,RTRIM(Customer.CustomerID),RTRIM(Customer.Name),RTRIM(Customer.ContactNo), GrandTotal, TotalPaid, Balance, RTRIM(InvoiceInfo.Remarks) from Customer,InvoiceInfo,Salesman where Customer.ID=InvoiceInfo.CustomerID and Salesman.SM_ID=InvoiceInfo.SalesmanID and Salesman.Name like '%" + txtSalesman.Text + "%' order by InvoiceDate Desc", con);
                cmd.Parameters.Add("@d2", SqlDbType.DateTime, 30, "Date").Value = DateTimePicker1.Value.Date;
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dgw.Rows.Clear();
                while (rdr.Read() == true)
                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13]);
                con.Close();
                var total1 = default(double);
                var total2 = default(double);
                var total3 = default(double);
                // Dim Row1 As DataGridViewRow
                foreach (DataGridViewRow Row in dgw.Rows)
                {
                    DataGridViewTextBoxCell celv = Row.Cells[10] as DataGridViewTextBoxCell;
                    DataGridViewTextBoxCell celv1 = Row.Cells[11] as DataGridViewTextBoxCell;
                    DataGridViewTextBoxCell celv2 = Row.Cells[12] as DataGridViewTextBoxCell;


                    if (Information.IsNumeric(celv.Value) == true)
                    {
                        total1 += Convert.ToDouble(celv.Value);
                        total2 += Convert.ToDouble(celv1.Value);
                        total3 += Convert.ToDouble(celv2.Value);
                    }


                }
                TextBox1.Text = total1.ToString();
                TextBox2.Text = total2.ToString();
                TextBox3.Text = total3.ToString();
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
                    if (lblSet.Text == "Sales Invoice")
                    {
                        DataGridViewRow dr = dgw.SelectedRows[0];


                        // Setting the fields in the POS form from the selected DataGridView row
                        POS.instance.Reset();   
                        POS.instance.txtID.Text = dr.Cells[0].Value.ToString();
                        POS.instance.txtInvoiceNo.Text = dr.Cells[1].Value.ToString();
                        POS.instance.dtpInvoiceDate.Text = dr.Cells[2].Value.ToString();
                        POS.instance.txtSM_ID.Text = dr.Cells[3].Value.ToString();
                        POS.instance.txtSalesmanID.Text = dr.Cells[4].Value.ToString();
                        POS.instance.txtSalesman.Text = dr.Cells[5].Value.ToString();
                        POS.instance.txtCustomerID.Text = dr.Cells[7].Value.ToString();
                        POS.instance.txtCID.Text = dr.Cells[6].Value.ToString();
                        POS.instance.txtCustomerName.Text = dr.Cells[8].Value.ToString();
                        POS.instance.txtContactNo.Text = dr.Cells[9].Value.ToString();
                        POS.instance.txtGrandTotal.Text = dr.Cells[10].Value.ToString();
                        POS.instance.txtTotalPayment.Text = dr.Cells[11].Value.ToString();
                        POS.instance.txtPaymentDue.Text = dr.Cells[12].Value.ToString();
                        POS.instance.txtRemarks.Text = dr.Cells[13].Value.ToString();

                        // Disabling and enabling relevant controls
                        POS.instance.btnSave.Enabled = false;
                        POS.instance.Button2.Enabled = false;
                        POS.instance.Button3.Enabled = true;
                        POS.instance.btnUpdate.Enabled = true;
                        POS.instance.btnPrint.Enabled = true;
                        POS.instance.btnDelete.Enabled = true;
                        POS.instance.lblSet.Text = "Not Allowed";
                        POS.instance.btnAdd.Enabled = true;
                        POS.instance.txtCustomerName.ReadOnly = true;
                        POS.instance.txtContactNo.ReadOnly = true;
                        POS.instance.button4.Enabled = true;
                        POS.instance.dataGridView3.Rows.Clear();
                        POS.instance.btnUpdate.Enabled=true;

                        // Fetching and setting data for DataGridView1
                        con.Open();
                        string sql = "SELECT RTRIM(ProductCode), RTRIM(ProductName), RTRIM(Invoice_Product.Barcode), Invoice_Product.CostPrice, Invoice_Product.SellingPrice, Invoice_Product.Margin, Invoice_Product.Qty, Invoice_Product.Amount, Invoice_Product.DiscountPer, Invoice_Product.Discount, Invoice_Product.VATPer, Invoice_Product.VAT, Invoice_Product.TotalAmount, Product.PID FROM InvoiceInfo INNER JOIN Invoice_Product ON InvoiceInfo.Inv_ID = Invoice_Product.InvoiceID INNER JOIN Product ON Product.PID = Invoice_Product.ProductID WHERE InvoiceInfo.Inv_ID = @d1";
                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.Parameters.AddWithValue("@d1", dr.Cells[0].Value);
                        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        POS.instance.DataGridView1.Rows.Clear();
                        while (rdr.Read())
                        {
                            POS.instance.DataGridView1.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13]);
                            POS.instance.dataGridView3.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13]);
                        }
                        con.Close();

                        // Fetching and setting data for DataGridView2
                        con.Open();
                        string sql1 = "SELECT RTRIM(PaymentMode), Invoice_Payment.TotalPaid, PaymentDate FROM InvoiceInfo INNER JOIN Invoice_Payment ON InvoiceInfo.Inv_ID = Invoice_Payment.InvoiceID WHERE InvoiceInfo.Inv_ID = @d1";
                        cmd = new SqlCommand(sql1, con);
                        cmd.Parameters.AddWithValue("@d1", dr.Cells[0].Value);
                        rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        POS.instance.DataGridView2.Rows.Clear();
                        while (rdr.Read())
                        {
                            POS.instance.DataGridView2.Rows.Add(rdr[0], rdr[1], rdr[2]);
                        }
                        con.Close();

                        // Fetching and setting the customer type
                        con.Open();
                        string ct = "SELECT RTRIM(CustomerType) FROM Customer WHERE ID = @customerId";
                        cmd = new SqlCommand(ct, con);
                        cmd.Parameters.AddWithValue("@customerId", dr.Cells[3].Value);
                        rdr = cmd.ExecuteReader();
                        if (rdr.Read())
                        {
                            POS.instance.txtCustomerType.Text = rdr[0].ToString();
                        }
                        rdr.Close();
                        con.Close();

                        POS.instance.Show();
                        this.Close();
                    }

                    if (lblSet.Text == "SR")
                    {
                        DataGridViewRow dr = dgw.SelectedRows[0];
                        SalesReturn frmSalesReturn =  SalesReturn.instance;

                        frmSalesReturn.txtSalesID.Text = dr.Cells[0].Value.ToString();
                        frmSalesReturn.txtSalesInvoiceNo.Text = dr.Cells[1].Value.ToString();
                        frmSalesReturn.dtpSalesDate.Text = dr.Cells[2].Value.ToString();
                        frmSalesReturn.txtCustomerID.Text = dr.Cells[7].Value.ToString();
                        frmSalesReturn.txtcust_ID.Text = dr.Cells[6].Value.ToString();
                        frmSalesReturn.txtCustomerName.Text = dr.Cells[8].Value.ToString();
                        frmSalesReturn.txtGrandTotal.Text = dr.Cells[10].Value.ToString();
                        frmSalesReturn.auto();
                        // Fetching and setting data for DataGridView2 in SalesReturn form
                        con.Open();
                        string sql = "SELECT RTRIM(ProductCode), RTRIM(ProductName), RTRIM(Invoice_Product.Barcode), Invoice_Product.SellingPrice, Invoice_Product.Qty, Invoice_Product.Amount, Invoice_Product.DiscountPer, Invoice_Product.Discount, Invoice_Product.VATPer, Invoice_Product.VAT, Invoice_Product.TotalAmount, Product.PID, Invoice_Product.CostPrice, Margin FROM InvoiceInfo INNER JOIN Invoice_Product ON InvoiceInfo.Inv_ID = Invoice_Product.InvoiceID INNER JOIN Product ON Product.PID = Invoice_Product.ProductID WHERE InvoiceInfo.Inv_ID = @d1";
                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.Parameters.AddWithValue("@d1", dr.Cells[0].Value);
                        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        frmSalesReturn.DataGridView2.Rows.Clear();
                        while (rdr.Read())
                        {
                            frmSalesReturn.DataGridView2.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13]);
                        }
                        con.Close();
                        this.Close();

                    }

                    if (lblSet.Text == "1")
                    {
                        DataGridViewRow dr = dgw.SelectedRows[0];
                        POS pOS = new POS();

                        pOS.txtID.Text = dr.Cells[0].Value.ToString();
                        pOS.txtInvoiceNo.Text = dr.Cells[1].Value.ToString();
                        pOS.dtpInvoiceDate.Text = dr.Cells[2].Value.ToString();
                        pOS.txtSM_ID.Text = dr.Cells[3].Value.ToString();
                        pOS.txtSalesmanID.Text = dr.Cells[4].Value.ToString();
                        pOS.txtSalesman.Text = dr.Cells[5].Value.ToString();
                        pOS.txtCustomerID.Text = dr.Cells[7].Value.ToString();
                        pOS.txtCID.Text = dr.Cells[6].Value.ToString();
                        pOS.txtCustomerName.Text = dr.Cells[8].Value.ToString();
                        pOS.txtContactNo.Text = dr.Cells[9].Value.ToString();
                        pOS.txtGrandTotal.Text = dr.Cells[10].Value.ToString();
                        pOS.txtTotalPayment.Text = dr.Cells[11].Value.ToString();
                        pOS.txtPaymentDue.Text = dr.Cells[12].Value.ToString();
                        pOS.txtRemarks.Text = dr.Cells[13].Value.ToString();

                        pOS.btnSave.Enabled = false;
                        pOS.Button2.Enabled = false;
                        pOS.Button3.Enabled = true;
                        pOS.btnUpdate.Enabled = false;
                        pOS.btnPrint.Enabled = true;
                        pOS.btnDelete.Enabled = false;
                        pOS.btnSelect.Enabled = false;
                        pOS.Button1.Enabled = false;
                        pOS.btnGetData.Enabled = false;
                        pOS.btnNew.Enabled = false;
                        pOS.btnListReset.Enabled = false;
                        pOS.btnAdd.Enabled = false;
                        pOS.txtCustomerName.ReadOnly = false;
                        pOS.txtContactNo.ReadOnly = false;

                        // Fetching and setting data for DataGridView1 in POS form
                        con.Open();
                        string sql = "SELECT RTRIM(ProductCode), RTRIM(ProductName), RTRIM(Invoice_Product.Barcode), Invoice_Product.CostPrice, Invoice_Product.SellingPrice, Invoice_Product.Margin, Invoice_Product.Qty, Invoice_Product.Amount, Invoice_Product.DiscountPer, Invoice_Product.Discount, Invoice_Product.VATPer, Invoice_Product.VAT, Invoice_Product.TotalAmount, Product.PID FROM InvoiceInfo INNER JOIN Invoice_Product ON InvoiceInfo.Inv_ID = Invoice_Product.InvoiceID INNER JOIN Product ON Product.PID = Invoice_Product.ProductID WHERE InvoiceInfo.Inv_ID = @d1";
                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.Parameters.AddWithValue("@d1", dr.Cells[0].Value);
                        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        pOS.DataGridView1.Rows.Clear();
                        while (rdr.Read())
                        {
                            pOS.DataGridView1.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13]);
                        }
                        con.Close();

                        // Fetching and setting data for DataGridView2 in POS form
                        con.Open();
                        string sql1 = "SELECT RTRIM(PaymentMode), Invoice_Payment.TotalPaid, PaymentDate FROM InvoiceInfo INNER JOIN Invoice_Payment ON InvoiceInfo.Inv_ID = Invoice_Payment.InvoiceID WHERE InvoiceInfo.Inv_ID = @d1";
                        cmd = new SqlCommand(sql1, con);
                        cmd.Parameters.AddWithValue("@d1", dr.Cells[0].Value);
                        rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        pOS.DataGridView2.Rows.Clear();
                        while (rdr.Read())
                        {
                            pOS.DataGridView2.Rows.Add(rdr[0], rdr[1], rdr[2]);
                        }
                        con.Close();

                        pOS.Show();
                        this.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                TextBox2.Visible = true;
                Label4.Visible = true;
                Label7.Visible = true;
                Label8.Visible = true;
                TextBox1.Visible = true; 
                TextBox3.Visible = true;
            }
            else
            {
                TextBox2.Visible = false;
                Label4.Visible = false;
                Label7.Visible = false;
                Label8.Visible = false;
                TextBox1.Visible = false;
                TextBox3.Visible = false;
            }
        }

        private void TextBox4_TextChanged(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Select Inv_ID, RTRIM(InvoiceNo), InvoiceDate,SM_ID, RTRIM(Salesman_ID),RTRIM(Salesman.Name),Customer.ID,RTRIM(Customer.CustomerID),RTRIM(Customer.Name),RTRIM(Customer.ContactNo), GrandTotal, TotalPaid, Balance, RTRIM(InvoiceInfo.Remarks) from Customer,InvoiceInfo,Salesman where Customer.ID=InvoiceInfo.CustomerID and Salesman.SM_ID=InvoiceInfo.SalesmanID and InvoiceInfo.Remarks like '%" + TextBox4.Text + "%' order by InvoiceDate Desc", con);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dgw.Rows.Clear();
                while (rdr.Read() == true)
                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13]);
                con.Close();
                var total1 = default(double);
                var total2 = default(double);
                var total3 = default(double);
                // Dim Row1 As DataGridViewRow
                foreach (DataGridViewRow Row in dgw.Rows)
                {
                    DataGridViewTextBoxCell celv = Row.Cells[10] as DataGridViewTextBoxCell;
                    DataGridViewTextBoxCell celv1 = Row.Cells[11] as DataGridViewTextBoxCell;
                    DataGridViewTextBoxCell celv2 = Row.Cells[12] as DataGridViewTextBoxCell;


                    if (Information.IsNumeric(celv.Value) == true)
                    {
                        total1 += Convert.ToDouble(celv.Value);
                        total2 += Convert.ToDouble(celv1.Value);
                        total3 += Convert.ToDouble(celv2.Value);
                    }


                }
                TextBox1.Text = total1.ToString();
                TextBox2.Text = total2.ToString();
                TextBox3.Text = total3.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GroupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

       

    }
}

