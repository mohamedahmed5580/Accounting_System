using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using static Accounting_System.SalesInvoiceScreen;
using Excel = Microsoft.Office.Interop.Excel;

namespace Accounting_System
{
    public partial class ProductsScreen : Form
    {
        private Pymentinvoice frmPurchaseEntry;
        private int currentPage = 1;
        private const int PageSize = 50; // How many records to load per scroll/batch
        private bool isLoading = false;
        private bool allDataLoaded = false;
        private int currentWarehouseId = -1; // Track the WID for which data is loaded

        public ProductsScreen()
        {
            InitializeComponent();
            // Load initial ComboBox data. This happens *before* LoadInitialData
            PopulateWarehouseComboBox(); // Renamed from comboBoxGenerate
            // Set default warehouse selection if items exist
            if (comboBox2.Items.Count > 0)
            {
                comboBox2.SelectedIndex = 0; // This will trigger SelectedIndexChanged -> LoadInitialData
            }
            else
            {
                WID.Text = ""; // Ensure WID is clear if no warehouses
                currentWarehouseId = -1;
                dgw.Rows.Clear(); // Ensure grid is clear
            }
        }
        private void ProductsScreen_Load(object sender, EventArgs e)
        {
        }

        public void PopulateWarehouseComboBox()
        {
            try
            {
                string query = "SELECT WarehouseName FROM [dbo].[Warehouses] ORDER BY WarehouseName";
                System.Data.DataTable dt = DataAccessLayer.ExecuteTable(query, CommandType.Text);

                comboBox2.Items.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    comboBox2.Items.Add(row["WarehouseName"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading warehouses: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        public void LoadInitialData()
        {
            // Clear grid and reset state variables for pagination
            dgw.Rows.Clear();
            currentPage = 1;
            isLoading = false;
            allDataLoaded = false;

            // Validate and store the current WID for loading
            if (string.IsNullOrWhiteSpace(WID.Text) || !int.TryParse(WID.Text, out currentWarehouseId))
            {
                currentWarehouseId = -1; // Mark as invalid WID
                                         // Optionally clear grid or show message if needed (grid is already cleared)
                                         // MessageBox.Show("Please select a valid Warehouse ID.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; // Don't load anything if WID is invalid
            }

            // Load the first page asynchronously based on the current filter (comboBox1)
            _ = LoadMoreDataAsync(); // Fire and forget the Task
        }


        private void dgw_Scroll(object sender, ScrollEventArgs e)
        {
            // Check if scrolling vertically near the bottom
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll && !isLoading && !allDataLoaded && currentWarehouseId > 0)
            {
                DataGridView dgv = dgw; // Use the class member directly

                // Calculate if near the bottom
                int displayedRows = dgv.DisplayedRowCount(false); // Use false for potentially partial rows
                int firstDisplayedRow = dgv.FirstDisplayedScrollingRowIndex;

                // Defensive check for valid scroll indices
                if (firstDisplayedRow < 0) return; // Should not happen in normal scrolling

                int buffer = 10; // Load when within 'buffer' rows of the end

                // Check if the bottom of the visible area is near the end of loaded data
                if (firstDisplayedRow + displayedRows >= dgv.RowCount - buffer)
                {
                    // Load more data asynchronously
                    _ = LoadMoreDataAsync(); // Fire and forget
                }
            }
        }


        // Async method to load the next page of data based on current filters
        private async Task LoadMoreDataAsync()
        {
            // Prevent re-entry or loading if all data is fetched or WID is invalid
            if (isLoading || allDataLoaded || currentWarehouseId <= 0)
            {
                return;
            }

            isLoading = true;
            // Optional: Show a loading indicator

            try
            {
                // Base part of the query
                string queryBase = @"
                SELECT
                    p.PID,
                    RTRIM(p.ProductCode) AS ProductCode,
                    RTRIM(p.ProductName) AS ProductName,
                    p.SubCategoryID,
                    RTRIM(c.CategoryName) AS CategoryName,
                    RTRIM(sc.SubCategoryName) AS SubCategoryName,
                    RTRIM(p.Description) AS Description,
                    p.CostPrice,
                    p.SellingPrice,
                    p.Discount,
                    p.VAT,
                    p.ReorderPoint,
                    p.Barcode,
                    p.OpeningStock,
                    p.ManufacturingDate,
                    p.ExpiryDate,
                    p.SellingPrice2,
                    p.Plimit,
                    w.WarehouseName,
                    w.WID,
                    ts.Qty -- Include Qty from Temp_Stock for filtering
                FROM
                    Category c
                INNER JOIN
                    SubCategory sc ON c.CategoryName = sc.Category
                INNER JOIN
                    Product p ON p.SubCategoryID = sc.ID
                INNER JOIN
                    Temp_Stock ts ON ts.ProductID = p.PID
                INNER JOIN
                    Warehouses w ON ts.WID = w.WID
            ";

                // WHERE clause based on filters
                string whereClause = "WHERE w.WID = @WID "; // Always filter by warehouse
                List<SqlParameter> parameters = new List<SqlParameter> {
                 DataAccessLayer.CreateParameter("@WID", SqlDbType.Int, currentWarehouseId)
             };

                // Add filter based on comboBox1 selection
                if (comboBox1.SelectedIndex == 1) // Zero Quantity
                {
                    whereClause += "AND ts.Qty = 0 ";
                }
                else if (comboBox1.SelectedIndex ==2) // Low Stock (1-5)
                {
                    whereClause += "AND ts.Qty BETWEEN 1 AND 5 "; // Use BETWEEN for range
                }
                else if (comboBox1.SelectedIndex == 3) // Products not in Temp_Stock (handled separately below)
                {
                    // Handle index 3 differently - it doesn't use Temp_Stock Qty filter
                }
                else // Default (Index 0 or other) - All quantities in the selected warehouse
                {
                    // No additional quantity filter needed beyond the join itself
                }

                // Handle Index 3: Products NOT in Temp_Stock (needs LEFT JOIN)
                if (comboBox1.SelectedIndex == 3)
                {
                    queryBase = @"
                    SELECT
                        p.PID,
                        RTRIM(p.ProductCode) AS ProductCode,
                        RTRIM(p.ProductName) AS ProductName,
                        p.SubCategoryID,
                        RTRIM(c.CategoryName) AS CategoryName,
                        RTRIM(sc.SubCategoryName) AS SubCategoryName,
                        RTRIM(p.Description) AS Description,
                        p.CostPrice,
                        p.SellingPrice,
                        p.Discount,
                        p.VAT,
                        p.ReorderPoint,
                        p.Barcode,
                        p.OpeningStock,
                        p.ManufacturingDate,
                        p.ExpiryDate,
                        p.SellingPrice2,
                        p.Plimit,
                        NULL AS WarehouseName, -- No warehouse context here
                        NULL AS WID,           -- No warehouse context here
                        NULL AS Qty            -- No Qty context here
                    FROM
                        Category c
                    INNER JOIN
                        SubCategory sc ON c.CategoryName = sc.Category
                    INNER JOIN
                        Product p ON p.SubCategoryID = sc.ID
                    LEFT JOIN
                        Temp_Stock ts ON ts.ProductID = p.PID AND ts.WID = @WID -- Check specific warehouse only
                    WHERE
                        ts.ProductID IS NULL -- Product does not exist in Temp_Stock for this WID
                ";
                    // Parameters list already contains @WID for the JOIN condition
                    whereClause = ""; // WHERE clause is built into the queryBase now
                }


                // Add Pagination
                string orderClause = "ORDER BY p.ProductCode ";
                string paginationClause = "OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                int offset = (currentPage - 1) * PageSize;
                parameters.Add(DataAccessLayer.CreateParameter("@Offset", SqlDbType.Int, offset));
                parameters.Add(DataAccessLayer.CreateParameter("@PageSize", SqlDbType.Int, PageSize));

                // Combine query parts
                string finalQuery = queryBase + whereClause + orderClause + paginationClause;


                // Execute asynchronously using DataAccessLayer
                System.Data.DataTable dt = await DataAccessLayer.ExecuteTableAsync(finalQuery, CommandType.Text, parameters.ToArray());

                int rowsAdded = dt.Rows.Count;

                if (rowsAdded > 0)
                {
                    // Update UI on the UI thread safely
                    System.Action addRowsAction = () => {
                        dgw.SuspendLayout();
                        foreach (DataRow row in dt.Rows)
                        {
                            // Add data directly to the grid
                            // Adjust indices based on the final SELECT list (21 columns now incl Qty)
                            dgw.Rows.Add(
                                row[0], row[1], row[2], row[3], row[4], row[5], row[6],
                                row[7], row[8], row[9], row[10], row[11], row[12],
                                row[13], row[14], row[15], row[16], row[17],
                                row[18], // WarehouseName
                                row[19]  // WID
                                         // row[20] is Qty - not added to grid visually? Adjust Add() if needed
                            );
                        }
                        dgw.ResumeLayout(true);
                    };

                    if (dgw.InvokeRequired)
                    {
                        dgw.BeginInvoke(addRowsAction);
                    }
                    else
                    {
                        addRowsAction();
                    }
                }

                // Check if this was the last page
                if (rowsAdded < PageSize)
                {
                    allDataLoaded = true;
                }
                else
                {
                    currentPage++; // Increment page number for the next load
                }
            }
            catch (SqlException dbEx)
            {
                MessageBox.Show("Database Error loading product data: " + dbEx.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                allDataLoaded = true; // Stop trying to load more on error
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred loading product data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                allDataLoaded = true; // Stop trying to load more on error
            }
            finally
            {
                isLoading = false;
                // Optional: Hide loading indicator
            }
        }


      
   
       

        public void Getdata()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
                {
                    cn.Open();
                    string query = @"
                                    SELECT
                                        p.PID, 
                                        RTRIM(p.ProductCode) AS ProductCode,
                                        RTRIM(p.ProductName) AS ProductName,
                                        p.SubCategoryID,
                                        RTRIM(c.CategoryName) AS CategoryName,
                                        RTRIM(sc.SubCategoryName) AS SubCategoryName,
                                        RTRIM(p.Description) AS Description,
                                        p.CostPrice,
                                        p.SellingPrice,
                                        p.Discount,
                                        p.VAT,
                                        p.ReorderPoint,
                                        p.Barcode,
                                        p.OpeningStock,
                                        p.ManufacturingDate,
                                        p.ExpiryDate,
                                        p.SellingPrice2,
                                        p.Plimit,
                                        w.WarehouseName,
                                        w.WID
                                    FROM 
                                        Category c
                                    INNER JOIN 
                                        SubCategory sc ON c.CategoryName = sc.Category
                                    INNER JOIN 
                                        Product p ON p.SubCategoryID = sc.ID
                                    INNER JOIN 
                                        Temp_Stock ts ON ts.ProductID = p.PID
                                    INNER JOIN 
                                        Warehouses w ON ts.WID = w.WID
                                    WHERE 
                                        w.WID = @WID
                                    ORDER BY 
                                        p.ProductCode;";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@WID", WID.Text.Trim()); // Assuming WIDD is a TextBox

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], rdr[16], rdr[17], rdr[18], rdr[19] );
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
            dgw.Rows.Clear();
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
                    Products products = Products.instance();
                    this.Hide();
                    products.txtID.Text = dr.Cells[0].Value?.ToString() ?? "";
                    products.txtProductCode.Text = dr.Cells[1].Value?.ToString() ?? "";
                    products.txtProductName.Text = dr.Cells[2].Value?.ToString() ?? "";
                    products.txtSubCategoryID.Text = dr.Cells[3].Value?.ToString() ?? "";
                    products.cmbCategory.Text = dr.Cells[4].Value?.ToString() ?? "";
                    products.cmbSubCategory.Text = dr.Cells[5].Value?.ToString() ?? "";
                    products.txtFeatures.Text = dr.Cells[6].Value?.ToString() ?? "";
                    products.txtCostPrice.Text = dr.Cells[7].Value?.ToString() ?? "";
                    products.txtSellingPrice.Text = dr.Cells[8].Value?.ToString() ?? "";
                    products.txtDiscount.Text = dr.Cells[9].Value?.ToString() ?? "";
                    products.txtVAT.Text = dr.Cells[10].Value?.ToString() ?? "";
                    products.txtReorderPoint.Text = dr.Cells[11].Value?.ToString() ?? "";
                    products.txtBarcode.Text = dr.Cells[12].Value?.ToString() ?? "";
                    products.txtBCode.Text = dr.Cells[12].Value?.ToString() ?? "";
                    products.txtOpeningStock.Text = dr.Cells[13].Value?.ToString() ?? "";
                    products.Plimit.Text = dr.Cells[17].Value?.ToString() ?? "";

                    bool expiryDateIsValid = DateTime.TryParse(dr.Cells[15].Value?.ToString(), out DateTime expiryDate);
                    bool manufacturingDateIsValid = DateTime.TryParse(dr.Cells[14].Value?.ToString(), out DateTime manufacturingDate);

                    if (expiryDateIsValid && manufacturingDateIsValid)
                    {
                        products.dtpExpiryDate.Value = expiryDate;
                        products.dtpManufacturingDate.Value = manufacturingDate;
                        products.dtpExpiryDate.Enabled = false;
                        products.dtpManufacturingDate.Enabled = false;

                    }

                    products.txtSellingPrice2.Text = dr.Cells[16].Value?.ToString() ?? "";

                    // Validate barcode and generate it
                    string barcodeValue = dr.Cells[12]?.Value?.ToString()?.Trim() ?? string.Empty;

                    if (string.IsNullOrEmpty(barcodeValue) || !long.TryParse(barcodeValue, out _))
                    {
                        MessageBox.Show("Invalid barcode format. Please check the product barcode.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        // Select appropriate barcode format
                        BarcodeFormat barcodeFormat = (barcodeValue.Length == 12 || barcodeValue.Length == 13)
                                                      ? BarcodeFormat.UPC_A
                                                      : BarcodeFormat.CODE_128;

                        BarcodeWriter barcodeWriter = new BarcodeWriter
                        {
                            Format = barcodeFormat
                        };

                        products.picBarcode.Image = barcodeWriter.Write(barcodeValue);
                    }


                    products.btnUpdate.Enabled = true;
                    products.btnDelete.Enabled = true;
                    products.button1.Enabled = true;
                    products.btnSave.Enabled = false;
                    products.txtOpeningStock.Enabled = true;
                    products.txtOpeningStock.ReadOnly = false;


                }
                else if (lblSet.Text == "payment")
                {
                    try
                    {
                        if (dgw.SelectedRows.Count == 0)
                        {
                            MessageBox.Show("Please select a product.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        DataGridViewRow dr = dgw.SelectedRows[0];

                        // Assign values to Pymentinvoice instance
                        Pymentinvoice.instance.txtProductID.Text = dr.Cells[0]?.Value?.ToString() ?? string.Empty;
                        Pymentinvoice.instance.txtProductCode.Text = dr.Cells[1]?.Value?.ToString() ?? string.Empty;
                        Pymentinvoice.instance.txtProductName.Text = dr.Cells[2]?.Value?.ToString() ?? string.Empty;
                        Pymentinvoice.instance.txtPricePerQty.Text = dr.Cells[7]?.Value?.ToString() ?? string.Empty;
                        Pymentinvoice.instance.txtBarcode.Text = dr.Cells[12]?.Value?.ToString() ?? string.Empty;
                        Pymentinvoice.instance.WID.Text = dr.Cells[19]?.Value?.ToString() ?? string.Empty;
                        Pymentinvoice.instance.WNeame.Text = dr.Cells[18]?.Value?.ToString() ?? string.Empty;
                        Pymentinvoice.instance.dgw.Visible = false;

                        // Set focus on txtQty
                        Pymentinvoice.instance.txtQty.Focus();

                        // Validate barcode and generate it
                        string barcodeValue = dr.Cells[12]?.Value?.ToString()?.Trim() ?? string.Empty;

                        if (string.IsNullOrEmpty(barcodeValue) || !long.TryParse(barcodeValue, out _))
                        {
                            MessageBox.Show("Invalid barcode format. Please check the product barcode.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            // Select appropriate barcode format
                            BarcodeFormat barcodeFormat = (barcodeValue.Length == 12 || barcodeValue.Length == 13)
                                                          ? BarcodeFormat.UPC_A
                                                          : BarcodeFormat.CODE_128;

                            BarcodeWriter barcodeWriter = new BarcodeWriter
                            {
                                Format = barcodeFormat
                            };

                            Pymentinvoice.instance.pictureBox1.Image = barcodeWriter.Write(barcodeValue);
                        }

                        // Show comboBox2 if warehouse ID is empty
                        Pymentinvoice.instance.comboBox2.Visible = string.IsNullOrEmpty(Pymentinvoice.instance.WID.Text);
                        Pymentinvoice.instance.btnAdd.Enabled = true;

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
        private void txtProductName_TextChanged_1(object sender, EventArgs e)
        {
            GetFilteredData("ProductName", txtProductName.Text);
        }


        private void txtBarcode_TextChanged_1(object sender, EventArgs e)
        {
            GetFilteredData("p.Barcode", txtBarcode.Text);
        }

        private void txtCategory_TextChanged_1(object sender, EventArgs e)
        {
            GetFilteredData("CategoryName", txtCategory.Text);
        }


        private void txtSubCategory_TextChanged_1(object sender, EventArgs e)
        {
            GetFilteredData("SubCategoryName", txtSubCategory.Text);
        }

        private void GetFilteredData(string columnName, string filterText)
        {
            try
            {
                // Validate columnName to prevent SQL injection
                if (columnName != "ProductName" && columnName != "p.Barcode" &&
                    columnName != "CategoryName" && columnName != "SubCategoryName")
                {
                    throw new ArgumentException("Invalid column name");
                }

                using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
                {
                    cn.Open();
                    string query = $@"
                SELECT
                    p.PID, 
                    RTRIM(p.ProductCode) AS ProductCode,
                    RTRIM(p.ProductName) AS ProductName,
                    p.SubCategoryID,
                    RTRIM(c.CategoryName) AS CategoryName,
                    RTRIM(sc.SubCategoryName) AS SubCategoryName,
                    RTRIM(p.Description) AS Description,
                    p.CostPrice,
                    p.SellingPrice,
                    p.Discount,
                    p.VAT,
                    p.ReorderPoint,
                    p.Barcode,  -- Specify p.Barcode to resolve ambiguity
                    p.OpeningStock,
                    p.ManufacturingDate,
                    p.ExpiryDate,
                    p.SellingPrice2,
                    p.Plimit
                FROM 
                    Category c
                INNER JOIN 
                    SubCategory sc ON c.CategoryName = sc.Category
                INNER JOIN 
                    Product p ON p.SubCategoryID = sc.ID
                INNER JOIN 
                    Temp_Stock ts ON ts.ProductID = p.PID
                INNER JOIN 
                    Warehouses w ON ts.WID = w.WID
                WHERE 
                    {columnName} LIKE @FilterText
                ORDER BY 
                    p.ProductCode;";

                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@FilterText", "%" + filterText.Trim() + "%");

                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], rdr[16], rdr[17]);
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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBox2.SelectedItem == null)
            {
                WID.Text = "";
                currentWarehouseId = -1;
                dgw.Rows.Clear(); // Clear grid if no warehouse selected
                return;
            }

            string selectedWarehouse = comboBox2.SelectedItem.ToString();
            try
            {
                string query = "SELECT WID FROM Warehouses WHERE WarehouseName = @Name";
                var param = DataAccessLayer.CreateParameter("@Name", SqlDbType.NVarChar, selectedWarehouse);
                object result = DataAccessLayer.ExecuteScalar(query, CommandType.Text, param);

                if (result != null && result != DBNull.Value && int.TryParse(result.ToString(), out int wid))
                {
                    WID.Text = wid.ToString();
                    // Trigger initial data load for the NEWLY selected warehouse
                    LoadInitialData();
                }
                else
                {
                    WID.Text = "";
                    currentWarehouseId = -1;
                    dgw.Rows.Clear();
                    MessageBox.Show("Could not find the selected warehouse ID.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                WID.Text = "";
                currentWarehouseId = -1;
                dgw.Rows.Clear();
                MessageBox.Show($"Error getting warehouse ID: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public void comboBoxGenerate()
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT WarehouseName FROM [dbo].[Warehouses]", con);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    comboBox2.Items.Clear();
                    while (rdr.Read()) // Loops through all available rows
                    {
                        comboBox2.Items.Add(rdr["WarehouseName"].ToString());
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            LoadInitialData();
        }

        private void dgw_CellContentClick_2(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}
