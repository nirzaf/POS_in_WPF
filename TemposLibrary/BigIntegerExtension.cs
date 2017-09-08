#if !DEMO
using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace TemposLibrary
{
    /// <summary>
    /// And extention class for using code writen for Java's BigInteger, which
    /// is an unsigned BigInteger, unlike System.Numerics.BigInteger. The extension
    /// methods in this class allow the code I wrote for java to run almost unaltered.
    /// The static Create() methods are used to used to replace java's constructors,
    /// since you can't create constructor extensions.
    /// </summary>
    public static class BigIntegerExtension
    {
        #region Licensed Access Only
        static BigIntegerExtension()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(AsynchronousSocket).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use TemposLibrary.dll");
            }
#endif
        }
        #endregion

        #region Constants
        private static readonly BigInteger One = new BigInteger(1);
        private static readonly BigInteger Two = new BigInteger(2);
        private static readonly int[] primesBelow2000 = {
            2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97,
            101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199,
	        211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293,
	        307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397,
	        401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499,
	        503, 509, 521, 523, 541, 547, 557, 563, 569, 571, 577, 587, 593, 599,
	        601, 607, 613, 617, 619, 631, 641, 643, 647, 653, 659, 661, 673, 677, 683, 691,
	        701, 709, 719, 727, 733, 739, 743, 751, 757, 761, 769, 773, 787, 797,
	        809, 811, 821, 823, 827, 829, 839, 853, 857, 859, 863, 877, 881, 883, 887,
	        907, 911, 919, 929, 937, 941, 947, 953, 967, 971, 977, 983, 991, 997,
	        1009, 1013, 1019, 1021, 1031, 1033, 1039, 1049, 1051, 1061, 1063, 1069, 1087, 1091, 1093, 1097,
	        1103, 1109, 1117, 1123, 1129, 1151, 1153, 1163, 1171, 1181, 1187, 1193,
	        1201, 1213, 1217, 1223, 1229, 1231, 1237, 1249, 1259, 1277, 1279, 1283, 1289, 1291, 1297,
	        1301, 1303, 1307, 1319, 1321, 1327, 1361, 1367, 1373, 1381, 1399,
	        1409, 1423, 1427, 1429, 1433, 1439, 1447, 1451, 1453, 1459, 1471, 1481, 1483, 1487, 1489, 1493, 1499,
	        1511, 1523, 1531, 1543, 1549, 1553, 1559, 1567, 1571, 1579, 1583, 1597,
	        1601, 1607, 1609, 1613, 1619, 1621, 1627, 1637, 1657, 1663, 1667, 1669, 1693, 1697, 1699,
	        1709, 1721, 1723, 1733, 1741, 1747, 1753, 1759, 1777, 1783, 1787, 1789,
	        1801, 1811, 1823, 1831, 1847, 1861, 1867, 1871, 1873, 1877, 1879, 1889,
	        1901, 1907, 1913, 1931, 1933, 1949, 1951, 1973, 1979, 1987, 1993, 1997, 1999 };
        #endregion

        #region Additional construction functions
        public static BigInteger Create(string text, int convertBase)
        {            
            if (convertBase == 16)
            {
                bool isEvenLength = (text.Length % 2 == 0);
                int length = (text.Length / 2) + (isEvenLength ? 0 : 1);
                byte[] result = new byte[length + 1];
                for (int i = 0; i < length; i++)
                {
                    int j = text.Length - (i * 2) - 1;
                    char ch1 = '0';
                    if (j > 0)
                        ch1 = text[j - 1];
                    byte b = GetByte(ch1, text[j]);
                    result[i] = b;
                }
                result[length] = 0;
                return new BigInteger(result);
            }
            else if (convertBase == 10)
            {
                return BigInteger.Parse("0" + text, NumberStyles.AllowLeadingSign);
            }
            else
                throw new Exception("Unsupported conversion base");
        }

        public static BigInteger Create(int numberOfBits, Random random)
        {
            int numberOfFullBytes = numberOfBits / 8;
            int numberOfRemainingBits = numberOfBits % 8;
            int numberOfBytes = numberOfFullBytes + (numberOfRemainingBits > 0 ? 1 : 0);
            byte[] randomBytes = new byte[numberOfBytes];
            random.NextBytes(randomBytes);

            // Created the padded byte array
            byte[] paddedBytes = new byte[randomBytes.Length + 1];
            for (int i = 0; i < randomBytes.Length; i++)
            {
                paddedBytes[i] = randomBytes[i];
            }
            
            // Mask off unused bits
            if (numberOfRemainingBits > 0)
            {
                paddedBytes[randomBytes.Length - 1] =
                    MaskBits(paddedBytes[randomBytes.Length - 1], 8 - numberOfRemainingBits);
            }
            
            // Make unsigned
            paddedBytes[randomBytes.Length] = 0;
            BigInteger result = new BigInteger(paddedBytes);

            // Return result
            return result;
        }
        #endregion

        #region Instance extensions for math functions
        public static BigInteger modPow(this BigInteger bigInt, BigInteger exponent,
            BigInteger modulus)
        {
            return BigInteger.ModPow(bigInt, exponent, modulus);
        }

        public static BigInteger multiply(this BigInteger bigInt, BigInteger value)
        {
            return BigInteger.Multiply(bigInt, value);
        }

        public static BigInteger add(this BigInteger bigInt, BigInteger value)
        {
            return BigInteger.Add(bigInt, value);
        }

        public static BigInteger subtract(this BigInteger bigInt, BigInteger value)
        {
            return BigInteger.Subtract(bigInt, value);
        }

        public static BigInteger divide(this BigInteger bigInt, BigInteger value)
        {
            return BigInteger.Divide(bigInt, value);
        }

        public static BigInteger gcd(this BigInteger bigInt, BigInteger value)
        {
            return BigInteger.GreatestCommonDivisor(bigInt, value);
        }
        #endregion

        #region Bit masking, IsNegative, and BitCount
        private static byte MaskBits(byte b, int bitCount)
        {
            if (bitCount == 7)
                return (byte)(b & 1);
            if (bitCount == 6)
                return (byte)(b & 3);
            if (bitCount == 5)
                return (byte)(b & 7);
            if (bitCount == 4)
                return (byte)(b & 15);
            if (bitCount == 3)
                return (byte)(b & 31);
            if (bitCount == 2)
                return (byte)(b & 63);
            if (bitCount == 1)
                return (byte)(b & 127);
            throw new ArgumentException("bitCount is not from 1-7");
        }

        public static bool IsNegative(this BigInteger bigInt)
        {
            return (bigInt < BigInteger.Zero);
        }

        public static int BitCount(this BigInteger bigInt)
        {
            byte[] bytes = bigInt.ToUnsignedByteArray();
            return (bytes.Length * 8);
        }
        #endregion

        #region Public helper function (ToUnsignedByteArray)
        public static byte[] ToUnsignedByteArray(this BigInteger bigInt)
        {
            byte[] bigIntByteArray = bigInt.ToByteArray();
            if (bigIntByteArray[bigIntByteArray.Length - 1] == 0)
            {
                byte[] shortenedByteArray = new byte[bigIntByteArray.Length - 1];
                for (int i = 0; i < shortenedByteArray.Length; i++)
                {
                    shortenedByteArray[i] = bigIntByteArray[i];
                }
                bigIntByteArray = shortenedByteArray;
            }
            return bigIntByteArray;
        }
        #endregion

        #region Prime Number Testing
        public static bool RabinMillerTest(this BigInteger thisVal, int confidence)
        {
            byte[] bytes = thisVal.ToUnsignedByteArray();
            if (bytes.Length == 1)
            {
                // test small numbers
                if (bytes[0] == 0 || bytes[0] == 1)
                    return false;
                else if (bytes[0] == 2 || bytes[0] == 3)
                    return true;
            }

            if ((bytes[0] & 0x1) == 0)     // even numbers
                return false;

            // calculate values of s and t
            BigInteger p_sub1 = thisVal - (new BigInteger(1));
            byte[] p_sub1Bytes = p_sub1.ToUnsignedByteArray();
            int s = 0;

            for (int index = 0; index < p_sub1Bytes.Length; index++)
            {
                uint mask = 0x01;

                for (int i = 0; i < 32; i++)
                {
                    if ((p_sub1Bytes[index] & mask) != 0)
                    {
                        index = p_sub1Bytes.Length;      // to break the outer loop
                        break;
                    }
                    mask <<= 1;
                    s++;
                }
            }

            BigInteger t = p_sub1 >> s;

            int bits = thisVal.BitCount();
            BigInteger a = new BigInteger();
            Random rand = new Random();

            for (int round = 0; round < confidence; round++)
            {
                bool done = false;

                while (!done)		// generate a < n
                {
                    int testBits = 0;

                    // make sure "a" has at least 2 bits
                    while (testBits < 2)
                        testBits = (int)(rand.NextDouble() * bits);

                    a = Create(testBits, rand);
                    byte[] aBytes = a.ToUnsignedByteArray();

                    // make sure "a" is not 0
                    if (aBytes.Length > 1 || (aBytes.Length == 1 && aBytes[0] != 1))
                        done = true;
                }

                // check whether a factor exists (fix for version 1.03)
                BigInteger gcdTest = a.gcd(thisVal);
                byte[] gcdBytes = gcdTest.ToUnsignedByteArray();
                if (gcdBytes.Length == 1 && gcdBytes[0] != 1)
                    return false;

                BigInteger b = a.modPow(t, thisVal);
                byte[] bBytes = b.ToUnsignedByteArray();

                bool result = false;

                if (bBytes.Length == 1 && bBytes[0] == 1)         // a^t mod p = 1
                    result = true;

                for (int j = 0; result == false && j < s; j++)
                {
                    if (b == p_sub1)         // a^((2^j)*t) mod p = p-1 for some 0 <= j <= s-1
                    {
                        result = true;
                        break;
                    }

                    b = (b * b) % thisVal;
                }

                if (result == false)
                    return false;
            }
            return true;
        }

        public static bool IsProbablePrime(this BigInteger thisVal, int confidence)
        {
            // test for divisibility by primes < 2000
            for (int p = 0; p < primesBelow2000.Length; p++)
            {
                BigInteger divisor = primesBelow2000[p];

                if (divisor >= thisVal)
                    break;

                BigInteger resultNum = thisVal % divisor;
                if (resultNum == BigInteger.Zero)
                {
                    return false;
                }
            }

            return (thisVal.RabinMillerTest(confidence));
        }

        public static bool IsSafePrime(this BigInteger thisVal, int confidence)
        {
            if (!thisVal.IsProbablePrime(confidence))
                return false;
            // thisVal (q) is safe prime if in q = (2p - 1), p is also a probable prime
            // p = (q - 1) / 2
            BigInteger test = thisVal.subtract(1).divide(2);
            return test.IsProbablePrime(confidence);
        }

        public static BigInteger GeneratePseudoPrime(int bits, int confidence, Random rand)
        {
            BigInteger result = new BigInteger();
            bool done = false;

            while (!done)
            {
                result = Create(bits, rand);
                if (BigInteger.Remainder(result, Two) == BigInteger.Zero)
                    result++;
                // prime test
                done = result.IsProbablePrime(confidence);
            }
            return result;
        }

        public static BigInteger GenerateSafePrime(int bits, int confidence, Random rand)
        {
            BigInteger pseudoPrime = GeneratePseudoPrime(bits, confidence, rand);
            bool found = false;

            while (!found)
            {
                found = pseudoPrime.IsSafePrime(confidence);
                if (!found)
                {
                    pseudoPrime = GeneratePseudoPrime(bits, confidence, rand);
                }
            }
            return pseudoPrime;
        }
        #endregion

        #region Private Helper functions
        private static byte GetByte(char c1, char c2)
        {
            byte upperByte = CharToByte(c1);
            byte lowByte = CharToByte(c2);
            upperByte = (byte)((int)upperByte << 4);
            return (byte)(upperByte | lowByte);
        }

        private static byte CharToByte(char c)
        {
            switch (c)
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'a': return 10;
                case 'A': return 10;
                case 'b': return 11;
                case 'B': return 11;
                case 'c': return 12;
                case 'C': return 12;
                case 'd': return 13;
                case 'D': return 13;
                case 'e': return 14;
                case 'E': return 14;
                case 'f': return 15;
                case 'F': return 15;
            }
            throw new FormatException("Invalid hexadecimal character");
        }
        #endregion
    }
}
#endif