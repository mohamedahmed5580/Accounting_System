using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Windows.Forms;
using WhatsAppApi;

namespace Accounting_System
{
    public partial class WSender : Form
    {
        /*        // Replace with your WhatsApp Cloud API access token and phone number ID
                private static readonly string accessToken = "EAAWT2ZAQC27cBO5SacrqdDYYLZAtO5dEip0EZBPZC0tqQ8iP3OpnCjxbcNFcX8CmuDukMPNcUY8Ou7TbVG3NQMYyWQy0CBP1vZBRqduX5pbnrg8LbFyXEZCwkZCJbD8wzrrLZCmG1XZCHpmMWzMtkLbzUwUbuntyFXqWR3JJm0uCLBaPPrmvgeOBeW3FJwzxYXCcCSwW0wx57lmKoujPKzmCXcLZCstMEZD"; // Replace this with your actual access token
                private static readonly string phoneNumberId = "+0201090971732"; // Replace with your Phone Number ID
                private static readonly string apiUrl = $"https://graph.facebook.com/v17.0/{phoneNumberId}/messages";
        */
/*        IWebDriver m_driver;
        private delegate void SafeCallDelegate(string text);
        private delegate void SafeCallDelegateToEnable(bool status);
        private bool bStopSendingMessage = false;*/
        public WSender()
        {
            InitializeComponent();
/*            m_driver = new ChromeDriver();
            m_driver.Url = "https://web.whatsapp.com/";
            m_driver.Manage().Window.Minimize();*/

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
            }else if (comboBox2.Text == "مناديب المبيعات")
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
                        listView1.Items.Clear(); // Clear the list before adding new items

                        // Loop through the SqlDataReader and add the data to the ListView
                        while (rdr.Read())
                        {
                            ListViewItem item = new ListViewItem
                            {
                                Text = rdr[0].ToString().Trim() // Set the 'Name' as the main text
                            };

                            // Add 'ContactNo' as a subitem
                            item.SubItems.Add(rdr[1].ToString().Trim());

                            // No third column to read here, so no attempt to access rdr[2]
                            listView1.Items.Add(item);
                        }
                    }
                }

        
                cn.Close();
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
                        listView1.Items.Clear(); // Clear the list before adding new items

                        // Loop through the SqlDataReader and add the data to the ListView
                        while (rdr.Read())
                        {
                            ListViewItem item = new ListViewItem
                            {
                                Text = rdr[0].ToString().Trim() // Set the 'Name' as the main text
                            };

                            // Add 'ContactNo' as a subitem
                            item.SubItems.Add(rdr[1].ToString().Trim());

                            // No third column to read here, so no attempt to access rdr[2]
                            listView1.Items.Add(item);
                        }
                    }
                }

                // Optionally check all items in the ListView after adding them
                /*                for (int i = 0; i < listView1.Items.Count; i++)
                                {
                                    listView1.Items[i].Checked = true;
                                }*/

                cn.Close();
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
                        listView1.Items.Clear(); // Clear the list before adding new items

                        // Loop through the SqlDataReader and add the data to the ListView
                        while (rdr.Read())
                        {
                            ListViewItem item = new ListViewItem
                            {
                                Text = rdr[0].ToString().Trim() // Set the 'Name' as the main text
                            };

                            // Add 'ContactNo' as a subitem
                            item.SubItems.Add(rdr[1].ToString().Trim());

                            // No third column to read here, so no attempt to access rdr[2]
                            listView1.Items.Add(item);
                        }
                    }
                }

                // Optionally check all items in the ListView after adding them
                /*                for (int i = 0; i < listView1.Items.Count; i++)
                                {
                                    listView1.Items[i].Checked = true;
                                }*/

                cn.Close();
            }
        }



        private async void button1_Click(object sender, EventArgs e)
        {
            // Get the message from the richTextBox1
            string message = richTextBox1.Text;

            // Ensure the message is not empty
            if (string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("برجاء كتابة الرسالة", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Loop through the selected items in listView1
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.Checked)
                {
                    // Assuming the phone number is in the second column (SubItem[1])
                    string phoneNumber = item.SubItems[1].Text;

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
            }
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
                SendKeys.Send("{ENTER}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void WSender_Load(object sender, EventArgs e)
        {

        }
    }
}
