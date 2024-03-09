using CorpseLib.Datafile;

namespace CorpseLib.XML
{
    public class XmlWriter() : DataFileFormatedWriter<XmlFormat>()
    {
        private void SingleLineLineBreak()
        {
            if (m_Format.LineBreakOnSingleLine && m_Format.DoLineBreak)
                LineBreak();
            else
                m_Builder.Append(' ');
        }

        internal void AppendContent(string content)
        {
            if (content.Contains("\r\n"))
                content = content.Replace("\r\n", "\n");
            if (content.Contains('\n'))
            {
                LineBreak();
                string[] lines = content.Split('\n');
                foreach (string line in lines)
                {
                    m_Builder.Append(XmlTag.XMLEncode(line));
                    LineBreak();
                }
            }
            else
            {
                SingleLineLineBreak();
                m_Builder.Append(XmlTag.XMLEncode(content));
                SingleLineLineBreak();
            }
        }
    }
}
