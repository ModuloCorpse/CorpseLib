using CorpseLib.Serialize;

namespace CorpseLib.Database
{

    [DefaultSerializer]
    public class DBEntrySerializer : BytesSerializer<DB.Entry>
    {
        protected override OperationResult<DB.Entry> Deserialize(BytesReader reader)
        {
            Guid id = reader.ReadGuid();
            int entryLength = reader.ReadInt();
            byte[] entryBytes = reader.ReadBytes(entryLength);
            return new(new(id, entryBytes));
        }

        protected override void Serialize(DB.Entry obj, BytesWriter writer)
        {
            writer.Write(obj.Guid);
            writer.Write(obj.Content.Length);
            writer.Write(obj.Content);
        }
    }

    [DefaultSerializer]
    public class DBSerializer : BytesSerializer<DB>
    {
        protected override OperationResult<DB> Deserialize(BytesReader reader)
        {
            DB db = new();
            int nbEntries = reader.ReadInt();
            for (int i = 0; i != nbEntries; ++i)
            {
                OperationResult<DB.Entry> result = reader.Read<DB.Entry>();
                if (result && result.Result is DB.Entry entry)
                    db.Insert(entry);
            }
            return new(db);
        }

        protected override void Serialize(DB obj, BytesWriter writer)
        {
            IEnumerable<DB.Entry> entries = obj.Entries;
            writer.Write(entries.Count());
            foreach (DB.Entry entry in entries)
                writer.Write(entry);
        }
    }
}
