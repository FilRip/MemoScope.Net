namespace WinFwk.UICommands
{
    public interface IUIDataProvider<out T>
    {
        T Data { get; }
    }
}
