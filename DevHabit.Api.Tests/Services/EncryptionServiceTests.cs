using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DevHabit.APi.Services;
using DevHabit.APi.Settings;
using Microsoft.Extensions.Options;

namespace DevHabit.Api.Tests.Services
{
    public sealed class EncryptionServiceTests
    {
        private readonly EncryptionService _encryptionServ;

        public EncryptionServiceTests()
        {
            IOptions<EncryptionOptions> options = Options.Create(new EncryptionOptions
            {
                Key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            });
            _encryptionServ = new EncryptionService(options);
        }

        [Fact]
        public void Decrypt_Should_Return_Plain_Text_When_Decrypting()
        {
            //Arrange
            const string plainText = "sensitive text";
            string CipherText = _encryptionServ.Encrypt(plainText);

            //Act
            string descryptedCiphertext = _encryptionServ.Decrypt(CipherText);

            //Assert
            Assert.Equal(plainText, descryptedCiphertext);
        }
        
    }
}