using System.Reflection;

namespace CorpseLib.Serialize
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class DefaultSerializer : Attribute { }

    public abstract class Serializer<TReader, TWriter>
    {
        private class SerializerTreeNode
        {
            private readonly Serializer<TReader, TWriter>? m_Serializer;
            private readonly Dictionary<Type, SerializerTreeNode> m_Children = new();

            public SerializerTreeNode(Serializer<TReader, TWriter> serializer) => m_Serializer = serializer;
            public SerializerTreeNode() => m_Serializer = null;

            public void Clear() => m_Children.Clear();

            public Type GetSerializerType() => m_Serializer!.GetSerializedType();

            public void AppendChild(KeyValuePair<Type, SerializerTreeNode> child) => m_Children[child.Key] = child.Value;

            public bool Insert(SerializerTreeNode node)
            {
                List<Type> typeToRemove = new();
                Type serializerType = node.GetSerializerType();
                foreach (var child in m_Children)
                {
                    if (child.Key.IsAssignableTo(serializerType))
                    {
                        typeToRemove.Add(child.Key);
                        node.AppendChild(child);
                    }
                }
                foreach (Type type in typeToRemove)
                    m_Children.Remove(type);
                bool ret = false;
                bool inserted = false;
                foreach (var child in m_Children)
                {
                    if (child.Key.IsAssignableFrom(serializerType))
                    {
                        ret = ret || child.Value.Insert(node);
                        inserted = true;
                    }
                }
                if (!inserted)
                {
                    m_Children[serializerType] = node;
                    ret = true;
                }
                return ret;
            }

            public void Search(Type type, uint depth, ref List<Tuple<uint, Serializer<TReader, TWriter>>> list)
            {
                bool found = false;
                foreach (var child in m_Children)
                {
                    if (child.Key.IsAssignableFrom(type))
                    {
                        child.Value.Search(type, depth + 1, ref list);
                        found = true;
                    }
                }
                if (!found && m_Serializer != null)
                    list.Add(new(depth, m_Serializer));
            }

            public Serializer<TReader, TWriter>? Search(string assemblyQualifiedName)
            {
                foreach (var child in m_Children)
                {
                    if (child.Key.AssemblyQualifiedName == assemblyQualifiedName)
                        return child.Value.m_Serializer;
                }
                foreach (var child in m_Children)
                {
                    Serializer<TReader, TWriter>? serializer = child.Value.Search(assemblyQualifiedName);
                    if (serializer != null)
                        return serializer;
                }
                return null;
            }
        }

        private class SerializerTreeRoot
        {
            private readonly SerializerTreeNode m_Classes = new();
            private readonly SerializerTreeNode m_Interfaces = new();

            public SerializerTreeRoot()
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.GetCustomAttributes(typeof(DefaultSerializer), true).Length > 0 &&
                            type.IsAssignableTo(typeof(Serializer<TReader, TWriter>)))
                        {
                            Serializer<TReader, TWriter>? serializer = (Serializer<TReader, TWriter>?)Activator.CreateInstance(type);
                            if (serializer != null)
                                Insert(serializer);
                        }
                    }
                }
            }

            public void Clear()
            {
                m_Classes.Clear();
                m_Interfaces.Clear();
            }

            public void Insert(Serializer<TReader, TWriter> serializer)
            {
                SerializerTreeNode node = new(serializer);
                Type serializerType = node.GetSerializerType();
                if (serializerType.IsInterface)
                    m_Interfaces.Insert(node);
                else
                    m_Classes.Insert(node);
            }

            public Serializer<TReader, TWriter>? Search(Type type)
            {
                List<Tuple<uint, Serializer<TReader, TWriter>>> classList = new();
                m_Classes.Search(type, 0, ref classList);
                if (classList.Count != 0)
                    return classList[0].Item2;
                List<Tuple<uint, Serializer<TReader, TWriter>>> interfaceList = new();
                m_Interfaces.Search(type, 0, ref interfaceList);
                uint depth = 0;
                Serializer<TReader, TWriter>? found = null;
                foreach (var elem in interfaceList)
                {
                    if (elem.Item1 >= depth)
                    {
                        depth = elem.Item1;
                        found = elem.Item2;
                    }
                }
                return found;
            }

            public Serializer<TReader, TWriter>? Search(string assemblyQualifiedName)
            {
                Serializer<TReader, TWriter>? classSerializer = m_Classes.Search(assemblyQualifiedName);
                if (classSerializer != null)
                    return classSerializer;
                Serializer<TReader, TWriter>? interfaceSerializer = m_Interfaces.Search(assemblyQualifiedName);
                if (interfaceSerializer != null)
                    return interfaceSerializer;
                return null;
            }
        }

        private static readonly SerializerTreeRoot ms_Serializers = new();


        public static void Clear() => ms_Serializers.Clear();
        public static void Register(Serializer<TReader, TWriter> serializer) => ms_Serializers.Insert(serializer);
        public static Serializer<TReader, TWriter>? GetSerializerFor(string assemblyQualifiedName) => ms_Serializers.Search(assemblyQualifiedName);
        public static Serializer<TReader, TWriter>? GetSerializerFor(Type type) => ms_Serializers.Search(type);

        internal abstract OperationResult<object?> DeserializeObj(TReader reader);
        internal abstract void SerializeObj(object obj, TWriter writer);
        internal abstract Type GetSerializedType();

        public override string ToString() => "Serializer[" + GetSerializedType().Name + "]";
    }

    public abstract class BytesSerializer : Serializer<BytesReader, BytesWriter>
    {
        public static T? Deserialize<T>(byte[] bytes)
        {
            BytesReader reader = new(bytes);
            return reader.Read<T>().Result;
        }

        public static byte[] Serialize(object obj)
        {
            BytesWriter writer = new();
            writer.Write(obj);
            return writer.Bytes;
        }

        public static new BytesSerializer? GetSerializerFor(string assemblyQualifiedName) => (BytesSerializer?)Serializer<BytesReader, BytesWriter>.GetSerializerFor(assemblyQualifiedName);
        public static new BytesSerializer? GetSerializerFor(Type type) => (BytesSerializer?)Serializer<BytesReader, BytesWriter>.GetSerializerFor(type);
        public override string ToString() => "BytesSerializer[" + GetSerializedType().Name + "]";
    }

    public abstract class BytesSerializer<T> : BytesSerializer
    {
        internal override OperationResult<object?> DeserializeObj(BytesReader reader) => Deserialize(reader).Cast<object?>();
        internal override void SerializeObj(object obj, BytesWriter writer) => Serialize((T)obj, writer);
        internal override Type GetSerializedType() => typeof(T);

        protected abstract OperationResult<T> Deserialize(BytesReader reader);
        protected abstract void Serialize(T obj, BytesWriter writer);
    }
}
