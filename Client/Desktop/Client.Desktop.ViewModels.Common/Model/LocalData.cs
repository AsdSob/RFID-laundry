namespace Client.Desktop.ViewModels.Common.Model
{
    public class LocalData
    {
        public UserData LastUser { get; set; }
    }

    public class UserData
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}