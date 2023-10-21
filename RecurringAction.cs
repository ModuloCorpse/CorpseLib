namespace CorpseLib
{
    public class RecurringAction
    {
        public event EventHandler? OnStart;
        public event EventHandler? OnUpdate;
        public event EventHandler? OnStop;

        private readonly int m_RefreshInterval;
        private volatile bool m_Running;

        public int RefreshInterval => m_RefreshInterval;
        public bool Running => m_Running;

        public RecurringAction(int refreshIntervalInMilliseconds)
        {
            m_RefreshInterval = refreshIntervalInMilliseconds;
        }

        public void Start()
        {
            OnActionStart();
            Task.Factory.StartNew(() => { NextLoop(); });
            m_Running = true;
        }

        private void NextLoop()
        {
            Thread.Sleep(m_RefreshInterval);
            if (m_Running)
            {
                OnActionUpdate();
                Task.Factory.StartNew(() => { NextLoop(); });
            }
        }

        public void Stop()
        {
            m_Running = false;
            OnActionStop();
        }

        protected virtual void OnActionStart() => OnStart?.Invoke(this, EventArgs.Empty);
        protected virtual void OnActionUpdate() => OnUpdate?.Invoke(this, EventArgs.Empty);
        protected virtual void OnActionStop() => OnStop?.Invoke(this, EventArgs.Empty);
    }
}
