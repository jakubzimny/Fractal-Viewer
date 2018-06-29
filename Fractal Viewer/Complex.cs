using System;

namespace Fractal_Viewer
{
    class Complex
    {
        public double re { get; private set; } //real part
        public double im { get; private set; } // imaginary part

        public Complex(double r, double i)
        {
            re = r;
            im = i;
        }
        public void Multiply(Complex number)
        {
            double temp = re * number.re - im * number.im;
            im = im * number.re + re * number.im;
            re = temp;
        }

        public void Square()
        {
            this.Multiply(this);
        }

        public double MagnitudeSquared()
        {
            return re*re + im*im;
        }

        public void Abs()
        {
            im = Math.Abs(im);
            re = Math.Abs(re);
        }

        public void Add(Complex number)
        {
            re += number.re;
            im += number.im;
        }
 
    }
}
