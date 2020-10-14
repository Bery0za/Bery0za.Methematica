using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

using Bery0za.Methematica.Extensions;

using static Bery0za.Methematica.Helpers;

using MathNet.Numerics;
#if DOUBLE
using Real = System.Double;
using Math = System.Math;
#else
using Real = System.Single;
using Math = Bery0za.Methematica.MathF;

#endif

namespace Bery0za.Methematica
{
    public class Polynomial
    {
        private static List<int[]> _binomialLut = new List<int[]>();

        public Complex[] P { get; }
        public int Order => P.Length - 1;

        public Polynomial(params Complex[] p)
        {
            P = p ?? new Complex[] { 0 };
        }

        public Polynomial(params Real[] p)
            : this(Array.ConvertAll(p, v => new Complex(v, 0))) { }

        public static Polynomial FromBernstein(params Complex[] bernsteinWeights)
        {
            return new Polynomial(BernsteinToPower(bernsteinWeights));
        }

        public static Polynomial FromBernstein(params Real[] bernsteinWeights)
        {
            return FromBernstein(Array.ConvertAll(bernsteinWeights, v => new Complex(v, 0)));
        }

        public static int BinomialCoef(int n, int k)
        {
            while (n >= _binomialLut.Count)
            {
                int s = _binomialLut.Count;
                int[] nextRow = new int[s + 1];

                nextRow[0] = 1;

                for (int i = 1, prev = s - 1; i <= prev; i++)
                {
                    nextRow[i] = _binomialLut[prev][i - 1] + _binomialLut[prev][i];
                }

                nextRow[s] = 1;

                _binomialLut.Add(nextRow);
            }

            return _binomialLut[n][k];
        }

        public static Func<Real, Real> BernsteinCoef(int n, int k)
        {
            int nk = n - k;

            return t => Math.Pow(1 - t, nk) * Math.Pow(t, k);
        }

        public static Real BernsteinCoef(int n, int k, Real t)
        {
            return Math.Pow(1 - t, n - k) * Math.Pow(t, k);
        }

        private static Complex[] BernsteinToPower(Complex[] bCoefs)
        {
            int n = bCoefs.Length - 1;
            Complex[] pCoefs = PCoefficients(bCoefs);

            for (int i = 0; i < bCoefs.Length; i++)
            {
                pCoefs[i] *= BinomialCoef(n, i);
            }

            return pCoefs;
        }

        private static Complex[] PCoefficients(Complex[] bCoefs)
        {
            Stack<Complex> pCoefs = new Stack<Complex>();
            List<Complex> table = bCoefs.ToList();

            for (int i = 0; i < bCoefs.Length; i++)
            {
                pCoefs.Push(table[0]);

                List<Complex> nextTable = new List<Complex>();

                for (int j = 0; j < table.Count - 1; j++)
                {
                    nextTable.Add(table[j + 1] - table[j]);
                }

                table = nextTable;
            }

            return pCoefs.ToArray();
        }

        public Real Evaluate(Real x)
        {
            return ToReal(Evaluate(new Complex(x, 0)).Real);
        }

        public Complex Evaluate(Complex x)
        {
            Complex result = 0;

            for (int i = 0; i < P.Length; i++)
            {
                result = result * x + P[i];
            }

            return result;
        }

        public Polynomial Derivative()
        {
            int order = P.Length - 1;
            Complex[] d = new Complex[order];

            for (int i = 0; i < order; i++)
            {
                d[i] = (order - i) * P[i];
            }

            return new Polynomial(d);
        }

        public Polynomial Monic()
        {
            Complex[] p = new Complex[P.Length];
            Array.Copy(P, p, P.Length);

            Complex leading = p[0];

            if (!(p[0] == Complex.One))
            {
                for (int i = 0; i < p.Length; i++)
                {
                    p[i] = p[i] / leading;
                }
            }

            return new Polynomial(p);
        }

        public IEnumerable<Real> RealRoots(Real epsilon = Numerics.EPSILON, int maxIterations = 25)
        {
            return Roots(epsilon, maxIterations)
                   .Where(r => r.Imaginary.AlmostEqualRelative(0, epsilon))
                   .Select(r => ToReal(r.Real));
        }

        public Complex[] Roots(Real epsilon = Numerics.EPSILON, int maxIterations = 25)
        {
            Complex[] a0 = new Complex[P.Length - 1];
            Complex[] a1 = new Complex[P.Length - 1];
            Polynomial monic = Monic();

            // Initialize a0
            Complex result = new Complex(0.4, 0.9);
            a0[0] = Complex.One;

            for (int i = 1; i < a0.Length; i++)
            {
                a0[i] = a0[i - 1] * result;
            }

            // Iterate
            int count = 0;

            while (true)
            {
                for (int i = 0; i < a0.Length; i++)
                {
                    result = Complex.One;

                    for (int j = 0; j < a0.Length; j++)
                    {
                        if (i != j)
                        {
                            result = (a0[i] - a0[j]) * result;
                        }
                    }

                    a1[i] = a0[i] - monic.Evaluate(a0[i]) / result;
                }

                count++;

                if (count > maxIterations || a0.EqualTo(a1, epsilon))
                {
                    break;
                }

                Array.Copy(a1, 0, a0, 0, a1.Length); // a0 := a1
            }

            return a1;
        }

        public (Polynomial quotent, Complex remainder, Complex factor) FactorDivision(Complex factorValue)
        {
            int count = P.Length;
            int zerosCount = ZerosCount();
            int order = count - zerosCount;

            if (order == 0)
            {
                return (new Polynomial(0), 0, factorValue);
            }

            if (order == 1)
            {
                return (new Polynomial(0, 0), P[count - 1], factorValue);
            }

            Complex[] res = new Complex[count - 1];
            Complex[] sub = new Complex[count];
            Array.Copy(P, sub, P.Length);

            for (int i = 0; i < count - 1; i++)
            {
                Complex[] mult = Enumerable.Repeat(Complex.Zero, count).ToArray();

                res[i] = sub[i];
                mult[i] = sub[i];
                mult[i + 1] = sub[i] * factorValue;

                sub = sub.Subtract(mult);
            }

            return (new Polynomial(res), sub.Last(), factorValue);
        }

        public int ZerosCount()
        {
            int count = 0;

            for (int i = 0; i < P.Length; i++)
            {
                if (P[i].AlmostEqualRelative(0))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count;
        }

        public (int idx, Complex value) FirstNonZero()
        {
            int zerosCount = ZerosCount();

            if (zerosCount >= P.Length)
            {
                return (-1, 0);
            }

            return (zerosCount, P[zerosCount]);
        }

        public override string ToString()
        {
            StringBuilder sb = Order == 0
                ? new StringBuilder($"Polynomial: {P[0]}.")
                : new StringBuilder($"Polynomial: {P[0]}x^{Order}.");

            for (int i = 1; i < Order; i++)
            {
                sb.Append(P[i].Real < Complex.Zero.Real ? $"-{P[i]}x^{i}" : $"+{P[i]}x^{i}");
            }

            return sb.ToString();
        }

        public static Polynomial operator +(Polynomial term, Polynomial addend)
        {
            bool termIsLongest = term.P.Length > addend.P.Length;
            Complex[] longest = termIsLongest ? term.P : addend.P;
            Complex[] shortest = termIsLongest ? addend.P : term.P;

            int diff = longest.Length - shortest.Length;

            Complex[] sum = new Complex[longest.Length];
            Array.Copy(longest, sum, longest.Length);

            for (int i = shortest.Length - 1; i >= 0; i--)
            {
                sum[diff + i] += shortest[i];
            }

            return new Polynomial(sum);
        }

        public static Polynomial operator -(Polynomial negatable)
        {
            return new Polynomial(negatable.P.Select(v => -v).ToArray());
        }

        public static Polynomial operator -(Polynomial subtrahend, Polynomial minuend)
        {
            return subtrahend + (-minuend);
        }

        public static Polynomial operator *(Polynomial multiplicand, Polynomial multiplier)
        {
            int lastIndex = multiplier.P.Length - 1;
            Complex last = multiplier.P[lastIndex];
            List<Complex> res = multiplicand.P.Select(p => p * last).ToList();

            for (int i = 1; i < multiplier.P.Length; i++)
            {
                List<Complex> t = multiplicand.P.Select(r => r * multiplier.P[lastIndex - i]).ToList();
                int s = res.Count - i;
                int q = t.Count - s;

                for (int j = s - 1; j >= 0; j--)
                {
                    res[j] += t[q + j];
                }

                for (int j = q - 1; j >= 0; j--)
                {
                    res.Insert(0, t[j]);
                }
            }

            return new Polynomial(res.ToArray());
        }

        public static Polynomial operator *(Polynomial multiplicand, Complex multiplier)
        {
            return new Polynomial(multiplicand.P.Select(v => v * multiplier).ToArray());
        }

        public static Polynomial operator /(Polynomial dividend, Polynomial divisor)
        {
            return Divide(dividend, divisor).quotent;
        }

        public static Polynomial operator %(Polynomial dividend, Polynomial divisor)
        {
            return Divide(dividend, divisor).remainder;
        }

        public static (Polynomial quotent, Polynomial remainder) Divide(Polynomial dividend, Polynomial divisor)
        {
            Complex[] res = new Complex[dividend.P.Length];
            Array.Copy(dividend.P, res, dividend.P.Length);

            Complex normalizer = divisor.P[0];

            for (int i = 0; i < dividend.P.Length - (divisor.P.Length - 1); i++)
            {
                res[i] /= normalizer;
                Complex coef = res[i];

                if (coef != Complex.Zero)
                {
                    for (int j = 1; j < divisor.P.Length; j++)
                    {
                        res[i + j] += -divisor.P[j] * coef;
                    }
                }
            }

            int separator = res.Length - (divisor.P.Length - 1);

            return (new Polynomial(res.Take(separator).ToArray()), new Polynomial(res.Skip(separator).ToArray()));
        }

        public static Polynomial operator /(Polynomial dividend, Complex divisor)
        {
            return new Polynomial(dividend.P.Select(v => v / divisor).ToArray());
        }
    }
}