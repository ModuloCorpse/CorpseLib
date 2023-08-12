using CorpseLib.Serialize;

namespace CorpseLib.Database
{
    public class DB
    {
        public class Entry
        {
            private readonly Guid m_Guid;
            private byte[] m_Content;

            public Guid Guid => m_Guid;
            public byte[] Content => m_Content;

            public Entry(Guid guid, byte[] content)
            {
                m_Guid = guid;
                m_Content = content;
            }

            internal bool SetContent(byte[] content)
            {
                if (m_Content.SequenceEqual(content))
                    return false;
                m_Content = content;
                return true;
            }
        }

        private readonly Dictionary<Guid, Entry> m_Entries = new();

        public event EventHandler<Guid>? EntryUpdated;
        public event EventHandler<Guid>? EntryAdded;
        public event EventHandler<Guid>? EntryRemoved;

        internal IEnumerable<Entry> Entries => m_Entries.Values;

        public Entry? GetEntry(Guid id) => m_Entries.TryGetValue(id, out Entry? entry) ? entry : null;
        public void Insert(Entry entry)
        {
            if (m_Entries.TryGetValue(entry.Guid, out Entry? oldEntry))
            {
                oldEntry.SetContent(entry.Content);
                EntryUpdated?.Invoke(this, entry.Guid);
            }
            else
            {
                m_Entries[entry.Guid] = entry;
                EntryAdded?.Invoke(this, entry.Guid);
            }
        }

        public void Insert(DBEntry entry)
        {
            EntryWriter newWriter = new(this);
            EntrySerializer.GetSerializerFor(entry.GetType())?.SerializeObj(entry, newWriter);
            byte[] entryBytes = newWriter.Bytes;
            if (m_Entries.TryGetValue(entry.ID, out Entry? storedEntry))
            {
                if (!storedEntry.SetContent(entryBytes))
                    return;
                EntryUpdated?.Invoke(this, entry.ID);
            }
            else
            {
                m_Entries[entry.ID] = new(entry.ID, entryBytes);
                EntryAdded?.Invoke(this, entry.ID);
            }
        }

        public OperationResult<T> Retrieve<T>(Guid id)
        {
            if (m_Entries.TryGetValue(id, out Entry? storedEntry))
            {
                EntryReader reader = new(this, storedEntry.Content);
                EntrySerializer? serializer = EntrySerializer.GetSerializerFor(typeof(T));
                if (serializer == null)
                    return new("Read error", string.Format("No serializer found for {0}", typeof(T).Name));
                return serializer.DeserializeObj(reader).Cast<T>();
            }
            return new("No such entry", string.Format("No entry {0} in the database", id));
        }

        public void Save(string path) => File.WriteAllBytes(path, BytesSerializer.Serialize(this));
        public static DB Load(string path) => (File.Exists(path)) ? BytesSerializer.Deserialize<DB>(File.ReadAllBytes(path))! : new();

    }
}
