namespace CorpseLib.Encryption
{
    public class LocalVault
    {
        private readonly IEncryptionAlgorithm m_DefaultEncryptionAlgorithm;
        private readonly string m_VaultPath;
        private readonly string m_VaultKeyPath;

        public LocalVault(string vaultPath, string vaultKeyPath, IEncryptionAlgorithm defaultEncryptionAlgorithm)
        {
            m_DefaultEncryptionAlgorithm = defaultEncryptionAlgorithm;
            m_VaultPath = vaultPath;
            m_VaultKeyPath = vaultKeyPath;
            if (!Directory.Exists(m_VaultPath))
                Directory.CreateDirectory(m_VaultPath);
        }

        public LocalVault(IEncryptionAlgorithm defaultEncryptionAlgorithm) : this("vault", "vault_key", defaultEncryptionAlgorithm) { }

        public void SetPassword(string password)
        {
            IEncryptionAlgorithm oldAlgorithm = GetEncryptionAlgorithm();
            IEncryptionAlgorithm newAlgorithm;
            if (!string.IsNullOrEmpty(password))
            {
                new EncryptedFile(m_VaultKeyPath) { m_DefaultEncryptionAlgorithm }.Write(password);
                newAlgorithm = new AesEncryptionAlgorithm(password);
            }
            else
            {
                File.Delete(m_VaultKeyPath);
                newAlgorithm = m_DefaultEncryptionAlgorithm;
            }
            string[] vaultFiles = Directory.GetFiles(m_VaultPath);
            foreach (string vaultFilePath in vaultFiles)
            {
                string vaultFile = Path.GetFileName(vaultFilePath);
                SaveValue(vaultFile, LoadValue(vaultFile, oldAlgorithm), newAlgorithm);
            }
        }

        private IEncryptionAlgorithm GetEncryptionAlgorithm()
        {
            if (File.Exists(m_VaultKeyPath))
            {
                EncryptedFile encryptedFile = new(m_VaultKeyPath) { m_DefaultEncryptionAlgorithm };
                return new AesEncryptionAlgorithm(encryptedFile.Read());
            }
            return m_DefaultEncryptionAlgorithm;
        }

        private string LoadValue(string key, IEncryptionAlgorithm algorithm)
        {
            string vaultFilePath = Path.Combine(m_VaultPath, key);
            if (File.Exists(vaultFilePath))
                return new EncryptedFile(vaultFilePath) { algorithm }.Read();
            return string.Empty;
        }

        public string Load(string key) => LoadValue(key, GetEncryptionAlgorithm());

        private void SaveValue(string key, string value, IEncryptionAlgorithm algorithm)
        {
            string vaultFilePath = Path.Combine(m_VaultPath, key);
            new EncryptedFile(vaultFilePath) { algorithm }.Write(value);
        }

        public void Store(string key, string value) => SaveValue(key, value, GetEncryptionAlgorithm());
    }
}
