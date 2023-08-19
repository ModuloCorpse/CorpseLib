using System.Collections;
using System.Text;

namespace CorpseLib.Ini
{
    public class IniFile : IEnumerable<IniSection>
    {
        private readonly Dictionary<string, IniSection> m_Sections = new();
        private IniSection? m_EmptySection;
        public IniSection? this[string key]
        {
            get => Get(key);
            set => AddSection(value);
        }

        public bool HaveEmptySection => m_EmptySection != null;


        private static IniFile GetParseResult(OperationResult<IniFile> result) => (result && result.Result != null) ? result.Result : new();
        public static IniFile ParseFile(string path) => GetParseResult(TryParseFile(path));
        public static OperationResult<IniFile> TryParseFile(string path) => File.Exists(path) ? TryParse(File.ReadAllText(path)) : new("Cannot load ini file", string.Format("File not found: {0}", path));

        public static IniFile Parse(string content) => GetParseResult(TryParse(content));
        public static OperationResult<IniFile> TryParse(string content)
        {
            IniFile ret = new();
            string[] lines = content.Split(Environment.NewLine);
            IniSection section = new();
            bool hasRead = false;
            int i = 0;
            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    if (line[0] == '[' && line[^1] == ']')
                    {
                        if (hasRead)
                            ret.AddSection(section);
                        section = new(line[1..^1]);
                        hasRead = false;
                    }
                    else
                    {
                        int keyPos = line.IndexOf('=');
                        if (keyPos != -1)
                        {
                            section.Add(line[..keyPos].Trim(), line[(keyPos + 1)..].Trim());
                            hasRead = true;
                        }
                        else
                            return new("Invalid ini file", string.Format("Invalid line {0}", i));
                    }
                }
                ++i;
            }
            if (hasRead)
                ret.AddSection(section);
            return new(ret);
        }

        public IniSection? Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                return m_EmptySection;
            else if (m_Sections.TryGetValue(key, out var section))
                return section;
            return null;
        }

        public IniSection GetOrAdd(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                m_EmptySection ??= new();
                return m_EmptySection;
            }
            else if (!m_Sections.ContainsKey(key))
                m_Sections[key] = new();
            return m_Sections[key];
        }

        public void NewSection(string name)
        {
            if (string.IsNullOrEmpty(name))
                m_EmptySection ??= new();
            else if (!m_Sections.ContainsKey(name))
                m_Sections[name] = new();
        }

        public void AddSection(IniSection? section)
        {
            if (section == null)
                return;
            if (string.IsNullOrEmpty(section.Name))
            {
                if (m_EmptySection == null)
                    m_EmptySection = section;
                else
                    m_EmptySection.Add(section);
            }
            else
            {
                if (!m_Sections.ContainsKey(section.Name))
                    m_Sections[section.Name] = section;
                else
                    m_Sections[section.Name].Add(section);
            }
        }

        public void Merge(IniFile file)
        {
            AddSection(file.m_EmptySection?.Duplicate());
            foreach (IniSection section in file)
                AddSection(section.Duplicate());
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            if (m_EmptySection != null)
                sb.Append(m_EmptySection);
            foreach (IniSection section in m_Sections.Values)
            {
                if (sb.Length > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                }
                sb.Append(section);
            }
            return sb.ToString();
        }

        public void WriteToFile(string path) => File.WriteAllText(path, ToString());

        public IEnumerator<IniSection> GetEnumerator() => ((IEnumerable<IniSection>)m_Sections.Values).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_Sections.Values).GetEnumerator();
    }
}
