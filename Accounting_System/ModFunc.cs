using Accounting_System; // Assuming StockBalance is here
 // Using the Data Access Layer namespace
using System;
using System.Data; // Added for SqlDbType
using System.Data.SqlClient; // Added for SqlException
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web; // Keep if HttpUtility is needed elsewhere
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
namespace Accounting_System
{
    public static class ModFunc
    {
        public static bool CheckForInternetConnection()
        {
            try
            {
                // Use a reliable, general-purpose address
                using (var client = new WebClient())
                // Setting a timeout is good practice
                using (var stream = client.OpenRead("http://clients3.google.com/generate_204")) // Google's lightweight check
                {
                    return true;
                }
            }
            catch (WebException) // Catch specific network errors
            {
                return false;
            }
            catch // Catch other potential errors
            {
                return false;
            }
        }

        public static void SMS(string st1)
        {
            try
            {
                string cb = "INSERT INTO SMS(Message, Date) VALUES (@d1, @d2)";
                var parameters = new[] {
                DataAccessLayer.CreateParameter("@d1", SqlDbType.NVarChar, st1),
                DataAccessLayer.CreateParameter("@d2", SqlDbType.DateTime, DateTime.Now)
            };
                DataAccessLayer.ExecuteNonQuery(cb, CommandType.Text, parameters);
            }
            catch (SqlException dbEx)
            {
                MessageBox.Show("Database Error saving SMS: " + dbEx.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("General Error saving SMS: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void LogFunc(string st1, string st2)
        {
            try
            {
                string cb = "INSERT INTO Logs(UserID, Date, Operation) VALUES (@d1, @d2, @d3)";
                var parameters = new[] {
                DataAccessLayer.CreateParameter("@d1", SqlDbType.NVarChar, st1), // Assuming UserID is string-based
                DataAccessLayer.CreateParameter("@d2", SqlDbType.DateTime, DateTime.Now),
                DataAccessLayer.CreateParameter("@d3", SqlDbType.NVarChar, st2)
            };
                DataAccessLayer.ExecuteNonQuery(cb, CommandType.Text, parameters);
            }
            catch (SqlException dbEx)
            {
                // Logging errors should ideally not show message boxes to the user
                Console.WriteLine("Database Error logging action: " + dbEx.Message);
                // Optionally show message box if required by application flow:
                // MessageBox.Show("Database Error logging action: " + dbEx.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error logging action: " + ex.Message);
                // Optionally show message box:
                // MessageBox.Show("General Error logging action: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SMSFunc(string st1, string st2, string st3) // Sends SMS via HTTP API
        {
            try
            {
                // Replace placeholders in the API URL template
                string apiUrl = st3.Replace("@MobileNo", Uri.EscapeDataString(st1)) // URL encode parameters
                                   .Replace("@Message", Uri.EscapeDataString(st2));

                var myUri = new Uri(apiUrl);
                var request = (HttpWebRequest)WebRequest.Create(myUri);
                request.Method = "GET"; // Or "POST" depending on the API requirement
                request.Timeout = 15000; // Set a timeout (e.g., 15 seconds)

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    // Optionally check response.StatusCode == HttpStatusCode.OK
                    // Optionally read response stream if the API returns useful info
                }
            }
            catch (UriFormatException uriEx)
            {
                MessageBox.Show("Invalid SMS API URL format: " + uriEx.Message, "SMS API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (WebException webEx)
            {
                MessageBox.Show("Error sending SMS via API: " + webEx.Message, "SMS API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // You might want to log webEx.Response if available for more details
            }
            catch (Exception ex)
            {
                MessageBox.Show("General Error in SMSFunc: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static string Encrypt(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;
            try
            {
                byte[] encode = Encoding.UTF8.GetBytes(password);
                return Convert.ToBase64String(encode);
            }
            catch (Exception ex) // Should not generally happen with Base64
            {
                Console.WriteLine("Encryption Error: " + ex.Message);
                return string.Empty; // Or throw
            }
        }

        public static string Decrypt(string encryptpwd)
        {
            if (string.IsNullOrEmpty(encryptpwd)) return string.Empty;
            try
            {
                byte[] todecodeByte = Convert.FromBase64String(encryptpwd);
                // Using Decoder directly is less common, GetString is simpler:
                return Encoding.UTF8.GetString(todecodeByte);

                /* // Original longer way:
                Decoder decode = Encoding.UTF8.GetDecoder();
                int charCount = decode.GetCharCount(todecodeByte, 0, todecodeByte.Length);
                char[] decodedChar = new char[charCount];
                decode.GetChars(todecodeByte, 0, todecodeByte.Length, decodedChar, 0);
                return new string(decodedChar);
                 */
            }
            catch (FormatException) // Catch specific error for invalid Base64
            {
                Console.WriteLine("Decryption Error: Input is not a valid Base64 string.");
                return string.Empty; // Or throw
            }
            catch (Exception ex)
            {
                Console.WriteLine("Decryption Error: " + ex.Message);
                return string.Empty; // Or throw
            }
        }

        public static void RefreshRecords() // Refreshes a specific open form
        {
            try
            {
                // Find the form - safer check
                StockBalance obj = Application.OpenForms.OfType<StockBalance>().FirstOrDefault();
                if (obj != null && !obj.IsDisposed)
                {
                    // Assuming Getdata() repopulates the source and DataGridView binds to it
                    obj.Getdata(); // Call the form's method to refresh data
                                   // Direct refresh/update might not be needed if Getdata() handles binding updates
                                   // obj.dataGridView1.Refresh();
                                   // obj.dataGridView1.Update();
                }
                else
                {
                    Console.WriteLine("RefreshRecords: StockBalance form not found or disposed.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error refreshing Stock Balance: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void ExportExcel(DataGridView dgv) // Changed parameter type for clarity
        {
            if (dgv == null || dgv.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Export Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Excel.Application xlApp = null; // Initialize to null
            Excel.Workbook excelBook = null;
            Excel.Worksheet excelWorksheet = null;

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                xlApp = new Excel.Application();
                if (xlApp == null)
                {
                    MessageBox.Show("Excel is not properly installed.", "Excel Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                excelBook = xlApp.Workbooks.Add(System.Reflection.Missing.Value); // Use Missing.Value
                excelWorksheet = (Excel.Worksheet)excelBook.Worksheets[1];
                xlApp.Visible = true;

                // Write Headers - Consider only visible columns
                int colIndex = 1;
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    if (col.Visible)
                    {
                        excelWorksheet.Cells[1, colIndex].Value = col.HeaderText;
                        colIndex++;
                    }
                }

                // Write Data Rows - Consider only visible columns
                for (int i = 0; i < dgv.Rows.Count; i++) // Use Rows.Count directly
                {
                    if (dgv.Rows[i].IsNewRow) continue; // Skip the new row placeholder

                    colIndex = 1;
                    foreach (DataGridViewColumn col in dgv.Columns)
                    {
                        if (col.Visible)
                        {
                            // Handle potential null values gracefully
                            excelWorksheet.Cells[i + 2, colIndex].Value = dgv.Rows[i].Cells[col.Index].FormattedValue; // Use FormattedValue
                            colIndex++;
                        }
                    }
                }

                // Formatting
                Excel.Range headerRange = excelWorksheet.Range[excelWorksheet.Cells[1, 1], excelWorksheet.Cells[1, dgv.Columns.GetColumnCount(DataGridViewElementStates.Visible)]];
                headerRange.Font.Bold = true;
                headerRange.Font.Size = 12;
                headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray); // Optional background

                // AutoFit Columns
                excelWorksheet.Columns.AutoFit();

                // Select first cell
                excelWorksheet.Cells[1, 1].Select();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting to Excel: " + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                // Release COM objects - IMPORTANT!
                if (excelWorksheet != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorksheet);
                if (excelBook != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(excelBook);
                if (xlApp != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
                excelWorksheet = null;
                excelBook = null;
                xlApp = null;
                GC.Collect(); // Suggest garbage collection
                GC.WaitForPendingFinalizers();
            }
        }

        // Overload LedgerSave to keep the original signature if needed elsewhere
        public static void LedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g, string h)
        {
            // Call the version that takes total_sale, passing a default value (e.g., 0)
            LedgerSave(a, b, c, d, e, f, g, h, 0m);
        }

        // New version matching the one used in POS.cs
        public static void LedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g, string h, decimal totalSale)
        {
            try
            {
                string cb = @"INSERT INTO LedgerBook(Date, Name, LedgerNo, Label, Debit, Credit, PartyID, Manual_Inv, total_sale)
                           VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7, @d8, @d11)";
                var parameters = new[] {
                 DataAccessLayer.CreateParameter("@d1", SqlDbType.DateTime, a),
                 DataAccessLayer.CreateParameter("@d2", SqlDbType.NVarChar, b),
                 DataAccessLayer.CreateParameter("@d3", SqlDbType.NVarChar, c), // LedgerNo is likely NVarChar
                 DataAccessLayer.CreateParameter("@d4", SqlDbType.NVarChar, d),
                 DataAccessLayer.CreateParameter("@d5", SqlDbType.Decimal, e), // Debit
                 DataAccessLayer.CreateParameter("@d6", SqlDbType.Decimal, f), // Credit
                 DataAccessLayer.CreateParameter("@d7", SqlDbType.NVarChar, g), // PartyID is likely NVarChar
                 DataAccessLayer.CreateParameter("@d8", SqlDbType.NVarChar, h), // Manual_Inv
                 DataAccessLayer.CreateParameter("@d11", SqlDbType.Decimal, totalSale) // Added total_sale
             };
                DataAccessLayer.ExecuteNonQuery(cb, CommandType.Text, parameters);
            }
            catch (SqlException dbEx)
            {
                MessageBox.Show("Database Error saving ledger entry: " + dbEx.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("General Error saving ledger entry: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public static void LedgerDelete(string a, string b) // a = LedgerNo, b = Label
        {
            try
            {
                string cq = "DELETE FROM LedgerBook WHERE LedgerNo=@d1 AND Label=@d2";
                var parameters = new[] {
                 DataAccessLayer.CreateParameter("@d1", SqlDbType.NVarChar, a),
                 DataAccessLayer.CreateParameter("@d2", SqlDbType.NVarChar, b)
             };
                DataAccessLayer.ExecuteNonQuery(cq, CommandType.Text, parameters);
            }
            catch (SqlException dbEx)
            {
                MessageBox.Show("Database Error deleting ledger entry: " + dbEx.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("General Error deleting ledger entry: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Overload LedgerUpdate to keep original signature
        public static void LedgerUpdate(DateTime a, string b, decimal e, decimal f, string g, string h, string i)
        {
            // Call the version that takes total_sale, passing a default value (e.g., 0)
            LedgerUpdate(a, b, e, f, g, h, i, 0m);
        }


        // New version matching the one used in POS.cs
        public static void LedgerUpdate(DateTime a, string b, decimal e, decimal f, string g, string h, string i, decimal totalSale)
        {
            try
            {
                string cb = @"UPDATE LedgerBook SET Date=@d1, Name=@d2, Debit=@d3, Credit=@d4, PartyID=@d5, total_sale=@d8
                           WHERE LedgerNo=@d6 AND Label=@d7";
                var parameters = new[] {
                  DataAccessLayer.CreateParameter("@d1", SqlDbType.DateTime, a),
                  DataAccessLayer.CreateParameter("@d2", SqlDbType.NVarChar, b), // Name
                  DataAccessLayer.CreateParameter("@d3", SqlDbType.Decimal, e), // Debit
                  DataAccessLayer.CreateParameter("@d4", SqlDbType.Decimal, f), // Credit
                  DataAccessLayer.CreateParameter("@d5", SqlDbType.NVarChar, g), // PartyID
                  DataAccessLayer.CreateParameter("@d6", SqlDbType.NVarChar, h), // LedgerNo (for WHERE)
                  DataAccessLayer.CreateParameter("@d7", SqlDbType.NVarChar, i), // Label (for WHERE)
                  DataAccessLayer.CreateParameter("@d8", SqlDbType.Decimal, totalSale) // total_sale
             };
                DataAccessLayer.ExecuteNonQuery(cb, CommandType.Text, parameters);
            }
            catch (SqlException dbEx)
            {
                MessageBox.Show("Database Error updating ledger entry: " + dbEx.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("General Error updating ledger entry: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public static void SupplierLedgerSave(DateTime a, string b, string c, string d, decimal e, decimal f, string g)
        {
            try
            {
                string cb = @"INSERT INTO SupplierLedgerBook(Date, Name, LedgerNo, Label, Debit, Credit, PartyID)
                          VALUES (@d1, @d2, @d3, @d4, @d5, @d6, @d7)";
                var parameters = new[] {
                 DataAccessLayer.CreateParameter("@d1", SqlDbType.DateTime, a),
                 DataAccessLayer.CreateParameter("@d2", SqlDbType.NVarChar, b),
                 DataAccessLayer.CreateParameter("@d3", SqlDbType.NVarChar, c), // LedgerNo
                 DataAccessLayer.CreateParameter("@d4", SqlDbType.NVarChar, d), // Label
                 DataAccessLayer.CreateParameter("@d5", SqlDbType.Decimal, e), // Debit
                 DataAccessLayer.CreateParameter("@d6", SqlDbType.Decimal, f), // Credit
                 DataAccessLayer.CreateParameter("@d7", SqlDbType.NVarChar, g)  // PartyID (Supplier)
             };
                DataAccessLayer.ExecuteNonQuery(cb, CommandType.Text, parameters);
            }
            catch (SqlException dbEx)
            {
                MessageBox.Show("Database Error saving supplier ledger: " + dbEx.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("General Error saving supplier ledger: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SendMail(string s1_from, string s2_to, string s3_body, string s5_subject, string s6_smtpHost, int s7_smtpPort, string s8_username, string s9_password)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(s1_from) || string.IsNullOrWhiteSpace(s2_to) || string.IsNullOrWhiteSpace(s6_smtpHost) || s7_smtpPort <= 0)
                {
                    MessageBox.Show("Invalid email configuration provided.", "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var msg = new MailMessage
                {
                    From = new MailAddress(s1_from),
                    Subject = s5_subject,
                    Body = s3_body,
                    IsBodyHtml = true // Assume HTML body based on original code
                };
                // Add recipient(s) - Consider supporting multiple recipients separated by ';' or ','
                foreach (var emailAddress in s2_to.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!string.IsNullOrWhiteSpace(emailAddress))
                    {
                        msg.To.Add(emailAddress.Trim());
                    }
                }

                if (msg.To.Count == 0)
                {
                    MessageBox.Show("No valid recipient email address provided.", "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                var smt = new SmtpClient(s6_smtpHost)
                {
                    Port = s7_smtpPort,
                    // Use credentials only if username is provided
                    Credentials = !string.IsNullOrWhiteSpace(s8_username) ? new NetworkCredential(s8_username, s9_password) : null,
                    EnableSsl = true // Commonly required, make this configurable if needed
                };
                smt.Send(msg);
                MessageBox.Show("Email sent successfully!", "Email Info", MessageBoxButtons.OK, MessageBoxIcon.Information); // Optional success message
            }
            catch (SmtpException smtpEx) // Catch specific SMTP errors
            {
                MessageBox.Show("SMTP Error sending email: " + smtpEx.Message + "\nStatus Code: " + smtpEx.StatusCode, "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FormatException formatEx) // Catch errors parsing email addresses
            {
                MessageBox.Show("Invalid email address format: " + formatEx.Message, "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("General Error sending email: " + ex.Message, "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SupplierLedgerDelete(string a) // a = LedgerNo
        {
            try
            {
                string cq = "DELETE FROM SupplierLedgerBook WHERE LedgerNo=@d1";
                var parameter = DataAccessLayer.CreateParameter("@d1", SqlDbType.NVarChar, a);
                DataAccessLayer.ExecuteNonQuery(cq, CommandType.Text, parameter);
            }
            catch (SqlException dbEx)
            {
                MessageBox.Show("Database Error deleting supplier ledger: " + dbEx.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("General Error deleting supplier ledger: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SupplierLedgerUpdate(DateTime a, string b, decimal e, decimal f, string g, string h) // g = LedgerNo, h = Label
        {
            try
            {
                // Assuming Label identifies the *type* of entry, not the primary key with LedgerNo
                string cb = @"UPDATE SupplierLedgerBook SET Date=@d1, Name=@d2, Debit=@d3, Credit=@d4
                          WHERE LedgerNo=@d5 AND Label=@d6";
                var parameters = new[] {
                  DataAccessLayer.CreateParameter("@d1", SqlDbType.DateTime, a), // Date
                  DataAccessLayer.CreateParameter("@d2", SqlDbType.NVarChar, b), // Name
                  DataAccessLayer.CreateParameter("@d3", SqlDbType.Decimal, e), // Debit
                  DataAccessLayer.CreateParameter("@d4", SqlDbType.Decimal, f), // Credit
                  DataAccessLayer.CreateParameter("@d5", SqlDbType.NVarChar, g), // LedgerNo (for WHERE)
                  DataAccessLayer.CreateParameter("@d6", SqlDbType.NVarChar, h)  // Label (for WHERE)
             };
                DataAccessLayer.ExecuteNonQuery(cb, CommandType.Text, parameters);
            }
            catch (SqlException dbEx)
            {
                MessageBox.Show("Database Error updating supplier ledger: " + dbEx.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("General Error updating supplier ledger: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}