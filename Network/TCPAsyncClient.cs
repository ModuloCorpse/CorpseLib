using System.Net.Sockets;

namespace CorpseLib.Network
{
    public class TCPAsyncClient : ATCPClient
    {
        private volatile bool m_IsReceiving = false;

        public TCPAsyncClient(AProtocol protocol, URI url, int id = 0) : base(protocol, url, id) { }
        internal TCPAsyncClient(AProtocol protocol, int id, Socket socket) : base(protocol, id, socket) { }

        internal void StartReceiving()
        {
            m_IsReceiving = true;
            Task.Factory.StartNew(async () =>
            {
                byte[] readBuffer = new byte[1024];
                try
                {
                    int bytesRead = await m_Stream.ReadAsync(readBuffer.AsMemory(0, readBuffer.Length));
                    if (bytesRead > 0)
                    {
                        Received(readBuffer, bytesRead);
                        StartReceiving();
                    }
                    else
                        InternalDisconnect();
                }
                catch (Exception ex)
                {
                    DiscardException(ex);
                    InternalDisconnect();
                }
            });
        }

        protected override void HandleReconnect() => Task.Factory.StartNew(async () =>
        {
            uint tryCount = 0;
            var periodicTimer = new PeriodicTimer(Delay);
            while (await periodicTimer.WaitForNextTickAsync())
            {
                ++tryCount;
                if (InternalConnect(TimeSpan.FromMinutes(1), true))
                {
                    SetIsReconnecting(false);
                    if (m_IsReceiving)
                        StartReceiving();
                    return;
                }
                else if (tryCount >= MaxNbTry)
                {
                    SetIsReconnecting(false);
                    return;
                }
            }
        });

        public override List<object> Read() => new();

        protected override void HandleActionAfterReconnect(Action action)
        {
            Task.Factory.StartNew(async () =>
            {
                var periodicTimer = new PeriodicTimer(Delay);
                while (await periodicTimer.WaitForNextTickAsync())
                {
                    if (!IsReconnecting())
                    {
                        action();
                        return;
                    }
                }
            });
        }

        public bool Start()
        {
            if (Connect())
            {
                StartReceiving();
                return true;
            }
            return false;
        }

        public override bool IsAsynchronous() => true;
    }
}
