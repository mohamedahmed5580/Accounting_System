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
    public partial class Barcode_printing : Form
    {
        SqlConnection cn = new SqlConnection(DataAccessLayer.Con());
        public Barcode_printing()
        {
            InitializeComponent();
            GetData();
            txtCategory.TextChanged += new EventHandler(txtCategory_TextChanged);
            txtProductName.TextChanged += new EventHandler(txtProductName_TextChanged);
        }

        private void Barcode_printing_Load(object sender, EventArgs e)
        {
           
        }

        private void GetData()
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT RTRIM(ProductCode), RTRIM(ProductName), RTRIM(Category), RTRIM(Temp_Stock.Barcode), Qty,BarcodeImage FROM Category, SubCategory, Product, Temp_Stock WHERE Category.CategoryName = SubCategory.Category AND Product.SubCategoryID = SubCategory.ID AND Temp_Stock.ProductID = Product.PID AND Qty > 0 ORDER BY ProductName", cn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    listView1.Items.Clear();
                    while (rdr.Read())
                    {
                        ListViewItem item = new ListViewItem
                        {
                            Text = rdr[0].ToString().Trim()
                        };
                        item.SubItems.Add(rdr[1].ToString().Trim());
                        item.SubItems.Add(rdr[2].ToString().Trim());
                        item.SubItems.Add(rdr[3].ToString().Trim());
                        item.SubItems.Add(rdr[4].ToString().Trim());
                        item.SubItems.Add(rdr[5].ToString().Trim());

                        listView1.Items.Add(item);
                    }
                }
            }
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                listView1.Items[i].Checked = true;
            }
            cn.Close();
        }
        private void Reset()
        {
            txtCategory.Text = "";
            txtProductName.Text = "";
            txtNoOfCopies.Text = "";
            GetData();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }
        private void txtCategory_TextChanged(object sender, EventArgs e)
        {
            cn.Open();
            string query = "SELECT RTRIM(ProductCode), RTRIM(ProductName), RTRIM(Category), RTRIM(Temp_Stock.Barcode), Qty " +
                           "FROM Category, SubCategory, Product, Temp_Stock " +
                           "WHERE Category.CategoryName = SubCategory.Category " +
                           "AND Product.SubCategoryID = SubCategory.ID " +
                           "AND Temp_Stock.ProductID = Product.PID " +
                           "AND Qty > 0 " +
                           "AND Category LIKE @Category " +
                           "ORDER BY ProductName";

            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue("@Category", "%" + txtCategory.Text.Trim() + "%");

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    listView1.Items.Clear();
                    while (rdr.Read())
                    {
                        ListViewItem item = new ListViewItem
                        {
                            Text = rdr[0].ToString().Trim()
                        };
                        item.SubItems.Add(rdr[1].ToString().Trim());
                        item.SubItems.Add(rdr[2].ToString().Trim());
                        item.SubItems.Add(rdr[3].ToString().Trim());
                        item.SubItems.Add(rdr[4].ToString().Trim());
                        listView1.Items.Add(item);
                    }
                }
            }
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                listView1.Items[i].Checked = true;
            }

            cn.Close();


        }
        private void txtProductName_TextChanged(object sender, EventArgs e)
        {
            cn.Open();
            string query = "SELECT RTRIM(ProductCode), RTRIM(ProductName), RTRIM(Category), RTRIM(Temp_Stock.Barcode), Qty " +
                           "FROM Category, SubCategory, Product, Temp_Stock " +
                           "WHERE Category.CategoryName = SubCategory.Category " +
                           "AND Product.SubCategoryID = SubCategory.ID " +
                           "AND Temp_Stock.ProductID = Product.PID " +
                           "AND Qty > 0 " +
                           "AND ProductName LIKE @ProductName " +
                           "ORDER BY ProductName";

            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue("@ProductName", "%" + txtProductName.Text.Trim() + "%");

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    listView1.Items.Clear();
                    while (rdr.Read())
                    {
                        ListViewItem item = new ListViewItem
                        {
                            Text = rdr[0].ToString().Trim()
                        };
                        item.SubItems.Add(rdr[1].ToString().Trim());
                        item.SubItems.Add(rdr[2].ToString().Trim());
                        item.SubItems.Add(rdr[3].ToString().Trim());
                        item.SubItems.Add(rdr[4].ToString().Trim());
                        listView1.Items.Add(item);
                    }
                }
            }

            for (int i = 0; i < listView1.Items.Count; i++)
            {
                listView1.Items[i].Checked = false;
            }
            cn.Close();
        }

        private void GroupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnGenerateBarcode_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count == 0)
            {
                MessageBox.Show("لا يوجد أصناف لعمل باركود ", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNoOfCopies.Text))
            {
                MessageBox.Show("الرجاء كتابة عدد النسخ المطلوبة", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNoOfCopies.Focus();
                return;
            }

            if (!int.TryParse(txtNoOfCopies.Text, out int noOfCopies) || noOfCopies <= 0)
            {
                MessageBox.Show("عدد النسخ المطلوبة يجب أن يكون أكبر من صفر", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNoOfCopies.Focus();
                return;
            }

            try
            {
                if (listView1.CheckedItems.Count > 0)
                {
                    string condition = "";
                    string condition1 = "";
                    foreach (ListViewItem item in listView1.CheckedItems)
                    {
                        condition += $"'{item.Text}',";
                        condition1 += $"'{item.SubItems[3].Text}',";
                    }

                    // Remove the last comma
                    condition = condition.TrimEnd(',');
                    condition1 = condition1.TrimEnd(',');

                    Cursor = Cursors.WaitCursor;
                    Timer1.Enabled = true;

                    // Initialize DataTable from the DataSet
                    DataSet ds = new BarcodeDataSet(); // Using the typed DataSet
                    DataTable dtable = ds.Tables["BarcodeTable"]; // Using the typed DataTable within the DataSet

                    // Fetch data from the database
                    using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                    {
                        con.Open();

                        string query = "SELECT Temp_Stock.Barcode, BarcodeImage, Product.Productname " +
                                       "FROM Category, SubCategory, Product, Temp_Stock " +
                                       "WHERE Category.CategoryName = SubCategory.Category " +
                                       "AND Product.SubCategoryID = SubCategory.ID " +
                                       "AND Temp_Stock.ProductID = Product.PID " +
                                       "AND ProductCode IN (" + condition + ") " +
                                       "AND Temp_Stock.Barcode IN (" + condition1 + ")";

                        for (int j = 1; j < noOfCopies; j++)
                        {
                            query += " UNION ALL " +
                                     "SELECT Temp_Stock.Barcode, BarcodeImage, Product.Productname " +
                                     "FROM Category, SubCategory, Product, Temp_Stock " +
                                     "WHERE Category.CategoryName = SubCategory.Category " +
                                     "AND Product.SubCategoryID = SubCategory.ID " +
                                     "AND Temp_Stock.ProductID = Product.PID " +
                                     "AND ProductCode IN (" + condition + ") " +
                                     "AND Temp_Stock.Barcode IN (" + condition1 + ")";
                        }

                        SqlCommand cmd = new SqlCommand(query, con);
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        adp.Fill(dtable); // Fill the DataTable with the data
                    }


                    var rpt = new rptBarcodeLabelPrinting_1();
                    rpt.SetDataSource(ds);

                    // Use a different name for the form instance to avoid confusion
                    frmReport reportForm = new frmReport();
                    reportForm.crystalReportViewer1.ReportSource = rpt;
                    reportForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                Timer1.Enabled = false;
            }

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
