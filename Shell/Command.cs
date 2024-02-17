namespace CorpseLib.Shell
{
    public abstract class Command(string name)
    {
        private readonly string m_Name = name;
        public string Name => m_Name;
        internal OperationResult<string> Call(string[] args) => Execute(args);
        protected abstract OperationResult<string> Execute(string[] args);
    }
}
