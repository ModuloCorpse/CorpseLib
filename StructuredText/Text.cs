using CorpseLib.Json;
using System.Collections;
using System.Text;

namespace CorpseLib.StructuredText
{
    public class Text : IEnumerable<Section>
    {
        public class JSerializer : AJSerializer<Text>
        {
            protected override OperationResult<Text> Deserialize(JObject reader) => new(new(reader.GetList<Section>("sections")));
            protected override void Serialize(Text obj, JObject writer) => writer["sections"] = obj.m_Sections;
        }

        private readonly List<Section> m_Sections = [];

        public Text() { }
        public Text(string text) => AddText(text);
        public Text(string text, Dictionary<string, object?> properties) => AddText(text, properties);
        internal Text(List<Section> sections) => m_Sections = sections;

        public void AddText(string text) { if (!string.IsNullOrEmpty(text)) m_Sections.Add(new(text, Section.Type.TEXT)); }
        public void AddText(string text, Dictionary<string, object?> properties) { if (!string.IsNullOrEmpty(text)) m_Sections.Add(new(text, Section.Type.TEXT, properties)); }
        public void AddImage(string url) => m_Sections.Add(new(url, Section.Type.IMAGE));
        public void AddImage(string url, Dictionary<string, object?> properties) => m_Sections.Add(new(url, Section.Type.IMAGE, properties));
        public void AddAnimatedImage(string url) => m_Sections.Add(new(url, Section.Type.ANIMATED_IMAGE));
        public void AddAnimatedImage(string url, Dictionary<string, object?> properties) => m_Sections.Add(new(url, Section.Type.ANIMATED_IMAGE, properties));

        public void AddTextFirst(string text) => m_Sections.Insert(0, new(text, Section.Type.TEXT));
        public void AddTextFirst(string text, Dictionary<string, object?> properties) => m_Sections.Insert(0, new(text, Section.Type.TEXT, properties));
        public void AddImageFirst(string url) => m_Sections.Insert(0, new(url, Section.Type.IMAGE));
        public void AddImageFirst(string url, Dictionary<string, object?> properties) => m_Sections.Insert(0, new(url, Section.Type.IMAGE, properties));
        public void AddAnimatedImageFirst(string url) => m_Sections.Insert(0, new(url, Section.Type.ANIMATED_IMAGE));
        public void AddAnimatedImageFirst(string url, Dictionary<string, object?> properties) => m_Sections.Insert(0, new(url, Section.Type.ANIMATED_IMAGE, properties));

        public void Append(Text text) => m_Sections.AddRange(text.m_Sections);

        public IEnumerator<Section> GetEnumerator() => ((IEnumerable<Section>)m_Sections).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_Sections).GetEnumerator();

        public static Text Concatenate(Text a, Text b)
        {
            Text ret = new();
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
    }
}
