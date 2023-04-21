namespace KykyshkaSDK.Core.Encryption
{
    /// <summary>
    /// Data Encryption Interface
    /// </summary>
    public interface IDataEncryption
    {
        string DecodeString(string encodedString);
        byte[] DecodeBinary(byte[] encodedData);
        string EncodeString(string decodedString);
        byte[] EncodeBinary(byte[] decodedData);
    }
}