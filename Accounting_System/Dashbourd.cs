using Microsoft.VisualBasic;

using Pharmacy.PL;
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
    public partial class Dashbourd : Form
    {
        public Notifications notifications = new Notifications();
        public POS POSS = new POS();
        public static TimeZoneInfo egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
        public static DateTime egyptTime = TimeZoneInfo.ConvertTime(DateTime.Now, egyptTimeZone);
        public Dashbourd()
        {
            InitializeComponent();
            this.FormClosed += new FormClosedEventHandler(frmMain_FormClosed);
        }

        private void gunaAdvenceButton1_Click(object sender, EventArgs e)
        {

        }
        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            notifications.Close();
            Application.Exit();
        }

        private void gunaGradientPanel1_Click(object sender, EventArgs e)
        {

        }

        private void gunaImageButton9_Click(object sender, EventArgs e)
        {

        }

        private void gunaImageButton5_Click(object sender, EventArgs e)
        {

        }

        private void gunaImageButton6_Click(object sender, EventArgs e)
        {

        }

        private void gunaImageButton7_Click(object sender, EventArgs e)
        {

        }

        private void gunaPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void gunaAdvenceButton28_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void menuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }




        private void basic_Load(object sender, EventArgs e)
        {
            try
            {
                timer5.Start();

            }
            catch
            {
                timer5.Stop();

                return;
            }

        }

        private void toolStripMenuItem27_Click(object sender, EventArgs e)
        {

        }

        private void btnStockAdjustment_Click(object sender, EventArgs e)
        {
            Warehouses stockBalance = new Warehouses();
            stockBalance.Show();
        }

        private void btnProductMaster_Click(object sender, EventArgs e)
        {

            Products p = new Products();
            p.Show();
        }

        private void btnBankReconciliation_Click(object sender, EventArgs e)
        {

        }

        private void btnBarcodeLabelPrinting_Click(object sender, EventArgs e)
        {
            Barcode_printing barcode_Printing = new Barcode_printing();
            barcode_Printing.Show();
        }



        private void btnPayment_Click(object sender, EventArgs e)
        {

        }

        private void btnStockTransfer_Issue_Click(object sender, EventArgs e)
        {

        }

        private void btnAccountingReports_Click(object sender, EventArgs e)
        {


        }

        private void btnPOSReport_Click(object sender, EventArgs e)
        {

        }

        private void BtnVoucher_Click(object sender, EventArgs e)
        {
            Voucher voucher = new Voucher();
            voucher.Reset();
            voucher.Show();
        }

        private void btnPOSReport_Click_1(object sender, EventArgs e)
        {
            SalesReport salesReport = new SalesReport();
            salesReport.Show();
        }

        private void btnPurchaseOrder_Click(object sender, EventArgs e)
        {
            PurchaseReport purchaseReport = new PurchaseReport();
            purchaseReport.Show();
        }

        private void btnPayment_Click_1(object sender, EventArgs e)
        {
            SalesmanLedger salesmanLedger = new SalesmanLedger();
            salesmanLedger.Show();
        }

        private void btnBankReconciliation_Click_1(object sender, EventArgs e)
        {
            supplier_payment supplier_Payment = new supplier_payment();
            supplier_Payment.Show();
        }

        private void btnStockTransfer_Issue_Click_1(object sender, EventArgs e)
        {
            Stock stock = new Stock();
            stock.Show();
        }

        private void btnAccountingReports_Click_1(object sender, EventArgs e)
        {
            GeneralDayBook generalDayBook = new GeneralDayBook();
            generalDayBook.Show();
        }

        private void btnWorkPeriod_Click(object sender, EventArgs e)
        {
            ProfitAndLossReport profitAndLossReport = new ProfitAndLossReport();
            profitAndLossReport.Show();
        }

        private void btnPOSRecord_Click(object sender, EventArgs e)
        {
            DebtorsReport debtorsReport = new DebtorsReport();
            debtorsReport.Show();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem32_Click(object sender, EventArgs e)
        {
            services services = new services();
            services.Show();
        }

        private void toolStripMenuItem20_Click(object sender, EventArgs e)
        {
            POS POSS = new POS();
            POSS.lblSet.Text = "POS Entry";
            POSS.Show();
        }

        private void toolStripMenuItem18_Click(object sender, EventArgs e)
        {
            AddCustomer customer = new AddCustomer();
            customer.Reset();
            customer.Show();
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {

        }

        private void PurchaseEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void PurchaseReturnToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem25_Click(object sender, EventArgs e)
        {
            Quotation frmQuotation = new Quotation();
            frmQuotation.Reset();
            frmQuotation.Show();

        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {

        }

        private void قائمةعروضالاسعارToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuotationRecord1 frmQuotationRecord1 = new QuotationRecord1();
            frmQuotationRecord1.Reset();
            frmQuotationRecord1.ShowDialog();
        }

        private void مبيعاتكلصنفToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProductsLedger frmProductsLedger = new ProductsLedger();
            frmProductsLedger.Show();
        }

        private void احصائياتعامةToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stat stat = new Stat();
            stat.Show();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Logs logs = new Logs();
            logs.Show();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            SalesReport salesReport = new SalesReport();
            salesReport.Show();
            timer5.Stop();

        }

        private void المشترياتToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PurchaseReport purchaseReport = new PurchaseReport();
            purchaseReport.Show();
            timer5.Stop();

        }

        private void الأرباحToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProfitAndLossReport profitAndLossReport = new ProfitAndLossReport();
            profitAndLossReport.Show();
            timer5.Stop();

        }

        private void ارصدةالزبائنالجميعToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DebtorsReport debtorsReport = new DebtorsReport();
            debtorsReport.Show();
            timer5.Stop();

        }

        private void التقريرالعامToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GeneralLedger generalLedger = new GeneralLedger();
            generalLedger.Show();
            timer5.Stop();

        }

        private void كشفحسابمندوبToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SalesmanLedger salesman = new SalesmanLedger();
            salesman.Show();
            timer5.Stop();

        }

        private void حالةالمخزونToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StockInAndOutReport st = new StockInAndOutReport();
            st.Show();
            timer5.Stop();

        }

        private void المصروفاتToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VoucherReport voucherReport = new VoucherReport();
            voucherReport.Show();
            timer5.Stop();

        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {

        }

        private void يوميةالمشترياتToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PurshaseDaybook purshaseDaybook = new PurshaseDaybook();
            purshaseDaybook.Show();
            timer5.Stop();

        }

        private void كشفحسابتاجرToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SupplierLedger supplierLedger = new SupplierLedger();
            supplierLedger.Show();
            timer5.Stop();

        }

        private void كشفحسابعميلToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomerLedger customerLedger = new CustomerLedger();
            customerLedger.Show();
            timer5.Stop();
        }

        private void كشفمبيعاتToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Venduer venduer = new Venduer();
            venduer.Show();
        }

        private void التقريرالعامToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void جهاتالاتصالToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Company company = new Company();
            company.Show();
        }

        private void الشركىToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OriginalCompany originalCompany = new OriginalCompany();
            originalCompany.Show();
        }

        private void عنالشركهToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void المبرمجينToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Programers programers = new Programers();
            programers.Show();
        }

        private void الالهالحاسبةToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("C:\\Windows\\System32\\calc.exe");
        }

        private void btnPayroll_Click(object sender, EventArgs e)
        {
            Payment_2 frmPayment_2 = new Payment_2();
            frmPayment_2.Reset();
            frmPayment_2.Show();

        }

        private void btnCreditCustomer_Click(object sender, EventArgs e)
        {
            AddCustomer customer = new AddCustomer();
            customer.Reset();
            customer.Show();
        }

        private void btnSupplier_Click(object sender, EventArgs e)
        {
            Supplier sup = new Supplier();
            sup.auto();
            sup.Show();
        }

        private void btnSalesmanMaster_Click(object sender, EventArgs e)
        {
            SalesMan saleman = new SalesMan();
            saleman.Show();
        }

        private void btnWallet_Click(object sender, EventArgs e)
        {
            POSS.lblSet.Text = "POS Entry";
            if (POSS.IsDisposed) // Check if notifn is null or disposed
            {
                POSS = new POS(); // Create a new instance
                POSS.Show();
            }
            else
            {
                POSS.Show();
            }
        }


        private void btnPurchase_Click(object sender, EventArgs e)
        {
            Pymentinvoice frmPurchaseEntry = new Pymentinvoice();
            frmPurchaseEntry.lblUser.Text = lblUser.Text;
            frmPurchaseEntry.lblUserType.Text = lblUserType.Text;
            frmPurchaseEntry.Reset();
            frmPurchaseEntry.Show();
        }

        private void btnSalesReturn_Click(object sender, EventArgs e)
        {
            SalesReturn frmPurchaseReturn = new SalesReturn();

            frmPurchaseReturn.lblUser.Text = lblUser.Text;
            frmPurchaseReturn.lblUserType.Text = lblUserType.Text;
            frmPurchaseReturn.Reset();
            frmPurchaseReturn.Show();

        }

        private void btnPurchaseReturn_Click(object sender, EventArgs e)
        {
            PurchaseReturn frmPurchaseReturn = new PurchaseReturn();
            frmPurchaseReturn.lblUser.Text = lblUser.Text;
            frmPurchaseReturn.lblUserType.Text = lblUserType.Text;
            frmPurchaseReturn.Reset();
            frmPurchaseReturn.Show();
        }

        private void toolStripMenuItem29_Click(object sender, EventArgs e)
        {
            Voucher voucher = new Voucher();
            voucher.Reset();
            voucher.Show();
        }

        private void toolStripMenuItem23_Click(object sender, EventArgs e)
        {
            Supplier sup = new Supplier();
            sup.auto();
            sup.Show();
        }

        private void toolStripMenuItem21_Click(object sender, EventArgs e)
        {
            supplier_payment supplier_Payment = new supplier_payment();
            supplier_Payment.Show();
        }

        private void مرتجعمبيعاتToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SalesReturn frmPurchaseReturn = new SalesReturn();

            frmPurchaseReturn.lblUser.Text = lblUser.Text;
            frmPurchaseReturn.lblUserType.Text = lblUserType.Text;
            frmPurchaseReturn.Reset();
            frmPurchaseReturn.Show();
        }

        private void قائمةالعملاءToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomerList c = new CustomerList();
            c.Reset();
            c.Show();
        }

        private void استيردظتصديرToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem15_Click(object sender, EventArgs e)
        {
            Payment_2 frmPayment_2 = new Payment_2();
            frmPayment_2.Reset();
            frmPayment_2.ShowDialog();
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            CustomerList se = new CustomerList();
            se.Reset();
            se.Show();
        }

        private void قائمةمناديبالمبيعاتToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SalesmanRecord saleman = new SalesmanRecord();
            saleman.Show();
        }

        private void قائمةالاصنافToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProductsScreen product = new ProductsScreen();
            product.Show();
        }

        private void قائمةالموردينToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SuppliersList suppliers = new SuppliersList();
            suppliers.Reset();
            suppliers.Show();
        }

        private void قائمةدفعاتالموردينToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PaymentRecord payment = new PaymentRecord();
            payment.Show();
        }

        private void قائمةفواتيرالشراءToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PymentinvoiceScreen screen = new PymentinvoiceScreen();
            screen.Reset();
            screen.Show();
        }

        private void قائمةفواتيرالبيعToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SalesInvoiceScreen frmSalesInvoiceRecord = new SalesInvoiceScreen();
            frmSalesInvoiceRecord.lblSet.Text = "1";
            frmSalesInvoiceRecord.Reset();
            frmSalesInvoiceRecord.Show();
            frmSalesInvoiceRecord.Focus();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Close();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }

        private void toolStripMenuItem33_Click(object sender, EventArgs e)
        {
            Stock stock = new Stock();
            stock.Show();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void المستخدمينToolStripMenuItem_Click(object sender, EventArgs e)
        {
            users users = new users();
            users.Show();
        }

        private void دفتراليوميةToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GeneralDayBook generalDayBook = new GeneralDayBook();
            generalDayBook.Show();
        }

        private void الاشعاراتToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (notifications.IsDisposed) // Check if notifn is null or disposed
            {
                notifications = new Notifications(); // Create a new instance
                notifications.ShowDialog();
            }
            else
            {
                notifications.Show();
            }
        }

        private void نسخاحطياتيToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Backup();
        }
        public void Backup()
        {
            try
            {
                DateTime dt = DateTime.Today;
                string destdir = Accounting_System.Properties.Settings.Default.Database + DateTime.Now.ToString(" dd-MM-yyyy") + ".bak";

                SaveFileDialog objdlg = new SaveFileDialog();
                objdlg.FileName = destdir;
                objdlg.ShowDialog();
                string Filename = objdlg.FileName;


                using (SqlConnection con = new SqlConnection(DataAccessLayer.Con())) // Ensure your connection string is correct
                {
                    con.Open();
                    string cb = "backup database [" + Accounting_System.Properties.Settings.Default.Database + "] to disk='" + Filename + "' with init, stats=10";
                    using (SqlCommand cmd = new SqlCommand(cb, con))
                    {
                        cmd.ExecuteReader();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void نسخToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string backupPath = "";

            // استخدام OpenFileDialog لاختيار ملف النسخة الاحتياطية
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Backup Files (*.bak)|*.bak";
                openFileDialog.Title = "اختر ملف النسخة الاحتياطية";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    backupPath = openFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("لم يتم تحديد ملف النسخة الاحتياطية.");
                    return;
                }
            }

            // أمر الاستعادة
            string restoreQuery = $@"
            RESTORE DATABASE [sa]
            FROM DISK = '{backupPath}'
            WITH REPLACE;
        ";

            try
            {
                // الاتصال وتنفيذ أمر الاستعادة
                using (SqlConnection connection = new SqlConnection(DataAccessLayer.Con()))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(restoreQuery, connection))
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show("تم استعادة النسخة الاحتياطية بنجاح!", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء استعادة النسخة الاحتياطية: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    
        public static void LogFunc(string st1, string st2)
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                string cb = "INSERT INTO Logs(UserID, Date, Operation) VALUES (@d1, @d2, @d3)";
                using (var cmd = new SqlCommand(cb, con))
                {
                    cmd.Parameters.AddWithValue("@d1", st1);
                    cmd.Parameters.AddWithValue("@d2", DateTime.Now);
                    cmd.Parameters.AddWithValue("@d3", st2);
                    cmd.ExecuteReader();
                }
            }
        }

        private void وتسابToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WSender wSender = new WSender();    
            wSender.Show();
        }

        

        private void النقلبينالمخازنToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Warehouses_transportation warehouses_Transportation = new Warehouses_transportation();
            warehouses_Transportation.Show();
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
/*            var dt = DateTime.Today;
            lblDateTime.Text = dt.ToString("dd/MM/yyyy");
            lblTime.Text = DateAndTime.TimeOfDay.ToString("h:mm:ss tt");
            ShowLogs();*/
        }
        public void ShowLogs()
        {
            using (SqlConnection con = new SqlConnection(DataAccessLayer.Con()))
            {
                con.Open();
                // Query to get the most recent log entry
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 RTRIM(Operation) FROM Logs ORDER BY Date DESC", con))
                {
                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (rdr.Read()) // Move to the first record
                        {
                            Notification.Text = rdr[0].ToString(); // Access the first column value
                        }
                        else
                        {
                            Notification.Text = "No logs found"; // Handle the case where there are no records
                        }
                    }
                }
            }

        }


        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void basic_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Show a confirmation dialog when attempting to close the program
            DialogResult result = MessageBox.Show("هل انت متاكد من الخروج من البرنامج", "تاكيد الخروج", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // If the user chooses 'No', cancel the close event
            if (result == DialogResult.No)
            {
                e.Cancel = true; // This cancels the close
            }
        }

        private void اصنافالمبيعاتToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SalesProductsReport salesProductsReport = new SalesProductsReport();

            salesProductsReport.Show();
        }

        private void اصنافالمشترياتToolStripMenuItem_Click(object sender, EventArgs e)
        {

            PaymentProductReport salesProductsReport = new PaymentProductReport();
            salesProductsReport.Show();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            CustomerInvReport customerInvReport = new CustomerInvReport();  
            customerInvReport.Show();   
        }

        private void اصنافمبيعاتكلعميلToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomerInvProductReport customer = new CustomerInvProductReport();
            customer.Show();

        }

        private void الارصدةالعامةToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomerSuppReport customer = new CustomerSuppReport();
            customer.Show();    
        }

       
        private void قائمةالرسائلToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem3_Click_1(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem3_Click_2(object sender, EventArgs e)
        {
            Currencies currencies = new Currencies();
            currencies.Show();
        }

        private void شركاتالشحنToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShippingCom com = new ShippingCom();
            com.lblUser.Text = "user";
            com.Show();
        }

        private void دفعاتشركاتالشحنToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShippingCom_pyment shippingCom_Pyment = new ShippingCom_pyment();
            shippingCom_Pyment.Show();
        }

        private void اعداداتToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show_database show_Database = new Show_database();
            show_Database.ShowDialog();
        }

        private void كشفحسابشركةشحنToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SupplierCompanyLedger supplierCompanyLedger = new SupplierCompanyLedger();
            supplierCompanyLedger.Show();
        }

        private void ارصدةالمخزنToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SalesPaymentReport salesPaymentReport = new SalesPaymentReport();
            salesPaymentReport.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ShowNotification("","");
        }

        private void ShowNotification(string title, string message)
        {
            // إذا لم يُمرّر أي نص للإشعار، استخدم عنوان الإشعار كرسالة
            if (string.IsNullOrWhiteSpace(message))
            {
                message = "(لا توجد تفاصيل)";
            }

            notifyIcon1.BalloonTipTitle = title;
            notifyIcon1.BalloonTipText = message;
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.ShowBalloonTip(5000);
        }

    }
}