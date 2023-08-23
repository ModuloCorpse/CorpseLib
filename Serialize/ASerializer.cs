namespace CorpseLib.Serialize
{
    public abstract class ASerializer
    {
        internal abstract Type GetSerializedType();
        public override string ToString() => "Serializer[" + GetSerializedType().Name + "]";
    }

    public abstract class ASerializer<TReader, TWriter> : ASerializer
    {
        internal abstract OperationResult<object?> DeserializeObj(TReader reader);
        internal abstract void SerializeObj(object obj, TWriter writer);
    }

    public abstract class ABytesSerializer : ASerializer<BytesReader, BytesWriter>
    {
        public override string ToString() => "BytesSerializer[" + GetSerializedType().Name + "]";
    }

    public abstract class ABytesSerializer<T> : ABytesSerializer
    {
        internal override OperationResult<object?> DeserializeObj(BytesReader reader) => Deserialize(reader).Cast<object?>();
        internal override void SerializeObj(object obj, BytesWriter writer) => Serialize((T)obj, writer);
        internal override Type GetSerializedType() => typeof(T);

        protected abstract OperationResult<T> Deserialize(BytesReader reader);
        protected abstract void Serialize(T obj, BytesWriter writer);

        public override string ToString() => "BytesSerializer[" + GetSerializedType().Name + "]";
    }
}
