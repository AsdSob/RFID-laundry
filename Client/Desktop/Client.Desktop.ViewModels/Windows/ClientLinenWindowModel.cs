using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Common.Windows;

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
        private ObservableCollection<DepartmentEntityViewModel> _sortedDepartments;

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

        public ObservableCollection<DepartmentEntityViewModel> SortedDepartments => Departments
            ?.Where(x => x.ClientId == SelectedLinen?.ClientId && x.ParentId == null).ToObservableCollection();

        public ObservableCollection<DepartmentEntityViewModel> SortedStaffs => Departments
            ?.Where(x => x.ParentId == SelectedLinen?.DepartmentId).ToObservableCollection();

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
        }

        public async Task Initialize()
        {
            _dialogService.ShowBusy();

            try
            {
                Clients = await _laundryService.Clients();
                Departments = await _laundryService.Departments();
                MasterLinens = await _laundryService.MasterLinens();
                //ClientLinens = await _laundryService.ClientLinens();


                SelectedLinen =new ClientLinenEntityViewModel();
                SelectedLinen.PropertyChanged += ItemOnPropertyChanged;
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

        /// <summary>
        /// Need to add ClientLinen List before
        /// </summary>
        /// <param name="item"></param>
        public async void SetItem(ClientLinenEntityViewModel item)
        {
            await Initialize();

            if (item != null)
            {
                SelectedLinen.Update(item.OriginalObject);
            }
        }

        private void NewLinen()
        {
            var linen = new ClientLinenEntityViewModel();

            SelectedLinen = linen;
        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is ClientLinenEntityViewModel item)) return;

            RaisePropertyChanged(() => SortedDepartments);
            RaisePropertyChanged(() => SortedStaffs);

            if (e.PropertyName == nameof(SelectedLinen.MasterLinenId))
            {
                SelectedLinen.PackingValue = MasterLinens.FirstOrDefault(x => x.Id == SelectedLinen.MasterLinenId).PackingValue;
            }
        }

        private void Save()
        {
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
