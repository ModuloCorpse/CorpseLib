using System.Text;

namespace CorpseLib.Serialize
{
    public abstract class ABytesReader(BytesSerializer serializer)
    {
        private readonly BytesSerializer m_Serializer = serializer;

        public BytesSerializer Serializer => m_Serializer;

        public abstract bool CanRead();

        //byte[]
        public abstract byte[] ReadBytes(int nb);
        public abstract byte[] ReadAll();


        public T Read<T>() => SafeRead<T>().Result!;
        public OperationResult<T> SafeRead<T>()
        {
            ABytesSerializer? serializer = m_Serializer.GetSerializerFor(typeof(T));
            if (serializer == null)
                return new("Read error", string.Format("No serializer found for {0}", typeof(T).Name));
            return serializer.DeserializeObj(this).Cast<T>();
        }

        public OperationResult<List<T>> ReadList<T>()
        {
            List<T> ret = [];
            int count = Read<int>();
            ABytesSerializer? serializer = m_Serializer.GetSerializerFor(typeof(T));
            if (serializer == null)
                return new("Read error", string.Format("No serializer found for {0}", typeof(T).Name));
            for (int i = 0; i != count; i++)
            {
                OperationResult<T> result = serializer.DeserializeObj(this).Cast<T>();
                if (result && result.Result != null)
                    ret.Add(result.Result);
                else
                    return new(result.Error, result.Description);
            }
            return new(ret);
        }
    }
}
