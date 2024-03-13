using CorpseLib.Serialize;

namespace CorpseLib.Database
{
    public class DBEntrySerializer : ABytesSerializer<DB.Entry>
    {
        protected override OperationResult<DB.Entry> Deserialize(ABytesReader reader)
        {
            Guid id = reader.Read<Guid>();
            int entryLength = reader.Read<int>();
            byte[] entryBytes = reader.ReadBytes(entryLength);
            return new(new(id, entryBytes));
        }

        protected override void Serialize(DB.Entry obj, ABytesWriter writer)
        {
            writer.Write(obj.Guid);
            writer.Write(obj.Content.Length);
            writer.Write(obj.Content);
        }
    }

    public class DBSerializer : ABytesSerializer<DB>
    {
        protected override OperationResult<DB> Deserialize(ABytesReader reader)
        {
            DB db = new();
            int nbEntries = reader.Read<int>();
            for (int i = 0; i != nbEntries; ++i)
            {
                OperationResult<DB.Entry> result = reader.SafeRead<DB.Entry>();
                if (result && result.Result is DB.Entry entry)
                    db.Insert(entry);
            }
            return new(db);
        }

        protected override void Serialize(DB obj, ABytesWriter writer)
        {
            IEnumerable<DB.Entry> entries = obj.Entries;
            writer.Write(entries.Count());
            foreach (DB.Entry entry in entries)
                writer.Write(entry);
        }
    }
}
