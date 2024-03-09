using System.Text;

namespace CorpseLib.Placeholder
{
    public static class Converter
    {
        private static string TreatVariable(string content, IContext[] contexts, Cache cache, out bool treated)
        {
            treated = true;
            if (string.IsNullOrEmpty(content))
                return string.Empty;
            if (content[0] == '@')
            {
                if (content.Length > 1)
                    return content[1..];
                return content;
            }
            else if (content[0] == '\'' && content[^1] == '\'')
            {
                if (content.Length > 1)
                    return content[1..^1];
                return content;
            }
            else if (content.Contains(" || "))
            {
                int pos = content.IndexOf(" || ");
                string variable = content[..pos];
                string variableReplacement = content[(pos + 4)..];
                string treatedVariableResult = TreatVariable(variable.Trim(), contexts, cache, out bool isTreated);
                if (isTreated)
                    return treatedVariableResult;
                return TreatVariable(variableReplacement.Trim(), contexts, cache, out treated);
            }
            else if (content.Contains('('))
            {
                int pos = content.IndexOf('(');
                string functionName = content[..pos];
                string functionVariables = content[(pos + 1)..content.LastIndexOf(')')];
                List<string> argumentsList = [];
                foreach (string functionVariable in functionVariables.Split(','))
                {
                    string treatedVariableResult = TreatVariable(functionVariable.Trim(), contexts, cache, out bool isTreated);
                    if (!isTreated)
                    {
                        treated = false;
                        return content;
                    }
                    argumentsList.Add(treatedVariableResult);
                }
                string[] arguments = [.. argumentsList];
                if (cache.TryGetFunctionResult(functionName, arguments, out string? cachedResult) && cachedResult != null)
                    return cachedResult;
                foreach (IContext context in contexts)
                {
                    string? ret = context.Call(functionName, arguments, cache);
                    if (ret != null)
                    {
                        cache.CacheFunctionResult(functionName, arguments, ret);
                        return ret;
                    }
                }
                treated = false;
                return content;
            }
            foreach (IContext context in contexts)
            {
                string? variable = context.GetVariable(content);
                if (variable != null)
                    return variable;
            }
            treated = false;
            return content;
        }

        public static string Convert(string str, params IContext[] context) => Convert(str, new(), context);

        public static string Convert(string str, Cache cache, params IContext[] context)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            bool isVariable = false;
            char previous = '\0';
            StringBuilder builder = new();
            StringBuilder variableBuilder = new();
            foreach (char c in str)
            {
                if (isVariable)
                {
                    if (c == '}')
                    {
                        isVariable = false;
                        string variable = variableBuilder.ToString();
                        if (cache.TryGetTreatedVariable(variable, out var result))
                            builder.Append(result);
                        else
                        {
                            string treatedVariable = TreatVariable(variable, context, cache, out bool _);
                            cache.CacheTreatedVariable(variable, treatedVariable);
                            builder.Append(treatedVariable);
                        }
                        variableBuilder.Clear();
                    }
                    else
                        variableBuilder.Append(c);
                }
                else if (previous == '$')
                {
                    if (c == '{')
                        isVariable = true;
                    else
                    {
                        builder.Append('$');
                        builder.Append(c);
                    }
                }
                else if (c != '$')
                    builder.Append(c);
                previous = c;
            }
            return builder.ToString();
        }

        //TODO Create analytics tool to get all variables from placeholder string
    }
}
