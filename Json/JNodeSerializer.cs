using CorpseLib.Serialize;
using System.Text;

namespace CorpseLib.Json
{
    public class JNodeSerializer : ABytesSerializer<JNode>
    {
        protected override OperationResult<JNode> Deserialize(BytesReader reader)
        {
            JReader jreader = new(reader.ReadString(reader.ReadInt()));
            try
            {
                return new(jreader.ReadNext());
            } catch (JException e)
            {
                return new("Deserialization error", e.Message);
            }
        }

        protected override void Serialize(JNode obj, BytesWriter writer)
        {
            string str = obj.ToNetworkString();
            writer.Write(Encoding.UTF8.GetBytes(str).Length);
            writer.Write(str);
        }
    }
}
