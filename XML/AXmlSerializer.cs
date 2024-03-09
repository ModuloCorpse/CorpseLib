using CorpseLib.Serialize;
using System.Reflection;

namespace CorpseLib.XML
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class DefaultXmlSerializer : Attribute { }

    public class XmlSerializer : Serializer<AXmlSerializer>
    {
        private static readonly List<AXmlSerializer> ms_DefaultSerializers = [];
        private static bool ms_IsInit = false;

        public XmlSerializer()
        {
            if (!ms_IsInit)
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.GetCustomAttributes(typeof(DefaultXmlSerializer), true).Length > 0 &&
                            type.IsAssignableTo(typeof(AXmlSerializer)))
                        {
                            AXmlSerializer? serializer = (AXmlSerializer?)Activator.CreateInstance(type);
                            if (serializer != null)
                            {
                                Register(serializer);
                                ms_DefaultSerializers.Add(serializer);
                            }
                        }
                    }
                }
                ms_IsInit = true;
            }
            else
            {
                foreach (AXmlSerializer serializer in ms_DefaultSerializers)
                    Register(serializer);
            }
        }
    }

    public abstract class AXmlSerializer : ASerializer<XmlElement, XmlElement>
    {
        public override string ToString() => "JSerializer[" + GetSerializedType().Name + "]";
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
