namespace CorpseLib.Json
{
    public class JsonArray : JsonNode, IEnumerable<JsonNode>
    {
        private Type? m_ArrayType = null;
        private readonly List<JsonNode> m_Children = [];

        public JsonArray() { }

        public JsonArray(JsonArray array)
        {
            m_ArrayType = array.m_ArrayType;
            m_Children = array.m_Children;
        }

        public JsonArray(List<JsonNode> children) => Add(children);

        private static Type? GetNodeType(JsonNode child)
        {
            if (child is JsonObject)
                return typeof(JsonObject);
            else if (child is JsonArray)
                return typeof(JsonArray);
            else if (child is JsonValue value)
                return value.Type;
            return null;
        }

        public bool Add(List<JsonNode> children)
        {
            foreach (JsonNode child in children)
            {
                if (!Add(child))
                {
                    m_Children.Clear();
                    return false;
                }
            }
            return true;
        }

        public bool Add(JsonNode child)
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

        protected override void AppendToWriter(ref JsonWriter writer)
        {
            writer.OpenArray();
            int i = 0;
            foreach (var child in m_Children)
            {
                if (i++ > 0)
                    writer.AppendSeparator();
                AppendObject(ref writer, child);
            }
            writer.CloseArray();
        }

        public IEnumerator<JsonNode> GetEnumerator() => ((IEnumerable<JsonNode>)m_Children).GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => ((System.Collections.IEnumerable)m_Children).GetEnumerator();

    }
}
