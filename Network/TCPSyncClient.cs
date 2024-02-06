using CorpseLib.Serialize;
using System.Net.Sockets;

namespace CorpseLib.Network
{
    public class TCPSyncClient : ATCPClient
    {
        public TCPSyncClient(AProtocol protocol, URI url, int id = 0) : base(protocol, url, id) { }
        internal TCPSyncClient(AProtocol protocol, int id, Socket socket) : base(protocol, id, socket) { }

        private int m_ReadTimeout = -1;

        public void SetReadTimeout(int milliseconds) => m_ReadTimeout = milliseconds;

        protected override void HandleReconnect()
        {
            uint tryCount = 0;
            while (tryCount < MaxNbTry)
            {
                if (tryCount != 0)
                    Thread.Sleep(Delay);
                ++tryCount;
                if (InternalConnect(TimeSpan.FromMinutes(1), true) || tryCount >= MaxNbTry)
                {
                    SetIsReconnecting(false);
                    return;
                }
            }
        }

        public override void TestRead(BytesWriter bytesWriter) => TestReceived(bytesWriter);
        public override List<object> Read()
        {
            m_Stream.ReadTimeout = m_ReadTimeout;
            byte[] readBuffer = new byte[1024];
            try
            {
                int bytesRead = m_Stream.Read(readBuffer, 0, readBuffer.Length);
                if (bytesRead > 0)
                    return Received(readBuffer, bytesRead);
            }
            catch (Exception ex)
            {
                DiscardException(ex);
            }
            InternalDisconnect();
            return [];
        }

        protected override void HandleReceivedPacket(object packet) => m_Protocol.TreatPacket(packet);

        protected override void HandleActionAfterReconnect(Action action)
        {
            while (IsReconnecting())
                Thread.Sleep(Delay);
            action();
        }

        public override bool IsAsynchronous() => false;
    }
}
