using CorpseLib.Serialize;
using System.Reflection;

namespace CorpseLib.Json
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class DefaultJsonSerializer : Attribute { }

    public class JsonSerializer : Serializer<AJsonSerializer>
    {
        private static readonly List<AJsonSerializer> ms_DefaultSerializers = [];
        private static bool ms_IsInit = false;

        public JsonSerializer()
        {
            if (!ms_IsInit)
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.GetCustomAttributes(typeof(DefaultJsonSerializer), true).Length > 0 &&
                            type.IsAssignableTo(typeof(AJsonSerializer)))
                        {
                            AJsonSerializer? serializer = (AJsonSerializer?)Activator.CreateInstance(type);
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
                foreach (AJsonSerializer serializer in ms_DefaultSerializers)
                    Register(serializer);
            }
        }
    }

    public abstract class AJsonSerializer : ASerializer<JsonObject, JsonObject>
    {
        public override string ToString() => "JsonSerializer[" + GetSerializedType().Name + "]";
    }

    public abstract class AJsonSerializer<T> : AJsonSerializer
    {
        internal override OperationResult<object?> DeserializeObj(JsonObject reader) => Deserialize(reader).Cast<object?>();
        internal override void SerializeObj(object obj, JsonObject writer) => Serialize((T)obj, writer);
        internal override Type GetSerializedType() => typeof(T);

        protected abstract OperationResult<T> Deserialize(JsonObject reader);
        protected abstract void Serialize(T obj, JsonObject writer);
    }
}
