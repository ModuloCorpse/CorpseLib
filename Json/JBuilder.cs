using System.Text;

namespace CorpseLib.Json
{
    public class JBuilder
    {
        private readonly StringBuilder m_Json = new();
        private readonly JFormat m_Format = new();
        private int m_IndentLevel = 0;

        public JBuilder() {}

        public JBuilder(JFormat format) => m_Format = format;

        internal void AppendValue(string value) => m_Json.Append(value);

        internal void AppendName(string name)
        {
            m_Json.Append('"');
            m_Json.Append(name);
            m_Json.Append("\":");
        }

        internal void LineBreak()
        {
            if (m_Format.DoLineBreak)
                m_Json.AppendLine();
            if (m_Format.DoIndent)
            {
                for (int i = 0; i != m_IndentLevel; ++i)
                    m_Json.Append(m_Format.IndentStr);
            }
        }

        internal void AppendSeparator()
        {
            m_Json.Append(',');
            LineBreak();
        }

        internal void OpenScope(char scope)
        {
            if (!m_Format.InlineScope && m_Json.Length != 0)
                LineBreak();
            m_Json.Append(scope);
            ++m_IndentLevel;
            LineBreak();
        }

        internal void CloseScope(char scope)
        {
            --m_IndentLevel;
            LineBreak();
            m_Json.Append(scope);
        }

        internal void OpenObject() => OpenScope('{');
        internal void CloseObject() => CloseScope('}');
        internal void OpenArray() => OpenScope('[');
        internal void CloseArray() => CloseScope(']');

        public override string ToString() => m_Json.ToString();
    }
}
