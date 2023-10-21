using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace CorpseLib
{
    public class BiMap<TFirst, TSecond> : IEnumerable<KeyValuePair<TFirst, TSecond>>
        where TFirst : notnull
        where TSecond : notnull
    {
        private readonly Dictionary<TFirst, TSecond> m_FtoS = new();
        private readonly Dictionary<TSecond, TFirst> m_StoF = new();

        public TSecond GetF(TFirst key) => m_FtoS[key];
        public void SetS(TFirst key, TSecond value) => Add(key, value);
        public TFirst GetS(TSecond key) => m_StoF[key];
        public void SetF(TSecond key, TFirst value) => Add(value, key);


        public int Count => m_FtoS.Count;

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

        public bool ContainsF(TFirst key) => m_FtoS.ContainsKey(key);

        public bool ContainsS(TSecond key) => m_StoF.ContainsKey(key);

        public IEnumerator<KeyValuePair<TFirst, TSecond>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TFirst, TSecond>>)m_FtoS).GetEnumerator();
        }

        public bool RemoveF(TFirst key)
        {
            if (m_FtoS.ContainsKey(key))
            {
                m_StoF.Remove(m_FtoS[key]);
                return m_FtoS.Remove(key);
            }
            return false;
        }

        public bool RemoveS(TSecond key)
        {
            if (m_StoF.ContainsKey(key))
            {
                m_FtoS.Remove(m_StoF[key]);
                return m_StoF.Remove(key);
            }
            return false;
        }

        public bool TryGetF(TFirst key, [MaybeNullWhen(false)] out TSecond value) => m_FtoS.TryGetValue(key, out value);

        public bool TryGetS(TSecond key, [MaybeNullWhen(false)] out TFirst value) => m_StoF.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_FtoS).GetEnumerator();
    }
}
