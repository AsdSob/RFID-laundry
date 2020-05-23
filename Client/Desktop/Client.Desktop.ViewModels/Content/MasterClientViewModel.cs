﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Windows;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Content
{
    public class MasterClientViewModel : ViewModelBase
    {
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;
        private readonly IResolver _resolverService;

        #region Parameters

        private ObservableCollection<ClientEntity> _clients;
        private ClientEntity _selectedClient;
        private ObservableCollection<DepartmentEntityViewModel> _departments;
        private List<UnitViewModel> _cities;
        private DepartmentEntityViewModel _selectedDepartment;
        private List<UnitViewModel> _departmentTypes;

        public List<UnitViewModel> DepartmentTypes
        {
            get => _departmentTypes;
            set => Set(() => DepartmentTypes, ref _departmentTypes, value);
        }
        public DepartmentEntityViewModel SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(() => SelectedDepartment, ref _selectedDepartment, value);
        }
        public List<UnitViewModel> Cities
        {
            get => _cities;
            set => Set(() => Cities, ref _cities, value);
        }
        public ObservableCollection<DepartmentEntityViewModel> Departments
        {
            get => _departments;
            set => Set(() => Departments, ref _departments, value);
        }
        public ClientEntity SelectedClient
        {
            get => _selectedClient;
            set => Set(() => SelectedClient, ref _selectedClient, value);
        }
        public ObservableCollection<ClientEntity> Clients
        {
            get => _clients;
            set => Set(() => Clients, ref _clients, value);
        }

        public ObservableCollection<DepartmentEntityViewModel> SortedDepartments => SortDepartments();

        #endregion

        public RelayCommand AddClientCommand { get; }
        public RelayCommand EditClientCommand { get; }
        public RelayCommand DeleteClientCommand { get; }
        public RelayCommand SaveCommand { get; }

        public RelayCommand AddDepartmentCommand { get; }
        public RelayCommand DeleteDepartmentCommand { get; }


        public MasterClientViewModel(ILaundryService dataService, IDialogService dialogService, IResolver resolver)
        {
            _laundryService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));

            AddClientCommand = new RelayCommand(AddNewClient);
            EditClientCommand = new RelayCommand(EditClient, (() => SelectedClient != null));
            DeleteClientCommand = new RelayCommand(DeleteClient, () => SelectedClient !=null);
            SaveCommand = new RelayCommand(Save);

            AddDepartmentCommand = new RelayCommand(AddNewDepartment, () => SelectedClient != null);
            DeleteDepartmentCommand = new RelayCommand(DeleteDepartment, (() => SelectedDepartment != null));

            GetData();
            PropertyChanged += OnPropertyChanged;

        }

        private async Task GetData()
        {
            var client = await _laundryService.GetAllAsync<ClientEntity>();
            Clients = client.ToObservableCollection();

            var department = await _laundryService.GetAllAsync<DepartmentEntity>();
            var departments = department.Select(x => new DepartmentEntityViewModel(x));
            Departments = departments.ToObservableCollection();

            Cities = EnumExtensions.GetValues<CitiesEnum>();
            DepartmentTypes = EnumExtensions.GetValues<DepartmentTypeEnum>();
        }


        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {

                AddDepartmentCommand.RaiseCanExecuteChanged();
                DeleteClientCommand.RaiseCanExecuteChanged();
                EditClientCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged((() =>  SortedDepartments));
            }

            if (e.PropertyName == nameof(SelectedDepartment))
            {
                DeleteDepartmentCommand.RaiseCanExecuteChanged();
            }

        }

        private ObservableCollection<DepartmentEntityViewModel> SortDepartments()
        {
            var departments = new ObservableCollection<DepartmentEntityViewModel>();
            if (SelectedClient == null) return departments;

            if (SelectedClient.IsNew)
            {
                //departments.AddRange(Departments.Where(x=> x.OriginalObject.ClientEntity == SelectedClient.OriginalObject));
            }
            else
            {
                departments.AddRange(Departments.Where(x=> x.ClientId == SelectedClient.Id));
            }

            return departments;
        }

        private void EditClient()
        {
            ClientWindow(new ClientEntityViewModel(SelectedClient));
        }

        private void AddNewClient()
        {
           ClientWindow(null);
        }

        private async void ClientWindow(ClientEntityViewModel client)
        {
            var clientWindow = _resolverService.Resolve<MasterClientWindowModel>();

            clientWindow.SetSelectedClient(client);

            if (_dialogService.ShowDialog(clientWindow))
            {
                Clients.Clear();

                await GetData();
            }
        }

        private void DeleteClient()
        {
            if(SelectedClient == null) return;
            if(!_dialogService.ShowQuestionDialog($"Do you want to DELETE {SelectedClient.Name} ?"))
                return;

            var client = SelectedClient;
            SelectedClient = null;

            _laundryService.DeleteAsync(client);

            Clients.Remove(client);
        }

        private void AddNewDepartment()
        {
            if(SelectedClient == null) return;
            if (!_dialogService.ShowQuestionDialog("Do you want add new department?"))
                return;

            var newDepartment = new DepartmentEntityViewModel();

            if (SelectedClient.IsNew)
            {
                //newDepartment.OriginalObject.ClientEntity = SelectedClient.OriginalObject;
            }
            else
            {
                newDepartment.ClientId = SelectedClient.Id;
            }

            Departments.Add(newDepartment);
            SelectedDepartment = newDepartment;
            RaisePropertyChanged(() => SortedDepartments);
        }

        private void DeleteDepartment()
        {
            if (SelectedDepartment == null) return;
            if (!_dialogService.ShowQuestionDialog($"Do you want to DELETE {SelectedDepartment.Name} ?"))
                return;

            var department = SelectedDepartment;
            SelectedDepartment = null;

            _laundryService.DeleteAsync(department.OriginalObject);

            Departments.Remove(department);
        }

        private void Save()
        {
            ArrayList entities = new ArrayList();

            var departments = Departments.Where(x => x.HasChanges()).ToList();

            departments?.ForEach(x => x.AcceptChanges());

            foreach (var department in departments)
            {
                _laundryService.AddOrUpdateAsync(department.OriginalObject);
            }

            _dialogService.ShowInfoDialog("All changes saved");
        }

    }
}
