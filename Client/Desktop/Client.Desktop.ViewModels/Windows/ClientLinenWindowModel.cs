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
        private readonly IMainDispatcher _dispatcher;
        private ObservableCollection<ClientLinenEntityViewModel> _clientLinens;
        private ClientLinenEntityViewModel _selectedLinen;
        private ObservableCollection<MasterLinenEntityViewModel> _masterLinens;
        private ObservableCollection<ClientEntityViewModel> _clients;
        private ObservableCollection<DepartmentEntityViewModel> _departments;
        private ObservableCollection<ClientStaffEntityViewModel> _staffs;

        public Action<bool> CloseAction { get; set; }

        public ObservableCollection<ClientStaffEntityViewModel> Staffs
        {
            get => _staffs;
            set => Set(ref _staffs, value);
        }
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
        public ObservableCollection<ClientStaffEntityViewModel> SortedStaffs => SortStaffs();

        public RelayCommand SaveCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand NewCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand InitializeCommand { get; }


        public ClientLinenWindowModel(ILaundryService laundryService, IDialogService dialogService, IMainDispatcher dispatcher)
        {
            _laundryService = laundryService ?? throw new ArgumentNullException(nameof(laundryService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(Close);
            NewCommand = new RelayCommand(NewLinen);
            DeleteCommand = new RelayCommand(Delete);
            //InitializeCommand = new RelayCommand(Initialize);

            //PropertyChanged += OnPropertyChanged;
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

                var staff = await _laundryService.GetAllAsync<ClientStaffEntity>();
                var staffs = staff.Select(x => new ClientStaffEntityViewModel(x));
                Staffs = staffs.ToObservableCollection();

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

            if (e.PropertyName == nameof(SelectedLinen.DepartmentId))
            {
                RaisePropertyChanged(() => SortedStaffs);
            }

            if (e.PropertyName == nameof(SelectedLinen.MasterLinenId))
            {
                SelectedLinen.PackingValue = MasterLinens.FirstOrDefault(x => x.Id == SelectedLinen.MasterLinenId).PackingValue;
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == nameof(SelectedDepartment))
            //{
            //    RaisePropertyChanged(() => SortedStaffs);
            //}
        }

        private ObservableCollection<DepartmentEntityViewModel> SortDepartments()
        {
            var departments = new ObservableCollection<DepartmentEntityViewModel>();

            if (SelectedLinen.ClientId == 0)
            {
                return departments;
            }

            departments = Departments?.Where(x => x.ClientId == SelectedLinen.ClientId).ToObservableCollection();

            if (departments != null && departments.All(x => x.Id != SelectedLinen.DepartmentId))
            {
                SelectedLinen.DepartmentId = 0;
            }
            return departments;
        }

        private ObservableCollection<ClientStaffEntityViewModel> SortStaffs()
        {
            var staffs = new ObservableCollection<ClientStaffEntityViewModel>();

            if (SelectedLinen.DepartmentId == 0)
            {
                return staffs;
            }
            
            staffs = Staffs?.Where(x => x.DepartmentId == SelectedLinen.DepartmentId).ToObservableCollection();

            if (staffs != null && staffs.All(x => x.Id != SelectedLinen.StaffId))
            {
                SelectedLinen.StaffId = null;
            }
            return staffs;
        }

        public void SetSelectedLinen(ClientLinenEntityViewModel linen)
        {
            SelectedLinen = linen;

            SelectedLinen.PropertyChanged += ItemOnPropertyChanged;
        }

        private void NewLinen()
        {
            var linen = new ClientLinenEntityViewModel()
            {
                DepartmentId = SelectedLinen.DepartmentId,
                ClientId = SelectedLinen.ClientId,
                StaffId = SelectedLinen.StaffId,
                Tag = SelectedLinen.Tag,
                MasterLinenId = 0,
            };

            SelectedLinen = linen;

            SelectedLinen.PropertyChanged += ItemOnPropertyChanged;
        }

        private void Save()
        {
            if (!SelectedLinen.IsValid)
            {
                return;
            }

            if (!SelectedLinen.HasChanges())
            {
                return;
            }

            SelectedLinen.AcceptChanges();

            _laundryService.AddOrUpdateAsync(SelectedLinen.OriginalObject);

            if (_dialogService.ShowQuestionDialog("Saved! \n Do you want to close window ? "))
            {
                Close();
            }
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
