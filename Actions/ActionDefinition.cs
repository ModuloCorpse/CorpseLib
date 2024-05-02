namespace CorpseLib.Actions
{
    public class ActionDefinition(string name, string description)
    {
        public class ArgumentDefinition(string name, string description, Type type, bool required, object? defaultValue)
        {
            private readonly object? m_DefaultValue = defaultValue;
            private readonly Type m_Type = type;
            private readonly string m_Name = name;
            private readonly string m_Description = description;
            private readonly bool m_Required = required;

            public object? Default => m_DefaultValue;
            public Type Type => m_Type;
            public string Name => m_Name;
            public string Description => m_Description;
            public bool Required => m_Required;
        }

        private Dictionary<string, ArgumentDefinition> m_Arguments = [];
        private readonly string m_Name = name;
        private readonly string m_Description = description;

        public ArgumentDefinition[] Arguments => [..m_Arguments.Values];
        public string Name => m_Name;
        public string Description => m_Description;

        public ActionDefinition AddArgument(string name, string description, Type type, bool required, object defaultValue)
        {
            m_Arguments.TryAdd(name, new(name, description, type, required, defaultValue));
            return this;
        }

        public ActionDefinition AddArgument(string name, string description, Type type, bool required)
        {
            m_Arguments.TryAdd(name, new(name, description, type, required, null));
            return this;
        }

        public ActionDefinition AddArgument(string name, string description, Type type, object defaultValue) => AddArgument(name, description, type, true, defaultValue);
        public ActionDefinition AddArgument(string name, string description, Type type) => AddArgument(name, description, type, true);
        public ActionDefinition AddArgument<T>(string name, string description, T defaultValue) => AddArgument(name, description, typeof(T), true, defaultValue!);
        public ActionDefinition AddArgument<T>(string name, string description) => AddArgument(name, description, typeof(T), true);
        public ActionDefinition AddOptionalArgument(string name, string description, Type type, object defaultValue) => AddArgument(name, description, type, false, defaultValue);
        public ActionDefinition AddOptionalArgument(string name, string description, Type type) => AddArgument(name, description, type, false);
        public ActionDefinition AddOptionalArgument<T>(string name, string description, T defaultValue) => AddArgument(name, description, typeof(T), false, defaultValue!);
        public ActionDefinition AddOptionalArgument<T>(string name, string description) => AddArgument(name, description, typeof(T), false);
    }
}
