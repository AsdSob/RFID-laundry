using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Common.Windows;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Windows
{
    public class MasterDepartmentWindowModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;
        private ObservableCollection<DepartmentEntityViewModel> _departments;
        private List<UnitViewModel> _departmentTypes;
        private DepartmentEntityViewModel _selectedDepartment;

        public DepartmentEntityViewModel SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(ref _selectedDepartment, value);
        }
        public List<UnitViewModel> DepartmentTypes
        {
            get => _departmentTypes;
            set => Set(ref _departmentTypes, value);
        }

        public ObservableCollection<DepartmentEntityViewModel> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }

        public Action<bool> CloseAction { get; set; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand InitializeCommand { get; }

        public MasterDepartmentWindowModel(ILaundryService laundryService, IDialogService dialogService)
        {
            _laundryService = laundryService ?? throw new ArgumentNullException(nameof(laundryService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(Close);
            DeleteCommand = new RelayCommand(Delete);
            //InitializeCommand = new RelayCommand(Initialize);

            DepartmentTypes = EnumExtensions.GetValues<DepartmentTypeEnum>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="department"></param>
        /// <param name="clientId">Set Selected Client Id even for new department</param>
        public async void SetItem(DepartmentEntityViewModel department, int clientId)
        {
            await Initialize();
            if (department != null)
            {
                SelectedDepartment = department;
                return;
            }

            SelectedDepartment = new DepartmentEntityViewModel(new DepartmentEntity()
            {
                ClientId = clientId,
                DepartmentTypeId = 1,
            });
        }

        //TODO: GetOnly Departments of Passing client
        public async Task Initialize()
        {
            _dialogService.ShowBusy();

            try
            {
                Departments = await _laundryService.Departments();
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

        private void Save()
        {
            if (!SelectedDepartment.IsValid || !SelectedDepartment.HasChanges())
            {
                return;
            }

            SelectedDepartment.AcceptChanges();
            _laundryService.AddOrUpdateAsync(SelectedDepartment.OriginalObject);
            Close();
        }

        private void Delete()
        {
            if (!_dialogService.ShowQuestionDialog($"Do you want to DELETE {SelectedDepartment.Name} ?"))
                return;

            if (!SelectedDepartment.IsNew)
            {
                _laundryService.DeleteAsync(SelectedDepartment.OriginalObject);
            }

            Close();
        }

        private void Close()
        {
            if (SelectedDepartment.HasChanges())
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
