namespace Accounting_System
{
    partial class loading
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(loading));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Prbar = new System.Windows.Forms.ProgressBar();
            this.pbDBConfig = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbDBConfig)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 145;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.LightSlateGray;
            this.label2.Location = new System.Drawing.Point(467, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 23);
            this.label2.TabIndex = 29;
            this.label2.Text = "ver 7.0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 15F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.LightSlateGray;
            this.label1.Location = new System.Drawing.Point(152, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(391, 29);
            this.label1.TabIndex = 28;
            this.label1.Text = "Accounting System Managment";
            // 
            // Prbar
            // 
            this.Prbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Prbar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Prbar.Location = new System.Drawing.Point(9, 207);
            this.Prbar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Prbar.Name = "Prbar";
            this.Prbar.Size = new System.Drawing.Size(544, 27);
            this.Prbar.TabIndex = 20;
            this.Prbar.Click += new System.EventHandler(this.Prbar_Click);
            // 
            // pbDBConfig
            // 
            this.pbDBConfig.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbDBConfig.Image = ((System.Drawing.Image)(resources.GetObject("pbDBConfig.Image")));
            this.pbDBConfig.Location = new System.Drawing.Point(9, 6);
            this.pbDBConfig.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pbDBConfig.Name = "pbDBConfig";
            this.pbDBConfig.Size = new System.Drawing.Size(31, 30);
            this.pbDBConfig.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbDBConfig.TabIndex = 30;
            this.pbDBConfig.TabStop = false;
            this.pbDBConfig.Click += new System.EventHandler(this.pbDBConfig_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Accounting_System.Properties.Resources.accounting;
            this.pictureBox1.Location = new System.Drawing.Point(9, 42);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(137, 122);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 27;
            this.pictureBox1.TabStop = false;
            // 
            // loading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 241);
            this.Controls.Add(this.pbDBConfig);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Prbar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "loading";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.loading_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbDBConfig)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.PictureBox pbDBConfig;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ProgressBar Prbar;
    }
}