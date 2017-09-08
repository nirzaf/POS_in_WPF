using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PosModels.Helpers
{
    public static class Bit
    {
        public static uint And(string number1, string number2)
        {
            return (ToNumber(number1) & ToNumber(number2));
        }

        public static uint And(uint number1, uint number2)
        {
            return (number1 & number2);
        }

        public static uint Or(string number1, string number2)
        {
            return (ToNumber(number1) | ToNumber(number2));
        }

        public static uint Or(uint number1, uint number2)
        {
            return (number1 | number2);
        }

        public static uint XOr(string number1, string number2)
        {
            return (ToNumber(number1) ^ ToNumber(number2));
        }

        public static uint XOr(uint number1, uint number2)
        {
            return (number1 ^ number2);
        }

        public static uint Not(string number)
        {
            return (~ToNumber(number));
        }

        public static uint Not(uint number)
        {
            return (~number);
        }

        public static uint LeftShift(string number, int numberOfDigits)
        {
            return (ToNumber(number) << numberOfDigits);
        }

        public static uint LeftShift(uint number, int numberOfDigits)
        {
            return (number << numberOfDigits);
        }

        public static uint RightShift(string number, int numberOfDigits)
        {
            return (ToNumber(number) >> numberOfDigits);
        }

        public static uint RightShift(uint number, int numberOfDigits)
        {
            return (number >> numberOfDigits);
        }

        public static bool TestBit(string number, int bitPosition)
        {
            return ((ToNumber(number) & (uint)Math.Pow(2, bitPosition)) != 0);
        }

        public static bool TestBit(uint number, int bitPosition)
        {
            return ((number & (uint)Math.Pow(2, bitPosition)) != 0);
        }

        public static byte SetBit(byte mask, byte srcValue)
        {
            srcValue ^= (byte)(((byte)(-1 ^ srcValue)) & mask);
            return srcValue;
        }

        public static byte ClearBit(byte mask, byte srcValue)
        {
            srcValue ^= (byte)(((byte)(0 ^ srcValue)) & mask);
            return srcValue;
        }

        public static uint SetBit(uint mask, uint srcValue)
        {
            srcValue ^= (((uint)(-1 ^ srcValue)) & mask);
            return srcValue;
        }

        public static uint ClearBit(uint mask, uint srcValue)
        {
            srcValue ^= (((uint)(0 ^ srcValue)) & mask);
            return srcValue;
        }

        public static string ToBinary(string number)
        {
            return ToBinary(ToNumber(number));
        }

        public static string ToBinary(uint number)
        {
            try
            {
                return Convert.ToString(number, 2);
            }
            catch
            {
                return "0";
            }
        }

        public static uint ToValue(string numberString)
        {
            try
            {
                return Convert.ToUInt32(numberString, 2);
            }
            catch
            {
                return 0;
            }
        }

        public static uint ToNumber(string numberString)
        {
            try
            {
                return Convert.ToUInt32(numberString);
            }
            catch
            {
                return 0;
            }
        }
    }
}
