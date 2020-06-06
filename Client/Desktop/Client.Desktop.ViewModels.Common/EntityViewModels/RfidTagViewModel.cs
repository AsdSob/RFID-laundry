using Client.Desktop.ViewModels.Common.ViewModels;

namespace Client.Desktop.ViewModels.Common.EntityViewModels
{
    public class RfidTagViewModel : ViewModelBase
    {
        private string _tag;
        private int _antenna;
        private bool _isRegistered;

        public bool IsRegistered
        {
            get => _isRegistered;
            set => Set(ref _isRegistered, value);
        }
        public int Antenna
        {
            get => _antenna;
            set => Set(ref _antenna, value);
        }
        public string Tag
        {
            get => _tag;
            set => Set(ref _tag, value);
        }
    }
}
