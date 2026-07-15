using System.Diagnostics;

namespace CorpseLib
{
    public class TimedAction
    {
        public event EventHandler? OnStart;
        public event EventHandler<TimeSpan>? OnUpdate;
        public event EventHandler? OnStop;
        public event EventHandler? OnFinish;

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

        public void Start()
        {
            OnActionStart();
            m_StopWatch.Start();
            Task.Run(NextLoop);
            m_Running = true;
        }

        private void NextLoop()
        {
            Thread.Sleep(m_RefreshInterval);
            if (m_Running)
                Update();
        }

        private void Update()
        {
            TimeSpan ellapsed = TimeSpan.FromMilliseconds(m_StopWatch.ElapsedMilliseconds);
            OnActionUpdate(ellapsed);
            if (m_Duration >= TimeSpan.Zero && ellapsed >= m_Duration)
            {
                m_Running = false;
                m_StopWatch.Stop();
                OnActionFinish();
            }
            else
                Task.Run(NextLoop);
        }

        public void Stop()
        {
            m_Running = false;
            m_StopWatch.Stop();
            m_StopWatch.Reset();
            OnActionStop();
        }

        protected virtual void OnActionStart() => OnStart?.Invoke(this, EventArgs.Empty);
        protected virtual void OnActionUpdate(TimeSpan ellapsed) => OnUpdate?.Invoke(this, ellapsed);
        protected virtual void OnActionStop() => OnStop?.Invoke(this, EventArgs.Empty);
        protected virtual void OnActionFinish() => OnFinish?.Invoke(this, EventArgs.Empty);
    }
}
