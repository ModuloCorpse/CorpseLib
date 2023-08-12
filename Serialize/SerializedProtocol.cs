using CorpseLib.Network;

namespace CorpseLib.Serialize
{
    public abstract class SerializedProtocol<T> : AProtocol
    {
        protected override void OnClientConnected() { }
        protected override void OnClientDisconnected() { }

        protected sealed override OperationResult<object> Read(BytesReader reader) => reader.Read<T>().Cast<object>();

        protected sealed override void Treat(object packet) => OnReceive((T)packet);

        protected abstract void OnReceive(T obj);
    }
}
