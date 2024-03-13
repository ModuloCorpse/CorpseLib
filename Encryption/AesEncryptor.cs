using System.Security.Cryptography;

namespace CorpseLib.Encryption
{
    public class AesEncryptor(string password)
    {
        private readonly string m_Password = password;
        public int KeySize = 32;
        public int SaltSize = 16;
        public int IVSize = 16;

        /*private static byte[] NewEncrypt(string plainText, string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (Rfc2898DeriveBytes pass = new(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                var keyBytes = pass.GetBytes(32);
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.GenerateIV();
                    using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(keyBytes, aesAlg.IV))
                    {
                        using (MemoryStream memoryStream = new())
                        {
                            memoryStream.Write(salt, 0, salt.Length);
                            memoryStream.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                            using (CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                return memoryStream.ToArray();
                            }
                        }
                    }
                }
            }
        }

        byte[] Decrypt(string cipherText, string passPhrase = null, byte[] salt = null)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return null;
            }

            if (passPhrase == null)
            {
                passPhrase = Options.DefaultPassPhrase;
            }

            if (salt == null)
            {
                salt = Options.DefaultSalt;
            }

            var cipherTextBytes = Convert.FromBase64String(cipherText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, salt, 10000, HashAlgorithmName.SHA256))
            {
                var keyBytes = password.GetBytes(Options.Keysize / 8);
                using (var aesAlg = Aes.Create())
                {
                    aesAlg.Mode = CipherMode.CBC;
                    using (var decryptor = aesAlg.CreateDecryptor(keyBytes, Options.InitVectorBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var totalReadCount = 0;
                                while (totalReadCount < cipherTextBytes.Length)
                                {
                                    var buffer = new byte[cipherTextBytes.Length];
                                    var readCount = cryptoStream.Read(buffer, 0, buffer.Length);
                                    if (readCount == 0)
                                    {
                                        break;
                                    }

                                    for (var i = 0; i < readCount; i++)
                                    {
                                        plainTextBytes[i + totalReadCount] = buffer[i];
                                    }

                                    totalReadCount += readCount;
                                }

                                return plainTextBytes;
                            }
                        }
                    }
                }
            }
        }*/

        public byte[] Encrypt(byte[] data)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] iv = RandomNumberGenerator.GetBytes(IVSize);
            byte[] key = new Rfc2898DeriveBytes(m_Password, salt, 10000, HashAlgorithmName.SHA256).GetBytes(KeySize);
            using Aes aesAlg = Aes.Create();
            aesAlg.Mode = CipherMode.CBC;
            byte[] encryptedData = aesAlg.CreateEncryptor(key, iv).TransformFinalBlock(data, 0, data.Length);
            byte[] ret = new byte[salt.Length + iv.Length + encryptedData.Length];
            Array.Copy(salt, 0, ret, 0, salt.Length);
            Array.Copy(iv, 0, ret, salt.Length, iv.Length);
            Array.Copy(encryptedData, 0, ret, salt.Length + iv.Length, encryptedData.Length);
            return ret;
        }

        public byte[] Decrypt(byte[] data)
        {
            byte[] salt = new byte[SaltSize];
            Array.Copy(data, 0, salt, 0, salt.Length);
            byte[] key = new Rfc2898DeriveBytes(m_Password, salt, 10000, HashAlgorithmName.SHA256).GetBytes(KeySize);
            using Aes aesAlg = Aes.Create();
            aesAlg.Mode = CipherMode.CBC;
            byte[] iv = new byte[IVSize];
            Array.Copy(data, salt.Length, iv, 0, iv.Length);
            int offset = salt.Length + iv.Length;
            return aesAlg.CreateDecryptor(key, iv).TransformFinalBlock(data, offset, (data.Length - offset));
        }
    }
}
