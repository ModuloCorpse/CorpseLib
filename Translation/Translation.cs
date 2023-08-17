using System.Collections;
using System.Globalization;
using System.Text;

namespace CorpseLib.Translation
{
    public class Translation : IEnumerable<KeyValuePair<TranslationKey, string>>
    {
        private readonly Dictionary<TranslationKey, string> m_Translations = new();
        private readonly HashSet<CultureInfo> m_CultureInfos = new();
        private readonly bool m_IsDefault;

        public CultureInfo[] CultureInfos => m_CultureInfos.ToArray();
        public bool IsDefault => m_IsDefault;

        public Translation() => m_IsDefault = true;

        public Translation(CultureInfo cultureInfo, bool isDefault = false)
        {
            AddCultureInfo(cultureInfo);
            m_IsDefault = isDefault;
        }

        public Translation(CultureInfo[] cultureInfos, bool isDefault = false)
        {
            AddCultureInfo(cultureInfos);
            m_IsDefault = isDefault;
        }

        public bool HaveKey(TranslationKey key) => m_Translations.ContainsKey(key);

        public void AddCultureInfo(CultureInfo cultureInfo) => m_CultureInfos.Add(cultureInfo);
        public void AddCultureInfo(CultureInfo[] cultureInfos)
        {
            foreach (CultureInfo cultureInfo in cultureInfos)
                m_CultureInfos.Add(cultureInfo);
        }

        public void Clear()
        {
            m_Translations.Clear();
            m_CultureInfos.Clear();
        }

        public void Merge(Translation translation)
        {
            foreach (CultureInfo cultureInfo in translation.m_CultureInfos)
                m_CultureInfos.Add(cultureInfo);
            foreach (var translationPair in translation.m_Translations)
                m_Translations[translationPair.Key] = translationPair.Value;
        }

        public void Add(TranslationKey key, string translation) => m_Translations[key] = translation;
        public void Add(string key, string translation) => Add(new TranslationKey(key), translation);

        public bool TryGetTranslation(TranslationKey key, out string? translation) => m_Translations.TryGetValue(key, out translation);

        public override string ToString()
        {
            StringBuilder builder = new();
            builder.Append('[');
            int i = 0;
            foreach (CultureInfo cultureInfo in m_CultureInfos)
            {
                if (i != 0)
                    builder.Append(", ");
                builder.Append(cultureInfo.Name);
                ++i;
            }
            if (m_IsDefault)
                builder.Append((i == 0) ? "*" : ", *");
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
