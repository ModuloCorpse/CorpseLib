using System.Diagnostics.CodeAnalysis;

namespace CorpseLib.Placeholder
{
    public class Cache
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

        private readonly Dictionary<string, List<FunctionResult>> m_FunctionResults = [];
        private readonly Dictionary<string, string> m_TreatedVariables = [];
        private readonly Dictionary<string, object> m_CustomCache = [];

        internal void CacheFunctionResult(string fctName, string[] args, string result)
        {
            if (!m_FunctionResults.ContainsKey(fctName))
                m_FunctionResults[fctName] = [];
            m_FunctionResults[fctName].Add(new(args, result));
        }

        public bool TryGetFunctionResult(string fctName, string[] args, [MaybeNullWhen(false)] out string? result)
        {
            if (m_FunctionResults.TryGetValue(fctName, out List<FunctionResult>? fctResult))
            {
                foreach (FunctionResult functionResult in fctResult)
                {
                    if (functionResult.MatchArguments(args))
                    {
                        result = functionResult.Result;
                        return true;
                    }
                }
            }
            result = null;
            return false;
        }

        internal void CacheTreatedVariable(string varName, string value) => m_TreatedVariables[varName] = value;
        public bool TryGetTreatedVariable(string varName, [MaybeNullWhen(false)] out string? value) => m_TreatedVariables.TryGetValue(varName, out value);

        public void CacheValue(string name, object value) => m_CustomCache[name] = value;

        public object? GetCachedValue(string name)
        {
            if (m_CustomCache.TryGetValue(name, out var cachedValue))
                return cachedValue;
            return null;
        }

        public T? GetCachedValue<T>(string name)
        {
            object? cachedValue = GetCachedValue(name);
            if (cachedValue is T ret)
                return ret;
            return default;
        }
    }
}
