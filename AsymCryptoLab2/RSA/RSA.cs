using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AsymCryptoLab2
{
    public class RSA
    {
        public readonly struct PublicKey
        {
            public readonly BigInteger n { get; init; }
            public readonly int exponent { get; init; }
        }

        public readonly struct SecretKey
        {
            public readonly BigInteger d { get; init; }
            public readonly BigInteger p { get; init; }
            public readonly BigInteger q { get; init; }
        }

        private PublicKey _pk;
        private SecretKey _sk;

        public PublicKey GetPublicKey()
        {
            return _pk;
        }

        public RSA(int keysSize)
        {
            UpdateKeys(keysSize);
        }

        private (PublicKey publicKey, SecretKey privateKey) KeysGenerator(int byteLength, int exponent = 65537) //na-h i'd win
        {
            Console.WriteLine("Generating p...");
            BigInteger p = Utils.GenerateStrongPrime(byteLength / 2);
            Console.WriteLine("Generating q...");
            BigInteger q = Utils.GenerateStrongPrime(byteLength / 2);

            BigInteger n = BigInteger.Multiply(p, q);
            BigInteger phi = BigInteger.Multiply(p - 1, q - 1);

            BigInteger d = 0;
            MathUtils.ExtendedGCD(exponent, phi, out d);

            PublicKey publicKey = new PublicKey { n = n , exponent = exponent};
            SecretKey privateKey = new SecretKey { d = d, p = p, q = q };

            return (publicKey, privateKey);
        }

        public void UpdateKeys(int keysSize)
        {
            (_pk, _sk) = KeysGenerator(keysSize);
            Debug.Assert((_sk.d * _pk.exponent) % ((_sk.p - 1) * (_sk.q - 1)) == 1);
        }

        public static BigInteger Encrypt(BigInteger m, PublicKey receiverKey)
        {
            BigInteger c = BigInteger.ModPow(m, receiverKey.exponent, receiverKey.n);
            return c;
        }

        public BigInteger Decrypt(BigInteger c)
        {
            BigInteger m = BigInteger.ModPow(c, _sk.d, _pk.n);
            return m;
        }

        public BigInteger Sign(BigInteger m)
        {
            BigInteger s = BigInteger.ModPow(m, _sk.d, _pk.n);
            return s;
        }

        // Sometimes works inappropriate
        public static bool Verify(BigInteger m, BigInteger s, PublicKey signerKey)
        {
            BigInteger t = BigInteger.ModPow(s, signerKey.exponent, signerKey.n);
            return t == m;
        }

        //public (byte[] encMsg, byte[] sign) EncryptAndSign(PublicKey receiverKey)
        //{

        //}

        //public (byte[] decMsg, bool verification) DecryptAndVerify(PublicKey signerKey)
        //{

        //}

    }
}