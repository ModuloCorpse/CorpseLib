namespace CorpseLib.Serialize
{
    public class BytesSerializer : Serializer<ABytesSerializer>
    {
        public BytesSerializer()
        {
            Register(new BoolBytesSerializer());
            Register(new CharBytesSerializer());
            Register(new IntBytesSerializer());
            Register(new UIntBytesSerializer());
            Register(new ShortBytesSerializer());
            Register(new UShortBytesSerializer());
            Register(new LongBytesSerializer());
            Register(new ULongBytesSerializer());
            Register(new FloatBytesSerializer());
            Register(new DoubleBytesSerializer());
            Register(new ByteBytesSerializer());
            Register(new SByteBytesSerializer());
            Register(new GuidBytesSerializer());
            Register(new DateTimeBytesSerializer());
            Register(new StringTimeBytesSerializer());
            Register(new DecimalTimeBytesSerializer());
        }

        public T? Deserialize<T>(byte[] bytes)
        {
            BytesReader reader = new(this, bytes);
            return reader.SafeRead<T>().Result;
        }

        public byte[] Serialize(object obj)
        {
            BytesWriter writer = new(this);
            writer.Write(obj);
            return writer.Bytes;
        }
    }
}
