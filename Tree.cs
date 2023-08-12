namespace CorpseLib
{
    public abstract class Tree<TKeyType, TPathType, TValueType> where TPathType : notnull
    {
        private class SearchTreeNode<UPathType, UValueType> where UPathType : notnull
        {
            private readonly Dictionary<UPathType, SearchTreeNode<UPathType, UValueType>> m_Children = new();
            private bool m_HasValue = false;
            private UValueType? m_Value = default;
            private ulong m_MaxDepth = 0;

            internal void Reset()
            {
                m_Children.Clear();
                m_HasValue = false;
                m_Value = default;
                m_MaxDepth = 0;
            }

            private SearchTreeNode<UPathType, UValueType>? GetChild(UPathType c)
            {
                if (m_Children.TryGetValue(c, out var ret))
                    return ret;
                return null;
            }

            private void SetValue(UValueType value)
            {
                m_Value = value;
                m_HasValue = true;
            }

            internal bool HaveValue()
            {
                if (m_HasValue)
                    return true;
                foreach (SearchTreeNode<UPathType, UValueType> child in m_Children.Values)
                {
                    if (child.HaveValue())
                        return true;
                }
                return false;
            }

            internal void RecomputeMaxDepth(UPathType[] path)
            {
                m_MaxDepth = 0;
                SearchTreeNode<UPathType, UValueType>? dictionarySearchTreeNode = GetChild(path[0]);
                if (path.Length > 1)
                    dictionarySearchTreeNode?.RecomputeMaxDepth(path[1..]);
                foreach (SearchTreeNode<UPathType, UValueType> child in m_Children.Values)
                {
                    ulong childDepth = child.m_MaxDepth + 1;
                    if (childDepth > m_MaxDepth)
                        m_MaxDepth = childDepth;
                }
            }

            private void CleanNode()
            {
                List<UPathType> keyToRemove = new();
                foreach (var pair in m_Children)
                {
                    if (!pair.Value.HaveValue())
                        keyToRemove.Add(pair.Key);
                }
                foreach (UPathType key in keyToRemove)
                    m_Children.Remove(key);
            }

            internal void Remove(UPathType[] path)
            {
                SearchTreeNode<UPathType, UValueType>? dictionarySearchTreeNode = GetChild(path[0]);
                if (dictionarySearchTreeNode == null)
                    return;
                if (path.Length > 1)
                    dictionarySearchTreeNode.Remove(path[1..]);
                else
                {
                    dictionarySearchTreeNode.m_Value = default;
                    dictionarySearchTreeNode.m_HasValue = false;
                }
                CleanNode();
            }

            internal void Add(UPathType[] path, UValueType value)
            {
                SearchTreeNode<UPathType, UValueType>? dictionarySearchTreeNode = GetChild(path[0]);
                if (dictionarySearchTreeNode == null)
                {
                    dictionarySearchTreeNode = new();
                    m_Children[path[0]] = dictionarySearchTreeNode;
                }
                if (path.Length > 1)
                    dictionarySearchTreeNode.Add(path[1..], value);
                else
                    dictionarySearchTreeNode.SetValue(value);
            }

            internal void Fill(ref List<UValueType> ret)
            {
                if (m_HasValue && m_Value != null)
                    ret.Add(m_Value);
                foreach (SearchTreeNode<UPathType, UValueType> child in m_Children.Values)
                    child.Fill(ref ret);
            }

            internal void Get(UPathType[] path, ref List<UValueType> ret)
            {
                SearchTreeNode<UPathType, UValueType>? dictionarySearchTreeNode = GetChild(path[0]);
                if (dictionarySearchTreeNode == null)
                    return;
                if (path.Length > 1)
                {
                    if ((ulong)path.Length < (m_MaxDepth + 1))
                        dictionarySearchTreeNode.Get(path[1..], ref ret);
                }
                else
                    dictionarySearchTreeNode.Fill(ref ret);
            }

            internal void Search(UPathType[] path, ref List<UValueType> ret)
            {
                if ((ulong)path.Length > (m_MaxDepth + 1))
                    return;
                Get(path, ref ret);
                foreach (SearchTreeNode<UPathType, UValueType> child in m_Children.Values)
                    child.Search(path, ref ret);
            }
        }

        private readonly SearchTreeNode<TPathType, TValueType> m_Root = new();

        public void Clear() => m_Root.Reset();

        public void Add(TKeyType key, TValueType value)
        {
            TPathType[] path = ConvertKey(key);
            if (path.Length == 0)
                return;
            m_Root.Add(path, value);
            m_Root.RecomputeMaxDepth(path);
        }

        public void Remove(TKeyType key)
        {
            TPathType[] path = ConvertKey(key);
            if (path.Length == 0)
                return;
            m_Root.Remove(path);
            m_Root.RecomputeMaxDepth(path);
        }

        public List<TValueType> Get(TKeyType key)
        {
            TPathType[] path = ConvertKey(key);
            List<TValueType> ret = new();
            if (path.Length == 0)
                m_Root.Fill(ref ret);
            else
                m_Root.Get(path, ref ret);
            return ret;
        }

        public List<TValueType> Search(TKeyType key)
        {
            TPathType[] path = ConvertKey(key);
            List<TValueType> ret = new();
            if (path.Length == 0)
                m_Root.Fill(ref ret);
            else
                m_Root.Search(path, ref ret);
            return ret;
        }

        protected abstract TPathType[] ConvertKey(TKeyType key);
    }
}
