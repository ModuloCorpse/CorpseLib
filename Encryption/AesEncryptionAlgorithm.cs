using System.Security.Cryptography;

namespace CorpseLib.Encryption
{
    public class AesEncryptionAlgorithm(string password) : IEncryptionAlgorithm
    {
        private readonly string m_Password = password;
        public int KeySize = 32;
        public int SaltSize = 16;
        public int IVSize = 16;

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
