using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;

namespace PALMS.NoteHistory.ViewModel.Windows
{
    public class ClientDepSelectViewModel : ViewModelBase, IWindowDialogViewModel
    {
        public Action<bool> CloseAction { get; set; }
        private readonly IDataService _dataService;
        private readonly IDispatcher _dispatcher;
        private readonly IDialogService _dialogService;
        private List<Client> _clients;
        private List<Department> _departments;
        private Client _selectedClient;
        private Department _selectedDepartment;

        public Department SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(ref _selectedDepartment, value);
        }
        public Client SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }
        public List<Department> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }
        public List<Client> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }

        public List<Department> SortedDepartments => Departments?.Where(x => x.ClientId == SelectedClient?.Id).ToList();

        public RelayCommand SelectDepartCommand { get; }
        public RelayCommand CloseCommand { get; }

        public async Task InitializeAsync()
        {
            var client = await _dataService.GetAsync<Client>();
            _dispatcher.RunInMainThread(() => Clients = client.OrderBy(x=> x.ShortName).ToList());

            var department = await _dataService.GetAsync<Department>();
            _dispatcher.RunInMainThread(() => Departments = department.ToList());
        }

        public ClientDepSelectViewModel(IDataService dataService, IDispatcher dispatcher, IDialogService dialogService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _dataService = dataService ?? throw new ArgumentException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentException(nameof(dispatcher));

            SelectDepartCommand = new RelayCommand(SetDepartment);
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                RaisePropertyChanged(()=> SortedDepartments);
            }
        }

        public void SetDepartment()
        {
            if (!_dialogService.ShowQuestionDialog("Do you want to move Note to ==> \n" + SelectedClient.ShortName +
                                                   ", " + SelectedDepartment.Name)) return;

            Close();
        }

        public Department GetSelectedDepartment()
        {
            return SelectedDepartment;
        }

        public void Close()
        {
            CloseAction?.Invoke(true);
        }
    }
}
