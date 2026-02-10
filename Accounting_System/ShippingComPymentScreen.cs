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
    public partial class ShippingComPymentScreen : Form
    {
        string connectionString = DataAccessLayer.Con();

        public ShippingComPymentScreen()
        {
            InitializeComponent();
        }

        private void ShippingComPymentScreen_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT sp.SHPID,sp.SHPCode AS SHPCode,sc.ShappingName,s.InvoiceNo,sp.ST_ID,sc.ShappingCode,sp.TotalPrice,sp.PymentMethod,sp.Comments FROM Shipping_Pyment sp, ShippingCom sc , Stock s WHERE sp.SHID = sc.SHID and sp.ST_ID = s.ST_ID;\r\n";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        dgw.Rows.Clear(); // Clear any existing rows in the DataGridView

                        while (rdr.Read())
                        {
                            dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }


        private void dgw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShippingCom_pyment ShippingCom_pyment = ShippingCom_pyment.instance;
            DataGridViewRow dr = dgw.SelectedRows[0]; 
            ShippingCom_pyment.instance.TextBox2.Text = dr.Cells[0].Value.ToString();//SHPID
            ShippingCom_pyment.instance.txtSupplierID.Text = dr.Cells[1].Value.ToString();
            ShippingCom_pyment.instance.txtSupplierName.Text = dr.Cells[2].Value.ToString();
            ShippingCom_pyment.instance.textBox5.Text = dr.Cells[3].Value.ToString();
            ShippingCom_pyment.instance.textBox4.Text = dr.Cells[4].Value.ToString();
            ShippingCom_pyment.instance.txtTransactionAmount.Text = dr.Cells[5].Value.ToString();
            ShippingCom_pyment.instance.btnDelete.Enabled = true;

            this.Hide();



            ShippingCom_pyment.instance.TextBox3.Text = dr.Cells[0].Value.ToString();
            ShippingCom_pyment.instance.txtTransactionNo.Text = dr.Cells[1].Value.ToString();
            ShippingCom_pyment.instance.txtSupplierName.Text = dr.Cells[2].Value.ToString();
            ShippingCom_pyment.instance.txtTransactionAmount.Text = dr.Cells[6].Value.ToString();
            ShippingCom_pyment.instance.txtSup_ID.Text = dr.Cells[4].Value.ToString();
            ShippingCom_pyment.instance.txtSupplierID.Text = dr.Cells[5].Value.ToString();
          //  ShippingCom_pyment.instance.txtSupplierName.Text = dr.Cells[6].Value.ToString();
            ShippingCom_pyment.instance.cmbPaymentMode.Text = dr.Cells[7].Value.ToString();
           // ShippingCom_pyment.instance.txtRemarks.Text = dr.Cells[8].Value.ToString();
            ShippingCom_pyment.instance.btnSave.Enabled = false;
            ShippingCom_pyment.instance.GetCompanyBalance();
            ShippingCom_pyment.instance.btnUpdate.Enabled = true;
            ShippingCom_pyment.instance.btnDelete.Enabled = true;
            //ShippingCom_pyment.instance.GetSupplierInfo();
            ShippingCom_pyment.instance.btnSelection.Enabled = false;
        }

        private void TextBox4_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Query to filter based on partial match with ShappingName using LIKE
                    string query = @"SELECT 
                                sp.SHPID,
                                sc.ShappingCode AS SHPCode,
                                sc.ShappingName,
                                sp.ST_ID,
                                sp.SHID,
                                sp.TotalPrice,
                                sp.PymentMethod,
                                sp.Comments
                             FROM 
                                Shipping_Pyment sp
                             JOIN 
                                ShippingCom sc ON sp.SHID = sc.SHID
                             WHERE 
                                sc.ShappingName LIKE @ShappingName"; // Filtering with LIKE clause

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Add parameter for partial match on ShappingName with wildcards
                        cmd.Parameters.AddWithValue("@ShappingName", "%" + TextBox4.Text + "%");

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            dgw.Rows.Clear(); // Clear any existing rows in the DataGridView

                            while (rdr.Read())
                            {
                                dgw.Rows.Add(
                                    rdr["SHPID"],
                                    rdr["SHPCode"],
                                    rdr["ShappingName"],
                                    rdr["ST_ID"],
                                    rdr["SHID"],
                                    rdr["TotalPrice"],
                                    rdr["PymentMethod"],
                                    rdr["Comments"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            LoadData();

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
    }
}
