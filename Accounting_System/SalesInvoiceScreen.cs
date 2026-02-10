using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;
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
using static Emgu.Util.Platform;

namespace Accounting_System
{
    public partial class SalesInvoiceScreen : Form
    {
        SqlConnection con = new SqlConnection(DataAccessLayer.Con());
        private static SalesInvoiceScreen _instance;
        // Add these private members to your form/class where dgw and Getdata reside
        private int currentPage = 1;
        private const int PageSize = 50; // How many records to load per scroll/batch
        private bool isLoading = false;
        private bool allDataLoaded = false;
        private int currentWarehouseId = -1; // Track the WID for which data is loaded
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
            // Only call comboBoxGenerate. It will trigger the SelectedIndexChanged
            // event, which will then load the data for the default warehouse.
            comboBoxGenerate();
            dgw.Scroll += dgw_Scroll;
            // DO NOT call Getdata(), fillInvoiceNo(), GetBalance() here.
            // They will be called by the comboBox2_SelectedIndexChanged event triggered above.

        }
        private void SalesInvoiceScreen_Load(object sender, EventArgs e)
        {
        }
        private void dgw_Scroll(object sender, ScrollEventArgs e)
        {
            // Check if the scroll is vertical and near the bottom
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                // Alternative calculation: checks if the last fully visible row's index is close to the end
                int displayedRows = dgw.DisplayedRowCount(true);
                int firstDisplayedRow = dgw.FirstDisplayedScrollingRowIndex;
                int lastVisibleRow = (firstDisplayedRow + displayedRows) - 1; // Index of the last row shown
                int buffer = 5; // How many rows away from the end triggers the load

                // Only trigger if not already loading, not all data loaded, and scrolled near the bottom
                if (!isLoading && !allDataLoaded && dgw.RowCount > 0 && lastVisibleRow >= dgw.RowCount - 1 - buffer)
                {
                    // Use Task.Run to ensure the await in LoadMoreDataAsync doesn't block
                    // the UI thread if there's any synchronous work within it, though ideally
                    // LoadMoreDataAsync is fully async I/O bound.
                    // Calling the async method directly might be okay if it's purely I/O.
                    LoadMoreDataAsync(); // Fire and forget - UI remains responsive

                    // If LoadMoreDataAsync were NOT async, you'd need:
                    // Task.Run(() => LoadMoreData()); // Run synchronously on a background thread
                }
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Ensure an item is actually selected
            if (comboBox2.SelectedItem == null)
            {
                WID.Text = ""; // Clear ID if selection is cleared or invalid
                dgw.Rows.Clear(); // Clear grid if no warehouse selected
                ClearBalanceTextBoxes(); // Clear balance totals
                LoadInitialData();                         // Optionally clear InvoiceNo ComboBox if fillInvoiceNo depends on warehouse
                                         // invoiceNoComboBox.Items.Clear();
                return;
            }

            try
            {
                // 1. Get the selected warehouse name from the ComboBox
                string selectedWarehouse = comboBox2.SelectedItem.ToString();

                // 2. Use the Repository to get the ID
                IWarehouseRepository repository = new WarehouseRepository();
                int? warehouseId = repository.GetWarehouseId(selectedWarehouse); // Uses DAL

                // 3. Update the WID TextBox
                if (warehouseId.HasValue)
                {
                    WID.Text = warehouseId.Value.ToString();

                    // 4. Load data *after* WID is set
                    //Getdata(); // Load grid data for the selected warehouse
                    LoadInitialData();                         // Optionally clear InvoiceNo ComboBox if fillInvoiceNo depends on warehouse

                    fillInvoiceNo(); // Fill related data (ensure this uses WID or grid data)
                    GetBalance(); // Calculate totals *after* grid is populated
                }
                else
                {
                    // Warehouse name selected, but ID not found (data inconsistency?)
                    WID.Text = ""; // Clear the ID
                    dgw.Rows.Clear(); // Clear the grid
                    ClearBalanceTextBoxes(); // Clear balance totals
                                             // Optionally clear InvoiceNo ComboBox
                                             // invoiceNoComboBox.Items.Clear();
                    MessageBox.Show($"لم يتم العثور على معرف للمخزن المحدد: '{selectedWarehouse}'.", "خطأ بيانات", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex) // Catch errors during repository access or UI update
            {
                WID.Text = ""; // Clear WID on error
                dgw.Rows.Clear(); // Clear the grid on error
                ClearBalanceTextBoxes(); // Clear balance totals
                MessageBox.Show($"حدث خطأ عند تحديث معرف المخزن أو تحميل البيانات: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- Data and UI Methods ---


        public void comboBoxGenerate()
        {
            try
            {
                IWarehouseRepository repository = new WarehouseRepository();
                List<string> warehouseNames = repository.GetWarehouseNames(); // Uses DAL

                comboBox2.Items.Clear();
                WID.Text = ""; // Clear the WID initially

                if (warehouseNames != null && warehouseNames.Count > 0)
                {
                    comboBox2.Items.AddRange(warehouseNames.ToArray());
                    // Set selected index AFTER adding items.
                    // This will trigger the SelectedIndexChanged event handler.
                    comboBox2.SelectedIndex = 0;
                }
                else
                {
                    // Handle case where no warehouses are found
                    ClearBalanceTextBoxes();
                    dgw.Rows.Clear();
                    MessageBox.Show("No warehouses found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading warehouse list: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // لمنع التكرار
        private HashSet<int> loadedInvoiceIds = new HashSet<int>();

        public void LoadInitialData()
        {
            dgw.Rows.Clear();
            loadedInvoiceIds.Clear(); // ⭐ مهم جداً
            ClearBalanceTextBoxes();

            currentPage = 1;
            isLoading = false;
            allDataLoaded = false;

            if (!int.TryParse(WID.Text, out currentWarehouseId))
            {
                currentWarehouseId = -1;
                return;
            }

            LoadMoreDataAsync();
        }


        // Async method to load a page of data
        private async Task LoadMoreDataAsync()
        {
            // Prevent concurrent loading or loading if all data is already fetched, or if WID is invalid
            if (isLoading || allDataLoaded || currentWarehouseId <= 0)
            {
                return;
            }

            isLoading = true;
            // Optional: Show some loading indicator in the UI

            try
            {
                string query = @"SELECT
                           Inv_ID, RTRIM(InvoiceNo) AS InvoiceNo, InvoiceDate, SM_ID,
                           RTRIM(Salesman_ID) AS SalesmanCode, RTRIM(Salesman.Name) AS SalesmanName,
                           Customer.ID AS CustomerID_PK, RTRIM(Customer.CustomerID) AS CustomerCode,
                           RTRIM(Customer.Name) AS CustomerName, RTRIM(Customer.ContactNo) AS ContactNo,
                           GrandTotal, TotalPaid, Balance, RTRIM(InvoiceInfo.Remarks) AS Remarks,
                           Warehouses.WID, RTRIM(Warehouses.WarehouseName) AS WarehouseName,
                           total_sale
                       FROM Customer
                       JOIN InvoiceInfo ON Customer.ID = InvoiceInfo.CustomerID
                       JOIN Salesman ON Salesman.SM_ID = InvoiceInfo.SalesmanID
                       JOIN Warehouses ON InvoiceInfo.WID = Warehouses.WID
                       WHERE InvoiceInfo.WID = @WID
                       ORDER BY Inv_ID DESC
                       OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY; -- Pagination!
                       ";

                int offset = (currentPage - 1) * PageSize;

                var paramWID = DataAccessLayer.CreateParameter("@WID", SqlDbType.Int, currentWarehouseId);
                var paramOffset = DataAccessLayer.CreateParameter("@Offset", SqlDbType.Int, offset);
                var paramPageSize = DataAccessLayer.CreateParameter("@PageSize", SqlDbType.Int, PageSize);

                int rowsAdded = 0;
                // Use the async version of ExecuteReader
                using (SqlDataReader reader = await DataAccessLayer.ExecuteReaderAsync(query, CommandType.Text, paramWID, paramOffset, paramPageSize))
                {
                    while (await reader.ReadAsync())
                    {
                        int invId = Convert.ToInt32(reader["Inv_ID"]);

                        // 🔒 منع التكرار
                        if (loadedInvoiceIds.Contains(invId))
                            continue;

                        loadedInvoiceIds.Add(invId);

                        System.Action addRow = () =>
                        {
                            dgw.Rows.Add(
                                reader[0], reader[1], reader[2], reader[3],
                                reader[4], reader[5], reader[6], reader[7],
                                reader[8], reader[9], reader[10], reader[11],
                                reader[12], reader[13], reader[14], reader[15],
                                0, reader[16]
                            );
                        };

                        if (dgw.InvokeRequired)
                            dgw.BeginInvoke(addRow);
                        else
                            addRow();

                        rowsAdded++;
                    }

                } // Reader is disposed, connection closed here

                // Check if we've loaded all data
                if (rowsAdded < PageSize)
                {
                    allDataLoaded = true;
                    // Optional: Display a message or change UI to indicate all data is loaded
                }
                else
                {
                    currentPage++; // Prepare for the next page load
                }

                // Call formatting after potentially adding rows
                // Consider if formatting needs Invoke/BeginInvoke if called from non-UI thread context
                System.Action formatAction = () => FormatGridRows();
                if (dgw.InvokeRequired)
                {
                    dgw.BeginInvoke(formatAction);
                }
                else
                {
                    formatAction();
                }

            }
            catch (SqlException dbEx)
            {
                MessageBox.Show("Database Error loading invoice data: " + dbEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                allDataLoaded = true; // Stop trying to load more on error
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred loading invoice data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                allDataLoaded = true; // Stop trying to load more on error
            }
            finally
            {
                isLoading = false;
                // Optional: Hide loading indicator
            }
        }
        public List<InvoiceData> Getdata(int warehouseId)
        {
            var invoiceList = new List<InvoiceData>(5000); // Pre-allocate capacity

            // The SQL query remains the same
            string query = @"SELECT
                   i.Inv_ID, RTRIM(i.InvoiceNo), i.InvoiceDate, s.SM_ID, RTRIM(s.Salesman_ID),
                   RTRIM(s.Name) AS SalesmanName, c.ID AS CustomerID_PK, RTRIM(c.CustomerID), RTRIM(c.Name) AS CustomerName,
                   RTRIM(c.ContactNo), i.GrandTotal, i.TotalPaid, i.Balance, RTRIM(i.Remarks),
                   w.WID, RTRIM(w.WarehouseName), i.total_sale
               FROM Customer c
               JOIN InvoiceInfo i ON c.ID = i.CustomerID
               JOIN Salesman s ON s.SM_ID = i.SalesmanID
               JOIN Warehouses w ON i.WID = w.WID
               WHERE i.WID = @WID
               ORDER BY i.Inv_ID DESC;";

            try
            {
                // 1. Create the parameter using the DAL helper
                var widParam = DataAccessLayer.CreateParameter("@WID", SqlDbType.Int, warehouseId);

                // 2. Execute the query using the DAL's ExecuteTable method
                System.Data.DataTable dt = DataAccessLayer.ExecuteTable(query, CommandType.Text, widParam);

                // 3. Process the results from the DataTable
                foreach (DataRow row in dt.Rows)
                {
                    // Create and populate the InvoiceData object
                    var invoice = new InvoiceData
                    {
                        // Access data by column index (ordinal) as in the original code
                        Inv_ID = Convert.ToInt32(row[0]),
                        InvoiceNo = row[1].ToString(),
                        InvoiceDate = Convert.ToDateTime(row[2]),
                        SM_ID = Convert.ToInt32(row[3]),
                        SalesmanCode = row[4].ToString(),
                        SalesmanName = row[5].ToString(),
                        CustomerID_PK = Convert.ToInt32(row[6]),
                        CustomerCode = row[7].ToString(),
                        CustomerName = row[8].ToString(),
                        // Handle potential NULL values from the DataRow
                        ContactNo = row[9] == DBNull.Value ? null : row[9].ToString(),
                        GrandTotal = Convert.ToDecimal(row[10]),
                        TotalPaid = Convert.ToDecimal(row[11]),
                        Balance = Convert.ToDecimal(row[12]),
                        Remarks = row[13] == DBNull.Value ? null : row[13].ToString(),
                        WID = Convert.ToInt32(row[14]),
                        WarehouseName = row[15].ToString(),
                        TotalSale = Convert.ToDecimal(row[16]) // Assuming decimal, adjust if needed
                    };
                    invoiceList.Add(invoice);
                }
            }
            catch (SqlException dbEx)
            {
                // Log the exception (ideally use a proper logging framework)
                Console.WriteLine($"Database Error loading invoice data: {dbEx}");
                MessageBox.Show("Database Error loading invoice data: " + dbEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Return empty list on database error
                return new List<InvoiceData>();
            }
            catch (Exception ex)
            {
                // Log the general exception
                Console.WriteLine($"An error occurred loading invoice data: {ex}");
                MessageBox.Show("An error occurred loading invoice data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Return empty list on general error
                return new List<InvoiceData>();
            }

            return invoiceList;
        }

        // Example DTO class
        public class InvoiceData
        {
            public int Inv_ID { get; set; }
            public string InvoiceNo { get; set; }
            public DateTime InvoiceDate { get; set; }
            public int SM_ID { get; set; }
            public string SalesmanCode { get; set; }
            public string SalesmanName { get; set; }
            public int CustomerID_PK { get; set; }
            public string CustomerCode { get; set; }
            public string CustomerName { get; set; }
            public string ContactNo { get; set; }
            public decimal GrandTotal { get; set; }
            public decimal TotalPaid { get; set; }
            public decimal Balance { get; set; }
            public string Remarks { get; set; }
            public int WID { get; set; }
            public string WarehouseName { get; set; }
            public decimal TotalSale { get; set; } // Adjust type if needed
        }

        // --- How to use it and populate the grid ---
        public void LoadDataIntoGrid()
        {
            // Clear grid before loading new data
            dgw.Rows.Clear();
            // Clear balance text boxes if needed
            // ClearBalanceTextBoxes();

            if (!int.TryParse(WID.Text, out int warehouseId))
            {
                // Handle invalid ID
                return;
            }

            // Fetch data efficiently into a list
            List<InvoiceData> data = Getdata(warehouseId);

            // --- Option 1: Manual Row Addition (Less Recommended for Large Datasets) ---
            // Can be slow for 5000 rows due to UI updates per row
            // dgw.SuspendLayout(); // Prevent redraws during add
            // foreach (var invoice in data)
            // {
            //     dgw.Rows.Add(
            //         invoice.Inv_ID, invoice.InvoiceNo, invoice.InvoiceDate, invoice.SM_ID, invoice.SalesmanCode,
            //         invoice.SalesmanName, invoice.CustomerID_PK, invoice.CustomerCode, invoice.CustomerName,
            //         invoice.ContactNo, invoice.GrandTotal, invoice.TotalPaid, invoice.Balance,
            //         invoice.Remarks, invoice.WID, invoice.WarehouseName,
            //         0, // The extra '0' column from your original code
            //         invoice.TotalSale
            //     );
            // }
            // dgw.ResumeLayout(); // Allow redraws

            // --- Option 2: Data Binding (Generally Preferred for Performance & Simplicity) ---
            // Ensure grid columns are set up (either in Designer or code) to match InvoiceData properties
            // Set AutoGenerateColumns = false if you defined columns manually in the designer
            dgw.DataSource = data;

            // Apply Formatting after data binding or population
            FormatGridRows();
        }
        private void FormatGridRows()
        {
            try
            {
                // Balance is at index 12 based on Getdata query
                int balanceColumnIndex = 12;
                // Find the column by index OR use design-time name if reliable
                DataGridViewColumn balanceCol = dgw.Columns[balanceColumnIndex]; // Or dgw.Columns["Column9"] if name is set

                if (balanceCol == null)
                {
                    Console.WriteLine("Balance column not found for formatting.");
                    return; // Cannot format if column doesn't exist
                }

                foreach (DataGridViewRow row in dgw.Rows)
                {
                    if (row.IsNewRow) continue; // Skip the new row placeholder

                    // Check if the cell exists and has a value
                    if (row.Cells[balanceCol.Index] != null && row.Cells[balanceCol.Index].Value != null)
                    {
                        if (decimal.TryParse(row.Cells[balanceCol.Index].Value.ToString(), out decimal balanceValue))
                        {
                            row.DefaultCellStyle.BackColor = (balanceValue == 0m) ? Color.LightGreen : Color.LightCoral;
                        }
                        else
                        {
                            // Non-numeric balance? Apply a different style or default.
                            row.DefaultCellStyle.BackColor = Color.Yellow; // Example: Highlight parsing issues
                        }
                    }
                    else
                    {
                        // Handle null balance cells if necessary
                        row.DefaultCellStyle.BackColor = dgw.DefaultCellStyle.BackColor; // Use grid default
                    }
                }
            }
            catch (Exception formatEx)
            {
                MessageBox.Show("An error occurred during row formatting: " + formatEx.Message, "Formatting Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void GetBalance()
        {
            double totalGrand = 0;
            double totalPaid = 0;
            double totalBalance = 0;

            // Indices from Getdata query: GrandTotal=10, TotalPaid=11, Balance=12
            int grandTotalColIndex = 10;
            int totalPaidColIndex = 11;
            int balanceColIndex = 12;

            try
            {
                foreach (DataGridViewRow row in dgw.Rows)
                {
                    if (row.IsNewRow) continue; // Skip the template row

                    // Use TryParse for safety
                    if (row.Cells[grandTotalColIndex]?.Value != null &&
                        double.TryParse(row.Cells[grandTotalColIndex].Value.ToString(), out double grandVal))
                    {
                        totalGrand += grandVal;
                    }

                    if (row.Cells[totalPaidColIndex]?.Value != null &&
                        double.TryParse(row.Cells[totalPaidColIndex].Value.ToString(), out double paidVal))
                    {
                        totalPaid += paidVal;
                    }

                    if (row.Cells[balanceColIndex]?.Value != null &&
                        double.TryParse(row.Cells[balanceColIndex].Value.ToString(), out double balanceVal))
                    {
                        totalBalance += balanceVal;
                    }
                }

                // Update TextBoxes (assuming TextBox1, TextBox2, TextBox3 correspond to these totals)
                TextBox1.Text = totalGrand.ToString("N2"); // Format as number with 2 decimals
                TextBox2.Text = totalPaid.ToString("N2");
                TextBox3.Text = totalBalance.ToString("N2");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating balances: " + ex.Message, "Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearBalanceTextBoxes(); // Clear on error
            }
        }

        private void ClearBalanceTextBoxes()
        {
            TextBox1.Text = "0.00";
            TextBox2.Text = "0.00";
            TextBox3.Text = "0.00";
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
            LoadInitialData();

        }

        public void fillInvoiceNo()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT RTRIM(InvoiceNo) FROM InvoiceInfo WHERE WID = @WID", con))
                    {
                        cmd.Parameters.AddWithValue("@WID", WID.Text);

                        using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            adp.Fill(ds);
                            System.Data.DataTable dtable = ds.Tables[0];
                            cmbInvoiceNo.Items.Clear();

                            foreach (System.Data.DataRow drow in dtable.Rows)
                            {
                                cmbInvoiceNo.Items.Add(drow[0].ToString());
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


        private void btnGetData_Click(object sender, EventArgs e)
        {

            try
            {
                con.Open();
                SqlCommand  cmd = new SqlCommand("Select Inv_ID, RTRIM(InvoiceNo), InvoiceDate,SM_ID, RTRIM(Salesman_ID),RTRIM(Salesman.Name),Customer.ID,RTRIM(Customer.CustomerID),RTRIM(Customer.Name),RTRIM(Customer.ContactNo), GrandTotal, TotalPaid, Balance, RTRIM(InvoiceInfo.Remarks), Warehouses.WID, RTRIM(Warehouses.WarehouseName),total_sale FROM Customer JOIN InvoiceInfo ON Customer.ID = InvoiceInfo.CustomerID JOIN Salesman ON Salesman.SM_ID = InvoiceInfo.SalesmanID JOIN Warehouses ON InvoiceInfo.WID = Warehouses.WID where Customer.ID=InvoiceInfo.CustomerID and Salesman.SM_ID=InvoiceInfo.SalesmanID and InvoiceDate between @d1 and @d2 order by InvoiceDate Desc", con);
                cmd.Parameters.Add("@d1", SqlDbType.DateTime, 30, "Date").Value = dtpDateFrom.Value.Date;
                cmd.Parameters.Add("@d2", SqlDbType.DateTime, 30, "Date").Value = dtpDateTo.Value.Date;
                SqlDataReader  rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dgw.Rows.Clear();
                while (rdr.Read() == true)
                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15],0, rdr[16]);
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

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
            GetBalance();

        }

        private void cmbInvoiceNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand  cmd = new SqlCommand("Select Inv_ID, RTRIM(InvoiceNo), InvoiceDate,SM_ID, RTRIM(Salesman_ID),RTRIM(Salesman.Name),Customer.ID,RTRIM(Customer.CustomerID),RTRIM(Customer.Name),RTRIM(Customer.ContactNo), GrandTotal, TotalPaid, Balance, RTRIM(InvoiceInfo.Remarks), Warehouses.WID, RTRIM(Warehouses.WarehouseName),total_sale FROM Customer JOIN InvoiceInfo ON Customer.ID = InvoiceInfo.CustomerID JOIN Salesman ON Salesman.SM_ID = InvoiceInfo.SalesmanID JOIN Warehouses ON InvoiceInfo.WID = Warehouses.WID where Customer.ID=InvoiceInfo.CustomerID and Salesman.SM_ID=InvoiceInfo.SalesmanID and InvoiceNo LIKE '" + cmbInvoiceNo.Text + "' order by InvoiceDate Desc", con);
                SqlDataReader  rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dgw.Rows.Clear();
                while (rdr.Read() == true)
                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], 0, rdr[16]);
                con.Close();
                GetBalance();

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

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand  cmd = new SqlCommand("Select Inv_ID, RTRIM(InvoiceNo), InvoiceDate,SM_ID, RTRIM(Salesman_ID),RTRIM(Salesman.Name),Customer.ID,RTRIM(Customer.CustomerID),RTRIM(Customer.Name),RTRIM(Customer.ContactNo), GrandTotal, TotalPaid, Balance, RTRIM(InvoiceInfo.Remarks), Warehouses.WID, RTRIM(Warehouses.WarehouseName),total_sale FROM Customer JOIN InvoiceInfo ON Customer.ID = InvoiceInfo.CustomerID JOIN Salesman ON Salesman.SM_ID = InvoiceInfo.SalesmanID JOIN Warehouses ON InvoiceInfo.WID = Warehouses.WID where Customer.ID=InvoiceInfo.CustomerID and Salesman.SM_ID=InvoiceInfo.SalesmanID and InvoiceDate between @d1 and @d2 and Balance > 0 order by InvoiceDate Desc", con);
                cmd.Parameters.Add("@d1", SqlDbType.DateTime, 30, "Date").Value = DateTimePicker2.Value.Date;
                cmd.Parameters.Add("@d2", SqlDbType.DateTime, 30, "Date").Value = DateTimePicker1.Value.Date;
                SqlDataReader  rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dgw.Rows.Clear();
                while (rdr.Read() == true)
                    dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], 0, rdr[16]);
                con.Close();
                GetBalance();

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

        private void txtCustomerName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // Ensure the connection string is valid before proceeding
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // Use parameterized query to prevent SQL injection
                    string query = @"
                SELECT TOP 25 
                    Inv_ID, RTRIM(InvoiceNo) AS InvoiceNo, InvoiceDate, SM_ID, 
                    RTRIM(Salesman_ID) AS SalesmanID, RTRIM(Salesman.Name) AS SalesmanName, Customer.ID AS CustomerID,
                    RTRIM(Customer.CustomerID) AS CustomerCode, RTRIM(Customer.Name) AS CustomerName, 
                    RTRIM(Customer.ContactNo) AS ContactNo, GrandTotal, TotalPaid, Balance, 
                    RTRIM(InvoiceInfo.Remarks) AS Remarks, Warehouses.WID, 
                    RTRIM(Warehouses.WarehouseName) AS WarehouseName, total_sale
                FROM Customer
                JOIN InvoiceInfo ON Customer.ID = InvoiceInfo.CustomerID 
                JOIN Salesman ON Salesman.SM_ID = InvoiceInfo.SalesmanID 
                JOIN Warehouses ON InvoiceInfo.WID = Warehouses.WID
                WHERE InvoiceInfo.WID = @WID AND Customer.Name LIKE @CustomerName
                ORDER BY Inv_ID DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@WID", WID.Text);
                        cmd.Parameters.AddWithValue("@CustomerName", "%" + txtCustomerName.Text + "%");

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            dgw.Rows.Clear();

                            while (rdr.Read())
                            {
                                dgw.Rows.Add(
                                    rdr["Inv_ID"],
                                    rdr["InvoiceNo"],
                                    rdr["InvoiceDate"],
                                    rdr["SM_ID"],
                                    rdr["SalesmanID"],
                                    rdr["SalesmanName"],
                                    rdr["CustomerID"],
                                    rdr["CustomerCode"],
                                    rdr["CustomerName"],
                                    rdr["ContactNo"],
                                    rdr["GrandTotal"],
                                    rdr["TotalPaid"],
                                    rdr["Balance"],
                                    rdr["Remarks"],
                                    rdr["WID"],
                                    rdr["WarehouseName"],
                                    0, // Placeholder for the last column
                                    rdr["total_sale"]
                                );
                            }
                        }
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

                    con.Close();
                }
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
                // Ensure the connection string is valid before proceeding
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // Use parameterized query to prevent SQL injection
                    string query = @"
                SELECT TOP 25 
                    Inv_ID, RTRIM(InvoiceNo) AS InvoiceNo, InvoiceDate, SM_ID, 
                    RTRIM(Salesman_ID) AS SalesmanID, RTRIM(Salesman.Name) AS SalesmanName, Customer.ID AS CustomerID,
                    RTRIM(Customer.CustomerID) AS CustomerCode, RTRIM(Customer.Name) AS CustomerName, 
                    RTRIM(Customer.ContactNo) AS ContactNo, GrandTotal, TotalPaid, Balance, 
                    RTRIM(InvoiceInfo.Remarks) AS Remarks, Warehouses.WID, 
                    RTRIM(Warehouses.WarehouseName) AS WarehouseName, total_sale
                FROM Customer
                JOIN InvoiceInfo ON Customer.ID = InvoiceInfo.CustomerID 
                JOIN Salesman ON Salesman.SM_ID = InvoiceInfo.SalesmanID 
                JOIN Warehouses ON InvoiceInfo.WID = Warehouses.WID
                WHERE InvoiceInfo.WID = @WID AND Salesman.Name LIKE @CustomerName
                ORDER BY Inv_ID DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@WID", WID.Text);
                        cmd.Parameters.AddWithValue("@CustomerName", "%" + txtSalesman.Text + "%");

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            dgw.Rows.Clear();

                            while (rdr.Read())
                            {
                                dgw.Rows.Add(
                                    rdr["Inv_ID"],
                                    rdr["InvoiceNo"],
                                    rdr["InvoiceDate"],
                                    rdr["SM_ID"],
                                    rdr["SalesmanID"],
                                    rdr["SalesmanName"],
                                    rdr["CustomerID"],
                                    rdr["CustomerCode"],
                                    rdr["CustomerName"],
                                    rdr["ContactNo"],
                                    rdr["GrandTotal"],
                                    rdr["TotalPaid"],
                                    rdr["Balance"],
                                    rdr["Remarks"],
                                    rdr["WID"],
                                    rdr["WarehouseName"],
                                    0, // Placeholder for the last column
                                    rdr["total_sale"]
                                );
                            }
                        }
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

                    con.Close();
                }
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
                    DataGridViewRow dr = dgw.SelectedRows[0];

                    if (lblSet.Text == "Sales Invoice")
                    {
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
                        POS.instance.txtPaymentDue.Text = dr.Cells[12].Value.ToString();
                        POS.instance.txtRemarks.Text = dr.Cells[13].Value.ToString();
                        POS.instance.WID.Text = dr.Cells[14].Value.ToString();
                        POS.instance.comboBox2.Text = dr.Cells[15].Value.ToString();
                        POS.instance.btnSave.Enabled = false;
                        POS.instance.Button2.Enabled = false;
                        POS.instance.Button3.Enabled = true;
                        POS.instance.total_sale_save.Enabled = false;
                        POS.instance.button8.Enabled = false;
                        POS.instance.button7.Enabled = false;
                        POS.instance.label26.Visible = true;
                        POS.instance.btnUpdate.Enabled = true;
                        POS.instance.btnPrint.Enabled = true;
                        POS.instance.btnDelete.Enabled = true;
                        POS.instance.lblSet.Text = "Not Allowed";
                        POS.instance.btnAdd.Enabled = true;
                        POS.instance.txtContactNo.ReadOnly = true;
                        POS.instance.button4.Enabled = true;
                        POS.instance.dataGridView3.Rows.Clear();
                        POS.instance.textBox3.Visible = true;
                        POS.instance.label18.Visible = true;
/*                        POS.instance.total_sale.Visible = false;
                        POS.instance.label26.Visible = false;*/
                        using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                        {
                            con.Open();

                            // SQL Query to fetch product details
                            string sql = @"
            SELECT DISTINCT 
                RTRIM(Product.ProductCode) AS ProductCode, 
                RTRIM(Product.ProductName) AS ProductName, 
                RTRIM(Invoice_Product.Barcode) AS Barcode, 
                Invoice_Product.CostPrice, 
                Invoice_Product.SellingPrice, 
                Invoice_Product.Margin, 
                Invoice_Product.Qty, 
                Invoice_Product.Amount, 
                Invoice_Product.DiscountPer, 
                Invoice_Product.Discount, 
                Invoice_Product.VATPer, 
                Invoice_Product.VAT, 
                Invoice_Product.TotalAmount, 
                Product.PID, 
                Warehouses.WID 
            FROM 
                InvoiceInfo 
            INNER JOIN 
                Invoice_Product ON InvoiceInfo.Inv_ID = Invoice_Product.InvoiceID 
            INNER JOIN 
                Product ON Product.PID = Invoice_Product.ProductID 
            INNER JOIN 
                Temp_Stock ON Temp_Stock.ProductID = Product.PID 
            INNER JOIN 
                Warehouses ON InvoiceInfo.WID = Warehouses.WID 
            WHERE 
                InvoiceInfo.Inv_ID = @d1 AND InvoiceInfo.WID = @WID";

                            using (SqlCommand cmd = new SqlCommand(sql, con))
                            {
                                cmd.Parameters.AddWithValue("@d1", dr.Cells[0].Value);
                                int parsedWID;
                                if (int.TryParse(dr.Cells[14].Value.ToString(), out parsedWID))
                                {
                                    cmd.Parameters.AddWithValue("@WID", parsedWID);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@WID", DBNull.Value);
                                }

                                using (SqlDataReader rdr = cmd.ExecuteReader())
                                {
                                    POS.instance.DataGridView1.Rows.Clear();
                                    POS.instance.dataGridView3.Rows.Clear();

                                    while (rdr.Read())
                                    {
                                        POS.instance.DataGridView1.Rows.Add(
                                            rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14]
                                        );
                                        POS.instance.dataGridView3.Rows.Add(
                                            rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14]
                                        );
                                    }
                                }
                            }

                            // Fetching and setting data for DataGridView2
                            string sql1 = "SELECT RTRIM(PaymentMode), Invoice_Payment.TotalPaid, PaymentDate FROM InvoiceInfo INNER JOIN Invoice_Payment ON InvoiceInfo.Inv_ID = Invoice_Payment.InvoiceID WHERE InvoiceInfo.Inv_ID = @d1";
                            using (SqlCommand cmd = new SqlCommand(sql1, con))
                            {
                                cmd.Parameters.AddWithValue("@d1", dr.Cells[0].Value);
                                using (SqlDataReader rdr = cmd.ExecuteReader())
                                {
                                    POS.instance.DataGridView2.Rows.Clear();
                                    while (rdr.Read())
                                    {
                                        POS.instance.DataGridView2.Rows.Add(rdr[0], rdr[1], rdr[2]);
                                    }
                                }
                            }

                            // Fetching and setting the customer type
                            string ct = "SELECT RTRIM(CustomerType) FROM Customer WHERE ID = @customerId";
                            using (SqlCommand cmd = new SqlCommand(ct, con))
                            {
                                cmd.Parameters.AddWithValue("@customerId", dr.Cells[3].Value);
                                using (SqlDataReader rdr = cmd.ExecuteReader())
                                {
                                    if (rdr.Read())
                                    {
                                        POS.instance.txtCustomerType.Text = rdr[0].ToString();
                                    }
                                }
                            }

                            // First query to get TC_ID
                            string query1 = "SELECT TC_ID FROM InvoiceInfo WHERE Inv_ID = @d1";
                            using (SqlCommand cmd1 = new SqlCommand(query1, con))
                            {
                                cmd1.Parameters.AddWithValue("@d1", dr.Cells[0].Value);
                                using (SqlDataReader reader1 = cmd1.ExecuteReader())
                                {
                                    if (reader1.Read())
                                    {
                                        POS.instance.txtT_ID_1.Text = reader1["TC_ID"].ToString();
                                    }
                                }
                            }

                            // Second query to get TransactionID
                            string query2 = "SELECT TransactionID FROM Payment_2 WHERE TC_ID = @tc_id";
                            using (SqlCommand cmd2 = new SqlCommand(query2, con))
                            {
                                cmd2.Parameters.AddWithValue("@tc_id", POS.instance.txtT_ID_1.Text);
                                using (SqlDataReader reader2 = cmd2.ExecuteReader())
                                {
                                    if (reader2.Read())
                                    {
                                        POS.instance.txtTransactionNo_1.Text = reader2["TransactionID"].ToString();
                                    }
                                }
                            }
                        }
                        POS.instance.total_sale.Text = dr.Cells[17].Value.ToString();
                        POS.instance.txtTotalPayment.Text = dr.Cells[11].Value.ToString();

                        this.Close();
                    }
                    else if (lblSet.Text == "SR")
                    {
                        SalesReturn frmSalesReturn = SalesReturn.instance;

                        frmSalesReturn.txtSalesID.Text = dr.Cells[0].Value.ToString();
                        frmSalesReturn.txtSalesInvoiceNo.Text = dr.Cells[1].Value.ToString();
                        frmSalesReturn.dtpSalesDate.Text = dr.Cells[2].Value.ToString();
                        frmSalesReturn.txtCustomerID.Text = dr.Cells[7].Value.ToString();
                        frmSalesReturn.txtcust_ID.Text = dr.Cells[6].Value.ToString();
                        frmSalesReturn.txtCustomerName.Text = dr.Cells[8].Value.ToString();
                        frmSalesReturn.textBox1.Text = dr.Cells[10].Value.ToString();
                        frmSalesReturn.txtTotalPayment.Text = dr.Cells[11].Value.ToString();
                        string def = (Convert.ToDouble(dr.Cells[10].Value) - Convert.ToDouble(dr.Cells[11].Value)).ToString();
                        frmSalesReturn.txtPaymentDue.Text= def;
                        frmSalesReturn.txtPaymentDueP.Text = def;

                        frmSalesReturn.totalsale.Text = dr.Cells[17].Value.ToString();
                        frmSalesReturn.btnDelete.Enabled = true;
                        frmSalesReturn.auto();

                        using (var con = new SqlConnection(DataAccessLayer.Con()))
                        {
                            con.Open();
                            string sql1 = "SELECT RTRIM(PaymentMode) FROM InvoiceInfo INNER JOIN Invoice_Payment ON InvoiceInfo.Inv_ID = Invoice_Payment.InvoiceID WHERE InvoiceInfo.Inv_ID = @d1";
                            using (SqlCommand cmd1 = new SqlCommand(sql1, con))
                            {
                                cmd1.Parameters.AddWithValue("@d1", dr.Cells[0].Value);

                                try
                                {
                                    using (SqlDataReader rdr1 = cmd1.ExecuteReader())
                                    {
                                        while (rdr1.Read())
                                        {
                                            if (!rdr1.IsDBNull(0))
                                            {
                                                SalesReturn.instance.PaymentMethod.Text = rdr1[0].ToString();

                                            }
                                            else
                                            {
                                                SalesReturn.instance.PaymentMethod.Text = string.Empty;
                                            }
                                        }
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        // Fetching and setting data for DataGridView2 in SalesReturn form
                        con.Open();


                        string sql = @"SELECT 
                                        RTRIM(Product.ProductCode) AS ProductCode, 
                                        RTRIM(Product.ProductName) AS ProductName, 
                                        RTRIM(Invoice_Product.Barcode) AS Barcode, 
                                        Invoice_Product.SellingPrice, 
                                        Invoice_Product.Qty, 
                                        Invoice_Product.Amount, 
                                        Invoice_Product.DiscountPer, 
                                        Invoice_Product.Discount, 
                                        Invoice_Product.VATPer, 
                                        Invoice_Product.VAT, 
                                        Invoice_Product.TotalAmount, 
                                        Product.PID, 
                                        Invoice_Product.CostPrice, 
                                        Invoice_Product.Margin,
                                        Warehouses.WID,
                                        Warehouses.WarehouseName

                                    FROM 
                                        InvoiceInfo
                                    INNER JOIN 
                                        Invoice_Product ON InvoiceInfo.Inv_ID = Invoice_Product.InvoiceID
                                    INNER JOIN 
                                        Product ON Product.PID = Invoice_Product.ProductID
                                    INNER JOIN 
                                        Warehouses ON InvoiceInfo.WID = Warehouses.WID
                                    WHERE 
                                        InvoiceInfo.Inv_ID = @d1
                                    ";
                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.Parameters.AddWithValue("@d1", dr.Cells[0].Value);
                        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        frmSalesReturn.DataGridView2.Rows.Clear();
                        while (rdr.Read())
                        {
                            frmSalesReturn.DataGridView2.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15]);
                        }
                        con.Close();
                        this.Close();

                    }
                    else if (lblSet.Text == "1")
                    {
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
                // Ensure the connection string is valid before proceeding
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    // Use parameterized query to prevent SQL injection
                    string query = @"
                SELECT TOP 25 
                    Inv_ID, RTRIM(InvoiceNo) AS InvoiceNo, InvoiceDate, SM_ID, 
                    RTRIM(Salesman_ID) AS SalesmanID, RTRIM(Salesman.Name) AS SalesmanName, Customer.ID AS CustomerID,
                    RTRIM(Customer.CustomerID) AS CustomerCode, RTRIM(Customer.Name) AS CustomerName, 
                    RTRIM(Customer.ContactNo) AS ContactNo, GrandTotal, TotalPaid, Balance, 
                    RTRIM(InvoiceInfo.Remarks) AS Remarks, Warehouses.WID, 
                    RTRIM(Warehouses.WarehouseName) AS WarehouseName, total_sale
                FROM Customer
                JOIN InvoiceInfo ON Customer.ID = InvoiceInfo.CustomerID 
                JOIN Salesman ON Salesman.SM_ID = InvoiceInfo.SalesmanID 
                JOIN Warehouses ON InvoiceInfo.WID = Warehouses.WID
                WHERE InvoiceInfo.WID = @WID AND InvoiceInfo.Remarks LIKE @CustomerName
                ORDER BY Inv_ID DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@WID", WID.Text);
                        cmd.Parameters.AddWithValue("@CustomerName", "%" + TextBox4.Text + "%");

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            dgw.Rows.Clear();

                            while (rdr.Read())
                            {
                                dgw.Rows.Add(
                                    rdr["Inv_ID"],
                                    rdr["InvoiceNo"],
                                    rdr["InvoiceDate"],
                                    rdr["SM_ID"],
                                    rdr["SalesmanID"],
                                    rdr["SalesmanName"],
                                    rdr["CustomerID"],
                                    rdr["CustomerCode"],
                                    rdr["CustomerName"],
                                    rdr["ContactNo"],
                                    rdr["GrandTotal"],
                                    rdr["TotalPaid"],
                                    rdr["Balance"],
                                    rdr["Remarks"],
                                    rdr["WID"],
                                    rdr["WarehouseName"],
                                    0, // Placeholder for the last column
                                    rdr["total_sale"]
                                );
                            }
                        }
                    }

                    con.Close();
                }
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


   
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void TextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void GroupBox4_Enter(object sender, EventArgs e)
        {

        }
    }
}

