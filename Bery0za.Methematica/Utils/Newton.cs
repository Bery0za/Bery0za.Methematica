using System;
using System.Collections.Generic;
using System.Numerics;

using Bery0za.Methematica.Extensions;

using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Bery0za.Methematica.Utils
{
    public class ZeroDeterminantException : Exception
    {

    }
    
    public class MaxIterationsException : Exception
    {
        public Array FoundSolution { get; }

        public MaxIterationsException(Array foundSolution)
        {
            FoundSolution = foundSolution;
        }
    }

    public static class Newton
    {
        public static T[] FindRoot<T>(Func<T[], T[]> f, Func<T[], T[][]> g, T[] initialGuess,
                                      Func<T, bool> zeroComparer, Func<IEnumerable<T>, double, bool> deltaZeroComparer,
                                      double epsilon = 1e-8, int maxIteraions = 25)
            where T : struct, IEquatable<T>, IFormattable
        {
            Vector<T> x = Vector<T>.Build.DenseOfArray(initialGuess);
            int count = maxIteraions;

            while (count > 0)
            {
                Vector<T> F = Vector<T>.Build.DenseOfArray(f(x.Storage.AsArray()));
                Matrix<T> J = Matrix<T>.Build.DenseOfRowArrays(g(x.Storage.AsArray()));

                if (zeroComparer(J.Determinant()))
                {
                    throw new ZeroDeterminantException();
                }

                Vector<T> delta = J.Solve(F);
                x -= delta;

                if (deltaZeroComparer(delta, epsilon))
                {
                    return x.Storage.AsArray();
                }

                count--;
            }

            throw new MaxIterationsException(x.Storage.AsArray());
        }
        
        public static float[] FindRoot(Func<float[], float[]> f, Func<float[], float[][]> g, float[] initialGuess,
                                         float epsilon = 1e-4f, int maxIteraions = 25)
        {
            return FindRoot(f, g, initialGuess,
                             d => d.AlmostEqualRelative(0, epsilon), Extent.AlmostZeroDelta,
                             epsilon, maxIteraions);
        }
        
        public static double[] FindRoot(Func<double[], double[]> f, Func<double[], double[][]> g, double[] initialGuess,
                                          double epsilon = 1e-8, int maxIteraions = 25)
        {
            return FindRoot(f, g, initialGuess,
                             d => d.AlmostEqualRelative(0, epsilon), Extent.AlmostZeroDelta,
                             epsilon, maxIteraions);
        }

        public static Complex[] FindRoot(Func<Complex[], Complex[]> f, Func<Complex[], Complex[][]> g, Complex[] initialGuess,
                                         double epsilon = 1e-8, int maxIteraions = 25)
        {
            return FindRoot(f, g, initialGuess,
                             d => d.AlmostEqualRelative(0, epsilon), Extent.AlmostZeroDelta,
                             epsilon, maxIteraions);
        }
        
        public static bool TryFindRoot<T>(Func<T[], T[]> f, Func<T[], T[][]> g, T[] initialGuess,
                                          Func<T, bool> determinantZeroComparer, Func<IEnumerable<T>, double, bool> deltaZeroComparer,
                                          out T[] root,
                                          double epsilon = 1e-8, int maxIteraions = 25)
            where T : struct, IEquatable<T>, IFormattable
        {
            try
            {
                root = FindRoot(f, g, initialGuess, determinantZeroComparer, deltaZeroComparer, epsilon, maxIteraions);
                return true;
            }
            catch
            {
                root = null;
                return false;
            }
        }
        
        public static bool TryFindRoot(Func<float[], float[]> f, Func<float[], float[][]> g, float[] initialGuess,
                                       out float[] root, float epsilon = 1e-4f, int maxIteraions = 25)
        {
            return TryFindRoot(f, g, initialGuess,
                               d => d.AlmostEqualRelative(0, epsilon), Extent.AlmostZeroDelta,
                               out root, epsilon, maxIteraions);
        }
        
        public static bool TryFindRoot(Func<double[], double[]> f, Func<double[], double[][]> g, double[] initialGuess,
                                       out double[] root, double epsilon = 1e-8, int maxIteraions = 25)
        {
            return TryFindRoot(f, g, initialGuess,
                               d => d.AlmostEqualRelative(0, epsilon), Extent.AlmostZeroDelta,
                               out root, epsilon, maxIteraions);
        }
        
        public static bool TryFindRoot(Func<Complex[], Complex[]> f, Func<Complex[], Complex[][]> g, Complex[] initialGuess,
                                       out Complex[] root, double epsilon = 1e-8, int maxIteraions = 25)
        {
            return TryFindRoot(f, g, initialGuess,
                               d => d.AlmostEqualRelative(0, epsilon), Extent.AlmostZeroDelta,
                               out root, epsilon, maxIteraions);
        }
    }
}
