namespace CorpseLib.Datafile
{
    public class DataFileParser<TObject, TWriter, TReader>
        where TReader : DataFileReader<TObject, TWriter>, new()
        where TObject : DataFileObject<TWriter>
        where TWriter : DataFileWriter, new()
    {
        public static TObject LoadFromFile(string path) => File.Exists(path) ? Parse(File.ReadAllText(path)) : throw new FileNotFoundException();

        public static TObject Parse(string content)
        {
            TReader reader = new();
            reader.SetContent(content);
            return reader.Read();
        }

        public static void WriteToFile(string path, TObject node) => File.WriteAllText(path, node.ToString());

    }
}
