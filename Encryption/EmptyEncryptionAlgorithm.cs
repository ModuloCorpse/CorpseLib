namespace CorpseLib.Encryption
{
    public class EmptyEncryptionAlgorithm() : IEncryptionAlgorithm
    {
        public byte[] Decrypt(byte[] input) => input;
        public byte[] Encrypt(byte[] input) => input;
    }
}
