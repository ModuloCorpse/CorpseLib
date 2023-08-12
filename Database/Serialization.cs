using CorpseLib.Serialize;

namespace CorpseLib.Database
{
    public class EntryWriter
    {
        private readonly DB m_DB;
        private readonly BytesWriter m_BytesWriter = new();

        internal byte[] Bytes => m_BytesWriter.Bytes;

        public EntryWriter(DB dB) => m_DB = dB;
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
                EntrySerializer.GetSerializerFor(type)?.SerializeObj(obj, this);
        }

        public void Write<T>(T obj) => Write(typeof(T), obj);
        public void Write(object obj) => Write(obj.GetType(), obj);
    }
    
    public class EntryReader
    {
        private readonly DB m_DB;
        private readonly BytesReader m_BytesReader;

        public EntryReader(DB dB, byte[] bytes)
        {
            m_DB = dB;
            m_BytesReader = new(bytes);
        }

        public void RemoveReadBytes() => m_BytesReader.RemoveReadBytes();
        public void LockIdx() => m_BytesReader.LockIdx();
        public void RevertIdx() => m_BytesReader.RevertIdx();
        public void UnlockIdx() => m_BytesReader.UnlockIdx();
        public bool CanRead() => m_BytesReader.CanRead();
        public int IndexOf(byte[] key) => m_BytesReader.IndexOf(key);
        public byte ReadByte() => m_BytesReader.ReadByte();
        public sbyte ReadSByte() => m_BytesReader.ReadSByte();
        public byte[] ReadBytes(int nb) => m_BytesReader.ReadBytes(nb);
        public byte[] ReadAll() => m_BytesReader.ReadAll();
        public bool ReadBool() => m_BytesReader.ReadBool();
        public char ReadChar() => m_BytesReader.ReadChar();
        public int ReadInt() => m_BytesReader.ReadInt();
        public uint ReadUInt() => m_BytesReader.ReadUInt();
        public short ReadShort() => m_BytesReader.ReadShort();
        public ushort ReadUShort() => m_BytesReader.ReadUShort();
        public long ReadLong() => m_BytesReader.ReadLong();
        public ulong ReadULong() => m_BytesReader.ReadULong();
        public float ReadFloat() => m_BytesReader.ReadFloat();
        public double ReadDouble() => m_BytesReader.ReadDouble();
        public string ReadString(int length) => m_BytesReader.ReadString(length);
        public string ReadString() => m_BytesReader.ReadString();

        public OperationResult<object?> Read(Type type)
        {
            EntrySerializer? serializer = EntrySerializer.GetSerializerFor(type);
            if (serializer == null)
                return new("Read error", string.Format("No serializer found for {0}", type.Name));
            if (type.IsAssignableTo(typeof(DBEntry)))
            {
                Guid entryID = m_BytesReader.ReadGuid();
                if (entryID == Guid.Empty)
                    return new(null);
                byte[]? entryBytes = m_DB.GetEntry(entryID)?.Content;
                if (entryBytes == null)
                    return new("Bad link", string.Format("Link {0} does not exist in DB", entryID));
                EntryReader newReader = new(m_DB, entryBytes);
                OperationResult<object?> result = serializer.DeserializeObj(newReader);
                if (result && result.Result is DBEntry entry)
                    entry.SetID(entryID);
                return result;
            }
            return serializer.DeserializeObj(this);
        }

        public OperationResult<T?> Read<T>() => Read(typeof(T)).Cast<T?>();

        public OperationResult<T?[]> ReadArray<T>()
        {
            List<T?> list = new();
            int count = m_BytesReader.ReadInt();
            for (int i = 0; i < count; ++i)
            {
                OperationResult<T?> result = Read<T>();
                if (result)
                    list.Add(result.Result);
                else
                    return new(result.Error, result.Description);
            }
            return new(list.ToArray());
        }

        public OperationResult<object?[]> ReadArray(Type type)
        {
            List<object?> list = new();
            int count = m_BytesReader.ReadInt();
            for (int i = 0; i < count; ++i)
            {
                OperationResult<object?> result = Read(type);
                if (result)
                    list.Add(result.Result);
                else
                    return new(result.Error, result.Description);
            }
            return new(list.ToArray());
        }
    }

    public abstract class EntrySerializer : Serializer<EntryReader, EntryWriter>
    {
        public static new EntrySerializer? GetSerializerFor(string assemblyQualifiedName) => (EntrySerializer?)Serializer<EntryReader, EntryWriter>.GetSerializerFor(assemblyQualifiedName);
        public static new EntrySerializer? GetSerializerFor(Type type) => (EntrySerializer?)Serializer<EntryReader, EntryWriter>.GetSerializerFor(type);
        public override string ToString() => "EntrySerializer[" + GetSerializedType().Name + "]";
    }

    public abstract class EntrySerializer<T> : EntrySerializer
    {
        internal override OperationResult<object?> DeserializeObj(EntryReader reader) => Deserialize(reader).Cast<object?>();
        internal override void SerializeObj(object obj, EntryWriter writer) => Serialize((T)obj, writer);
        internal override Type GetSerializedType() => typeof(T);
        protected abstract OperationResult<T> Deserialize(EntryReader reader);
        protected abstract void Serialize(T obj, EntryWriter writer);
    }
}
