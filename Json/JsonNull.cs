namespace CorpseLib.Json
{
    public class JsonNull : JsonNode
    {
        protected override void AppendToWriter(ref JsonWriter writer) => writer.Append("null");
    }
}
