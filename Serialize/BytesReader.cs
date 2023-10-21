using System.Text;

namespace CorpseLib.Serialize
{
    public class BytesReader
    {
        private readonly BytesSerializer m_Serializer;
        private readonly Stack<int> m_Locks = new();
        private byte[] m_Bytes;
        private int m_Idx = 0;

        public BytesReader(BytesSerializer serializer)
        {
            m_Serializer = serializer;
            m_Bytes = Array.Empty<byte>();
        }

        public BytesReader(BytesSerializer serializer, byte[] bytes)
        {
            m_Serializer = serializer;
            m_Bytes = bytes;
        }
        public BytesReader(BytesSerializer serializer, byte[] bytes, int idx) : this(serializer, bytes) => m_Idx = idx;

        public BytesSerializer Serializer => m_Serializer;
        public int Length => m_Bytes.Length - m_Idx;

        public byte[] Bytes => m_Bytes[m_Idx..];

        public void Clear()
        {
            m_Bytes = Array.Empty<byte>();
            m_Locks.Clear();
            m_Idx = 0;
        }

        public void Append(byte[] bytes) => Append(bytes, bytes.Length);

        public void Append(byte[] bytes, int nbBytes)
        {
            int bufferLength = m_Bytes.Length;
            Array.Resize(ref m_Bytes, bufferLength + nbBytes);
            for (int i = 0; i < nbBytes; ++i)
                m_Bytes[i + bufferLength] = bytes[i];
        }

        public void RemoveReadBytes()
        {
            if (m_Bytes.Length > m_Idx)
                m_Bytes = m_Bytes[m_Idx..];
            else
                m_Bytes = Array.Empty<byte>();
            m_Locks.Clear();
            m_Idx = 0;
        }

        public void LockIdx() => m_Locks.Push(m_Idx);
        public void RevertIdx() { if (m_Locks.Count > 0) m_Idx = m_Locks.Pop(); }
        public void UnlockIdx() => m_Locks.Pop();

        public bool CanRead() => m_Bytes.Length > m_Idx;

        private bool Compare(byte[] key, int idx)
        {
            int keyIdx = 0;
            while (keyIdx != key.Length)
            {
                if ((idx + keyIdx) >= m_Bytes.Length || m_Bytes[idx + keyIdx] != key[keyIdx])
                    return false;
                ++keyIdx;
            }
            return true;
        }

        public int IndexOf(byte[] key)
        {
            int idx = m_Idx;
            while (idx != m_Bytes.Length)
            {
                if (Compare(key, idx))
                    return idx - m_Idx;
                ++idx;
            }
            return -1;
        }

        //byte
        public byte ReadByte() => m_Bytes[m_Idx++];
        public sbyte ReadSByte() => (sbyte)m_Bytes[m_Idx++];

        //byte[]
        public byte[] ReadBytes(int nb)
        {
            byte[] ret = m_Bytes.Skip(m_Idx).Take(nb).ToArray();
            m_Idx += nb;
            return ret;
        }
        public byte[] ReadPBytes(int nb, bool reverse)
        {
            byte[] bytes = ReadBytes(nb);
            if (bytes.Length > 1 && reverse)
                Array.Reverse(bytes);
            return bytes;
        }
        public byte[] ReadAll()
        {
            byte[] ret = m_Bytes.Skip(m_Idx).ToArray();
            m_Idx = m_Bytes.Length;
            return ret;
        }

        public bool ReadBool() => BitConverter.ToBoolean(ReadPBytes(sizeof(bool), BitConverter.IsLittleEndian), 0);
        public char ReadChar() => BitConverter.ToChar(ReadPBytes(sizeof(char), BitConverter.IsLittleEndian), 0);
        public int ReadInt() => BitConverter.ToInt32(ReadPBytes(sizeof(int), BitConverter.IsLittleEndian), 0);
        public uint ReadUInt() => BitConverter.ToUInt32(ReadPBytes(sizeof(uint), BitConverter.IsLittleEndian), 0);
        public short ReadShort() => BitConverter.ToInt16(ReadPBytes(sizeof(short), BitConverter.IsLittleEndian), 0);
        public ushort ReadUShort() => BitConverter.ToUInt16(ReadPBytes(sizeof(ushort), BitConverter.IsLittleEndian), 0);
        public long ReadLong() => BitConverter.ToInt64(ReadPBytes(sizeof(long), BitConverter.IsLittleEndian), 0);
        public ulong ReadULong() => BitConverter.ToUInt16(ReadPBytes(sizeof(ulong), BitConverter.IsLittleEndian), 0);
        public float ReadFloat() => BitConverter.ToSingle(ReadPBytes(sizeof(float), BitConverter.IsLittleEndian), 0);
        public double ReadDouble() => BitConverter.ToDouble(ReadPBytes(sizeof(double), BitConverter.IsLittleEndian), 0);
        public int ReadInt(bool reverse) => BitConverter.ToInt32(ReadPBytes(sizeof(int), reverse), 0);
        public uint ReadUInt(bool reverse) => BitConverter.ToUInt32(ReadPBytes(sizeof(uint), reverse), 0);
        public short ReadShort(bool reverse) => BitConverter.ToInt16(ReadPBytes(sizeof(short), reverse), 0);
        public ushort ReadUShort(bool reverse) => BitConverter.ToUInt16(ReadPBytes(sizeof(ushort), reverse), 0);
        public long ReadLong(bool reverse) => BitConverter.ToInt64(ReadPBytes(sizeof(long), reverse), 0);
        public ulong ReadULong(bool reverse) => BitConverter.ToUInt16(ReadPBytes(sizeof(ulong), reverse), 0);
        public float ReadFloat(bool reverse) => BitConverter.ToSingle(ReadPBytes(sizeof(float), reverse), 0);
        public double ReadDouble(bool reverse) => BitConverter.ToDouble(ReadPBytes(sizeof(double), reverse), 0);
        public string ReadString(int length) => Encoding.UTF8.GetString(ReadBytes(length));
        public string ReadString() => Encoding.UTF8.GetString(ReadBytes(ReadInt()));
        public Guid ReadGuid() => new(ReadBytes(16));

        public OperationResult<T> Read<T>()
        {
            ABytesSerializer? serializer = m_Serializer.GetSerializerFor(typeof(T));
            if (serializer == null)
                return new("Read error", string.Format("No serializer found for {0}", typeof(T).Name));
            return serializer.DeserializeObj(this).Cast<T>();
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine("{");
            sb.Append("Read: ");
            sb.AppendLine(Encoding.UTF8.GetString(m_Bytes[..m_Idx]));
            sb.Append("To read: ");
            sb.AppendLine(Encoding.UTF8.GetString(m_Bytes[m_Idx..]));
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
