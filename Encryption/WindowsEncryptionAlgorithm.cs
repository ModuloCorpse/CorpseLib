using System.Runtime.Versioning;
using System.Security.Cryptography;

namespace CorpseLib.Encryption
{
    [SupportedOSPlatform("windows")]
    public class WindowsEncryptionAlgorithm(byte[] entropy) : IEncryptionAlgorithm
    {
        private readonly byte[] m_Entropy = entropy;

        public byte[] Decrypt(byte[] input)
        {
            try
            {
                return ProtectedData.Unprotect(input, m_Entropy, DataProtectionScope.CurrentUser);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("Data was not decrypted. An error occurred.");
                Console.WriteLine(e.ToString());
                return [];
            }
        }

        public byte[] Encrypt(byte[] input)
        {
            try
            {
                return ProtectedData.Protect(input, m_Entropy, DataProtectionScope.CurrentUser);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("Data was not encrypted. An error occurred.");
                Console.WriteLine(e.ToString());
                return [];
            }
        }
    }
}
