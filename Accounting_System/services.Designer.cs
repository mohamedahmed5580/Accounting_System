namespace Accounting_System
{
    partial class services
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label Label5;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(services));
            this.Panel3 = new System.Windows.Forms.Panel();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnGetData = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.Label8 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.txtRemarks = new System.Windows.Forms.TextBox();
            this.btnSelect = new System.Windows.Forms.Button();
            this.Label3 = new System.Windows.Forms.Label();
            this.txtCustomerID = new System.Windows.Forms.TextBox();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.txtProblemDescription = new System.Windows.Forms.TextBox();
            this.dtpEstimatedRepairDate = new System.Windows.Forms.DateTimePicker();
            this.txtContactNo = new System.Windows.Forms.TextBox();
            this.txtID = new System.Windows.Forms.TextBox();
            this.txtCID = new System.Windows.Forms.TextBox();
            this.txtItemsDescription = new System.Windows.Forms.TextBox();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.txtUpfront = new System.Windows.Forms.TextBox();
            this.txtChargesQuote = new System.Windows.Forms.TextBox();
            this.cmbServiceType = new System.Windows.Forms.ComboBox();
            this.Label13 = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.Label12 = new System.Windows.Forms.Label();
            this.lblUserType = new System.Windows.Forms.Label();
            this.lblUser = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.Panel2 = new System.Windows.Forms.Panel();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.Label11 = new System.Windows.Forms.Label();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.GroupBox4 = new System.Windows.Forms.GroupBox();
            this.Label10 = new System.Windows.Forms.Label();
            this.Label9 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.dtpServiceCreationDate = new System.Windows.Forms.DateTimePicker();
            this.txtServiceCode = new System.Windows.Forms.TextBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.Timer1 = new System.Windows.Forms.Timer(this.components);
            Label5 = new System.Windows.Forms.Label();
            this.Panel3.SuspendLayout();
            this.GroupBox3.SuspendLayout();
            this.Panel2.SuspendLayout();
            this.Panel1.SuspendLayout();
            this.GroupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // Label5
            // 
            Label5.BackColor = System.Drawing.Color.DarkSlateGray;
            Label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            Label5.ForeColor = System.Drawing.Color.White;
            Label5.Location = new System.Drawing.Point(1037, 23);
            Label5.Name = "Label5";
            Label5.Size = new System.Drawing.Size(225, 32);
            Label5.TabIndex = 268;
            Label5.Text = "كود الخدمة :";
            Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Panel3
            // 
            this.Panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel3.Controls.Add(this.btnPrint);
            this.Panel3.Controls.Add(this.btnGetData);
            this.Panel3.Controls.Add(this.btnDelete);
            this.Panel3.Controls.Add(this.btnUpdate);
            this.Panel3.Controls.Add(this.btnSave);
            this.Panel3.Controls.Add(this.btnNew);
            this.Panel3.Location = new System.Drawing.Point(1287, 95);
            this.Panel3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Panel3.Name = "Panel3";
            this.Panel3.Size = new System.Drawing.Size(183, 381);
            this.Panel3.TabIndex = 3;
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnPrint.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPrint.Enabled = false;
            this.btnPrint.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnPrint.Location = new System.Drawing.Point(13, 320);
            this.btnPrint.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(156, 49);
            this.btnPrint.TabIndex = 6;
            this.btnPrint.Text = "طباعة";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnGetData
            // 
            this.btnGetData.BackColor = System.Drawing.Color.Gold;
            this.btnGetData.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGetData.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnGetData.Location = new System.Drawing.Point(13, 255);
            this.btnGetData.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnGetData.Name = "btnGetData";
            this.btnGetData.Size = new System.Drawing.Size(156, 49);
            this.btnGetData.TabIndex = 5;
            this.btnGetData.Text = "بحث";
            this.btnGetData.UseVisualStyleBackColor = false;
            this.btnGetData.Click += new System.EventHandler(this.btnGetData_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.Firebrick;
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(13, 188);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(156, 49);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "حذف";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.Color.SkyBlue;
            this.btnUpdate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnUpdate.Location = new System.Drawing.Point(13, 127);
            this.btnUpdate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(156, 49);
            this.btnUpdate.TabIndex = 2;
            this.btnUpdate.Text = "تعديل";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnSave
            // 
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnSave.Location = new System.Drawing.Point(13, 66);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(156, 49);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "حفظ + طباعة";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnNew
            // 
            this.btnNew.BackColor = System.Drawing.Color.ForestGreen;
            this.btnNew.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnNew.ForeColor = System.Drawing.Color.White;
            this.btnNew.Location = new System.Drawing.Point(13, 9);
            this.btnNew.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(156, 49);
            this.btnNew.TabIndex = 0;
            this.btnNew.Text = "جديد";
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // GroupBox3
            // 
            this.GroupBox3.BackColor = System.Drawing.Color.AntiqueWhite;
            this.GroupBox3.Controls.Add(this.Label8);
            this.GroupBox3.Controls.Add(this.Label2);
            this.GroupBox3.Controls.Add(this.txtRemarks);
            this.GroupBox3.Controls.Add(this.btnSelect);
            this.GroupBox3.Controls.Add(this.Label3);
            this.GroupBox3.Controls.Add(this.txtCustomerID);
            this.GroupBox3.Controls.Add(this.txtCustomerName);
            this.GroupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.GroupBox3.Location = new System.Drawing.Point(815, 282);
            this.GroupBox3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GroupBox3.Size = new System.Drawing.Size(452, 239);
            this.GroupBox3.TabIndex = 343;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "معلومات العميل :";
            // 
            // Label8
            // 
            this.Label8.BackColor = System.Drawing.Color.DarkSlateGray;
            this.Label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.Label8.ForeColor = System.Drawing.Color.White;
            this.Label8.Location = new System.Drawing.Point(268, 111);
            this.Label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(157, 32);
            this.Label8.TabIndex = 85;
            this.Label8.Text = "ملاحظات :";
            this.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label2
            // 
            this.Label2.BackColor = System.Drawing.Color.DarkSlateGray;
            this.Label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.Label2.ForeColor = System.Drawing.Color.White;
            this.Label2.Location = new System.Drawing.Point(268, 75);
            this.Label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(157, 32);
            this.Label2.TabIndex = 7;
            this.Label2.Text = "اسم العميل :";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtRemarks
            // 
            this.txtRemarks.BackColor = System.Drawing.Color.White;
            this.txtRemarks.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.txtRemarks.Location = new System.Drawing.Point(20, 146);
            this.txtRemarks.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtRemarks.Multiline = true;
            this.txtRemarks.Name = "txtRemarks";
            this.txtRemarks.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtRemarks.Size = new System.Drawing.Size(360, 79);
            this.txtRemarks.TabIndex = 10;
            // 
            // btnSelect
            // 
            this.btnSelect.Image = ((System.Drawing.Image)(resources.GetObject("btnSelect.Image")));
            this.btnSelect.Location = new System.Drawing.Point(20, 23);
            this.btnSelect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(65, 46);
            this.btnSelect.TabIndex = 2;
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // Label3
            // 
            this.Label3.BackColor = System.Drawing.Color.DarkSlateGray;
            this.Label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.Label3.ForeColor = System.Drawing.Color.White;
            this.Label3.Location = new System.Drawing.Point(268, 36);
            this.Label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(157, 32);
            this.Label3.TabIndex = 0;
            this.Label3.Text = "كود العميل :";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtCustomerID
            // 
            this.txtCustomerID.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerID.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.txtCustomerID.Location = new System.Drawing.Point(93, 37);
            this.txtCustomerID.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtCustomerID.Name = "txtCustomerID";
            this.txtCustomerID.ReadOnly = true;
            this.txtCustomerID.Size = new System.Drawing.Size(145, 30);
            this.txtCustomerID.TabIndex = 0;
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.txtCustomerName.Location = new System.Drawing.Point(20, 75);
            this.txtCustomerName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(219, 30);
            this.txtCustomerName.TabIndex = 1;
            // 
            // txtProblemDescription
            // 
            this.txtProblemDescription.BackColor = System.Drawing.Color.White;
            this.txtProblemDescription.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.txtProblemDescription.Location = new System.Drawing.Point(27, 261);
            this.txtProblemDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtProblemDescription.Multiline = true;
            this.txtProblemDescription.Name = "txtProblemDescription";
            this.txtProblemDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtProblemDescription.Size = new System.Drawing.Size(563, 203);
            this.txtProblemDescription.TabIndex = 4;
            // 
            // dtpEstimatedRepairDate
            // 
            this.dtpEstimatedRepairDate.CustomFormat = "dd/MM/yyyy";
            this.dtpEstimatedRepairDate.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.dtpEstimatedRepairDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEstimatedRepairDate.Location = new System.Drawing.Point(819, 206);
            this.dtpEstimatedRepairDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpEstimatedRepairDate.Name = "dtpEstimatedRepairDate";
            this.dtpEstimatedRepairDate.Size = new System.Drawing.Size(203, 30);
            this.dtpEstimatedRepairDate.TabIndex = 7;
            // 
            // txtContactNo
            // 
            this.txtContactNo.BackColor = System.Drawing.SystemColors.Control;
            this.txtContactNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtContactNo.Location = new System.Drawing.Point(499, 46);
            this.txtContactNo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtContactNo.Name = "txtContactNo";
            this.txtContactNo.ReadOnly = true;
            this.txtContactNo.Size = new System.Drawing.Size(172, 24);
            this.txtContactNo.TabIndex = 319;
            this.txtContactNo.Visible = false;
            // 
            // txtID
            // 
            this.txtID.BackColor = System.Drawing.SystemColors.Control;
            this.txtID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtID.Location = new System.Drawing.Point(380, 49);
            this.txtID.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtID.Name = "txtID";
            this.txtID.ReadOnly = true;
            this.txtID.Size = new System.Drawing.Size(92, 24);
            this.txtID.TabIndex = 12;
            this.txtID.Visible = false;
            // 
            // txtCID
            // 
            this.txtCID.BackColor = System.Drawing.SystemColors.Control;
            this.txtCID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCID.Location = new System.Drawing.Point(380, 16);
            this.txtCID.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtCID.Name = "txtCID";
            this.txtCID.ReadOnly = true;
            this.txtCID.Size = new System.Drawing.Size(92, 24);
            this.txtCID.TabIndex = 7;
            this.txtCID.Visible = false;
            // 
            // txtItemsDescription
            // 
            this.txtItemsDescription.BackColor = System.Drawing.Color.White;
            this.txtItemsDescription.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.txtItemsDescription.Location = new System.Drawing.Point(27, 30);
            this.txtItemsDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtItemsDescription.Multiline = true;
            this.txtItemsDescription.Name = "txtItemsDescription";
            this.txtItemsDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtItemsDescription.Size = new System.Drawing.Size(563, 176);
            this.txtItemsDescription.TabIndex = 3;
            // 
            // cmbStatus
            // 
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.cmbStatus.FormattingEnabled = true;
            this.cmbStatus.Items.AddRange(new object[] {
            "تم أنجازها",
            "قيد العمل",
            "لم يتم انجازها"});
            this.cmbStatus.Location = new System.Drawing.Point(819, 242);
            this.cmbStatus.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(203, 31);
            this.cmbStatus.TabIndex = 8;
            // 
            // txtUpfront
            // 
            this.txtUpfront.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.txtUpfront.Location = new System.Drawing.Point(819, 170);
            this.txtUpfront.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtUpfront.Name = "txtUpfront";
            this.txtUpfront.Size = new System.Drawing.Size(203, 30);
            this.txtUpfront.TabIndex = 6;
            this.txtUpfront.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtChargesQuote
            // 
            this.txtChargesQuote.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.txtChargesQuote.Location = new System.Drawing.Point(819, 134);
            this.txtChargesQuote.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtChargesQuote.Name = "txtChargesQuote";
            this.txtChargesQuote.Size = new System.Drawing.Size(203, 30);
            this.txtChargesQuote.TabIndex = 5;
            this.txtChargesQuote.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // cmbServiceType
            // 
            this.cmbServiceType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbServiceType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbServiceType.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.cmbServiceType.FormattingEnabled = true;
            this.cmbServiceType.Location = new System.Drawing.Point(819, 96);
            this.cmbServiceType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbServiceType.Name = "cmbServiceType";
            this.cmbServiceType.Size = new System.Drawing.Size(203, 31);
            this.cmbServiceType.TabIndex = 2;
            // 
            // Label13
            // 
            this.Label13.BackColor = System.Drawing.Color.DarkSlateGray;
            this.Label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.Label13.ForeColor = System.Drawing.Color.White;
            this.Label13.Location = new System.Drawing.Point(1036, 242);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(225, 33);
            this.Label13.TabIndex = 342;
            this.Label13.Text = "الحالة :";
            this.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label7
            // 
            this.Label7.BackColor = System.Drawing.Color.DarkSlateGray;
            this.Label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.Label7.ForeColor = System.Drawing.Color.White;
            this.Label7.Location = new System.Drawing.Point(597, 31);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(164, 32);
            this.Label7.TabIndex = 337;
            this.Label7.Text = "وصف الخدمة :";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label12
            // 
            this.Label12.BackColor = System.Drawing.Color.DarkSlateGray;
            this.Label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.Label12.ForeColor = System.Drawing.Color.White;
            this.Label12.Location = new System.Drawing.Point(1036, 206);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(225, 32);
            this.Label12.TabIndex = 341;
            this.Label12.Text = "الوقت المقدر للتصليح :";
            this.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblUserType
            // 
            this.lblUserType.AutoSize = true;
            this.lblUserType.Location = new System.Drawing.Point(275, 22);
            this.lblUserType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUserType.Name = "lblUserType";
            this.lblUserType.Size = new System.Drawing.Size(71, 16);
            this.lblUserType.TabIndex = 315;
            this.lblUserType.Text = "User Type";
            this.lblUserType.Visible = false;
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(275, 38);
            this.lblUser.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(36, 16);
            this.lblUser.TabIndex = 313;
            this.lblUser.Text = "User";
            this.lblUser.Visible = false;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.BackColor = System.Drawing.Color.Transparent;
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.Label1.ForeColor = System.Drawing.Color.White;
            this.Label1.Location = new System.Drawing.Point(680, 20);
            this.Label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(86, 36);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "خدمات";
            // 
            // Panel2
            // 
            this.Panel2.BackColor = System.Drawing.Color.DarkSlateGray;
            this.Panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Panel2.Controls.Add(this.txtContactNo);
            this.Panel2.Controls.Add(this.txtID);
            this.Panel2.Controls.Add(this.txtCID);
            this.Panel2.Controls.Add(this.lblUserType);
            this.Panel2.Controls.Add(this.lblUser);
            this.Panel2.Controls.Add(this.Label1);
            this.Panel2.Location = new System.Drawing.Point(12, 9);
            this.Panel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Panel2.Name = "Panel2";
            this.Panel2.Size = new System.Drawing.Size(1459, 76);
            this.Panel2.TabIndex = 0;
            // 
            // Label11
            // 
            this.Label11.BackColor = System.Drawing.Color.DarkSlateGray;
            this.Label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.Label11.ForeColor = System.Drawing.Color.White;
            this.Label11.Location = new System.Drawing.Point(1036, 170);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(227, 32);
            this.Label11.TabIndex = 340;
            this.Label11.Text = "أيداع مسبق :";
            this.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Panel1
            // 
            this.Panel1.BackColor = System.Drawing.Color.White;
            this.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel1.Controls.Add(this.GroupBox4);
            this.Panel1.Controls.Add(this.Panel3);
            this.Panel1.Controls.Add(this.Panel2);
            this.Panel1.Location = new System.Drawing.Point(5, 6);
            this.Panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(1487, 633);
            this.Panel1.TabIndex = 3;
            // 
            // GroupBox4
            // 
            this.GroupBox4.Controls.Add(this.GroupBox3);
            this.GroupBox4.Controls.Add(this.txtProblemDescription);
            this.GroupBox4.Controls.Add(this.dtpEstimatedRepairDate);
            this.GroupBox4.Controls.Add(this.txtItemsDescription);
            this.GroupBox4.Controls.Add(this.cmbStatus);
            this.GroupBox4.Controls.Add(this.txtUpfront);
            this.GroupBox4.Controls.Add(this.txtChargesQuote);
            this.GroupBox4.Controls.Add(this.cmbServiceType);
            this.GroupBox4.Controls.Add(this.Label13);
            this.GroupBox4.Controls.Add(this.Label7);
            this.GroupBox4.Controls.Add(this.Label12);
            this.GroupBox4.Controls.Add(this.Label11);
            this.GroupBox4.Controls.Add(this.Label10);
            this.GroupBox4.Controls.Add(this.Label9);
            this.GroupBox4.Controls.Add(this.Label6);
            this.GroupBox4.Controls.Add(this.dtpServiceCreationDate);
            this.GroupBox4.Controls.Add(this.txtServiceCode);
            this.GroupBox4.Controls.Add(this.Label4);
            this.GroupBox4.Controls.Add(Label5);
            this.GroupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.GroupBox4.Location = new System.Drawing.Point(12, 87);
            this.GroupBox4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GroupBox4.Name = "GroupBox4";
            this.GroupBox4.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GroupBox4.Size = new System.Drawing.Size(1275, 534);
            this.GroupBox4.TabIndex = 0;
            this.GroupBox4.TabStop = false;
            this.GroupBox4.Text = "معلومات الخدمة";
            this.GroupBox4.Enter += new System.EventHandler(this.GroupBox4_Enter);
            // 
            // Label10
            // 
            this.Label10.BackColor = System.Drawing.Color.DarkSlateGray;
            this.Label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.Label10.ForeColor = System.Drawing.Color.White;
            this.Label10.Location = new System.Drawing.Point(1037, 134);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(225, 32);
            this.Label10.TabIndex = 339;
            this.Label10.Text = "الرسوم :";
            this.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label9
            // 
            this.Label9.BackColor = System.Drawing.Color.DarkSlateGray;
            this.Label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.Label9.ForeColor = System.Drawing.Color.White;
            this.Label9.Location = new System.Drawing.Point(597, 261);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(164, 32);
            this.Label9.TabIndex = 338;
            this.Label9.Text = "وصف المشكلة :";
            this.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label6
            // 
            this.Label6.BackColor = System.Drawing.Color.DarkSlateGray;
            this.Label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.Label6.ForeColor = System.Drawing.Color.White;
            this.Label6.Location = new System.Drawing.Point(1037, 96);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(225, 33);
            this.Label6.TabIndex = 336;
            this.Label6.Text = "نوع الخدمة :";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dtpServiceCreationDate
            // 
            this.dtpServiceCreationDate.CustomFormat = "dd/MM/yyyy";
            this.dtpServiceCreationDate.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.dtpServiceCreationDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpServiceCreationDate.Location = new System.Drawing.Point(819, 59);
            this.dtpServiceCreationDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpServiceCreationDate.Name = "dtpServiceCreationDate";
            this.dtpServiceCreationDate.Size = new System.Drawing.Size(203, 30);
            this.dtpServiceCreationDate.TabIndex = 1;
            // 
            // txtServiceCode
            // 
            this.txtServiceCode.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.txtServiceCode.Location = new System.Drawing.Point(819, 23);
            this.txtServiceCode.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtServiceCode.Name = "txtServiceCode";
            this.txtServiceCode.ReadOnly = true;
            this.txtServiceCode.Size = new System.Drawing.Size(203, 30);
            this.txtServiceCode.TabIndex = 0;
            // 
            // Label4
            // 
            this.Label4.BackColor = System.Drawing.Color.DarkSlateGray;
            this.Label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.Label4.ForeColor = System.Drawing.Color.White;
            this.Label4.Location = new System.Drawing.Point(1036, 59);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(225, 32);
            this.Label4.TabIndex = 335;
            this.Label4.Text = "تاريخ انشاء الخدمة :";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // services
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkSlateGray;
            this.ClientSize = new System.Drawing.Size(1500, 646);
            this.Controls.Add(this.Panel1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "services";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Panel3.ResumeLayout(false);
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox3.PerformLayout();
            this.Panel2.ResumeLayout(false);
            this.Panel2.PerformLayout();
            this.Panel1.ResumeLayout(false);
            this.GroupBox4.ResumeLayout(false);
            this.GroupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Panel Panel3;
        internal System.Windows.Forms.Button btnPrint;
        internal System.Windows.Forms.Button btnGetData;
        internal System.Windows.Forms.Button btnDelete;
        internal System.Windows.Forms.Button btnUpdate;
        internal System.Windows.Forms.Button btnSave;
        internal System.Windows.Forms.Button btnNew;
        internal System.Windows.Forms.GroupBox GroupBox3;
        internal System.Windows.Forms.Label Label8;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.TextBox txtRemarks;
        internal System.Windows.Forms.Button btnSelect;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.TextBox txtCustomerID;
        internal System.Windows.Forms.TextBox txtCustomerName;
        internal System.Windows.Forms.TextBox txtProblemDescription;
        internal System.Windows.Forms.DateTimePicker dtpEstimatedRepairDate;
        internal System.Windows.Forms.TextBox txtContactNo;
        internal System.Windows.Forms.TextBox txtID;
        internal System.Windows.Forms.TextBox txtCID;
        internal System.Windows.Forms.TextBox txtItemsDescription;
        internal System.Windows.Forms.ComboBox cmbStatus;
        internal System.Windows.Forms.TextBox txtUpfront;
        internal System.Windows.Forms.TextBox txtChargesQuote;
        internal System.Windows.Forms.ComboBox cmbServiceType;
        internal System.Windows.Forms.Label Label13;
        internal System.Windows.Forms.Label Label7;
        internal System.Windows.Forms.Label Label12;
        internal System.Windows.Forms.Label lblUserType;
        internal System.Windows.Forms.Label lblUser;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Panel Panel2;
        internal System.Windows.Forms.ToolTip ToolTip1;
        internal System.Windows.Forms.Label Label11;
        internal System.Windows.Forms.Panel Panel1;
        internal System.Windows.Forms.GroupBox GroupBox4;
        internal System.Windows.Forms.Label Label10;
        internal System.Windows.Forms.Label Label9;
        internal System.Windows.Forms.Label Label6;
        internal System.Windows.Forms.DateTimePicker dtpServiceCreationDate;
        internal System.Windows.Forms.TextBox txtServiceCode;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.Timer Timer1;
    }
}