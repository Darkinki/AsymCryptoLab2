using AsymCryptoLab2;
using CryptoGenerators;
using CryptoGenerators.Seed;
using MathNet.Numerics;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Globalization;

namespace AsymCryptoLab2
{
    internal class Program
    {
        struct ReceiveKeyResponse
        {
            public string key;
            public bool verified;
        }

        static readonly HttpClient httpClient = new()
        {
        };

        public static void TestPrimeTest()
        {
            bool prime = false;
            int k = 0;
            BigInteger res = BigInteger.One;
            while (!prime)
            {
                k++;
                res = Utils.RandomBigInteger(DateTime.Now.Ticks, 2);
                prime = Utils.PrimeTest(res);
                Console.WriteLine("{0}: {1}", k, prime);
            }

            Console.WriteLine(res);
        }

        public static void TestRSA()
        {
            RSA Alice = new(32);
            RSA Bob = new(32);
            Console.WriteLine($"Alice public key: {Alice.GetPublicKey().n}\nBob public key: {Bob.GetPublicKey().n}");

            BBSGenerator bbs = new();

            var msg = Utils.RandomBigInteger(Seed.FromSystemTime(), 30);
            MD5 hash = MD5.Create();
            hash.Initialize();
            
            Console.WriteLine($"Message: {msg}");
            var encMsg = RSA.Encrypt(msg, Bob.GetPublicKey());
            Console.WriteLine($"Encrypted message: {encMsg}");
            var sign = Alice.Sign(new BigInteger(new ReadOnlySpan<byte>(hash.ComputeHash(encMsg.ToByteArray())), true));
            Console.WriteLine($"Sign: {sign}");
            var decMsg = Bob.Decrypt(encMsg);
            Console.WriteLine($"decMsg: {decMsg}");
            hash.Initialize();
            bool isVerified = RSA.Verify(new BigInteger(new ReadOnlySpan<byte>(hash.ComputeHash(encMsg.ToByteArray())), true), sign, Alice.GetPublicKey());

            Debug.Assert(isVerified);
            Debug.Assert(decMsg == msg);
        }

        public static async Task<string?> GetResponseContent(string request)
        {
            var response = await httpClient.GetAsync(new Uri(request));
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Error response: {await response.Content.ReadAsStringAsync()}");
            }
            var content = await response.Content.ReadAsStringAsync();

            return content;
        }

        public static async Task<RSA.PublicKey> GetServerKeys()
        {
            var response = await httpClient.GetAsync(new Uri("http://asymcryptwebservice.appspot.com/rsa/serverKey?keySize=256"));
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("Cannot receive public keys from server");
            }
            var content = await response.Content.ReadAsStringAsync();
            var res = JsonDocument.Parse(content);
            var parsed_res = res.Deserialize<Dictionary<string, string>>();

            return new RSA.PublicKey { n = BigInteger.Parse($"0{parsed_res["modulus"]}", System.Globalization.NumberStyles.AllowHexSpecifier), exponent = Int32.Parse($"0{parsed_res["publicExponent"]}", System.Globalization.NumberStyles.AllowHexSpecifier) };
        }

        public static async Task<BigInteger> ServerEncrypt(RSA.PublicKey pk, BigInteger message)
        {
            string requestData = 
                string.Format("modulus={0}&publicExponent={1}&message={2}", 
                pk.n.ToString("X"), pk.exponent.ToString("X"), message.ToString("X"));
            
            var content_string = await GetResponseContent($"http://asymcryptwebservice.appspot.com/rsa/encrypt?{requestData}");
            var content = JsonDocument.Parse(content_string).Deserialize<Dictionary<string, string>>();
            if (content == null)
            {
                throw new Exception("Undefined content!");
            }


            return BigInteger.Parse($"0{content["cipherText"]}", System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        public static async Task<BigInteger> ServerDecrypt(BigInteger message)
        {
            string requestData =
                string.Format("cipherText={0}", message.ToString("X"));

            var content_string = await GetResponseContent($"http://asymcryptwebservice.appspot.com/rsa/decrypt?{requestData}");
            var content = JsonDocument.Parse(content_string).Deserialize<Dictionary<string, string>>();
            if (content == null)
            {
                throw new Exception("Undefined content!");
            }

            return BigInteger.Parse($"0{content["message"]}", System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        public static async Task<BigInteger> SeverSign(BigInteger message)
        {
            string requestData =
                string.Format("message={0}", message.ToString("X"));

            var content_string = await GetResponseContent($"http://asymcryptwebservice.appspot.com/rsa/sign?{requestData}");
            var content = JsonDocument.Parse(content_string).Deserialize<Dictionary<string, string>>();
            if (content == null)
            {
                throw new Exception("Undefined content!");
            }

            return BigInteger.Parse($"0{content["signature"]}", System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        public static async Task<bool> SeverVerify(RSA.PublicKey pk, BigInteger message, BigInteger signature)
        {
            string requestData =
                string.Format("modulus={0}&publicExponent={1}&message={2}&signature={3}",
                pk.n.ToString("X"), pk.exponent.ToString("X"), message.ToString("X"), signature.ToString("X"));

            var content_string = await GetResponseContent($"http://asymcryptwebservice.appspot.com/rsa/verify?{requestData}");
            var content = JsonDocument.Parse(content_string).Deserialize<Dictionary<string, bool>>();
            if (content == null)
            {
                throw new Exception("Undefined content!");
            }

            return content["verified"];
        }

        public static async Task<(BigInteger, BigInteger)> ServerSendKey(RSA.PublicKey pk)
        {
            string requestData =
                string.Format("modulus={0}&publicExponent={1}",
                pk.n.ToString("X"), pk.exponent.ToString("X"));

            var content_string = await GetResponseContent($"http://asymcryptwebservice.appspot.com/rsa/sendKey?{requestData}");
            var content = JsonDocument.Parse(content_string).Deserialize<Dictionary<string, string>>();

            if (content == null)
            {
                throw new Exception("Undefined content!");
            }

            return (BigInteger.Parse($"0{content["key"]}", System.Globalization.NumberStyles.AllowHexSpecifier), BigInteger.Parse($"0{content["signature"]}", System.Globalization.NumberStyles.AllowHexSpecifier));
        }
        public static async Task<(BigInteger, bool)> ServerReceiveKey(RSA.PublicKey pk, BigInteger key, BigInteger signature)
        {
            string requestData =
                string.Format("key={0}&signature={1}&modulus={2}&publicExponent={3}",
                key.ToString("X"), signature.ToString("X"), pk.n.ToString("X"), pk.exponent.ToString("X"));

            var content_string = await GetResponseContent($"http://asymcryptwebservice.appspot.com/rsa/receiveKey?{requestData}");
            ReceiveKeyResponse content = JsonDocument.Parse(content_string).Deserialize<ReceiveKeyResponse>();

            return (BigInteger.Parse($"0{content.key}", System.Globalization.NumberStyles.AllowHexSpecifier), content.verified);
        }

        public static async Task Main(string[] args)
        {
            //Console.WriteLine("Testing RSA implementation...");
            //TestRSA();
            //TestRSA();
            //TestRSA();
            //TestRSA();            
            //TestRSA();
            //TestRSA();
            //TestRSA();
            //TestRSA();
            //Console.WriteLine("<<Great success>>");

            var serverKey = await GetServerKeys();
            Console.WriteLine($"Received keys! n = {serverKey.n}, e = {serverKey.exponent}");

            RSA client = new(32);

            while (client.GetPublicKey().n > serverKey.n)
            {
                client.UpdateKeys(32);
            }

            Console.WriteLine($"Client keys! n = {client.GetPublicKey().n}, e = {client.GetPublicKey().exponent}");

            BigInteger k = Utils.RandomBigInteger(Seed.FromSystemTime(), 10);
            Console.WriteLine($"Generated shared secret: {k}");
            var k_1 = RSA.Encrypt(k, serverKey);
            var s = client.Sign(k);
            var s_1 = RSA.Encrypt(s, serverKey);

            var sendedKey = await ServerSendKey(client.GetPublicKey());
            Console.WriteLine($"SendedKey: {sendedKey.Item1}; {sendedKey.Item2}");
            var signDecrypted = client.Decrypt(sendedKey.Item2);
            var messageDecrypted = client.Decrypt(sendedKey.Item1);
            var signCheck = RSA.Verify(messageDecrypted, signDecrypted, serverKey);
            Console.WriteLine($"Verification: {signCheck}");

            var receivedSignedKey = await ServerReceiveKey(client.GetPublicKey(), k_1, s_1);
            Console.WriteLine($"Receiving  Key response: {receivedSignedKey.Item1}, {receivedSignedKey.Item2}");

            //var decryptedSignature = await ServerDecrypt(s_1);
            //Console.WriteLine($"Signature: {s}");
            //Console.WriteLine($"Encrypted signature: {s_1}");
            //Console.WriteLine($"Decrypted Signature: {decryptedSignature}");

            //var isVerified = await SeverVerify(client.GetPublicKey(), k, s);
            //Console.WriteLine($"Is signature verified: {isVerified}");


        }
    }
}