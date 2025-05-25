using CorpseLib.Serialize;
using System;
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
            Task.Run(async () =>
            {
                byte[] readBuffer = new byte[1024];
                byte[] buffer = [];
                try
                {
                    int bytesRead = await m_Stream.ReadAsync(readBuffer.AsMemory(0, readBuffer.Length));
                    Append(ref buffer, readBuffer, bytesRead);
                    if (bytesRead == readBuffer.Length)
                        ReadAllStream(ref buffer);
                    if (buffer.Length > 0)
                        Received(buffer);
                    StartReceiving();
                }
                catch (Exception ex)
                {
                    DiscardException(ex);
                    InternalDisconnect();
                }
            });
        }

        protected override void HandleReceivedPacket(object packet) => Task.Run(() => { m_Protocol.TreatPacket(packet); });

        protected override void HandleReconnect() => Task.Run(async () =>
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

        public override void TestRead(BytesWriter bytesWriter) => Task.Run(() => TestReceived(bytesWriter));

        public override List<object> Read() => [];

        protected override void HandleActionAfterReconnect(Action action)
        {
            Task.Run(async () =>
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
