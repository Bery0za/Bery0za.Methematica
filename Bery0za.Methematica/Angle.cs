using System;
using System.Runtime.CompilerServices;

#if DOUBLE
using Real = System.Double;
using Math = System.Math;
#else
using Real = System.Single; 
using Math = Bery0za.Methematica.MathF;
#endif

namespace Bery0za.Methematica
{
    [Serializable]
    public struct Angle : IEquatable<Angle>
    {
        public static Angle Zero => FromRadians(0);
        public static Angle HaflPi => FromRadians(HALF_PI);
        public static Angle Pi => FromRadians(PI);
        public static Angle ThreeQuartersPi => FromRadians(PI + HALF_PI);
        public static Angle TwoPi => FromRadians(TWO_PI);
        
#if DOUBLE
        public const Real PI = System.Math.PI;
#else
        public const Real PI = (float)System.Math.PI;
#endif

        public const Real HALF_PI = PI / 2;
        public const Real TWO_PI = PI * 2;
        private const Real DEG_IN_RAD = 360 / TWO_PI;
        private const Real RAD_IN_DEG = TWO_PI / 360;
        
        public readonly Real Radians;
        public readonly Real Degrees;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle FromRadians(Real radians)
        {
            return new Angle(radians, null);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle FromDegrees(Real degrees)
        {
            return new Angle(null, degrees);
        }

        private Angle(Real radians, Real? degrees)
        {
            Radians = radians;
            Degrees = radians * DEG_IN_RAD;
        }
        
        private Angle(Real? radians, Real degrees)
        {
            Radians = degrees * RAD_IN_DEG;
            Degrees = degrees;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Real Cos()
        {
            return Math.Cos(Radians);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Real Cosh()
        {
            return Math.Cosh(Radians);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Angle other)
        {
            return Radians == other.Radians;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is Angle a && Equals(a);
        }
        
        /// <summary>
        /// Clamps the angle to range [-π; π)
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Angle Clamp()
        {
            Real frac = Radians % TWO_PI;

            if (frac >= PI)
            {
                frac -= TWO_PI;
            }
            else if (frac < -PI)
            {
                frac += TWO_PI;
            }

            return FromRadians(frac);
        }
        
        /// <summary>
        /// Removes all of full turns (360° or 2π) from the angle
        /// </summary>
        /// <param name="strictlyPositive">Result angle will be in range [0, 2π) if true, (-2π, 2π) if false</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Angle Frac(bool strictlyPositive = false)
        {
            Real frac = Radians % TWO_PI;

            if (strictlyPositive && frac < 0)
            {
                frac = TWO_PI - Math.Abs(frac);
            }

            return FromRadians(frac);
        }
        
        public override int GetHashCode()
        {
            return Radians.GetHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Real Sin()
        {
            return Math.Sin(Radians);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Real Sinh()
        {
            return Math.Sinh(Radians);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Real Tan()
        {
            return Math.Tan(Radians);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Real Tanh()
        {
            return Math.Tanh(Radians);
        }
        
        public override string ToString()
        {
            return $"\u2220 {Radians} rad, {Degrees}°";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle Acos(Real radians)
        {
            return FromRadians(Math.Acos(radians));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle Asin(Real radians)
        {
            return FromRadians(Math.Asin(radians));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle Atan(Real radians)
        {
            return FromRadians(Math.Atan(radians));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle Atan2(Real y, Real x)
        {
            return FromRadians(Math.Atan2(y, x));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle operator -(Angle a)
        {
            return FromRadians(-a.Radians);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle operator +(Angle a, Angle b)
        {
            return FromRadians(a.Radians + b.Radians);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle operator -(Angle a, Angle b)
        {
            return FromRadians(a.Radians - b.Radians);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle operator *(Angle a, Real b)
        {
            return FromRadians(a.Radians * b);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle operator *(Real a, Angle b)
        {
            return FromRadians(a * b.Radians);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle operator /(Angle a, Real b)
        {
            return FromRadians(a.Radians / b);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Angle a, Angle b)
        {
            return a.Equals(b);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Angle a, Angle b)
        {
            return !a.Equals(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Angle a, Angle b)
        {
            return a.Radians > b.Radians;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Angle a, Angle b)
        {
            return a.Radians >= b.Radians;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Angle a, Angle b)
        {
            return a.Radians < b.Radians;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Angle a, Angle b)
        {
            return a.Radians <= b.Radians;
        }
    }
}
