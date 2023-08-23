namespace CorpseLib.Serialize
{
    public class Serializer<TSerializer> where TSerializer : ASerializer
    {
        private class SerializerTreeNode
        {
            private readonly TSerializer? m_Serializer;
            private readonly Dictionary<Type, SerializerTreeNode> m_Children = new();

            public SerializerTreeNode(TSerializer serializer) => m_Serializer = serializer;
            public SerializerTreeNode() => m_Serializer = default;

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

            public void Search(Type type, uint depth, ref List<Tuple<uint, TSerializer>> list)
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

            public TSerializer? Search(string assemblyQualifiedName)
            {
                foreach (var child in m_Children)
                {
                    if (child.Key.AssemblyQualifiedName == assemblyQualifiedName)
                        return child.Value.m_Serializer;
                }
                foreach (var child in m_Children)
                {
                    TSerializer? serializer = child.Value.Search(assemblyQualifiedName);
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

            public void Insert(TSerializer serializer)
            {
                SerializerTreeNode node = new(serializer);
                Type serializerType = node.GetSerializerType();
                if (serializerType.IsInterface)
                    m_Interfaces.Insert(node);
                else
                    m_Classes.Insert(node);
            }

            public TSerializer? Search(Type type)
            {
                List<Tuple<uint, TSerializer>> classList = new();
                m_Classes.Search(type, 0, ref classList);
                if (classList.Count != 0)
                    return classList[0].Item2;
                List<Tuple<uint, TSerializer>> interfaceList = new();
                m_Interfaces.Search(type, 0, ref interfaceList);
                uint depth = 0;
                TSerializer? found = null;
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

            public TSerializer? Search(string assemblyQualifiedName)
            {
                TSerializer? classSerializer = m_Classes.Search(assemblyQualifiedName);
                if (classSerializer != null)
                    return classSerializer;
                TSerializer? interfaceSerializer = m_Interfaces.Search(assemblyQualifiedName);
                if (interfaceSerializer != null)
                    return interfaceSerializer;
                return null;
            }
        }

        private readonly SerializerTreeRoot m_Serializers = new();

        public void Clear() => m_Serializers.Clear();
        public void Register(TSerializer serializer) => m_Serializers.Insert(serializer);
        public TSerializer? GetSerializerFor(string assemblyQualifiedName) => m_Serializers.Search(assemblyQualifiedName);
        public TSerializer? GetSerializerFor(Type type) => m_Serializers.Search(type);
    }
}
