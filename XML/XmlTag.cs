using System.Text;

namespace CorpseLib.XML
{
    public abstract class XmlTag(string name) : XmlObject()
    {
        protected readonly Dictionary<string, string> m_Attributes = [];
        private readonly string m_Name = name;

        internal IEnumerable<KeyValuePair<string, string>> Attributes => m_Attributes.Skip(0);
        public string Name => m_Name;

        public static string XMLDecode(string value)
        {
            return value.Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Replace("&apos;", "'")
                .Replace("&quot;", "\"")
                .Replace("&amp;", "&");
        }

        public static string XMLEncode(string value)
        {
            return value.Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("'", "&apos;")
                .Replace("\"", "&quot;");
        }

        public bool ContainsAttribute(string key) => m_Attributes.ContainsKey(key);

        public bool AddAttribute(string name, string value)
        {
            //TODO Check name validity
            if (m_Attributes.ContainsKey(name))
                return false;
            m_Attributes[name] = XMLDecode(value);
            return true;
        }

        public string GetAttribute(string name)
        {
            if (m_Attributes.TryGetValue(name, out string? attribute))
                return attribute;
            return string.Empty;
        }

        public bool TryGetAttribute(string name, out string ret)
        {
            if (m_Attributes.TryGetValue(name, out string? attribute))
            {
                ret = attribute;
                return true;
            }
            ret = string.Empty;
            return false;
        }

        protected void AddTagInfoToStringBuilder(StringBuilder builder)
        {
            builder.Append(m_Name);
            foreach (var attribute in m_Attributes)
            {
                builder.Append(' ');
                builder.Append(attribute.Key);
                builder.Append("=\"");
                builder.Append(XMLEncode(attribute.Value));
                builder.Append('"');
            }
        }
    }
}
