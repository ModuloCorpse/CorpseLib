namespace CorpseLib.Network
{
    internal class ReconnectionTask
    {
        private readonly ATCPClient m_Client;
        private TimeSpan m_Delay = TimeSpan.Zero;
        private uint m_MaxNbTry = 1;
        private volatile bool m_IsReconnecting = false;
        private volatile bool m_IsEnable = false;

        public bool IsReconnecting => m_IsReconnecting;
        public TimeSpan Delay => m_Delay;

        public ReconnectionTask(ATCPClient client) => m_Client = client;

        public void Enable() => m_IsEnable = true;

        public void Disable() => m_IsEnable = false;

        private void WaitForReconnection(Action action)
        {
            if (!m_IsReconnecting)
            {
                action();
                return;
            }

            if (m_Client.IsAsynchronous())
            {
                Task.Factory.StartNew(async () =>
                {
                    var periodicTimer = new PeriodicTimer(m_Delay);
                    while (await periodicTimer.WaitForNextTickAsync())
                    {
                        if (!m_IsReconnecting)
                        {
                            action();
                            return;
                        }
                    }
                });
            }
            else
            {
                while (m_IsReconnecting)
                    Thread.Sleep(m_Delay);
                action();
            }
        }

        public void SetDelay(TimeSpan delay) => WaitForReconnection(() => m_Delay = delay);
        public void SetMaxNbTry(uint maxNbTry) => WaitForReconnection(() => m_MaxNbTry = maxNbTry);
        public void SetDelayAndMaxNbTry(TimeSpan delay, uint maxNbTry) => WaitForReconnection(() =>
        {
            m_Delay = delay;
            m_MaxNbTry = maxNbTry;
        });

        public void Start()
        {
            if (m_IsEnable && !m_IsReconnecting)
            {
                m_IsReconnecting = true;
                if (m_Client.IsAsynchronous())
                {
                    Task.Factory.StartNew(async () =>
                    {
                        uint tryCount = 0;
                        var periodicTimer = new PeriodicTimer(m_Delay);
                        while (await periodicTimer.WaitForNextTickAsync())
                        {
                            ++tryCount;
                            if (m_Client.Connect() || tryCount >= m_MaxNbTry)
                                return;
                        }
                    });
                }
                else
                {
                    uint tryCount = 0;
                    while (tryCount < m_MaxNbTry)
                    {
                        if (tryCount != 0)
                            Thread.Sleep(m_Delay);
                        ++tryCount;
                        if (m_Client.Connect() || tryCount >= m_MaxNbTry)
                        {
                            m_IsReconnecting = false;
                            return;
                        }
                    }
                }
            }
        }
    }
}
