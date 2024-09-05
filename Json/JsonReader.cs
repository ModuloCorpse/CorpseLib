using CorpseLib.DataNotation;
using System.Text;

namespace CorpseLib.Json
{
    public class JsonReader() : DataReader()
    {
        private string ReadString()
        {
            StringBuilder stringBuilder = new();
            while (CanRead)
            {
                Next();
                if (CanRead)
                {
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
            }
            throw new JsonException("Unclosed string");
        }

        private DataValue ReadNextValue()
        {
            Previous();
            return ReadValue();
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
                    throw new JsonException("Missformated number");
            }
            else if (c >= '0' && c <= '9')
                value = (c - '0');
            else
                throw new JsonException("Misformatted json : Missformated number");
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
                throw new JsonException("Misformatted json : Json ended with a number");
            return isNegative ? -value : value;
        }

        private DataValue ReadValue()
        {
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
            SkipWhitespace();
            while (CanRead)
            {
                char c = Current;
                if (c == ']')
                {
                    Next();
                    return;
                }
                else if (c != ',')
                {
                    if (!array.Add(ReadNext()))
                        throw new JsonException("Missformated array: Multiple element type within array");
                }
                else
                    Next();
                SkipWhitespace();
            }
            throw new JsonException("Missformated array: Missing ]");
        }

        private DataArray ReadArray()
        {
            DataArray array = [];
            ReadArray(array);
            return array;
        }

        private void ReadObject(DataObject obj)
        {
            SkipWhitespace();
            while (CanRead)
            {
                char c = Current;
                if (c == '}')
                {
                    Next();
                    return;
                }
                else if (c == '"')
                {
                    string name = ReadString();
                    SkipWhitespace();
                    c = Current;
                    if (c != ':')
                        throw new JsonException("Missformated object: Missing \":\" after property name");
                    Next();
                    SkipWhitespace();
                    obj.Add(name, ReadNext());
                }
                else
                    Next();
                SkipWhitespace();
            }
            throw new JsonException("Missformated object: Missing }");
        }

        private DataObject ReadObject()
        {
            DataObject obj = [];
            ReadObject(obj);
            return obj;
        }

        private DataNode ReadNext()
        {
            SkipWhitespace();
            char c = Current;
            Next();
            return c switch
            {
                '{' => ReadObject(),
                '[' => ReadArray(),
                _ => ReadNextValue(),
            };
        }

        public override DataObject Read()
        {
            SkipWhitespace();
            if (!CanRead)
                throw new JsonException("Empty Json");
            if (Current != '{')
                throw new JsonException("Json should start with a {");
            return ReadObject();
        }
    }
}
