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
        private readonly BytesReader m_BytesReader = new();
        //private byte[] m_Buffer = Array.Empty<byte>();
        private readonly int m_ID;
        private readonly bool m_IsServerSide;
        private bool m_IsConnected = false;

        public URI GetURL() => m_URL;
        public int GetID() => m_ID;
        public bool IsConnected() => m_IsConnected;
        public bool IsServerSide() => m_IsServerSide;

        public ATCPClient(AProtocol protocol, URI url, int id = 0)
        {
            m_ReconnectionTask = new(this);
            m_Protocol = protocol;
            protocol.SetClient(this);
            m_ID = id;
            m_URL = url;
            m_IsServerSide = false;
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_Stream = Stream.Null;
        }

        internal ATCPClient(AProtocol protocol, int id, Socket socket)
        {
            m_ReconnectionTask = new(this);
            m_Protocol = protocol;
            protocol.SetClient(this);
            m_ID = id;
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

        public void SetSelfReconnectionDelay(TimeSpan delay) => m_ReconnectionTask.SetDelay(delay);
        public void SetSelfReconnectionMaxNbTry(uint maxNbTry) => m_ReconnectionTask.SetMaxNbTry(maxNbTry);
        public void SetSelfReconnectionDelayAndMaxNbTry(TimeSpan delay, uint maxNbTry) => m_ReconnectionTask.SetDelayAndMaxNbTry(delay, maxNbTry);
        public void EnableSelfReconnection() => m_ReconnectionTask.Enable();
        public void DisableSelfReconnection() => m_ReconnectionTask.Disable();

        public bool Connect() => Connect(TimeSpan.FromSeconds(1));
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
                            return true;
                        }
                        else
                        {
                            InternalDisconnect();
                        }
                    }
                }
            } catch (Exception ex)
            {
                InternalDisconnect();
            }
            return false;
        }

        protected List<object> Received(byte[] readBuffer, int bytesRead)
        {
            m_BytesReader.Append(readBuffer, bytesRead);
            while (bytesRead == readBuffer.Length)
            {
                bytesRead = m_Stream.Read(readBuffer, 0, readBuffer.Length);
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

        public void Send(object message) => m_Protocol.Send(message);

        internal void WriteToStream(byte[] message)
        {
            try
            {
                m_Stream.Write(message);
                m_Stream.Flush();
            } catch
            {
                InternalDisconnect();
            }
        }

        internal void SetStream(Stream stream) => m_Stream = stream;
        internal Stream GetStream() => m_Stream;

        private void AttemptReconnect(int attemptCount)
        {
            Thread.Sleep(1000); //Use member
            if (!Connect())
                AttemptReconnect(attemptCount + 1);
        }

        protected void InternalDisconnect()
        {
            if (Disconnect())
                m_ReconnectionTask?.Start();
        }

        public bool Disconnect()
        {
            if (m_IsConnected)
            {
                m_IsConnected = false;
                OnDisconnect?.Invoke(this);
                m_Socket.Close();
                m_Stream.Close();
                return true;
            }
            return false;
        }

        public abstract bool IsAsynchronous();

        ~ATCPClient() => InternalDisconnect();
    }
}
