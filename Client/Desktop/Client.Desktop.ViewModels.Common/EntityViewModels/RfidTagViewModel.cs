using System.ComponentModel;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;

namespace Client.Desktop.ViewModels.Common.EntityViewModels
{
    public class RfidTagViewModel : ViewModelBase, IDecorationItem
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

        public RfidTagViewModel()
        {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsRegistered))
            {
                ItemDecorationType = IsRegistered ? ItemDecorationType.Registered : ItemDecorationType.None;
            }
        }

        public ItemDecorationType ItemDecorationType { get; set; }
    }
}
