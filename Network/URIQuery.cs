using System.Collections;
using System.Text;

namespace CorpseLib.Network
{
    public class URIQuery : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Dictionary<string, string> m_Data = new();
        private readonly char m_Delimiter;
        private bool m_IsEncoded = false;

        public URIQuery(char delimiter) => m_Delimiter = delimiter;
        public URIQuery(string query, char[] queryDelimiters)
        {
            foreach (char queryDelimiter in queryDelimiters)
            {
                if (query.Contains(queryDelimiter))
                {
                    m_Delimiter = queryDelimiter;
                    string[] datas = query.Split(queryDelimiter);
                    foreach (string s in datas)
                    {
                        int n = s.IndexOf('=');
                        if (n >= 0)
                            m_Data[s[..n]] = s[(n + 1)..];
                        else
                            m_Data[s] = string.Empty;
                    }
                    return;
                }
            }
            m_Delimiter = '\0';
            m_Data[query] = string.Empty;
        }

        public string this[string key]
        {
            get => m_Data[key];
            set => Add(key, value);
        }

        public void Add(string key, string value)
        {
            if (m_IsEncoded)
                m_Data[URI.Encode(key)] = URI.Encode(value);
            else
                m_Data[URI.Decode(key)] = URI.Decode(value);
        }

        public bool HaveData(string key) => m_Data.ContainsKey(key);

        public void AddData(Dictionary<string, string> attributes)
        {
            foreach (KeyValuePair<string, string> attr in attributes)
                m_Data[attr.Key] = attr.Value;
        }

        public void Encode()
        {
            Dictionary<string, string> encodedData = m_Data.ToDictionary(entry => URI.Encode(entry.Key), entry => URI.Encode(entry.Value));
            m_Data.Clear();
            AddData(encodedData);
            m_IsEncoded = true;
        }

        public void Decode()
        {
            Dictionary<string, string> decodedData = m_Data.ToDictionary(entry => URI.Decode(entry.Key), entry => URI.Decode(entry.Value));
            m_Data.Clear();
            AddData(decodedData);
            m_IsEncoded = false;
        }

        public string ToDebugString()
        {
            StringBuilder builder = new();
            builder.Append("{ delimiter: '");
            builder.Append(m_Delimiter);
            builder.Append("', data [");
            uint i = 0;
            foreach (KeyValuePair<string, string> data in m_Data)
            {
                if (i != 0)
                    builder.Append(", ");
                builder.Append(data.Key);
                if (!string.IsNullOrEmpty(data.Value))
                {
                    builder.Append('=');
                    builder.Append(data.Value);
                }
                ++i;
            }
            builder.Append("]}");
            return builder.ToString();
        }

        public override string ToString()
        {
            if (m_Data.Count == 0)
                return string.Empty;
            StringBuilder builder = new();
            builder.Append('?');
            uint i = 0;
            foreach (KeyValuePair<string, string> data in m_Data)
            {
                if (i != 0)
                    builder.Append(m_Delimiter);
                builder.Append(data.Key);
                if (!string.IsNullOrEmpty(data.Value))
                {
                    builder.Append('=');
                    builder.Append(data.Value);
                }
                ++i;
            }
            return builder.ToString();
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => ((IEnumerable<KeyValuePair<string, string>>)m_Data).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_Data).GetEnumerator();
    }
}
