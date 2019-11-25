using System;
using System.ComponentModel;
using GalaSoft.MvvmLight;

namespace Laundristic.ViewModel.EntityViewModel
{
    public class ConveyorItemViewModel : ViewModelBase
    {
        private bool _isSelected;
        private bool _isEmpty;
        private int _slotNumber;
        private int _beltNumber;
        private string _rfidTag;

        public string RfidTag
        {
            get => _rfidTag;
            set => Set(() => RfidTag, ref _rfidTag, value);
        }
        public int BeltNumber
        {
            get => _beltNumber;
            set => Set(ref _beltNumber, value);
        }
        public int SlotNumber
        {
            get => _slotNumber;
            set => Set(ref _slotNumber, value);
        }
        public bool IsEmpty
        {
            get => _isEmpty;
            set => Set(ref _isEmpty, value);
        }
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }



        public ConveyorItemViewModel()
        {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RfidTag))
            {
                IsEmpty = String.IsNullOrWhiteSpace(RfidTag);
            }

        }
    }
}
