namespace CorpseLib.Serialize
{
    public class FileBytesReader(BytesSerializer serializer, string fileName) : ABytesReader(serializer)
    {
        private readonly FileStream m_File = File.Open(fileName, FileMode.Open, FileAccess.Read);
        private int m_Idx = 0;

        public override bool CanRead() => m_File.Length > m_Idx;

        public override byte[] ReadAll()
        {
            byte[] ret = [];
            byte[] buffer = new byte[4096];
            int nbBytes = m_File.Read(buffer, m_Idx, 4096);
            while (nbBytes > 0)
            {
                int bufferLength = ret.Length;
                Array.Resize(ref ret, bufferLength + nbBytes);
                for (int i = 0; i < nbBytes; ++i)
                    ret[i + bufferLength] = buffer[i];
                m_Idx += nbBytes;
                nbBytes = m_File.Read(buffer, 0, 4096);
            }
            return ret;
        }

        public override byte[] ReadBytes(int nb)
        {
            byte[] buff = new byte[nb];
            m_Idx += m_File.Read(buff, 0, nb);
            return buff;
        }

        public void Close()
        {
            m_File.Close();
            m_File.Dispose();
        }
    }
}
