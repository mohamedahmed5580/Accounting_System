using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;
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
    public partial class ProductsScreen : Form
    {
        SqlConnection cn = new SqlConnection(DataAccessLayer.Con());
        private Pymentinvoice frmPurchaseEntry;

        public ProductsScreen()
        {
            InitializeComponent();
        }

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        public void Getdata()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
                {
                    cn.Open();
                    string query = @"Select  PID, RTRIM(ProductCode), RTRIM(Productname), SubCategoryID, 
                                     RTRIM(CategoryName), RTRIM(SubCategoryName), RTRIM(Description), 
                                     CostPrice, SellingPrice, Discount, VAT, ReorderPoint, RTRIM(Barcode), 
                                     OpeningStock, ManufacturingDate, ExpiryDate, SellingPrice2,CurrencyName,Plimit
                                    from Category,SubCategory,Product where
                                    Category.CategoryName=SubCategory.Category and
                                    Product.SubCategoryID=SubCategory.ID 
                                     order by ProductCode";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], rdr[16], rdr[17], rdr[18]);
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


      
        private void frmLogs_Load(object sender, EventArgs e)
        {
            


        }




 
        public void Reset()
        {
            txtProductName.Text = "";
            txtCategory.Text = "";
            txtSubCategory.Text = "";
            txtBarcode.Text = "";
            Getdata();
        }
    


        private void txtSubCategory_TextChanged(object sender, EventArgs e)
        {
           
        }
       

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
          /*  ExportExcel(dgw);*/
        }

       

        private void Label6_Click(object sender, EventArgs e)
        {

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (dgw.SelectedRows.Count > 0 && lblSet.Text == "Product Entry")
                {

                    DataGridViewRow dr = dgw.SelectedRows[0];
                    Products frmProduct = new Products();

                    frmProduct.txtID.Text = dr.Cells[0].Value?.ToString() ?? "";
                    frmProduct.txtProductCode.Text = dr.Cells[1].Value?.ToString() ?? "";
                    frmProduct.txtProductName.Text = dr.Cells[2].Value?.ToString() ?? "";
                    frmProduct.txtSubCategoryID.Text = dr.Cells[3].Value?.ToString() ?? "";
                    frmProduct.cmbCategory.Text = dr.Cells[4].Value?.ToString() ?? "";
                    frmProduct.cmbSubCategory.Text = dr.Cells[5].Value?.ToString() ?? "";
                    frmProduct.txtFeatures.Text = dr.Cells[6].Value?.ToString() ?? "";
                    frmProduct.txtCostPrice.Text = dr.Cells[7].Value?.ToString() ?? "";
                    frmProduct.txtSellingPrice.Text = dr.Cells[8].Value?.ToString() ?? "";
                    frmProduct.txtDiscount.Text = dr.Cells[9].Value?.ToString() ?? "";
                    frmProduct.txtVAT.Text = dr.Cells[10].Value?.ToString() ?? "";
                    frmProduct.txtReorderPoint.Text = dr.Cells[11].Value?.ToString() ?? "";
                    frmProduct.txtBarcode.Text = dr.Cells[12].Value?.ToString() ?? "";
                    frmProduct.txtBCode.Text = dr.Cells[12].Value?.ToString() ?? "";
                    frmProduct.txtOpeningStock.Text = dr.Cells[13].Value?.ToString() ?? "";

                    bool expiryDateIsValid = DateTime.TryParse(dr.Cells[15].Value?.ToString(), out DateTime expiryDate);
                    bool manufacturingDateIsValid = DateTime.TryParse(dr.Cells[14].Value?.ToString(), out DateTime manufacturingDate);

                    if (expiryDateIsValid && manufacturingDateIsValid)
                    {
                        frmProduct.dtpExpiryDate.Value = expiryDate;
                        frmProduct.dtpManufacturingDate.Value = manufacturingDate;
                        frmProduct.dtpExpiryDate.Enabled = false;
                        frmProduct.dtpManufacturingDate.Enabled = false;

                    }

                    frmProduct.txtSellingPrice2.Text = dr.Cells[16].Value?.ToString() ?? "";

                    // Fetch and load product images
                    using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
                    {
                        cn.Open();
                        using (SqlCommand cmd = new SqlCommand("SELECT Photo FROM Product_Join WHERE ProductID = @d1", cn))
                        {
                            cmd.Parameters.AddWithValue("@d1", dr.Cells[0].Value.ToString());
                            using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                            {
                                frmProduct.dgw.Rows.Clear();
                                while (rdr.Read())
                                {
                                    if (!Convert.IsDBNull(rdr[0]))
                                    {
                                        byte[] data = (byte[])rdr[0];
                                        using (var ms = new MemoryStream(data))
                                        {
                                            Image img = Image.FromStream(ms);
                                            frmProduct.dgw.Rows.Add(img); // Assuming the DataGridView has an image column
                                        }
                                    }
                                }
                            }
                        }
                    }
                    frmProduct.Show();

                    frmProduct.btnUpdate.Enabled = true;
                    frmProduct.btnDelete.Enabled = true;
                    frmProduct.button1.Enabled = true; 
                    frmProduct.btnSave.Enabled = false;
                    frmProduct.txtOpeningStock.Enabled = true;
                    frmProduct.txtOpeningStock.ReadOnly = false;
                    this.Hide();
                   

                }
                else if (lblSet.Text == "payment")
                {
                    try
                    {
                        DataGridViewRow dr = dgw.SelectedRows[0];
                        Pymentinvoice.instance.txtProductID.Text = dr.Cells[0]?.Value?.ToString() ?? string.Empty;
                        Pymentinvoice.instance.txtProductCode.Text = dr.Cells[1]?.Value?.ToString() ?? string.Empty;
                        Pymentinvoice.instance.txtProductName.Text = dr.Cells[2]?.Value?.ToString() ?? string.Empty;
                        Pymentinvoice.instance.txtPricePerQty.Text = dr.Cells[7]?.Value?.ToString() ?? string.Empty;
                        Pymentinvoice.instance.txtBarcode.Text = dr.Cells[12]?.Value?.ToString() ?? string.Empty;
                        Pymentinvoice.instance.currencies.Text = dr.Cells[17]?.Value?.ToString() ?? string.Empty;
                        Pymentinvoice.instance.txtQty.Focus();
                        this.Hide();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ProductsScreen_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void ProductsScreen_Load(object sender, EventArgs e)
        {
            Getdata();
        }

        private void txtProductName_TextChanged_1(object sender, EventArgs e)
        {
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("Select PID, RTRIM(ProductCode),RTRIM(Productname), SubCategoryID,RTRIM(CategoryName),RTRIM(SubCategoryName), RTRIM(Description), CostPrice,SellingPrice, Discount, VAT, ReorderPoint,RTRIM(Barcode),OpeningStock,ManufacturingDate,ExpiryDate,SellingPrice2,CurrencyName,Plimit from Category,SubCategory,Product where Category.CategoryName=SubCategory.Category and Product.SubCategoryID=SubCategory.ID and Product.ProductName like '%" + txtProductName.Text + "%' order by ProductCode", cn);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dgw.Rows.Clear();
                while (rdr.Read() == true)
                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], rdr[16], rdr[17], rdr[18]);
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgw_RowPostPaint_1(object sender, DataGridViewRowPostPaintEventArgs e)
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

        private void txtBarcode_TextChanged_1(object sender, EventArgs e)
        {
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("Select PID, RTRIM(ProductCode),RTRIM(Productname), SubCategoryID,RTRIM(CategoryName),RTRIM(SubCategoryName), RTRIM(Description), CostPrice,SellingPrice, Discount, VAT, ReorderPoint,RTRIM(Barcode),OpeningStock,ManufacturingDate,ExpiryDate,SellingPrice2,CurrencyName,Plimit from Category,SubCategory,Product where Category.CategoryName=SubCategory.Category and Product.SubCategoryID=SubCategory.ID and Barcode like '%" + txtBarcode.Text + "%' order by ProductCode", cn);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dgw.Rows.Clear();
                while (rdr.Read() == true)
                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], rdr[16], rdr[16], rdr[17], rdr[18]);
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void txtCategory_TextChanged_1(object sender, EventArgs e)
        {
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("Select PID, RTRIM(ProductCode),RTRIM(Productname), SubCategoryID,RTRIM(CategoryName),RTRIM(SubCategoryName), RTRIM(Description), CostPrice,SellingPrice, Discount, VAT, ReorderPoint,RTRIM(Barcode),OpeningStock,ManufacturingDate,ExpiryDate,SellingPrice2,CurrencyName,Plimit from Category,SubCategory,Product where Category.CategoryName=SubCategory.Category and Product.SubCategoryID=SubCategory.ID and CategoryName like '%" + txtCategory.Text + "%' order by ProductCode", cn);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dgw.Rows.Clear();
                while (rdr.Read() == true)
                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], rdr[16], rdr[17], rdr[18]);
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSubCategory_TextChanged_1(object sender, EventArgs e)
        {
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("Select PID, RTRIM(ProductCode),RTRIM(Productname), SubCategoryID,RTRIM(CategoryName),RTRIM(SubCategoryName), RTRIM(Description), CostPrice,SellingPrice, Discount, VAT, ReorderPoint,RTRIM(Barcode),OpeningStock,ManufacturingDate,ExpiryDate,SellingPrice2,CurrencyName,Plimit from Category,SubCategory,Product where Category.CategoryName=SubCategory.Category and Product.SubCategoryID=SubCategory.ID and SubCategoryName like '%" + txtSubCategory.Text + "%' order by ProductCode", cn);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dgw.Rows.Clear();
                while (rdr.Read() == true)
                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], rdr[16], rdr[17], rdr[18]);
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgw_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void btnReset_Click_1(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnExportExcel_Click_1(object sender, EventArgs e)
        {
            ExportExcel(dgw);
        }
        public void ExportExcel(DataGridView dataGridView)
        {
            int rowsTotal, colsTotal;
            int I, j, iC;
            Cursor.Current = Cursors.WaitCursor;
            Excel.Application xlApp = new Excel.Application();

            try
            {
                Excel.Workbook excelBook = xlApp.Workbooks.Add();
                Excel.Worksheet excelWorksheet = (Excel.Worksheet)excelBook.Worksheets[1];
                xlApp.Visible = true;

                rowsTotal = dataGridView.RowCount;
                colsTotal = dataGridView.Columns.Count - 3;

                excelWorksheet.Cells.Select();
                excelWorksheet.Cells.Delete();

                for (iC = 0; iC <= colsTotal; iC++)
                {
                    excelWorksheet.Cells[1, iC + 1].Value = dataGridView.Columns[iC].HeaderText;
                }

                for (I = 0; I < rowsTotal; I++)
                {
                    for (j = 0; j <= colsTotal; j++)
                    {
                        excelWorksheet.Cells[I + 2, j + 1].Value = dataGridView.Rows[I].Cells[j].Value;
                    }
                }


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

        private void ProductsScreen_FormClosed_1(object sender, FormClosedEventArgs e)
        {
           
            
        }
    }
}
