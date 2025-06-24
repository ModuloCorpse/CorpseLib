using System.Collections;

namespace CorpseLib.DataNotation
{
    public class DataObject : DataNode, IEnumerable<KeyValuePair<string, DataNode>>
    {
        private readonly Dictionary<string, DataNode> m_Children = [];

        public DataObject() { }
        public DataObject(DataObject obj) => m_Children = obj.m_Children;

        public object? this[string key]
        {
            get => Get<object>(key);
            set => Add(key, value);
        }

        public bool ContainsKey(string key) => m_Children.ContainsKey(key);

        public bool TryGet(string key, Type type, out object? ret)
        {
            DataNode? token = m_Children.GetValueOrDefault(key);
            if (token != null)
                return DataHelper.Cast(token, out ret, type);
            ret = default;
            return false;
        }

        public bool TryGet<T>(string key, out T? ret)
        {
            if (TryGet(key, typeof(T), out object? val))
            {
                ret = (T?)val;
                return true;
            }
            ret = default;
            return false;
        }

        public object? Get(string key, Type type)
        {
            if (TryGet(key, type, out object? ret))
                return ret;
            throw new DataException($"No node {key} in the JSON");
        }

        public T? Get<T>(string key) => (T?)Get(key, typeof(T));

        public object? GetOrDefault(string key, Type type, object? defaultReturn)
        {
            if (TryGet(key, type, out object? ret))
                return ret;
            return defaultReturn;
        }

        public T? GetOrDefault<T>(string key) => (T?)GetOrDefault(key, typeof(T), default(T));
        public T GetOrDefault<T>(string key, T defaultReturn) => (T)GetOrDefault(key, typeof(T), defaultReturn)!;

        public Dictionary<string, T> GetDictionary<T>(string key)
        {
            if (TryGet(key, typeof(Dictionary<string, T>), out object? ret))
                return (Dictionary<string, T>)ret!;
            return [];
        }

        public List<T> GetList<T>(string key)
        {
            if (TryGet(key, typeof(List<T>), out object? ret))
                return (List<T>)ret!;
            return [];
        }

        public T[] GetArray<T>(string key)
        {
            if (TryGet(key, typeof(T[]), out object? ret))
                return (T[])ret!;
            return [];
        }

        public void Add(string key, object? obj) => m_Children[key] = DataHelper.Cast(obj);

        public DataNode? Get(string name) => m_Children.GetValueOrDefault(name);

        public IEnumerator<KeyValuePair<string, DataNode>> GetEnumerator() => ((IEnumerable<KeyValuePair<string, DataNode>>)m_Children).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_Children).GetEnumerator();
    }
}
