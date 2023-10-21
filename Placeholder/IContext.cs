namespace CorpseLib.Placeholder
{
    public interface IContext
    {
        public string? Call(string functionName, string[] args);
        public string? GetVariable(string name);
    }
}
