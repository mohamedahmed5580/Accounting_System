using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Accounting_System
{
    public partial class OriginalCompany : Form
    {
        public OriginalCompany()
        {
            InitializeComponent();
            linkLabel1.Links.Add(0, linkLabel1.Text.Length, "https://WWW.URTECH-EGY.com");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string target = e.Link.LinkData as string;

            if (!string.IsNullOrEmpty(target))
            {
                try
                {
                    Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to open link: " + ex.Message);
                }
            }
        }
    }
}
