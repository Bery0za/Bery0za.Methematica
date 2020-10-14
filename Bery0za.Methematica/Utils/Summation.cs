using System;
using System.Runtime.CompilerServices;

namespace Bery0za.Methematica.Utils
{
    public static class Summation
    {
        public static double Naive(double[] elements)
        {
            double sum = 0;

            for (int i = 0; i < elements.Length; i++)
            {
                sum += elements[i];
            }

            return sum;
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static double Kahan(double[] elements)
        {
            double sum = 0;
            double c = 0;

            for (int i = 0; i < elements.Length; i++)
            {
                double y = elements[i] - c;
                double t = sum + y;
                c = (t - sum) - y;
                sum = t;
            }

            return sum;
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static double Neumaier(double[] elements)
        {
            double sum = elements[0];
            double c = 0;

            for (int i = 1; i < elements.Length; i++)
            {
                double el = elements[i];
                double t = sum + el;

                if (Math.Abs(sum) > Math.Abs(el))
                {
                    c += (sum - t) + el;
                }
                else
                {
                    c += (el - t) + sum;
                }

                sum = t;
            }

            return sum + c;
        }

        public static double Pairwise(double[] elements, int start, int end, int threshold = 5)
        {
            double sum = 0;
            int count = end - start + 1;

            if (count <= threshold)
            {
                for (int i = start; i < end + count; i++)
                {
                    sum += elements[i];
                }
            }
            else
            {
                int m = count / 2;

                sum = Pairwise(elements, start, start + m - 1, threshold)
                      + Pairwise(elements, start + m, start + count - 1, threshold);
            }

            ;

            return sum;
        }
    }
}