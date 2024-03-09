using CorpseLib.Serialize;
using System.Text;

namespace CorpseLib.Json
{
    public class JsonObjectSerializer : ABytesSerializer<JsonObject>
    {
        protected override OperationResult<JsonObject> Deserialize(BytesReader reader)
        {
            try
            {
                return new(JsonParser.Parse(reader.ReadString(reader.ReadInt())));
            } catch (JsonException e)
            {
                return new("Deserialization error", e.Message);
            }
        }

        protected override void Serialize(JsonObject obj, BytesWriter writer)
        {
            string str = obj.ToNetworkString();
            writer.Write(Encoding.UTF8.GetByteCount(str));
            writer.Write(str);
        }
    }
}
