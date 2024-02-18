﻿namespace CorpseLib.Shell
{
    public class CLI(char commandPrefix)
    {
        private readonly Dictionary<string, Command> m_Commands = [];
        private readonly char m_Prefix = commandPrefix;

        public CLI() : this('\0') { }

        public bool AddCommand(Command command)
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
            string[] args = [..splitResult.Result!];
            if (args.Length == 0)
                return new("Command ill-formed", "Command is empty");
            string commandName = args[0];
            if (m_Prefix == '\0' || commandName[0] == m_Prefix)
            {
                commandName = commandName[1..];
                if (m_Commands.TryGetValue(commandName, out Command? cmd))
                    return cmd.Call(args[1..]);
                else
                    return new("Unknown command", string.Format("Unknown command {0}", commandName));
            }
            else
                return new("Command ill-formed", string.Format("Command '{0}' doesn't start with {1}", commandName, m_Prefix));
        }
    }
}
