namespace CorpseLib.DataNotation
{
    public abstract class DataNode
    {
        private readonly List<string> m_Comments = [];
        public string[] Comments => [..m_Comments];

        public void AddComment(string comment) => m_Comments.Add(comment);
        public void AddComments(IEnumerable<string> comments) => m_Comments.AddRange(comments);
        public void ClearComment() => m_Comments.Clear();
    }
}
