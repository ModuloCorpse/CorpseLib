using CorpseLib.Datafile;

namespace CorpseLib.Json
{
    public class JsonWriter() : DataFileFormatedWriter<JsonFormat>()
    {
        internal void AppendName(string name)
        {
            m_Builder.Append('"');
            m_Builder.Append(name);
            m_Builder.Append("\":");
        }

        internal void AppendSeparator()
        {
            m_Builder.Append(',');
            LineBreak();
        }

        private void OpenScope(char scope)
        {
            if (!m_Format.InlineScope && m_Builder.Length != 0)
                LineBreak();
            m_Builder.Append(scope);
            Indent();
            LineBreak();
        }

        private void CloseScope(char scope)
        {
            Unindent();
            LineBreak();
            m_Builder.Append(scope);
        }

        internal void OpenObject() => OpenScope('{');
        internal void CloseObject() => CloseScope('}');
        internal void OpenArray() => OpenScope('[');
        internal void CloseArray() => CloseScope(']');
    }
}
