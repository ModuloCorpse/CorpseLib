using CorpseLib.DataNotation;
using System.Text;

namespace CorpseLib.Cmon
{
    public class CmonReader() : DataReader
    {
        internal static readonly char ARRAY_SEPARATOR = ',';

        private string ReadComment()
        {
            StringBuilder stringBuilder = new();
            while (CanRead)
            {
                Next();
                char c = Current;
                if (c == '\\')
                {
                    Next();
                    c = Current;
                    if (c == '*')
                        stringBuilder.Append('*');
                    else
                    {
                        stringBuilder.Append('\\');
                        stringBuilder.Append(c);
                    }
                }
                else if (c == '*')
                {
                    Next();
                    return stringBuilder.ToString();
                }
                else
                    stringBuilder.Append(c);
            }
            throw new CmonException("Unclosed comment");
        }

        private string[] ReadComments()
        {
            List<string> comments = [];
            while (true)
            {
                char c = Current;
                if (char.IsWhiteSpace(c))
                    SkipWhitespace();
                else if (c == '*')
                    comments.Add(ReadComment());
                else
                    return [.. comments];
            }
        }

        private string ReadString()
        {
            StringBuilder stringBuilder = new();
            while (CanRead)
            {
                Next();
                char c = Current;
                if (c == '\\')
                {
                    Next();
                    c = Current;
                    switch (c)
                    {
                        case '\'': stringBuilder.Append('\''); break;
                        case '"': stringBuilder.Append('\"'); break;
                        case '\\': stringBuilder.Append('\\'); break;
                        case 'n': stringBuilder.Append('\n'); break;
                        case 'r': stringBuilder.Append('\r'); break;
                        case 't': stringBuilder.Append('\t'); break;
                        case 'b': stringBuilder.Append('\b'); break;
                        case 'f': stringBuilder.Append('\f'); break;
                        default: stringBuilder.Append('\\'); stringBuilder.Append(c); break;
                    }
                }
                else if (c == '"')
                {
                    Next();
                    return stringBuilder.ToString();
                }
                else
                    stringBuilder.Append(c);
            }
            throw new CmonException("Unclosed string");
        }

        private double ReadNumber(bool canBeNegative)
        {
            char c = Current;
            Next();
            double value = 0;
            bool isNegative = false;
            if (c == '-')
            {
                if (canBeNegative)
                    isNegative = true;
                else
                    throw new CmonException("Missformated number");
            }
            else if (c >= '0' && c <= '9')
                value = (c - '0');
            else
                throw new CmonException("Misformatted cmon : Missformated number");
            do
            {
                c = Current;
                if (c >= '0' && c <= '9')
                {
                    Next();
                    value = (value * 10) + (c - '0');
                }
            } while (CanRead && c >= '0' && c <= '9');
            if (!CanRead)
                throw new CmonException("Misformatted cmon : Cmon ended with a number");
            return isNegative ? -value : value;
        }

        private DataValue ReadValue()
        {
            SkipWhitespace();
            char c = Current;
            if (c == '"')
            {
                string str = ReadString();
                if (str.Length == 1)
                    return new(str[0]);
                return new(str);
            }
            else if (StartWith("true"))
                return new(true);
            else if (StartWith("false"))
                return new(false);
            else if (StartWith("null"))
                return new();
            else
            {
                bool isDecimal = false;
                double value = ReadNumber(true);
                c = Current;
                if (c == '.')
                {
                    Next();
                    isDecimal = true;
                    double dec = ReadNumber(false);
                    while (dec >= 1)
                        dec /= 10;
                    value += dec;
                }
                c = Current;
                if (c == 'E' || c == 'e')
                {
                    Next();
                    if (Current == '+')
                        Next();
                    long exponent = (long)ReadNumber(true);

                    if (exponent < 0)
                    {
                        for (long u = 0; u != (-exponent); u++)
                            value /= 10;
                        isDecimal = true;
                    }
                    else
                    {
                        for (long u = 0; u != exponent; u++)
                            value *= 10;
                    }
                }
                if (!isDecimal)
                {
                    long intValue = (long)value;
                    if (intValue < 0)
                    {
                        if (intValue >= short.MinValue)
                            return new((short)value);
                        else if (intValue >= int.MinValue)
                            return new((int)value);
                        return new((long)value);
                    }
                    else
                    {
                        if (intValue <= short.MaxValue)
                            return new((short)value);
                        else if (intValue <= ushort.MaxValue)
                            return new((ushort)value);
                        else if (intValue <= int.MaxValue)
                            return new((int)value);
                        else if (intValue <= uint.MaxValue)
                            return new((uint)value);
                        else if (intValue <= long.MaxValue)
                            return new((long)value);
                        return new((ulong)value);
                    }
                }
                else
                    return new(value);
            }
        }

        private void ReadArray(DataArray array)
        {
            while (CanRead)
            {
                string[] comments = ReadComments();
                if (CanRead)
                {
                    char c = Current;
                    if (c == ']')
                    {
                        Next();
                        return;
                    }
                    if (c == ARRAY_SEPARATOR)
                        Next();
                    else
                    {
                        DataNode node = ReadNext();
                        node.AddComments(comments);
                        if (!array.Add(node))
                            throw new CmonException("Missformated array: Multiple element type within array");
                    }
                }
            }
            throw new CmonException("Missformated array: Missing ]");
        }

        private string ReadName()
        {
            StringBuilder stringBuilder = new();
            while (CanRead)
            {
                char c = Current;
                if (!char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c) && c != '_' && c != '-' && c != ':')
                    return stringBuilder.ToString();
                else if (!char.IsWhiteSpace(c))
                    stringBuilder.Append(c);
                Next();
            }
            throw new CmonException("Unclosed name");
        }

        private void ReadObject(DataObject obj, bool needClose)
        {
            while (CanRead)
            {
                string[] comments = ReadComments();
                if (CanRead)
                {
                    char c = Current;
                    if (c == '}')
                    {
                        Next();
                        return;
                    }
                    string name = ReadName();
                    DataNode node = ReadNext();
                    node.AddComments(comments);
                    obj.Add(name, node);
                }
            }
            if (needClose)
                throw new CmonException("Missformated object: Missing }");
        }

        private DataNode ReadNext()
        {
            switch (Current)
            {
                case '{':
                {
                    Next();
                    DataObject newObj = [];
                    ReadObject(newObj, true);
                    return newObj;
                }
                case '[':
                {
                    Next();
                    DataArray newArray = [];
                    ReadArray(newArray);
                    return newArray;
                }
                case '=':
                {
                    Next();
                    return ReadValue();
                }
                default:
                    return ReadValue();
            }
        }

        public override DataObject Read()
        {
            if (!CanRead)
                throw new CmonException("Empty Cmon");
            SkipWhitespace();
            string[] comments = ReadComments();
            bool needClose = false;
            if (Current == '{')
            {
                needClose = true;
                Next();
            }
            DataObject obj = [];
            obj.AddComments(comments);
            ReadObject(obj, needClose);
            return obj;
        }
    }
}
