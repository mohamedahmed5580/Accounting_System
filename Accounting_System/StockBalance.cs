using AltoControls;
using Microsoft.Office.Interop.Excel;
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
using Excel = Microsoft.Office.Interop.Excel;

namespace Accounting_System
{
    public partial class StockBalance : Form
    {
        SqlConnection cn = new SqlConnection(DataAccessLayer.Con());
        public StockBalance()
        {
            InitializeComponent();
            txtBarcode.TextChanged += new EventHandler(txtBarcode_TextChanged);
            txtProductName.TextChanged += new EventHandler(txtProductName_TextChanged);

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtBarcode.Text = "";
            txtProductName.Text = "";
            slideButton2.IsOn = false;
            TextBox2.Visible = false;
            TextBox3.Visible = false;
            TextBox4.Visible = false;
            Label2.Visible = false;
            Label3.Visible = false;
            Label5.Visible = false;
            Getdata();
            double total1 = 0;
            double total2 = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue; // Skip the new row placeholder

                var celv = row.Cells[3] as DataGridViewTextBoxCell;
                var celv1 = row.Cells[4] as DataGridViewTextBoxCell;
                var celv2 = row.Cells[9] as DataGridViewTextBoxCell;

                if (celv != null && celv1 != null && celv2 != null && celv.Value != null && celv1.Value != null && celv2.Value != null)
                {
                    if (double.TryParse(celv.Value.ToString(), out double value1) &&
                        double.TryParse(celv1.Value.ToString(), out double value2) &&
                        double.TryParse(celv2.Value.ToString(), out double quantity))
                    {
                        total1 += quantity * value1;
                        total2 += value2 * quantity;
                    }
                }
            }

            TextBox2.Text = total1.ToString();
            TextBox3.Text = total2.ToString();
            TextBox4.Text = (total2 - total1).ToString();

        }

        private void StockBalance_Load(object sender, EventArgs e)
        {
            Getdata();



            dataGridView1.ClearSelection();


            dataGridView1.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[7].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

            double total1 = 0;
            double total2 = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue; // Skip the new row placeholder

                var celv = row.Cells[3] as DataGridViewTextBoxCell;
                var celv1 = row.Cells[4] as DataGridViewTextBoxCell;
                var celv2 = row.Cells[9] as DataGridViewTextBoxCell;

                if (celv != null && celv1 != null && celv2 != null && celv.Value != null && celv1.Value != null && celv2.Value != null)
                {
                    if (double.TryParse(celv.Value.ToString(), out double value1) &&
                        double.TryParse(celv1.Value.ToString(), out double value2) &&
                        double.TryParse(celv2.Value.ToString(), out double quantity))
                    {
                        total1 += quantity * value1;
                        total2 += value2 * quantity;
                    }
                }
            }

            TextBox2.Text = total1.ToString();
            TextBox3.Text = total2.ToString();
            TextBox4.Text = (total2 - total1).ToString();
        }

        private void slideButton2_Click(object sender, EventArgs e)
        {
            if (slideButton2.IsOn)
            {
                TextBox2.Visible = true;
                Label2.Visible = true;
                Label3.Visible = true;
                Label5.Visible = true;
                TextBox3.Visible = true;
                TextBox4.Visible = true;
            }
            else
            {
                TextBox2.Visible = false;
                Label2.Visible = false;
                Label3.Visible = false;
                Label5.Visible = false;
                TextBox3.Visible = false;
                TextBox4.Visible = false;

            }
        }
        public void Getdata()
        {
            
            SqlCommand cmd;
            SqlDataReader rdr;


            cn.Open();
            cmd = new SqlCommand("SELECT RTRIM(Product.ProductCode),RTRIM(ProductName),RTRIM(Temp_Stock.Barcode),CostPrice,SellingPrice,Discount,VAT,ManufacturingDate,ExpiryDate,Qty from Temp_Stock,Product where Product.PID=Temp_Stock.ProductID and Qty > 0 order by ProductCode", cn);
            rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            dataGridView1.Rows.Clear();

            while (rdr.Read())
            {
                dataGridView1.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9]);

                foreach (DataGridViewRow k in dataGridView1.Rows)
                {
                    if (k.Cells[8].Value == DBNull.Value)
                    {
                        k.DefaultCellStyle.BackColor = Color.White;
                    }
                    else
                    {
                        DateTime ndate = DateTime.Now.Date;
                        DateTime sdate = Convert.ToDateTime(k.Cells[8].Value);
                        int diff = (sdate - ndate).Days;
                        if (diff < 0)
                        {
                            k.DefaultCellStyle.BackColor = Color.Red;
                        }
                        else if (diff < 30)
                        {
                            k.DefaultCellStyle.BackColor = Color.Blue;
                        }
                    }
                }
            }
            cn.Close();

            foreach (DataGridViewRow r in dataGridView1.Rows)
            {
                int i1 = 0, i2 = 0;


                cn.Open();
                string ct = "select ReorderPoint from Product where ProductCode=@d1";
                cmd = new SqlCommand(ct, cn);
                cmd.Parameters.AddWithValue("@d1", r.Cells[0].Value.ToString());
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    i1 = Convert.ToInt32(rdr.GetValue(0));
                }
                cn.Close();


                cn.Open();
                string ct1 = "select sum(Qty) from Product,Temp_Stock where Product.PID=Temp_Stock.ProductID and ProductCode=@d1";
                cmd = new SqlCommand(ct1, cn);
                cmd.Parameters.AddWithValue("@d1", r.Cells[0].Value.ToString());
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    i2 = Convert.ToInt32(rdr.GetValue(0));
                }
                cn.Close();

                if (i2 < i1)
                {
                    r.DefaultCellStyle.BackColor = Color.Cyan;
                }
            }
            dataGridView1.ClearSelection();
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            ExportExcel(dataGridView1);
        }
        public void ExportExcel(object obj)
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
        private void txtBarcode_TextChanged(object sender, EventArgs e)
        {
            SqlCommand cmd;
            SqlDataReader rdr;


            cn.Open();
            cmd = new SqlCommand("SELECT RTRIM(Product.ProductCode),RTRIM(ProductName),RTRIM(Temp_Stock.Barcode),CostPrice,SellingPrice,Discount,VAT,ManufacturingDate,ExpiryDate,Qty from Temp_Stock,Product WHERE Product.PID = Temp_Stock.ProductID AND Qty > 0 AND Temp_Stock.Barcode LIKE @barcode ORDER BY ProductCode", cn);
            cmd.Parameters.AddWithValue("@barcode", "%" + txtBarcode.Text + "%");
            rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            dataGridView1.Rows.Clear();

            while (rdr.Read())
            {
                dataGridView1.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9]);

                foreach (DataGridViewRow k in dataGridView1.Rows)
                {
                    if (k.Cells[8].Value == DBNull.Value)
                    {
                        k.DefaultCellStyle.BackColor = Color.White;
                    }
                    else
                    {
                        DateTime ndate = DateTime.Now.Date;
                        DateTime sdate = Convert.ToDateTime(k.Cells[8].Value);
                        int diff = (sdate - ndate).Days;
                        if (diff < 0)
                        {
                            k.DefaultCellStyle.BackColor = Color.Red;
                        }
                        else if (diff < 30)
                        {
                            k.DefaultCellStyle.BackColor = Color.Blue;
                        }
                    }
                }
            }
            cn.Close();

            foreach (DataGridViewRow r in dataGridView1.Rows)
            {
                int i1 = 0, i2 = 0;


                cn.Open();
                string ct = "select ReorderPoint from Product where ProductCode=@d1";
                cmd = new SqlCommand(ct, cn);
                cmd.Parameters.AddWithValue("@d1", r.Cells[0].Value.ToString());
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    i1 = Convert.ToInt32(rdr.GetValue(0));
                }
                cn.Close();


                cn.Open();
                string ct1 = "select sum(Qty) from Product,Temp_Stock where Product.PID=Temp_Stock.ProductID and ProductCode=@d1";
                cmd = new SqlCommand(ct1, cn);
                cmd.Parameters.AddWithValue("@d1", r.Cells[0].Value.ToString());
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    i2 = Convert.ToInt32(rdr.GetValue(0));
                }
                cn.Close();

                if (i2 < i1)
                {
                    r.DefaultCellStyle.BackColor = Color.Cyan;
                }
            }
            dataGridView1.ClearSelection();
            double total1 = 0;
            double total2 = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue; // Skip the new row placeholder

                var celv = row.Cells[3] as DataGridViewTextBoxCell;
                var celv1 = row.Cells[4] as DataGridViewTextBoxCell;
                var celv2 = row.Cells[9] as DataGridViewTextBoxCell;

                if (celv != null && celv1 != null && celv2 != null && celv.Value != null && celv1.Value != null && celv2.Value != null)
                {
                    if (double.TryParse(celv.Value.ToString(), out double value1) &&
                        double.TryParse(celv1.Value.ToString(), out double value2) &&
                        double.TryParse(celv2.Value.ToString(), out double quantity))
                    {
                        total1 += quantity * value1;
                        total2 += value2 * quantity;
                    }
                }
            }

            TextBox2.Text = total1.ToString();
            TextBox3.Text = total2.ToString();
            TextBox4.Text = (total2 - total1).ToString();
        }
        private void txtProductName_TextChanged(object sender, EventArgs e) 
        {
            SqlCommand cmd;
            SqlDataReader rdr;


            cn.Open();
            cmd = new SqlCommand("SELECT RTRIM(Product.ProductCode),RTRIM(ProductName),RTRIM(Temp_Stock.Barcode),CostPrice,SellingPrice,Discount,VAT,ManufacturingDate,ExpiryDate,Qty from Temp_Stock,Product WHERE Product.PID = Temp_Stock.ProductID AND Qty > 0 AND ProductName LIKE @productName ORDER BY ProductName", cn);
            cmd.Parameters.AddWithValue("@productName", "%" + txtProductName.Text + "%");
            rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            dataGridView1.Rows.Clear();

            while (rdr.Read())
            {
                dataGridView1.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9]);

                foreach (DataGridViewRow k in dataGridView1.Rows)
                {
                    if (k.Cells[8].Value == DBNull.Value)
                    {
                        k.DefaultCellStyle.BackColor = Color.White;
                    }
                    else
                    {
                        DateTime ndate = DateTime.Now.Date;
                        DateTime sdate = Convert.ToDateTime(k.Cells[8].Value);
                        int diff = (sdate - ndate).Days;
                        if (diff < 0)
                        {
                            k.DefaultCellStyle.BackColor = Color.Red;
                        }
                        else if (diff < 30)
                        {
                            k.DefaultCellStyle.BackColor = Color.Blue;
                        }
                    }
                }
            }
            cn.Close();

            foreach (DataGridViewRow r in dataGridView1.Rows)
            {
                int i1 = 0, i2 = 0;


                cn.Open();
                string ct = "select ReorderPoint from Product where ProductCode=@d1";
                cmd = new SqlCommand(ct, cn);
                cmd.Parameters.AddWithValue("@d1", r.Cells[0].Value.ToString());
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    i1 = Convert.ToInt32(rdr.GetValue(0));
                }
                cn.Close();


                cn.Open();
                string ct1 = "select sum(Qty) from Product,Temp_Stock where Product.PID=Temp_Stock.ProductID and ProductCode=@d1";
                cmd = new SqlCommand(ct1, cn);
                cmd.Parameters.AddWithValue("@d1", r.Cells[0].Value.ToString());
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    i2 = Convert.ToInt32(rdr.GetValue(0));
                }
                cn.Close();

                if (i2 < i1)
                {
                    r.DefaultCellStyle.BackColor = Color.Cyan;
                }
            }
            dataGridView1.ClearSelection();
            double total1 = 0;
            double total2 = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue; // Skip the new row placeholder

                var celv = row.Cells[3] as DataGridViewTextBoxCell;
                var celv1 = row.Cells[4] as DataGridViewTextBoxCell;
                var celv2 = row.Cells[9] as DataGridViewTextBoxCell;

                if (celv != null && celv1 != null && celv2 != null && celv.Value != null && celv1.Value != null && celv2.Value != null)
                {
                    if (double.TryParse(celv.Value.ToString(), out double value1) &&
                        double.TryParse(celv1.Value.ToString(), out double value2) &&
                        double.TryParse(celv2.Value.ToString(), out double quantity))
                    {
                        total1 += quantity * value1;
                        total2 += value2 * quantity;
                    }
                }
            }

            TextBox2.Text = total1.ToString();
            TextBox3.Text = total2.ToString();
            TextBox4.Text = (total2 - total1).ToString();
        }
    }
}
