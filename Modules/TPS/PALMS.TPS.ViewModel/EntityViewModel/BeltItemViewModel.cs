using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace PALMS.TPS.ViewModel.EntityViewModel
{
    public class BeltItemViewModel : ViewModelBase
    {
        private bool _isSelected;
        private bool _isEmpty;
        private int _slotNumber;
        public CClientLinenViewModel OriginalObject { get; set; }

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

        public BeltItemViewModel(CClientLinenViewModel entity)
        {
            OriginalObject = entity;
        }

        public void Update(CClientLinenViewModel entity)
        {
            OriginalObject = entity;
            IsEmpty = false;
        }
    }
}
