namespace CorpseLib.Canal
{
    public class Canal<TCanalID> where TCanalID : notnull
    {
        private readonly Type? m_Type;
        private readonly List<Action<TCanalID, object?>> m_MessageListeners = new();
        private readonly object m_Lock = new();
        private readonly TCanalID m_Id;

        public Canal(TCanalID id, Type type)
        {
            m_Type = type;
            m_Id = id;
        }

        public bool IsValid<T>() => m_Type?.IsAssignableFrom(typeof(T)) ?? false;

        public void Add(Action<TCanalID, object?> listener) { lock (m_Lock) { m_MessageListeners.Add(listener); } }
        public void Remove(Action<TCanalID, object?> listener) { lock (m_Lock) { m_MessageListeners.Remove(listener); } }
        public void Iterate(object? arg) => Task.Run(() => { lock (m_Lock) { foreach (Action<TCanalID, object?> listener in m_MessageListeners) listener(m_Id, arg); } });

        public void Clear() { lock (m_Lock) { m_MessageListeners.Clear(); } }
    }
}
