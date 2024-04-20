using CorpseLib.DataNotation;

namespace CorpseLib.Cmon
{
    public class CmonParser : DataParser<CmonReader, CmonWriter, CmonFormat>
    {
        public static readonly CmonFormat NETWORK_FORMAT = new()
        {
            InlineScope = true,
            OpenScopeFirst = true,
            DoLineBreak = false,
            DoIndent = false,
        };

        public static string NetStr(DataNode node) => Str(node, NETWORK_FORMAT);
    }
}
