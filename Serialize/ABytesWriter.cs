using System.Collections;
using System.Text;

namespace CorpseLib.Serialize
{
    public abstract class ABytesWriter(BytesSerializer serializer)
    {
        private readonly BytesSerializer m_Serializer = serializer;

        public BytesSerializer Serializer => m_Serializer;

        //byte[]
        public abstract void Write(byte[] bytes);

        //Serializers
        public void Write<T>(T obj) => m_Serializer.Serialize(obj!, this);
        public void Write(object obj) => m_Serializer.Serialize(obj, this);
        public void WriteList<T>(IEnumerable<T> obj)
        {
            Write(obj.Count());
            //We get the serializer to avoid retrieving it for every item in list
            ABytesSerializer? serializer = (ABytesSerializer?)m_Serializer.GetSerializerFor(typeof(T));
            if (serializer == null)
                return;
            foreach (T elem in obj)
                serializer.SerializeObj(elem!, this);
        }
        public void WriteList(IEnumerable obj, Type type)
        {
            int count = 0;
            foreach (object elem in obj)
                count++;
            Write(count);
            //We get the serializer to avoid retrieving it for every item in list
            ABytesSerializer? serializer = (ABytesSerializer?)m_Serializer.GetSerializerFor(type);
            if (serializer == null)
                return;
            foreach (object elem in obj)
                serializer.SerializeObj(elem!, this);
        }
    }
}
