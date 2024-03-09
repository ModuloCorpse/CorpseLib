namespace CorpseLib.Placeholder
{
    public interface IContext
    {
        public string? Call(string functionName, string[] args, Cache cache);
        public string? GetVariable(string name);
    }
}
