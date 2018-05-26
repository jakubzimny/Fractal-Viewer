using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractal_Viewer
{
    class Complex
    {
        private double re { get; set; } //real part
        private double im { get; set; } // imaginary part

        public Complex(double r, double i)
        {
            re = r;
            im = i;
        }

        public void Square()
        {
            double temp = re * re - im * im;
            im = 2.0 * re * im;
            re = temp;
        }

        public double Magnitude()
        {
            return Math.Sqrt(re*re + im*im);
        }

        public void Add(Complex number)
        {
            re += number.re;
            im += number.im;
        }
    }
}
