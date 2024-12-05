using AsymCryptoLab2;
using System;
using System.Numerics;
using System.Security.Cryptography;
using CryptoGenerators.Tests;
using CryptoGenerators.Seed;
using CryptoGenerators;
using System.Net.Sockets;


namespace AsymCryptoLab2
{
    class Utils
    {
        // Not the best implementaion, todo- make better)
        public static BigInteger GenerateRandomPrime(int byteLength)
        {
            BigInteger res = BigInteger.Zero;
            while (!PrimeTest(res))
            {
                res = RandomBigInteger(Seed.FromSystemTime(), byteLength);
            }
            return res;
        }

        public static BigInteger GenerateStrongPrime(int byteLength)
        {
            BigInteger strong_prime = BigInteger.Zero;
            var p = GenerateRandomPrime(byteLength - 1);
            int i = 1;
            while(true)
            {
                strong_prime = 2 * i * p + 1;
                if (PrimeTest(strong_prime)) return strong_prime;
                i++;
            }
        }

        public static bool PrimeTest(BigInteger n, int k = 10)
        {
            if (n == 0 || n == 1) return false;

            bool res = true;
            var d = n - 1;
            int s = 0;

            while((d & 1) == 0)
            {
                d >>= 1;
                s++;
            }

            for (int i = 0; i < k; i++)
            {

                BigInteger x = BigInteger.One;
                while(x == BigInteger.One) x = RandomBigInteger(DateTime.Now.Ticks, n.GetByteCount(true)) % n;

                if (MathUtils.ExtendedGCD(x, n, out _) != 1) return false;
                if (MathUtils.CheckStrongPrime(n, x, d, s))
                {
                    continue;
                }
                res = false;
                break;
            }

            return res;
        }

        // IMPORTANT!! len in bytes
        public static BigInteger RandomBigInteger(long seed, int bytes_len)
        {
            IGenerator gen = new BBSGenerator();
            var bytes = gen.GenBytes(seed, bytes_len);
            return new(new ReadOnlySpan<byte>(bytes), true);
        }

        public static BigInteger RandomBigIntegerInRange(BigInteger min, BigInteger max)
        {
            if (min >= max)
            {
                throw new ArgumentException("min cannot be greater than or equal to max");
            }

            BigInteger res = min - 1;  // Theoreticly, may be bad approach
            var byte_count = ((int)BigInteger.GreatestCommonDivisor(min.GetByteCount(true), max.GetByteCount(true)));

            while ( (res < min) || (res >= max) )
            {
                res = RandomBigInteger(DateTime.Now.Ticks, byte_count);
                Console.WriteLine(res);
            }

            return res;
        }
    }

}