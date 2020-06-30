using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Common.Windows;

namespace Client.Desktop.ViewModels.Windows
{
    public class MasterStaffWindowModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;
        private StaffDetailsEntityViewModel _selectedStaffDetails;
        private ObservableCollection<DepartmentEntityViewModel> _departments;
        private DepartmentEntityViewModel _selectedStaff;

        public DepartmentEntityViewModel SelectedStaff
        {
            get => _selectedStaff;
            set => Set(ref _selectedStaff, value);
        }
        public ObservableCollection<DepartmentEntityViewModel> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }

        public StaffDetailsEntityViewModel SelectedStaffDetails
        {
            get => _selectedStaffDetails;
            set => Set(ref _selectedStaffDetails, value);
        }

        public Action<bool> CloseAction { get; set; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand InitializeCommand { get; }

        public MasterStaffWindowModel(ILaundryService laundryService, IDialogService dialogService)
        {
            _laundryService = laundryService ?? throw new ArgumentNullException(nameof(laundryService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(Close);
            DeleteCommand = new RelayCommand(Delete);
            //InitializeCommand = new RelayCommand(Initialize);

        }

        public void SetSelectedStaff(DepartmentEntityViewModel item, DepartmentEntityViewModel parent)
        {
            SelectedStaff = null;

            if (item != null)
            {
                SelectedStaff = item;

                if (item.OriginalObject.StaffDetailsEntity == null)
                {
                    SelectedStaffDetails = new StaffDetailsEntityViewModel();
                }
                else
                {
                    SelectedStaffDetails = new StaffDetailsEntityViewModel(item.OriginalObject.StaffDetailsEntity);
                }
                return;
            }

            NewItem(parent);
        }

        private void NewItem(DepartmentEntityViewModel parent)
        {
            SelectedStaff = new DepartmentEntityViewModel
            {
                ParentId = parent.Id,
                ClientId = parent.ClientId,
            };

            SelectedStaffDetails = new StaffDetailsEntityViewModel();
        }

        private async void Initialize()
        {
            _dialogService.ShowBusy();

            try
            {
                //var staff = await _laundryService.GetAllAsync<StaffDetailsEntity>();
                //var staffs = staff.Select(x => new StaffDetailsEntityViewModel(x));
                //Staffs = staffs.ToObservableCollection();

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
            var canClose = false;
            
            if (SelectedStaff.IsValid && SelectedStaff.HasChanges())
            {
                SelectedStaff.AcceptChanges();
                _laundryService.AddOrUpdateAsync(SelectedStaff.OriginalObject);
                canClose =true;
            }

            if (SelectedStaffDetails.IsValid && SelectedStaffDetails.HasChanges())
            {
                SelectedStaffDetails.DepartmentId = SelectedStaff.OriginalObject.Id;
                SelectedStaffDetails.AcceptChanges();
                _laundryService.AddOrUpdateAsync(SelectedStaffDetails.OriginalObject);
                canClose = true;
            }

            if (canClose)
            {
                Close();
            }
        }

        private void Delete()
        {
            if (!_dialogService.ShowQuestionDialog($"Do you want to DELETE {SelectedStaff.Name} ?"))
                return;

            if (!SelectedStaff.IsNew)
            {
                _laundryService.DeleteAsync(SelectedStaff.OriginalObject);
            }

            if (!SelectedStaffDetails.IsNew)
            {
                _laundryService.DeleteAsync(SelectedStaffDetails.OriginalObject);
            }

            Close();
        }

        private void Close()
        {
            if (SelectedStaff.HasChanges() || SelectedStaffDetails.HasChanges())
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
