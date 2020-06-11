using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Shapes;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Windows;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Content
{
    public class TagRegistrationViewModel :ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly ILaundryService _laundryService;
        private readonly IResolver _resolverService;
        private readonly IMainDispatcher _dispatcher;

        private ObservableCollection<ClientEntity> _clients;
        private ClientEntity _selectedClient;
        private ObservableCollection<DepartmentEntity> _departments;
        private DepartmentEntity _selectedDepartment;
        private ObservableCollection<ClientStaffEntity> _staffs;
        private ClientStaffEntity _selectedStaff;
        private ObservableCollection<ClientLinenEntityViewModel> _linens;
        private ClientLinenEntityViewModel _selectedClientLinen;
        private List<MasterLinenEntity> _masterLinens;
        private RfidTagViewModel _selectedTag;
        private string _addShowButton;

        public string AddShowButton
        {
            get => _addShowButton;
            set => Set(ref _addShowButton, value);
        }
        public RfidTagViewModel SelectedTag
        {
            get => _selectedTag;
            set => Set(() => SelectedTag, ref _selectedTag, value);
        }
        public List<MasterLinenEntity> MasterLinens
        {
            get => _masterLinens;
            set => Set(() => MasterLinens, ref _masterLinens, value);
        }
        public ClientLinenEntityViewModel SelectedClientLinen
        {
            get => _selectedClientLinen;
            set => Set(() => SelectedClientLinen, ref _selectedClientLinen, value);
        }
        public ObservableCollection<ClientLinenEntityViewModel> Linens
        {
            get => _linens;
            set => Set(() => Linens, ref _linens, value);
        }
        public ClientStaffEntity SelectedStaff
        {
            get => _selectedStaff;
            set => Set(() => SelectedStaff, ref _selectedStaff, value);
        }
        public ObservableCollection<ClientStaffEntity> Staffs
        {
            get => _staffs;
            set => Set(() => Staffs, ref _staffs, value);
        }
        public DepartmentEntity SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(() => SelectedDepartment, ref _selectedDepartment, value);
        }
        public ObservableCollection<DepartmentEntity> Departments
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

        public RfidReaderWindowModel RfidReaderWindow { get; set; }
        public string SearchingTag { get; set; }

        public ObservableCollection<DepartmentEntity> SortedDepartments =>
            Departments?.Where(x => x?.ClientId == SelectedClient?.Id).ToObservableCollection();

        public ObservableCollection<ClientStaffEntity> SortedStaff => SortStaff();

        public ObservableCollection<ClientLinenEntityViewModel> SortedLinens => SortLinens();
           

        public RelayCommand NewStaffCommand { get; }
        public RelayCommand EditStaffCommand { get; }
        public RelayCommand DeleteStaffCommand { get; }

        public RelayCommand NewLinenCommand { get; }
        public RelayCommand EditLinenCommand { get; }
        public RelayCommand DeleteLinenCommand { get; }

        public RelayCommand InitializeCommand { get; }
        public RelayCommand ClearSelectedStaffCommand { get; }

        public RelayCommand AddShowLinenByTagCommand { get; }
        public RelayCommand DeleteTagCommand { get; }


        public TagRegistrationViewModel(ILaundryService dataService, IDialogService dialogService, IResolver resolver, IMainDispatcher dispatcher)
        {
            _laundryService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            NewStaffCommand = new RelayCommand(()=> StaffWindow(null),(() => SelectedDepartment !=null));
            EditStaffCommand = new RelayCommand(()=>StaffWindow(SelectedStaff), (() => SelectedStaff !=null));
            DeleteStaffCommand = new RelayCommand(DeleteStaff,(() => SelectedStaff !=null));

            NewLinenCommand = new RelayCommand(() => LinenWindow(null)/*, () => SelectedDepartment !=null*/);
            EditLinenCommand = new RelayCommand(() => LinenWindow(SelectedClientLinen),() => SelectedClientLinen !=null);
            DeleteLinenCommand = new RelayCommand(DeleteLinen,(() => SelectedClientLinen !=null));

            ClearSelectedStaffCommand = new RelayCommand(ClearSelectedStaff);
            AddShowLinenByTagCommand = new RelayCommand(AddShowLinenByTag, () => SelectedTag != null);
            DeleteTagCommand = new RelayCommand(DeleteTag, () => SelectedTag != null || SelectedClientLinen != null);

            InitializeCommand = new RelayCommand(Initialize);
            RfidReaderWindow = _resolverService.Resolve<RfidReaderWindowModel>();

            AddShowButton = "Add";

            RfidReaderWindow.Tags.CollectionChanged += TagsCollectionChanged;

            PropertyChanged += OnPropertyChanged;
        }

        //private void UnSubscribeItem(ClientLinenEntityViewModel item)
        //{
        //    item.PropertyChanged -= ItemOnPropertyChanged;
        //}

        //private void SubscribeItem(ClientLinenEntityViewModel item)
        //{
        //    item.PropertyChanged += ItemOnPropertyChanged;
        //}

        //private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (!(sender is ClientLinenEntityViewModel item)) return;

        //    if (item.HasChanges() && item.IsValid)
        //    {
        //        item.AcceptChanges();
        //        _laundryService.AddOrUpdateAsync(item.OriginalObject);
        //    }
        //}

        private void TagsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var tag in RfidReaderWindow.Tags)
                {
                    if (Linens.Any(x => x.Tag == tag.Tag))
                    {
                        tag.IsRegistered = true;
                    }
                }
            }
        }

        private async void Initialize()
        {
            var client = await _laundryService.GetAllAsync<ClientEntity>();
            Clients = client.ToObservableCollection();

            var departments = await _laundryService.GetAllAsync<DepartmentEntity>();
            Departments = departments.ToObservableCollection();

            var masterLinen = await _laundryService.GetAllAsync<MasterLinenEntity>();
            MasterLinens = masterLinen.ToList();

            GetStaffs();
            GetClientLinens();
        }

        private async void GetStaffs()
        {
            var staff = await _laundryService.GetAllAsync<ClientStaffEntity>();
            Staffs = staff.ToObservableCollection();

            RaisePropertyChanged(() => SortedStaff);
        }

        private async void GetClientLinens()
        {
            var linen = await _laundryService.GetAllAsync<ClientLinenEntity>();
            var linens = linen.Select(x => new ClientLinenEntityViewModel(x));
            Linens = linens.ToObservableCollection();

            RaisePropertyChanged(()=> SortedLinens);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == nameof(SelectedClient))
            //{
            //    RaisePropertyChanged(() => SortedDepartments);
            //}else

            //if (e.PropertyName == nameof(SelectedDepartment))
            //{
            //    NewStaffCommand?.RaiseCanExecuteChanged();
            //    NewLinenCommand?.RaiseCanExecuteChanged();

            //    RaisePropertyChanged(() => SortedLinens);
            //    RaisePropertyChanged(() => SortedStaff);
            //}else

            //if (e.PropertyName == nameof(SelectedStaff))
            //{
            //    EditStaffCommand.RaiseCanExecuteChanged();
            //    DeleteStaffCommand?.RaiseCanExecuteChanged();

            //    RaisePropertyChanged(() => SortedLinens);
            //}else

            if (e.PropertyName == nameof(SelectedClientLinen))
            {
                DeleteLinenCommand?.RaiseCanExecuteChanged();
                EditLinenCommand?.RaiseCanExecuteChanged();
                DeleteTagCommand?.RaiseCanExecuteChanged();
            }
            else

            if (e.PropertyName == nameof(SelectedTag))
            {
                AddShowLinenByTagCommand?.RaiseCanExecuteChanged();
                DeleteTagCommand?.RaiseCanExecuteChanged();
                AddShowButtonName();
            }
        }

        private ObservableCollection<ClientLinenEntityViewModel> SortLinens()
        {
            return SelectedStaff == null ?
                Linens?.Where(x => x.DepartmentId == SelectedDepartment?.Id).ToObservableCollection() :
                Linens?.Where(x => x?.StaffId == SelectedStaff?.Id).ToObservableCollection();
        }

        private ObservableCollection<ClientStaffEntity> SortStaff()
        {
            var staff = new ObservableCollection<ClientStaffEntity>();

            if (SelectedDepartment == null && SortedDepartments != null)
            {
                foreach (var department in SortedDepartments.Where(x => x.ClientId == SelectedClient?.Id))
                {
                    staff.AddRange(Staffs?.Where(x => x?.DepartmentId == department.Id).ToObservableCollection());
                }
            }
            else
            {
                staff = Staffs?.Where(x => x?.DepartmentId == SelectedDepartment?.Id).ToObservableCollection();
            }

            return staff;
        }

        private void StaffWindow(ClientStaffEntity staff)
        {
            var staffWindow = _resolverService.Resolve<MasterStaffWindowModel>();

            staffWindow.SetSelectedStaff(staff, SelectedDepartment);

            _dialogService.ShowDialog(staffWindow);

            if (staffWindow.HasChanges)
            {
                GetStaffs();
            }
        }

        private void DeleteStaff()
        {
            var staff = SelectedStaff;

            if (staff == null) return;
            if (!_dialogService.ShowQuestionDialog($"Do you want to DELETE {staff.Name}?")) return;

            _laundryService.DeleteAsync(staff);
            Staffs.Remove(staff);

            var linens = Linens.Where(x => x.StaffId == staff.Id).ToList();

            if (!_dialogService.ShowQuestionDialog($"Do you want to DELETE {staff.Name} linens ?"))
            {
                foreach (var linen in linens)
                {
                    linen.StaffId = null;
                    _laundryService.AddOrUpdateAsync(linen.OriginalObject);
                }
                return;
            }

            foreach (var linen in linens)
            {
                _laundryService.DeleteAsync(linen.OriginalObject);
                Linens.Remove(linen);
            }
        }

        private void LinenWindow(ClientLinenEntityViewModel linen)
        {
            var linenWindow = _resolverService.Resolve<ClientLinenWindowModel>();

            linenWindow.Clients = Clients.ToList();
            linenWindow.Departments = Departments.ToList();
            linenWindow.Staffs = Staffs.ToList();
            linenWindow.MasterLinens = MasterLinens.ToList();
            linenWindow.ClientLinens = Linens.ToList();

            if (linen == null)
            {
                linen = new ClientLinenEntityViewModel()
                {
                    //ClientId = SelectedClient?.Id,
                    //DepartmentId = SelectedDepartment.Id,
                    //StaffId = SelectedStaff?.Id,
                    Tag = SelectedTag?.Tag,
                };
            }

            linenWindow.SetSelectedLinen(linen);

            _dialogService.ShowDialog(linenWindow);

            if (linenWindow.HasChanges)
            {
                GetClientLinens();
                CheckTags();
            }
        }

        private async void DeleteLinen()
        {
            var masterLinen = MasterLinens.FirstOrDefault(x => x.Id == SelectedClientLinen.MasterLinenId);

            if (!_dialogService.ShowQuestionDialog(
                $"Do you want to DELETE {masterLinen?.Name} ?"))
                return;

            await _laundryService.DeleteAsync(SelectedClientLinen.OriginalObject);

            Linens.Remove(SelectedClientLinen);
            CheckTags();
            RaisePropertyChanged(()=> SortedLinens);
        }

        private void AddShowButtonName()
        {
            if(SelectedTag == null) return;

            AddShowButton = SelectedTag.IsRegistered ? "Show" : "Add";
        }

        private void CheckTags()
        {
            foreach (var tag in RfidReaderWindow.Tags)
            {
                 tag.IsRegistered = Linens.Any(x => Equals(x.Tag, tag.Tag));
            }
        }

        private void AddShowLinenByTag()
        {
            if (SelectedTag.IsRegistered)
            {
                var linen = Linens.FirstOrDefault(x => x.Tag == SelectedTag.Tag);
                if (linen == null) return;

                SelectedClientLinen = linen;

                LinenWindow(linen);

                //SelectedClient = Clients.FirstOrDefault(x => x.Id == linen.ClientId);
                //SelectedDepartment = SortedDepartments.FirstOrDefault(x => x.Id == linen.DepartmentId);

                //if (linen.StaffId != null)
                //{
                //    SelectedStaff = SortedStaff.FirstOrDefault(x => x.Id == linen.StaffId);
                //}
            }
            else
            {
                UseTag();
                CheckTags();
            }
        }

        private void DeleteTag()
        {
            if (!_dialogService.ShowQuestionDialog($"Do you want to Delete \"{SelectedClientLinen.Tag}\" ?"))
            {
                return;
            }

            SelectedClientLinen.Tag = null;
            SelectedClientLinen.AcceptChanges();
            CheckTags();

            _laundryService.AddOrUpdateAsync(SelectedClientLinen.OriginalObject);
        }

        private void ClearSelectedStaff()
        {
            SelectedStaff = null;
        }

        private void UseTag()
        {
            if(SelectedClientLinen ==null || SelectedTag == null) return;

            SelectedClientLinen.Tag = SelectedTag.Tag;
            SelectedClientLinen.AcceptChanges();
            SelectedTag.IsRegistered = true;

            _laundryService.AddOrUpdateAsync(SelectedClientLinen.OriginalObject);

            //if (Linens.Any(x => x.Tag == SelectedTag.Item2))
            //{
            //    var existLinen = Linens.FirstOrDefault(x => Equals(x.Tag, SelectedTag.Item2));
            //    var staff = Staffs.FirstOrDefault(x => x.Id == existLinen.StaffId);

            //    if (!_dialogService.ShowQuestionDialog(
            //        $"Tag {SelectedTag.Item2} already using by {staff?.Name} in <{existLinen.Id}> linen \n Do you want to shift Tag?")
            //    ) return;

            //    existLinen.Tag = null;
            //    SelectedClientLinen.Tag = SelectedTag.Item2;
            //    return;
            //}

            //if (!String.IsNullOrWhiteSpace(SelectedClientLinen.Tag))
            //{
            //    if (_dialogService.ShowQuestionDialog($"Selected linen has tag, \n Do you want to replace it?"))
            //    {
            //        SelectedClientLinen.Tag = SelectedTag.Item2;
            //        return;
            //    }
            //}
        }
    }
}
