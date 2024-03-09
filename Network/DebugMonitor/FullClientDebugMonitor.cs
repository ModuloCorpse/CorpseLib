using System.Text;

namespace CorpseLib.Network
{
    public class FullClientDebugMonitor : IMonitor
    {
        public void OnClose() => Console.WriteLine("Connection with server closed");
        public void OnOpening() => Console.WriteLine("Connection with server opening");
        public void OnOpen() => Console.WriteLine("Connection with server open");
        public void OnReopening() => Console.WriteLine("Connection with server reopening");
        public void OnReopen() => Console.WriteLine("Connection with server reopenned");
        public void OnReceive(byte[] bytes) => Console.WriteLine(string.Format("Received from server {0} bytes [UTF-8 : {1}]", bytes.Length, Encoding.UTF8.GetString(bytes)));
        public void OnReceive(object obj) => Console.WriteLine(string.Format("Received from server: {0}", obj));
        public void OnSend(object obj) => Console.WriteLine(string.Format("Sent to server: {0}", obj));
        public void OnSend(byte[] bytes) => Console.WriteLine(string.Format("Sent to server {0} bytes [UTF-8 : {1}]", bytes.Length, Encoding.UTF8.GetString(bytes)));
        public void OnException(Exception ex) => Console.WriteLine(ex);
    }
}
