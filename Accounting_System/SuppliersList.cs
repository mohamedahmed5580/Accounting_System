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
    public partial class SuppliersList : Form
    {
        SqlConnection cn = new SqlConnection(DataAccessLayer.Con());

        public SuppliersList()
        {
            InitializeComponent();
 
            Getdata();
            this.dgw.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dgw_MouseClick);
            this.txtSupplierName.TextChanged += new System.EventHandler(this.txtSupplierName_TextChanged);
            this.txtCity.TextChanged += new System.EventHandler(this.txtCity_TextChanged);
            this.txtContactNo.TextChanged += new System.EventHandler(this.txtContactNo_TextChanged);
        }
        public void Getdata()
        {
            try
            {
               
                
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT RTRIM(ID), RTRIM(SupplierID), RTRIM([Name]), RTRIM(Address), RTRIM(City), RTRIM(State), RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID), RTRIM(TIN), RTRIM(STNo), RTRIM(CST), RTRIM(PAN), RTRIM(AccountName), RTRIM(AccountNumber), RTRIM(Bank), RTRIM(Branch), RTRIM(IFSCCode), OpeningBalance, RTRIM(OpeningBalanceType), RTRIM(Remarks) FROM Supplier ORDER BY [Name]", cn))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dgw.Rows.Clear();
                            while (rdr.Read())
                            {
                                dgw.Rows.Add(rdr[0].ToString(), rdr[1].ToString(), rdr[2].ToString(), rdr[3].ToString(), rdr[4].ToString(), rdr[5].ToString(), rdr[6].ToString(), rdr[7].ToString(), rdr[8].ToString(), rdr[9].ToString(), rdr[10].ToString(), rdr[11].ToString(), rdr[12].ToString(), rdr[13].ToString(), rdr[14].ToString(), rdr[15].ToString(), rdr[16].ToString(), rdr[17].ToString(), rdr[18].ToString(), rdr[19].ToString(), rdr[20].ToString());
                            }
                        }
                    }
                    cn.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dgw_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                if (dgw.Rows.Count > 0)
                {
                    DataGridViewRow dr = dgw.SelectedRows[0];
                    supplier_payment frmPayment =  supplier_payment.instance;
                    frmPayment.txtSup_ID.Text = dr.Cells[0].Value.ToString();
                    frmPayment.txtSupplierID.Text = dr.Cells[1].Value.ToString();
                    frmPayment.txtSupplierName.Text = dr.Cells[2].Value.ToString();
                    frmPayment.txtAddress.Text = dr.Cells[3].Value.ToString();
                    frmPayment.txtCity.Text = dr.Cells[4].Value.ToString();
                    frmPayment.txtContactNo.Text = dr.Cells[7].Value.ToString();
                    frmPayment.GetSupplierBalance();
                    lblSet.Text = "";
                    this.Close();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void Reset()
        {
            txtSupplierName.Text = "";
            txtContactNo.Text = "";
            txtCity.Text = "";
            Getdata();
        }

        private void txtSupplierName_TextChanged(object sender, EventArgs e)
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT RTRIM(ID),RTRIM(SupplierID),RTRIM([Name]), RTRIM(Address),RTRIM(City),RTRIM(State),RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID),RTRIM(TIN),RTRIM(STNo),RTRIM(CST),RTRIM(PAN),RTRIM(AccountName),RTRIM(AccountNumber),RTRIM(Bank),RTRIM(Branch),RTRIM(IFSCCode),OpeningBalance,RTRIM(OpeningBalanceType),RTRIM(Remarks) from Supplier where name like @name order by name", cn))
            {
                cmd.Parameters.AddWithValue("@name", "%" + txtSupplierName.Text + "%");

                using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    dgw.Rows.Clear();
                    while (rdr.Read())
                    {
                        dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], rdr[16], rdr[17], rdr[18], rdr[19], rdr[20]);
                    }
                }
            }
        }
        private void txtCity_TextChanged(object sender, EventArgs e)
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT RTRIM(ID),RTRIM(SupplierID),RTRIM([Name]), RTRIM(Address),RTRIM(City),RTRIM(State),RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID),RTRIM(TIN),RTRIM(STNo),RTRIM(CST),RTRIM(PAN),RTRIM(AccountName),RTRIM(AccountNumber),RTRIM(Bank),RTRIM(Branch),RTRIM(IFSCCode),OpeningBalance,RTRIM(OpeningBalanceType),RTRIM(Remarks) from Supplier where City like @city order by city", cn))
            {
                cmd.Parameters.AddWithValue("@city", "%" + txtCity.Text + "%");

                using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    dgw.Rows.Clear();
                    while (rdr.Read())
                    {
                        dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], rdr[16], rdr[17], rdr[18], rdr[19], rdr[20]);
                    }
                }
            }
        }

        private void txtContactNo_TextChanged(object sender, EventArgs e)
        {
            cn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT RTRIM(ID),RTRIM(SupplierID),RTRIM([Name]), RTRIM(Address),RTRIM(City),RTRIM(State),RTRIM(ZipCode), RTRIM(ContactNo), RTRIM(EmailID),RTRIM(TIN),RTRIM(STNo),RTRIM(CST),RTRIM(PAN),RTRIM(AccountName),RTRIM(AccountNumber),RTRIM(Bank),RTRIM(Branch),RTRIM(IFSCCode),OpeningBalance,RTRIM(OpeningBalanceType),RTRIM(Remarks) from Supplier where ContactNo like @contactNo order by city", cn))
            {
                cmd.Parameters.AddWithValue("@contactNo", "%" + txtContactNo.Text + "%");

                using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    dgw.Rows.Clear();
                    while (rdr.Read())
                    {
                        dgw.Rows.Add(rdr[0], rdr[1], rdr[2], rdr[3], rdr[4], rdr[5], rdr[6], rdr[7], rdr[8], rdr[9], rdr[10], rdr[11], rdr[12], rdr[13], rdr[14], rdr[15], rdr[16], rdr[17], rdr[18], rdr[19], rdr[20]);
                    }
                }
            }
        }




        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dgw_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void SuppliersList_Load(object sender, EventArgs e)
        {

        }
    }
}
