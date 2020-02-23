namespace Client.Desktop.ViewModels.Common
{
    public interface IPassword
    {
        string Password { get; set; }
    }

    public interface ISelectedItem
    {
        object GetSelected();
    }
}
