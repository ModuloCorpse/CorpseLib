namespace CorpseLib.Canal
{
    public class CanalManager<TCanalID> where TCanalID : notnull
    {
        private readonly Dictionary<TCanalID, Canal<TCanalID>> m_Canals = new();
        private readonly Dictionary<TCanalID, TriggerCanal<TCanalID>> m_TriggerCanals = new();

        public bool DeleteCanal(TCanalID canalID)
        {
            if (m_TriggerCanals.ContainsKey(canalID))
            {
                m_TriggerCanals.Remove(canalID);
                return true;
            }
            else if (m_Canals.ContainsKey(canalID))
            {
                m_Canals.Remove(canalID);
                return true;
            }
            return false;
        }

        public bool NewCanal<T>(TCanalID canalID)
        {
            if (m_Canals.ContainsKey(canalID))
                return false;
            m_Canals[canalID] = new Canal<TCanalID>(canalID, typeof(T));
            return true;
        }

        public bool NewCanal(TCanalID canalID)
        {
            if (m_TriggerCanals.ContainsKey(canalID))
                return false;
            m_TriggerCanals[canalID] = new TriggerCanal<TCanalID>(canalID);
            return true;
        }

        public bool Register<T>(TCanalID canalID, Action<TCanalID, object?> listener)
        {
            if (m_Canals.TryGetValue(canalID, out Canal<TCanalID>? canal) && canal.IsValid<T>())
            {
                canal.Add(listener);
                return true;
            }
            return false;
        }

        public bool Register(TCanalID canalID, Action<TCanalID> trigger)
        {
            if (m_TriggerCanals.TryGetValue(canalID, out TriggerCanal<TCanalID>? canal))
            {
                canal.Add(trigger);
                return true;
            }
            return false;
        }

        public bool Unregister<T>(TCanalID canalID, Action<TCanalID, object?> listener)
        {
            if (m_Canals.TryGetValue(canalID, out Canal<TCanalID>? canal) && canal.IsValid<T>())
            {
                canal.Remove(listener);
                return true;
            }
            return false;
        }

        public bool Unregister(TCanalID canalID, Action<TCanalID> trigger)
        {
            if (m_TriggerCanals.TryGetValue(canalID, out TriggerCanal<TCanalID>? canal))
            {
                canal.Remove(trigger);
                return true;
            }
            return false;
        }

        public bool Clear(TCanalID canalID)
        {
            if (m_TriggerCanals.TryGetValue(canalID, out TriggerCanal<TCanalID>? triggerCanal))
            {
                triggerCanal.Clear();
                return true;
            }
            else if (m_Canals.TryGetValue(canalID, out Canal<TCanalID>? canal))
            {
                canal.Clear();
                return true;
            }
            return false;
        }

        public bool Emit<T>(TCanalID canalID, T args)
        {
            if (m_Canals.TryGetValue(canalID, out Canal<TCanalID>? canal) && canal.IsValid<T>())
            {
                canal.Iterate(args);
                return true;
            }
            return false;
        }

        public bool Emit(TCanalID canalID)
        {
            if (m_TriggerCanals.TryGetValue(canalID, out TriggerCanal<TCanalID>? canal))
            {
                canal.Iterate();
                return true;
            }
            return false;
        }
    }
}
