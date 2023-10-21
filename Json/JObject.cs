using System.Collections;

namespace CorpseLib.Json
{
    public class JObject : JNode, IEnumerable<KeyValuePair<string, JNode>>
    {
        public static JObject Parse(string content)
        {
            JObject obj = new();
            if (!string.IsNullOrEmpty(content))
            {
                JReader reader = new(content);
                reader.Read(obj);
            }
            return obj;
        }

        private readonly Dictionary<string, JNode> m_Children = new();

        public JObject() { }
        public JObject(JObject obj) => m_Children = obj.m_Children;

        public object? this[string key]
        {
            get => Get<object>(key);
            set => Set(key, value);
        }

        public bool ContainsKey(string key) => m_Children.ContainsKey(key);

        public object? Get(string key, Type type)
        {
            if (TryGet(key, type, out object? ret))
                return ret;
            throw new JException(string.Format("No node {0} in the JSON", key));
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

        public bool TryGet(string key, Type type, out object? ret)
        {
            JNode? token = m_Children.GetValueOrDefault(key);
            if (token != null)
                return JHelper.Cast(token, out ret, type);
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

        public List<object> GetList(string key, Type type)
        {
            List<object> ret = new();
            JNode? token = m_Children.GetValueOrDefault(key);
            if (token != null && token is JArray arr)
            {
                foreach (JNode item in arr)
                {
                    if (JHelper.Cast(item, out object? cast, type))
                        ret.Add(cast!);
                }
            }
            return ret;
        }

        public List<T> GetList<T>(string key) => GetList(key, typeof(T)).Cast<T>().ToList();

        private static JArray ToArray(IEnumerable arr)
        {
            JArray ret = new();
            foreach (object item in arr)
                ret.Add(JHelper.Cast(item));
            return ret;
        }


        public void Set(string key, object? obj)
        {
            if (obj is IList arr)
                m_Children[key] = ToArray(arr);
            else
                m_Children[key] = JHelper.Cast(obj);
        }

        public void Set(string key, object?[] obj) => m_Children[key] = ToArray(obj);

        public void Add(string name, JNode? child) => m_Children[name] = child ?? new JNull();
        public void Add(string key, object? obj) => Set(key, obj);

        public JNode? Get(string name) => m_Children.GetValueOrDefault(name);

        public override void ToJson(ref JBuilder builder)
        {
            builder.OpenObject();
            int i = 0;
            foreach (var child in m_Children)
            {
                if (i++ > 0)
                    builder.AppendSeparator();
                builder.AppendName(child.Key);
                child.Value.ToJson(ref builder);
            }
            builder.CloseObject();
        }

        public IEnumerator<KeyValuePair<string, JNode>> GetEnumerator() => ((IEnumerable<KeyValuePair<string, JNode>>)m_Children).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_Children).GetEnumerator();
    }
}
