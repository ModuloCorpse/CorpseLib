using CorpseLib.Serialize;

namespace CorpseLib.Network
{
    public abstract class AProtocol
    {
        private ATCPClient? m_Client = null;

        //====================PROTECTED====================\\
        protected List<object> ReadFromStream() => m_Client!.Read();
        protected void SetStream(Stream stream) => m_Client!.SetStream(stream);
        protected Stream GetStream() => m_Client!.GetStream();
        protected abstract void OnClientConnected();
        protected abstract void OnClientDisconnected();
        protected abstract OperationResult<object> Read(BytesReader reader);
        protected virtual void Write(BytesWriter writer, object obj) => writer.Write(obj);
        protected abstract void Treat(object packet);
        protected abstract void SetupSerializer(ref BytesSerializer serializer);

        //====================INTERNAL====================\\
        internal void SetClient(ATCPClient client) => m_Client = client;
        internal OperationResult<object> ReadFromStream(BytesReader reader) => Read(reader);
        internal void TreatPacket(object packet) => Treat(packet);
        internal void ClientConnected() => OnClientConnected();
        internal void ClientDisconnected() => OnClientDisconnected();
        internal void FillSerializer(ref BytesSerializer serializer) => SetupSerializer(ref serializer);

        //====================PUBLIC====================\\
        public bool IsSynchronous() => !m_Client!.IsAsynchronous();
        public bool IsAsynchronous() => m_Client!.IsAsynchronous();
        public URI GetURL() => m_Client!.GetURL();
        public int GetID() => m_Client!.GetID();
        public bool IsConnected() => m_Client!.IsConnected();
        public bool IsServerSide() => m_Client!.IsServerSide();
        public void Disconnect() => m_Client!.Disconnect();
        public void Connect() => m_Client!.Connect();
        public void Reconnect() => m_Client!.Reconnect();
        public void SetMonitor(IMonitor monitor) => m_Client!.SetMonitor(monitor);
        public void Send(object msg)
        {
            BytesWriter writer = new(m_Client!.BytesSerializer);
            Write(writer, msg);
            m_Client.WriteToStream(writer.Bytes);
        }
    }
}
