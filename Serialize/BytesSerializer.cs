namespace CorpseLib.Serialize
{
    public class BytesSerializer : Serializer<ABytesSerializer>
    {
        public T? Deserialize<T>(byte[] bytes)
        {
            BytesReader reader = new(this, bytes);
            return reader.Read<T>().Result;
        }

        public byte[] Serialize(object obj)
        {
            BytesWriter writer = new(this);
            writer.Write(obj);
            return writer.Bytes;
        }
    }
}
