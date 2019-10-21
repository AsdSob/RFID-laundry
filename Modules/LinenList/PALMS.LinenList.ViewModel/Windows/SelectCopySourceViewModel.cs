using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;

namespace PALMS.LinenList.ViewModel
{
    public class SelectCopySourceViewModel: ViewModelBase, IWindowDialogViewModel
    {
        private readonly IDialogService _dialogService;
        private List<Department> _departments;
        private ObservableCollection<LinenListModelViewModel> _fullLinenList;
        private Department _selectedDepartment;
        private ObservableCollection<LinenListModelViewModel> _newLinenList;
        private ObservableCollection<MasterLinensViewModel> _masterLinens;
        private List<Client> _clients;
        private Client _selectedClient;

        public Client SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }
        public List<Client> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }
        public ObservableCollection<MasterLinensViewModel> MasterLinens
        {
            get => _masterLinens;
            set => Set(ref _masterLinens, value);
        }
        public ObservableCollection<LinenListModelViewModel> NewLinenList
        {
            get => _newLinenList;
            set => Set(ref _newLinenList, value);
        }

        public Department SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(ref _selectedDepartment, value);
        }
        public ObservableCollection<LinenListModelViewModel> FullLinenList
        {
            get => _fullLinenList;
            set => Set(ref _fullLinenList, value);
        }
        public List<Department> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }

        public ObservableCollection<LinenListModelViewModel> SortedLinenList => SortLinenList();
        public List<Department> SortedDepartments => SortDepartments();


        public Action<bool> CloseAction { get; set; }

        public RelayCommand SaveCommand { get; }

        public SelectCopySourceViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            SaveCommand = new RelayCommand(Save);

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                RaisePropertyChanged(() => SortedDepartments);
            }
            if (e.PropertyName == nameof(SelectedDepartment))
            {
                RaisePropertyChanged(() => SortedLinenList);
            }
        }

        public async Task InitializeAsync(List<Client> clients, List<Department> departments,
            ObservableCollection<LinenListModelViewModel> linenList, ObservableCollection<MasterLinensViewModel> masterLinens)
        {
            Clients = clients;
            Departments = departments;
            FullLinenList = linenList;
            MasterLinens = masterLinens;
        }

        private ObservableCollection<LinenListModelViewModel> SortLinenList()
        {
            if (SelectedDepartment == null) return null;

            return FullLinenList.Where(x => x.DepartmentId == SelectedDepartment.Id).ToObservableCollection();
        }

        private List<Department> SortDepartments()
        {
            if (SelectedClient == null) return null;

            return Departments.Where(x => x.ClientId == SelectedClient?.Id).ToList();
        }

        private void Save()
        {
            if (!_dialogService.ShowQuestionDialog($"Do you want to copy {SelectedDepartment.Name} Linens? "))
                return;

            NewLinenList = SortedLinenList;

            CloseAction?.Invoke(true);
        }

        public List<int> GetMasterLinens()
        {
            var masterLinens = new List<int>();

            foreach (var linenList in NewLinenList)
            {
                masterLinens.Add(linenList.MasterLinenId);
            }
            return masterLinens;
        }
    }
}
