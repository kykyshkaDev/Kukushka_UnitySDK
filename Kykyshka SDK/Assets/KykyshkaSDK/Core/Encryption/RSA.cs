using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace KykyshkaSDK.Core.Encryption
{
    /// <summary>
    /// RSA Encryption Provider
    /// </summary>
    public class RSA : IDataEncryption
    {
        public class EncodingOptions
        {
            public string PublicKey = "RSAPublicKey";
            public string PrivateKey = "RSAPrivateKey";
        }
        private static EncodingOptions _encryptor = new EncodingOptions();

        /// <summary>
        /// RSA Encryption Provider
        /// </summary>
        /// <param name="options"></param>
        public RSA(EncodingOptions options)
        {
            _encryptor = options;
        }
        
        /// <summary>
        /// Encode String Data
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public string EncodeString(string plane)
        {
            byte[] encrypted = EncodeBinary(Encoding.UTF8.GetBytes(plane));
            return Convert.ToBase64String(encrypted);
        }
        
        /// <summary>
        /// Encrypt Data Bytes
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public byte[] EncodeBinary(byte[] src)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(_encryptor.PublicKey);
                byte[] encrypted = rsa.Encrypt(src, false);
                return encrypted;
            }
        }
        
        /// <summary>
        /// Decrypt String
        /// </summary>
        /// <param name="encrtpted"></param>
        /// <returns></returns>
        public string DecodeString(string encrtpted)
        {
            byte[] decripted = DecodeBinary(Convert.FromBase64String(encrtpted));
            return Encoding.UTF8.GetString(decripted);
        }
        
        /// <summary>
        /// Decrypt Binary Data
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public byte[] DecodeBinary(byte[] src)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(_encryptor.PrivateKey);
                byte[] decrypted = rsa.Decrypt(src, false);
                return decrypted;
            }
        }
        
        /// <summary>
        /// Generate Key Pair
        /// </summary>
        /// <param name="keySize"></param>
        /// <returns></returns>
        public static KeyValuePair<string, string> GenrateKeyPair(int keySize){
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize);
            string publicKey = rsa.ToXmlString(false);
            string privateKey = rsa.ToXmlString(true);
            return new KeyValuePair<string, string>(publicKey, privateKey);
        }
    }
}