using System.Text;

namespace CorpseLib.Shell
{
    public class Helper
    {
        public static string[] Split(string input, char separator)
        {
            OperationResult<List<string>> result = SplitCommand(input, separator);
            if (result)
                return result.Result!.ToArray();
            return Array.Empty<string>();
        }

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
                    if (c == '\\')
                    {
                        ++i;
                        c = content[i];
                        builder.Append(c);
                    }
                    else if (c == stringChar)
                    {
                        inString = false;
                        while ((i + 1) < content.Length && content[i + 1] != separator)
                        {
                            ++i;
                            if (!char.IsWhiteSpace(content[i]))
                                return new("Command ill-formed", string.Format("String must be followed by a '{0}'", separator));
                        }
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
                    {
                        foreach (char builderChar in builder.ToString())
                        {
                            if (!char.IsWhiteSpace(content[i]))
                                return new("Command ill-formed", string.Format("String must follow a '{0}'", separator));
                        }
                        builder.Clear();
                    }
                }
                else if (c == '\\')
                {
                    ++i;
                    c = content[i];
                    builder.Append(c);
                }
                else if (!char.IsWhiteSpace(c))
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

        public static void TrimCommand(ref string content)
        {
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
                        inString = false;
                    builder.Append(c);
                }
                else if (!char.IsWhiteSpace(c))
                {
                    if (c == '"' || c == '\'')
                    {
                        inString = true;
                        stringChar = c;
                        builder.Append(c);
                    }
                    else if (c == '\\')
                    {
                        ++i;
                        c = content[i];
                        builder.Append(c);
                    }
                    else
                        builder.Append(c);
                }
                else
                {
                    if (builder.Length != 0 && builder[^1] != ' ')
                        builder.Append(' ');
                }
                ++i;
            }

            content = builder.ToString();

            if (content.Length > 0 && content[^1] == ' ')
                content = content[..^1];
        }
    }
}
