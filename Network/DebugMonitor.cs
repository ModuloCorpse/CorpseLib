using System.Text;

namespace CorpseLib.Network
{
    public class DebugMonitor : IMonitor
    {
        public void OnClose() => Console.WriteLine("Connection closed");

        public void OnOpen() => Console.WriteLine("Connection open");

        public void OnReceive(byte[] bytes) => Console.WriteLine(string.Format("Received {0} bytes [UTF-8 : {1}]", bytes.Length, Encoding.UTF8.GetString(bytes)));

        public void OnReceive(object obj) => Console.WriteLine(string.Format("Received: {0}", obj));

        public void OnSend(object obj) => Console.WriteLine(string.Format("Sent: {0}", obj));

        public void OnSend(byte[] bytes) => Console.WriteLine(string.Format("Sent {0} bytes [UTF-8 : {1}]", bytes.Length, Encoding.UTF8.GetString(bytes)));
    }
}
