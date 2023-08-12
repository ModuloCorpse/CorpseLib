using System.Net;
using System.Net.Sockets;

namespace CorpseLib.Network
{
    /// <summary>
    /// Class representing an asynchronous TCP server
    /// </summary>
    public abstract class ATCPServer
    {
        public delegate AProtocol ProtocolFactory();

        private readonly ProtocolFactory m_ProtocolFactory;
        private readonly Dictionary<int, ATCPClient> m_Clients = new();
        private readonly List<int> m_FreeIdx = new();
        protected readonly Socket m_ServerSocket;
        private int m_CurrentIdx = 0;
        private readonly int m_Port;
        private volatile bool m_Running = false;

        public bool IsRunning() => m_Running;

        protected int NewClientID()
        {
            int clientID = m_CurrentIdx;
            if (m_FreeIdx.Count == 0)
                ++m_CurrentIdx;
            else
            {
                clientID = m_FreeIdx[0];
                m_FreeIdx.RemoveAt(0);
            }
            return clientID;
        }

        protected AProtocol NewProtocol() => m_ProtocolFactory();

        protected void AddClient(ATCPClient client)
        {
            client.OnDisconnect += delegate (ATCPClient client)
            {
                int clientID = client.GetID();
                m_Clients.Remove(clientID);
                m_FreeIdx.Add(clientID);
            };
            m_Clients[client.GetID()] = client;
        }

        /// <summary>
        /// Create a TCP server on the specified <paramref name="port"/>
        /// </summary>
        /// <param name="port">Port on which the server will listen</param>
        protected ATCPServer(ProtocolFactory protocolFactory, int port)
        {
            m_ProtocolFactory = protocolFactory;
            m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_Port = port;
        }

        public void Listen()
        {
            if (!m_Running)
            {
                m_Running = true;
                try
                {
                    m_ServerSocket.Bind(new IPEndPoint(IPAddress.Any, m_Port));
                    m_ServerSocket.Listen(10);
                }
                catch (Exception ex)
                {
                    Stop();
                    throw new Exception("Listening error" + ex);
                }
            }
        }

        public void Stop()
        {
            if (m_Running)
            {
                foreach (var pair in m_Clients)
                    pair.Value.Disconnect();
                m_Clients.Clear();
                m_ServerSocket.Close();
                m_Running = false;
            }
        }

        ~ATCPServer() => Stop();
    }
}
