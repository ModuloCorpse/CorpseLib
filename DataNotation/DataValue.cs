using CorpseLib.Placeholder;

namespace CorpseLib.DataNotation
{
    public class DataValue : DataNode, Context.IVariable
    {
        private readonly object? m_Value;

        public DataValue() => m_Value = null;
        public DataValue(object? value) => m_Value = value;
        public DataValue(DataValue value) => m_Value = value.m_Value;

        public Type Type => m_Value?.GetType() ?? typeof(object);
        public object? Value => m_Value;

        public object? Cast(Type type) => Helper.Cast(m_Value, type);
        public T? ValueCast<T>() => Helper.Cast<T>(m_Value);

        public string ToVariableString() => m_Value?.ToString() ?? string.Empty;
    }
}
