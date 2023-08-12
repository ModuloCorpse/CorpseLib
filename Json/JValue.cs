namespace CorpseLib.Json
{
    public class JValue : JNode
    {
        private readonly object m_Value;

        public JValue(object value) => m_Value = value;
        public JValue(JValue value) => m_Value = value.m_Value;

        public Type Type => m_Value.GetType();

        public object? Cast(Type type)
        {
            if (m_Value.GetType() == type)
                return m_Value;
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (m_Value == null)
                    return default;
                type = Nullable.GetUnderlyingType(type)!;
            }
            if (type.IsEnum)
            {
                int enumValue = (int)Convert.ChangeType(m_Value, typeof(int));
                return Enum.ToObject(type, enumValue);
            }
            return Convert.ChangeType(m_Value, type);
        }

        public T? Cast<T>() => (T?)Cast(typeof(T));

        public override void ToJson(ref JBuilder builder)
        {
            if (m_Value is string str)
                builder.AppendValue(string.Format("\"{0}\"", str));
            else if (m_Value is bool b)
                builder.AppendValue(b ? "true" : "false");
            else if (m_Value is char c)
                builder.AppendValue(string.Format("\"{0}\"", c));
            else if (Type.IsEnum)
                builder.AppendValue(((int)Convert.ChangeType(m_Value!, typeof(int))).ToString()!);
            else
                builder.AppendValue(m_Value!.ToString()!.Replace(',', '.'));
        }
    }
}
