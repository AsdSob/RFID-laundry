using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Common.Windows;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Windows
{
    public class MasterDepartmentWindowModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;
        private readonly IMainDispatcher _dispatcher;
        private List<DepartmentEntity> _departments;
        private List<UnitViewModel> _departmentTypes;
        private DepartmentEntityViewModel _selectedDepartment;
        private ClientEntity _selectedClient;

        public ClientEntity SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }
        public DepartmentEntityViewModel SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(ref _selectedDepartment, value);
        }
        public List<UnitViewModel> DepartmentTypes
        {
            get => _departmentTypes;
            set => Set(ref _departmentTypes, value);
        }

        public List<DepartmentEntity> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }

        public Action<bool> CloseAction { get; set; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand InitializeCommand { get; }

        public MasterDepartmentWindowModel(ILaundryService laundryService, IDialogService dialogService, IMainDispatcher dispatcher)
        {
            _laundryService = laundryService ?? throw new ArgumentNullException(nameof(laundryService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(Close);
            DeleteCommand = new RelayCommand(Delete);
            InitializeCommand = new RelayCommand(Initialize);

            DepartmentTypes = EnumExtensions.GetValues<DepartmentTypeEnum>();
        }

        public void SetSelectedDepartment(DepartmentEntity department, ClientEntity selectedClient)
        {
            SelectedDepartment = null;
            SelectedClient = selectedClient;

            if (department != null)
            {
                SelectedDepartment = new DepartmentEntityViewModel(department);
                return;
            }

            SelectedDepartment = new DepartmentEntityViewModel(new DepartmentEntity()
            {
                ClientId = selectedClient.Id,
                DepartmentTypeId = 1,
            });
        }

        private async void Initialize()
        {
            _dialogService.ShowBusy();

            try
            {
                var department = await _laundryService.GetAllAsync<DepartmentEntity>();
                var departments = department.Where(x => x.ClientId == SelectedClient.Id);
                Departments = departments.ToList();

            }
            catch (Exception e)
            {
                _dialogService.HideBusy();
            }
            finally
            {
                _dialogService.HideBusy();
            }

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedDepartment))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private void Save()
        {
            if (!SelectedDepartment.IsValid || !SelectedDepartment.HasChanges())
            {
                return;
            }

            SelectedDepartment.AcceptChanges();
            _laundryService.AddOrUpdateAsync(SelectedDepartment.OriginalObject);

            Close();
        }

        private void Delete()
        {
            if (!_dialogService.ShowQuestionDialog($"Do you want to DELETE {SelectedDepartment.Name} ?"))
                return;

            if (!SelectedDepartment.IsNew)
            {
                _laundryService.DeleteAsync(SelectedDepartment.OriginalObject);
            }

            Close();
        }

        private void Close()
        {
            if (SelectedDepartment.HasChanges())
            {
                if (_dialogService.ShowQuestionDialog($"Do you want to close window ? \n \"Changes is NOT saved\""))
                {
                    CloseAction?.Invoke(false);
                    return;
                }
            }

            CloseAction?.Invoke(true);

        }


    }

}
