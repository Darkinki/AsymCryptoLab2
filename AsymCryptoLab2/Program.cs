using System;
using System.Security.Cryptography;

namespace AsymCryptoLab2
{
    internal class Program
    {
        public static void Main(string[] args)
        {


             (RSA.PublicKey publicKey, RSA.PrivateKey privateKey) = RSA.KeysGenerator(258);

             string messageString = "Ad impossibilia nemo tenetur.";
             byte[] message = System.Text.Encoding.UTF8.GetBytes(messageString);

            byte[] ciphertext = RSA.Encrypt(message, publicKey);

            byte[] decryptedMessage = RSA.Decrypt(ciphertext, privateKey);

             string decryptedMessageString = System.Text.Encoding.UTF8.GetString(decryptedMessage);
             Console.WriteLine("start: " + messageString);
             Console.WriteLine("encrypt: " + Convert.ToBase64String(ciphertext));
             Console.WriteLine("decrypt: " + decryptedMessageString);
        }
    }
}