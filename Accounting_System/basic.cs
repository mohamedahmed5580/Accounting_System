using Microsoft.VisualBasic;
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

namespace Accounting_System
{
    public partial class basic : Form
    {
        public static basic instance;
        public Notifications notifications = new Notifications();

        bool sideBar_Expand = true;
        bool Sales_Expand = false;
        bool Customer_Expand = false;
        bool Tables_Expand = false;
        bool Table_Expand = false;

        public basic()
        {
            InitializeComponent();
            instance = this;

        }

        private void gunaAdvenceButton1_Click(object sender, EventArgs e)
        {

        }
        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
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
            timer5.Start(); 
            notifications.Show();
            notifications.Hide();

        }

        private void toolStripMenuItem27_Click(object sender, EventArgs e)
        {
            
        }
        private void Home_Button_Click(object sender, EventArgs e)
        {
            MainMenu main = new MainMenu() { TopLevel = false, TopMost = true };
            panel1.Controls.Clear();

            main.FormBorderStyle = FormBorderStyle.None;
            panel1.Controls.Add(main);
            main.Show();
        }

        private void Orders_Button_Click(object sender, EventArgs e)
        {
            Stock stock = new Stock() { TopLevel = false, TopMost = true };
            panel1.Controls.Clear();
            stock.Width = 1614;
            stock.FormBorderStyle = FormBorderStyle.None;
            panel1.Controls.Add(stock);
            stock.Show();
        }


        private void btnStockAdjustment_Click(object sender, EventArgs e)
        {
            StockBalance stockBalance = new StockBalance();
            stockBalance.Show();
        }

        private void btnProductMaster_Click(object sender, EventArgs e)
        {

                Products p = new Products();
                p.Reset();
                p.Show();


        }


        private void btnBankReconciliation_Click(object sender, EventArgs e)
        {
            supplier_payment supplier_Payment = new supplier_payment();
            supplier_Payment.Show();
        }

        private void btnBarcodeLabelPrinting_Click(object sender, EventArgs e)
        {
            Barcode_printing barcode_Printing = new Barcode_printing(); 
            barcode_Printing.Show();    
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
            GeneralLedger generalLedger = new GeneralLedger();  
            generalLedger.Show();   
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
        }

        private void المشترياتToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PurchaseReport purchaseReport = new PurchaseReport();
            purchaseReport.Show();
            
        }

        private void الأرباحToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProfitAndLossReport profitAndLossReport = new ProfitAndLossReport();
            profitAndLossReport.Show();
        }

        private void ارصدةالزبائنالجميعToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DebtorsReport debtorsReport = new DebtorsReport();
            debtorsReport.Show();   
        }

        private void التقريرالعامToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GeneralLedger generalLedger = new GeneralLedger();
            generalLedger.Show();
        }

        private void كشفحسابمندوبToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SalesmanLedger salesman = new SalesmanLedger();
            salesman.Show();
        }

        private void حالةالمخزونToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StockInAndOutReport st = new StockInAndOutReport();
            st.Show();
        }

        private void المصروفاتToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VoucherReport voucherReport = new VoucherReport();  
            voucherReport.Show();
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {

        }

        private void يوميةالمشترياتToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PurshaseDaybook purshaseDaybook = new PurshaseDaybook();
            purshaseDaybook.Show(); 
        }

        private void كشفحسابتاجرToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SupplierLedger supplierLedger = new SupplierLedger();
            supplierLedger.Show();
        }

        private void كشفحسابعميلToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomerLedger customerLedger = new CustomerLedger();
            customerLedger.Show();
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
            product.Reset();
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

  
        private void btnPayroll_Click(object sender, EventArgs e)
        {
            Payment_2 frmPayment_2 = new Payment_2();
            frmPayment_2.Reset();
            frmPayment_2.ShowDialog();

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
            //POS POSS = new POS();

            //POSS.lblSet.Text = "POS Entry";
            //POSS.Show();
            POS pos = new POS() { TopLevel = false, TopMost = true };
            basic.instance.panel1.Controls.Clear();

            pos.lblSet.Text = "POS Entry";
            pos.FormBorderStyle = FormBorderStyle.None;
            basic.instance.panel1.Controls.Add(pos);
            pos.Show();
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

        private void gunaPanel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void gunaButton6_Click(object sender, EventArgs e)
        {
            timer2.Start();
        }

        private void Timer_Sidebar_Menu_Tick(object sender, EventArgs e)
        {
            if (sideBar_Expand)
            {
                SideBar.Width -= 300;
                if (SideBar.Width == SideBar.MinimumSize.Width)
                {
                    sideBar_Expand = false;
                    Timer_Sidebar_Menu.Stop();
                    pictureBox1.Visible = false;

                    /*    Orders_Button.Dock = DockStyle.Right;
                        gunaButton1.Dock = DockStyle.Right;
                        gunaButton2.Dock = DockStyle.Right;
                        gunaButton3.Dock = DockStyle.Right;
                        gunaButton4.Dock = DockStyle.Right;
                        gunaButton5.Dock = DockStyle.Right;
                        gunaButton6.Dock = DockStyle.Right;
                        gunaButton7.Dock = DockStyle.Right;
                        gunaButton9.Dock = DockStyle.Right;
                        gunaButton10.Dock = DockStyle.Right;
                        gunaButton11.Dock = DockStyle.Right;*/
                }
            }
            else
            {
                SideBar.Width += 215;
                if (SideBar.Width == SideBar.MaximumSize.Width)
                {
                    sideBar_Expand = true;
                    Timer_Sidebar_Menu.Stop();
                    pictureBox1.Visible = true;
                    /* Orders_Button.Dock = DockStyle.None;
                     gunaButton1.Dock = DockStyle.None;
                     gunaButton2.Dock = DockStyle.None;
                     gunaButton3.Dock = DockStyle.None;
                     gunaButton4.Dock = DockStyle.None;
                     gunaButton5.Dock = DockStyle.None;
                     gunaButton6.Dock = DockStyle.None;
                     gunaButton7.Dock = DockStyle.None;
                     gunaButton9.Dock = DockStyle.None;
                     gunaButton10.Dock = DockStyle.None;
                     gunaButton11.Dock = DockStyle.None;*/
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Sales_Expand)
            {
                gunaPanel11.Height -= 105;
                Sales_Expand = false;
                timer1.Stop();

                
            }
            else
            {
                gunaPanel11.Height += 105;
                Sales_Expand = true;
                timer1.Stop();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (Customer_Expand)
            {
                gunaPanel10.Height -= 105;
                Customer_Expand = false;
                timer2.Stop();
            }
            else
            {
                gunaPanel10.Height += 105;
                Customer_Expand = true;
                timer2.Stop();
            }
        }

        private void Menu_Button_Click(object sender, EventArgs e)
        {
            Timer_Sidebar_Menu.Start();

        }

        private void gunaButton5_Click(object sender, EventArgs e)
        {
            timer1.Start();

        }

        private void gunaButton9_Click(object sender, EventArgs e)
        {
            timer3.Start();

        }

        private void gunaButton18_Click(object sender, EventArgs e)
        {

        }

        private void gunaButton8_Click(object sender, EventArgs e)
        {
            timer4.Start();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (Tables_Expand)
            {
                gunaPanel6.Height -= 400;
                Tables_Expand = false;
                timer3.Stop();
            }
            else
            {
                gunaPanel6.Height += 300;
                Tables_Expand = true;
                timer3.Stop();
            }
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


        private void gunaButton16_Click(object sender, EventArgs e)
        {

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            if (Table_Expand)
            {
                gunaPanel7.Height -= 405;
                Table_Expand = false;
                timer4.Stop();
            }
            else
            {
                gunaPanel7.Height += 405;
                Table_Expand = true;
                timer4.Stop();
            }
        }

        private void gunaButton1_Click(object sender, EventArgs e)
        {
            Quotation frmQuotation = new Quotation();
            frmQuotation.Reset();
            frmQuotation.Show();

        }

        private void gunaLinePanel1_Paint(object sender, PaintEventArgs e)
        {
            ShowLogs();
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

        private void نسخToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void العملاتToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Currencies currencies = new Currencies();
            currencies.Show();
        }

        private void gunaButton2_Click(object sender, EventArgs e)
        {
            Voucher voucher = new Voucher();
            voucher.Reset();
            voucher.Show();
        }

        private void gunaButton3_Click(object sender, EventArgs e)
        {
            Supplier sup = new Supplier();
            sup.auto();
            sup.Show();
        }

        private void gunaButton7_Click(object sender, EventArgs e)
        {
            Payment_2 frmPayment_2 = new Payment_2();
            frmPayment_2.Reset();
            frmPayment_2.ShowDialog();
        }

        private void SideBar_Paint(object sender, PaintEventArgs e)
        {

        }

        private void gunaButton12_Click(object sender, EventArgs e)
        {
            POS pos = new POS() { TopLevel = false, TopMost = true };
            panel1.Controls.Clear();

            pos.lblSet.Text = "POS Entry";
            pos.FormBorderStyle = FormBorderStyle.None;
            panel1.Controls.Add(pos);
            pos.Show();
        }

        private void gunaButton13_Click(object sender, EventArgs e)
        {
            SalesReturn frmPurchaseReturn = new SalesReturn();

            frmPurchaseReturn.lblUser.Text = lblUser.Text;
            frmPurchaseReturn.lblUserType.Text = lblUserType.Text;
            frmPurchaseReturn.Reset();
            frmPurchaseReturn.Show();

        }

        private void gunaButton15_Click(object sender, EventArgs e)
        {
            AddCustomer customer = new AddCustomer();
            customer.Reset();
            customer.Show();
        }

        private void gunaButton14_Click(object sender, EventArgs e)
        {
            CustomerList c = new CustomerList();
            c.Reset();
            c.Show();
        }

        private void gunaButton4_Click(object sender, EventArgs e)
        {
            supplier_payment supplier_Payment = new supplier_payment();
            supplier_Payment.Show();

        }

        private void gunaButton10_Click(object sender, EventArgs e)
        {

            Logs logs = new Logs();
            logs.Show();

        }

        private void gunaButton11_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            var dt = DateTime.Today;
            lblDateTime.Text = dt.ToString("dd/MM/yyyy");
            lblTime.Text = DateAndTime.TimeOfDay.ToString("h:mm:ss tt");
            ShowLogs();
            //Notifications.instance.notificationTimer.Start();
        }

        private void lblDateTime_Click(object sender, EventArgs e)
        {

        }

        private void lblTime_Click(object sender, EventArgs e)
        {

        }

        private void Notification_Click(object sender, EventArgs e)
        {

        }

        private void gunaLinePanel1_DoubleClick(object sender, EventArgs e)
        {
        }
    }
}