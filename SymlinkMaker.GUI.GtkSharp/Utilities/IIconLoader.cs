namespace SymlinkMaker.GUI.GtkSharp
{
    public interface IIconLoader<T>
    {
        T Load(string name);
    }
}

