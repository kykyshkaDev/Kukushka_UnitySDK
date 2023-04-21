using System;
using System.Security.Cryptography;
using System.Text;

namespace KykyshkaSDK.Core.Encryption
{
    /// <summary>
    /// SHA512 Encryption Provider
    /// </summary>
    public class SHA512 : IDataEncryption
    {
        /// <summary>
        /// SHA512 Encryption Provider
        /// </summary>
        public SHA512(){}
        
        /// <summary>
        /// Encode String Data
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public string EncodeString(string plane)
        {
            using (SHA512Managed sha256 = new SHA512Managed())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(plane));
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
            using (SHA512Managed sha512 = new SHA512Managed())
            {
                var hash = sha512.ComputeHash(src);
                return hash;
            }
        }
        
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