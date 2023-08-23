namespace CorpseLib.Network
{
    public interface IMonitor
    {
        void OnOpen();
        void OnSend(object obj);
        void OnSend(byte[] bytes);
        void OnReceive(byte[] bytes);
        void OnReceive(object obj);
        void OnClose();
    }
}
