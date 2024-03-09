using System.Text;

namespace CorpseLib.Datafile
{
    public abstract class DataFileReader<TObject, TWriter>() where TObject : DataFileObject<TWriter> where TWriter : DataFileWriter, new()
    {
        private string m_Content = string.Empty;
        private int m_Idx = 0;

        protected bool CanRead => m_Idx != m_Content.Length;
        protected char Current => m_Content[m_Idx];

        internal void SetContent(string content) => m_Content = content;

        protected void Next() => m_Idx++;
        protected void Previous() => m_Idx--;

        private static bool IsWhitespace(char c) => c == ' ' || c == '\t' || c == '\r' || c == '\n' || c == '\v' || c == '\f';

        protected void SkipWhitespace()
        {
            if (m_Idx < m_Content.Length)
            {
                char c = m_Content[m_Idx];
                while (IsWhitespace(c) || (m_Idx == m_Content.Length))
                    c = m_Content[++m_Idx];
            }
        }

        protected bool StartWith(string content)
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

        protected string NextLine()
        {
            StringBuilder lineBuilder = new();
            while (m_Idx != m_Content.Length)
            {
                if (m_Content[m_Idx] == '\r' && m_Content[m_Idx + 1] == '\n')
                {
                    m_Idx += 2;
                    return lineBuilder.ToString();
                }
                else if (m_Content[m_Idx] == '\n')
                {
                    m_Idx++;
                    return lineBuilder.ToString();
                }
                lineBuilder.Append(m_Content[m_Idx]);
                m_Idx++;
            }
            return lineBuilder.ToString();
        }

        public abstract TObject Read();
    }
}
