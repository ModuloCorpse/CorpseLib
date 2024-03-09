using System.Text;

namespace CorpseLib.Network
{
    public class FullServerDebugMonitor : IMonitor
    {
        public void OnClose() => Console.WriteLine("Connection with client closed");
        public void OnOpening() => Console.WriteLine("Connection with client opening");
        public void OnOpen() => Console.WriteLine("Connection with client open");
        public void OnReopening() => Console.WriteLine("Connection with client reopening");
        public void OnReopen() => Console.WriteLine("Connection with client reopenned");
        public void OnReceive(byte[] bytes) => Console.WriteLine(string.Format("Received from client {0} bytes [UTF-8 : {1}]", bytes.Length, Encoding.UTF8.GetString(bytes)));
        public void OnReceive(object obj) => Console.WriteLine(string.Format("Received from client: {0}", obj));
        public void OnSend(object obj) => Console.WriteLine(string.Format("Sent to client: {0}", obj));
        public void OnSend(byte[] bytes) => Console.WriteLine(string.Format("Sent to client {0} bytes [UTF-8 : {1}]", bytes.Length, Encoding.UTF8.GetString(bytes)));
        public void OnException(Exception ex) => Console.WriteLine(ex);
    }
}
