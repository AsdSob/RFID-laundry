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
    public class MasterStaffWindowModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;
        private readonly IMainDispatcher _dispatcher;
        private ClientStaffEntityViewModel _selectedClientStaff;
        private ObservableCollection<ClientStaffEntityViewModel> _staffs;

        public ObservableCollection<ClientStaffEntityViewModel> Staffs
        {
            get => _staffs;
            set => Set(ref _staffs, value);
        }
        public ClientStaffEntityViewModel SelectedClientStaff
        {
            get => _selectedClientStaff;
            set => Set(ref _selectedClientStaff, value);
        }

        public Action<bool> CloseAction { get; set; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand InitializeCommand { get; }

        public MasterStaffWindowModel(ILaundryService laundryService, IDialogService dialogService, IMainDispatcher dispatcher)
        {
            _laundryService = laundryService ?? throw new ArgumentNullException(nameof(laundryService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(Close);
            DeleteCommand = new RelayCommand(Delete);
            //InitializeCommand = new RelayCommand(Initialize);

        }

        public void SetSelectedStaff(ClientStaffEntityViewModel staff, DepartmentEntityViewModel department)
        {
            SelectedClientStaff = null;

            if (staff != null)
            {
                SelectedClientStaff = staff;
                return;
            }

            SelectedClientStaff = new ClientStaffEntityViewModel(new ClientStaffEntity()
            {
                DepartmentId = department.Id,
            });
        }

        private async void Initialize()
        {
            _dialogService.ShowBusy();

            try
            {
                var staff = await _laundryService.GetAllAsync<ClientStaffEntity>();
                var staffs = staff.Select(x => new ClientStaffEntityViewModel(x));
                Staffs = staffs.ToObservableCollection();

            }
            catch (Exception e)
            {
                _dialogService.HideBusy();
            }
            finally
            {
                _dialogService.HideBusy();
            }

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        private void Save()
        {
            if (!SelectedClientStaff.IsValid || !SelectedClientStaff.HasChanges())
            {
                return;
            }

            SelectedClientStaff.AcceptChanges();
            _laundryService.AddOrUpdateAsync(SelectedClientStaff.OriginalObject);

            Close();
        }

        private void Delete()
        {
            if (!_dialogService.ShowQuestionDialog($"Do you want to DELETE {SelectedClientStaff.Name} ?"))
                return;

            if (!SelectedClientStaff.IsNew)
            {
                _laundryService.DeleteAsync(SelectedClientStaff.OriginalObject);
            }

            Close();
        }

        private void Close()
        {
            if (SelectedClientStaff.HasChanges())
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
