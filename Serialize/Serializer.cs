namespace CorpseLib.Serialize
{
    public class Serializer<TReader, TWriter>
    {
        private class SerializerTreeNode
        {
            private readonly ASerializer<TReader, TWriter>? m_Serializer;
            private readonly Dictionary<Type, SerializerTreeNode> m_Children = [];

            public SerializerTreeNode(ASerializer<TReader, TWriter> serializer) => m_Serializer = serializer;
            public SerializerTreeNode() => m_Serializer = default;

            public void Clear() => m_Children.Clear();

            public Type GetSerializerType() => m_Serializer!.GetSerializedType();

            public void AppendChild(KeyValuePair<Type, SerializerTreeNode> child) => m_Children[child.Key] = child.Value;

            public bool Insert(SerializerTreeNode node)
            {
                List<Type> typeToRemove = [];
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

            public void Search(Type type, uint depth, ref List<Tuple<uint, ASerializer<TReader, TWriter>>> list)
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

            public ASerializer<TReader, TWriter>? Search(string assemblyQualifiedName)
            {
                foreach (var child in m_Children)
                {
                    if (child.Key.AssemblyQualifiedName == assemblyQualifiedName)
                        return child.Value.m_Serializer;
                }
                foreach (var child in m_Children)
                {
                    ASerializer<TReader, TWriter>? serializer = child.Value.Search(assemblyQualifiedName);
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

            public void Clear()
            {
                m_Classes.Clear();
                m_Interfaces.Clear();
            }

            public void Insert(ASerializer<TReader, TWriter> serializer)
            {
                SerializerTreeNode node = new(serializer);
                Type serializerType = node.GetSerializerType();
                if (serializerType.IsInterface)
                    m_Interfaces.Insert(node);
                else
                    m_Classes.Insert(node);
            }

            public ASerializer<TReader, TWriter>? Search(Type type)
            {
                List<Tuple<uint, ASerializer<TReader, TWriter>>> classList = [];
                m_Classes.Search(type, 0, ref classList);
                if (classList.Count != 0)
                    return classList[0].Item2;
                List<Tuple<uint, ASerializer<TReader, TWriter>>> interfaceList = [];
                m_Interfaces.Search(type, 0, ref interfaceList);
                uint depth = 0;
                ASerializer<TReader, TWriter>? found = null;
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

            public ASerializer<TReader, TWriter>? Search(string assemblyQualifiedName)
            {
                ASerializer<TReader, TWriter>? classSerializer = m_Classes.Search(assemblyQualifiedName);
                if (classSerializer != null)
                    return classSerializer;
                ASerializer<TReader, TWriter>? interfaceSerializer = m_Interfaces.Search(assemblyQualifiedName);
                if (interfaceSerializer != null)
                    return interfaceSerializer;
                return null;
            }
        }

        private static readonly SerializerTreeRoot ms_Serializers = new();
        private readonly SerializerTreeRoot m_Serializers = new();

        public static void RegisterDefault(ASerializer<TReader, TWriter> serializer) => ms_Serializers.Insert(serializer);

        public void Clear() => m_Serializers.Clear();
        public void Register(ASerializer<TReader, TWriter> serializer) => m_Serializers.Insert(serializer);

        public ASerializer<TReader, TWriter>? GetSerializerFor(Type type)
        {
            ASerializer<TReader, TWriter>? ret = ms_Serializers.Search(type);
            if (ret != null)
                return ret;
            return m_Serializers.Search(type);
        }

        public OperationResult<T> Deserialize<T>(TReader reader) => Deserialize(reader, typeof(T)).Cast<T>();
        public OperationResult<object?> Deserialize(TReader reader, Type type)
        {
            ASerializer<TReader, TWriter>? serializer = GetSerializerFor(type);
            if (serializer != null)
                return serializer.DeserializeObj(reader);
            return new("Serialization error", string.Format("No serializer found for type {0}", type.Name));
        }

        public bool Serialize(object obj, TWriter writer)
        {
            ASerializer<TReader, TWriter>? serializer = GetSerializerFor(obj.GetType());
            if (serializer != null)
            {
                serializer.SerializeObj(obj, writer);
                return true;
            }
            return false;
        }
    }
    
    public abstract class ASerializer
    {
        internal abstract string GetSerializerName();
        internal abstract Type GetSerializedType();
        public override string ToString() => string.Format("{0}[{1}]", GetSerializerName(), GetSerializedType().Name);
    }

    public abstract class ASerializer<TReader, TWriter> : ASerializer
    {
        internal abstract OperationResult<object?> DeserializeObj(TReader reader);
        internal abstract void SerializeObj(object obj, TWriter writer);
    }
}
