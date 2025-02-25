using System.Text;

namespace CorpseLib.Serialize
{
    public class BytesReader : ABytesReader
    {
        private readonly Stack<int> m_Locks = new();
        private byte[] m_Bytes;
        private int m_Idx = 0;

        public int Length => m_Bytes.Length - m_Idx;
        public byte[] Bytes => m_Bytes[m_Idx..];

        public BytesReader(BytesSerializer serializer) : base(serializer) => m_Bytes = [];
        public BytesReader(BytesSerializer serializer, byte[] bytes) : base(serializer) => m_Bytes = bytes;
        public BytesReader(BytesSerializer serializer, byte[] bytes, int idx) : this(serializer, bytes) => m_Idx = idx;

        public void Clear()
        {
            m_Bytes = [];
            m_Locks.Clear();
            m_Idx = 0;
        }

        public void LockIdx() => m_Locks.Push(m_Idx);
        public void RevertIdx() { if (m_Locks.Count > 0) m_Idx = m_Locks.Pop(); }
        public void UnlockIdx() => m_Locks.Pop();

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
                m_Bytes = [];
            m_Locks.Clear();
            m_Idx = 0;
        }

        public override bool CanRead() => m_Bytes.Length > m_Idx;

        public bool StartWith(byte[] key) => Compare(key, m_Idx);

        public int IndexOf(byte[] key)
        {
            int idx = m_Idx;
            while (idx < m_Bytes.Length)
            {
                if (Compare(key, idx))
                    return idx - m_Idx;
                ++idx;
            }
            return -1;
        }

        //byte[]
        public override byte[] ReadBytes(int nb)
        {
            byte[] ret = m_Bytes.Skip(m_Idx).Take(nb).ToArray();
            m_Idx += nb;
            return ret;
        }
        public override byte[] ReadAll()
        {
            byte[] ret = m_Bytes.Skip(m_Idx).ToArray();
            m_Idx = m_Bytes.Length;
            return ret;
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
