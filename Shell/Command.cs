namespace CorpseLib.Shell
{
    public abstract class Command(string name, string description)
    {
        private readonly string m_Name = name;
        private readonly string m_Description = description;
        public string Name => m_Name;
        public string Description => m_Description;
        internal OperationResult<string> Call(string[] args) => Execute(args);
        protected abstract OperationResult<string> Execute(string[] args);
    }
}
