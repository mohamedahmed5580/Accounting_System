namespace Accounting_System
{
    partial class NewFile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewFile));
            this.TXT_LATIN = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.TXT_LEVEL_CURRENCYE = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.CMB_CURRENCY = new System.Windows.Forms.ComboBox();
            this.TXT_NAME = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.TXT_DATABASE = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.PL_4 = new System.Windows.Forms.Panel();
            this.Button1 = new System.Windows.Forms.Button();
            this.Button2 = new System.Windows.Forms.Button();
            this.Label4 = new System.Windows.Forms.Label();
            this.rbWindows = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.tbServer = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.PL_4.SuspendLayout();
            this.SuspendLayout();
            // 
            // TXT_LATIN
            // 
            this.TXT_LATIN.Font = new System.Drawing.Font("Tahoma", 10F);
            this.TXT_LATIN.Location = new System.Drawing.Point(120, 60);
            this.TXT_LATIN.Margin = new System.Windows.Forms.Padding(4);
            this.TXT_LATIN.Name = "TXT_LATIN";
            this.TXT_LATIN.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.TXT_LATIN.Size = new System.Drawing.Size(272, 28);
            this.TXT_LATIN.TabIndex = 1076;
            this.TXT_LATIN.Text = "USD";
            this.TXT_LATIN.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TXT_LATIN.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(401, 65);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(111, 18);
            this.label7.TabIndex = 1077;
            this.label7.Text = "العملة انجليزي";
            this.label7.Visible = false;
            // 
            // TXT_LEVEL_CURRENCYE
            // 
            this.TXT_LEVEL_CURRENCYE.Font = new System.Drawing.Font("Tahoma", 10F);
            this.TXT_LEVEL_CURRENCYE.Location = new System.Drawing.Point(120, 95);
            this.TXT_LEVEL_CURRENCYE.Margin = new System.Windows.Forms.Padding(4);
            this.TXT_LEVEL_CURRENCYE.Name = "TXT_LEVEL_CURRENCYE";
            this.TXT_LEVEL_CURRENCYE.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.TXT_LEVEL_CURRENCYE.Size = new System.Drawing.Size(272, 28);
            this.TXT_LEVEL_CURRENCYE.TabIndex = 1074;
            this.TXT_LEVEL_CURRENCYE.Text = "سنت";
            this.TXT_LEVEL_CURRENCYE.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TXT_LEVEL_CURRENCYE.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(401, 100);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 18);
            this.label5.TabIndex = 1075;
            this.label5.Text = "جزء العملة";
            this.label5.Visible = false;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.Location = new System.Drawing.Point(401, 33);
            this.Label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(52, 18);
            this.Label3.TabIndex = 1073;
            this.Label3.Text = "العملة";
            this.Label3.Visible = false;
            // 
            // CMB_CURRENCY
            // 
            this.CMB_CURRENCY.Font = new System.Drawing.Font("Tahoma", 10F);
            this.CMB_CURRENCY.FormattingEnabled = true;
            this.CMB_CURRENCY.Items.AddRange(new object[] {
            "ليرة سورية",
            "دولار أمريكي",
            "درهم إماراتي",
            "دينار كويتي",
            "ريال سعودي",
            "دينار عراقي",
            "ليرة لبنانية",
            "دينار بحريني",
            "ريال قطري",
            "ريال يمني",
            "جنيه مصري",
            "يورو",
            "ريال عماني",
            "ليرة تركية"});
            this.CMB_CURRENCY.Location = new System.Drawing.Point(120, 35);
            this.CMB_CURRENCY.Margin = new System.Windows.Forms.Padding(4);
            this.CMB_CURRENCY.Name = "CMB_CURRENCY";
            this.CMB_CURRENCY.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.CMB_CURRENCY.Size = new System.Drawing.Size(272, 29);
            this.CMB_CURRENCY.TabIndex = 1072;
            this.CMB_CURRENCY.Text = "دولار أمريكي";
            this.CMB_CURRENCY.Visible = false;
            // 
            // TXT_NAME
            // 
            this.TXT_NAME.Font = new System.Drawing.Font("Tahoma", 10F);
            this.TXT_NAME.Location = new System.Drawing.Point(32, 32);
            this.TXT_NAME.Margin = new System.Windows.Forms.Padding(4);
            this.TXT_NAME.Name = "TXT_NAME";
            this.TXT_NAME.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.TXT_NAME.Size = new System.Drawing.Size(421, 28);
            this.TXT_NAME.TabIndex = 1070;
            this.TXT_NAME.Text = " ";
            this.TXT_NAME.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.Location = new System.Drawing.Point(28, 11);
            this.Label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(81, 18);
            this.Label2.TabIndex = 1071;
            this.Label2.Text = "الاســــــــم";
            // 
            // TXT_DATABASE
            // 
            this.TXT_DATABASE.Font = new System.Drawing.Font("Tahoma", 10F);
            this.TXT_DATABASE.Location = new System.Drawing.Point(31, 157);
            this.TXT_DATABASE.Margin = new System.Windows.Forms.Padding(4);
            this.TXT_DATABASE.Name = "TXT_DATABASE";
            this.TXT_DATABASE.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.TXT_DATABASE.Size = new System.Drawing.Size(421, 28);
            this.TXT_DATABASE.TabIndex = 1068;
            this.TXT_DATABASE.Text = " ";
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.Label1.Location = new System.Drawing.Point(27, 136);
            this.Label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(137, 18);
            this.Label1.TabIndex = 1069;
            this.Label1.Text = "اسم قاعدة البيانات";
            // 
            // PL_4
            // 
            this.PL_4.BackColor = System.Drawing.Color.White;
            this.PL_4.Controls.Add(this.TXT_LATIN);
            this.PL_4.Controls.Add(this.label7);
            this.PL_4.Controls.Add(this.TXT_LEVEL_CURRENCYE);
            this.PL_4.Controls.Add(this.CMB_CURRENCY);
            this.PL_4.Controls.Add(this.label5);
            this.PL_4.Controls.Add(this.Label3);
            this.PL_4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.PL_4.Location = new System.Drawing.Point(0, 350);
            this.PL_4.Margin = new System.Windows.Forms.Padding(4);
            this.PL_4.Name = "PL_4";
            this.PL_4.Size = new System.Drawing.Size(467, 10);
            this.PL_4.TabIndex = 1067;
            // 
            // Button1
            // 
            this.Button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button1.Image = ((System.Drawing.Image)(resources.GetObject("Button1.Image")));
            this.Button1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Button1.Location = new System.Drawing.Point(326, 303);
            this.Button1.Margin = new System.Windows.Forms.Padding(4);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(127, 39);
            this.Button1.TabIndex = 1047;
            this.Button1.Text = "خروج";
            this.Button1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Button1.UseVisualStyleBackColor = false;
            this.Button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // Button2
            // 
            this.Button2.BackColor = System.Drawing.Color.MediumSpringGreen;
            this.Button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button2.Image = ((System.Drawing.Image)(resources.GetObject("Button2.Image")));
            this.Button2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Button2.Location = new System.Drawing.Point(32, 305);
            this.Button2.Margin = new System.Windows.Forms.Padding(4);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(171, 39);
            this.Button2.TabIndex = 1046;
            this.Button2.Text = "انشاء ملف جديد";
            this.Button2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Button2.UseVisualStyleBackColor = false;
            this.Button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(207, 237);
            this.Label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(232, 16);
            this.Label4.TabIndex = 1078;
            this.Label4.Text = " (يجب أن يكون باللغة الانجليزية وبدون مسافات)";
            // 
            // rbWindows
            // 
            this.rbWindows.AutoSize = true;
            this.rbWindows.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbWindows.Location = new System.Drawing.Point(138, 266);
            this.rbWindows.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rbWindows.Name = "rbWindows";
            this.rbWindows.Size = new System.Drawing.Size(209, 22);
            this.rbWindows.TabIndex = 1082;
            this.rbWindows.Text = "Windows Authintecation";
            this.rbWindows.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.Location = new System.Drawing.Point(28, 270);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 18);
            this.label6.TabIndex = 1081;
            this.label6.Text = "Login Way:";
            // 
            // tbServer
            // 
            this.tbServer.Location = new System.Drawing.Point(32, 96);
            this.tbServer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tbServer.Name = "tbServer";
            this.tbServer.Size = new System.Drawing.Size(421, 22);
            this.tbServer.TabIndex = 1080;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label8.Location = new System.Drawing.Point(29, 76);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 18);
            this.label8.TabIndex = 1079;
            this.label8.Text = "السيرفر";
            // 
            // NewFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 360);
            this.Controls.Add(this.rbWindows);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbServer);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.TXT_NAME);
            this.Controls.Add(this.Button1);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.TXT_DATABASE);
            this.Controls.Add(this.Button2);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.PL_4);
            this.Controls.Add(this.Label4);
            this.Name = "NewFile";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Text = "NewFile";
            this.Load += new System.EventHandler(this.NewFile_Load);
            this.PL_4.ResumeLayout(false);
            this.PL_4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.TextBox TXT_LATIN;
        internal System.Windows.Forms.Label label7;
        internal System.Windows.Forms.TextBox TXT_LEVEL_CURRENCYE;
        internal System.Windows.Forms.Label label5;
        internal System.Windows.Forms.Label Label3;
        private System.Windows.Forms.ComboBox CMB_CURRENCY;
        internal System.Windows.Forms.TextBox TXT_NAME;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.TextBox TXT_DATABASE;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Panel PL_4;
        private System.Windows.Forms.Button Button1;
        private System.Windows.Forms.Button Button2;
        internal System.Windows.Forms.Label Label4;
        private System.Windows.Forms.RadioButton rbWindows;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbServer;
        private System.Windows.Forms.Label label8;
    }
}