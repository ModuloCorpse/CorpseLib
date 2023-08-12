using System.Net.Sockets;

namespace CorpseLib.Network
{
    public class TCPAsyncServer : ATCPServer
    {
        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket acceptedSocket = m_ServerSocket.EndAccept(ar);
                TCPAsyncClient client = new(NewProtocol(), NewClientID(), acceptedSocket);
                AddClient(client);
                client.StartReceiving();
                m_ServerSocket.BeginAccept(AcceptCallback, m_ServerSocket);
            }
            catch
            {
                Stop();
            }
        }

        public TCPAsyncServer(ProtocolFactory protocolFactory, int port) : base(protocolFactory, port) { }

        public void Start()
        {
            Listen();
            if (IsRunning())
                m_ServerSocket.BeginAccept(AcceptCallback, null);
        }
    }
}
