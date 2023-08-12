namespace CorpseLib.Network
{
    public class TCPSyncServer : ATCPServer
    {
        public TCPSyncServer(ProtocolFactory protocolFactory, int port) : base(protocolFactory, port) { }

        public TCPSyncClient Accept()
        {
            TCPSyncClient client = new(NewProtocol(), NewClientID(), m_ServerSocket.Accept());
            AddClient(client);
            return client;
        }
    }
}
