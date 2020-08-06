using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Common.Windows;

namespace Client.Desktop.ViewModels.Windows
{
    public class DeliveryNoteWidowModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;
        private DeliveryNoteEntityViewModel _selectedDeliveryNote;
        private ObservableCollection<ClientEntityViewModel> _clients;
        private ObservableCollection<DeliveryNoteEntityViewModel> _deliveryNotes;
        private ObservableCollection<DepartmentEntityViewModel> _departments;
        private DepartmentEntityViewModel _selectedDepartment;
        private DepartmentEntityViewModel _selectedStaff;
        private ClientEntityViewModel _selectedClient;

        public ClientEntityViewModel SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }

        public Action<bool> CloseAction { get; set; }

        public DepartmentEntityViewModel SelectedStaff
        {
            get => _selectedStaff;
            set => Set(ref _selectedStaff, value);
        }

        public DepartmentEntityViewModel SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(ref _selectedDepartment, value);
        }

        public ObservableCollection<DepartmentEntityViewModel> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }

        public ObservableCollection<DeliveryNoteEntityViewModel> DeliveryNotes
        {
            get => _deliveryNotes;
            set => Set(ref _deliveryNotes, value);
        }

        public ObservableCollection<ClientEntityViewModel> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }

        public DeliveryNoteEntityViewModel SelectedDeliveryNote
        {
            get => _selectedDeliveryNote;
            set => Set(ref _selectedDeliveryNote, value);
        }

        public ObservableCollection<DepartmentEntityViewModel> SortedDepartments =>
            Departments?.Where(x => x.ClientId == SelectedClient?.Id && x.ParentId == null).ToObservableCollection();

        public ObservableCollection<DepartmentEntityViewModel> SortedStaffs =>
            Departments?.Where(x => x.ClientId == SelectedClient?.Id && x.ParentId == SelectedDepartment?.Id)
                .ToObservableCollection();

        public RelayCommand SaveCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand InitializeCommand { get; }


        public DeliveryNoteWidowModel(ILaundryService laundryService, IDialogService dialogService)
        {
            _laundryService = laundryService ?? throw new ArgumentNullException(nameof(laundryService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(Close);
            DeleteCommand = new RelayCommand(Delete);
            InitializeCommand = new RelayCommand(Initialize);

            PropertyChanged += OnPropertyChanged;
        }

        public void SetSelectedDeliveryNote(DeliveryNoteEntityViewModel item)
        {
            if (item == null)
            {
                _dialogService.ShowWarnigDialog("Passing item cannot be null");
                CloseAction(false);
            }

            SelectedDeliveryNote = item;
        }

        private async void Initialize()
        {
            _dialogService.ShowBusy();

            try
            {
                Clients = await _laundryService.Clients();
                Departments = await _laundryService.Departments();
                //DeliveryNotes = await _laundryService.DeliveryNotes();
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
            if (e.PropertyName == nameof(SelectedClient))
            {
                RaisePropertyChanged(() => SortedDepartments);
                SelectedDepartment = SortedDepartments.FirstOrDefault();
            }
            else if (e.PropertyName == nameof(SelectedDepartment))
            {
                RaisePropertyChanged(() => SortedStaffs);
                SelectedDeliveryNote.DepartmentId = SelectedDepartment.Id;

            }
            else if (e.PropertyName == nameof(SelectedStaff))
            {
                if (SelectedStaff == null) ;

                SelectedDeliveryNote.DepartmentId = SelectedDepartment.Id;
            }
        }

        private void Save()
        {
            //if (!SelectedDeliveryNote.HasChanges() || !SelectedDeliveryNote.IsValid)
            //{
            //    return;
            //}

            //SelectedDeliveryNote.AcceptChanges();

            //_laundryService.AddOrUpdateAsync(SelectedDeliveryNote.OriginalObject);


            Close();
        }

        private void Delete()
        {
            //if (!_dialogService.ShowQuestionDialog($"Do you want to DELETE {SelectedDeliveryNote.Name} ?"))
            //    return;

            //if (!SelectedDeliveryNote.IsNew)
            //{
            //    _laundryService.DeleteAsync(SelectedDeliveryNote.OriginalObject);
            //}

            //Close();
        }

        private void Close()
        {
            //if (SelectedDeliveryNote.HasChanges())
            //{
            //    if (_dialogService.ShowQuestionDialog($"Do you want to close window ? \n \"Changes is NOT saved\""))
            //    {
            //        CloseAction?.Invoke(false);
            //        return;
            //    }
            //}

            CloseAction?.Invoke(true);
        }
    }
}
