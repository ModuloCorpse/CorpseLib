﻿using CorpseLib.DataNotation;

namespace CorpseLib.Json
{
    public class JsonParser : DataParser<JsonReader, JsonWriter, JsonFormat>
    {
        public static readonly JsonFormat NETWORK_FORMAT = new()
        {
            InlineScope = true,
            DoLineBreak = false,
            DoIndent = false,
        };

        public static string NetStr(DataNode node) => Str(node, NETWORK_FORMAT);
    }
}
