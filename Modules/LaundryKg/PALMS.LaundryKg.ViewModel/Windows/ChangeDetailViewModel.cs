using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;

namespace PALMS.LaundryKg.ViewModel.Windows
{
    public class ChangeDetailViewModel : ViewModelBase, IWindowDialogViewModel
    {
        public Action<bool> CloseAction { get; set; }

        private readonly IDialogService _dialogService;
        private List<UnitViewModel> _staffShifts;
        private List<UnitViewModel> _kgTypes;
        private DateTime _washingDate;
        private int _selectedShiftId;
        private int _selectedKgTypeId;
        private DateTime _oldWashingDate;
        private int _oldShiftId;
        private int _oldKyTypeId;

        public int OldKyTypeId
        {
            get => _oldKyTypeId;
            set => Set(ref _oldKyTypeId, value);
        }
        public int OldShiftId
        {
            get => _oldShiftId;
            set => Set(ref _oldShiftId, value);
        }
        public DateTime OldWashingDate
        {
            get => _oldWashingDate;
            set => Set(ref _oldWashingDate, value);
        }
        public int SelectedKgTypeId
        {
            get => _selectedKgTypeId;
            set => Set(ref _selectedKgTypeId, value);
        }
        public int SelectedShiftId
        {
            get => _selectedShiftId;
            set => Set(ref _selectedShiftId, value);
        }
        public DateTime WashingDate
        {
            get => _washingDate;
            set => Set(ref _washingDate, value);
        }
        public List<UnitViewModel> KgTypes
        {
            get => _kgTypes;
            set => Set(ref _kgTypes, value);
        }
        public List<UnitViewModel> StaffShifts
        {
            get => _staffShifts;
            set => Set(ref _staffShifts, value);
        }
        public RelayCommand ApplyCommand { get; }
        public RelayCommand CloseCommand { get; }


        public async Task InitializeAsync(int shiftId, int kgTypeId, DateTime date)
        {
            _dialogService.ShowBusy();
            try
            {
                StaffShifts = EnumExtentions.GetValues<StaffShiftEnum>();
                KgTypes = EnumExtentions.GetValues<KgTypeEnum>();

                OldKyTypeId = _oldKyTypeId;
                OldShiftId = shiftId;
                OldWashingDate = date;

                WashingDate = date;
                SelectedShiftId = shiftId;
                SelectedKgTypeId = kgTypeId;
            }

            catch (Exception ex)
            {
                _dialogService.HideBusy();
                Helper.RunInMainThread(() => _dialogService.ShowErrorDialog($"Initialization error: {ex.Message}"));
            }

            finally
            {
                _dialogService.HideBusy();
            }
        }

        public ChangeDetailViewModel(IDialogService dialog)
        {
            _dialogService = dialog ?? throw new ArgumentNullException(nameof(dialog));

            ApplyCommand = new RelayCommand(Apply);
            CloseCommand = new RelayCommand(Close);
        }

        public void Close()
        {
            if (_dialogService.ShowQuestionDialog($"Do you want to close window ? "))
                CloseAction?.Invoke(false);
        }

        public void Apply()
        {
            CloseAction?.Invoke(_dialogService.ShowQuestionDialog($"Do you want to change kg details? "));
        }


        public DateTime? GetChangedDate()
        {
            if(!Equals(OldWashingDate, WashingDate))
            {
                return WashingDate;
            }
            return null;
        }

        public int? GetChangedShift()
        {
            if (!Equals(OldShiftId, SelectedShiftId))
            {
                return SelectedShiftId;
            }
            return null;
        }

        public int? GetChangedKgType()
        {
            if (!Equals(OldKyTypeId, SelectedKgTypeId))
            {
                return SelectedKgTypeId;
            }
            return null;
        }
    }
}
