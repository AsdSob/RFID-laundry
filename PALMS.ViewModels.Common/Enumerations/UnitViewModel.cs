using GalaSoft.MvvmLight;

namespace PALMS.ViewModels.Common.Enumerations
{
    public class UnitViewModel : ViewModelBase
    {
        public int Id { get; }
        public string Name { get; }

        public UnitViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
