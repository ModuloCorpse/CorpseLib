using System.Collections.Concurrent;
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
        private readonly ConcurrentDictionary<int, ATCPClient> m_Clients = new();
        private readonly List<int> m_FreeIdx = [];
        protected readonly Socket m_ServerSocket;
        private IMonitor? m_Monitor = null;
        private int m_CurrentIdx = 0;
        private readonly int m_Port;
        private volatile bool m_Running = false;

        public void SetMonitor(IMonitor monitor) => m_Monitor = monitor;

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
            if (!m_Running)
                return;
            if (m_Monitor != null)
                client.SetMonitor(m_Monitor);
            client.OnDisconnect += delegate (ATCPClient client)
            {
                int clientID = client.GetID();
                m_Clients.Remove(clientID, out var _);
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
                m_Running = false;
                foreach (var pair in m_Clients)
                    pair.Value.Disconnect();
                m_Clients.Clear();
                m_ServerSocket.Close();
            }
        }

        ~ATCPServer() => Stop();
    }
}
