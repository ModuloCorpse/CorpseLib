using CorpseLib.Serialize;

namespace CorpseLib.XML
{

    public class XmlSerializer : Serializer<XmlElement, XmlElement> { }

    public abstract class AXmlSerializer : ASerializer<XmlElement, XmlElement>
    {
        internal override string GetSerializerName() => "XmlSerializer";
    }

    public abstract class AXmlSerializer<T> : AXmlSerializer
    {
        internal override OperationResult<object?> DeserializeObj(XmlElement reader) => Deserialize(reader).Cast<object?>();
        internal override void SerializeObj(object obj, XmlElement writer) => Serialize((T)obj, writer);
        internal override Type GetSerializedType() => typeof(T);

        protected abstract OperationResult<T> Deserialize(XmlElement reader);
        protected abstract void Serialize(T obj, XmlElement writer);
    }
}
