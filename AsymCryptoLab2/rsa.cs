using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AsymCryptoLab2
{
    internal class RSA
    {
        ///encryprion(є), decryprion(є), key generation(detector?...) and DA(не є)

        public struct PublicKey
        {
            public BigInteger n;
            public int e;
        }

        public struct PrivateKey
        {
            public BigInteger d;
            public BigInteger p;
            public BigInteger q;
        }

        public static (PublicKey publicKey, PrivateKey privateKey) KeysGenerator(int bitLenght) //na-h i'd win
        {
            BigInteger p = Detector.GenerateRandomPrime(bitLenght / 2);
            BigInteger q = Detector.GenerateRandomPrime(bitLenght / 2);


            BigInteger n = BigInteger.Multiply(p, q);
            BigInteger phi = BigInteger.Multiply(p-1, q-1);

            int e = 65537; //2^16 +1

            BigInteger d = 0; 
            MathUtils.ExtendedGCD(e, phi, out d);

            PublicKey publicKey = new PublicKey { n = n, e = e };
            PrivateKey privateKey = new PrivateKey { d = d, p = p, q = q };

            return (publicKey, privateKey);
        }
        public static byte[] Encrypt(byte[] message, PublicKey publicKey)
        {
            BigInteger m = new BigInteger(message);
            BigInteger c = BigInteger.ModPow(m, publicKey.e, publicKey.n);
            return c.ToByteArray();
        }

        public static byte[] Decrypt(byte[] ciphertext, PrivateKey privateKey)
        {
            BigInteger c = new BigInteger(ciphertext);
            BigInteger m = BigInteger.ModPow(c, privateKey.d, privateKey.p * privateKey.q);
            return m.ToByteArray();
        }

    }
}