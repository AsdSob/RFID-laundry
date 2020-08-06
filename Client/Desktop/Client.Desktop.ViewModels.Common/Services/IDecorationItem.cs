namespace Client.Desktop.ViewModels.Common.Services
{
    public interface IDecorationItem
    {
        ItemDecorationType ItemDecorationType{ get; }
    }

    public enum ItemDecorationType
    {
        None =0,
        Registered = 1,

    }

}
