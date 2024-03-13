namespace CorpseLib.Serialize
{
    public class BytesWriter(BytesSerializer serializer) : ABytesWriter(serializer)
    {
        private byte[] m_Bytes = [];

        public byte[] Bytes => m_Bytes;
         
        //byte[]
        public override void Write(byte[] bytes)
        {
            byte[] tmp = new byte[m_Bytes.Length + bytes.Length];
            m_Bytes.CopyTo(tmp, 0);
            bytes.CopyTo(tmp, m_Bytes.Length);
            m_Bytes = tmp;
        }
    }
}
