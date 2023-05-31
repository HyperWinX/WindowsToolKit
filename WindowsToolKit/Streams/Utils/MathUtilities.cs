using System;
namespace WindowsToolKit.Streams
{
    public static class MathUtilities
    {
        public static long RoundUp(long value, long unit)
        {
            return (value + (unit - 1)) / unit * unit;
        }
        public static int RoundUp(int value, int unit)
        {
            return (value + (unit - 1)) / unit * unit;
        }
        public static long RoundDown(long value, long unit)
        {
            return value / unit * unit;
        }
        public static int Ceil(int numerator, int denominator)
        {
            return (numerator + (denominator - 1)) / denominator;
        }
        public static uint Ceil(uint numerator, uint denominator)
        {
            return (numerator + (denominator - 1)) / denominator;
        }
        public static long Ceil(long numerator, long denominator)
        {
            return (numerator + (denominator - 1)) / denominator;
        }
        public static int Log2(uint val)
        {
            if (val == 0)
                throw new ArgumentException("Cannot calculate log of Zero", nameof(val));
            int result = 0;
            while ((val & 1) != 1)
            {
                val >>= 1;
                ++result;
            }
            if (val == 1)
                return result;
            throw new ArgumentException("Input is not a power of Two", nameof(val));
        }
        public static int Log2(int val)
        {
            if (val == 0)
            {
                throw new ArgumentException("Cannot calculate log of Zero", nameof(val));
            }

            int result = 0;
            while ((val & 1) != 1)
            {
                val >>= 1;
                ++result;
            }

            if (val == 1)
            {
                return result;
            }
            throw new ArgumentException("Input is not a power of Two", nameof(val));
        }
    }

}