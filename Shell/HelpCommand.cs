using System.Text;

namespace CorpseLib.Shell
{
    public class HelpCommand(string name, CLI cli) : Command(name, string.Empty)
    {
        private readonly CLI m_CLI = cli;

        protected override OperationResult<string> Execute(string[] args)
        {
            StringBuilder sb = new();
            if (args.Length == 0)
            {
                //TODO List help commands
            }
            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "list":
                    {
                        break;
                    }
                    default:
                        break;
                }
            }
            return new(sb.ToString());
        }
    }
}
