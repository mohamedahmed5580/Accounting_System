using Microsoft.Office.Interop.Excel;
using Pharmacy.PL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.Xpo.Helpers.PerformanceCounters;
using DataTable = System.Data.DataTable;

namespace Accounting_System
{
    public partial class Show_database : Form
    {
        public string str_connection_config;
        public string str_connection_newdatabase;
        public static Show_database instance = null;
        public static Show_database Instancee()
        {
            if (instance == null)
            {
                instance = new Show_database();
            }

            return instance;
        }
        public Show_database()
        {
            InitializeComponent();
            ShowCompany();
            instance = this;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {

            try
            {
                using (var F = new NewFile())
                {
                    F.Owner = this;
                    F.ShowDialog();
                }
            }
            catch
            {
            }
        }

        private void Show_database_Load(object sender, EventArgs e)
        {
            try
            {
                ShowCompany();
            }
            catch (Exception)
            {
                // Swallowing exceptions to match VB behavior.
            }

        }

        public void ShowCompany()
        {
            int count__ = 0;
            string str_connection_config;

            try
            {
                using (var con = new SqlConnection(DataAccessLayer.Con()))
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }

                    con.Open();
                    DGV.Rows.Clear();
                    var dt = new DataTable();
                    dt.Clear();

                    string query = "select [COMPANY], [databasename] from [masterDB].[dbo].[COMPANY]";
                    using (var adp = new SqlDataAdapter(query, con))
                    {
                        adp.Fill(dt);
                    }

                    if (dt.Rows.Count > 0)
                    {
                        DGV.RowCount = dt.Rows.Count;

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            DGV.Rows[count__].Cells[0].Value = dt.Rows[i]["COMPANY"].ToString();
                            DGV.Rows[count__].Cells[1].Value = dt.Rows[i]["databasename"].ToString();
                            count__++;
                        }
                    }

                    con.Close();
                }

            }
            catch (Exception)
            {
            }


        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

            DBConfig dBConfig = new DBConfig();
            dBConfig.Show();
        }

        private void DGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void DGV_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (DGV.CurrentRow != null && DGV.CurrentRow.Index >= 0)
                {
                    // أخذ قيمة اسم قاعدة البيانات
                    string selectedDatabase = DGV.CurrentRow.Cells[1].Value.ToString();

                    // حفظ الإعدادات
                    Accounting_System.Properties.Settings.Default.Database = selectedDatabase;
                    Accounting_System.Properties.Settings.Default.Save();

                    MessageBox.Show("تم اختيار قاعدة البيانات: " + selectedDatabase, "نجاح",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // فتح فورم التحميل
                    loading frm = new loading();
                    frm.Show();

                    // إخفاء الفورم الحالي بدل إغلاقه
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private string BuildMasterConnection()
        {
            if (Accounting_System.Properties.Settings.Default.Mode == false)
            {
                return $"Data Source={Properties.Settings.Default.Server};Initial Catalog=master;" +
                       $"User ID={Properties.Settings.Default.Database};Password={Properties.Settings.Default.Pass};Integrated Security=false";
            }
            else
            {
                return $"Data Source={Properties.Settings.Default.Server};Initial Catalog=master;Integrated Security=True";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (DGV.CurrentRow != null && DGV.CurrentRow.Index >= 0)
                {
                    string databaseName = DGV.CurrentRow.Cells[1].Value.ToString();
                    string companyName = DGV.CurrentRow.Cells[0].Value.ToString();

                    var confirm = MessageBox.Show(
                        "هل أنت متأكد أنك تريد حذف الشركة: " + companyName +
                        " وقاعدة البيانات: " + databaseName + " ؟",
                        "تأكيد الحذف",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (confirm == DialogResult.Yes)
                    {
                        // Ensure we are connecting to master or a neutral DB initially
                        using (var con = new SqlConnection(DataAccessLayer.Con()))
                        {
                            con.Open();

                            // أولاً: حذف السجل من جدول COMPANY
                            string query1 = @"DELETE FROM [masterDB].[dbo].[COMPANY] 
                                      WHERE [COMPANY] = @COMPANY 
                                      AND [databasename] = @databasename";

                            using (var cmd1 = new SqlCommand(query1, con))
                            {
                                cmd1.Parameters.AddWithValue("@COMPANY", companyName);
                                cmd1.Parameters.AddWithValue("@databasename", databaseName);

                                int rowsAffected = cmd1.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    // ثانياً: حذف قاعدة البيانات نفسها
                                    // We use 'USE master' to ensure we aren't locking the DB we want to delete.
                                    // We use 'SET SINGLE_USER' to kick off other users/connections immediately.
                                    string query2 = $@"
                                USE master;
                                ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                                DROP DATABASE [{databaseName}];";

                                    using (var cmd2 = new SqlCommand(query2, con))
                                    {
                                        cmd2.ExecuteNonQuery();
                                    }

                                    // Update UI after successful DB operations
                                    DGV.Rows.RemoveAt(DGV.CurrentRow.Index);

                                    MessageBox.Show("تم حذف الشركة وقاعدة البيانات بنجاح", "نجاح",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    MessageBox.Show("لم يتم العثور على الشركة في جدول COMPANY", "ملاحظة",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ: " + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }

    }
}
