namespace CorpseLib
{
    public class RecurringAction(int refreshIntervalInMilliseconds)
    {
        public event EventHandler? OnStart;
        public event EventHandler? OnUpdate;
        public event EventHandler? OnStop;

        private readonly int m_RefreshInterval = refreshIntervalInMilliseconds;
        private Task? m_RunningTask = null;
        private CancellationTokenSource m_CancellationToken = new();
        private volatile bool m_Running;

        public int RefreshInterval => m_RefreshInterval;
        public bool Running => m_Running;

        public void Start()
        {
            if (!m_Running)
            {
                OnActionStart();
                m_CancellationToken = new();
                m_RunningTask = Task.Run(LoopTask, m_CancellationToken.Token);
                m_Running = true;
            }
        }

        private async Task LoopTask()
        {
            while (m_Running)
            {
                await Task.Delay(m_RefreshInterval, m_CancellationToken.Token);
                if (!m_CancellationToken.IsCancellationRequested)
                    OnActionUpdate();
            }
        }

        public void Stop()
        {
            if (m_Running)
            {
                m_CancellationToken?.Cancel();
                m_Running = false;
                m_RunningTask = null;
                OnActionStop();
            }
        }

        protected virtual void OnActionStart() => OnStart?.Invoke(this, EventArgs.Empty);
        protected virtual void OnActionUpdate() => OnUpdate?.Invoke(this, EventArgs.Empty);
        protected virtual void OnActionStop() => OnStop?.Invoke(this, EventArgs.Empty);

        ~RecurringAction() => Stop();
    }
}
