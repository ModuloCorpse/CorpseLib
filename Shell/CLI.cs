using System.Collections;

namespace CorpseLib.Shell
{
    public class CLI(char commandPrefix) : IEnumerable<Command>
    {
        private readonly Dictionary<string, Command> m_Commands = [];
        private readonly char m_Prefix = commandPrefix;

        public CLI() : this('\0') { }

        public bool Add(Command command)
        {
            if (m_Commands.ContainsKey(command.Name))
                return false;
            m_Commands.Add(command.Name, command);
            return true;
        }

        public OperationResult<string> Execute(string command)
        {
            OperationResult<List<string>> splitResult = Helper.SplitCommand(command);
            if (!splitResult)
                return new(splitResult.Error, splitResult.Description);
            string[] args = [.. splitResult.Result!];
            if (args.Length == 0)
                return new("Command ill-formed", "Command is empty");
            string commandName = args[0];
            if (m_Prefix != '\0')
            {
                if (commandName[0] == m_Prefix)
                    commandName = commandName[1..];
                else
                    return new("Command ill-formed", $"Command '{commandName}' doesn't start with {m_Prefix}");
            }
            if (m_Commands.TryGetValue(commandName, out Command? cmd))
                return cmd.Call(args[1..]);
            else
                return new("Unknown command", $"Unknown command {commandName}");
        }

        public IEnumerator<Command> GetEnumerator() => ((IEnumerable<Command>)m_Commands.Values).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_Commands.Values).GetEnumerator();
    }
}
