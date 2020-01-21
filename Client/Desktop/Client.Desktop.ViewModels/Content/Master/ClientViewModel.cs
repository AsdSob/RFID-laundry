using System;
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
using Storage.Laundry.Models;
using Storage.Laundry.Models.Abstract;

namespace Client.Desktop.ViewModels.Content.Master
{
    public class ClientViewModel : ViewModelBase
    {
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;
        private readonly IResolver _resolverService;

        #region Parameters

        private ObservableCollection<ClientEntityViewModel> _clients;
        private ClientEntityViewModel _selectedClient;
        private ObservableCollection<DepartmentEntityViewModel> _departments;
        private List<UnitViewModel> _cities;
        private DepartmentEntityViewModel _selectedDepartment;

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
        public ClientEntityViewModel SelectedClient
        {
            get => _selectedClient;
            set => Set(() => SelectedClient, ref _selectedClient, value);
        }
        public ObservableCollection<ClientEntityViewModel> Clients
        {
            get => _clients;
            set => Set(() => Clients, ref _clients, value);
        }

        public ObservableCollection<DepartmentEntityViewModel> SortedDepartments => SortDepartments();

        public ObservableCollection<ClientEntityViewModel> SortedParentClients =>
           Clients?.Where(x=> x.ParentId == 0 || x.ParentId == null).ToObservableCollection();

        #endregion

        public RelayCommand AddClientCommand { get; }
        public RelayCommand DeleteClientCommand { get; }
        public RelayCommand SaveCommand { get; }

        public RelayCommand AddDepartmentCommand { get; }
        public RelayCommand DeleteDepartmentCommand { get; }


        public ClientViewModel(ILaundryService dataService, IDialogService dialogService, IResolver resolver)
        {
            _laundryService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));

            AddClientCommand = new RelayCommand(AddNewClient);
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
            var clients = client.Select(x => new ClientEntityViewModel(x));
            Clients = clients.ToObservableCollection();

            var department = await _laundryService.GetAllAsync<DepartmentEntity>();
            var departments = department.Select(x => new DepartmentEntityViewModel(x));
            Departments = departments.ToObservableCollection();

            Cities = EnumExtensions.GetValues<CitiesEnum>();
        }


        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {

                AddDepartmentCommand.RaiseCanExecuteChanged();
                DeleteClientCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged((() =>  SortedDepartments));
                RaisePropertyChanged((() =>  SortedParentClients));
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
                departments.AddRange(Departments.Where(x=> x.OriginalObject.ClientEntity == SelectedClient.OriginalObject));
            }
            else
            {
                departments.AddRange(Departments.Where(x=> x.ClientId == SelectedClient.Id));
            }

            return departments;
        }

        private void AddNewClient()
        {
            if(!_dialogService.ShowQuestionDialog("Do you want add new client?")) 
                return;

            var newClient = new ClientEntityViewModel()
            {
                Active = true,
                CityId = (int) CitiesEnum.AbuDhabi,
            };

            Clients.Add(newClient);
            SelectedClient = newClient;
        }

        private void DeleteClient()
        {
            if(SelectedClient == null) return;
            if(!_dialogService.ShowQuestionDialog($"Do you want to DELETE {SelectedClient.Name} ?"))
                return;

            var client = SelectedClient;
            SelectedClient = null;

            _laundryService.Delete(client.OriginalObject);

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
                newDepartment.OriginalObject.ClientEntity = SelectedClient.OriginalObject;
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

            _laundryService.Delete(department.OriginalObject);

            Departments.Remove(department);
        }

        private void Save()
        {
            ArrayList entities = new ArrayList();

            var clients = Clients.Where(x => x.HasChanges()).ToList();
            var departments = Departments.Where(x => x.HasChanges()).ToList();

            clients?.ForEach(x=> x.AcceptChanges());
            departments?.ForEach(x => x.AcceptChanges());

            //entities.Add(clients.Select(x=> x.OriginalObject));
            //entities.Add(departments.Select(x=> x.OriginalObject));

            foreach (var client in clients)
            {
                _laundryService.AddOrUpdate(client.OriginalObject);
            }
            foreach (var client in departments)
            {
                _laundryService.AddOrUpdate(client.OriginalObject);
            }

            //_laundryService.AddOrUpdate(entities);
            _dialogService.ShowInfoDialog("All changes saved");
        }

    }
}
