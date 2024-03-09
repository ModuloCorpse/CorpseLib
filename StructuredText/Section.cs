using CorpseLib.Json;
using System.Diagnostics.CodeAnalysis;

namespace CorpseLib.StructuredText
{
    public class Section
    {
        public class JSerializer : AJsonSerializer<Section>
        {
            protected override OperationResult<Section> Deserialize(JsonObject reader)
            {
                JsonNode? propertiesNode = reader.Get("properties");
                if (propertiesNode != null && propertiesNode is JsonObject propertiesObj)
                {
                    Dictionary<string, object> properties = (Dictionary<string, object>)JsonHelper.Flatten(propertiesObj)!;
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

            protected override void Serialize(Section obj, JsonObject writer)
            {
                if (obj.m_Properties.Count == 0)
                    writer["properties"] = new JsonObject();
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

        private readonly Dictionary<string, object> m_Properties = [];
        private readonly string m_Content;
        private readonly string m_Alt;
        private readonly Type m_Type;

        public Dictionary<string, object> Properties => m_Properties;
        public string Content => m_Content;
        public string Alt => m_Alt;
        public Type SectionType => m_Type;

        internal Section(string content, string alt, Type type)
        {
            m_Content = content;
            m_Alt = alt;
            m_Type = type;
        }

        internal Section(string content, Type type) : this(content, string.Empty, type) { }
        internal Section(string content, string alt, Type type, Dictionary<string, object> properties) : this(content, alt, type) => m_Properties = properties;
        internal Section(string content, Type type, Dictionary<string, object> properties) : this(content, type) => m_Properties = properties;

        public bool TryGetProperties<T>(string key, [NotNullWhen(true)] out T? ret)
        {
            if (m_Properties.TryGetValue(key, out object? obj))
            {
                if (obj is T t)
                {
                    ret = t;
                    return true;
                }
                else
                {

                    T? tmp = Helper.Cast<T>(obj);
                    if (tmp != null)
                    {
                        ret = tmp;
                        return true;
                    }
                }
            }
            ret = default;
            return false;
        }

        public T? GetPropertiesOr<T>(string key, T? defaultValue)
        {
            if (TryGetProperties(key, out T? obj))
                return obj;
            return defaultValue;
        }
    }
}
