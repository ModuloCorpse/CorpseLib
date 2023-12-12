using System.Text;

namespace CorpseLib.Json
{
    public class JReader(string content)
    {
        private readonly string m_Content = content.Trim();
        private int m_Idx = 0;

        private static bool IsWhitespace(char c) => c == ' ' || c == '\t' || c == '\r' || c == '\n' || c == '\v' || c == '\f';

        private void SkipWhitespace()
        {
            if (m_Idx < m_Content.Length)
            {
                char c = m_Content[m_Idx];
                while (IsWhitespace(c) || (m_Idx == m_Content.Length))
                    c = m_Content[++m_Idx];
            }
        }

        private bool StartWith(string content)
        {
            int n = m_Idx;
            int i = 0;
            while (n != m_Content.Length && i != content.Length)
            {
                if (m_Content[n] != content[i])
                    return false;
                ++i;
                ++n;
            }
            if (i == content.Length)
            {
                m_Idx += content.Length;
                return true;
            }
            return false;
        }

        private string ReadString()
        {
            StringBuilder stringBuilder = new();
            while (m_Idx != m_Content.Length)
            {
                char c = m_Content[++m_Idx];
                if (c == '\\')
                {
                    c = m_Content[++m_Idx];
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
                    ++m_Idx;
                    return stringBuilder.ToString();
                }
                else
                    stringBuilder.Append(c);
            }
            throw new JException("Unclosed string");
        }

        private JNull ReadNull()
        {
            if (StartWith("ull"))
                return new();
            throw new JException("Not a valid value");
        }

        private JValue ReadNextValue()
        {
            --m_Idx;
            return ReadValue();
        }

        private double ReadNumber(bool canBeNegative)
        {
            char c = m_Content[m_Idx++];
            double value = 0;
            bool isNegative = false;
            if (c == '-')
            {
                if (canBeNegative)
                    isNegative = true;
                else
                    throw new JException("Missformated number");
            }
            else if (c >= '0' && c <= '9')
                value = (c - '0');
            else
                throw new JException("Misformatted json : Missformated number");
            do
            {
                c = m_Content[m_Idx];
                if (c >= '0' && c <= '9')
                {
                    m_Idx++;
                    value = (value * 10) + (c - '0');
                }
            } while (m_Idx != m_Content.Length && c >= '0' && c <= '9');
            if (m_Idx == m_Content.Length)
                throw new JException("Misformatted json : Json ended with a number");
            return isNegative ? -value : value;
        }

        private JValue ReadValue()
        {
            char c = m_Content[m_Idx];
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
            else
            {
                bool isDecimal = false;
                double value = ReadNumber(true);
                c = m_Content[m_Idx];
                if (c == '.')
                {
                    ++m_Idx;
                    isDecimal = true;
                    double dec = ReadNumber(false);
                    while (dec >= 1)
                        dec /= 10;
                    value += dec;
                }
                c = m_Content[m_Idx];
                if (c == 'E' || c == 'e')
                {
                    ++m_Idx;
                    if (m_Content[m_Idx] == '+')
                        ++m_Idx;
                    bool negativeExponent = false;
                    long exponent = (long)ReadNumber(true);

                    if (exponent < 0)
                    {
                        exponent = -exponent;
                        negativeExponent = true;
                        isDecimal = true;
                    }

                    for (long u = 0; u != exponent; u++)
                    {
                        if (negativeExponent)
                            value /= 10;
                        else
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

        private void ReadArray(JArray array)
        {
            SkipWhitespace();
            while (m_Idx != m_Content.Length)
            {
                char c = m_Content[m_Idx];
                if (c == ']')
                {
                    ++m_Idx;
                    return;
                }
                else if (c != ',')
                {
                    if (!array.Add(ReadNext()))
                        throw new JException("Missformated array: Multiple element type within array");
                }
                else
                    ++m_Idx;
                SkipWhitespace();
            }
            throw new JException("Missformated array: Missing ]");
        }

        private JArray ReadArray()
        {
            JArray array = [];
            ReadArray(array);
            return array;
        }

        private void ReadObject(JObject obj)
        {
            SkipWhitespace();
            while (m_Idx != m_Content.Length)
            {
                char c = m_Content[m_Idx];
                if (c == '}')
                {
                    ++m_Idx;
                    return;
                }
                else if (c == '"')
                {
                    string name = ReadString();
                    SkipWhitespace();
                    c = m_Content[m_Idx];
                    if (c != ':')
                        throw new JException("Missformated object: Missing \":\" after property name");
                    ++m_Idx;
                    SkipWhitespace();
                    obj.Add(name, ReadNext());
                }
                else
                    ++m_Idx;
                SkipWhitespace();
            }
            throw new JException("Missformated object: Missing }");
        }

        private JObject ReadObject()
        {
            JObject obj = [];
            ReadObject(obj);
            return obj;
        }

        public JNode ReadNext()
        {
            char c = m_Content[m_Idx++];
            return c switch
            {
                '{' => ReadObject(),
                '[' => ReadArray(),
                'n' => ReadNull(),
                _ => ReadNextValue(),
            };
        }

        public JObject Read()
        {
            SkipWhitespace();
            if (m_Idx == m_Content.Length)
                throw new JException("Empty Json");
            if (m_Content[m_Idx] != '{')
                throw new JException("Json should start with a {");
            return ReadObject();
        }

        public void Read(JObject obj)
        {
            SkipWhitespace();
            if (m_Idx == m_Content.Length)
                throw new JException("Empty Json");
            if (m_Content[m_Idx] != '{')
                throw new JException("Json should start with a {");
            ReadObject(obj);
        }
    }
}
