namespace CorpseLib.Json
{
    public class JException : Exception
    {
        public JException() {}
        public JException(string message) : base(message) {}
    }
}
