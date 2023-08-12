namespace CorpseLib.Json
{
    public class JFile : JObject
    {
        public static JFile LoadFromFile(string path) => File.Exists(path) ? new(File.ReadAllText(path)) : new();

        public JFile(): base() {}
        public JFile(string content) : base()
        {
            if (!string.IsNullOrEmpty(content))
            {
                JReader reader = new(content);
                reader.Read(this);
            }
        }
        public JFile(JObject json) : base(json) {}

        public void WriteToFile(string path) => File.WriteAllText(path, ToString());
    }
}
