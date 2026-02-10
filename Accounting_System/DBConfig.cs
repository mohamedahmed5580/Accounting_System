using Accounting_System;

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


namespace Pharmacy.PL
{
    public partial class DBConfig : MetroFramework.Forms.MetroForm
    {
        public DBConfig()
        {
            InitializeComponent();
            // Example of accessing a setting called "ConnectionString" in Properties.Settings

            if (Accounting_System.Properties.Settings.Default.Mode == true)
                rbWindows.Checked = true;
            else
                rdSQL.Checked = true;

            tbServer.Text = Accounting_System.Properties.Settings.Default.Server;
            tbDb.Text = Accounting_System.Properties.Settings.Default.Database;
            tbUser.Text = Accounting_System.Properties.Settings.Default.Name;
            tbPass.Text = Accounting_System.Properties.Settings.Default.Pass;
        }

        private void rbWindows_CheckedChanged(object sender, EventArgs e)
        {
            if (rbWindows.Checked == true)
                tbUser.Enabled = tbPass.Enabled = false;
            else
                tbUser.Enabled = tbPass.Enabled = true;

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            Accounting_System.Properties.Settings.Default.Mode = (rbWindows.Checked == true) ? true : false;
            Accounting_System.Properties.Settings.Default.Server = tbServer.Text;
            Accounting_System.Properties.Settings.Default.Database = tbDb.Text;
            Accounting_System.Properties.Settings.Default.Name = tbUser.Text;
            Accounting_System.Properties.Settings.Default.Pass = tbPass.Text;
            Accounting_System.Properties.Settings.Default.Save();

            MetroFramework.MetroMessageBox.Show(this, "", "Success", MessageBoxButtons.OK, MessageBoxIcon.Question);
            this.Hide();

        }

        private void rdSQL_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void DBConfig_Load(object sender, EventArgs e)
        {

        }

        private void tbServer_TextChanged(object sender, EventArgs e)
        {

        }

        private void gunaButton1_Click(object sender, EventArgs e)
        {
            /* try
             {
                 OpenFileDialog openFileDialog1 = new OpenFileDialog
                 {
                     Filter = "DB Backup File|*.bak;",
                     FilterIndex = 1,
                     Title = "Select a Backup File",
                     FileName = ""
                 };

                 if (openFileDialog1.ShowDialog() == DialogResult.OK)
                 {
                     SqlConnection.ClearAllPools();

                     string connectionString = @"Server=" + Accounting_System.Properties.Settings.Default.Server + ";Database=master;Integrated Security=True;";
                     string databaseName = "sa"; // Replace with your database name

                     using (SqlConnection con = new SqlConnection(connectionString))
                     {
                         con.Open();

                         // Get default data path from SQL Server
                         string defaultDataPath;
                         using (SqlCommand cmd = new SqlCommand("SELECT SERVERPROPERTY('InstanceDefaultDataPath')", con))
                         {
                             defaultDataPath = cmd.ExecuteScalar().ToString();
                         }

                         // Get file list from backup
                         DataTable fileList = new DataTable();
                         using (SqlCommand cmd = new SqlCommand($"RESTORE FILELISTONLY FROM DISK = '{openFileDialog1.FileName}'", con))
                         {
                             using (SqlDataReader dr = cmd.ExecuteReader())
                             {
                                 fileList.Load(dr);
                             }
                         }

                         // Build MOVE clauses
                         List<string> moveClauses = new List<string>();
                         foreach (DataRow row in fileList.Rows)
                         {
                             string logicalName = row["LogicalName"].ToString();
                             string physicalName = Path.GetFileName(row["PhysicalName"].ToString());
                             string newPath = Path.Combine(defaultDataPath, physicalName);
                             moveClauses.Add($"MOVE '{logicalName}' TO '{newPath}'");
                         }

                         // Build restore command
                         string restoreQuery = $@"
                   ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                   RESTORE DATABASE [{databaseName}] 
                   FROM DISK = '{openFileDialog1.FileName}' 
                   WITH {string.Join(", ", moveClauses)}, REPLACE;
                   ALTER DATABASE [{databaseName}] SET MULTI_USER;";

                         using (SqlCommand cmd = new SqlCommand(restoreQuery, con))
                         {
                             cmd.CommandTimeout = 0; // Set appropriate timeout
                             cmd.ExecuteNonQuery();
                         }
                     }

                     MessageBox.Show("Database restored successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                 }
                 else
                 {
                     MessageBox.Show("No backup file was selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 }
             }
             catch (Exception ex)
             {
                 MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
             }
 */
            try
            {
                // Configure OpenFileDialog to select the backup file
                OpenFileDialog openFileDialog1 = new OpenFileDialog
                {
                    Filter = "DB Backup File|*.bak;",
                    FilterIndex = 1,
                    Title = "Select a Backup File",
                    FileName = "" // Clear the file name
                };

                // Show the file dialog and get the selected file
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    // Clear connection pools to avoid issues with database locking
                    SqlConnection.ClearAllPools();

                    // Define the connection string with Windows Authentication
                    string connectionString = @"Server=" + Accounting_System.Properties.Settings.Default.Server + ";Database=master;Integrated Security=True;";

                    // Database name to restore
                    string databaseName = "sa"; // Replace with your database name

                    // SQL command to check and create the database if it doesn't exist
                    string createDatabaseQuery = $@"
                                            IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{databaseName}')
                                            BEGIN
                                                CREATE DATABASE [{databaseName}];
                                            END";

                    // SQL commands for restore
                    string setSingleUserQuery = $"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                    string restoreQuery = $"RESTORE DATABASE [{databaseName}] FROM DISK = '{openFileDialog1.FileName}' WITH REPLACE;";
                    string setMultiUserQuery = $"ALTER DATABASE [{databaseName}] SET MULTI_USER;";

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();

                        // Check and create the database if necessary
                        using (SqlCommand cmd = new SqlCommand(createDatabaseQuery, con))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        // Set the database to SINGLE_USER mode
                        using (SqlCommand cmd = new SqlCommand(setSingleUserQuery, con))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        // Perform the restore operation
                        using (SqlCommand cmd = new SqlCommand(restoreQuery, con))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        // Set the database back to MULTI_USER mode
                        using (SqlCommand cmd = new SqlCommand(setMultiUserQuery, con))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Log success and show message
                    MessageBox.Show("Database restored successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No backup file was selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                // Show detailed error message
                MessageBox.Show($"An error occurred during the restore operation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pbDBConfig_Click(object sender, EventArgs e)
        {
            Show_database show_Database = new Show_database();
            show_Database.ShowDialog();
        }
    }
}