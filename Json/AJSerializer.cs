using CorpseLib.Serialize;
using System.Reflection;

namespace CorpseLib.Json
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class DefaultJSerializer : Attribute { }

    public class JSerializer : Serializer<AJSerializer>
    {
        private static readonly List<AJSerializer> ms_DefaultSerializers = [];
        private static bool ms_IsInit = false;

        public JSerializer()
        {
            if (!ms_IsInit)
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.GetCustomAttributes(typeof(DefaultJSerializer), true).Length > 0 &&
                            type.IsAssignableTo(typeof(AJSerializer)))
                        {
                            AJSerializer? serializer = (AJSerializer?)Activator.CreateInstance(type);
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
                foreach (AJSerializer serializer in ms_DefaultSerializers)
                    Register(serializer);
            }
        }
    }

    public abstract class AJSerializer : ASerializer<JObject, JObject>
    {
        public override string ToString() => "JSerializer[" + GetSerializedType().Name + "]";
    }

    public abstract class AJSerializer<T> : AJSerializer
    {
        internal override OperationResult<object?> DeserializeObj(JObject reader) => Deserialize(reader).Cast<object?>();
        internal override void SerializeObj(object obj, JObject writer) => Serialize((T)obj, writer);
        internal override Type GetSerializedType() => typeof(T);

        protected abstract OperationResult<T> Deserialize(JObject reader);
        protected abstract void Serialize(T obj, JObject writer);
    }
}
