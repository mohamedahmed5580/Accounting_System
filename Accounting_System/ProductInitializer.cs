
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Accounting_System.Products;

namespace Accounting_System
{
    internal class ProductInitializer
    {
        private readonly Products _form;

        public ProductInitializer(Products form)
        {
            _form = form;
        }

        public void ResetControls()
        {
            // Disable date controls
            _form.dtpExpiryDate.Enabled = false;
            _form.dtpManufacturingDate.Enabled = false;

            // Set dates to today's date
            _form.dtpExpiryDate.Value = DateTime.Today;
            _form.dtpManufacturingDate.Value = DateTime.Today;

            // Clear TextBoxes
            _form.txtSellingPrice2.Clear();
            _form.txtBarcode.Clear();
            _form.TextBox1.Clear();
            _form.txtCostPrice.Clear();
            _form.txtProductCode.Clear();
            _form.txtDiscount.Text = "0";
            _form.txtSellingPrice.Clear();
            _form.txtVAT.Text = "0";
            _form.txtOpeningStock.Clear();
            _form.txtReorderPoint.Clear();
            _form.txtFeatures.Clear();
            _form.txtProductName.Clear();
            _form.Plimit.Clear();

            // Configure stock input controls
            _form.txtOpeningStock.ReadOnly = false;
            _form.txtOpeningStock.Enabled = true;
            _form.cmbSubCategory.Enabled = false;

            // Enable/disable buttons appropriately
            _form.btnSave.Enabled = true;
            _form.btnUpdate.Enabled = false;
            _form.button1.Enabled = false;
            _form.btnDelete.Enabled = false;
            _form.btnRemove.Enabled = false;
            _form.btnExportExcel.Enabled = false;

            // Reset PictureBox Image
            _form.Picture.Image = Properties.Resources._12;

            // Clear DataGridView rows
            _form.dgw.Rows.Clear();

            // Populate ComboBoxes
            _form.fillCategory();      // Suppose this method fills cmbCategory.
            if (_form.cmbCategory.Items.Count > 0)
                _form.cmbCategory.SelectedIndex = 0;
            else
                _form.cmbCategory.SelectedIndex = -1;

            _form.cmbSubCategory.SelectedIndex = _form.cmbSubCategory.Items.Count > 0 ? 0 : -1;
            _form.comboBox2.SelectedIndex = _form.comboBox2.Items.Count > 0 ? 0 : -1;
            _form.comboBoxGenerate();  // Fills comboBox2 from repository logic.
            _form.Getdata();         // Presumed to refresh a grid or additional controls.

            // Set focus to Barcode field and finalize date control settings
            _form.txtBarcode.Focus();
            _form.dtpExpiryDate.Enabled = false;
            _form.dtpManufacturingDate.Enabled = false;

            // Call any additional auto-generation logic.
            _form.auto();
            _form.comboBox2.SelectedIndex = 0;
            // Optionally GenerateBarcode() could be called as needed.
        }

    }

    public class ProductDto
    {
        public int ID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int SubCategoryID { get; set; }
        public string Description { get; set; }
        public double CostPrice { get; set; }
        public double SellingPrice { get; set; }
        public double Discount { get; set; }
        public double VAT { get; set; }
        public int ReorderPoint { get; set; }
        public int OpeningStock { get; set; }
        public string Barcode { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public string SellingPrice2 { get; set; }
        public int Plimit { get; set; }
        public int WarehouseID { get; set; }
        public Image BarcodeImage { get; set; }
        public List<Image> Photos { get; set; }
    }

    public interface IWarehouseRepository
    {
        List<string> GetWarehouseNames();
        int? GetWarehouseId(string warehouseName); // Added method signature (nullable int)
    }

    public class WarehouseRepository : IWarehouseRepository
    {
        /// <summary>
        /// Gets a list of all warehouse names from the database.
        /// </summary>
        /// <returns>A List of warehouse names.</returns>
        public List<string> GetWarehouseNames()
        {
            List<string> warehouseNames = new List<string>();
            string query = "SELECT WarehouseName FROM [dbo].[Warehouses] ORDER BY WarehouseName"; // Added Order By

            try
            {
                DataTable dt = DataAccessLayer.ExecuteTable(query, CommandType.Text);
                foreach (DataRow row in dt.Rows)
                {
                    // Ensure the value is not null before adding
                    if (row["WarehouseName"] != DBNull.Value && row["WarehouseName"] != null)
                    {
                        warehouseNames.Add(row["WarehouseName"].ToString());
                    }
                }
            }
            catch (SqlException dbEx)
            {
                // Log the exception details as needed.
                Console.WriteLine($"Database Error fetching warehouse names: {dbEx.Message}");
                // Depending on requirements, you might want to throw or return empty list.
            }
            catch (Exception ex)
            {
                // Log the exception details as needed.
                Console.WriteLine($"General Error fetching warehouse names: {ex.Message}");
            }
            return warehouseNames;
        }

        /// <summary>
        /// Gets the WID (Warehouse ID) for a given warehouse name.
        /// </summary>
        /// <param name="warehouseName">The name of the warehouse to find.</param>
        /// <returns>The integer WID if found; otherwise, null.</returns>
        public int? GetWarehouseId(string warehouseName)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(warehouseName))
            {
                return null; // Cannot find ID for empty name
            }

            string query = "SELECT WID FROM [dbo].[Warehouses] WHERE WarehouseName = @Name";
            int? warehouseId = null; // Default to null (not found)

            try
            {
                // Create parameter using the DAL helper
                var parameter = DataAccessLayer.CreateParameter("@Name", SqlDbType.NVarChar, warehouseName);

                // Use ExecuteScalar as we expect only one value (the ID)
                object result = DataAccessLayer.ExecuteScalar(query, CommandType.Text, parameter);

                // Check if a result was returned and it's not DBNull
                if (result != null && result != DBNull.Value)
                {
                    // Try to convert the result to an integer
                    if (int.TryParse(result.ToString(), out int id))
                    {
                        warehouseId = id;
                    }
                    else
                    {
                        // Log error: The WID column in the DB might not be an integer?
                        Console.WriteLine($"Error converting Warehouse ID to int for name '{warehouseName}'. Value was: {result}");
                    }
                }
                // If result is null or DBNull, warehouseId remains null (not found)
            }
            catch (SqlException dbEx)
            {
                // Log the database exception details
                Console.WriteLine($"Database Error fetching Warehouse ID for '{warehouseName}': {dbEx.Message}");
                // warehouseId remains null
            }
            catch (Exception ex)
            {
                // Log any other general exceptions
                Console.WriteLine($"General Error fetching Warehouse ID for '{warehouseName}': {ex.Message}");
                // warehouseId remains null
            }

            return warehouseId;
        }
    }
}
