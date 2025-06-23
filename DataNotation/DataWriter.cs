using System.Text;

namespace CorpseLib.DataNotation
{
    public abstract class DataWriter<TFormat> : FormattedStringBuilder<TFormat> where TFormat : StringBuilderFormat, new()
    {
        public abstract void AppendNode(DataNode node);
    }
}
