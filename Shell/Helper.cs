using System.Text;

namespace CorpseLib.Shell
{
    public class Helper
    {
        public static OperationResult<List<string>> SplitCommand(string content, ref string error)
        {
            List<string> result = new();
            bool inString = false;
            StringBuilder builder = new();
            int i = 0;
            while (i < content.Length)
            {
                char c = content[i];
                switch (c)
                {
                    case ' ':
                    {
                        if (!inString)
                        {
                            if (builder.Length > 0)
                            {
                                result.Add(builder.ToString());
                                builder.Clear();
                            }
                        }
                        else
                            builder.Append(c);
                        break;
                    }
                    case '"':
                    {
                        inString = !inString;
                        if (!inString && (i + 1) < content.Length && content[i + 1] != ' ')
                            return new("Command ill-formed", "String must be followed by a space");
                        if (inString && builder.Length > 0)
                            return new("Command ill-formed", "String must follow a space");
                        break;
                    }
                    case '\\':
                    {
                        ++i;
                        c = content[i];
                        builder.Append(c);
                        break;
                    }
                    default:
                    {
                        builder.Append(c);
                        break;
                    }
                }
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
