using System.Text;

namespace CorpseLib
{
    public class FormattedStringBuilder<TFormat> where TFormat : StringBuilderFormat, new()
    {
        protected readonly StringBuilder m_Builder = new();
        protected TFormat m_Format = new();
        private int m_IndentLevel = 0;

        public int Length => m_Builder.Length;

        public void SetFormat(TFormat format) => m_Format = format;

        public void Indent() => m_IndentLevel++;
        public void Unindent() => m_IndentLevel--;

        public void AppendLine()
        {
            if (m_Format.DoLineBreak)
                m_Builder.AppendLine();
            else if (m_Format.BreakWithSpace)
                m_Builder.Append(' ');
            AppendIndent();
        }

        public void AppendIndent()
        {
            if (m_Format.DoIndent)
            {
                for (int i = 0; i != m_IndentLevel; ++i)
                    m_Builder.Append(m_Format.IndentStr);
            }
        }

        public void Append(bool value) => m_Builder.Append(value);
        public void  Append(sbyte value) => m_Builder.Append(value);
        public void Append(byte value) => m_Builder.Append(value);
        public void Append(char value) => m_Builder.Append(value);
        public void Append(short value) => m_Builder.Append(value);
        public void Append(ushort value) => m_Builder.Append(value);
        public void Append(int value) => m_Builder.Append(value);
        public void Append(uint value) => m_Builder.Append(value);
        public void Append(long value) => m_Builder.Append(value);
        public void Append(ulong value) => m_Builder.Append(value);
        public void Append(float value) => m_Builder.Append(value);
        public void Append(double value) => m_Builder.Append(value);
        public void Append(decimal value) => m_Builder.Append(value);
        public void Append(string value) => m_Builder.Append(value);
        public void Append(object value) => m_Builder.Append(value);

        public void AppendLine(bool value) { Append(value); AppendLine(); }
        public void AppendLine(sbyte value) { Append(value); AppendLine(); }
        public void AppendLine(byte value) { Append(value); AppendLine(); }
        public void AppendLine(char value) { Append(value); AppendLine(); }
        public void AppendLine(short value) { Append(value); AppendLine(); }
        public void AppendLine(ushort value) { Append(value); AppendLine(); }
        public void AppendLine(int value) { Append(value); AppendLine(); }
        public void AppendLine(uint value) { Append(value); AppendLine(); }
        public void AppendLine(long value) { Append(value); AppendLine(); }
        public void AppendLine(ulong value) { Append(value); AppendLine(); }
        public void AppendLine(float value) { Append(value); AppendLine(); }
        public void AppendLine(double value) { Append(value); AppendLine(); }
        public void AppendLine(decimal value) { Append(value); AppendLine(); }
        public void AppendLine(string value) { Append(value); AppendLine(); }
        public void AppendLine(object value) { Append(value); AppendLine(); }

        public override string ToString() => m_Builder.ToString();
    }

    public class FormattedStringBuilder() : FormattedStringBuilder<StringBuilderFormat>() { }
}
