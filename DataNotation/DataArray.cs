using System.Collections;

namespace CorpseLib.DataNotation
{
    public class DataArray : DataNode, IEnumerable<DataNode>
    {
        private Type? m_ArrayType = null;
        private readonly List<DataNode> m_Children = [];

        public Type? ArrayType => m_ArrayType;

        public DataArray() { }

        public DataArray(DataArray array)
        {
            m_ArrayType = array.m_ArrayType;
            m_Children = array.m_Children;
        }

        public DataArray(List<DataNode> children) => Add(children);

        private static Type? GetNodeType(DataNode child)
        {
            if (child is DataObject)
                return typeof(DataObject);
            else if (child is DataArray)
                return typeof(DataArray);
            else if (child is DataValue value)
                return value.Type;
            return null;
        }

        public bool Add(List<DataNode> children)
        {
            foreach (DataNode child in children)
            {
                if (!Add(child))
                {
                    m_Children.Clear();
                    return false;
                }
            }
            return true;
        }

        public bool Add(DataNode child)
        {
            if (m_ArrayType == null)
            {
                m_ArrayType = GetNodeType(child);
                if (m_ArrayType != null)
                {
                    m_Children.Add(child);
                    return true;
                }
            }
            else if (GetNodeType(child) == m_ArrayType)
            {
                m_Children.Add(child);
                return true;
            }
            return false;
        }

        public IEnumerator<DataNode> GetEnumerator() => ((IEnumerable<DataNode>)m_Children).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_Children).GetEnumerator();
    }
}
