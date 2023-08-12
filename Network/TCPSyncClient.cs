using System.Net.Sockets;

namespace CorpseLib.Network
{
    public class TCPSyncClient : ATCPClient
    {
        public TCPSyncClient(AProtocol protocol, URI url, int id = 0) : base(protocol, url, id) { }
        internal TCPSyncClient(AProtocol protocol, int id, Socket socket) : base(protocol, id, socket) { }

        private int m_ReadTimeout = -1;

        public void SetReadTimeout(int milliseconds) => m_ReadTimeout = milliseconds;

        public override List<object> Read()
        {
            m_Stream.ReadTimeout = m_ReadTimeout;
            byte[] readBuffer = new byte[1024];
            try
            {
                int bytesRead = m_Stream.Read(readBuffer, 0, readBuffer.Length);
                if (bytesRead > 0)
                    return Received(readBuffer, bytesRead);
            } catch { }
            InternalDisconnect();
            return new();
        }

        public override bool IsAsynchronous() => false;
    }
}
