using System.Text;

namespace CorpseLib.Serialize
{
    public class BytesWriter(BytesSerializer serializer)
    {
        private readonly BytesSerializer m_Serializer = serializer;
        private byte[] m_Bytes = [];

        public BytesSerializer Serializer => m_Serializer;
        public byte[] Bytes => m_Bytes;

        //byte
        public void Write(byte value)
        {
            byte[] tmp = new byte[m_Bytes.Length + 1];
            m_Bytes.CopyTo(tmp, 0);
            tmp[m_Bytes.Length] = value;
            m_Bytes = tmp;
        }
        public void Write(sbyte value) => Write((byte)value);

        //byte[]
        public void Write(byte[] bytes)
        {
            byte[] tmp = new byte[m_Bytes.Length + bytes.Length];
            m_Bytes.CopyTo(tmp, 0);
            bytes.CopyTo(tmp, m_Bytes.Length);
            m_Bytes = tmp;
        }

        private void WriteP(dynamic value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            Write(bytes);
        }

        public void Write(bool value) => WriteP(value);
        public void Write(char value) => WriteP(value);
        public void Write(int value) => WriteP(value);
        public void Write(uint value) => WriteP(value);
        public void Write(short value) => WriteP(value);
        public void Write(ushort value) => WriteP(value);
        public void Write(long value) => WriteP(value);
        public void Write(ulong value) => WriteP(value);
        public void Write(float value) => WriteP(value);
        public void Write(double value) => WriteP(value);
        public void Write(string str) => Write(Encoding.UTF8.GetBytes(str));
        public void WriteWithLength(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            Write(bytes.Length);
            Write(bytes);
        }
        public void Write(Guid guid) => Write(guid.ToByteArray());
        public void Write<T>(T obj) => m_Serializer.GetSerializerFor(typeof(T))?.SerializeObj(obj!, this);
        public void Write(object obj) => m_Serializer.GetSerializerFor(obj.GetType())?.SerializeObj(obj, this);
    }
}
