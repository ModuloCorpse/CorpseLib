using System.Text;

namespace CorpseLib.Datafile
{
    public class DataFileWriter()
    {
        protected readonly StringBuilder m_Builder = new();

        public int Length => m_Builder.Length;

        public override string ToString() => m_Builder.ToString();

        public void Append(string str) => m_Builder.Append(str);

        public virtual void LineBreak() => m_Builder.AppendLine();
    }
}
