using System;
using System.Linq;

using MathNet.Numerics;

using static Bery0za.Methematica.Helpers;
#if DOUBLE
using Real = System.Double;
using Math = System.Math;
#else
using Real = System.Single;
using Math = Bery0za.Methematica.MathF;

#endif

namespace Bery0za.Methematica.Utils
{
    public static class Solve
    {
        public static bool Linear(Real a, Real b, Real epsilon, out Real root)
        {
            root = -b / a;
            return !a.AlmostEqualRelative(0, epsilon);
        }

        public static bool QuadraticReal(Real a, Real b, Real c, Real epsilon, out Real[] roots)
        {
            Real x1 = Real.PositiveInfinity;
            Real x2 = Real.PositiveInfinity;

            if (a.AlmostEqualRelative(0, epsilon))
            {
                // This could just be a linear equation
                if (b.AlmostEqualRelative(0, epsilon))
                {
                    bool cIsZero = c.AlmostEqualRelative(0, epsilon);
                    roots = cIsZero ? new Real[0] : new Real[] { 0 };

                    return !cIsZero;
                }

                if (Linear(b, c, epsilon, out Real root))
                {
                    roots = new[] { root };
                    return true;
                }

                roots = null;
                return false;
            }

            // a, b, c are expected to be the coefficients of the equation:
            // Ax² - 2Bx + C == 0, so we take b = -b/2:
            b *= ToReal(-0.5);
            Real D = GetDiscriminant(a, b, c);

            // If the discriminant is very small, we can try to normalize
            // the coefficients, so that we may get better accuracy.
            if (D.AlmostEqualRelative(0, epsilon))
            {
                var f = GetNormalizationFactor(Math.Abs(a), Math.Abs(b), Math.Abs(c));

                if (f != 0)
                {
                    a *= f;
                    b *= f;
                    c *= f;
                    D = GetDiscriminant(a, b, c);
                }
            }

            if (D >= 0) // No real roots if D < 0
            {
                Real Q = D < 0 ? 0 : Math.Sqrt(D);
                Real R = b + (b < 0 ? -Q : Q);

                // Try to minimize Realing point noise.
                if (R == 0)
                {
                    x1 = c / a;
                    x2 = -x1;
                }
                else
                {
                    x1 = R / a;
                    x2 = c / R;
                }
            }

            bool exist = false;
            roots = null;

            // We need to include EPSILON in the comparisons with min / max,
            // as some solutions are ever so lightly out of bounds.
            if (!Real.IsInfinity(x1))
            {
                roots = new[] { x1 };
                exist = true;

                if (x2 != x1 && !Real.IsInfinity(x2))
                {
                    roots = new[] { x1, x2 };
                }
            }

            return exist;
        }

        // d = b^2 - a * c  computed accurately enough by a tricky scheme.
        // Ported from @hkrish's polysolve.c
        private static Real GetDiscriminant(Real a, Real b, Real c)
        {
            Func<Real, Real[]> split = (v) =>
            {
                Real x = v * 134217729;
                Real y = v - x;
                Real hi = y + x; // Don't optimize y away!
                Real lo = v - hi;
                return new[] { hi, lo };
            };

            Real D = b * b - a * c;
            Real E = b * b + a * c;

            if (Math.Abs(D) * 3 < E)
            {
                Real[] ad = split(a);
                Real[] bd = split(b);
                Real[] cd = split(c);
                Real p = b * b;
                Real dp = (bd[0] * bd[0] - p + 2 * bd[0] * bd[1]) + bd[1] * bd[1];
                Real q = a * c;
                Real dq = (ad[0] * cd[0] - q + ad[0] * cd[1] + ad[1] * cd[0]) + ad[1] * cd[1];
                D = (p - q) + (dp - dq); // Don’t omit parentheses!
            }

            return D;
        }

        public static bool CubicReal(Real a, Real b, Real c, Real d, Real epsilon, out Real[] roots)
        {
            Real f = GetNormalizationFactor(Math.Abs(a), Math.Abs(b), Math.Abs(c), Math.Abs(d));

            if (f != 0)
            {
                a *= f;
                b *= f;
                c *= f;
                d *= f;
            }

            Real x = 0, b1 = 0, c2 = 0, qd = 0, q = 0;

            Action<Real> evaluate = x0 =>
            {
                x = x0;

                // Evaluate q, q', b1 and c2 at x
                Real tmp = a * x;
                b1 = tmp + b;
                c2 = b1 * x + c;
                qd = (tmp + b1) * x + c2;
                q = c2 * x + d;
            };

            // If a or d is zero, we only need to solve a quadratic, so we set
            // the coefficients appropriately.
            if (a.AlmostEqualRelative(0, epsilon))
            {
                a = b;
                b1 = c;
                c2 = d;
                x = Real.PositiveInfinity;
            }
            else if (d.AlmostEqualRelative(0, epsilon))
            {
                b1 = b;
                c2 = c;
                x = 0;
            }
            else
            {
                // Here onwards we iterate for the leftmost root. Proceed to
                // deflate the cubic into a quadratic (as a side effect to the
                // iteration) and solve the quadratic.
                evaluate(-(b / a) / 3);

                // Get a good initial approximation.
                Real t = q / a;
                Real r = Math.Pow(Math.Abs(t), ToReal(1) / 3);
                Real s = t < 0 ? -1 : 1;
                Real td = -qd / a;

                // See Kahan's notes on why 1.324718*... works.
                Real rd = td > 0 ? (Real)(1.324717957244746 * Math.Max(r, Math.Sqrt(td))) : r;
                Real x0 = x - s * rd;

                if (x0 != x)
                {
                    do
                    {
                        evaluate(x0);
                        // Newton's. Divide by 1 + MACHINE_EPSILON (1.000...002)
                        // to avoid x0 crossing over a root.
                        x0 = qd == 0 ? x : x - q / qd / (1 + Real.Epsilon);
                    }
                    while (s * x0 > s * x);

                    // Adjust the coefficients for the quadratic.
                    if (Math.Abs(a) * x * x > Math.Abs(d / x))
                    {
                        c2 = -d / x;
                        b1 = (c2 - c) / x;
                    }
                }
            }

            // The cubic has been deflated to a quadratic.
            bool exist = QuadraticReal(a, b1, c2, epsilon, out roots);

            if (!Real.IsInfinity(x))
            {
                roots = exist ? new[] { roots[0], roots[1], x } : new[] { x };
                exist = true;
            }

            return exist;
        }

        // Normalize coefficients à la Jenkins & Traub's RPOLY.
        // Normalization is done by scaling coefficients with a power of 2, so
        // that all the bits in the mantissa remain unchanged.
        // Use the infinity norm (max(sum(abs(a)…)) to determine the appropriate
        // scale factor. See @hkrish in #1087#issuecomment-231526156
        private static Real GetNormalizationFactor(params Real[] values)
        {
            var norm = values.Max();

            return norm != 0 && (norm < 1e-8 || norm > 1e8)
                ? Math.Pow(2, -Math.Round(Math.Log(norm)))
                : 0f;
        }
    }
}