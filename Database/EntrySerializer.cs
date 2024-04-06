using CorpseLib.Serialize;

namespace CorpseLib.Database
{
    public class EntrySerializer : Serializer<EntryReader, EntryWriter> { }

    public abstract class AEntrySerializer : ASerializer<EntryReader, EntryWriter>
    {
        internal override string GetSerializerName() => "EntrySerializer";
    }

    public abstract class AEntrySerializer<T> : AEntrySerializer
    {
        internal override OperationResult<object?> DeserializeObj(EntryReader reader) => Deserialize(reader).Cast<object?>();
        internal override void SerializeObj(object obj, EntryWriter writer) => Serialize((T)obj, writer);
        internal override Type GetSerializedType() => typeof(T);
        protected abstract OperationResult<T> Deserialize(EntryReader reader);
        protected abstract void Serialize(T obj, EntryWriter writer);
    }
}
