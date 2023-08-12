namespace CorpseLib.Json
{
    public class JArray : JNode, IEnumerable<JNode>
    {
        private Type? m_ArrayType = null;
        private readonly List<JNode> m_Children = new();

        public JArray() { }

        public JArray(JArray array)
        {
            m_ArrayType = array.m_ArrayType;
            m_Children = array.m_Children;
        }

        public JArray(List<JNode> children) => Add(children);

        private static Type? GetNodeType(JNode child)
        {
            if (child is JObject)
                return typeof(JObject);
            else if (child is JArray)
                return typeof(JArray);
            else if (child is JValue value)
                return value.Type;
            return null;
        }

        public bool Add(List<JNode> children)
        {
            foreach (JNode child in children)
            {
                if (!Add(child))
                {
                    m_Children.Clear();
                    return false;
                }
            }
            return true;
        }

        public bool Add(JNode child)
        {
            if (m_ArrayType == null)
            {
                m_ArrayType = GetNodeType(child);
                if (m_ArrayType != null)
                {
                    m_Children.Add(child);
                    return true;
                }
            }
            else if (GetNodeType(child) == m_ArrayType)
            {
                m_Children.Add(child);
                return true;
            }
            return false;
        }

        public override void ToJson(ref JBuilder builder)
        {
            builder.OpenArray();
            int i = 0;
            foreach (var child in m_Children)
            {
                if (i++ > 0)
                    builder.AppendSeparator();
                child.ToJson(ref builder);
            }
            builder.CloseArray();
        }

        public IEnumerator<JNode> GetEnumerator() => ((IEnumerable<JNode>)m_Children).GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => ((System.Collections.IEnumerable)m_Children).GetEnumerator();

    }
}
