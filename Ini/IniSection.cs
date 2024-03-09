using CorpseLib.Datafile;
using System.Collections;

namespace CorpseLib.Ini
{
    public class IniSection : DataFileObject<IniWriter>, IEnumerable<KeyValuePair<string, string>>
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

        protected override void AppendToWriter(ref IniWriter writer)
        {
            if (!string.IsNullOrEmpty(m_Name))
            {
                writer.Append(string.Format("[{0}]", m_Name));
                writer.LineBreak();
            }

            int i = 0;
            foreach (var pair in m_Properties)
            {
                if (i > 0)
                    writer.LineBreak();
                writer.Append(string.Format("{0}={1}", pair.Key, pair.Value));
                ++i;
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => ((IEnumerable<KeyValuePair<string, string>>)m_Properties).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_Properties).GetEnumerator();
    }
}
