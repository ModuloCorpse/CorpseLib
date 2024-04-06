using System.Text;

namespace CorpseLib.Serialize
{
    public class StringSerializer : Serializer<string, StringBuilder>
    {
        public string Serialize(object obj)
        {
            StringBuilder builder = new();
            Serialize(obj, builder);
            return builder.ToString();
        }
    }

    public abstract class AStringSerializer : ASerializer<string, StringBuilder>
    {
        internal override string GetSerializerName() => "StringSerializer";
    }

    public abstract class AStringSerializer<T> : AStringSerializer
    {
        internal override OperationResult<object?> DeserializeObj(string reader) => Deserialize(reader).Cast<object?>();
        internal override void SerializeObj(object obj, StringBuilder writer) => Serialize((T)obj, writer);
        internal override Type GetSerializedType() => typeof(T);

        protected abstract OperationResult<T> Deserialize(string reader);
        protected abstract void Serialize(T obj, StringBuilder writer);

        public override string ToString() => "StringSerializer[" + GetSerializedType().Name + "]";
    }
}
