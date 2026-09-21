using CorpseLib.DataNotation;
using System.Collections;
using System.Text;

namespace CorpseLib.StructuredText
{
    public class Text : IEnumerable<Section>, ICloneable
    {
        public class DataSerializer : ADataSerializer<Text>
        {
            protected override OperationResult<Text> Deserialize(DataObject reader) => new(new(reader.GetList<Section>("sections")));
            protected override void Serialize(Text obj, DataObject writer) => writer["sections"] = obj.m_Sections;
        }

        private readonly List<Section> m_Sections = [];

        public Text() { }
        public Text(string text) => AddText(text);
        public Text(string text, Dictionary<string, object> properties) => AddText(text, properties);
        internal Text(List<Section> sections) => m_Sections = sections;

        public bool IsEmpty => m_Sections.Count == 0;

        private Section AddAndReturn(Section section) { m_Sections.Add(section); return section; }
        public void Add(Section section) => m_Sections.Add(section);
        public Section AddText(string text) => AddAndReturn(new TextSection(text));
        public Section AddText(string text, Dictionary<string, object> properties) => AddAndReturn(new TextSection(text, properties));
        public Section AddImage(string url) => AddAndReturn(new ImageSection(url));
        public Section AddImage(string url, string alt) => AddAndReturn(new ImageSection(url, alt));
        public Section AddImage(string url, Dictionary<string, object> properties) => AddAndReturn(new ImageSection(url, properties));
        public Section AddImage(string url, string alt, Dictionary<string, object> properties) => AddAndReturn(new ImageSection(url, alt, properties));
        public Section AddAnimatedImage(string url) => AddAndReturn(new AnimatedImageSection(url));
        public Section AddAnimatedImage(string url, string alt) => AddAndReturn(new AnimatedImageSection(url, alt));
        public Section AddAnimatedImage(string url, Dictionary<string, object> properties) => AddAndReturn(new AnimatedImageSection(url, properties));
        public Section AddAnimatedImage(string url, string alt, Dictionary<string, object> properties) => AddAndReturn(new AnimatedImageSection(url, alt, properties));

        private Section AddFirstAndReturn(Section section) { m_Sections.Insert(0, section); return section; }
        public void AddFirst(Section section) => m_Sections.Insert(0, section);
        public Section AddTextFirst(string text) => AddFirstAndReturn(new TextSection(text));
        public Section AddTextFirst(string text, Dictionary<string, object> properties) => AddFirstAndReturn(new TextSection(text, properties));
        public Section AddImageFirst(string url) => AddFirstAndReturn(new ImageSection(url));
        public Section AddImageFirst(string url, string alt) => AddFirstAndReturn(new ImageSection(url, alt));
        public Section AddImageFirst(string url, Dictionary<string, object> properties) => AddFirstAndReturn(new ImageSection(url, properties));
        public Section AddImageFirst(string url, string alt, Dictionary<string, object> properties) => AddFirstAndReturn(new ImageSection(url, alt, properties));
        public Section AddAnimatedImageFirst(string url) => AddFirstAndReturn(new AnimatedImageSection(url));
        public Section AddAnimatedImageFirst(string url, string alt) => AddFirstAndReturn(new AnimatedImageSection(url, alt));
        public Section AddAnimatedImageFirst(string url, Dictionary<string, object> properties) => AddFirstAndReturn(new AnimatedImageSection(url, properties));
        public Section AddAnimatedImageFirst(string url, string alt, Dictionary<string, object> properties) => AddFirstAndReturn(new AnimatedImageSection(url, alt, properties));

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
