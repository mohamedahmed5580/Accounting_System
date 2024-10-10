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
using Excel = Microsoft.Office.Interop.Excel;

namespace Accounting_System
{
    public partial class productRecord : Form
    {
        private Quotation frmQuotation;
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        public productRecord()
        {
            InitializeComponent();
            Getdata();
            dgw.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dgw_RowPostPaint);
            txtCategory.TextChanged += new EventHandler(txtCategory_TextChanged);
            txtSubCategory.TextChanged += new EventHandler(txtSubCategory_TextChanged);
            txtBarcode.TextChanged += new EventHandler(txtBarcode_TextChanged);
            txtProductName.TextChanged += new EventHandler(txtProductName_TextChanged);
            dgw.MouseDoubleClick += new MouseEventHandler(dgw_MouseDoubleClick);
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void Getdata()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT PID, RTRIM(ProductCode), RTRIM(Productname), SubCategoryID, RTRIM(CategoryName), RTRIM(SubCategoryName), RTRIM(Description), CostPrice, SellingPrice, Discount, VAT, ReorderPoint, RTRIM(Barcode), OpeningStock, ManufacturingDate, ExpiryDate, SellingPrice2 " +
                                   "FROM Category " +
                                   "INNER JOIN SubCategory ON Category.CategoryName = SubCategory.Category " +
                                   "INNER JOIN Product ON Product.SubCategoryID = SubCategory.ID " +
                                   "ORDER BY ProductCode";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        dgw.Rows.Clear();
                        while (rdr.Read())
                        {
                            dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], rdr[16]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgw_RowPostPaint(object sender , DataGridViewRowPostPaintEventArgs e)
        {

/*                string strRowNumber = (e.RowIndex + 1).ToString();
                using (Graphics g = e.Graphics)
                {
                    SizeF size = g.MeasureString(strRowNumber, this.Font);
                    if (this.dgw.RowHeadersWidth < Convert.ToInt32(size.Width + 20))
                    {
                        this.dgw.RowHeadersWidth = Convert.ToInt32(size.Width + 20);
                    }

                    Brush b = SystemBrushes.ControlText;
                    g.DrawString(strRowNumber, this.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2));
                }
            */

        }
        public void Reset()
        {
            txtProductName.Text = string.Empty;
            txtCategory.Text = string.Empty;
            txtSubCategory.Text = string.Empty;
            txtBarcode.Text = string.Empty;
            Getdata();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }
        private void txtCategory_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT PID, RTRIM(ProductCode), RTRIM(ProductName), SubCategoryID, RTRIM(CategoryName), RTRIM(SubCategoryName), RTRIM(Description), CostPrice, SellingPrice, Discount, VAT, ReorderPoint, RTRIM(Barcode), OpeningStock, ManufacturingDate, ExpiryDate, SellingPrice2 " +
                                   "FROM Category " +
                                   "JOIN SubCategory ON Category.CategoryName = SubCategory.Category " +
                                   "JOIN Product ON Product.SubCategoryID = SubCategory.ID " +
                                   "WHERE CategoryName LIKE @CategoryName " +
                                   "ORDER BY ProductCode";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CategoryName", "%" + txtCategory.Text + "%");
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr.GetValue(0), rdr.GetValue(1), rdr.GetValue(2), rdr.GetValue(3), rdr.GetValue(4), rdr.GetValue(5), rdr.GetValue(6), rdr.GetValue(7), rdr.GetValue(8), rdr.GetValue(9), rdr.GetValue(10), rdr.GetValue(11), rdr.GetValue(12), rdr.GetValue(13), rdr.GetValue(14), rdr.GetValue(15), rdr.GetValue(16));
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
        private void txtSubCategory_TextChanged(object sender, EventArgs e) {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT PID, RTRIM(ProductCode), RTRIM(ProductName), SubCategoryID, RTRIM(CategoryName), RTRIM(SubCategoryName), RTRIM(Description), CostPrice, SellingPrice, Discount, VAT, ReorderPoint, RTRIM(Barcode), OpeningStock, ManufacturingDate, ExpiryDate, SellingPrice2 " +
                                   "FROM Category " +
                                   "JOIN SubCategory ON Category.CategoryName = SubCategory.Category " +
                                   "JOIN Product ON Product.SubCategoryID = SubCategory.ID " +
                                   "WHERE SubCategoryName LIKE @SubCategoryName " +
                                   "ORDER BY ProductCode";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SubCategoryName", "%" + txtSubCategory.Text + "%");
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr.GetValue(0), rdr.GetValue(1), rdr.GetValue(2), rdr.GetValue(3), rdr.GetValue(4), rdr.GetValue(5), rdr.GetValue(6), rdr.GetValue(7), rdr.GetValue(8), rdr.GetValue(9), rdr.GetValue(10), rdr.GetValue(11), rdr.GetValue(12), rdr.GetValue(13), rdr.GetValue(14), rdr.GetValue(15), rdr.GetValue(16));
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
        private void txtBarcode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT PID, RTRIM(ProductCode), RTRIM(ProductName), SubCategoryID, RTRIM(CategoryName), RTRIM(SubCategoryName), RTRIM(Description), CostPrice, SellingPrice, Discount, VAT, ReorderPoint, RTRIM(Barcode), OpeningStock, ManufacturingDate, ExpiryDate, SellingPrice2 " +
                                   "FROM Category " +
                                   "JOIN SubCategory ON Category.CategoryName = SubCategory.Category " +
                                   "JOIN Product ON Product.SubCategoryID = SubCategory.ID " +
                                   "WHERE Barcode LIKE @Barcode " +
                                   "ORDER BY ProductCode";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Barcode", "%" + txtBarcode.Text + "%");
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr.GetValue(0), rdr.GetValue(1), rdr.GetValue(2), rdr.GetValue(3), rdr.GetValue(4), rdr.GetValue(5), rdr.GetValue(6), rdr.GetValue(7), rdr.GetValue(8), rdr.GetValue(9), rdr.GetValue(10), rdr.GetValue(11), rdr.GetValue(12), rdr.GetValue(13), rdr.GetValue(14), rdr.GetValue(15), rdr.GetValue(16));
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
        private void txtProductName_TextChanged(object sender, EventArgs e) {

            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = "SELECT PID, RTRIM(ProductCode), RTRIM(ProductName), SubCategoryID, RTRIM(CategoryName), RTRIM(SubCategoryName), RTRIM(Description), CostPrice, SellingPrice, Discount, VAT, ReorderPoint, RTRIM(Barcode), OpeningStock, ManufacturingDate, ExpiryDate, SellingPrice2 " +
                                   "FROM Category " +
                                   "JOIN SubCategory ON Category.CategoryName = SubCategory.Category " +
                                   "JOIN Product ON Product.SubCategoryID = SubCategory.ID " +
                                   "WHERE Product.ProductName LIKE @ProductName " +
                                   "ORDER BY ProductCode";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ProductName", "%" + txtProductName.Text + "%");
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr.GetValue(0), rdr.GetValue(1), rdr.GetValue(2), rdr.GetValue(3), rdr.GetValue(4), rdr.GetValue(5), rdr.GetValue(6), rdr.GetValue(7), rdr.GetValue(8), rdr.GetValue(9), rdr.GetValue(10), rdr.GetValue(11), rdr.GetValue(12), rdr.GetValue(13), rdr.GetValue(14), rdr.GetValue(15), rdr.GetValue(16));
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

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            ExportExcel(dgw);
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
        private void OpenQuotationForm()
        {
            if (frmQuotation == null || frmQuotation.IsDisposed)
            {
                frmQuotation = new Quotation();
            }
            frmQuotation.Show();
        }
        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (dgw.Rows.Count > 0)
                {
                    /*                    if (lblSet.Text == "Product Entry")
                                        {
                                            DataGridViewRow dr = dgw.SelectedRows[0];
                                            frmProduct.Show();
                                            this.Close();
                                            frmProduct.txtID.Text = dr.Cells[0].Value.ToString();
                                            frmProduct.txtProductCode.Text = dr.Cells[1].Value.ToString();
                                            frmProduct.txtProductName.Text = dr.Cells[2].Value.ToString();
                                            frmProduct.txtSubCategoryID.Text = dr.Cells[3].Value.ToString();
                                            frmProduct.cmbCategory.Text = dr.Cells[4].Value.ToString();
                                            frmProduct.cmbSubCategory.Text = dr.Cells[5].Value.ToString();
                                            frmProduct.txtFeatures.Text = dr.Cells[6].Value.ToString();
                                            frmProduct.txtCostPrice.Text = dr.Cells[7].Value.ToString();
                                            frmProduct.txtSellingPrice.Text = dr.Cells[8].Value.ToString();
                                            frmProduct.txtDiscount.Text = dr.Cells[9].Value.ToString();
                                            frmProduct.txtVAT.Text = dr.Cells[10].Value.ToString();
                                            frmProduct.txtReorderPoint.Text = dr.Cells[11].Value.ToString();
                                            frmProduct.txtBarcode.Text = dr.Cells[12].Value.ToString();
                                            frmProduct.txtBCode.Text = dr.Cells[12].Value.ToString();
                                            frmProduct.txtOpeningStock.Text = dr.Cells[13].Value.ToString();

                                            if (dr.Cells[15].Value == DBNull.Value)
                                            {
                                                frmProduct.dtpExpiryDate.Value = DateTime.Today;
                                                frmProduct.dtpManufacturingDate.Value = DateTime.Today;
                                                frmProduct.CheckBox1.Checked = false;
                                            }
                                            else
                                            {
                                                frmProduct.dtpExpiryDate.Value = Convert.ToDateTime(dr.Cells[15].Value);
                                                frmProduct.dtpManufacturingDate.Value = Convert.ToDateTime(dr.Cells[14].Value);
                                                frmProduct.CheckBox1.Checked = true;
                                            }
                                            frmProduct.txtSellingPrice2.Text = dr.Cells[16].Value.ToString();

                                            using (SqlConnection con = new SqlConnection(cs))
                                            {
                                                con.Open();
                                                string query = "SELECT Photo FROM Product JOIN Product_Join ON Product.PID = Product_Join.ProductID WHERE Product.PID = @d1";
                                                using (SqlCommand cmd = new SqlCommand(query, con))
                                                {
                                                    cmd.Parameters.AddWithValue("@d1", dr.Cells[0].Value.ToString());
                                                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                                                    {
                                                        frmProduct.dgw.Rows.Clear();
                                                        while (rdr.Read())
                                                        {
                                                            byte[] data = (byte[])rdr[0];
                                                            using (MemoryStream ms = new MemoryStream(data))
                                                            {
                                                                Image img = Image.FromStream(ms);
                                                                frmProduct.dgw.Rows.Add(img);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            frmProduct.btnUpdate.Enabled = true;
                                            frmProduct.btnDelete.Enabled = true;
                                            frmProduct.btnSave.Enabled = false;
                                            frmProduct.txtOpeningStock.Enabled = true;
                                            frmProduct.txtOpeningStock.ReadOnly = false;
                                            lblSet.Text = "";
                                        }*/

                    if (lblSet.Text == "Quotation")
                    {
                        DataGridViewRow dr = dgw.SelectedRows[0];

                        // Access the singleton instance
                        Quotation frmQuotation = Quotation.Instance;

                        // Update the form's controls
                        frmQuotation.txtProductID.Text = dr.Cells[0].Value.ToString();
                        frmQuotation.txtProductCode.Text = dr.Cells[1].Value.ToString();
                        frmQuotation.txtProductName.Text = dr.Cells[2].Value.ToString();
                        frmQuotation.txtSellingPrice.Text = dr.Cells[8].Value.ToString();
                        frmQuotation.txtDiscountPer.Text = dr.Cells[9].Value.ToString();
                        frmQuotation.txtVAT.Text = dr.Cells[10].Value.ToString();

                        // Show the form
                        frmQuotation.Show();

                        // Close the current form
                        this.Close();

                        lblSet.Text = "";
                    }


                    /*                    if (lblSet.Text == "Stock")
                                        {
                                            DataGridViewRow dr = dgw.SelectedRows[0];
                                            frmPurchaseEntry.Show();
                                            this.Close();
                                            frmPurchaseEntry.txtProductID.Text = dr.Cells[0].Value.ToString();
                                            frmPurchaseEntry.txtProductCode.Text = dr.Cells[1].Value.ToString();
                                            frmPurchaseEntry.txtProductName.Text = dr.Cells[2].Value.ToString();
                                            frmPurchaseEntry.txtPricePerQty.Text = dr.Cells[7].Value.ToString();
                                            frmPurchaseEntry.txtBarcode.Text = dr.Cells[12].Value.ToString();
                                            frmPurchaseEntry.txtQty.Focus();
                                            lblSet.Text = "";
                                        }*/
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

        private void productRecord_Load(object sender, EventArgs e)
        {

        }
    }
}
