namespace PALMS.Notes.ViewModel
{
    public interface ISettingsContent
    {
        string Name { get; }

        int NoteStatus { get; }
        //bool HasChanges();
    }
}