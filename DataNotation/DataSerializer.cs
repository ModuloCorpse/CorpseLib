using CorpseLib.Serialize;

namespace CorpseLib.DataNotation
{
    public class DataSerializer : Serializer<DataObject, DataObject> { }

    public abstract class ADataSerializer : ASerializer<DataObject, DataObject>
    {
        internal override string GetSerializerName() => "DataSerializer";
    }

    public abstract class ADataSerializer<T> : ADataSerializer
    {
        internal override OperationResult<object?> DeserializeObj(DataObject reader) => Deserialize(reader).Cast<object?>();
        internal override void SerializeObj(object obj, DataObject writer) => Serialize((T)obj, writer);
        internal override Type GetSerializedType() => typeof(T);

        protected abstract OperationResult<T> Deserialize(DataObject reader);
        protected abstract void Serialize(T obj, DataObject writer);
    }
}
