namespace CorpseLib
{
    public class Canal(bool isAsync = false)
    {
        private readonly List<Action> m_MessageListeners = [];
        private readonly List<Func<Task>> m_AsyncMessageListeners = [];
        private readonly Lock m_MessageListenersLock = new();
        private readonly Lock m_AsyncMessageListenersLock = new();
        private readonly bool m_IsAsync = isAsync;

        public void Register(Action listener)
        {
            m_MessageListenersLock.Enter();
            m_MessageListeners.Add(listener);
            m_MessageListenersLock.Exit();
        }

        public void Register(Func<Task> listener)
        {
            m_AsyncMessageListenersLock.Enter();
            m_AsyncMessageListeners.Add(listener);
            m_AsyncMessageListenersLock.Exit();
        }

        public void Unregister(Action listener)
        {
            m_MessageListenersLock.Enter();
            m_MessageListeners.Remove(listener);
            m_MessageListenersLock.Exit();
        }

        public void Unregister(Func<Task> listener)
        {
            m_AsyncMessageListenersLock.Enter();
            m_AsyncMessageListeners.Remove(listener);
            m_AsyncMessageListenersLock.Exit();
        }

        private async Task InternalTrigger()
        {
            m_MessageListenersLock.Enter();
            foreach (Action listener in m_MessageListeners)
                listener();
            m_MessageListenersLock.Exit();

            m_AsyncMessageListenersLock.Enter();
            foreach (Func<Task> listener in m_AsyncMessageListeners)
            {
                if (m_IsAsync)
                    _ = listener();
                else
                    await listener();
            }
            m_AsyncMessageListenersLock.Exit();
        }

        public async Task Trigger()
        {
            if (m_IsAsync)
                _ = InternalTrigger();
            else
                await InternalTrigger();
        }

        public void Clear()
        {
            m_MessageListenersLock.Enter();
            m_MessageListeners.Clear();
            m_MessageListenersLock.Exit();

            m_AsyncMessageListenersLock.Enter();
            m_AsyncMessageListeners.Clear();
            m_AsyncMessageListenersLock.Exit();
        }
    }

    public class Canal<TEventType>(bool isAsync = false)
    {
        private readonly List<Action<TEventType?>> m_MessageListeners = [];
        private readonly List<Func<TEventType?, Task>> m_AsyncMessageListeners = [];
        private readonly Lock m_MessageListenersLock = new();
        private readonly Lock m_AsyncMessageListenersLock = new();
        private readonly bool m_IsAsync = isAsync;

        public void Register(Action<TEventType?> listener)
        {
            m_MessageListenersLock.Enter();
            m_MessageListeners.Add(listener);
            m_MessageListenersLock.Exit();
        }

        public void Register(Func<TEventType?, Task> listener)
        {
            m_AsyncMessageListenersLock.Enter();
            m_AsyncMessageListeners.Add(listener);
            m_AsyncMessageListenersLock.Exit();
        }

        public void Unregister(Action<TEventType?> listener)
        {
            m_MessageListenersLock.Enter();
            m_MessageListeners.Remove(listener);
            m_MessageListenersLock.Exit();
        }

        public void Unregister(Func<TEventType?, Task> listener)
        {
            m_AsyncMessageListenersLock.Enter();
            m_AsyncMessageListeners.Remove(listener);
            m_AsyncMessageListenersLock.Exit();
        }

        private async Task InternalEmit(TEventType? arg)
        {
            m_MessageListenersLock.Enter();
            foreach (Action<TEventType?> listener in m_MessageListeners)
                listener(arg);
            m_MessageListenersLock.Exit();

            m_AsyncMessageListenersLock.Enter();
            foreach (Func<TEventType?, Task> listener in m_AsyncMessageListeners)
            {
                if (m_IsAsync)
                    _ = listener(arg);
                else
                    await listener(arg);
            }
            m_AsyncMessageListenersLock.Exit();
        }

        public async Task Emit(TEventType? arg)
        {
            if (m_IsAsync)
                _ = InternalEmit(arg);
            else
                await InternalEmit(arg);
        }

        public void Clear()
        {
            m_MessageListenersLock.Enter();
            m_MessageListeners.Clear();
            m_MessageListenersLock.Exit();

            m_AsyncMessageListenersLock.Enter();
            m_AsyncMessageListeners.Clear();
            m_AsyncMessageListenersLock.Exit();
        }
    }
}
