using CorpseLib.Serialize;

namespace CorpseLib.Json
{
    public abstract class JSerializer : Serializer<JObject, JObject>
    {
        public static new JSerializer? GetSerializerFor(string assemblyQualifiedName) => (JSerializer?)Serializer<JObject, JObject>.GetSerializerFor(assemblyQualifiedName);
        public static new JSerializer? GetSerializerFor(Type type) => (JSerializer?)Serializer<JObject, JObject>.GetSerializerFor(type);
        public override string ToString() => "JSerializer[" + GetSerializedType().Name + "]";
    }

    public abstract class JSerializer<T> : JSerializer
    {
        internal override OperationResult<object?> DeserializeObj(JObject reader) => Deserialize(reader).Cast<object?>();
        internal override void SerializeObj(object obj, JObject writer) => Serialize((T)obj, writer);
        internal override Type GetSerializedType() => typeof(T);

        protected abstract OperationResult<T> Deserialize(JObject reader);
        protected abstract void Serialize(T obj, JObject writer);
    }
}
