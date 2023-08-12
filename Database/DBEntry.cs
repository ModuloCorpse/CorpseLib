namespace CorpseLib.Database
{
    public class DBEntry
    {
        private Guid m_ID = Guid.NewGuid();

        public Guid ID => m_ID;

        internal void SetID(Guid id) => m_ID = id;
    }
}
