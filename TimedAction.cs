using System.Diagnostics;

namespace CorpseLib
{
    public class TimedAction
    {
        public event AsyncEventHandler? OnStart;
        public event AsyncEventHandler<TimeSpan>? OnUpdate;
        public event AsyncEventHandler? OnStop;
        public event AsyncEventHandler? OnFinish;

        private readonly Stopwatch m_StopWatch = new();
        private TimeSpan m_Duration;
        private TimeSpan m_RefreshInterval;
        private volatile bool m_Running;

        public TimeSpan Duration => m_Duration;
        public TimeSpan RefreshInterval => m_RefreshInterval;
        public bool Running => m_Running;

        public TimedAction(TimeSpan refreshInterval)
        {
            m_RefreshInterval = refreshInterval;
            m_Duration = TimeSpan.MinValue;
        }

        public TimedAction(TimeSpan refreshInterval, TimeSpan duration)
        {
            m_RefreshInterval = refreshInterval;
            m_Duration = duration;
        }

        public void SetDuration(TimeSpan duration) => m_Duration = duration;
        public void SetRefreshInterval(TimeSpan refreshInterval) => m_RefreshInterval = refreshInterval;

        public async Task Start()
        {
            await OnActionStart();
            m_StopWatch.Start();
            _ = Task.Run(NextLoop);
            m_Running = true;
        }

        private async Task NextLoop()
        {
            Thread.Sleep(m_RefreshInterval);
            if (m_Running)
                await Update();
        }

        private async Task Update()
        {
            TimeSpan ellapsed = TimeSpan.FromMilliseconds(m_StopWatch.ElapsedMilliseconds);
            await OnActionUpdate(ellapsed);
            if (m_Duration >= TimeSpan.Zero && ellapsed >= m_Duration)
            {
                m_Running = false;
                m_StopWatch.Stop();
                await OnActionFinish();
            }
            else
                _ = Task.Run(NextLoop);
        }

        public async Task Stop()
        {
            m_Running = false;
            m_StopWatch.Stop();
            m_StopWatch.Reset();
            await OnActionStop();
        }

        protected virtual async Task OnActionStart() => await Helper.CallAsyncEventHandler(OnStart);
        protected virtual async Task OnActionUpdate(TimeSpan ellapsed) => await Helper.CallAsyncEventHandler(OnUpdate, ellapsed);
        protected virtual async Task OnActionStop() => await Helper.CallAsyncEventHandler(OnStop);
        protected virtual async Task OnActionFinish() => await Helper.CallAsyncEventHandler(OnFinish);
    }
}
