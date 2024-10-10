using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Accounting_System
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void btnStockAdjustment_Click(object sender, EventArgs e)
        {
            StockBalance stockBalance = new StockBalance();
            stockBalance.Show();
        }
        private void btnBarcodeLabelPrinting_Click(object sender, EventArgs e)
        {
            Barcode_printing barcode_Printing = new Barcode_printing();
            barcode_Printing.Show();
        }
        private void BtnVoucher_Click(object sender, EventArgs e)
        {
            Voucher voucher = new Voucher();
            voucher.Reset();
            voucher.Show();
        }
        private void btnPOSReport_Click(object sender, EventArgs e)
        {
            SalesReport salesReport = new SalesReport();
            salesReport.Show();
        }
        private void btnPurchaseOrder_Click(object sender, EventArgs e)
        {
            PurchaseReport purchaseReport = new PurchaseReport();
            purchaseReport.Show();
        }

        private void btnPayment_Click(object sender, EventArgs e)
        {
            SalesmanLedger salesmanLedger = new SalesmanLedger();
            salesmanLedger.Show();
        }
        private void btnBankReconciliation_Click(object sender, EventArgs e)
        {
            supplier_payment supplier_Payment = new supplier_payment();
            supplier_Payment.Show();
        }
        private void btnStockTransfer_Issue_Click(object sender, EventArgs e)
        {
            Stock stock = new Stock();
            stock.Show();
        }
        private void btnAccountingReports_Click(object sender, EventArgs e)
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
            /*            POS POSS = new POS();

                        POSS.lblSet.Text = "POS Entry";
                        POSS.Show();*/

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
        private void button7_Click(object sender, EventArgs e)
        {
            Stock stock = new Stock() { TopLevel = false, TopMost = true };
            stock.FormBorderStyle = FormBorderStyle.None;
            gunaPanel1.Controls.Add(stock);
            stock.Show();
        }
        private void btnProductMaster_Click(object sender, EventArgs e)
        {
            Products p = new Products();
            p.Reset();
            p.Show();
        }
    }
}
