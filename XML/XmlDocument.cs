namespace CorpseLib.XML
{
    public class XmlDocument(XmlElement root) : XmlObject
    {
        private readonly List<XmlDeclaration> m_Declarations = [];
        private readonly XmlElement m_Root = root;

        protected override void AppendToWriter(ref XmlWriter writer)
        {
            foreach (XmlDeclaration declaration in m_Declarations)
                AppendObject(ref writer, declaration);
            AppendObject(ref writer, m_Root);
        }
    }
}
