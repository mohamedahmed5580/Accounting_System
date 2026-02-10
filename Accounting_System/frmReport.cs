using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrystalDecisions.ReportAppServer.ReportDefModel;
using CrystalDecisions.Shared;
using CrystalDecisions.Windows.Forms;

using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Accounting_System
{

    public partial class frmReport : Form
    {

        public frmReport()
        {
            InitializeComponent();
        }
        // Button to export the report and send it to WhatsApp

        private void gunaAdvenceButton1_Click(object sender, EventArgs e)
        {
            // Ensure the message is not empty
         

            try
            {
                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
                {
                   
                    PrintAndExportReport();
                    sentthenassegee();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving the report: " + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void PrintAndExportReport()
        {
            // Check if crystalReportViewer1 has a valid report source
            if (crystalReportViewer1.ReportSource is CrystalDecisions.CrystalReports.Engine.ReportDocument reportDocument)
            {
                try
                {
                    string currentDirectory = Directory.GetCurrentDirectory();
                    // Define the export path
                    string exportPath = Path.Combine(currentDirectory, "Reports", "report.pdf");

                    // Create the 'Reports' directory if it doesn't exist
                    string reportsDirectory = Path.Combine(currentDirectory, "Reports");
                    if (!Directory.Exists(reportsDirectory))
                    {
                        Directory.CreateDirectory(reportsDirectory);
                    }

                    // Export the report to PDF
                    reportDocument.ExportToDisk(ExportFormatType.PortableDocFormat, exportPath);

                    // Print the report directly to the default printer
                    MessageBox.Show("Report saved successfully at " + exportPath, "Export Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Uncomment the following line if you want to print the report after exporting
                    // reportDocument.PrintToPrinter(1, false, 0, 0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error exporting or printing report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No report loaded in the viewer.", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



        private async void sentthenassegee()
        {
            // Get the message from the richTextBox1
            string currentDirectory = Directory.GetCurrentDirectory();
            string exportPath = currentDirectory + @"\Reports\";

            string message = "the PDF in " + exportPath;

            // Ensure the message is not empty
            if (string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("برجاء كتابة الرسالة", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Assuming the phone number is in the second column (SubItem[1])
            string phoneNumber = comboBox1.Text;

            // Clean up the phone number
            phoneNumber = phoneNumber.Trim().Replace(" ", "").Replace("-", "");

            // Ensure the phone number has the correct format
            if (!phoneNumber.StartsWith("2"))
            {
                phoneNumber = "2" + phoneNumber; // Modify according to your country code
            }

            // Call the sendMessage method with the formatted phone number and the message
            await sendMessageAsync(phoneNumber, message);

            // Wait for 2 seconds before sending the next message
            await Task.Delay(2000);
        }


        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        private async Task sendMessageAsync(string num, string message)
        {
            try
            {
                // Properly encode the message for use in a URL
                string encodedMessage = Uri.EscapeDataString(message);

                // Ensure the phone number format is correct
                string url = $"https://web.whatsapp.com/send?phone={num}&text={encodedMessage}";

                // Open the WhatsApp link in the default browser
                var process = System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });

                // Find the window titled "WhatsApp" and bring it to the foreground
                IntPtr hWnd = FindWindow(null, "WhatsApp");
                if (hWnd != IntPtr.Zero)
                {
                    SetForegroundWindow(hWnd);
                    await Task.Delay(2000); // Brief delay to ensure window is focused
                }
                // Give enough time for WhatsApp Web to open and load the chat
                await Task.Delay(20000);  // Adjust delay as needed for loading time

                // Simulate pressing "Enter" to send the message
                //SendKeys.Send("{ENTER}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void OpenWhatsAppWeb(string phoneNumber, string message)
        {
            // Format the message to be URL-friendly
            string encodedMessage = Uri.EscapeDataString(message);

            // Create the WhatsApp link
            string whatsappLink = $"https://wa.me/{phoneNumber}?text={encodedMessage}";

        }

        public int counter = 0;
        private void Timer1_Tick(object sender, EventArgs e)
        {
            // Simulate pressing the "Enter" key to send the message
            SendKeys.SendWait("{ENTER}");

            // Stop the timer after sending the message
            Timer1.Stop();
            counter = 0;
        }


        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {
            // Optional: Additional setup for the viewer can go here.
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.Text == "العملاء")
            {
                GetDataCustomer();


            }
            else if (comboBox2.Text == "الموردين")
            {
                GetDataSupplier();
            }
            else if (comboBox2.Text == "مناديب المبيعات")
            {
                GetDataSaleMan();
            }
        }

        private void GetDataCustomer()
        {
            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT RTRIM(Name), RTRIM(ContactNo) FROM Customer ORDER BY CustomerID", cn))
                {
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        comboBox1.Items.Clear(); // Clear the list before adding new items
                        comboBox3.Items.Clear(); // Clear the list before adding new items

                        // Loop through the SqlDataReader and add the data to the ComboBox
                        while (rdr.Read())
                        {
                            string name = rdr[0].ToString().Trim();
                            string contactNo = rdr[1].ToString().Trim();

                            // Combine name and contact number for display in the ComboBox
                            comboBox3.Items.Add(name);
                        }
                    }
                }
            }
        }

        private void GetDataSupplier()
        {
            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT RTRIM(Name), RTRIM(ContactNo) FROM Supplier ORDER BY SupplierID", cn))
                {
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        comboBox1.Items.Clear(); // Clear the list before adding new items
                        comboBox3.Items.Clear(); // Clear the list before adding new items

                        // Loop through the SqlDataReader and add the data to the ComboBox
                        while (rdr.Read())
                        {
                            string name = rdr[0].ToString().Trim();
                            string contactNo = rdr[1].ToString().Trim();

                            // Combine name and contact number for display in the ComboBox
                            string displayText = $"{name} - {contactNo}"; // You can format this as needed
                            comboBox3.Items.Add(name);
                        }
                    }
                }
            }
        }

        private void GetDataSaleMan()
        {
            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT RTRIM(Name), RTRIM(ContactNo) FROM SalesMan ORDER BY SalesMan_ID", cn))
                {
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        comboBox1.Items.Clear(); // Clear the list before adding new items
                        comboBox3.Items.Clear(); // Clear the list before adding new items

                        // Loop through the SqlDataReader and add the data to the ComboBox
                        while (rdr.Read())
                        {
                            string name = rdr[0].ToString().Trim();
                            string contactNo = rdr[1].ToString().Trim();

                            // Combine name and contact number for display in the ComboBox
                            string displayText = $"{name} - {contactNo}"; // You can format this as needed
                            comboBox3.Items.Add(name);
                        }
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(comboBox3.Text))
                return; // If no valid selection, exit the method

            using (SqlConnection cn = new SqlConnection(DataAccessLayer.Con()))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT RTRIM(Name), RTRIM(ContactNo) FROM Customer WHERE Name = @name ORDER BY CustomerID", cn))
                {
                    cmd.Parameters.AddWithValue("@name", comboBox3.Text);

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        comboBox1.Items.Clear(); // Clear the list before adding new items

                        // Loop through the SqlDataReader and add the contact numbers to comboBox1
                        while (rdr.Read())
                        {
                            string contactNo = rdr[1].ToString().Trim();
                            comboBox1.Items.Add(contactNo);
                        }
                    }
                }
            }
        }

        private void frmReport_Load(object sender, EventArgs e)
        {

        }
    }

}
