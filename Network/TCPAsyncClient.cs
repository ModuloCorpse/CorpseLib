using System.Net.Sockets;

namespace CorpseLib.Network
{
    public class TCPAsyncClient : ATCPClient
    {
        public TCPAsyncClient(AProtocol protocol, URI url, int id = 0) : base(protocol, url, id) { }
        internal TCPAsyncClient(AProtocol protocol, int id, Socket socket) : base(protocol, id, socket) { }

        internal void StartReceiving() => Task.Factory.StartNew(async () =>
        {
            byte[] readBuffer = new byte[1024];
            int bytesRead = await m_Stream.ReadAsync(readBuffer.AsMemory(0, readBuffer.Length));
            if (bytesRead > 0)
            {
                Received(readBuffer, bytesRead);
                StartReceiving();
            }
            else
                InternalDisconnect();
        });

        public override List<object> Read() => new();

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
