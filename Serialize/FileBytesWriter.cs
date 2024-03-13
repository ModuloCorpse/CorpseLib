namespace CorpseLib.Serialize
{
    public class FileBytesWriter(BytesSerializer serializer, string fileName) : ABytesWriter(serializer)
    {
        private readonly FileStream m_File = new(fileName, FileMode.Create, FileAccess.Write);

        public override void Write(byte[] bytes) => m_File.Write(bytes, 0, bytes.Length);
    }
}
