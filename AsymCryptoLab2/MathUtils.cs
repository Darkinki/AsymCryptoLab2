﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Cache;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AsymCryptoLab2
{
    public static class MathUtils
    {

        public static BigInteger Mod(BigInteger a, BigInteger n)
        {
            if (a < 0)
            {
                a = -a * (n - 1);
            }

            BigInteger res = a % n;
            return res;
        }
        public static BigInteger GCD(BigInteger a, BigInteger b)
        {
            if (b == 0) return a;
            return GCD(b, Mod(a, b));
        }
        public static BigInteger LCM(BigInteger a, BigInteger b)
        {
            return (a * b) / GCD(a, b);
        }
        public static bool CheckStrongPrime(BigInteger n, BigInteger x)
        {
            if (x >= n || x <= 1) throw new Exception("Bad x: its must be in range");
            BigInteger s, t;
            s = 0;
            t = n - 1;

            while (t % 2 == 0)
            {
                t >>= 1;
                s++;
            }

            BigInteger y = BigInteger.ModPow(x, t, n);

            if ((y == 1) || (y == n - 1))
            {
                return true;
            }

            for (int i = 1; i < s; i++)
            {
                y = BigInteger.ModPow(y, 2, n);
                if (y == n - 1) return true;
            }

            return false;

        }
        /*   public static BigInteger ModInverse(BigInteger a, BigInteger m)
           {
               BigInteger m0 = m;
               BigInteger y = 0;
               BigInteger x = 1;

               if (m == 1)
               {
                   return 0;
               }

               while (a > 1)
               {
                   BigInteger q = BigInteger.DivRem(a, m, out BigInteger t);
                   (a, m) = (m, a);
                   (x, y) = (y, BigInteger.Subtract(x, BigInteger.Multiply(q, y)));
               }

               if (x < 0)
               {
                   x = BigInteger.Add(x, m0);
               }

               Console.WriteLine(x);
               return x;

           }*/

        public static BigInteger ExtendedGCD(BigInteger x, BigInteger n, out BigInteger xReverse)
        {
            xReverse = 0;

            BigInteger rPrev = x;
            BigInteger r = n;
            BigInteger q = 0;

            BigInteger u1 = 1;
            BigInteger u2 = 0;
            BigInteger u3 = 0;

            BigInteger v1 = 0;
            BigInteger v2 = 1;
            BigInteger v3 = 0;

            while (true)
            {
                (q, BigInteger rNext) = BigInteger.DivRem(rPrev, r);

                u3 = u1 - (u2 * q);
                v3 = v1 - (v2 * q);

                if (rNext == 0) break;

                // Preparing for the next iteration
                rPrev = r;
                r = rNext;
                u1 = u2;
                u2 = u3;
                v1 = v2;
                v2 = v3;
            }

            if (r == 1) xReverse = Mod(u2, n);

            return r;
        }

    }
}