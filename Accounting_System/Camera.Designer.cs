using System.Threading.Tasks;
using System.Threading;
using System;

namespace Accounting_System
{
    partial class Camera
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        private CancellationTokenSource cancellationTokenSource;

        private async Task SomeBackgroundTask()
        {
            cancellationTokenSource = new CancellationTokenSource();
            try
            {
                await Task.Run(() =>
                {
                    // Your background task code here
                }, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation
            }
        }

        private void CancelBackgroundTask()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CancelBackgroundTask();
                StopAndDisposeCamera();
                if (Timer1 != null)
                {
                    Timer1.Dispose();
                }
                components?.Dispose();
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
            this.btnSave = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btnCapture = new System.Windows.Forms.Button();
            this.cmbCamera = new System.Windows.Forms.ComboBox();
            this.lblCamera = new System.Windows.Forms.Label();
            this.Timer1 = new System.Windows.Forms.Timer(this.components);
            this.Label1 = new System.Windows.Forms.Label();
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.picFeed = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFeed)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.MistyRose;
            this.btnSave.Enabled = false;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(394, 304);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(276, 38);
            this.btnSave.TabIndex = 18;
            this.btnSave.Text = "نسخ";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCapture
            // 
            this.btnCapture.BackColor = System.Drawing.Color.MistyRose;
            this.btnCapture.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCapture.Location = new System.Drawing.Point(10, 304);
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.Size = new System.Drawing.Size(276, 38);
            this.btnCapture.TabIndex = 16;
            this.btnCapture.Text = "التقاط";
            this.btnCapture.UseVisualStyleBackColor = false;
            this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
            // 
            // cmbCamera
            // 
            this.cmbCamera.BackColor = System.Drawing.Color.MistyRose;
            this.cmbCamera.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCamera.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbCamera.FormattingEnabled = true;
            this.cmbCamera.Location = new System.Drawing.Point(10, 15);
            this.cmbCamera.Name = "cmbCamera";
            this.cmbCamera.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cmbCamera.Size = new System.Drawing.Size(130, 21);
            this.cmbCamera.TabIndex = 15;
            // 
            // lblCamera
            // 
            this.lblCamera.BackColor = System.Drawing.Color.DarkSlateGray;
            this.lblCamera.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCamera.ForeColor = System.Drawing.Color.White;
            this.lblCamera.Location = new System.Drawing.Point(160, 19);
            this.lblCamera.Name = "lblCamera";
            this.lblCamera.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblCamera.Size = new System.Drawing.Size(125, 26);
            this.lblCamera.TabIndex = 14;
            this.lblCamera.Text = "أختيار الكاميرا :";
            this.lblCamera.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label1
            // 
            this.Label1.BackColor = System.Drawing.Color.DarkSlateGray;
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.ForeColor = System.Drawing.Color.White;
            this.Label1.Location = new System.Drawing.Point(541, 19);
            this.Label1.Name = "Label1";
            this.Label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Label1.Size = new System.Drawing.Size(130, 27);
            this.Label1.TabIndex = 19;
            this.Label1.Text = "عرض الصورة :";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picPreview
            // 
            this.picPreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picPreview.Location = new System.Drawing.Point(394, 49);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(276, 249);
            this.picPreview.TabIndex = 17;
            this.picPreview.TabStop = false;
            this.picPreview.Click += new System.EventHandler(this.picPreview_Click);
            // 
            // picFeed
            // 
            this.picFeed.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picFeed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picFeed.Location = new System.Drawing.Point(10, 49);
            this.picFeed.Name = "picFeed";
            this.picFeed.Size = new System.Drawing.Size(276, 249);
            this.picFeed.TabIndex = 13;
            this.picFeed.TabStop = false;
            // 
            // Camera
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(688, 357);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.picPreview);
            this.Controls.Add(this.btnCapture);
            this.Controls.Add(this.cmbCamera);
            this.Controls.Add(this.lblCamera);
            this.Controls.Add(this.picFeed);
            this.Controls.Add(this.Label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Camera";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.Camera_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFeed)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btnCapture;
        private System.Windows.Forms.ComboBox cmbCamera;
        private System.Windows.Forms.Label lblCamera;
        private System.Windows.Forms.PictureBox picFeed;
        internal System.Windows.Forms.Timer Timer1;
        private System.Windows.Forms.Label Label1;
    }
}