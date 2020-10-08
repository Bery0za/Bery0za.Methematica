using System;

#if DOUBLE
using Real = System.Double;
using Math = System.Math;
#else
using Real = System.Single; 
using Math = Bery0za.Methematica.MathF;
#endif

namespace Bery0za.Methematica.Utils
{
    public static class Polar
    {
        public static (Real r, Angle a) FromCartesian(Real x, Real y)
        {
            return (Math.Sqrt(x * x + y * y), Angle.FromRadians(Math.Atan2(y, x)));
        }

        public static (Real x, Real y) ToCartesian(Real r, Angle a)
        {
            return (r * Math.Cos(a.Radians), r * Math.Sin(a.Radians));
        }
    }
}
