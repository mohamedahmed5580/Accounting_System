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
using System.Xml.Linq;

namespace Accounting_System
{
    public partial class Warehouses_transportation : Form
    {
        public static Warehouses_transportation instance;
        public Warehouses_transportation()
        {
            InitializeComponent();
            instance = this;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {

        }

      

        private void btnDelete_Click(object sender, EventArgs e)
        {

        }

        private void txtTransactionNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtRemarks_TextChanged(object sender, EventArgs e)
        {

        }

        private void autherWarehouse_TextChanged(object sender, EventArgs e)
        {

        }

        private void WName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtFWN_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtProtuct_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtQty_TextChanged(object sender, EventArgs e)
        {

        }




        private void txtTID_TextChanged(object sender, EventArgs e)
        {

        }
        private void btnSelection_Click(object sender, EventArgs e)
        {
            Stock productsStock = new Stock();
            productsStock.lblSet.Text = "WTransport";
            productsStock.Show();
        }

        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (Bitmap bitmap = new Bitmap(image))
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                }
                return ms.ToArray();
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            string connectionString = DataAccessLayer.Con();

            if (string.IsNullOrEmpty(PID.Text))
            {
                // Call the delete method
                MessageBox.Show("الرجاء كتابة رقم الصنف.");
                return;
            }
            if (string.IsNullOrEmpty(txtQty.Text))
            {
                // Call the delete method
                MessageBox.Show("الرجاء كتابة كمية الصنف");
                return;
            }
            if (string.IsNullOrEmpty(FtxtWIDTxt.Text))
            {
                // Call the delete method
                MessageBox.Show("Please Write a warehouse ID");
                return;
            }
            if (string.IsNullOrEmpty(Barcode.Text))
            {
                // Call the delete method
                MessageBox.Show("Please Write a Barcode.");
                return;
            }

            string query1 = @" تم نقل بضاعه من المخزن " + txtFWN.Text + " الي المخزن ";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("TransferQuantityBetweenWarehouses", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ProductID", PID.Text);
                    cmd.Parameters.AddWithValue("@Qty", txtQty.Text);
                    cmd.Parameters.AddWithValue("@SourceWID", FtxtWIDTxt.Text);
                    cmd.Parameters.AddWithValue("@DestinationWID", TtxtWIDTxt.Text);
                    cmd.Parameters.AddWithValue("@Barcode", Barcode.Text);

                    if (pictureBox1.Image != null)
                    {
                        byte[] imageData = ImageToByteArray(pictureBox1.Image);
                        cmd.Parameters.AddWithValue("@BarcodeImage", imageData);

                        try
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("تم نقل الكمية بنجاح بين المخازن!", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (SqlException ex)
                        {
                            MessageBox.Show("خطأ: " + ex.Message, "فشل في النقل", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image found in pictureBox1.");
                    }
                }
            }
           /* using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO dbo.StoredProcedures (ProcedureName, ProcedureDefinition) VALUES (@ProcedureName, @ProcedureDefinition)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    // Define parameters for the query
                    cmd.Parameters.AddWithValue("@ProcedureName", txtTransactionNo.Text);
                    cmd.Parameters.AddWithValue("@ProcedureDefinition", query1);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Failed to Save Procedure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

*/        
            Reset();

        }


        public void TransferProductQuantity(int productId, float qty, int sourceWarehouseId, int destinationWarehouseId)
        {
            // استبدل بسلسلة الاتصال المناسبة لقاعدة البيانات الخاصة بك

        }
        private void btnNew_Click_1(object sender, EventArgs e)
        {
            Reset();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Warehouses warehouses = new Warehouses();
            warehouses.lbSet.Text = "WTransport";

            warehouses.Show();
        }

        private void Warehouses_transportation_Load(object sender, EventArgs e)
        {
            auto();
        }
        private void auto()
        {
            try
            {
                txtTID.Text = GenerateID();
                txtTransactionNo.Text = "TW-" + GenerateID();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private string GenerateID()
        {
            string newID = "00001"; // Default starting ID if no records exist

            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 Id FROM StoredProcedures ORDER BY Id DESC", con))
                    {
                        var result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            int latestID = int.Parse(result.ToString());
                            newID = (latestID + 1).ToString("D5"); // Format to 5 digits with leading zeros
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log exception (if necessary)
                    newID = "00001"; // Fallback ID if any error occurs
                }
            }

            return newID;
        }
        public void Reset()
        {
            auto();
            PID.Text = "";
            FtxtWIDTxt.Text = "";
            TtxtWIDTxt.Text = "";
            txtProtuct.Text = "";
            txtQty.Text = "";
            autherWarehouse.Text = "";
            WName.Text = "";
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            pictureBox1.Image = null;
            Barcode.Text = "";
            txtFWN.Text = "";
            Qtyin.Text = "";
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {

        }
    }
}
