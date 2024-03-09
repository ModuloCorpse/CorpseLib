using CorpseLib.Datafile;

namespace CorpseLib.Ini
{
    public class IniWriter() : DataFileWriter()
    {
        public void AppendSection(IniSection section)
        {
            if (!string.IsNullOrEmpty(section.Name))
            {
                m_Builder.Append('[');
                m_Builder.Append(section.Name);
                m_Builder.Append(']');
                m_Builder.AppendLine();
            }

            int i = 0;
            foreach (var pair in section)
            {
                if (i > 0)
                    m_Builder.AppendLine();
                m_Builder.Append(pair.Key);
                m_Builder.Append('=');
                m_Builder.Append(pair.Value);
                ++i;
            }
        }
    }
}
