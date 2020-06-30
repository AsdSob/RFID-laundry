using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Common.Windows;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Windows
{
    public class ClientLinenWindowModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;
        private ObservableCollection<ClientLinenEntityViewModel> _clientLinens;
        private ClientLinenEntityViewModel _selectedLinen;
        private ObservableCollection<MasterLinenEntityViewModel> _masterLinens;
        private ObservableCollection<ClientEntityViewModel> _clients;
        private ObservableCollection<DepartmentEntityViewModel> _departments;
        private DepartmentEntityViewModel _selectedDepartment;

        public DepartmentEntityViewModel SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(ref _selectedDepartment, value);
        }
        public Action<bool> CloseAction { get; set; }

        public ObservableCollection<DepartmentEntityViewModel> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }
        public ObservableCollection<ClientEntityViewModel> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }
        public ObservableCollection<MasterLinenEntityViewModel> MasterLinens
        {
            get => _masterLinens;
            set => Set(ref _masterLinens, value);
        }
        public ClientLinenEntityViewModel SelectedLinen
        {
            get => _selectedLinen;
            set => Set(ref _selectedLinen, value);
        }
        public ObservableCollection<ClientLinenEntityViewModel> ClientLinens
        {
            get => _clientLinens;
            set => Set(ref _clientLinens, value);
        }

        public ObservableCollection<DepartmentEntityViewModel> SortedDepartments => SortDepartments();
        public ObservableCollection<DepartmentEntityViewModel> SortedStaffs => SortStaffs();

        public RelayCommand SaveCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand NewCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand InitializeCommand { get; }


        public ClientLinenWindowModel(ILaundryService laundryService, IDialogService dialogService)
        {
            _laundryService = laundryService ?? throw new ArgumentNullException(nameof(laundryService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(Close);
            NewCommand = new RelayCommand(NewLinen);
            DeleteCommand = new RelayCommand(Delete);
            //InitializeCommand = new RelayCommand(Initialize);

            PropertyChanged += OnPropertyChanged;
        }

        public async void Initialize()
        {
            _dialogService.ShowBusy();

            try
            {
                var client = await _laundryService.GetAllAsync<ClientEntity>();
                var clients = client.Select(x => new ClientEntityViewModel(x));
                Clients = clients.ToObservableCollection();

                var department = await _laundryService.GetAllAsync<DepartmentEntity>();
                var departments = department.Select(x => new DepartmentEntityViewModel(x));
                Departments = departments.ToObservableCollection();

                var masterLinen = await _laundryService.GetAllAsync<MasterLinenEntity>();
                var masterLinens = masterLinen.Select(x => new MasterLinenEntityViewModel(x));
                MasterLinens = masterLinens.ToObservableCollection();

                var linen = await _laundryService.GetAllAsync<ClientLinenEntity>();
                var linens = linen.Select(x => new ClientLinenEntityViewModel(x));
                ClientLinens = linens.ToObservableCollection();

                RaisePropertyChanged(() => SortedDepartments);
                RaisePropertyChanged(() => SortedStaffs);
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

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is ClientLinenEntityViewModel item)) return;

            if (e.PropertyName == nameof(SelectedLinen.ClientId))
            {
                RaisePropertyChanged(() => SortedDepartments);
                RaisePropertyChanged(() => SortedStaffs);
            }

            if (e.PropertyName == nameof(SelectedLinen.MasterLinenId))
            {
                SelectedLinen.PackingValue = MasterLinens.FirstOrDefault(x => x.Id == SelectedLinen.MasterLinenId).PackingValue;
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedDepartment))
            {
                RaisePropertyChanged(() => SortedStaffs);
            }
        }

        private ObservableCollection<DepartmentEntityViewModel> SortDepartments()
        {
            var departments = new ObservableCollection<DepartmentEntityViewModel>();

            if (SelectedLinen.ClientId == 0)
            {
                return departments;
            }

            departments = Departments?.Where(x => x.ClientId == SelectedLinen.ClientId && x.ParentId == null).ToObservableCollection();

            return departments;
        }

        private ObservableCollection<DepartmentEntityViewModel> SortStaffs()
        {
            var staffs = Departments?.Where(x => x.ParentId == SelectedDepartment?.Id).ToObservableCollection();

            return staffs;
        }

        public void SetSelectedLinen(ClientLinenEntityViewModel linen)
        {
            if (linen == null)
            {
                NewLinen();
                return;
            }

            SelectedLinen = linen;

            var department = Departments.FirstOrDefault(x => x.Id == linen.DepartmentId);

            if (department.ParentId.HasValue)
            {
                SelectedDepartment = Departments.FirstOrDefault(x => x.Id == department.ParentId);
            }
            else
            {
                SelectedDepartment = department;
            }

            SelectedLinen.PropertyChanged += ItemOnPropertyChanged;
        }

        private void NewLinen()
        {
            var linen = new ClientLinenEntityViewModel();

            SelectedLinen = linen;

            SelectedLinen.PropertyChanged += ItemOnPropertyChanged;
        }

        private void Save()
        {
            if(SelectedLinen.DepartmentId == 0)
            {
                SelectedLinen.DepartmentId = SelectedDepartment.Id;
            }

            if (!SelectedLinen.IsValid || !SelectedLinen.HasChanges())
            {
                return;
            }

            SelectedLinen.AcceptChanges();
            _laundryService.AddOrUpdateAsync(SelectedLinen.OriginalObject);

            Close();
        }

        private void Delete()
        {
            var masterLinen = MasterLinens.FirstOrDefault(x => x.Id == SelectedLinen.MasterLinenId);

            if (!_dialogService.ShowQuestionDialog($"Do you want to DELETE {masterLinen?.Name} ?"))
                return;

            if (!SelectedLinen.IsNew)
            {
                _laundryService.DeleteAsync(SelectedLinen.OriginalObject);
            }

            Close();
        }

        private void Close()
        {
            if (SelectedLinen.HasChanges())
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
