using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.ViewModels;

namespace Conveyor
{
    public class ConveyorItemViewModel : ViewModelBase
    {
        private bool _isSelected;
        private bool _isEmpty;
        private int _slotNumber;
        private int _beltNumber;
        public ClientLinenViewModel OriginalObject { get; set; }

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

        public ConveyorItemViewModel(ClientLinenViewModel entity)
        {
            OriginalObject = entity;

        }

        public ConveyorItemViewModel()
        {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OriginalObject))
            {
                if (OriginalObject == null)
                {
                    IsEmpty = true;
                }
                else
                {
                    IsEmpty = false;
                }
            }

        }
    }
}
