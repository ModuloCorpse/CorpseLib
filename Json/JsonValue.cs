using CorpseLib.Placeholder;

namespace CorpseLib.Json
{
    public class JsonValue : JsonNode, Context.IVariable
    {
        private readonly object m_Value;

        public JsonValue(object value) => m_Value = value;
        public JsonValue(JsonValue value) => m_Value = value.m_Value;

        public Type Type => m_Value.GetType();
        public object Value => m_Value;

        public object? Cast(Type type) => Helper.Cast(m_Value, type);
        public T? ValueCast<T>() => Helper.Cast<T>(m_Value);

        protected override void AppendToWriter(ref JsonWriter writer)
        {
            if (m_Value is string str)
            {
                writer.Append(string.Format("\"{0}\"", str
                    .Replace("\\", "\\\\")
                    .Replace("\"", "\\\"")
                    .Replace("\n", "\\n")
                    .Replace("\r", "\\r")
                    .Replace("\t", "\\t")
                    .Replace("\b", "\\b")
                    .Replace("\f", "\\f")));
            }
            else if (m_Value is Guid guid)
                writer.Append(string.Format("\"{0}\"", guid));
            else if (m_Value is DateTime date)
                writer.Append(date.Ticks.ToString());
            else if (m_Value is TimeSpan timeSpan)
                writer.Append(timeSpan.Ticks.ToString());
            else if (m_Value is bool b)
                writer.Append(b ? "true" : "false");
            else if (m_Value is char c)
            {
                if (c == '"')
                    writer.Append("\"\\\"\"");
                else
                    writer.Append(string.Format("\"{0}\"", c));
            }
            else if (Type.IsEnum)
                writer.Append(((int)Convert.ChangeType(m_Value!, typeof(int))).ToString()!);
            else
                writer.Append(m_Value!.ToString()!.Replace(',', '.'));
        }

        public string ToVariableString() => m_Value.ToString() ?? string.Empty;
    }
}
