using System;
using System.Security.Cryptography;
using System.Text;

namespace KykyshkaSDK.Core.Encryption
{
    /// <summary>
    /// SHA1 Encryption Provider
    /// </summary>
    public class SHA1 : IDataEncryption
    {
        /// <summary>
        /// Encode String Data
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public string EncodeString(string plane)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(plane));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }
        
        /// <summary>
        /// Encrypt Data Bytes
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public byte[] EncodeBinary(byte[] src)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(src);
                return hash;
            }
        }
        
        /// <summary>
        /// SHA1 Encryption Provider
        /// </summary>
        public SHA1(){}
        
        /// <summary>
        /// Decrypt String
        /// </summary>
        /// <param name="encrtpted"></param>
        /// <returns></returns>
        public string DecodeString(string encrtpted)
        {
            throw new Exception("You can't decode MD5 hash. Please, replace encoding provider.");
        }
        
        /// <summary>
        /// Decrypt Binary Data
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public byte[] DecodeBinary(byte[] src)
        {
            throw new Exception("You can't decode MD5 hash. Please, replace encoding provider.");
        }
    }
}