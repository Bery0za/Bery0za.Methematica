using System;
using System.Collections.Generic;
using System.Linq;

namespace Bery0za.Methematica.Utils
{
    public static class Cantor
    {
        public static ulong ZtoN(int z)
        {
            return ZtoN((long)z);
        }

        public static ulong ZtoN(long z)
        {
            return checked(z < 0 ? (ulong)(-z * 2 - 1) : (ulong)(z * 2));
        }

        public static ulong Pairing(int x, int y)
        {
            return Pairing(ZtoN(x), ZtoN(y));
        }

        public static ulong Pairing(ulong x, ulong y)
        {
            return checked((ulong)(0.5 * (x + y) * (x + y + 1) + y));
        }

        public static ulong Tuple(IEnumerable<ulong> nums)
        {
            var numList = nums as IList<ulong> ?? nums.ToList();

            if (numList.Count < 2)
            {
                throw new ArgumentException("At least two numbers needed.");
            }
            
            if (numList.Count == 2)
            {
                return Pairing(numList.First(), numList.Last());
            }

            return Pairing(Tuple(numList.Take(numList.Count - 1)), numList.Last());
        }

        public static ulong Tuple(params int[] nums)
        {
            return Tuple(nums.Select(ZtoN));
        }
    }
}
