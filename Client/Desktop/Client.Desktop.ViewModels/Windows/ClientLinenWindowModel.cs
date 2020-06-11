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
    public class ClientLinenWindowModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;
        private readonly IMainDispatcher _dispatcher;
        private List<ClientLinenEntityViewModel> _clientLinens;
        private ClientLinenEntityViewModel _selectedLinen;
        private List<MasterLinenEntity> _masterLinens;
        private List<ClientEntity> _clients;
        private List<DepartmentEntity> _departments;
        private List<ClientStaffEntity> _staffs;
        private bool _hasChanges;


        public Action<bool> CloseAction { get; set; }

        public bool HasChanges
        {
            get => _hasChanges;
            set => Set(ref _hasChanges, value);
        }
        public List<ClientStaffEntity> Staffs
        {
            get => _staffs;
            set => Set(ref _staffs, value);
        }
        public List<DepartmentEntity> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }
        public List<ClientEntity> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }
        public List<MasterLinenEntity> MasterLinens
        {
            get => _masterLinens;
            set => Set(ref _masterLinens, value);
        }
        public ClientLinenEntityViewModel SelectedLinen
        {
            get => _selectedLinen;
            set => Set(ref _selectedLinen, value);
        }
        public List<ClientLinenEntityViewModel> ClientLinens
        {
            get => _clientLinens;
            set => Set(ref _clientLinens, value);
        }

        public List<DepartmentEntity> SortedDepartments => SortDepartments();
        public List<ClientStaffEntity> SortedStaffs => SortStaffs();

        public RelayCommand SaveCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand NewCommand { get; }
        public RelayCommand ClearSelectedStaffCommand { get; }
        public RelayCommand InitializeCommand { get; }


        public ClientLinenWindowModel(ILaundryService laundryService, IDialogService dialogService, IMainDispatcher dispatcher)
        {
            _laundryService = laundryService ?? throw new ArgumentNullException(nameof(laundryService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(Close);
            NewCommand = new RelayCommand(NewLinen);
            ClearSelectedStaffCommand = new RelayCommand(ClearSelectedStaff);
            //InitializeCommand = new RelayCommand(Initialize);

            PropertyChanged += OnPropertyChanged;
        }

        public async void Initialize()
        {
            _dialogService.ShowBusy();

            try
            {
                var clients = await _laundryService.GetAllAsync<ClientEntity>();
                Clients = clients.ToList();

                var departments = await _laundryService.GetAllAsync<DepartmentEntity>();
                Departments = departments.ToList();

                var staffs = await _laundryService.GetAllAsync<ClientStaffEntity>();
                Staffs = staffs.ToList();

                var masterLinens = await _laundryService.GetAllAsync<MasterLinenEntity>();
                MasterLinens = masterLinens.ToList();

                var linen = await _laundryService.GetAllAsync<ClientLinenEntity>();
                var linens = linen.Select(x => new ClientLinenEntityViewModel(x));
                ClientLinens = linens.ToList();

                RaisePropertyChanged(() => SortedDepartments);
                RaisePropertyChanged(() => SortedStaffs);

                HasChanges = false;
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

        private List<DepartmentEntity> SortDepartments()
        {
            var departments = new List<DepartmentEntity>();

            if (SelectedLinen.ClientId == 0)
            {
                return departments;
            }

            departments = Departments?.Where(x => x.ClientId == SelectedLinen.ClientId).ToList();

            if (departments != null && departments.All(x => x.Id != SelectedLinen.DepartmentId))
            {
                SelectedLinen.DepartmentId = 0;
            }
            return departments;
        }

        private List<ClientStaffEntity> SortStaffs()
        {
            var staffs = new List<ClientStaffEntity>();

            if (SelectedLinen.DepartmentId == 0)
            {
                return staffs;
            }
            
            staffs = Staffs?.Where(x => x.DepartmentId == SelectedLinen.DepartmentId).ToList();

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

        private void ClearSelectedStaff()
        {
            SelectedLinen.StaffId = null;
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
            HasChanges = true;

            if (_dialogService.ShowQuestionDialog("Saved! \n Do you want to close window ? "))
            {
                CloseWindow();
            }
        }

        private void Close()
        {
            if (SelectedLinen.HasChanges())
            {
                if (_dialogService.ShowQuestionDialog($"Do you want to close window ? \n \"All changes will be canceled\""))
                {
                    CloseWindow();
                }
            }
            else
            {
                CloseWindow();
            }
        }

        private void CloseWindow()
        {
            CloseAction?.Invoke(HasChanges);
        }
    }
}
