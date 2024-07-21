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
    public partial class basic : Form
    {
        public basic()
        {
            InitializeComponent();
        }

        private void gunaAdvenceButton1_Click(object sender, EventArgs e)
        {

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

        private void gunaAdvenceTileButton1_Click(object sender, EventArgs e)
        {
            if (!tabPage2.Created)
            {
                tabControl1.TabPages.Add(tabPage2);
                tabControl1.SelectedTab = tabPage2;
            }
            else
            {
                tabControl1.SelectedTab = tabPage2;
            }



        }


        private void basic_Load(object sender, EventArgs e)
        {
            tabControl1.TabPages.Remove(tabPage2);
            // Hide the second tab
            /* TabControl tabControl = Controls["tabControl1"] as TabControl;
             if (tabControl != null && tabControl.TabPages.Count > 1)
             {
                 TabPage hiddenTabPage = tabControl.TabPages[1];
                 tabControl.TabPages.Remove(hiddenTabPage);
             }
 */
        }

        private void toolStripMenuItem27_Click(object sender, EventArgs e)
        {
        }
    }
}
