using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace CameraDetector
{
    public partial class MyForm : Form
    {
        FilterInfoCollection videoDevices;
        VideoCaptureDevice videoSource;

        public MyForm()
        {
            InitializeComponent();
        }

        private void MyForm_Load(object sender, EventArgs e)
        {
            // get the collection of video input devices
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            // list these devices in the combobox
            foreach (FilterInfo device in videoDevices)
            {
                comboBoxDevices.Items.Add(device.Name);
               
            }

            videoSource = new VideoCaptureDevice();
            comboBoxDevices.SelectedIndex = 0; // default selected item will be the first device
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            startPreview();
        }

        private void startPreview()
        {
            if (videoSource.IsRunning)
            {
                videoSource.Stop();
                pictureBoxOutput.Image = null;
                pictureBoxOutput.Invalidate();
            }
            else
            {
                videoSource = new VideoCaptureDevice(videoDevices[comboBoxDevices.SelectedIndex].MonikerString);
                videoSource.NewFrame += videoSource_NewFrame;
                videoSource.Start();
            }
        }

        void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBoxOutput.Image = (Bitmap)eventArgs.Frame.Clone();
        }

       
        private void comboBoxDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void BrightnessScroll(object sender, EventArgs e)
        {//
            //videoSource.
        }

        private void MyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoSource.IsRunning)
            {
                videoSource.Stop();
            }
        }

        private void CapturePhoto(object sender, EventArgs e)
        {
            if (pictureBoxOutput.Image != null)
            {
                captureImage();
            }
            else
            { MessageBox.Show("null exception"); }
        }

        private void captureImage()
        {
            Bitmap varBmp = new Bitmap(pictureBoxOutput.Image);
            Bitmap current = (Bitmap) varBmp.Clone();
            string filepath = Environment.CurrentDirectory;
            string fileName = System.IO.Path.Combine(filepath, @"name.bmp");
            current.Save(fileName);
            current.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        bool shouldDetect = false;
        private void button3_Click(object sender, EventArgs e)
        {

            Bitmap firstBitmap;
            Bitmap secondBitmap;
            for (int i = 0; i < 1000; i++)
                {
                    firstBitmap = new Bitmap(pictureBoxOutput.Image);
                    Thread.Sleep(50);
                    secondBitmap = new Bitmap(pictureBoxOutput.Image);
                //                    label1.Text = bitmapsAreEqual(firstBitmap, secondBitmap) + "%";

                    

                    if (similiarityOfBitmaps(firstBitmap, secondBitmap) < 10)
                    {
                        Console.WriteLine("Wykryto ruch" + i);
                    }
                Thread.Sleep(50);
                firstBitmap.Dispose();
                    secondBitmap.Dispose();
                }

        }



        private double similiarityOfBitmaps(Bitmap a, Bitmap b)
        {
            var height = a.PhysicalDimension.Height;
            var width = a.PhysicalDimension.Width;
            double sumPixels = 0;
            double sameColors = 0;

            for (int i = 0; i < height; i+=10)
            {
                for (int j = 0; j < width; j+=10)
                {
                    sumPixels++;
                    Color firstPixel = a.GetPixel(j, i);
                    Color secondPixel = b.GetPixel(j, i);
                    if (compareTwoPixels(firstPixel, secondPixel))
                    {
                        sameColors++;
                    }
                }
            }
            double similarity = (sameColors / sumPixels)*100;
            return similarity;
        }

        private bool compareTwoPixels(Color a, Color b)
        {

            return a.GetBrightness().Equals(b.GetBrightness());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            startPreview();
            videoSource.DisplayPropertyPage(Handle);
        }
    }
}
