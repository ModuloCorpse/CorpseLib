namespace CorpseLib.Shell
{
    public class CLI
    {
        private readonly Dictionary<string, Command> m_Commands = [];

        public OperationResult<string> Execute(string command)
        {
            OperationResult<List<string>> splitResult = Helper.SplitCommand(command);
            if (!splitResult)
                return new(splitResult.Error, splitResult.Description);
            string[] args = [..splitResult.Result!];
            if (args.Length == 0)
                return new("Command ill-formed", "Command is empty");
            string commandName = args[0];
            if (m_Commands.TryGetValue(commandName, out Command? cmd))
                return cmd.Call(args[1..]);
            else
                return new("Unknown command", string.Format("Unknown command {0}", commandName));
        }
    }
}
