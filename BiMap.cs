using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CorpseLib
{
    public class BiMap<TKey, TValue> : IEnumerable<(TKey Key, TValue Value)>
    {
        private Entry[] m_Entries;
        private int[] m_KeyBuckets;
        private int[] m_ValueBuckets;
        private int m_Count = 0;
        private int m_FreeIndex = 0;

        [StructLayout(LayoutKind.Sequential)]
        private struct Entry
        {
            public TKey Key;
            public TValue Value;
            public int KeyHashCode;
            public int ValueHashCode;
            public int NextKey;
            public int NextValue;
            public bool IsDeleted;
        }

        public BiMap() : this(16) { }

        public BiMap(int capacity)
        {
            m_Entries = new Entry[capacity];
            m_KeyBuckets = new int[capacity];
            m_ValueBuckets = new int[capacity];
            Array.Fill(m_KeyBuckets, -1);
            Array.Fill(m_ValueBuckets, -1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetBucketIndex(int hashCode, int length) => (hashCode & 0x7FFFFFFF) % length;

        public void Add(TKey key, TValue value)
        {
            int keyHash = key?.GetHashCode() ?? 0;
            int valHash = value?.GetHashCode() ?? 0;
            if (FindIndexByKey(key, keyHash) >= 0)
                throw new ArgumentException("Key already exist.");
            if (FindIndexByValue(value, valHash) >= 0)
                throw new ArgumentException("Value already exist.");
            if (m_FreeIndex >= m_Entries.Length)
                Resize();
            int keyBucket = BiMap<TKey, TValue>.GetBucketIndex(keyHash, m_KeyBuckets.Length);
            int valueBucket = BiMap<TKey, TValue>.GetBucketIndex(valHash, m_ValueBuckets.Length);
            m_Entries[m_FreeIndex] = new Entry { Key = key, Value = value, KeyHashCode = keyHash, ValueHashCode = valHash, NextKey = m_KeyBuckets[keyBucket], NextValue = m_ValueBuckets[valueBucket], IsDeleted = false };
            m_KeyBuckets[keyBucket] = m_FreeIndex;
            m_ValueBuckets[valueBucket] = m_FreeIndex;
            m_FreeIndex++;
            m_Count++;
        }

        public bool TryGetValue(TKey key, out TValue? value)
        {
            int index = FindIndexByKey(key, key?.GetHashCode() ?? 0);
            if (index >= 0)
            {
                value = m_Entries[index].Value;
                return true;
            }
            value = default;
            return false;
        }

        public bool TryGetKey(TValue value, out TKey? key)
        {
            int index = FindIndexByValue(value, value?.GetHashCode() ?? 0);
            if (index >= 0)
            {
                key = m_Entries[index].Key;
                return true;
            }
            key = default;
            return false;
        }

        public TValue GetValue(TKey key)
        {
            if (TryGetValue(key, out var value))
                return value!;
            throw new KeyNotFoundException($"Cannot find key : {key}");
        }

        public TKey GetKey(TValue value)
        {
            if (TryGetKey(value, out var key))
                return key!;
            throw new KeyNotFoundException($"Cannot find value : {value}");
        }

        public bool RemoveByKey(TKey key)
        {
            int hash = key?.GetHashCode() ?? 0;
            int bucket = BiMap<TKey, TValue>.GetBucketIndex(hash, m_KeyBuckets.Length);
            int prev = -1;
            for (int i = m_KeyBuckets[bucket]; i >= 0; i = m_Entries[i].NextKey)
            {
                if (!m_Entries[i].IsDeleted && m_Entries[i].KeyHashCode == hash && Equals(m_Entries[i].Key, key))
                {
                    if (prev < 0)
                        m_KeyBuckets[bucket] = m_Entries[i].NextKey;
                    else
                        m_Entries[prev].NextKey = m_Entries[i].NextKey;
                    RemoveFromValueBucket(i);
                    m_Entries[i].IsDeleted = true;
                    m_Count--;
                    return true;
                }
                prev = i;
            }
            return false;
        }

        public bool RemoveByValue(TValue value)
        {
            int hash = value?.GetHashCode() ?? 0;
            int bucket = BiMap<TKey, TValue>.GetBucketIndex(hash, m_ValueBuckets.Length);
            int prev = -1;
            for (int i = m_ValueBuckets[bucket]; i >= 0; i = m_Entries[i].NextValue)
            {
                if (!m_Entries[i].IsDeleted && m_Entries[i].ValueHashCode == hash && Equals(m_Entries[i].Value, value))
                {
                    if (prev < 0)
                        m_ValueBuckets[bucket] = m_Entries[i].NextValue;
                    else
                        m_Entries[prev].NextValue = m_Entries[i].NextValue;
                    RemoveFromKeyBucket(i);
                    m_Entries[i].IsDeleted = true;
                    m_Count--;
                    return true;
                }
                prev = i;
            }
            return false;
        }

        private void RemoveFromKeyBucket(int index)
        {
            int bucket = BiMap<TKey, TValue>.GetBucketIndex(m_Entries[index].KeyHashCode, m_KeyBuckets.Length);
            int prev = -1;
            for (int i = m_KeyBuckets[bucket]; i >= 0; i = m_Entries[i].NextKey)
            {
                if (i == index)
                {
                    if (prev < 0)
                        m_KeyBuckets[bucket] = m_Entries[i].NextKey;
                    else
                        m_Entries[prev].NextKey = m_Entries[i].NextKey;
                    return;
                }
                prev = i;
            }
        }

        private void RemoveFromValueBucket(int index)
        {
            int bucket = BiMap<TKey, TValue>.GetBucketIndex(m_Entries[index].ValueHashCode, m_ValueBuckets.Length);
            int prev = -1;
            for (int i = m_ValueBuckets[bucket]; i >= 0; i = m_Entries[i].NextValue)
            {
                if (i == index)
                {
                    if (prev < 0)
                        m_ValueBuckets[bucket] = m_Entries[i].NextValue;
                    else
                        m_Entries[prev].NextValue = m_Entries[i].NextValue;
                    return;
                }
                prev = i;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FindIndexByKey(TKey key, int hash)
        {
            int bucket = BiMap<TKey, TValue>.GetBucketIndex(hash, m_KeyBuckets.Length);
            for (int i = m_KeyBuckets[bucket]; i >= 0; i = m_Entries[i].NextKey)
            {
                if (!m_Entries[i].IsDeleted && m_Entries[i].KeyHashCode == hash && Equals(m_Entries[i].Key, key))
                    return i;
            }
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FindIndexByValue(TValue value, int hash)
        {
            int bucket = BiMap<TKey, TValue>.GetBucketIndex(hash, m_ValueBuckets.Length);
            for (int i = m_ValueBuckets[bucket]; i >= 0; i = m_Entries[i].NextValue)
            {
                if (!m_Entries[i].IsDeleted && m_Entries[i].ValueHashCode == hash && Equals(m_Entries[i].Value, value))
                    return i;
            }
            return -1;
        }

        private void Resize()
        {
            int newSize = m_Entries.Length * 2;
            Array.Resize(ref m_Entries, newSize);
            var newKeyBuckets = new int[newSize];
            var newValueBuckets = new int[newSize];
            Array.Fill(newKeyBuckets, -1);
            Array.Fill(newValueBuckets, -1);
            for (int i = 0; i < m_FreeIndex; i++)
            {
                if (m_Entries[i].IsDeleted)
                    continue;
                int keyBucket = BiMap<TKey, TValue>.GetBucketIndex(m_Entries[i].KeyHashCode, newSize);
                int valueBucket = BiMap<TKey, TValue>.GetBucketIndex(m_Entries[i].ValueHashCode, newSize);
                m_Entries[i].NextKey = newKeyBuckets[keyBucket];
                m_Entries[i].NextValue = newValueBuckets[valueBucket];
                newKeyBuckets[keyBucket] = i;
                newValueBuckets[valueBucket] = i;
            }
            m_KeyBuckets = newKeyBuckets;
            m_ValueBuckets = newValueBuckets;
        }

        public int Count => m_Count;

        public IEnumerator<(TKey Key, TValue Value)> GetEnumerator()
        {
            for (int i = 0; i < m_FreeIndex; i++)
            {
                if (!m_Entries[i].IsDeleted)
                    yield return (m_Entries[i].Key, m_Entries[i].Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ConcurrentBiMap<TKey, TValue> : IEnumerable<(TKey Key, TValue Value)>
    {
        private readonly ReaderWriterLockSlim m_Lock = new();
        private readonly BiMap<TKey, TValue> m_Inner = [];

        public void Add(TKey key, TValue value)
        {
            m_Lock.EnterWriteLock();
            try { m_Inner.Add(key, value); }
            finally { m_Lock.ExitWriteLock(); }
        }

        public bool RemoveByKey(TKey key)
        {
            m_Lock.EnterWriteLock();
            try { return m_Inner.RemoveByKey(key); }
            finally { m_Lock.ExitWriteLock(); }
        }

        public bool RemoveByValue(TValue value)
        {
            m_Lock.EnterWriteLock();
            try { return m_Inner.RemoveByValue(value); }
            finally { m_Lock.ExitWriteLock(); }
        }

        public bool TryGetValue(TKey key, out TValue? value)
        {
            m_Lock.EnterReadLock();
            try { return m_Inner.TryGetValue(key, out value); }
            finally { m_Lock.ExitReadLock(); }
        }

        public bool TryGetKey(TValue value, out TKey? key)
        {
            m_Lock.EnterReadLock();
            try { return m_Inner.TryGetKey(value, out key); }
            finally { m_Lock.ExitReadLock(); }
        }

        public TValue GetValue(TKey key)
        {
            m_Lock.EnterReadLock();
            try { return m_Inner.GetValue(key); }
            finally { m_Lock.ExitReadLock(); }
        }

        public TKey GetKey(TValue value)
        {
            m_Lock.EnterReadLock();
            try { return m_Inner.GetKey(value); }
            finally { m_Lock.ExitReadLock(); }
        }

        public int Count
        {
            get
            {
                m_Lock.EnterReadLock();
                try { return m_Inner.Count; }
                finally { m_Lock.ExitReadLock(); }
            }
        }

        public IEnumerator<(TKey Key, TValue Value)> GetEnumerator()
        {
            m_Lock.EnterReadLock();
            try
            {
                foreach (var kv in m_Inner)
                    yield return kv;
            }
            finally
            {
                m_Lock.ExitReadLock();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
