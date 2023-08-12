using System.Reflection;

namespace CorpseLib.Placeholder
{
    public class Context : AFunctionalContext
    {
        private readonly Dictionary<string, object> m_Variables = new();

        public Context() : base() {}

        public void AddVariable(string name, object variable) => m_Variables[name] = variable;

        public void AddVariables(Dictionary<string, object> variables)
        {
            foreach (var pair in variables)
                m_Variables[pair.Key] = pair.Value;
        }

        public override string GetVariable(string name)
        {
            int pos = name.IndexOf('.');
            if (pos != -1)
            {
                string variableName = name[..pos];
                string variableField = name[(pos + 1)..];
                if (m_Variables.TryGetValue(variableName, out object? obj))
                {
                    Type type = obj.GetType();
                    PropertyInfo? propertyInfo = type.GetProperty(variableField);
                    if (propertyInfo == null)
                    {
                        FieldInfo? fieldInfo = type.GetField(variableField);
                        if (fieldInfo == null)
                            return name;
                        else
                            return fieldInfo.GetValue(obj)?.ToString() ?? name;
                    }
                    else
                        return propertyInfo.GetValue(obj, null)?.ToString() ?? name;
                }
                return name;
            }
            else
            {
                if (m_Variables.TryGetValue(name, out object? obj))
                    return obj.ToString() ?? name;
                return name;
            }
        }
    }
}
