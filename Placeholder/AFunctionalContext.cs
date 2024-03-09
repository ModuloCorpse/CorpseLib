namespace CorpseLib.Placeholder
{
    public abstract class AFunctionalContext : IContext
    {
        public delegate string Function(string[] args, Cache cache);

        private readonly Dictionary<string, Function> m_Functions = [];

        protected AFunctionalContext()
        {
            m_Functions["Lower"] = (variables, _) => variables[0].ToLower();
            m_Functions["Upper"] = (variables, _) => variables[0].ToUpper();
        }

        public void AddFunction(string name, Function function) => m_Functions[name] = function;

        public void AddFunctions(Dictionary<string, Function> functions)
        {
            foreach (var pair in functions)
                m_Functions[pair.Key] = pair.Value;
        }

        public virtual string? Call(string functionName, string[] args, Cache cache)
        {
            if (m_Functions.TryGetValue(functionName, out Function? func))
                return func(args, cache);
            return null;
        }

        public abstract string? GetVariable(string name);
    }
}
