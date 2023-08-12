namespace CorpseLib.StructuredText
{
    public class Section
    {
        public enum Type
        {
            IMAGE,
            TEXT
        }

        private readonly Dictionary<string, object> m_Properties = new();
        private readonly string m_Content;
        private readonly Type m_Type;

        public string Content => m_Content;
        public Type SectionType => m_Type;
        public object this[string key] => m_Properties[key];

        internal Section(string content, Type type)
        {
            m_Content = content;
            m_Type = type;
        }

        internal Section(string content, Type type, Dictionary<string, object> properties) : this(content, type)
        {
            m_Properties = properties;
        }
    }
}
