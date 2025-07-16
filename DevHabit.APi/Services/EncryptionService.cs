using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DevHabit.APi.Settings;
using Microsoft.Extensions.Options;

namespace DevHabit.APi.Services
{
    public sealed class EncryptionService(IOptions<EncryptionOptions> options)
    {
        private readonly byte[] _masterKey = Convert.FromBase64String(options.Value.Key);
        private const int IvSize = 16;

        public string Encrypt(string plainText)
        {
            try
            {
                using var aes = Aes.Create();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = _masterKey;
                aes.IV = RandomNumberGenerator.GetBytes(IvSize);

                using var memoryStream = new MemoryStream();
                memoryStream.Write(aes.IV, 0, IvSize);

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                using (var streamWriter = new StreamWriter(cryptoStream))
                {
                    streamWriter.Write(plainText);
                }

                return Convert.ToBase64String(memoryStream.ToArray());

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public string Decrypt(string CipherText)
        {
            try
            {
                byte[] cipherData = Convert.FromBase64String(CipherText);

                byte[] iv = new byte[IvSize];
                byte[] encrytedData = new byte[cipherData.Length - IvSize];

                Buffer.BlockCopy(cipherData, 0, iv, 0, IvSize);
                Buffer.BlockCopy(cipherData, IvSize, encrytedData, 0, encrytedData.Length);

                using var aes = Aes.Create();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = _masterKey;
                aes.IV = iv;

                using MemoryStream memoryStream = new(encrytedData);
                using ICryptoTransform decryptor = aes.CreateDecryptor();
                using CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read);
                using StreamReader streamReader = new(cryptoStream);

                return streamReader.ReadToEnd();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
    }
}