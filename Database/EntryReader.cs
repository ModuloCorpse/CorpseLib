using CorpseLib.Serialize;

namespace CorpseLib.Database
{
    public class EntryReader(BytesSerializer serializer, EntrySerializer entrySerializer, DB dB, byte[] bytes)
    {
        private readonly DB m_DB = dB;
        private readonly BytesReader m_BytesReader = new(serializer, bytes);
        private readonly EntrySerializer m_EntrySerializer = entrySerializer;

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
            AEntrySerializer? serializer = m_EntrySerializer.GetSerializerFor(type);
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
                EntryReader newReader = new(m_BytesReader.Serializer, m_EntrySerializer, m_DB, entryBytes);
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
            List<T?> list = [];
            int count = m_BytesReader.ReadInt();
            for (int i = 0; i < count; ++i)
            {
                OperationResult<T?> result = Read<T>();
                if (result)
                    list.Add(result.Result);
                else
                    return new(result.Error, result.Description);
            }
            return new([.. list]);
        }

        public OperationResult<object?[]> ReadArray(Type type)
        {
            List<object?> list = [];
            int count = m_BytesReader.ReadInt();
            for (int i = 0; i < count; ++i)
            {
                OperationResult<object?> result = Read(type);
                if (result)
                    list.Add(result.Result);
                else
                    return new(result.Error, result.Description);
            }
            return new([.. list]);
        }
    }
}
