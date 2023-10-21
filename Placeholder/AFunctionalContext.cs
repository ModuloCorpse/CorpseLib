namespace CorpseLib.Placeholder
{
    public abstract class AFunctionalContext : IContext
    {
        public delegate string Function(string[] args);

        private readonly Dictionary<string, Function> m_Functions = new();

        protected AFunctionalContext()
        {
            m_Functions["Lower"] = (variables) => variables[0].ToLower();
            m_Functions["Upper"] = (variables) => variables[0].ToUpper();
        }

        public void AddFunction(string name, Function function) => m_Functions[name] = function;

        public void AddFunctions(Dictionary<string, Function> functions)
        {
            foreach (var pair in functions)
                m_Functions[pair.Key] = pair.Value;
        }

        public string? Call(string functionName, string[] args)
        {
            if (m_Functions.TryGetValue(functionName, out Function? func))
                return func(args);
            return null;
        }

        public abstract string? GetVariable(string name);
    }
}
