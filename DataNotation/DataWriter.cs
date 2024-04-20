using System.Text;

namespace CorpseLib.DataNotation
{
    public abstract class DataWriter<TFormat> where TFormat : DataFormat, new()
    {
        protected readonly StringBuilder m_Builder = new();
        protected TFormat m_Format = new();
        private int m_IndentLevel = 0;

        public int Length => m_Builder.Length;

        public void Append(string str) => m_Builder.Append(str);

        public void SetFormat(TFormat format) => m_Format = format;

        public void Indent() => m_IndentLevel++;
        public void Unindent() => m_IndentLevel--;

        public void LineBreak()
        {
            if (m_Format.DoLineBreak)
                m_Builder.AppendLine();
            if (m_Format.DoIndent)
            {
                for (int i = 0; i != m_IndentLevel; ++i)
                    m_Builder.Append(m_Format.IndentStr);
            }
        }

        public abstract void AppendNode(DataNode node);

        public override string ToString() => m_Builder.ToString();
    }
}
