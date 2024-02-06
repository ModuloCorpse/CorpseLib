using System.Diagnostics;

namespace CorpseLib
{
    public class TimedAction
    {
        public event EventHandler? OnStart;
        public event EventHandler<long>? OnUpdate;
        public event EventHandler? OnStop;
        public event EventHandler? OnFinish;

        private readonly Stopwatch m_StopWatch = new();
        private readonly long m_Duration;
        private readonly int m_RefreshInterval;
        private volatile bool m_Running;

        public long Duration => m_Duration;
        public int RefreshInterval => m_RefreshInterval;
        public bool Running => m_Running;

        public TimedAction(int refreshIntervalInMilliseconds)
        {
            m_RefreshInterval = refreshIntervalInMilliseconds;
            m_Duration = -1;
        }

        public TimedAction(int refreshIntervalInMilliseconds, long durationInMilliseconds)
        {
            m_RefreshInterval = refreshIntervalInMilliseconds;
            m_Duration = durationInMilliseconds;
        }

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
            long ellapsed = m_StopWatch.ElapsedMilliseconds;
            OnActionUpdate(ellapsed);
            if (m_Duration >= 0 && ellapsed >= m_Duration)
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
            OnActionStop();
        }

        protected virtual void OnActionStart() => OnStart?.Invoke(this, EventArgs.Empty);
        protected virtual void OnActionUpdate(long ellapsed) => OnUpdate?.Invoke(this, ellapsed);
        protected virtual void OnActionStop() => OnStop?.Invoke(this, EventArgs.Empty);
        protected virtual void OnActionFinish() => OnFinish?.Invoke(this, EventArgs.Empty);
    }
}
