namespace CorpseLib
{
    public class DictionaryTree<T> : Tree<string, char, T>
    {
        protected override char[] ConvertKey(string key) => key.ToCharArray();
    }
}
