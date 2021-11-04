using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.Extensions.Logging;
using ServiceBusDriver.Server.Settings;
using ServiceBusDriver.Shared.Constants;
using ServiceBusDriver.Shared.Features.Error;

namespace ServiceBusDriver.Server.Services.Password
{
    public class AesEncryptService : IAesEncryptService
    {
        private readonly ISettings _settings;
        private readonly ILogger<AesEncryptService> _logger;

        public AesEncryptService(ISettings settings, ILogger<AesEncryptService> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public string Encrypt(string text)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var uriEncodedText = HttpUtility.UrlEncode(text);

                var key = Encoding.UTF8.GetBytes(_settings.AesKey);

                using (var aesAlg = Aes.Create())
                {
                    using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                    {
                        using (var msEncrypt = new MemoryStream())
                        {
                            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(uriEncodedText);
                            }

                            var iv = aesAlg.IV;

                            var decryptedContent = msEncrypt.ToArray();

                            var result = new byte[iv.Length + decryptedContent.Length];

                            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                            Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                            stopwatch.Stop();
                            _logger.LogTrace("Finish {0}", nameof(Encrypt));
                            _logger.LogTrace("Encryption for text of length {0} Finished with Time: {1} Milliseconds", text.Length, stopwatch.ElapsedMilliseconds);

                            return Convert.ToBase64String(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while encrypting text");
                throw new AppException
                {
                    ErrorMessage = new AppErrorMessageDto
                    {
                        Code = AppErrorConstants.EncryptionFailureCode,
                        UserMessageText = AppErrorConstants.EncryptionFailureMessage
                    }
                };
            }
        }

        public string Decrypt(string cipherText)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var fullCipher = Convert.FromBase64String(cipherText);

                var iv = new byte[16];
                var cipher = new byte[fullCipher.Length - iv.Length];

                Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length);
                var key = Encoding.UTF8.GetBytes(_settings.AesKey);

                using (var aesAlg = Aes.Create())
                {
                    using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                    {
                        string result;
                        using (var msDecrypt = new MemoryStream(cipher))
                        {
                            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (var srDecrypt = new StreamReader(csDecrypt))
                                {
                                    result = srDecrypt.ReadToEnd();
                                }
                            }
                        }

                        var decodedString = HttpUtility.UrlDecode(result);
                        _logger.LogTrace("Finish {0}", nameof(Decrypt));
                        _logger.LogTrace("Decryption for text of length {0} Finished with Time: {1} Milliseconds", decodedString.Length, stopwatch.ElapsedMilliseconds);

                        return decodedString;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while decrypting text");
                throw new AppException
                {
                    ErrorMessage = new AppErrorMessageDto
                    {
                        Code = AppErrorConstants.DecryptionFailureCode,
                        UserMessageText = AppErrorConstants.DecryptionFailureMessage
                    }
                };
            }
        }
    }
}