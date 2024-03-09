using CorpseLib.Datafile;

namespace CorpseLib.Json
{
    public class JsonException(string message) : DataFileException(message) { }
}
