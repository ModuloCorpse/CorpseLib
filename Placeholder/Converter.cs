using System.Text;

namespace CorpseLib.Placeholder
{
    public static class Converter
    {
        private class FunctionResult
        {
            private readonly string[] m_Arguments;
            private readonly string m_Result;

            internal FunctionResult(string[] arguments, string result)
            {
                m_Arguments = arguments;
                m_Result = result;
            }

            internal bool MatchArguments(string[] args)
            {
                if (args.Length != m_Arguments.Length)
                    return false;
                for (int i = 0; i < m_Arguments.Length; ++i)
                {
                    if (m_Arguments[i] != args[i])
                        return false;
                }
                return true;
            }

            internal string Result => m_Result;
        }

        private static string TreatVariable(string content, IContext[] contexts, ref Dictionary<string, List<FunctionResult>> fctResults, out bool treated)
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
                string treatedVariableResult = TreatVariable(variable.Trim(), contexts, ref fctResults, out bool isTreated);
                if (isTreated)
                    return treatedVariableResult;
                return TreatVariable(variableReplacement.Trim(), contexts, ref fctResults, out treated);
            }
            else if (content.Contains('('))
            {
                int pos = content.IndexOf('(');
                string functionName = content[..pos];
                string functionVariables = content[(pos + 1)..content.LastIndexOf(')')];
                List<string> argumentsList = [];
                foreach (string functionVariable in functionVariables.Split(','))
                {
                    string treatedVariableResult = TreatVariable(functionVariable.Trim(), contexts, ref fctResults, out bool isTreated);
                    if (!isTreated)
                    {
                        treated = false;
                        return content;
                    }
                    argumentsList.Add(treatedVariableResult);
                }
                string[] arguments = [.. argumentsList];
                if (fctResults.TryGetValue(functionName, out var fctResult))
                {
                    foreach (FunctionResult functionResult in fctResult)
                    {
                        if (functionResult.MatchArguments(arguments))
                            return functionResult.Result;
                    }
                }
                foreach (IContext context in contexts)
                {
                    string? ret = context.Call(functionName, arguments);
                    if (ret != null)
                    {
                        if (!fctResults.ContainsKey(functionName))
                            fctResults[functionName] = [];
                        fctResults[functionName].Add(new(arguments, ret));
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

        public static string Convert(string str, params IContext[] context)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            bool isVariable = false;
            char previous = '\0';
            Dictionary<string, string> treatedVariables = [];
            Dictionary<string, List<FunctionResult>> results = [];
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
                        if (treatedVariables.TryGetValue(variable, out var result))
                            builder.Append(result);
                        else
                        {
                            string treatedVariable = TreatVariable(variable, context, ref results, out bool _);
                            treatedVariables[variable] = treatedVariable;
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
    }
}
