using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace Fractal_Viewer
{
    public partial class MainWindow : Window
    {
        BitmapSource bitmap;
        PixelFormat pf;
        int width, height, stride;
        byte[] pixelData;
        DispatcherTimer timer;
        string chosenFractal;
        string chosenColorScheme;
        public MainWindow()
        {          
            InitializeComponent();
            pf = PixelFormats.Rgb24; //8-bit per color
            width = (int)image.Width * 2; //*2 to increase resolution
            height = (int)image.Height * 2;
            stride = (width * pf.BitsPerPixel + 7) / 8;
            pixelData = new byte[stride * height];
        }

        void SetPixel(int x, int y, Color c, byte[] buffer, int rawStride)
        {
            int xIndex = x * 3;
            int yIndex = y * rawStride;
            buffer[xIndex + yIndex] = c.R;
            buffer[xIndex + yIndex + 1] = c.G;
            buffer[xIndex + yIndex + 2] = c.B;
        }

        void RenderMandelbrot()
        {
            //TODO: Optimize this
            int iterMax = 1000;
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    double a = (col - (width / 2.0)) * 4.0 / width; 
                    double b = (row - (height / 2.0)) * 4.0 / width;
                    double x = 0, y = 0;
                    int counter = 0;
                    while (x * x + y * y < 4 && counter < iterMax)
                    {
                        double newX = x * x - y * y + a;
                        y = 2 * x * y + b;
                        x = newX;
                        counter++;
                    }
                    if (counter < iterMax) SetPixel(col, row, GetColorMapping(counter),
                        pixelData, stride);
                    else SetPixel(col, row, Colors.Black, pixelData, stride);
                }
            }
        }

        void Render()
        {
            if (chosenFractal == "Mandelbrot Set") RenderMandelbrot();
            else
            {
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        SetPixel(col, row, Colors.Gray, pixelData, stride);
                    }
                }
            }
        }
        void UpdateScreen(object o, System.EventArgs e)
        {
            bitmap = BitmapSource.Create(width, height, 96, 96, 
                pf, null, pixelData, stride);
            image.Source = bitmap;
        }
        void button_Click(object sender, RoutedEventArgs e)
        {
            chosenFractal = fractalCB.Text;
            chosenColorScheme = colorCB.Text;
            Task t = Task.Factory.StartNew(Render);
            timer = new DispatcherTimer();
            timer.Interval = System.TimeSpan.FromMilliseconds(100);
            timer.Tick += UpdateScreen;
            timer.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            timer.Stop();
        }

        private Color GetColorMapping(int n)
        {
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
                    mapping[0] = Color.FromRgb(255, 58, 71);
                    mapping[1] = Color.FromRgb(244, 24, 117);
                    mapping[2] = Color.FromRgb(194, 1, 190);
                    mapping[3] = Color.FromRgb(168, 4, 213);
                    mapping[4] = Color.FromRgb(102, 34, 250);
                    mapping[5] = Color.FromRgb(75, 55, 255);
                    mapping[6] = Color.FromRgb(56, 74, 255);
                    mapping[7] = Color.FromRgb(3, 172, 210);
                    mapping[8] = Color.FromRgb(4, 215, 166);
                    mapping[9] = Color.FromRgb(31, 248, 106);
                    mapping[10] = Color.FromRgb(88, 253, 45);
                    mapping[11] = Color.FromRgb(146, 82, 96);
                    mapping[12] = Color.FromRgb(157,222, 7);
                    mapping[13] = Color.FromRgb(212, 171, 3);
                    mapping[14] = Color.FromRgb(240, 125, 19);
                    mapping[15] = Color.FromRgb(254, 82, 49);
                    return mapping[i];
            }              
        }
    }
}
