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
    public partial class Stock : Form
    {
        SqlConnection cn = new SqlConnection(DataAccessLayer.Con());
        public Stock()
        {
            InitializeComponent();
            txtProductName.TextChanged += new EventHandler(txtProductName_TextChanged);
            txtBarcode.TextChanged += new EventHandler(txtBarcode_TextChanged);
            dgw.MouseClick += new MouseEventHandler(dgw_MouseDoubleClick);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            
            txtProductName.Text = "";
            txtBarcode.Text = "";
            Getdata();
        }
        private void Getdata()
        {
            SqlCommand cmd;
            SqlDataReader rdr;

            cn.Open();
            cmd = new SqlCommand("SELECT PID, RTRIM(Product.ProductCode), RTRIM(ProductName), RTRIM(Temp_Stock.Barcode), CostPrice, SellingPrice, Discount, VAT, Qty, RTRIM(Product.SellingPrice2),BarcodeImage,Plimit,CurrencyName " +
                                 "FROM Temp_Stock, Product " +
                                 "WHERE Product.PID = Temp_Stock.ProductID AND Qty > 0 " +
                                 "ORDER BY ProductCode", cn);
            rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            dgw.Rows.Clear();

            while (rdr.Read())
            {
                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12]);
            }

            cn.Close();
        
    }
        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Stock_Load(object sender, EventArgs e)
        {
            Getdata();  
/*            dgw.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgw.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgw.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgw.Columns[7].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgw.Columns[8].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;*/
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

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            ExportExcel(dgw);
        }
        private void txtProductName_TextChanged(object sender, EventArgs e)
        {
            cn.Open();

            string query = "SELECT PID, RTRIM(Product.ProductCode), RTRIM(ProductName), RTRIM(Temp_Stock.Barcode), CostPrice, SellingPrice, Discount, VAT, Qty, RTRIM(Product.SellingPrice2),BarcodeImage,Plimit " +
                           "FROM Temp_Stock, Product " +
                           "WHERE Product.PID = Temp_Stock.ProductID AND Qty > 0 AND ProductName LIKE @ProductName " +
                           "ORDER BY ProductCode";

            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue("@ProductName", "%" + txtProductName.Text + "%");

                using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    dgw.Rows.Clear();

                    while (rdr.Read())
                    {
                        dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11]);
                    }
                }
            }
        }
        private void txtBarcode_TextChanged(object sender, EventArgs e)
        {
            cn.Open();

            string query = "SELECT PID, RTRIM(Product.ProductCode), RTRIM(ProductName), RTRIM(Temp_Stock.Barcode), CostPrice, SellingPrice, Discount, VAT, Qty, RTRIM(Product.SellingPrice2),BarcodeImage,Plimit " +
                            "FROM Temp_Stock, Product " +
                           "WHERE Product.PID = Temp_Stock.ProductID AND Qty > 0 AND Temp_Stock.Barcode LIKE @Barcode " +
                           "ORDER BY ProductCode";

            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                // Add parameter to prevent SQL injection
                cmd.Parameters.AddWithValue("@Barcode", "%" + txtBarcode.Text + "%");

                using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    dgw.Rows.Clear();

                    while (rdr.Read())
                    {
                        dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11]);
                    }
                }
            }
        }
        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (dgw.Rows.Count > 0 && dgw.SelectedRows.Count > 0)
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];
                    if (lblSet.Text == "Billing")
                    {

                        POS.instance.txtProductID.Text = dr.Cells[0].Value.ToString();
                        POS.instance.txtProductCode.Text = dr.Cells[1].Value.ToString();
                        POS.instance.txtProductName.Text = dr.Cells[2].Value.ToString();
                        POS.instance.txtBarcode.Text = dr.Cells[3].Value.ToString();
                        POS.instance.txtCostPrice.Text = dr.Cells[4].Value.ToString();
                        if (POS.instance.ComboBox1.SelectedIndex == 0)
                        {
                            POS.instance.txtSellingPrice.Text = dr.Cells[5].Value.ToString();
                        }
                        else
                        {
                            POS.instance.txtSellingPrice.Text = dr.Cells[9].Value.ToString();
                        }

                        POS.instance.txtAmount.Text = dr.Cells[5].Value.ToString();
                        POS.instance.txtQty.Focus();
                        double num;
                        num = Convert.ToDouble(dr.Cells[5].Value) - Convert.ToDouble(dr.Cells[4].Value);
                        num = Math.Round(num, 2);
                        POS.instance.txtMargin.Text = num.ToString();
                        POS.instance.Plimit.Text = dr.Cells[11].Value.ToString();

                        POS.instance.txtVAT.Text = dr.Cells[7].Value.ToString();
                        POS.instance.txtDiscountPer.Text = dr.Cells[6].Value.ToString();
                        lblSet.Text = "";
                        POS.instance.dgw.Visible = false;
                        POS.instance.txtQty.Focus();

                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("No row selected or no rows available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
