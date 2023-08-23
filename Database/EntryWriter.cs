using CorpseLib.Serialize;

namespace CorpseLib.Database
{
    public class EntryWriter
    {
        private readonly DB m_DB;
        private readonly BytesWriter m_BytesWriter;
        private readonly EntrySerializer m_EntrySerializer;

        internal byte[] Bytes => m_BytesWriter.Bytes;

        public EntryWriter(BytesSerializer serializer, EntrySerializer entrySerializer, DB dB)
        {
            m_DB = dB;
            m_BytesWriter = new(serializer);
            m_EntrySerializer = entrySerializer;
        }

        public void Write(byte value) => m_BytesWriter.Write(value);
        public void Write(sbyte value) => m_BytesWriter.Write(value);
        public void Write(byte[] bytes) => m_BytesWriter.Write(bytes);
        public void Write(bool value) => m_BytesWriter.Write(value);
        public void Write(char value) => m_BytesWriter.Write(value);
        public void Write(int value) => m_BytesWriter.Write(value);
        public void Write(uint value) => m_BytesWriter.Write(value);
        public void Write(short value) => m_BytesWriter.Write(value);
        public void Write(ushort value) => m_BytesWriter.Write(value);
        public void Write(long value) => m_BytesWriter.Write(value);
        public void Write(ulong value) => m_BytesWriter.Write(value);
        public void Write(float value) => m_BytesWriter.Write(value);
        public void Write(double value) => m_BytesWriter.Write(value);
        public void Write(string str) => m_BytesWriter.Write(str);
        public void WriteWithLength(string str) => m_BytesWriter.WriteWithLength(str);

        public void WriteArray<T>(IEnumerable<T> arr)
        {
            Type type = typeof(T);
            m_BytesWriter.Write(arr.Count());
            foreach (T item in arr)
                Write(type, item);
        }

        public void WriteArray(IEnumerable<object> arr)
        {
            int count = arr.Count();
            m_BytesWriter.Write(count);
            if (count > 0)
            {
                Type type = arr.ElementAt(0).GetType();
                foreach (object item in arr)
                    Write(type, item);
            }
        }

        private void Write(Type type, object? obj)
        {
            if (type.IsAssignableTo(typeof(DBEntry)))
            {
                if (obj == null)
                    m_BytesWriter.Write(Guid.Empty);
                else if (obj is DBEntry entry)
                {
                    m_DB.Insert(entry);
                    m_BytesWriter.Write(entry.ID);
                }
            }
            else if (obj != null)
                m_EntrySerializer.GetSerializerFor(type)?.SerializeObj(obj, this);
        }

        public void Write<T>(T obj) => Write(typeof(T), obj);
        public void Write(object obj) => Write(obj.GetType(), obj);
    }
}
