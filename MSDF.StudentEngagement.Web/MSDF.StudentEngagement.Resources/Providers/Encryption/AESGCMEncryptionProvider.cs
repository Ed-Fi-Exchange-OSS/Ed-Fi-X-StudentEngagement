using System;
using System.Security.Cryptography;
using System.Text;

namespace MSDF.StudentEngagement.Resources.Providers.Encryption
{
    public class AESGCMEncryptionProvider
    {
        public static string Decrypt(string cypherTextBase64, string keyBase64, string ivBase64)
        {
            var keyArr = Convert.FromBase64String(keyBase64);
            var ivArr = Convert.FromBase64String(ivBase64);
            var cipherTextArr = Convert.FromBase64String(cypherTextBase64);

            var aes = new AesGcm(keyArr);

            ExtractCipherAndTag(cipherTextArr, out byte[] ciphertext, out byte[] tag);
            var plainTextBytes = new byte[ciphertext.Length];

            aes.Decrypt(ivArr, ciphertext, tag, plainTextBytes);

            return Encoding.Unicode.GetString(plainTextBytes);
        }

        private static void ExtractCipherAndTag(byte[] cipherTextArr, out byte[] ciphertext, out byte[] tag)
        {
            var tagLengthInBytes = 128 / 8;
            var ct = cipherTextArr;
            ciphertext = new byte[ct.Length - tagLengthInBytes];
            tag = new byte[tagLengthInBytes];
            Array.Copy(ct, 0, ciphertext, 0, ct.Length - tagLengthInBytes);
            Array.Copy(ct, ct.Length - tagLengthInBytes, tag, 0, tagLengthInBytes);
        }
    }
}
