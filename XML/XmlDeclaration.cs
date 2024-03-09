using System.Collections;
using System.Text;

namespace CorpseLib.XML
{
    public class XmlDeclaration(string name) : XmlTag(name), IEnumerable<KeyValuePair<string, string>>
    {
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => ((IEnumerable<KeyValuePair<string, string>>)m_Attributes).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_Attributes).GetEnumerator();

        protected override void AppendToWriter(ref XmlWriter writer)
        {
            StringBuilder stringBuilder = new("<?");
            AddTagInfoToStringBuilder(stringBuilder);
            stringBuilder.Append("?>");
            writer.Append(stringBuilder.ToString());
        }
    }
}
