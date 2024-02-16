using System.Collections.Concurrent;

namespace CorpseLib
{
    public class Canal
    {
        private readonly ConcurrentDictionary<Action, byte> m_TriggerListeners = [];
        public void Register(Action listener) { m_TriggerListeners.TryAdd(listener, 1); }
        public void Unregister(Action listener) { m_TriggerListeners.Remove(listener, out byte _); }
        public void Trigger() => Task.Run(() => { foreach (Action listener in m_TriggerListeners.Keys) listener(); });
        public void Clear() { m_TriggerListeners.Clear(); }
    }

    public class Canal<TEventType>
    {
        private readonly ConcurrentDictionary<Action<TEventType?>, byte> m_MessageListeners = [];
        public void Register(Action<TEventType?> listener) { m_MessageListeners.TryAdd(listener, 1); }
        public void Unregister(Action<TEventType?> listener) { m_MessageListeners.Remove(listener, out byte _); }
        public void Emit(TEventType? arg) => Task.Run(() => { foreach (Action<TEventType?> listener in m_MessageListeners.Keys) listener(arg); });
        public void Clear() { m_MessageListeners.Clear(); }
    }
}
