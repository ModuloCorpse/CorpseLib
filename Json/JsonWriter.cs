using CorpseLib.DataNotation;

namespace CorpseLib.Json
{
    public class JsonWriter() : DataWriter<JsonFormat>()
    {
        private void AppendSeparator()
        {
            m_Builder.Append(',');
            AppendLine();
        }

        private void OpenScope(char scope)
        {
            if (!m_Format.InlineScope && m_Builder.Length != 0)
                AppendLine();
            m_Builder.Append(scope);
            Indent();
            AppendLine();
        }

        private void CloseScope(char scope)
        {
            Unindent();
            AppendLine();
            m_Builder.Append(scope);
        }

        private void AppendObject(DataObject obj)
        {
            OpenScope('{');
            int i = 0;
            foreach (var child in obj)
            {
                if (i++ > 0)
                    AppendSeparator();
                m_Builder.Append('"');
                m_Builder.Append(child.Key);
                m_Builder.Append("\":");
                AppendNode(child.Value);
            }
            CloseScope('}');
        }

        private void AppendArray(DataArray arr)
        {
            OpenScope('[');
            int i = 0;
            foreach (var child in arr)
            {
                if (i++ > 0)
                    AppendSeparator();
                AppendNode(child);
            }
            CloseScope(']');
        }

        private void AppendValue(DataValue value)
        {
            if (value.Value == null)
            {
                Append("null");
                return;
            }
            if (value.Value is string str)
            {
                Append(string.Format("\"{0}\"", str
                    .Replace("\\", "\\\\")
                    .Replace("\"", "\\\"")
                    .Replace("\n", "\\n")
                    .Replace("\r", "\\r")
                    .Replace("\t", "\\t")
                    .Replace("\b", "\\b")
                    .Replace("\f", "\\f")));
            }
            else if (value.Value is Guid guid)
                Append(string.Format("\"{0}\"", guid));
            else if (value.Value is DateTime date)
                Append(date.Ticks.ToString());
            else if (value.Value is TimeSpan timeSpan)
                Append(timeSpan.Ticks.ToString());
            else if (value.Value is bool b)
                Append(b ? "true" : "false");
            else if (value.Value is char c)
            {
                if (c == '"')
                    Append("\"\\\"\"");
                else
                    Append(string.Format("\"{0}\"", c));
            }
            else if (value.Type.IsEnum)
                Append(((int)Convert.ChangeType(value.Value, typeof(int))).ToString()!);
            else
                Append(value.Value.ToString()!.Replace(',', '.'));
        }

        public override void AppendNode(DataNode node)
        {
            if (node is DataObject obj)
                AppendObject(obj);
            else if (node is DataArray arr)
                AppendArray(arr);
            else if (node is DataValue value)
                AppendValue(value);
        }
    }
}
