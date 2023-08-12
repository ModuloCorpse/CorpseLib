namespace CorpseLib.Translation
{
    public class TranslationKey
    {
        private readonly string m_Key;

        public string Key => m_Key;

        public TranslationKey(string key) => m_Key = key;

        public override string ToString() => Translator.Translate("${" + m_Key + "}");
        public string ToString(params object[] args) => Translator.Translate("${" + m_Key + "}", args);

        public override bool Equals(object? obj) => obj is TranslationKey key && m_Key == key.m_Key;

        public override int GetHashCode() => HashCode.Combine(m_Key);
    }
}
