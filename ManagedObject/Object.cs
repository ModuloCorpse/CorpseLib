using CorpseLib.Json;

namespace CorpseLib.ManagedObject
{
    public abstract class Object<CRTP> where CRTP : Object<CRTP>
    {
        public class Info(string id, string name)
        {
            private readonly string m_ID = id;
            private readonly string m_Name = name;
            public string ID => m_ID;
            public string Name => m_Name;
            public override string ToString() => m_Name;
        }

        private readonly bool m_IsSerializable = true;
        private readonly Info m_ObjectInfo;
        private CRTP? m_Parent = default;

        protected Object(string name, bool isSerializable = true)
        {
            m_ObjectInfo = new(Guid.NewGuid().ToString(), name);
            m_IsSerializable = isSerializable;
        }

        protected Object(string id, string name, bool isSerializable = true)
        {
            m_ObjectInfo = new(id, name);
            m_IsSerializable = isSerializable;
        }

        protected Object(JObject json)
        {
            string id = json.GetOrDefault("id", Guid.NewGuid().ToString())!;
            string name = json.GetOrDefault("name", string.Empty);
            m_ObjectInfo = new(id, name!);
            Load(json);
        }

        internal void Fill(JObject json) => Load(json);

        public bool IsSerializable() => m_IsSerializable;

        public void SetParent(CRTP parent) => m_Parent = parent;

        internal JFile Serialize()
        {
            JFile json = new()
            {
                { "id", ID },
                { "name", Name }
            };
            if (m_Parent != null)
                json.Add("parent", m_Parent.ID);
            JObject obj = json;
            Save(ref obj);
            return json;
        }

        abstract protected void Save(ref JObject json);
        abstract protected void Load(JObject json);

        public override bool Equals(object? obj) => (obj != null && obj is Object<CRTP> other && other.ID == ID);

        public override int GetHashCode() => HashCode.Combine(ID);

        public string ID => m_ObjectInfo.ID;
        public string Name => m_ObjectInfo.Name;
        public CRTP? Parent => m_Parent;
        public Info ObjectInfo => m_ObjectInfo;
    }
}
