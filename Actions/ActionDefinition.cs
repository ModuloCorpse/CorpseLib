namespace CorpseLib.Actions
{
    public class ActionDefinition(string name)
    {
        public class ArgumentDefinition(string name, Type type, bool required, object? defaultValue)
        {
            private readonly object? m_DefaultValue = defaultValue;
            private readonly Type m_Type = type;
            private readonly string m_Name = name;
            private readonly bool m_Required = required;

            public object? Default => m_DefaultValue;
            public Type Type => m_Type;
            public string Name => m_Name;
            public bool Required => m_Required;
        }

        private Dictionary<string, ArgumentDefinition> m_Arguments = [];
        private readonly string m_Name = name;

        public ArgumentDefinition[] Arguments => [..m_Arguments.Values];
        public string Name => m_Name;

        public ActionDefinition AddArgument(string name, Type type, bool required, object defaultValue)
        {
            m_Arguments.TryAdd(name, new(name, type, required, defaultValue));
            return this;
        }

        public ActionDefinition AddArgument(string name, Type type, bool required)
        {
            m_Arguments.TryAdd(name, new(name, type, required, null));
            return this;
        }

        public ActionDefinition AddArgument(string name, Type type, object defaultValue) => AddArgument(name, type, true, defaultValue);
        public ActionDefinition AddArgument(string name, Type type) => AddArgument(name, type, true);
        public ActionDefinition AddArgument<T>(string name, T defaultValue) => AddArgument(name, typeof(T), true, defaultValue!);
        public ActionDefinition AddArgument<T>(string name) => AddArgument(name, typeof(T), true);
        public ActionDefinition AddOptionalArgument(string name, Type type, object defaultValue) => AddArgument(name, type, false, defaultValue);
        public ActionDefinition AddOptionalArgument(string name, Type type) => AddArgument(name, type, false);
        public ActionDefinition AddOptionalArgument<T>(string name, T defaultValue) => AddArgument(name, typeof(T), false, defaultValue!);
        public ActionDefinition AddOptionalArgument<T>(string name) => AddArgument(name, typeof(T), false);
    }
}
