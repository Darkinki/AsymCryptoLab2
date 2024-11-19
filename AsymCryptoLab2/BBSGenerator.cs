using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AsymCryptoLab2
{
    // TODO: find out chunking in the begining of the sequence 
    // Update: Seems everything is allright. 
    public class BBSGenerator
    {
        private readonly BigInteger p = BigInteger.Parse("d5bbb96d30086ec484eba3d7f9caeb07", System.Globalization.NumberStyles.AllowHexSpecifier);
        private readonly BigInteger q = BigInteger.Parse("425d2b9bfdb25b9cf6c416cc6e37b59c1f", System.Globalization.NumberStyles.AllowHexSpecifier);
        private readonly BigInteger n;

        public BBSGenerator()
        {
            n = p * q;
        }

        public BitArray GenBits(long seed, int length)
        {
            BitArray res = new(length);
            BigInteger r_prev = new(seed);
            BigInteger r = new();
            for (int i = 0; i < length; i++)
            {
                r = BigInteger.ModPow(r_prev, 2, n);
                res[i] = (r % 2) == 1;
                r_prev = r;
            }
            return res;
        }

        public byte[] GenBytes(long seed, int length)
        {
            byte[] res = new byte[length];
            BigInteger r = new(seed);
            for (int i = 0; i < length; i++)
            {
                r = BigInteger.ModPow(r, 2, n);
                res[i] = (byte)(r & 255);
            }
            return res;
        }
    }
}
