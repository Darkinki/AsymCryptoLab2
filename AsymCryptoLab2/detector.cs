using AsymCryptoLab2;
using System;
using System.Numerics;
using System.Security.Cryptography;

internal class Detector
{
    public static BigInteger GenerateRandomPrime(int bitLength)
    {
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            while (true)
            {
                byte[] bytes = new byte[bitLength / 8];
                rng.GetBytes(bytes);

                bytes[bytes.Length - 1] |= 0x80;

                BigInteger candidate = new BigInteger(bytes);
                if (candidate < 0)
                {
                    candidate = -candidate;
                }

                if (candidate.IsEven)
                {
                    candidate++;
                }

                if (candidate > 1 && PrimeTest(candidate))
                {
                    return candidate;
                }
            }
        }
    }

    public static bool PrimeTest(BigInteger n, int k = 1) // к - ітерації, не чіпай - хата сгорить
    {
        if (n < 0) n = -n;
        if ((n == 2) || (n == 3) || (n == 5)) return true;
        if ((n % 2 == 0) || (n == 1)) return false;

        BigInteger x = 1;
        for (int i = 0; i < k; i++)
        {
            Random rand = new Random();
            x = RandomBigInteger(3,n/2); 
            if (MathUtils.GCD(x, n) > 1) return false;
            if (MathUtils.CheckStrongPrime(n, x)) return true;
        }

        return false;

    }

    //діапазон
    public static BigInteger RandomBigInteger(BigInteger min, BigInteger max)
    {
        if (min >= max)
        {
            throw new ArgumentException("min cannot be greater than or equal to max");
        }

        byte[] maxBytes = (max - 1).ToByteArray();
        byte[] bytes = new byte[maxBytes.Length];

        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }

        BigInteger value = new BigInteger(bytes);
        BigInteger range = max - min;
        value = (BigInteger.Abs(value) % range) + min;
        return value;
    }
}