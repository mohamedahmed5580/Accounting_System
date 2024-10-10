using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Runtime.InteropServices;
namespace Accounting_System
{

    using AForge.Video;
    using AForge.Video.DirectShow;

    public partial class Camera : Form
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private AddCustomer addCustomerForm;
        public Camera()
        {
            InitializeComponent();
            this.Load += Camera_Load;
            this.FormClosed += Camera_FormClosed;

        }

        public Camera(AddCustomer addCustomerForm)
        {
            InitializeComponent();
            this.FormClosed += Camera_FormClosed;
            this.Load += Camera_Load;
        }
        private void Camera_Load(object sender, EventArgs e)
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count > 0)
            {
                cmbCamera.Items.Clear();
                foreach (FilterInfo device in videoDevices)
                {
                    cmbCamera.Items.Add(device.Name);
                }
                cmbCamera.SelectedIndex = 0;
                StartCamera();
            }
            else
            {
                MessageBox.Show("No camera found.");
                this.Close();
            }
            picFeed.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void StartCamera()
        {
            videoSource = new VideoCaptureDevice(videoDevices[cmbCamera.SelectedIndex].MonikerString);

            // Set the desired resolution (e.g., 640x480)
            videoSource.VideoResolution = videoSource.VideoCapabilities
                                            .FirstOrDefault(cap => cap.FrameSize.Width == 640 && cap.FrameSize.Height == 480);

            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
            videoSource.Start();
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap frame = (Bitmap)eventArgs.Frame.Clone();
            picFeed.Image = frame;
        }

        private void Camera_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopAndDisposeCamera();
        }

        private void StopAndDisposeCamera()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                videoSource.NewFrame -= video_NewFrame;
                videoSource.Stop();
                videoSource = null;

            }
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            if (picFeed.Image != null)
            {
                picPreview.Image = (Bitmap)picFeed.Image.Clone();
                btnSave.Enabled = true;
            }
            picFeed.SizeMode = PictureBoxSizeMode.Zoom;
        }
        private void Camera_FormClosed(object sender, FormClosedEventArgs e)
        {
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (picPreview.Image != null)
            {
                string sTempFileName = Path.GetTempFileName();

                try
                {
                    using (Bitmap b = new Bitmap(picPreview.Image))
                    {
                        b.Save(sTempFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        MessageBox.Show($"Image saved: {sTempFileName}");
                    }

                    // Update the PictureBox on AddCustomer form
                    if (addCustomerForm != null)
                    {
                        addCustomerForm.UpdatePictureBox((Bitmap)picPreview.Image.Clone());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while saving the image: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("No image to save.");
            }

            // Stop Timer and other background processes
            if (Timer1 != null)
            {
                Timer1.Enabled = false;
                Timer1.Dispose();
            }

            StopAndDisposeCamera();
            this.Close();
            this.Dispose();
        }

        private void picPreview_Click(object sender, EventArgs e)
        {

        }
    }

}
