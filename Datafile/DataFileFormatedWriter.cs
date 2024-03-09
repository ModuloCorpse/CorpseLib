namespace CorpseLib.Datafile
{
    public class DataFileFormatedWriter<TFormat>() : DataFileWriter where TFormat : DataFileFormat, new()
    {
        protected TFormat m_Format = new();
        private int m_IndentLevel = 0;

        public void SetFormat(TFormat format) => m_Format = format;

        public void Indent() => m_IndentLevel++;
        public void Unindent() => m_IndentLevel--;

        public override void LineBreak()
        {
            if (m_Format.DoLineBreak)
                base.LineBreak();
            if (m_Format.DoIndent)
            {
                for (int i = 0; i != m_IndentLevel; ++i)
                    m_Builder.Append(m_Format.IndentStr);
            }
        }
    }
}
