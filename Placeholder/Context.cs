using CorpseLib.Shell;
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
            OperationResult<List<string>> nameSplit = Helper.SplitCommand(name, '.');
            if (!nameSplit)
                return name;
            List<string> variableFields = nameSplit.Result!;
            if (variableFields.Count == 0)
                return name;
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
                                return name;
                            else
                            {
                                MethodInfo? getMethodInfo = propertyInfo.GetMethod;
                                if (getMethodInfo != null && getMethodInfo.GetParameters().Length == 1 && getMethodInfo.GetParameters()[0].ParameterType == typeof(string))
                                    obj = propertyInfo.GetValue(obj, new object[] { variableField });
                                else
                                    return name;
                            }
                        }
                        else
                            obj = fieldInfo.GetValue(obj);
                    }
                    else
                        obj = propertyInfo.GetValue(obj, null);
                    if (obj == null)
                        return name;
                }
                return obj.ToString() ?? name;
            }
            return name;
        }
    }
}
