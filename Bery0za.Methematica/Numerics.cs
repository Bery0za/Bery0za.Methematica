#if DOUBLE
using Real = System.Double;
#else
using Real = System.Single;

#endif

namespace Bery0za.Methematica
{
    public static class Numerics
    {
#if DOUBLE
        public const Real EPSILON = 1e-8f;
        public const Real MACHINE_EPSILON = double.Epsilon;
        public const Real TRIGONOMETRY_EPSILON = 1e-10f;
        public const Real JACOBIAN_STEP = 0.0001;
#else
        public const Real EPSILON = 1e-4f;
        public const Real MACHINE_EPSILON = float.Epsilon;
        public const Real TRIGONOMETRY_EPSILON = 1e-5f;
        public const Real JACOBIAN_STEP = 0.0001f;
#endif
    }
}