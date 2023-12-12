using System.Collections;
using System.Text;

namespace CorpseLib.Ini
{
    public class IniSection : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Dictionary<string, string> m_Properties = [];
        private readonly string m_Name;

        public string Name => m_Name;

        public IniSection() => m_Name = string.Empty;
        public IniSection(string name) => m_Name = name;
        public IniSection(string name, Dictionary<string, string> settings): this(name) => m_Properties = settings;

        public string this[string key]
        {
            get => m_Properties[key];
            set => CheckSet(key.Trim(), value.Trim());
        }

        public bool HaveProperties(string key) => m_Properties.ContainsKey(key);

        private bool CheckSet(string key, string value)
        {
            if (key.Contains('='))
                return false;
            m_Properties[key] = value;
            return true;
        }

        public void Set(IEnumerable<KeyValuePair<string, string>> enumerable)
        {
            foreach (KeyValuePair<string, string> pair in enumerable)
                CheckSet(pair.Key, pair.Value);
        }

        public bool Set(string key, string value) => CheckSet(key.Trim(), value.Trim());

        private bool CheckAdd(string key, string value)
        {
            if (key.Contains('=') || m_Properties.ContainsKey(key))
                return false;
            m_Properties.Add(key, value);
            return true;
        }

        public void Add(IEnumerable<KeyValuePair<string, string>> enumerable)
        {
            foreach (KeyValuePair<string, string> pair in enumerable)
                Add(pair.Key, pair.Value);
        }

        public bool Add(string key, string value) => CheckAdd(key.Trim(), value.Trim());

        public string Get(string key)
        {
            if (m_Properties.TryGetValue(key, out var ret))
                return ret;
            return string.Empty;
        }

        public string GetOrAdd(string key, string value)
        {
            if (m_Properties.TryGetValue(key, out var ret))
                return ret;
            m_Properties[key] = value;
            return value;
        }

        public IniSection Duplicate() => new(m_Name) { m_Properties };

        public override string ToString()
        {
            StringBuilder sb = new();
            if (!string.IsNullOrEmpty(m_Name))
            {
                sb.Append('[');
                sb.Append(m_Name);
                sb.Append(']');
                sb.AppendLine();
            }

            int i = 0;
            foreach (var pair in m_Properties)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.Append(pair.Key);
                sb.Append('=');
                sb.Append(pair.Value);
                ++i;
            }

            return sb.ToString();
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => ((IEnumerable<KeyValuePair<string, string>>)m_Properties).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_Properties).GetEnumerator();
    }
}
