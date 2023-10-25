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
                writer["properties"] = obj.m_Properties;
                writer["content"] = obj.m_Content;
                writer["type"] = obj.m_Type;
            }
        }

        public enum Type
        {
            IMAGE,
            TEXT
        }

        private readonly Dictionary<string, object?> m_Properties = new();
        private readonly string m_Content;
        private readonly Type m_Type;

        public Dictionary<string, object?> Properties => m_Properties;
        public string Content => m_Content;
        public Type SectionType => m_Type;
        public object? this[string key] => m_Properties[key];

        internal Section(string content, Type type)
        {
            m_Content = content;
            m_Type = type;
        }

        internal Section(string content, Type type, Dictionary<string, object?> properties) : this(content, type) => m_Properties = properties;
    }
}
