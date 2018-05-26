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
        PixelFormat pf = PixelFormats.Rgb24;
        int width, height, rawStride;
        byte[] pixelData;
        DispatcherTimer timer;
        Color clr;
        public MainWindow()
        {
            InitializeComponent();
        }
        void Init()
        {
            width = (int)(image.Width);
            height = (int)(image.Height);
            rawStride = (width * pf.BitsPerPixel + 7) / 8;
            pixelData = new byte[rawStride * height];
        }
        void SetPixel(int x, int y, Color c, byte[] buffer, int rawStride)
        {
            int xIndex = x * 3;
            int yIndex = y * rawStride;
            buffer[xIndex + yIndex] = c.R;
            buffer[xIndex + yIndex + 1] = c.G;
            buffer[xIndex + yIndex + 2] = c.B;
        }
        void Render()
        {
            int iterMax = 800;
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    double a = (double)(col - (width / 2.0)) * (double)(4.0 / width);
                    double b = (double)(row - (height / 2.0)) * (double)(4.0 / width);
                    double x = 0, y = 0;
                    int counter = 0;
                    while (x * x + y * y < 4 && counter < iterMax)
                    {
                        double newX = x * x - y * y + a;
                        y = 2 * x * y + b;
                        x = newX;
                        counter++;
                    }
                    if (counter < iterMax) SetPixel(col, row, GetColorMapping(counter), pixelData, rawStride);
                    else SetPixel(col, row, Colors.Black, pixelData, rawStride);
                }
            }
        }
        void UpdateScreen(object o, System.EventArgs e)
        {
            bitmap = BitmapSource.Create(width, height,
                96, 96, pf, null, pixelData, rawStride);
            image.Source = bitmap;
        }
        void button_Click(object sender, RoutedEventArgs e)
        {
            //string c = comboBox.Text;
            //clr = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(c);
            Init();
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
        }
    }
}
