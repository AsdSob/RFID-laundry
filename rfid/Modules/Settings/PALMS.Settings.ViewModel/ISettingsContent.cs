namespace PALMS.Settings.ViewModel
{
    public interface ISettingsContent
    {
        string Name { get; }

        bool HasChanges();
    }
}