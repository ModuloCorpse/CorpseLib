using CorpseLib.Serialize;

namespace CorpseLib.Json
{
    public class JsonSerializer : Serializer<JsonObject, JsonObject> { }

    public abstract class AJsonSerializer : ASerializer<JsonObject, JsonObject>
    {
        internal override string GetSerializerName() => "JsonSerializer";
    }

    public abstract class AJsonSerializer<T> : AJsonSerializer
    {
        internal override OperationResult<object?> DeserializeObj(JsonObject reader) => Deserialize(reader).Cast<object?>();
        internal override void SerializeObj(object obj, JsonObject writer) => Serialize((T)obj, writer);
        internal override Type GetSerializedType() => typeof(T);

        protected abstract OperationResult<T> Deserialize(JsonObject reader);
        protected abstract void Serialize(T obj, JsonObject writer);
    }
}
