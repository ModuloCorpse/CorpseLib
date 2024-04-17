using CorpseLib.Json;
using System.Collections;
using System.Text;

namespace CorpseLib.StructuredText
{
    public class Text : IEnumerable<Section>, ICloneable
    {
        public class JSerializer : AJsonSerializer<Text>
        {
            protected override OperationResult<Text> Deserialize(JsonObject reader) => new(new(reader.GetList<Section>("sections")));
            protected override void Serialize(Text obj, JsonObject writer) => writer["sections"] = obj.m_Sections;
        }

        private readonly List<Section> m_Sections = [];

        public Text() { }
        public Text(string text) => AddText(text);
        public Text(string text, Dictionary<string, object> properties) => AddText(text, properties);
        internal Text(List<Section> sections) => m_Sections = sections;

        public bool IsEmpty => m_Sections.Count == 0;

        public void Add(Section section) => m_Sections.Add(section);
        public void AddText(string text) { if (!string.IsNullOrEmpty(text)) m_Sections.Add(new TextSection(text)); }
        public void AddText(string text, Dictionary<string, object> properties) { if (!string.IsNullOrEmpty(text)) m_Sections.Add(new TextSection(text, properties)); }
        public void AddImage(string url) => m_Sections.Add(new ImageSection(url));
        public void AddImage(string url, string alt) => m_Sections.Add(new ImageSection(url, alt));
        public void AddImage(string url, Dictionary<string, object> properties) => m_Sections.Add(new ImageSection(url, properties));
        public void AddImage(string url, string alt, Dictionary<string, object> properties) => m_Sections.Add(new ImageSection(url, alt, properties));
        public void AddAnimatedImage(string url) => m_Sections.Add(new AnimatedImageSection(url));
        public void AddAnimatedImage(string url, string alt) => m_Sections.Add(new AnimatedImageSection(url, alt));
        public void AddAnimatedImage(string url, Dictionary<string, object> properties) => m_Sections.Add(new AnimatedImageSection(url, properties));
        public void AddAnimatedImage(string url, string alt, Dictionary<string, object> properties) => m_Sections.Add(new AnimatedImageSection(url, alt, properties));

        public void AddFirst(Section section) => m_Sections.Insert(0, section);
        public void AddTextFirst(string text) => m_Sections.Insert(0, new TextSection(text));
        public void AddTextFirst(string text, Dictionary<string, object> properties) => m_Sections.Insert(0, new TextSection(text, properties));
        public void AddImageFirst(string url) => m_Sections.Insert(0, new ImageSection(url));
        public void AddImageFirst(string url, string alt) => m_Sections.Insert(0, new ImageSection(url, alt));
        public void AddImageFirst(string url, Dictionary<string, object> properties) => m_Sections.Insert(0, new ImageSection(url, properties));
        public void AddImageFirst(string url, string alt, Dictionary<string, object> properties) => m_Sections.Insert(0, new ImageSection(url, alt, properties));
        public void AddAnimatedImageFirst(string url) => m_Sections.Insert(0, new AnimatedImageSection(url));
        public void AddAnimatedImageFirst(string url, string alt) => m_Sections.Insert(0, new AnimatedImageSection(url, alt));
        public void AddAnimatedImageFirst(string url, Dictionary<string, object> properties) => m_Sections.Insert(0, new AnimatedImageSection(url, properties));
        public void AddAnimatedImageFirst(string url, string alt, Dictionary<string, object> properties) => m_Sections.Insert(0, new AnimatedImageSection(url, alt, properties));

        public void Append(Text text) => m_Sections.AddRange(text.m_Sections);

        public IEnumerator<Section> GetEnumerator() => ((IEnumerable<Section>)m_Sections).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_Sections).GetEnumerator();

        public static Text Concatenate(Text a, Text b)
        {
            Text ret = [];
            ret.Append(a);
            ret.Append(b);
            return ret;
        }

        public override string ToString()
        {
            StringBuilder builder = new();
            foreach (Section section in m_Sections)
            {
                if (section.SectionType == Section.Type.TEXT)
                    builder.Append(section.Content);
            }
            return builder.ToString();
        }

        public object Clone()
        {
            Text clone = [];
            foreach (Section section in m_Sections)
                clone.m_Sections.Add((Section)section.Clone());
            return clone;
        }
    }
}
