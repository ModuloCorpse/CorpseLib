using CorpseLib.Datafile;
using System.Collections;

namespace CorpseLib.Ini
{
    public class IniFile : DataFileObject<IniWriter>, IEnumerable<IniSection>
    {
        private readonly Dictionary<string, IniSection> m_Sections = [];
        private IniSection? m_EmptySection;
        public IniSection? this[string key]
        {
            get => Get(key);
            set => Add(value);
        }

        public bool HaveEmptySection => m_EmptySection != null;

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
                m_EmptySection ??= [];
                return m_EmptySection;
            }
            else if (!m_Sections.ContainsKey(key))
                m_Sections[key] = [];
            return m_Sections[key];
        }

        public void NewSection(string name)
        {
            if (string.IsNullOrEmpty(name))
                m_EmptySection ??= [];
            else if (!m_Sections.ContainsKey(name))
                m_Sections[name] = [];
        }

        public void Add(IniSection? section)
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
                if (m_Sections.TryGetValue(section.Name, out IniSection? value))
                    value.Add(section);
                else
                    m_Sections[section.Name] = section;
            }
        }

        public void Merge(IniFile file)
        {
            Add(file.m_EmptySection?.Duplicate());
            foreach (IniSection section in file)
                Add(section.Duplicate());
        }

        protected override void AppendToWriter(ref IniWriter writer)
        {
            if (m_EmptySection != null)
                AppendObject(ref writer, m_EmptySection);
            foreach (IniSection section in m_Sections.Values)
            {
                if (writer.Length > 0)
                {
                    writer.LineBreak();
                    writer.LineBreak();
                }
                AppendObject(ref writer, section);
            }
        }

        public void WriteToFile(string path) => File.WriteAllText(path, ToString());

        public IEnumerator<IniSection> GetEnumerator() => ((IEnumerable<IniSection>)m_Sections.Values).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_Sections.Values).GetEnumerator();
    }
}
