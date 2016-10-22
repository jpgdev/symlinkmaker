namespace SymlinkMaker.Core
{
    public interface ILocator<TKey, TValue>
    {
        TValue Get(TKey key);
    }
}
