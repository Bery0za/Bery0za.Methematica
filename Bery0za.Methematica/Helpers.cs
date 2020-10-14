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
    public static class Helpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Real ToReal(double real)
        {
            return (Real)real;
        }
    }
}