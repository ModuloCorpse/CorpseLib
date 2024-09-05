namespace CorpseLib.Encryption
{
    public interface IEncryptionAlgorithm
    {
        public byte[] Encrypt(byte[] input);
        public byte[] Decrypt(byte[] input);
    }
}
