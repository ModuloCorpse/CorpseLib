namespace CorpseLib.Serialize
{
    public class BytesSerializer : Serializer<ABytesReader, ABytesWriter>
    {
        static BytesSerializer()
        {
            RegisterDefault(new BoolBytesSerializer());
            RegisterDefault(new CharBytesSerializer());
            RegisterDefault(new IntBytesSerializer());
            RegisterDefault(new UIntBytesSerializer());
            RegisterDefault(new ShortBytesSerializer());
            RegisterDefault(new UShortBytesSerializer());
            RegisterDefault(new LongBytesSerializer());
            RegisterDefault(new ULongBytesSerializer());
            RegisterDefault(new FloatBytesSerializer());
            RegisterDefault(new DoubleBytesSerializer());
            RegisterDefault(new ByteBytesSerializer());
            RegisterDefault(new SByteBytesSerializer());
            RegisterDefault(new GuidBytesSerializer());
            RegisterDefault(new DateTimeBytesSerializer());
            RegisterDefault(new StringTimeBytesSerializer());
            RegisterDefault(new DecimalTimeBytesSerializer());
        }

        public T? Deserialize<T>(byte[] bytes) => Deserialize<T>(new BytesReader(this, bytes)).Result;

        public byte[] Serialize(object obj)
        {
            BytesWriter writer = new(this);
            Serialize(obj, writer);
            return writer.Bytes;
        }
    }

    public abstract class ABytesSerializer : ASerializer<ABytesReader, ABytesWriter>
    {
        internal override string GetSerializerName() => "BytesSerializer";
    }

    public abstract class ABytesSerializer<T> : ABytesSerializer
    {
        internal override OperationResult<object?> DeserializeObj(ABytesReader reader) => Deserialize(reader).Cast<object?>();
        internal override void SerializeObj(object obj, ABytesWriter writer) => Serialize((T)obj, writer);
        internal override Type GetSerializedType() => typeof(T);

        protected abstract OperationResult<T> Deserialize(ABytesReader reader);
        protected abstract void Serialize(T obj, ABytesWriter writer);

        public override string ToString() => "BytesSerializer[" + GetSerializedType().Name + "]";
    }
}
