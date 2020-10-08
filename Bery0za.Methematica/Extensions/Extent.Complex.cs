using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using MathNet.Numerics;

namespace Bery0za.Methematica.Extensions
{
    internal static partial class Extent
    {       
        public static Complex[] Subtract(this Complex[] thisArr, Complex[] otherArr)
        {
             if (thisArr.Length != otherArr.Length) throw new ArgumentException("Arrays of various length!");

            return thisArr.Select((v, i) => v - otherArr[i]).ToArray();
        }

        public static bool EqualTo(this Complex[] thisArr, Complex[] otherArr, double epsilon)
        {
            if (thisArr.Length != otherArr.Length) throw new ArgumentException("Arrays of various length!");

            bool compared = true;

            for (int i = 0; i < thisArr.Length; i++)
            {
                Complex delta = thisArr[i] - otherArr[i];
                compared &= delta.AlmostEqualRelative(0, epsilon);
            }

            return compared;
        }

        public static bool AlmostZeroDelta(this IEnumerable<Complex> deltaArray, double maximumError)
        {
            return deltaArray.Aggregate(Complex.Zero, (acc, v) => acc += v * v).SquareRoot().AlmostEqualRelative(0, maximumError);
        }
    }
}