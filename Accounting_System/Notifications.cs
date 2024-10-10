using Microsoft.Win32;
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
using System.IO;
using static DevExpress.Data.Helpers.ExpressiveSortInfo;
using System.Xml.Linq;
namespace Accounting_System
{
    public partial class Notifications : Form
    {
        public static Notifications instance;
        public static TimeZoneInfo egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
        public static DateTime egyptTime = TimeZoneInfo.ConvertTime(DateTime.Now, egyptTimeZone);

        public Notifications()
        {
            InitializeComponent();
            instance = this;
        }

        private void Notifications_Load(object sender, EventArgs e)
        {

            GetNotifications();
            notificationTimer.Start();
            CheckForQuantity();

        }
        private void GetNotifications()
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT NID, Name,Hours,Minutes,Timing, Date FROM Notifications", con))
                {
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        dgw.Rows.Clear(); // Clear any existing rows in the DataGridView

                        while (rdr.Read())
                        {
                            // Add rows to the DataGridView with the retrieved data
                            dgw.Rows.Add(
                                rdr["NID"].ToString(),         // First column: ID
                                rdr["Name"].ToString(),       // Second column: Name
                                rdr["Hours"].ToString(),       // Third  column: hours
                                rdr["Minutes"].ToString(),       // Third  column: hours
                                rdr["Timing"].ToString(),       // Third  column: hours
                                Convert.ToDateTime(rdr["Date"]).ToString("yyyy-MM-dd") // Fours column: Date (formatted)
                            );
                        }
                    }
                }
            }
            comboBox1.Text = egyptTime.ToString("tt");
            auto();
        }
        // Set up the timer to check for notifications every minute
       
        private void btnSave_Click(object sender, EventArgs e)
        {
            // Step 1: Insert the data into the DataGridView
            // dgw.Rows.Add(txtCompanyName.Text, dateTimePicker1.Value.ToString());

            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();


                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Notifications (Name, Hours,Minutes,Timing, Date,IsNotified) VALUES (@Name, @Hours,@Minutes,@Timing, @Date,@IsNotified)", con))
                    {
                        // Insert values from the DataGridView row
                        cmd.Parameters.AddWithValue("@Name", txtCompanyName.Text); // Name from the first cell

                        // Ensure Hours is a valid integer
                        int hours;
                        if (int.TryParse(textBox1.Text, out hours))
                        {
                            cmd.Parameters.AddWithValue("@Hours", hours); // Valid integer value
                        }
                        else
                        {
                            MessageBox.Show("Invalid value for Hours. Please enter a valid number.");
                            return;
                        }
                        int Minutes;
                        if (int.TryParse(textBox2.Text, out Minutes))
                        {
                            cmd.Parameters.AddWithValue("@Minutes", Minutes); // Valid integer value
                        }
                        else
                        {
                            MessageBox.Show("Invalid value for Hours. Please enter a valid number.");
                            return;
                        }
                        int IsNotified;
                        if (int.TryParse(textBox2.Text, out IsNotified))
                        {
                            cmd.Parameters.AddWithValue("@IsNotified", 0); // Valid integer value
                        }
                        else
                        {
                            MessageBox.Show("Invalid value for Hours. Please enter a valid number.");
                            return;
                        }

                        try
                        {
                            cmd.Parameters.AddWithValue("@Timing", comboBox1.SelectedItem); // Valid integer value

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("خطأ في اختيار التوقيت");

                        }

                        // Ensure the Date is handled properly
                        DateTime dateValue;
                        if (DateTime.TryParse(dateTimePicker1.Text.ToString(), out dateValue))
                        {
                            cmd.Parameters.AddWithValue("@Date", dateValue); // Valid date value
                        }
                        else
                        {
                            MessageBox.Show("Invalid date format.");
                            return;
                        }

                        // Execute the query
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("تم حفظ التاريخ بنجاح");

                    }
                }

            }
            catch
            {
                MessageBox.Show("erorr in save data");

            }
            // Step 2: Insert the data into the database
            GetNotifications();

        }

        private string GenerateID()
        {
            string value = "0000";
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                try
                {
                    // Fetch the latest ID from the database
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 NID FROM Notifications ORDER BY NID DESC", con))
                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            value = rdr["PID"].ToString();
                        }
                    }
                    // we need to replace them. If necessary.
                    if (Convert.ToDouble(value) <= 9) // Value is between 0 and 10
                    {
                        value = "000" + value;
                    }
                    else if (Convert.ToDouble(value) <= 99) // Value is between 9 and 100
                    {
                        value = "00" + value;
                    }
                    else if (Convert.ToDouble(value) <= 999) // Value is between 999 and 1000
                    {
                        value = "0" + value;
                    }
                    // Increase the ID by 1
                    int numericValue = int.Parse(value);
                    numericValue += 1;
                    value = numericValue.ToString("D4"); // Ensure the string is padded with leading zeros if necessary
                }
                catch (Exception ex)
                {
                    // If an error occurs, set the value to "0000"
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                    value = "0000";
                }
            }
            return value;
        }
        private void auto()
        {
            try
            {
                txtID.Text = GenerateID();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void notificationTimer_Tick(object sender, EventArgs e)
        {

            CheckForNotifications();
        }



        private void CheckForNotifications()
        {
            try
            {
                DateTime egyptTime = TimeZoneInfo.ConvertTime(DateTime.Now, egyptTimeZone);
                string currentDate = egyptTime.ToString("yyyy-MM-dd");
                string currentPeriod = egyptTime.ToString("tt"); // AM or PM
                int currentHour = egyptTime.Hour > 12 ? egyptTime.Hour - 12 : egyptTime.Hour;
                currentHour = currentHour == 0 ? 12 : currentHour; // Adjust for midnight/noon
                string currentHourStr = currentHour.ToString("D2");
                string currentMinuteStr = egyptTime.Minute.ToString("D2");

                label9.Text = currentDate;
                label11.Text = currentHourStr;
                label12.Text = currentMinuteStr;

                List<int> notificationIds = new List<int>();

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();
                    string query = @"SELECT NID, Name FROM Notifications 
                             WHERE CAST(Date AS DATE) = @CurrentDate 
                             AND Hours = @CurrentHour 
                             AND Minutes = @CurrentMinute 
                             AND Timing = @CurrentPeriod 
                             AND IsNotified = 0";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CurrentDate", currentDate);
                        cmd.Parameters.AddWithValue("@CurrentHour", currentHourStr);
                        cmd.Parameters.AddWithValue("@CurrentMinute", currentMinuteStr);
                        cmd.Parameters.AddWithValue("@CurrentPeriod", currentPeriod);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                string notificationName = rdr["Name"].ToString();
                                int notificationId = Convert.ToInt32(rdr["NID"]);

                                // Show the notification    
                                ShowNotification("تذكير", $"الاشعار: {notificationName}");

                                // Collect the NID for updating later
                                notificationIds.Add(notificationId);
                            }
                        }

                        // After closing the reader, perform the updates
                        foreach (int nid in notificationIds)
                        {
                            string updateQuery = "UPDATE Notifications SET IsNotified = 1 WHERE NID = @Id";
                            using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                            {
                                updateCmd.Parameters.AddWithValue("@Id", nid);
                                int rowsAffected = updateCmd.ExecuteNonQuery();

                                // Optional: Log or verify if the update was successful
                                if (rowsAffected == 0)
                                {
                                    // Handle the case where no rows were updated
                                    LogError(new Exception($"No rows updated for NID: {nid}"));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                LogError(ex);
            }
        }
        private void LogError(Exception ex)
        {
            // Implement logging logic here (e.g., write to a file, event log, etc.)
            // Example:
            System.IO.File.AppendAllText("error_log.txt", $"{DateTime.Now}: {ex.Message}{Environment.NewLine}");
        }

        private void MarkNotificationAsNotified(int notificationId, SqlConnection con)
        {
            string updateQuery = "UPDATE Notifications SET IsNotified = 1 WHERE NID = @Id";
            using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
            {
                updateCmd.Parameters.AddWithValue("@Id", notificationId);
                updateCmd.ExecuteNonQuery();

            }
        }

        private void CheckForQuantity()
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                // Open the connection
                con.Open();

                // Define the query and command
                using (SqlCommand cmd = new SqlCommand("SELECT PID, RTRIM(Product.ProductCode), RTRIM(ProductName), Qty " +
                                         "FROM Temp_Stock, Product " +
                                         "WHERE Product.PID = Temp_Stock.ProductID AND Qty = 0 " +
                                         "ORDER BY ProductCode", con))
                {
                    // Execute the query and read the data
                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        // Clear the DataGridView before adding new rows
                        dgw2.Rows.Clear();

                        // Loop through the data and add rows to the DataGridView
                        while (rdr.Read())
                        {
                            dgw2.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3]);
                            if (rdr[3].ToString() == "0")
                            {
                                // Show the notification    
                                ShowNotification("تذكير", $"الاشعار: هاذا الصنف قد نفذ   {rdr[2].ToString()}");

                            }
                        }

                    }
                }
            }
        }



        private void ShowNotification(string title, string message)
        {
            // Display a balloon tip notification
            notifyIcon1.BalloonTipTitle = title;
            notifyIcon1.BalloonTipText = message;
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info; // You can set it to Info, Warning, or Error
            notifyIcon1.ShowBalloonTip(2000); // Display for 3 seconds/
            

        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateNotification();
        }
        private void UpdateNotification()
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();


                using (SqlCommand cmd = new SqlCommand("UPDATE Notifications SET Name = @Name,Hours=@Hours,Minutes=@Minutes,Timing=@Timing, Date = @Date WHERE NID = @id", con))
                {
                    // Assuming id is in the first column (Cells[0]) and needs to be updated
                    cmd.Parameters.AddWithValue("@id", txtID.Text); // ID of the record
                    cmd.Parameters.AddWithValue("@Name", txtCompanyName.Text.ToString()); // Name from the second cell
                    cmd.Parameters.AddWithValue("@Hours", textBox1.Text); // Name from the second cell
                    cmd.Parameters.AddWithValue("@Minutes", textBox2.Text); // name from the second cell
                    cmd.Parameters.AddWithValue("@Timing", comboBox1.Text.ToString()); // Name from the second cell
                    cmd.Parameters.AddWithValue("@Date", Convert.ToDateTime(dateTimePicker1.Value)); // Date from the third cell

                    // Execute the query
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("تمت عملية التعديل بنجاح");

                }

            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteNotification();
        }
        private void DeleteNotification()
        {
            if (dgw.SelectedRows.Count > 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = dgw.SelectedRows[0];

                // Get the id from the first column (assuming the id is stored there)
                int id = Convert.ToInt32(selectedRow.Cells[0].Value);

                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Notifications WHERE NID = @id", con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        // Execute the query
                        cmd.ExecuteNonQuery();
                    }
                }

                // Remove the row from DataGridView
                dgw.Rows.Remove(selectedRow);
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            txtCompanyName.Text = "";
            textBox1.Text = "";
            dateTimePicker1.ResetText();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        
            //ShowNotification(textBox1.Text, txtCompanyName.Text);
            AddApplicationToStartup();
        }

        public static void AddApplicationToStartup()
        {
            string appName = "URTECH";
            string appPath = Application.ExecutablePath; // Path of the running application

            RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (rk.GetValue(appName) == null)
            {
                rk.SetValue(appName, appPath);
            }
        }

        public static void RemoveApplicationFromStartup()
        {
            string appName = "URTECH";

            RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (rk.GetValue(appName) != null)
            {
                rk.DeleteValue(appName);
            }
        }

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
  

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();


        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the key pressed is not a control key (like backspace) and is not a digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                // If it's not a number, ignore the input
                e.Handled = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void label9_Click(object sender, EventArgs e)
        {
           
        }

        private void label11_Click(object sender, EventArgs e)
        {
            //label11.Text= int.Parse(egyptTime.ToString("mm")).ToString();
           
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void Notifications_FormClosing(object sender, FormClosingEventArgs e)
        {
            

        }

        private void Notifications_Move(object sender, EventArgs e)
        {
            if (this.WindowState==FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Notifications_FormClosed(object sender, FormClosedEventArgs e)
        {
            


        }

        private void dgw_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (dgw.Rows.Count > 0)
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];
                    txtID.Text = dr.Cells[0].Value.ToString();
                    txtCompanyName.Text = dr.Cells[1].Value.ToString();
                    textBox1.Text = dr.Cells[2].Value.ToString();
                    textBox2.Text = dr.Cells[3].Value.ToString();
                    comboBox1.Text = dr.Cells[4].Value.ToString();
                    dateTimePicker1.Value =Convert.ToDateTime( dr.Cells[5].Value);
                    btnUpdate.Enabled = true;
                    btnDelete.Enabled = true;
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void gunaImageButton1_Click(object sender, EventArgs e)
        {
            this.Hide();

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void dgw2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
      
        private void dgw2_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void Panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Label3_Click(object sender, EventArgs e)
        {

        }

        private void txtCompanyName_TextChanged(object sender, EventArgs e)
        {

        }

        private void Panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblUser_Click(object sender, EventArgs e)
        {

        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void txtTIN_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtID_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSTNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCIN_TextChanged(object sender, EventArgs e)
        {

        }

        private void Label6_Click(object sender, EventArgs e)
        {

        }

        private void Label8_Click(object sender, EventArgs e)
        {

        }

        private void Label7_Click(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            Pymentinvoice frmPurchaseEntry = new Pymentinvoice();
            frmPurchaseEntry.lblUser.Text = lblUser.Text;
            frmPurchaseEntry.Reset();
            frmPurchaseEntry.Show();
        }
    }
}
