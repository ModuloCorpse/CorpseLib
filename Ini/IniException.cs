using CorpseLib.Datafile;

namespace CorpseLib.Ini
{
    public class IniException(string message) : DataFileException(message) { }
}
