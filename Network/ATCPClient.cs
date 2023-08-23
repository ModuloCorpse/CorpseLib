using CorpseLib.Serialize;
using System.Net;
using System.Net.Sockets;

namespace CorpseLib.Network
{
    /// <summary>
    /// Class representing an asynchronous TCP client
    /// </summary>
    public abstract class ATCPClient
    {
        public delegate void Callback(ATCPClient client);
        public event Callback? OnDisconnect;

        private readonly ReconnectionTask m_ReconnectionTask;
        private readonly AProtocol m_Protocol;
        private readonly URI m_URL;
        private Socket m_Socket;
        protected Stream m_Stream;
        private readonly BytesSerializer m_BytesSerializer = new();
        private readonly BytesReader m_BytesReader;
        private IMonitor? m_Monitor = null;
        private readonly int m_ID;
        private readonly bool m_IsServerSide;
        private bool m_IsConnected = false;

        public URI GetURL() => m_URL;
        public int GetID() => m_ID;
        public bool IsConnected() => m_IsConnected;
        public bool IsServerSide() => m_IsServerSide;

        private ATCPClient(AProtocol protocol, int id)
        {
            m_Protocol = protocol;
            m_Protocol.FillSerializer(ref m_BytesSerializer);
            m_BytesReader = new(m_BytesSerializer);
            m_ReconnectionTask = new(this);
            protocol.SetClient(this);
            m_ID = id;
            m_URL = new(string.Empty, null, string.Empty, string.Empty, string.Empty);
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_Stream = Stream.Null;
        }

        public ATCPClient(AProtocol protocol, URI url, int id = 0) : this(protocol, id)
        {
            m_URL = url;
            m_IsServerSide = false;
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_Stream = Stream.Null;
        }

        internal ATCPClient(AProtocol protocol, int id, Socket socket) : this(protocol, id)
        {
            m_IsServerSide = true;
            m_IsConnected = true;
            m_Socket = socket;
            IPEndPoint? endpoint = (IPEndPoint?)socket.RemoteEndPoint;
            m_URL = URI.Build(string.Empty)
                .Host(endpoint?.Address?.ToString() ?? string.Empty)
                .Port(endpoint?.Port ?? -1).Build();
            m_Stream = new NetworkStream(socket);
            m_Protocol.ClientConnected();
        }

        internal BytesSerializer BytesSerializer => m_BytesSerializer;

        public void SetMonitor(IMonitor monitor)
        {
            m_Monitor = monitor;
            if (m_IsServerSide)
                m_Monitor?.OnOpen();
        }

        public void SetSelfReconnectionDelay(TimeSpan delay) => m_ReconnectionTask.SetDelay(delay);
        public void SetSelfReconnectionMaxNbTry(uint maxNbTry) => m_ReconnectionTask.SetMaxNbTry(maxNbTry);
        public void SetSelfReconnectionDelayAndMaxNbTry(TimeSpan delay, uint maxNbTry) => m_ReconnectionTask.SetDelayAndMaxNbTry(delay, maxNbTry);
        public void EnableSelfReconnection() => m_ReconnectionTask.Enable();
        public void DisableSelfReconnection() => m_ReconnectionTask.Disable();

        public bool Connect() => Connect(TimeSpan.FromMinutes(1));
        public bool Connect(TimeSpan timeout)
        {
            if (m_IsServerSide)
                return false;
            try
            {
                //Recreate socket and reset stream to allow reconnection
                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_Stream = Stream.Null;
                foreach (IPAddress ip in Dns.GetHostEntry(m_URL.Host).AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        IAsyncResult result = m_Socket.BeginConnect(ip, m_URL.Port, null, null);
                        result.AsyncWaitHandle.WaitOne(timeout, true);
                        if (m_Socket.Connected)
                        {
                            m_Socket.EndConnect(result);
                            m_Stream = new NetworkStream(m_Socket);
                            m_Protocol.ClientConnected();
                            m_IsConnected = true;
                            m_Monitor?.OnOpen();
                            return true;
                        }
                        else
                        {
                            InternalDisconnect();
                        }
                    }
                }
            } catch
            {
                InternalDisconnect();
            }
            return false;
        }

        protected List<object> Received(byte[] readBuffer, int bytesRead)
        {
            m_Monitor?.OnReceive(readBuffer[..bytesRead]);
            m_BytesReader.Append(readBuffer, bytesRead);
            while (bytesRead == readBuffer.Length)
            {
                bytesRead = m_Stream.Read(readBuffer, 0, readBuffer.Length);
                m_Monitor?.OnReceive(readBuffer[..bytesRead]);
                m_BytesReader.Append(readBuffer, bytesRead);
            }
            List<object> packets = new();
            bool readSuccess;
            do
            {
                m_BytesReader.LockIdx();
                OperationResult<object> result = m_Protocol.ReadFromStream(m_BytesReader);
                if (result)
                {
                    m_BytesReader.RemoveReadBytes();
                    object? resultData = result.Result;
                    if (resultData != null)
                    {
                        packets.Add(resultData);
                        m_Monitor?.OnReceive(resultData);
                        m_Protocol.TreatPacket(resultData);
                    }
                    readSuccess = true;
                }
                else
                {
                    m_BytesReader.RevertIdx();
                    readSuccess = false;
                }
            } while (m_BytesReader.CanRead() && readSuccess);
            return packets;
        }

        public abstract List<object> Read();

        public void Send(object message)
        {
            m_Monitor?.OnSend(message);
            m_Protocol.Send(message);
        }

        internal void WriteToStream(byte[] message)
        {
            try
            {
                m_Stream.Write(message);
                m_Stream.Flush();
                m_Monitor?.OnSend(message);
            } catch
            {
                InternalDisconnect();
            }
        }

        internal void SetStream(Stream stream) => m_Stream = stream;
        internal Stream GetStream() => m_Stream;

        protected void InternalDisconnect()
        {
            if (Disconnect())
                Reconnect();
        }

        public void Reconnect() => m_ReconnectionTask?.Start();

        public bool Disconnect()
        {
            if (m_IsConnected)
            {
                m_IsConnected = false;
                OnDisconnect?.Invoke(this);
                m_Socket.Close();
                m_Stream.Close();
                m_Monitor?.OnClose();
                return true;
            }
            return false;
        }

        public abstract bool IsAsynchronous();

        ~ATCPClient() => InternalDisconnect();
    }
}
