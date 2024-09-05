using System.Collections;
using System.Text;

namespace CorpseLib.Encryption
{
    public class EncryptedFile(string path) : IEnumerable<IEncryptionAlgorithm>
    {
        private readonly List<IEncryptionAlgorithm> m_Algorithm = [];
        private readonly string m_Path = path;

        public void Add(IEncryptionAlgorithm algorithm) => m_Algorithm.Add(algorithm);

        public void Write(string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            foreach (IEncryptionAlgorithm algorithm in m_Algorithm)
                bytes = algorithm.Encrypt(bytes);
            File.WriteAllBytes(m_Path, bytes);
        }

        public string Read()
        {
            if (!File.Exists(m_Path))
                return string.Empty;
            byte[] bytes = File.ReadAllBytes(m_Path);
            IEnumerable<IEncryptionAlgorithm> algorithms = m_Algorithm;
            foreach (IEncryptionAlgorithm algorithm in algorithms.Reverse())
                bytes = algorithm.Decrypt(bytes);
            return Encoding.UTF8.GetString(bytes);
        }

        public IEnumerator<IEncryptionAlgorithm> GetEnumerator() => ((IEnumerable<IEncryptionAlgorithm>)m_Algorithm).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_Algorithm).GetEnumerator();
    }
}
