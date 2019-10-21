using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.ViewModels.Common.Window;

namespace PALMS.Notes.ViewModel.Window
{
    public class PrintCopyNumberViewModel: ViewModelBase, IWindowDialogViewModel
    {
        private int _selectedCopyNumber;
        private string _noteType;
        public Action<bool> CloseAction { get; set; }

        public string NoteType
        {
            get => _noteType;
            set => Set(ref _noteType, value);
        }
        public int SelectedCopyNumber
        {
            get => _selectedCopyNumber;
            set => Set(ref _selectedCopyNumber, value);
        }
        public RelayCommand<object> PrintCopyCommand { get; }
        public RelayCommand CloseCommand { get; }

        public PrintCopyNumberViewModel()
        {
            SelectedCopyNumber = 0;

            PrintCopyCommand = new RelayCommand<object>(SetPrintCopy);
            CloseCommand = new RelayCommand(Close);
        }

        public async Task InitializeAsync()
        {
        }

        public void Close()
        {
            CloseAction?.Invoke(true);
        }

        public void SetPrintCopy(object param)
        {
            SelectedCopyNumber = int.Parse(param.ToString());

            CloseAction?.Invoke(true);
        }
    }
}
