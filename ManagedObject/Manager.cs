using CorpseLib.Json;

namespace CorpseLib.ManagedObject
{
    public abstract class Manager<T>(string directoryPath) where T : Object<T>
    {
        private readonly string m_DirPath = directoryPath;
        private readonly Dictionary<string, T> m_Objects = [];
        private T? m_CurrentObject = null;

        public T? GetObject(string id)
        {
            if (m_Objects.TryGetValue(id, out T? obj))
                return obj;
            return null;
        }

        protected void SetObject(T obj)
        {
            if (obj.ID == CurrentObjectID)
                m_CurrentObject = obj;
            m_Objects[obj.ID] = obj;
        }

        protected void AddObject(T obj) => m_Objects[obj.ID] = obj;

        public void RemoveObject(string id)
        {
            if (m_Objects.Remove(id))
            {
                File.Delete(string.Format("{0}/{1}.json", m_DirPath, id));
                if (m_CurrentObject != null && m_CurrentObject.ID == id)
                    m_CurrentObject = null;
            }
        }

        protected bool SetCurrentObject(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                m_CurrentObject = null;
                return true;
            }
            else
            {
                T? obj = GetObject(id);
                if (obj != null)
                {
                    m_CurrentObject = obj;
                    return true;
                }
                return false;
            }
        }

        public T? CurrentObject => m_CurrentObject;
        public IReadOnlyList<T> Objects => m_Objects.Values.ToList().AsReadOnly();
        public IReadOnlyList<Object<T>.Info> ObjectsInfo => m_Objects.Select(pair => pair.Value.ObjectInfo).ToList().AsReadOnly();
        public string CurrentObjectID => (m_CurrentObject != null) ? m_CurrentObject.ID : string.Empty;

        public void Load()
        {
            if (!Directory.Exists(m_DirPath))
                Directory.CreateDirectory(m_DirPath);
            else
            {
                List<Tuple<string, string>> parentsLinks = [];
                string[] files = Directory.GetFiles(m_DirPath);
                string? currentID = null;
                foreach (string file in files)
                {
                    string objID = Path.GetFileNameWithoutExtension(file);
                    try
                    {
                        JsonObject json = JsonParser.LoadFromFile(file);
                        if (objID == "settings")
                        {
                            currentID = json.GetOrDefault("current", string.Empty);
                            LoadSettings(json);
                        }
                        else
                        {
                            if (m_Objects.TryGetValue(objID, out T? oldObj))
                            {
                                oldObj.Fill(json);
                                string parent = json.GetOrDefault("parent", string.Empty)!;
                                if (!string.IsNullOrWhiteSpace(parent))
                                    parentsLinks.Add(new(parent, objID));
                            }
                            else
                            {
                                T? newObject = DeserializeObject(json);
                                if (newObject != null)
                                {
                                    AddObject(newObject);
                                    string parent = json.GetOrDefault("parent", string.Empty)!;
                                    if (!string.IsNullOrWhiteSpace(parent))
                                        parentsLinks.Add(new(parent, newObject.ID));
                                }
                            }
                        }
                    } catch {}
                }
                foreach (var parentsLink in parentsLinks)
                {
                    string parentID = parentsLink.Item1;
                    string childID = parentsLink.Item2;
                    if (m_Objects.TryGetValue(parentID, out T? parent) && m_Objects.TryGetValue(childID, out T? child))
                        child.SetParent(parent);
                }
                if (currentID != null)
                    SetCurrentObject(currentID);
            }
        }

        public void Save()
        {
            if (!Directory.Exists(m_DirPath))
                Directory.CreateDirectory(m_DirPath);

            JsonObject json = [];
            json.Add("current", m_CurrentObject?.ID ?? string.Empty);
            SaveSettings(ref json);
            JsonParser.WriteToFile(string.Format("{0}/settings.json", m_DirPath), json);

            foreach (var obj in m_Objects.Values)
            {
                if (obj.IsSerializable())
                    JsonParser.WriteToFile(string.Format("{0}/{1}.json", m_DirPath, obj.ID), obj.Serialize());
            }
        }

        public void LinkParentChild(string parentID, string childID)
        {
            if (m_Objects.TryGetValue(parentID, out T? parent) && m_Objects.TryGetValue(childID, out T? child))
                child.SetParent(parent);
        }

        protected abstract T? DeserializeObject(JsonObject obj);

        protected virtual void LoadSettings(JsonObject obj) { }
        protected virtual void SaveSettings(ref JsonObject obj) { }
    }
}
