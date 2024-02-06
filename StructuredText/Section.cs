using CorpseLib.Json;

namespace CorpseLib.StructuredText
{
    public class Section
    {
        public class JSerializer : AJSerializer<Section>
        {
            protected override OperationResult<Section> Deserialize(JObject reader)
            {
                JNode? propertiesNode = reader.Get("properties");
                if (propertiesNode != null && propertiesNode is JObject propertiesObj)
                {
                    Dictionary<string, object?> properties = (Dictionary<string, object?>)JHelper.Flatten(propertiesObj)!;
                    if (reader.TryGet("content", out string? content))
                    {
                        if (reader.TryGet("type", out Type? type))
                        {
                            if (reader.TryGet("alt", out string? alt))
                                return new(new(content!, alt!, (Type)type!, properties));
                            else
                                return new(new(content!, (Type)type!, properties));
                        }
                        return new("Bad json", "No section type");
                    }
                    return new("Bad json", "No content");
                }
                return new("Bad json", "No properties");
            }

            protected override void Serialize(Section obj, JObject writer)
            {
                if (obj.m_Properties.Count == 0)
                    writer["properties"] = new JObject();
                else
                    writer["properties"] = obj.m_Properties;
                writer["content"] = obj.m_Content;
                if (!string.IsNullOrEmpty(obj.m_Alt))
                    writer["alt"] = obj.m_Alt;
                writer["type"] = obj.m_Type;
            }
        }

        public enum Type
        {
            TEXT,
            IMAGE,
            ANIMATED_IMAGE
        }

        private readonly Dictionary<string, object?> m_Properties = [];
        private readonly string m_Content;
        private readonly string m_Alt;
        private readonly Type m_Type;

        public Dictionary<string, object?> Properties => m_Properties;
        public string Content => m_Content;
        public string Alt => m_Alt;
        public Type SectionType => m_Type;
        public object? this[string key] => m_Properties[key];

        internal Section(string content, string alt, Type type)
        {
            m_Content = content;
            m_Alt = alt;
            m_Type = type;
        }

        internal Section(string content, Type type) : this(content, string.Empty, type) { }
        internal Section(string content, string alt, Type type, Dictionary<string, object?> properties) : this(content, alt, type) => m_Properties = properties;
        internal Section(string content, Type type, Dictionary<string, object?> properties) : this(content, type) => m_Properties = properties;
    }
}
