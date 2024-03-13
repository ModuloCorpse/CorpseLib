using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace CorpseLib
{
    public abstract class ABiMap<TFirst, TSecond> : IEnumerable<KeyValuePair<TFirst, TSecond>> where TFirst : notnull where TSecond : notnull
    {
        private readonly Dictionary<TFirst, TSecond> m_FtoS = [];
        private readonly Dictionary<TSecond, TFirst> m_StoF = [];

        public int Count => m_FtoS.Count;
        public Dictionary<TFirst, TSecond>.KeyCollection Keys => m_FtoS.Keys;
        public Dictionary<TFirst, TSecond>.ValueCollection Values => m_FtoS.Values;

        protected TSecond GetF(TFirst key) => m_FtoS[key];
        protected TFirst GetS(TSecond key) => m_StoF[key];

        protected bool ContainsF(TFirst key) => m_FtoS.ContainsKey(key);
        protected bool ContainsS(TSecond key) => m_StoF.ContainsKey(key);

        protected bool RemoveF(TFirst key)
        {
            if (m_FtoS.TryGetValue(key, out TSecond? stofToRemove))
            {
                m_StoF.Remove(stofToRemove);
                return m_FtoS.Remove(key);
            }
            return false;
        }

        protected bool RemoveS(TSecond key)
        {
            if (m_StoF.TryGetValue(key, out TFirst? ftosToRemove))
            {
                m_FtoS.Remove(ftosToRemove);
                return m_StoF.Remove(key);
            }
            return false;
        }

        protected bool TryGetF(TFirst key, [MaybeNullWhen(false)] out TSecond value) => m_FtoS.TryGetValue(key, out value);
        protected bool TryGetS(TSecond key, [MaybeNullWhen(false)] out TFirst value) => m_StoF.TryGetValue(key, out value);

        public void Add(TFirst key, TSecond value)
        {
            m_FtoS[key] = value;
            m_StoF[value] = key;
        }

        public void Clear()
        {
            m_FtoS.Clear();
            m_StoF.Clear();
        }

        public IEnumerator<KeyValuePair<TFirst, TSecond>> GetEnumerator() => ((IEnumerable<KeyValuePair<TFirst, TSecond>>)m_FtoS).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_FtoS).GetEnumerator();
    }

    public class BiMap<TFirst, TSecond> : ABiMap<TFirst, TSecond> where TFirst : notnull where TSecond : notnull
    {
        public TSecond this[TFirst key]
        {
            get => GetF(key);
            set => Add(key, value);
        }
        public TFirst this[TSecond key]
        {
            get => GetS(key);
            set => Add(value, key);
        }

        public bool ContainsKey(TFirst key) => ContainsF(key);
        public bool ContainsKey(TSecond key) => ContainsS(key);
        public bool Remove(TFirst key) => RemoveF(key);
        public bool Remove(TSecond key) => RemoveS(key);
        public bool TryGetValue(TFirst key, [MaybeNullWhen(false)] out TSecond value) => TryGetF(key, out value);
        public bool TryGetValue(TSecond key, [MaybeNullWhen(false)] out TFirst value) => TryGetS(key, out value);
    }

    public class BiMap<T> : ABiMap<T, T> where T : notnull
    {
        public T GetFirst(T key) => GetF(key);
        public void SetFirst(T key, T value) => Add(key, value);
        public T GetSecond(T key) => GetS(key);
        public void SetSecond(T key, T value) => Add(value, key);
        public bool ContainsKeyFirst(T key) => ContainsF(key);
        public bool ContainsKeySecond(T key) => ContainsS(key);
        public bool RemoveFirst(T key) => RemoveF(key);
        public bool RemoveSecond(T key) => RemoveS(key);
        public bool TryGetValueFirst(T key, [MaybeNullWhen(false)] out T value) => TryGetF(key, out value);
        public bool TryGetValueSecond(T key, [MaybeNullWhen(false)] out T value) => TryGetS(key, out value);
    }
}
