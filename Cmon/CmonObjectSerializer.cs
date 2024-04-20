using CorpseLib.DataNotation;
using CorpseLib.Serialize;

namespace CorpseLib.Cmon
{
    public class CmonObjectSerializer : ABytesSerializer<DataObject>
    {
        protected override OperationResult<DataObject> Deserialize(ABytesReader reader)
        {
            try
            {
                return new(CmonParser.Parse(reader.Read<string>()));
            } catch (CmonException e)
            {
                return new("Deserialization error", e.Message);
            }
        }

        protected override void Serialize(DataObject obj, ABytesWriter writer)
        {
            string str = CmonParser.NetStr(obj);
            writer.Write(str);
        }
    }
}
