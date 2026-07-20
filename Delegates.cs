namespace CorpseLib
{
    public delegate Task AsyncEventHandler();
    public delegate Task AsyncEventHandler<TArgs>(TArgs args);
}
