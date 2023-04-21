using System;
using System.Text;

namespace KykyshkaSDK.Core.Encryption
{
    /// <summary>
    /// Base64 Data Encryption
    /// </summary>
    public class Base64 : IDataEncryption
    {
        /// <summary>
        /// Base64 Constructor
        /// </summary>
        public Base64(){}
        
        /// <summary>
        /// Encode String Data
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public string EncodeString(string plane)
        {
            byte[] bytesToEncode = Encoding.UTF8.GetBytes (plane);
            string encodedText = Convert.ToBase64String (bytesToEncode);
            return encodedText;
        }
        
        /// <summary>
        /// Encrypt Data Bytes
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public byte[] EncodeBinary(byte[] src)
        {
            string encodedText = Convert.ToBase64String(src);
            byte[] encodedBytes = Encoding.UTF8.GetBytes (encodedText);
            return encodedBytes;
        }
        
        /// <summary>
        /// Decrypt String
        /// </summary>
        /// <param name="encrtpted"></param>
        /// <returns></returns>
        public string DecodeString(string encrtpted)
        {
            byte[] decodedBytes = Convert.FromBase64String (encrtpted);
            string decodedText = Encoding.UTF8.GetString (decodedBytes);
            return decodedText;
        }
        
        /// <summary>
        /// Decrypt Binary Data
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public byte[] DecodeBinary(byte[] src)
        {
            string encoded = Encoding.UTF8.GetString(src);
            byte[] decodedBytes = Convert.FromBase64String(encoded);
            return decodedBytes;
        }
    }
}