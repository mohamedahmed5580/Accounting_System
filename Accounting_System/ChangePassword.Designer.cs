namespace Accounting_System
{
    partial class ChangePassword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangePassword));
            this.Panel1 = new System.Windows.Forms.Panel();
            this.OldPassword = new System.Windows.Forms.TextBox();
            this.UserID = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Button1 = new System.Windows.Forms.Button();
            this.NewPassword = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.ConfirmPassword = new System.Windows.Forms.TextBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.Panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel1
            // 
            this.Panel1.BackColor = System.Drawing.Color.AliceBlue;
            this.Panel1.Controls.Add(this.OldPassword);
            this.Panel1.Controls.Add(this.UserID);
            this.Panel1.Controls.Add(this.Label1);
            this.Panel1.Controls.Add(this.Label4);
            this.Panel1.Controls.Add(this.Label2);
            this.Panel1.Controls.Add(this.Button1);
            this.Panel1.Controls.Add(this.NewPassword);
            this.Panel1.Controls.Add(this.Label3);
            this.Panel1.Controls.Add(this.ConfirmPassword);
            this.Panel1.Location = new System.Drawing.Point(-7, 53);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(575, 286);
            this.Panel1.TabIndex = 60;
            this.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.Panel1_Paint);
            // 
            // OldPassword
            // 
            this.OldPassword.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.OldPassword.ForeColor = System.Drawing.Color.DarkViolet;
            this.OldPassword.Location = new System.Drawing.Point(136, 54);
            this.OldPassword.Name = "OldPassword";
            this.OldPassword.PasswordChar = '•';
            this.OldPassword.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.OldPassword.Size = new System.Drawing.Size(200, 26);
            this.OldPassword.TabIndex = 11;
            // 
            // UserID
            // 
            this.UserID.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.UserID.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.UserID.ForeColor = System.Drawing.Color.DarkViolet;
            this.UserID.Location = new System.Drawing.Point(136, 22);
            this.UserID.Name = "UserID";
            this.UserID.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.UserID.Size = new System.Drawing.Size(200, 26);
            this.UserID.TabIndex = 10;
            // 
            // Label1
            // 
            this.Label1.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.ForeColor = System.Drawing.Color.White;
            this.Label1.Location = new System.Drawing.Point(342, 53);
            this.Label1.Name = "Label1";
            this.Label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Label1.Size = new System.Drawing.Size(139, 29);
            this.Label1.TabIndex = 9;
            this.Label1.Text = "كلمة السر القديمة :";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label4
            // 
            this.Label4.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.Label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label4.ForeColor = System.Drawing.Color.White;
            this.Label4.Location = new System.Drawing.Point(342, 17);
            this.Label4.Name = "Label4";
            this.Label4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Label4.Size = new System.Drawing.Size(139, 29);
            this.Label4.TabIndex = 17;
            this.Label4.Text = "اسم المستخدم :";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label2
            // 
            this.Label2.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.Label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.ForeColor = System.Drawing.Color.White;
            this.Label2.Location = new System.Drawing.Point(342, 85);
            this.Label2.Name = "Label2";
            this.Label2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Label2.Size = new System.Drawing.Size(139, 29);
            this.Label2.TabIndex = 12;
            this.Label2.Text = "كلمة السر الجديدة :";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Button1
            // 
            this.Button1.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.Button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Button1.BackgroundImage")));
            this.Button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Button1.FlatAppearance.BorderSize = 0;
            this.Button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button1.ForeColor = System.Drawing.Color.White;
            this.Button1.Location = new System.Drawing.Point(176, 161);
            this.Button1.Name = "Button1";
            this.Button1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Button1.Size = new System.Drawing.Size(200, 57);
            this.Button1.TabIndex = 15;
            this.Button1.Text = "&تغيير كلمة السر";
            this.Button1.UseVisualStyleBackColor = false;
            this.Button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // NewPassword
            // 
            this.NewPassword.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.NewPassword.ForeColor = System.Drawing.Color.DarkViolet;
            this.NewPassword.Location = new System.Drawing.Point(136, 86);
            this.NewPassword.Name = "NewPassword";
            this.NewPassword.PasswordChar = '•';
            this.NewPassword.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.NewPassword.Size = new System.Drawing.Size(200, 26);
            this.NewPassword.TabIndex = 13;
            // 
            // Label3
            // 
            this.Label3.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.Label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.ForeColor = System.Drawing.Color.White;
            this.Label3.Location = new System.Drawing.Point(342, 117);
            this.Label3.Name = "Label3";
            this.Label3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Label3.Size = new System.Drawing.Size(139, 29);
            this.Label3.TabIndex = 16;
            this.Label3.Text = "تأكيد كلمة السر :";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ConfirmPassword
            // 
            this.ConfirmPassword.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.ConfirmPassword.ForeColor = System.Drawing.Color.DarkViolet;
            this.ConfirmPassword.Location = new System.Drawing.Point(136, 118);
            this.ConfirmPassword.Name = "ConfirmPassword";
            this.ConfirmPassword.PasswordChar = '•';
            this.ConfirmPassword.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ConfirmPassword.Size = new System.Drawing.Size(200, 26);
            this.ConfirmPassword.TabIndex = 14;
            // 
            // Label5
            // 
            this.Label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Label5.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.Label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label5.ForeColor = System.Drawing.Color.White;
            this.Label5.Location = new System.Drawing.Point(-7, 2);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(564, 50);
            this.Label5.TabIndex = 61;
            this.Label5.Text = " تغيير كلمة السر";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ChangePassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkSlateGray;
            this.ClientSize = new System.Drawing.Size(558, 276);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.Label5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangePassword";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Panel Panel1;
        internal System.Windows.Forms.TextBox OldPassword;
        internal System.Windows.Forms.TextBox UserID;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.Button Button1;
        internal System.Windows.Forms.TextBox NewPassword;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.TextBox ConfirmPassword;
        internal System.Windows.Forms.Label Label5;
    }
}