using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Newtonsoft.Json;

namespace MSDF.StudentEngagement.Resources.Services.Encryption
{
    public interface IEncryptionService
    {
        string Decrypt(string jsonEncryptionModel, string exportedKey);
        string Decrypt(EncryptionModel encryptionModel, string exportedKey);
    }

    public class EncryptionService: IEncryptionService
    {
        const int TAG_LENGHT = 128;/* bits*/
        const int TAG_LENGHT_BYTES = TAG_LENGHT / 8;/* bytes*/
        public string Decrypt(EncryptionModel encryptionModel, string exportedKey)
        {

            var key =  GenEncryptionKeyFromExportedKey(exportedKey);
            var nonce = encryptionModel.ibiv.ToArray();

            ExtractCipherAndTag(encryptionModel, out byte[] ciphertext, out byte[] tag);

            var plainTextBytes = new byte[ciphertext.Length];

            AesGcm aesGcm = new AesGcm(key);
            aesGcm.Decrypt(nonce, ciphertext, tag, plainTextBytes);

            return Encoding.UTF8.GetString(plainTextBytes);
        }

        private static void ExtractCipherAndTag(EncryptionModel encryptionModel, out byte[] ciphertext, out byte[] tag)
        {
            var ct = encryptionModel.ibct.ToArray();
            ciphertext = new byte[ct.Length - TAG_LENGHT_BYTES];
            tag = new byte[TAG_LENGHT_BYTES];
            Array.Copy(ct, 0, ciphertext, 0, ct.Length - TAG_LENGHT_BYTES);
            Array.Copy(ct, ct.Length - TAG_LENGHT_BYTES, tag, 0, TAG_LENGHT_BYTES);
        }

        public string Decrypt(string jsonEncryptionModel, string exportedKey)
        {
            var encryptionModel = JsonConvert.DeserializeObject<EncryptionModel>(jsonEncryptionModel);
            return Decrypt(encryptionModel, exportedKey);  
        }
        
        private byte[] GenEncryptionKeyFromExportedKey(string exportedKey)
        {
            return Convert.FromBase64String(exportedKey);
        }
    }
}
