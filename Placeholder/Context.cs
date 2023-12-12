using System.Reflection;

namespace CorpseLib.Placeholder
{
    public class Context : AFunctionalContext
    {
        public interface IVariable
        {
            public string ToVariableString();
        }

        private readonly Dictionary<string, object> m_Variables = [];

        public Context() : base() {}

        public void AddVariable(string name, object variable) => m_Variables[name] = variable;

        public void AddVariables(Dictionary<string, object> variables)
        {
            foreach (var pair in variables)
                m_Variables[pair.Key] = pair.Value;
        }

        public override string? GetVariable(string name)
        {
            OperationResult<List<string>> nameSplit = Shell.Helper.SplitCommand(name, '.');
            if (!nameSplit)
                return null;
            List<string> variableFields = nameSplit.Result!;
            if (variableFields.Count == 0)
                return null;
            string variableName = variableFields[0];
            variableFields.RemoveAt(0);
            if (m_Variables.TryGetValue(variableName, out object? obj))
            {
                foreach (var variableField in variableFields)
                {
                    Type type = obj.GetType();
                    PropertyInfo? propertyInfo = type.GetProperty(variableField);
                    if (propertyInfo == null)
                    {
                        FieldInfo? fieldInfo = type.GetField(variableField);
                        if (fieldInfo == null)
                        {
                            propertyInfo = type.GetProperty("Item");
                            if (propertyInfo == null)
                                return null;
                            else
                            {
                                MethodInfo? getMethodInfo = propertyInfo.GetMethod;
                                if (getMethodInfo != null && getMethodInfo.GetParameters().Length == 1 && getMethodInfo.GetParameters()[0].ParameterType == typeof(string))
                                    obj = propertyInfo.GetValue(obj, new object[] { variableField });
                                else
                                    return null;
                            }
                        }
                        else
                            obj = fieldInfo.GetValue(obj);
                    }
                    else
                        obj = propertyInfo.GetValue(obj, null);
                    if (obj == null)
                        return null;
                }
                if (obj is IVariable variableObj)
                    return variableObj.ToVariableString();
                return obj.ToString();
            }
            return null;
        }
    }
}
