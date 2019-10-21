using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Notes.ViewModel.EntityViewModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;

namespace PALMS.Notes.ViewModel.Window
{
    public class DepartmentSelectionViewModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly IDialogService _dialogService;
        private ObservableCollection<DepartmentViewModel> _departments;
        private DepartmentViewModel _selectedDepartment;
        public Action<bool> CloseAction { get; set; }
        private bool IsSelected { get; set; }

        public DepartmentViewModel SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(ref _selectedDepartment, value);
        }
        public ObservableCollection<DepartmentViewModel> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand CloseCommand { get; }

        public async Task InitializeAsync(ObservableCollection<DepartmentViewModel> departments)
        {
            Departments = departments.OrderBy(x=> x.Name).ToObservableCollection();

            
            SelectedDepartment = new DepartmentViewModel();
            IsSelected = false;
        }

        public DepartmentSelectionViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            AddCommand = new RelayCommand(Add, () => SelectedDepartment != null);
            CloseCommand = new RelayCommand(Close);
        }

        public void Add()
        {
            if(SelectedDepartment ==null) return;

            if (!_dialogService.ShowQuestionDialog($"Use \"{SelectedDepartment.Name}\" Department ? "))
                return;
            CloseAction?.Invoke(true);
        }

        public void Close()
        {
            if (_dialogService.ShowQuestionDialog("Do you want to Save changes and close window ? "))
            {
                CloseAction?.Invoke(IsSelected);
            }
        }

        public DepartmentViewModel GetSelectedDepartment()
        {
            return SelectedDepartment;
        }
    }
}
