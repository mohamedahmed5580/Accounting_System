namespace Accounting_System
{
    partial class Actives
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Actives));
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxMacAddress = new Guna.UI.WinForms.GunaTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxKey = new Guna.UI.WinForms.GunaTextBox();
            this.buttonActivation = new Guna.UI.WinForms.GunaAdvenceButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Tai Le", 36F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(290, 37);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(328, 61);
            this.label1.TabIndex = 0;
            this.label1.Text = "ادخل كود التفعيل ";
            // 
            // textBoxMacAddress
            // 
            this.textBoxMacAddress.BackColor = System.Drawing.Color.Transparent;
            this.textBoxMacAddress.BaseColor = System.Drawing.Color.White;
            this.textBoxMacAddress.BorderColor = System.Drawing.Color.Silver;
            this.textBoxMacAddress.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBoxMacAddress.Enabled = false;
            this.textBoxMacAddress.FocusedBaseColor = System.Drawing.Color.White;
            this.textBoxMacAddress.FocusedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.textBoxMacAddress.FocusedForeColor = System.Drawing.SystemColors.ControlText;
            this.textBoxMacAddress.Font = new System.Drawing.Font("Berlin Sans FB", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMacAddress.Location = new System.Drawing.Point(249, 188);
            this.textBoxMacAddress.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxMacAddress.Name = "textBoxMacAddress";
            this.textBoxMacAddress.PasswordChar = '\0';
            this.textBoxMacAddress.Radius = 10;
            this.textBoxMacAddress.SelectedText = "";
            this.textBoxMacAddress.Size = new System.Drawing.Size(394, 45);
            this.textBoxMacAddress.TabIndex = 1;
            this.textBoxMacAddress.TextChanged += new System.EventHandler(this.gunaTextBox1_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(245, 148);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 26);
            this.label2.TabIndex = 2;
            this.label2.Text = "Mac Address";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(245, 253);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(179, 26);
            this.label3.TabIndex = 4;
            this.label3.Text = "Enter the Code:";
            // 
            // textBoxKey
            // 
            this.textBoxKey.BackColor = System.Drawing.Color.Transparent;
            this.textBoxKey.BaseColor = System.Drawing.Color.White;
            this.textBoxKey.BorderColor = System.Drawing.Color.Silver;
            this.textBoxKey.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBoxKey.FocusedBaseColor = System.Drawing.Color.White;
            this.textBoxKey.FocusedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.textBoxKey.FocusedForeColor = System.Drawing.SystemColors.ControlText;
            this.textBoxKey.Font = new System.Drawing.Font("Berlin Sans FB", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxKey.Location = new System.Drawing.Point(249, 292);
            this.textBoxKey.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxKey.Name = "textBoxKey";
            this.textBoxKey.PasswordChar = '●';
            this.textBoxKey.Radius = 10;
            this.textBoxKey.SelectedText = "";
            this.textBoxKey.Size = new System.Drawing.Size(394, 45);
            this.textBoxKey.TabIndex = 3;
            this.textBoxKey.UseSystemPasswordChar = true;
            // 
            // buttonActivation
            // 
            this.buttonActivation.AnimationHoverSpeed = 0.07F;
            this.buttonActivation.AnimationSpeed = 0.03F;
            this.buttonActivation.BackColor = System.Drawing.Color.Transparent;
            this.buttonActivation.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.buttonActivation.BorderColor = System.Drawing.Color.Black;
            this.buttonActivation.CheckedBaseColor = System.Drawing.Color.Gray;
            this.buttonActivation.CheckedBorderColor = System.Drawing.Color.Black;
            this.buttonActivation.CheckedForeColor = System.Drawing.Color.White;
            this.buttonActivation.CheckedImage = ((System.Drawing.Image)(resources.GetObject("buttonActivation.CheckedImage")));
            this.buttonActivation.CheckedLineColor = System.Drawing.Color.DimGray;
            this.buttonActivation.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonActivation.FocusedColor = System.Drawing.Color.Empty;
            this.buttonActivation.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.buttonActivation.ForeColor = System.Drawing.Color.White;
            this.buttonActivation.Image = null;
            this.buttonActivation.ImageSize = new System.Drawing.Size(20, 20);
            this.buttonActivation.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(58)))), ((int)(((byte)(170)))));
            this.buttonActivation.Location = new System.Drawing.Point(337, 369);
            this.buttonActivation.Margin = new System.Windows.Forms.Padding(2);
            this.buttonActivation.Name = "buttonActivation";
            this.buttonActivation.OnHoverBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(151)))), ((int)(((byte)(143)))), ((int)(((byte)(255)))));
            this.buttonActivation.OnHoverBorderColor = System.Drawing.Color.Black;
            this.buttonActivation.OnHoverForeColor = System.Drawing.Color.White;
            this.buttonActivation.OnHoverImage = null;
            this.buttonActivation.OnHoverLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(58)))), ((int)(((byte)(170)))));
            this.buttonActivation.OnPressedColor = System.Drawing.Color.Black;
            this.buttonActivation.Radius = 15;
            this.buttonActivation.Size = new System.Drawing.Size(230, 56);
            this.buttonActivation.TabIndex = 5;
            this.buttonActivation.Text = "تفعيل";
            this.buttonActivation.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.buttonActivation.Click += new System.EventHandler(this.buttonActivation_Click_1);
            // 
            // Actives
            // 
            this.AcceptButton = this.buttonActivation;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(952, 457);
            this.Controls.Add(this.buttonActivation);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxKey);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxMacAddress);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Actives";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "التفعيل";
            this.Load += new System.EventHandler(this.Actives_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Guna.UI.WinForms.GunaTextBox textBoxMacAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private Guna.UI.WinForms.GunaTextBox textBoxKey;
        private Guna.UI.WinForms.GunaAdvenceButton buttonActivation;
    }
}