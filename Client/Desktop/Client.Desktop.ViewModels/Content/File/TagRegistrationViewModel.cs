using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Windows;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Content.File
{
    public class TagRegistrationViewModel :ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly ILaundryService _laundryService;
        private readonly IResolver _resolverService;
        private readonly IMainDispatcher _dispatcher;

        private ObservableCollection<ClientEntity> _clients;
        private ObservableCollection<DepartmentEntity> _departments;
        private ObservableCollection<ClientStaffEntity> _staffs;
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
        public ObservableCollection<ClientStaffEntity> Staffs
        {
            get => _staffs;
            set => Set(() => Staffs, ref _staffs, value);
        }
        public ObservableCollection<DepartmentEntity> Departments
        {
            get => _departments;
            set => Set(() => Departments, ref _departments, value);
        }
        public ObservableCollection<ClientEntity> Clients
        {
            get => _clients;
            set => Set(() => Clients, ref _clients, value);
        }

        public RfidReaderWindowModel RfidReaderWindow { get; set; }
        public string SearchingTag { get; set; }

        
        public RelayCommand NewLinenCommand { get; }
        public RelayCommand EditLinenCommand { get; }
        public RelayCommand DeleteLinenCommand { get; }

        public RelayCommand InitializeCommand { get; }

        public RelayCommand AddShowLinenByTagCommand { get; }
        public RelayCommand DeleteTagCommand { get; }


        public TagRegistrationViewModel(ILaundryService dataService, IDialogService dialogService, IResolver resolver, IMainDispatcher dispatcher)
        {
            _laundryService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));


            NewLinenCommand = new RelayCommand(() => LinenWindow(null));
            EditLinenCommand = new RelayCommand(() => LinenWindow(SelectedClientLinen),() => SelectedClientLinen !=null);
            DeleteLinenCommand = new RelayCommand(DeleteLinen,(() => SelectedClientLinen !=null));

            AddShowLinenByTagCommand = new RelayCommand(AddShowLinenByTag, () => SelectedTag != null);
            DeleteTagCommand = new RelayCommand(DeleteTag, () => SelectedTag != null || SelectedClientLinen != null);

            InitializeCommand = new RelayCommand(Initialize);
            RfidReaderWindow = _resolverService.Resolve<RfidReaderWindowModel>();

            AddShowButton = "Add";

            RfidReaderWindow.Tags.CollectionChanged += TagsCollectionChanged;

            PropertyChanged += OnPropertyChanged;
        }

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
        }

        private async void GetClientLinens()
        {
            var linen = await _laundryService.GetAllAsync<ClientLinenEntity>();
            var linens = linen.Select(x => new ClientLinenEntityViewModel(x));
            Linens = linens.ToObservableCollection();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
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
                    Tag = SelectedTag?.Tag,
                };
            }

            linenWindow.SetSelectedLinen(linen);

            if (_dialogService.ShowDialog(linenWindow))
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


        private void UseTag()
        {
            if(SelectedClientLinen ==null || SelectedTag == null) return;

            SelectedClientLinen.Tag = SelectedTag.Tag;
            SelectedClientLinen.AcceptChanges();
            SelectedTag.IsRegistered = true;

            _laundryService.AddOrUpdateAsync(SelectedClientLinen.OriginalObject);
        }
    }
}
