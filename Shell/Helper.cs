using System.Text;

namespace CorpseLib.Shell
{
    public class Helper
    {
        public static OperationResult<List<string>> SplitCommand(string content, char separator = ' ')
        {
            if (separator == '"' || separator == '\'' || separator == '\\')
                return new("Bad split call", "Separator cannot be a '\"', a ''' or a '\\'");
            List<string> result = new();
            bool inString = false;
            StringBuilder builder = new();
            int i = 0;
            char stringChar = '\0';
            while (i < content.Length)
            {
                char c = content[i];
                if (inString)
                {
                    if (c == stringChar)
                    {
                        inString = false;
                        if ((i + 1) < content.Length && content[i + 1] != separator)
                            return new("Command ill-formed", string.Format("String must be followed by a '{0}'", separator));
                    }
                    else
                        builder.Append(c);
                }
                else if (c == separator)
                {
                    if (builder.Length > 0)
                    {
                        result.Add(builder.ToString());
                        builder.Clear();
                    }
                }
                else if (c == '"' || c == '\'')
                {
                    inString = true;
                    stringChar = c;
                    if (builder.Length > 0)
                        return new("Command ill-formed", string.Format("String must follow a '{0}'", separator));
                }
                else if (c == '\\')
                {
                    ++i;
                    c = content[i];
                    builder.Append(c);
                }
                else
                    builder.Append(c);
                ++i;
            }
            if (builder.Length > 0)
            {
                result.Add(builder.ToString());
                builder.Clear();
            }
            return new(result);
        }
    }
}
