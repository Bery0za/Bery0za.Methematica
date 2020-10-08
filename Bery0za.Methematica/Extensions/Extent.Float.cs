using System;
using System.Collections.Generic;
using System.Linq;

using MathNet.Numerics;

namespace Bery0za.Methematica.Extensions
{
    internal static partial class Extent
    {
        public static bool AlmostZeroDelta(this IEnumerable<float> deltaArray, double maximumError)
        {
            return Math.Sqrt(deltaArray.Sum(v => v * v)).AlmostEqualRelative(0, maximumError);
        }
    }
}