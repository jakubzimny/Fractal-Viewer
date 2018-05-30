using System.IO;
using System.Linq;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace Fractal_Viewer
{
    public partial class MainWindow : Window
    {
        WriteableBitmap bitmap;
        PixelFormat pf;
        int width, height, stride;
        byte[] pixelData;
        DispatcherTimer timer;
        string chosenFractal;
        string chosenColorScheme;
        string renderTime;
        double c_re;
        double c_im;
        public const int iterMax = 1000;
        public MainWindow()
        {
            InitializeComponent();
            pf = PixelFormats.Rgb24; // 1 byte for each color
            width = (int)image.Width * 2; // *2 to increase resolution
            height = (int)image.Height * 2;
            stride = width * pf.BitsPerPixel / 8; // bytes per row
            pixelData = new byte[stride * height];
            bitmap = new WriteableBitmap(width, height, 96, 96, pf, null);
        }

        private void SetPixel(int x, int y, Color color, byte[] buffer, int stride)
        {
            int xIndex = x * 3;
            int yIndex = y * stride;
            buffer[xIndex + yIndex] = color.R;
            buffer[xIndex + yIndex + 1] = color.G;
            buffer[xIndex + yIndex + 2] = color.B;
        }

        private void RenderMandelbrot()
        {
            Parallel.For(0, height, row =>
            {
                double a, b;
                Complex z, c;
                for (int col = 0; col < width; col++)
                {
                    a = (col - (width / 2.0)) * 4.0 / width;
                    b = (row - (height / 2.0)) * 4.0 / width;
                    z = new Complex(0, 0);
                    c = new Complex(a, b);
                    int counter = 0;
                    while (z.MagnitudeSquared() < 4 && counter++ < iterMax)
                    {
                        z.Square();
                        z.Add(c);
                    }
                    if (counter < iterMax) SetPixel(col, row, GetColorMapping(counter),
                        pixelData, stride);
                    else SetPixel(col, row, Colors.Black, pixelData, stride);
                }
            });
        }

        private void RenderBurningShip()
        {
            Parallel.For(0, height, row =>
            {
                double a, b;
                Complex z, c;
                for (int col = 0; col < width; col++)
                {
                    a = (col - (width / 2.0)) * 4.0 / width;
                    b = (row - (height / 2.0)) * 4.0 / width;
                    z = new Complex(0, 0);
                    c = new Complex(a, b);
                    int counter = 0;
                    while (z.MagnitudeSquared() < 4 && counter++ < iterMax)
                    {
                        z.Abs();
                        z.Square();
                        z.Add(c);
                    }
                    if (counter < iterMax) SetPixel(col, row, GetColorMapping(counter),
                        pixelData, stride);
                    else SetPixel(col, row, Colors.Black, pixelData, stride);
                }
            });
        }

        private void RenderJuliaSet()
        {
            Parallel.For(0, height, row =>
            {
                double a, b;
                Complex z, c;
                for (int col = 0; col < width; col++)
                {
                    a = (col - (width / 2.0)) * 4.0 / width;
                    b = (row - (height / 2.0)) * 4.0 / width;
                    z = new Complex(a, b);
                    c = new Complex(c_re, c_im);
                    int counter = 0;
                    while (z.MagnitudeSquared() < 4 && counter++ < iterMax)
                    {
                        z.Square();
                        z.Add(c);
                    }
                    if (counter < iterMax) SetPixel(col, row, GetColorMapping(counter),
                        pixelData, stride);
                    else SetPixel(col, row, Colors.Black, pixelData, stride);
                }

            });
        }

        private void RenderLyapunovFractal()
        {
            int[] sequence = { 1,0 };
            Parallel.For(0, height, row =>
            {
                double a, b, r;
                for (int col = 0; col < width; col++)
                {
                    int counter = 0;
                    double lambda = 0;

                    double x = 0.5;
                    while (counter++ < 150)//capped at 150 for now
                    {
                        a = (double)row / (double)height * 2+2 ;
                        b = (double)col / (double)width * 2+2;
                        //Lyapunov Exponent
                        r = sequence[counter % sequence.Length] == 1 ? b : a;
                        x = r * x * (1 - x);
                        lambda += Math.Log(Math.Abs(r * (1.0 - 2.0 * x)));
                    }
                    lambda /= counter;
                    SetPixel(col, row, GetColorLyapunov(lambda), pixelData, stride);
                }
            });
        }

        private Color GetColorLyapunov(double lambda)
        {
            //TODO: Change this method 
            Color c;
            lambda *= 100;
            if (lambda < -280) c = Color.FromRgb(0, 0, 0);
            else if (lambda < -260) c = Color.FromRgb(49, 25, 0);
            else if (lambda < -230) c = Color.FromRgb(85, 50, 0);
            else if (lambda < -200) c = Color.FromRgb(116, 70, 0);
            else if (lambda < -180) c = Color.FromRgb(141, 90, 0);
            else if (lambda < -150) c = Color.FromRgb(169, 112, 0);
            else if (lambda < -120) c = Color.FromRgb(174, 119, 0);
            else if (lambda < -100) c = Color.FromRgb(189, 127, 0);
            else if (lambda < -80) c = Color.FromRgb(196, 138, 0);
            else if (lambda < -50) c = Color.FromRgb(200, 149, 0);
            else if (lambda < -20) c = Color.FromRgb(205, 162, 0);
            else if (lambda < -10) c = Color.FromRgb(212, 169, 0);
            else if (lambda < 0) c = Color.FromRgb(220, 175, 0);
            else if (lambda > 40) c = Color.FromRgb(0, 0, 55);
            else if (lambda > 35) c = Color.FromRgb(0, 0, 75);
            else if (lambda > 30) c = Color.FromRgb(0, 0, 100);
            else if (lambda > 25) c = Color.FromRgb(0, 0, 115);
            else if (lambda > 20) c = Color.FromRgb(0, 0, 125);
            else if (lambda > 15) c = Color.FromRgb(0, 0, 145);
            else if (lambda > 10) c = Color.FromRgb(0, 0, 175);
            else if (lambda > 5) c = Color.FromRgb(0, 0, 185);
            else c = Color.FromRgb(0, 0, 200);
            return c;
        }

        private void Render()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (chosenFractal == "Mandelbrot Set") RenderMandelbrot();
            else if (chosenFractal == "Burning Ship") RenderBurningShip();
            else if (chosenFractal == "Julia Set") RenderJuliaSet();
            else if (chosenFractal == "Lyapunov Fractal") RenderLyapunovFractal();
            sw.Stop();
            renderTime = sw.Elapsed.TotalSeconds.ToString() + " s";
        }

        private void UpdateScreen(object o, System.EventArgs e)
        {
            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixelData, stride, 0);
            image.Source = bitmap;
            if (renderTime != null)
                timeTB.Text = renderTime;
            else timeTB.Text = "Rendering...";
        }

        private void renderButton_Click(object sender, RoutedEventArgs e)
        {
            renderTime = null;
            chosenFractal = fractalCB.Text;
            chosenColorScheme = colorCB.Text;
            if (chosenFractal == "Julia Set")
            { // parse only when Julia Set because it doesn't affect other fractals
                try
                {
                    c_re = double.Parse(reTB.Text, System.Globalization.CultureInfo.InvariantCulture); // Invariant Culture in order to accept '.' 
                    c_im = double.Parse(imTB.Text, System.Globalization.CultureInfo.InvariantCulture); // instead of ',' while inputting number
                }
                catch (FormatException)
                {
                    System.Windows.MessageBox.Show("Entered c value is not number",
                        "Format Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            Task t = Task.Factory.StartNew(Render); // Render in another thread so UI stays responsive
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += UpdateScreen; // every 50 miliseconds update displayed image
            timer.Start();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "Image Files(*.png)|*.png|All(*.*)|*"
            };
            if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                using (FileStream fileStream = new FileStream(sfd.FileName, FileMode.OpenOrCreate))
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    encoder.Save(fileStream);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            timer.Stop();
        }

        private Color GetColorMapping(int n)
        {
            //TODO: Some gradients
            int i = n % 16;
            Color[] mapping = new Color[16];
            switch (chosenColorScheme)
            {
                case "1":
                    mapping[0] = Color.FromRgb(66, 30, 15);
                    mapping[1] = Color.FromRgb(25, 7, 26);
                    mapping[2] = Color.FromRgb(9, 1, 47);
                    mapping[3] = Color.FromRgb(4, 4, 73);
                    mapping[4] = Color.FromRgb(0, 7, 100);
                    mapping[5] = Color.FromRgb(12, 44, 138);
                    mapping[6] = Color.FromRgb(24, 82, 177);
                    mapping[7] = Color.FromRgb(57, 125, 209);
                    mapping[8] = Color.FromRgb(134, 181, 229);
                    mapping[9] = Color.FromRgb(211, 236, 248);
                    mapping[10] = Color.FromRgb(241, 233, 191);
                    mapping[11] = Color.FromRgb(248, 201, 95);
                    mapping[12] = Color.FromRgb(255, 170, 0);
                    mapping[13] = Color.FromRgb(204, 128, 0);
                    mapping[14] = Color.FromRgb(153, 87, 0);
                    mapping[15] = Color.FromRgb(106, 52, 3);
                    return mapping[i];
                case "2":
                    mapping[0] = Color.FromRgb(60, 25, 58);
                    mapping[1] = Color.FromRgb(3, 2, 45);
                    mapping[2] = Color.FromRgb(32, 17, 32);
                    mapping[3] = Color.FromRgb(75, 40, 25);
                    mapping[4] = Color.FromRgb(123, 67, 30);
                    mapping[5] = Color.FromRgb(152, 86, 38);
                    mapping[6] = Color.FromRgb(203, 124, 45);
                    mapping[7] = Color.FromRgb(235, 156, 87);
                    mapping[8] = Color.FromRgb(255, 187, 128);
                    mapping[9] = Color.FromRgb(226, 147, 133);
                    mapping[10] = Color.FromRgb(182, 107, 111);
                    mapping[11] = Color.FromRgb(146, 82, 96);
                    mapping[12] = Color.FromRgb(114, 61, 83);
                    mapping[13] = Color.FromRgb(68, 35, 66);
                    mapping[14] = Color.FromRgb(35, 18, 52);
                    mapping[15] = Color.FromRgb(19, 10, 43);
                    return mapping[i];
                default:
                    mapping[0] = Color.FromRgb(95, 44, 235);
                    mapping[1] = Color.FromRgb(95, 44, 235);
                    mapping[2] = Color.FromRgb(100, 20, 220);
                    mapping[3] = Color.FromRgb(95, 4, 213);
                    mapping[4] = Color.FromRgb(87, 34, 250);
                    mapping[5] = Color.FromRgb(75, 55, 255);
                    mapping[6] = Color.FromRgb(56, 74, 255);
                    mapping[7] = Color.FromRgb(3, 172, 210);
                    mapping[8] = Color.FromRgb(4, 215, 166);
                    mapping[9] = Color.FromRgb(31, 248, 106);
                    mapping[10] = Color.FromRgb(88, 253, 45);
                    mapping[11] = Color.FromRgb(146, 82, 96);
                    mapping[12] = Color.FromRgb(157, 222, 7);
                    mapping[13] = Color.FromRgb(212, 171, 3);
                    mapping[14] = Color.FromRgb(240, 125, 19);
                    mapping[15] = Color.FromRgb(254, 82, 49);
                    return mapping[i];
            }
        }
    }
}
