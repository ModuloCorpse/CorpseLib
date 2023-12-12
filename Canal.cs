namespace CorpseLib
{
    public class Canal
    {
        private readonly List<Action> m_TriggerListeners = [];
        private readonly object m_Lock = new();

        public void Register(Action listener) { lock (m_Lock) { m_TriggerListeners.Add(listener); } }
        public void Unregister(Action listener) { lock (m_Lock) { m_TriggerListeners.Remove(listener); } }
        public void Trigger() => Task.Run(() => { lock (m_Lock) { foreach (Action listener in m_TriggerListeners) listener(); } });
        public void Clear() { lock (m_Lock) { m_TriggerListeners.Clear(); } }
    }

    public class Canal<TEventType>
    {
        private readonly List<Action<TEventType?>> m_MessageListeners = [];
        private readonly object m_Lock = new();

        public void Register(Action<TEventType?> listener) { lock (m_Lock) { m_MessageListeners.Add(listener); } }
        public void Unregister(Action<TEventType?> listener) { lock (m_Lock) { m_MessageListeners.Remove(listener); } }
        public void Emit(TEventType? arg) => Task.Run(() => { lock (m_Lock) { foreach (Action<TEventType?> listener in m_MessageListeners) listener(arg); } });

        public void Clear() { lock (m_Lock) { m_MessageListeners.Clear(); } }
    }
}
