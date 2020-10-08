using System;

namespace Bery0za.Methematica.Utils
{
    public static class Mapping
    {
        public static (double x, double y) CircleToSquare(double u, double v)
        {
            double tst = 2 * Math.Sqrt(2);
            double usq = u * u;
            double vsq = v * v;

            double x = 0.5 * (Math.Sqrt(2 + tst * u + usq - vsq) - Math.Sqrt(2 - tst * u + usq - vsq));
            double y = 0.5 * (Math.Sqrt(2 + tst * v + vsq - usq) - Math.Sqrt(2 - tst * v + vsq - usq));

            return (x, y);
        }

        public static (double u, double v) SquareToCircle(double x, double y)
        {
            double u = x * Math.Sqrt(1 - 0.5 * y * y);
            double v = y * Math.Sqrt(1 - 0.5 * x * x);

            return (u, v);
        }
    }
}
