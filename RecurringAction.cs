namespace CorpseLib
{
    public class RecurringAction(TimeSpan refreshInterval)
    {
        public event AsyncEventHandler? OnStart;
        public event AsyncEventHandler? OnUpdate;
        public event AsyncEventHandler? OnStop;

        private readonly TimeSpan m_RefreshInterval = refreshInterval;
        private Task? m_RunningTask = null;
        private CancellationTokenSource m_CancellationToken = new();
        private volatile bool m_Running;

        public TimeSpan RefreshInterval => m_RefreshInterval;
        public bool Running => m_Running;

        public async Task Start()
        {
            if (!m_Running)
            {
                await OnActionStart();
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
                    await OnActionUpdate();
            }
        }

        public async Task Stop()
        {
            if (m_Running)
            {
                m_CancellationToken?.Cancel();
                m_Running = false;
                m_RunningTask = null;
                await OnActionStop();
            }
        }

        protected virtual async Task OnActionStart() => await Helper.CallAsyncEventHandler(OnStart);
        protected virtual async Task OnActionUpdate() => await Helper.CallAsyncEventHandler(OnUpdate);
        protected virtual async Task OnActionStop() => await Helper.CallAsyncEventHandler(OnStop);

        ~RecurringAction() => _ = Stop();
    }
}
