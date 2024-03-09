using System.Collections;
using System.Text;

namespace CorpseLib.XML
{
    public class XmlElement(string name) : XmlTag(name), IEnumerable<XmlElement>
    {
        private readonly List<XmlElement> m_Children = [];
        private string m_Content = string.Empty;

        public string Content => m_Content;
        public int ChildrenCount => m_Children.Count;
        public bool HasContent => m_Children.Count != 0 || !string.IsNullOrEmpty(m_Content);

        public void AddChild(XmlElement element) => m_Children.Add(element);
        public void SetContent(string content) => m_Content = XMLDecode(content);

        public IEnumerator<XmlElement> GetEnumerator() => ((IEnumerable<XmlElement>)m_Children).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_Children).GetEnumerator();

        internal string GetBeginMarkup()
        {
            StringBuilder builder = new('<');
            AddTagInfoToStringBuilder(builder);
            if (HasContent)
                builder.Append('>');
            else
                builder.Append(" />");
            return builder.ToString();
        }

        internal string GetEndMarkup()
        {
            StringBuilder builder = new("</");
            builder.Append(Name);
            builder.Append('>');
            return builder.ToString();
        }

        protected override void AppendToWriter(ref XmlWriter writer)
        {
            writer.Append(GetBeginMarkup());
            if (HasContent)
            {
                writer.Indent();
                if (m_Children.Count > 0)
                {
                    writer.LineBreak();
                    foreach (XmlElement child in m_Children)
                    {
                        AppendObject(ref writer, child);
                        writer.LineBreak();
                    }
                }
                else
                    writer.AppendContent(m_Content);
                writer.Unindent();
                writer.Append(GetEndMarkup());
            }
        }
    }
}
