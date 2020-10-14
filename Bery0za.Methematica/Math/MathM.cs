using System;

namespace Bery0za.Methematica
{
    public static class MathM
    {
        public const decimal PI = 3.14159265358979323846264338327950288419716939937510M;
        public const decimal E = 2.7182818284590452353602874713526624977572470936999595749M;

        private const decimal TWO_PI = 6.28318530717958647692528676655900576839433879875021M;
        private const decimal HALF_PI = 1.570796326794896619231321691639751442098584699687552910487M;
        private const decimal QUARTER_PI = 0.785398163397448309615660845819875721049292349843776455243M;
        private const decimal E_INV = 0.3678794411714423215955237701614608674458111310317678M;
        private const decimal LOG_10_INV = 0.434294481903251827651128918916605082294397005803666566114M;

        private const decimal HALF = 0.5M;
        private const decimal ONE = 1M;
        private const decimal ZERO = 0M;

        public static decimal Abs(decimal x)
        {
            if (x <= ZERO) return -x;

            return x;
        }

        public static decimal Acos(decimal x)
        {
            if (x == ZERO) return HALF_PI;
            if (x == ONE) return ZERO;
            if (x < ZERO) return PI - Acos(-x);

            return HALF_PI - Asin(x);
        }

        public static decimal Asin(decimal x)
        {
            if (x > ONE || x < -ONE)
            {
                throw new ArgumentException("x must be in [-1,1]");
            }

            // Known values
            if (x == ZERO) return ZERO;
            if (x == ONE) return HALF_PI;

            // Asin function is an odd function
            if (x < ZERO) return -Asin(-x);

            // My optimize trick here

            // Used a math formula to speed up:
            // asin(x) = 0.5 * (PI / 2 - asin (1 - 2 * x * x)) 
            // if x>=0 is true

            var newX = ONE - 2 * x * x;

            // For calculating a new value closer to zero than current
            // because we gain more speed with values close to zero
            if (Abs(x) > Abs(newX))
            {
                var t = Asin(newX);
                return HALF * (HALF_PI - t);
            }

            var y = ZERO;
            var result = x;
            decimal cachedResult;
            var i = 1;
            y += result;
            var xx = x * x;

            do
            {
                cachedResult = result;
                result *= xx * (ONE - HALF / (i));
                y += result / (2 * i + 1);
                i++;
            }
            while (cachedResult != result);

            return y;
        }

        public static decimal Atan(decimal x)
        {
            if (x == ZERO) return ZERO;
            if (x == ONE) return QUARTER_PI;

            return Asin(x / Sqrt(ONE + x * x));
        }

        public static decimal Atan2(decimal y, decimal x)
        {
            if (x > ZERO) return Atan(y / x);
            if (x < ZERO && y >= ZERO) return Atan(y / x) + PI;
            if (x < ZERO && y < ZERO) return Atan(y / x) - PI;
            if (x == ZERO && y > ZERO) return HALF_PI;
            if (x == ZERO && y < ZERO) return -HALF_PI;

            throw new ArgumentException("invalid atan2 arguments");
        }

        public static decimal Cos(decimal x)
        {
            while (x > TWO_PI)
            {
                x -= TWO_PI;
            }

            while (x < -TWO_PI)
            {
                x += TWO_PI;
            }

            // Now x is in (-2 * PI; 2 * PI)
            if (x >= PI && x <= TWO_PI)
            {
                return -Cos(x - PI);
            }

            if (x >= -TWO_PI && x <= -PI)
            {
                return -Cos(x + PI);
            }

            x = x * x;

            // y = 1 - x/2! + x^2/4! - x^3/6!...
            var xx = -x * HALF;
            var y = ONE + xx;
            var cachedY = y - ONE; // Init cache with different value

            for (var i = 1; cachedY != y; i++)
            {
                cachedY = y;
                decimal factor = i * (i + i + 3) + 1; // 2i^2 + 2i + i + 1 = 2i^2 + 3i + 1
                factor = -HALF / factor;
                xx *= x * factor;
                y += xx;
            }

            return y;
        }

        public static decimal Cosh(decimal x)
        {
            var y = Exp(x);
            var yy = ONE / y;
            return (y + yy) * HALF;
        }

        public static decimal Exp(decimal x)
        {
            var count = 0;

            while (x > ONE)
            {
                x--;
                count++;
            }

            while (x < ZERO)
            {
                x++;
                count--;
            }

            var iteration = 1;
            var result = ONE;
            var factorial = ONE;
            decimal cachedResult;

            do
            {
                cachedResult = result;
                factorial *= x / iteration++;
                result += factorial;
            }
            while (cachedResult != result);

            if (count != 0) result = result * PowN(E, count);

            return result;
        }

        public static decimal Log(decimal x)
        {
            if (x <= ZERO)
            {
                throw new ArgumentException("x must be greater than zero");
            }

            var count = 0;

            while (x >= ONE)
            {
                x *= E_INV;
                count++;
            }

            while (x <= E_INV)
            {
                x *= E;
                count--;
            }

            x--;
            if (x == 0) return count;

            var result = ZERO;
            var iteration = 0;
            var y = ONE;
            var cacheResult = result - ONE;

            while (cacheResult != result)
            {
                iteration++;
                cacheResult = result;
                y *= -x;
                result += y / iteration;
            }

            return count - result;
        }

        public static decimal Log10(decimal x)
        {
            return Log(x) * LOG_10_INV;
        }

        public static decimal Pow(decimal x, decimal y)
        {
            return Exp(y * Log(x));
        }

        public static decimal PowN(decimal x, int y)
        {
            if (y == ZERO) return ONE;
            if (y < ZERO) return PowN(ONE / x, -y);

            decimal @base = x;
            int power = y;
            decimal result = ONE;

            while (power > 0)
            {
                if (power % 2 == 1) result *= @base;
                power >>= 1;
                @base *= @base;
            }

            return result;
        }

        public static decimal Root(decimal x, int n)
        {
            if (n < ZERO) throw new OverflowException("Cannot calculate root from a negative number");

            // Initial approximation
            decimal current = (decimal)Math.Pow((double)x, 1d / n), previous;

            do
            {
                previous = current;
                if (previous == ZERO) return ZERO;

                current = 1m / n * ((n - 1) * previous + x / PowN(previous, n - 1));
            }
            while (previous != current);

            return current;
        }

        public static int Sign(decimal x)
        {
            return x < ZERO ? -1 : (x > ZERO ? 1 : 0);
        }

        public static decimal Sin(decimal x)
        {
            var cos = Cos(x);
            var moduleOfSin = Sqrt(ONE - (cos * cos));
            var sineIsPositive = IsSignOfSinePositive(x);

            if (sineIsPositive) return moduleOfSin;

            return -moduleOfSin;
        }

        public static decimal Sinh(decimal x)
        {
            var y = Exp(x);
            var yy = ONE / y;

            return (y - yy) * HALF;
        }

        public static decimal Sqrt(decimal x)
        {
            if (x < ZERO) throw new OverflowException("Cannot calculate square root from a negative number");

            // Initial approximation
            decimal current = (decimal)Math.Sqrt((double)x), previous;

            do
            {
                previous = current;
                if (previous == ZERO) return ZERO;

                current = (previous + x / previous) * HALF;
            }
            while (previous != current);

            return current;
        }

        public static decimal Tan(decimal x)
        {
            var cos = Cos(x);
            if (cos == ZERO) throw new ArgumentException(nameof(x));

            return Sin(x) / cos;
        }

        public static decimal Tanh(decimal x)
        {
            var y = Exp(x);
            var yy = ONE / y;

            return (y - yy) / (y + yy);
        }

        private static bool IsSignOfSinePositive(decimal x)
        {
            // Еruncating to  [-2 * PI; 2 * PI]
            while (x >= TWO_PI)
            {
                x -= TWO_PI;
            }

            while (x <= -TWO_PI)
            {
                x += TWO_PI;
            }

            // Now x is in [-2 * PI; 2 * PI]
            if (x >= -TWO_PI && x <= -PI) return true;
            if (x >= -PI && x <= ZERO) return false;
            if (x >= ZERO && x <= PI) return true;
            if (x >= PI && x <= TWO_PI) return false;

            // Can not be reached
            throw new ArgumentException(nameof(x));
        }
    }
}