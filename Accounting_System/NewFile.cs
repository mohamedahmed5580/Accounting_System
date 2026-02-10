using Microsoft.Office.Interop.Excel;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Accounting_System
{
    public partial class NewFile : Form
    {
        public string str_connection_config;
        public string str_connection_newdatabase;

        public NewFile()
        {
            InitializeComponent();
            string serverName = Environment.MachineName + @"\SQLEXPRESS";
            

            tbServer.Text = serverName;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TXT_DATABASE.Text))
                {
                    MessageBox.Show(" يجب كتابة اسم قاعدة البيانات", "خطأ",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(TXT_NAME.Text))
                {
                    MessageBox.Show(" يجب كتابة اسم الملف ", "خطأ",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Accounting_System.Properties.Settings.Default.Mode = (rbWindows.Checked == true) ? true : false;
                Accounting_System.Properties.Settings.Default.Server = tbServer.Text;
                Accounting_System.Properties.Settings.Default.Database = TXT_DATABASE.Text;

                Accounting_System.Properties.Settings.Default.Save();
                connectiondatatbase();

                // Always connect to master
                using (var con = new SqlConnection(BuildMasterConnection()))
                {
                    con.Open();

                    // Check if masterDB exists
                    using (var cmd = new SqlCommand("SELECT name FROM sys.databases WHERE name = 'masterDB'", con))
                    {
                        var result = cmd.ExecuteScalar();
                        if (result == null)
                        {
                            createconfig();
                        }
                    }

                    // Check if database already exists in COMPANY
                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM  [masterDB].[dbo].[COMPANY] WHERE databasename = @dbname", con))
                    {
                        cmd.Parameters.AddWithValue("@dbname", TXT_DATABASE.Text.Trim());
                        int exists = (int)cmd.ExecuteScalar();
                        if (exists > 0)
                        {
                            MessageBox.Show("اسم قاعدة البيانات موجود من قبل");
                            return;
                        }
                    }

                    // Create new DB
                    using (var cmd = new SqlCommand(
                        $"CREATE DATABASE [{TXT_DATABASE.Text.Trim()}] COLLATE Arabic_CI_AS", con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

               

                create_tb();
                insert_config();
                MessageBox.Show("تم انشاء الملف بنجاح", "ملف جديد",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Show_database show_Db =  Show_database.instance;
                show_Db.ShowCompany();
                this.Close();
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
        private void connectiondatatbase()
        {
            if (!Accounting_System.Properties.Settings.Default.Mode == true)
            {
                str_connection_config =
                    "Data Source=" + Properties.Settings.Default.Server +
                    ";Initial Catalog=masterDB;User ID=" + Properties.Settings.Default.Database +
                    ";password=" + Properties.Settings.Default.Pass +
                    ";Integrated Security=false";

                str_connection_newdatabase =
                    "Data Source=" + Properties.Settings.Default.Server +
                    ";Initial Catalog=" + TXT_DATABASE.Text.Trim() +
                    ";User ID=" + Properties.Settings.Default.Database +
                    ";password=" + Properties.Settings.Default.Pass +
                    ";Integrated Security=false";
            }
            else
            {
                str_connection_config =
                    " Data Source=" + Properties.Settings.Default.Server +
                    ";Initial Catalog=masterDB;Integrated Security=True";

                str_connection_newdatabase =
                    " Data Source=" + Properties.Settings.Default.Server +
                    ";Initial Catalog=" + TXT_DATABASE.Text.Trim() +
                    ";Integrated Security=True";
            }
        }

        private void insert_config()
{
    // تأكيد أن الاتصال على masterDB
    var cs = str_connection_config.Replace(
        "Initial Catalog=master;",
        "Initial Catalog=masterDB;");

    using (var con = new SqlConnection(cs))
    {
        con.Open();

        string sql = "INSERT INTO COMPANY (COMPANY, databasename) VALUES (@company, @dbname)";
        using (var cmd = new SqlCommand(sql, con))
        {
            cmd.Parameters.AddWithValue("@company", TXT_NAME.Text.Trim());
            cmd.Parameters.AddWithValue("@dbname", TXT_DATABASE.Text.Trim());
            cmd.ExecuteNonQuery();
        }
    }
}


        private void create_tb()
        {
            using (var cn_new = new SqlConnection(str_connection_newdatabase))
            {
                cn_new.Open();

                string script = File.ReadAllText(System.Windows.Forms.Application.StartupPath + "\\db_script.sql");
                var commandStrings = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                foreach (var commandString in commandStrings)
                {
                    if (!string.IsNullOrWhiteSpace(commandString))
                    {
                        using (var command = new SqlCommand(commandString, cn_new))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private void createconfig()
        {
            using (var con = new SqlConnection(BuildMasterConnection()))
            {
                con.Open();

                using (var cmd = new SqlCommand("CREATE DATABASE [masterDB] COLLATE Arabic_CI_AS", con))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            using (var con = new SqlConnection(str_connection_config))
            {
                con.Open();

                string script = File.ReadAllText(System.Windows.Forms.Application.StartupPath + "\\master_script.sql");
                var commandStrings = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                foreach (var commandString in commandStrings)
                {
                    if (!string.IsNullOrWhiteSpace(commandString))
                    {
                        using (var command = new SqlCommand(commandString, con))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

      

        private void CMB_CURRENCY_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (CMB_CURRENCY.Text == "دولار أمريكي")
                {
                    TXT_LATIN.Text = "USD";
                    TXT_LEVEL_CURRENCYE.Text = "سنت";
                }
                if (CMB_CURRENCY.Text == "جنيه مصري")
                {
                    TXT_LATIN.Text = "Egypt Pound";
                    TXT_LEVEL_CURRENCYE.Text = "قرش";
                }
                if (CMB_CURRENCY.Text == "درهم إماراتي")
                {
                    TXT_LATIN.Text = "UAE Dirham";
                    TXT_LEVEL_CURRENCYE.Text = "فلس";
                }
                if (CMB_CURRENCY.Text == "دينار كويتي")
                {
                    TXT_LATIN.Text = "Kuwait Dinar";
                    TXT_LEVEL_CURRENCYE.Text = "فلس";
                }
                if (CMB_CURRENCY.Text == "ريال سعودي")
                {
                    TXT_LATIN.Text = "Saudi Riyal";
                    TXT_LEVEL_CURRENCYE.Text = "هللة";
                }
                if (CMB_CURRENCY.Text == "دينار عراقي")
                {
                    TXT_LATIN.Text = "Iraq Dinar";
                    TXT_LEVEL_CURRENCYE.Text = "فلس";
                }
                if (CMB_CURRENCY.Text == "ليرة لبنانية")
                {
                    TXT_LATIN.Text = "Lebanon Pound";
                    TXT_LEVEL_CURRENCYE.Text = "قرش";
                }
                if (CMB_CURRENCY.Text == "ليرة سورية")
                {
                    TXT_LATIN.Text = "Syrian Pound";
                    TXT_LEVEL_CURRENCYE.Text = "قرش";
                }
                if (CMB_CURRENCY.Text == "دينار بحريني")
                {
                    TXT_LATIN.Text = "Bahrain Dinar";
                    TXT_LEVEL_CURRENCYE.Text = "فلس";
                }
                if (CMB_CURRENCY.Text == "ريال قطري")
                {
                    TXT_LATIN.Text = "Qatar Riyal";
                    TXT_LEVEL_CURRENCYE.Text = "هللة";
                }
                if (CMB_CURRENCY.Text == "ريال يمني")
                {
                    TXT_LATIN.Text = "Yemen Riyal";
                    TXT_LEVEL_CURRENCYE.Text = "هللة";
                }
                if (CMB_CURRENCY.Text == "يورو")
                {
                    TXT_LATIN.Text = "Euro";
                    TXT_LEVEL_CURRENCYE.Text = "سنت";
                }
            }
            catch (Exception)
            {
                // Intentionally empty
            }
        }

        private void CMB_CURRENCY_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void NewFile_Load(object sender, EventArgs e)
        {

        }
    }
}
