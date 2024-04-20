using CorpseLib.DataNotation;
using System.Diagnostics.CodeAnalysis;

namespace CorpseLib.StructuredText
{
    public abstract class Section(string content, string alt, Section.Type type) : ICloneable
    {
        public class DataSerializer : ADataSerializer<Section>
        {
            protected override OperationResult<Section> Deserialize(DataObject reader)
            {
                DataNode? propertiesNode = reader.Get("properties");
                if (propertiesNode != null && propertiesNode is DataObject propertiesObj)
                {
                    Dictionary<string, object> properties = (Dictionary<string, object>)DataHelper.Flatten(propertiesObj)!;
                    if (reader.TryGet("content", out string? content) && content != null)
                    {
                        if (reader.TryGet("type", out Type? type) && type != null)
                        {
                            string alt = reader.GetOrDefault("alt", string.Empty);
                            return type switch
                            {
                                Type.TEXT => new(new TextSection(content, alt, properties)),
                                Type.IMAGE => new(new ImageSection(content, alt, properties)),
                                Type.ANIMATED_IMAGE => new(new AnimatedImageSection(content, alt, properties)),
                                _ => new("Bad json", "Unknown section type")
                            };
                        }
                        return new("Bad json", "No section type");
                    }
                    return new("Bad json", "No content");
                }
                return new("Bad json", "No properties");
            }

            protected override void Serialize(Section obj, DataObject writer)
            {
                if (obj.m_Properties.Count == 0)
                    writer["properties"] = new DataObject();
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

        protected readonly Dictionary<string, object> m_Properties = [];
        protected readonly string m_Content = content;
        protected readonly string m_Alt = alt;
        private readonly Type m_Type = type;

        public Dictionary<string, object> Properties => m_Properties;
        public string Content => m_Content;
        public string Alt => m_Alt;
        public Type SectionType => m_Type;

        protected Section(string content, Type type) : this(content, string.Empty, type) { }
        protected Section(string content, Type type, Dictionary<string, object> properties) : this(content, string.Empty, type) => m_Properties = properties;
        protected Section(string content, string alt, Type type, Dictionary<string, object> properties) : this(content, alt, type) => m_Properties = properties;

        public void SetProperties(string key, object value) => m_Properties[key] = value;

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

        public abstract object Clone();
    }

    public class TextSection : Section
    {
        public TextSection(string content, string alt) : base(content, alt, Type.TEXT) { }
        public TextSection(string content) : base(content, Type.TEXT) { }
        public TextSection(string content, string alt, Dictionary<string, object> properties) : base(content, alt, Type.TEXT, properties) { }
        public TextSection(string content, Dictionary<string, object> properties) : base(content, Type.TEXT, properties) { }

        public override object Clone()
        {
            TextSection copy = new(m_Content, m_Alt);
            foreach (var property in m_Properties)
            {
                if (property.Value is ICloneable cloneable)
                    copy.m_Properties[property.Key] = cloneable.Clone();
                else
                    copy.m_Properties[property.Key] = property.Value;
            }
            return copy;
        }
    }

    public class ImageSection : Section
    {
        public ImageSection(string content, string alt) : base(content, alt, Type.IMAGE) { }
        public ImageSection(string content) : base(content, Type.IMAGE) { }
        public ImageSection(string content, string alt, Dictionary<string, object> properties) : base(content, alt, Type.IMAGE, properties) { }
        public ImageSection(string content, Dictionary<string, object> properties) : base(content, Type.IMAGE, properties) { }

        public override object Clone()
        {
            ImageSection copy = new(m_Content, m_Alt);
            foreach (var property in m_Properties)
            {
                if (property.Value is ICloneable cloneable)
                    copy.m_Properties[property.Key] = cloneable.Clone();
                else
                    copy.m_Properties[property.Key] = property.Value;
            }
            return copy;
        }
    }

    public class AnimatedImageSection : Section
    {
        public AnimatedImageSection(string content, string alt) : base(content, alt, Type.ANIMATED_IMAGE) { }
        public AnimatedImageSection(string content) : base(content, Type.ANIMATED_IMAGE) { }
        public AnimatedImageSection(string content, string alt, Dictionary<string, object> properties) : base(content, alt, Type.ANIMATED_IMAGE, properties) { }
        public AnimatedImageSection(string content, Dictionary<string, object> properties) : base(content, Type.ANIMATED_IMAGE, properties) { }

        public override object Clone()
        {
            AnimatedImageSection copy = new(m_Content, m_Alt);
            foreach (var property in m_Properties)
            {
                if (property.Value is ICloneable cloneable)
                    copy.m_Properties[property.Key] = cloneable.Clone();
                else
                    copy.m_Properties[property.Key] = property.Value;
            }
            return copy;
        }
    }
}
