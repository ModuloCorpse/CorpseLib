using System.Collections;
using System.Globalization;
using System.Text;

namespace CorpseLib.Translation
{
    public class Translation : IEnumerable<KeyValuePair<TranslationKey, string>>
    {
        private readonly Dictionary<TranslationKey, string> m_Translations = new();
        private readonly CultureInfo m_CultureInfo;

        public CultureInfo CultureInfo => m_CultureInfo;

        public Translation(CultureInfo cultureInfo) => m_CultureInfo = cultureInfo;

        public void SetTranslation(TranslationKey key, string translation) => m_Translations[key] = translation;
        public void Add(TranslationKey key, string translation)
        {
            if (!m_Translations.ContainsKey(key))
                m_Translations[key] = translation;
        }

        public bool TryGetTranslation(TranslationKey key, out string? translation) => m_Translations.TryGetValue(key, out translation);

        public override string ToString()
        {
            StringBuilder builder = new();
            builder.Append('[');
            builder.Append(m_CultureInfo.Name);
            builder.Append(']');
            builder.AppendLine();
            foreach (var pair in m_Translations)
            {
                builder.Append(pair.Key.Key);
                builder.Append(':');
                builder.Append(pair.Value);
                builder.AppendLine();
            }
            return builder.ToString();
        }

        public IEnumerator<KeyValuePair<TranslationKey, string>> GetEnumerator() => m_Translations.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => m_Translations.GetEnumerator();
    }
}
