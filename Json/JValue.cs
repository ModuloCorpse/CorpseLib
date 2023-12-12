using CorpseLib.Placeholder;

namespace CorpseLib.Json
{
    public class JValue : JNode, Context.IVariable
    {
        private readonly object m_Value;

        public JValue(object value) => m_Value = value;
        public JValue(JValue value) => m_Value = value.m_Value;

        public Type Type => m_Value.GetType();
        public object Value => m_Value;

        public object? Cast(Type type) => Helper.Cast(m_Value, type);
        public T? ValueCast<T>() => Helper.Cast<T>(m_Value);

        public override void ToJson(ref JBuilder builder)
        {
            if (m_Value is string str)
            {
                builder.AppendValue(string.Format("\"{0}\"", str
                    .Replace("\\", "\\\\")
                    .Replace("\"", "\\\"")
                    .Replace("\n", "\\n")
                    .Replace("\r", "\\r")
                    .Replace("\t", "\\t")
                    .Replace("\b", "\\b")
                    .Replace("\f", "\\f")));
            }
            else if (m_Value is bool b)
                builder.AppendValue(b ? "true" : "false");
            else if (m_Value is char c)
            {
                if (c == '"')
                    builder.AppendValue("\"\\\"\"");
                else
                    builder.AppendValue(string.Format("\"{0}\"", c));
            }
            else if (Type.IsEnum)
                builder.AppendValue(((int)Convert.ChangeType(m_Value!, typeof(int))).ToString()!);
            else
                builder.AppendValue(m_Value!.ToString()!.Replace(',', '.'));
        }

        public string ToVariableString() => m_Value.ToString() ?? string.Empty;
    }
}
