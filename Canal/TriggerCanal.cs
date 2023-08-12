namespace CorpseLib
{
    public class TriggerCanal<TCanalID> where TCanalID : notnull
    {
        private readonly List<Action<TCanalID>> m_TriggerListeners = new();
        private readonly object m_Lock = new();
        private readonly TCanalID m_Id;

        public TriggerCanal(TCanalID id) => m_Id = id;

        public void Add(Action<TCanalID> listener) { lock (m_Lock) { m_TriggerListeners.Add(listener); } }
        public void Remove(Action<TCanalID> listener) { lock (m_Lock) { m_TriggerListeners.Remove(listener); } }
        public void Iterate() => Task.Run(() => { lock (m_Lock) { foreach (Action<TCanalID> listener in m_TriggerListeners) listener(m_Id); } });
        public void Clear() { lock (m_Lock) { m_TriggerListeners.Clear(); } }
    }
}
