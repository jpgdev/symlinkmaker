namespace SymlinkMaker.Core
{
    // TODO : Finc better name
    public interface ILocator<TKey, TValue>
	{
        TValue Get(TKey key);
	}
}
