namespace CorpseLib.Shell
{
    public abstract class Command(string name, string description)
    {
        private readonly string m_Name = name;
        private readonly string m_Description = description;
        public string Name => m_Name;
        public string Description => m_Description;
        internal async Task<OperationResult<string>> Call(string[] args) => await Execute(args);
        protected abstract Task<OperationResult<string>> Execute(string[] args);
    }
}
