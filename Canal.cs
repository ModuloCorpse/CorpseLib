namespace CorpseLib
{
    public class Canal(bool isAsync = false)
    {
        private readonly List<Action> m_TriggerListeners = [];
        private readonly object m_Lock = new();
        private readonly bool m_IsAsync = isAsync;

        public void Register(Action listener)
        {
            lock (m_Lock)
            {
                m_TriggerListeners.Add(listener);
            }
        }

        public void Unregister(Action listener)
        {
            lock (m_Lock)
            {
                m_TriggerListeners.Remove(listener);
            }
        }

        private void InternalTrigger()
        {
            lock (m_Lock)
            {
                foreach (Action listener in m_TriggerListeners)
                    listener();
            }
        }

        public void Trigger()
        {
            if (m_IsAsync)
                Task.Run(InternalTrigger);
            else
                InternalTrigger();
        }

        public void Clear()
        {
            lock (m_Lock)
            {
                m_TriggerListeners.Clear();
            }
        }
    }

    public class Canal<TEventType>(bool isAsync = false)
    {
        private readonly List<Action<TEventType?>> m_MessageListeners = [];
        private readonly object m_Lock = new();
        private readonly bool m_IsAsync = isAsync;

        public void Register(Action<TEventType?> listener)
        {
            lock (m_Lock)
            {
                m_MessageListeners.Add(listener);
            }
        }

        public void Unregister(Action<TEventType?> listener)
        {
            lock (m_Lock)
            {
                m_MessageListeners.Remove(listener);
            }
        }

        private void InternalEmit(TEventType? arg)
        {
            lock (m_Lock)
            {
                foreach (Action<TEventType?> listener in m_MessageListeners)
                    listener(arg);
            }
        }

        public void Emit(TEventType? arg)
        {
            if (m_IsAsync)
                Task.Run(() => InternalEmit(arg));
            else
                InternalEmit(arg);
        }

        public void Clear()
        {
            lock (m_Lock)
            {
                m_MessageListeners.Clear();
            }
        }
    }
}
