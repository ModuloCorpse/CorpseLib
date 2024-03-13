using CorpseLib.Serialize;

namespace CorpseLib.Json
{
    public class JsonObjectSerializer : ABytesSerializer<JsonObject>
    {
        protected override OperationResult<JsonObject> Deserialize(ABytesReader reader)
        {
            try
            {
                return new(JsonParser.Parse(reader.Read<string>()));
            } catch (JsonException e)
            {
                return new("Deserialization error", e.Message);
            }
        }

        protected override void Serialize(JsonObject obj, ABytesWriter writer)
        {
            string str = obj.ToNetworkString();
            writer.Write(str);
        }
    }
}
