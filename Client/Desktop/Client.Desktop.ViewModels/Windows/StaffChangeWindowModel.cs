using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Common.Windows;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Windows
{
    public class StaffChangeWindowModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;
        public Action<bool> CloseAction { get; set; }

        private List<ClientEntity> _clients;
        private ClientEntity _selectedClient;
        private List<DepartmentEntity> _departments;
        private DepartmentEntity _selectedDepartment;
        private StaffEntityViewModel _selectedStaff;
        private bool _hadChanges;

        public bool HadChanges
        {
            get => _hadChanges;
            set => Set(() => HadChanges, ref _hadChanges, value);
        }
        public StaffEntityViewModel SelectedStaff
        {
            get => _selectedStaff;
            set => Set(() => SelectedStaff, ref _selectedStaff, value);
        }
        public DepartmentEntity SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(() => SelectedDepartment, ref _selectedDepartment, value);
        }
        public List<DepartmentEntity> Departments
        {
            get => _departments;
            set => Set(() => Departments, ref _departments, value);
        }
        public ClientEntity SelectedClient
        {
            get => _selectedClient;
            set => Set(() => SelectedClient, ref _selectedClient, value);
        }
        public List<ClientEntity> Clients
        {
            get => _clients;
            set => Set(() => Clients, ref _clients, value);
        }

        public List<DepartmentEntity> SortedDepartments =>
            Departments?.Where(x => x?.ClientId == SelectedClient?.Id).ToList();

        public RelayCommand SaveCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand InitializeCommand { get; }

        public StaffChangeWindowModel(ILaundryService laundryService, IDialogService dialogService)
        {
            _laundryService = laundryService ?? throw new ArgumentNullException(nameof(laundryService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            SaveCommand = new RelayCommand(Save, SaveCommandCanExecute);
            CloseCommand = new RelayCommand(Close);
            DeleteCommand = new RelayCommand(Delete);

            InitializeCommand = new RelayCommand(Initialize);

            HadChanges = false;
            PropertyChanged += OnPropertyChanged;
        }

        public void SetSelectedStaff(StaffEntityViewModel staff)
        {
            SelectedStaff = staff;
        }

        private async void Initialize()
        {
            _dialogService.ShowBusy();

            try
            {
                var client = await _laundryService.GetAllAsync<ClientEntity>();
                Clients = client.ToList();

                var departments = await _laundryService.GetAllAsync<DepartmentEntity>();
                Departments = departments.ToList();


                var department = Departments?.FirstOrDefault(x => x?.Id == SelectedStaff?.DepartmentId);

                SelectedClient = Clients?.FirstOrDefault(x => x?.Id == department?.ClientId);
                SelectedDepartment = SortedDepartments?.FirstOrDefault(x => x?.Id == department?.Id);

            }
            catch (Exception e)
            {
                _dialogService.HideBusy();
            }
            finally
            {
                _dialogService.HideBusy();
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                RaisePropertyChanged(() => SortedDepartments);
            }

        }

        private async void Save()
        {
            if(!_dialogService.ShowQuestionDialog($"Do you want to SAVE {SelectedStaff.StaffName} changes ?"))
                return;

            SelectedStaff.AcceptChanges();

            await _laundryService.AddOrUpdateAsync(SelectedStaff.OriginalObject);

            HadChanges = true;
        }

        private bool SaveCommandCanExecute()
        {
            return SelectedStaff.HasChanges();
        }

        private async void Delete()
        {
            if (!_dialogService.ShowQuestionDialog($"Do you want to DELETE {SelectedStaff.StaffName} ?"))
                return;

            await _laundryService.DeleteAsync(SelectedStaff.OriginalObject);

            HadChanges = true;
        }

        private void Close()
        {
            CloseAction?.Invoke(HadChanges);
        }
    }
}
