using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.LinenModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;
using PALMS.ViewModels.EntityViewModel;

namespace PALMS.Invoices.ViewModel.Windows
{
    public class DepartmentDetailsViewModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly IDataService _dataService;
        private Client _selectedClient;
        private ObservableCollection<DepartmentViewModel> _departments;
        private DepartmentViewModel _selectedDepartment;
        private ObservableCollection<DepartmentContractViewModel> _departmentContracts;
        private DepartmentContractViewModel _selectedContract;

        private List<FamilyLinen> _linenTypeList;

        private bool? _hasChanges;
        private bool _isValid;
        private bool _isVisibleContracts;

        public bool IsVisibleContracts
        {
            get => _isVisibleContracts;
            set => Set(ref _isVisibleContracts, value);
        }
        public bool IsValid
        {
            get => _isValid;
            set => Set(ref _isValid, value);
        }
        public Client SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }
        public DepartmentViewModel SelectedDepartment
        {
            get => _selectedDepartment;
            set => Set(ref _selectedDepartment, value);
        }
        public ObservableCollection<DepartmentViewModel> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }
        public ObservableCollection<DepartmentContractViewModel> DepartmentContracts
        {
            get => _departmentContracts;
            set => Set(ref _departmentContracts, value);
        }
        public DepartmentContractViewModel SelectedContract
        {
            get => _selectedContract;
            set => Set(ref _selectedContract, value);
        }
        public List<FamilyLinen> LinenTypeList
        {
            get => _linenTypeList;
            set => Set(ref _linenTypeList, value);
        }

        public int? SelectedInvoiceId { get; set; }

        public ObservableCollection<DepartmentContractViewModel> SortedContracts => 
            // Two new departments have a DepartmentId = 0
            // will be check by DepartmentViewModel
            DepartmentContracts?.Where(x => (x.DepartmentViewModel == null && x.DepartmentId == SelectedDepartment?.Id) ||
                                            Equals(x.DepartmentViewModel, SelectedDepartment))
                                .OrderBy(x => x.OrderNumber)
                                .ToObservableCollection();

        public Action<bool> CloseAction { get; set; }
        public Action CancelEditAction { get; set; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand ExitCommand { get; }
        public RelayCommand AddContractCommand { get; }
        public RelayCommand RemoveContractCommand { get; }


        public DepartmentDetailsViewModel(IDialogService dialogService, IDataService dataService)
        {
            _dialogService = dialogService;
            _dataService = dataService;

            SaveCommand = new RelayCommand(() => Save(), CanSave);
            ExitCommand = new RelayCommand(() => Close());
            AddContractCommand =new RelayCommand(AddContract, () => SelectedDepartment !=null);
            RemoveContractCommand = new RelayCommand(RemoveContract, () => SelectedContract != null);

            PropertyChanged += OnPropertyChanged;
        }

        public async Task InitializeAsync(Client client, int? invoiceId)
        {
            if (client == null) return ;

            SelectedClient = client;
            SelectedInvoiceId = invoiceId;

            var departmentContract = await _dataService.GetAsync<DepartmentContract>();
            var departmentContracts = departmentContract.Where(x => x.InvoiceId == SelectedInvoiceId);
            DepartmentContracts = departmentContracts.Select(x => new DepartmentContractViewModel(x)).ToObservableCollection();

            var department = await _dataService.GetAsync<Department>(x=> x.ClientId == SelectedClient.Id);
            var departments = department.Select(x => new DepartmentViewModel(x));
            Departments = departments.ToObservableCollection();

            var familyType = await _dataService.GetAsync<FamilyLinen>();
            LinenTypeList = familyType;

            Departments.ForEach(SubscribeDepartment);
            DepartmentContracts.CollectionChanged += CollectionChanged;

            SelectedDepartment = Departments.FirstOrDefault();
        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is DepartmentViewModel item)) return;

            IsVisibleContracts = !item.AllFree;

            if (!item.KeyParentId.HasValue)
            {
                foreach (var child in Departments.Where(x => x.KeyParentId == item.KeyId))
                {
                    child.AllFree = item.AllFree;
                }
            }
        }

        private void UnSubscribeDepartment(DepartmentViewModel item)
        {
            item.PropertyChanged -= ItemOnPropertyChanged;
        }

        private void SubscribeDepartment(DepartmentViewModel item)
        {
            item.PropertyChanged += ItemOnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedDepartment))
            {
                RaisePropertyChanged(() => SortedContracts);
                AddContractCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(SelectedContract))
            {
                RemoveContractCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == nameof(IsValid))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }

        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(() => SortedContracts);
        }


        private void AddContract()
        {
            DepartmentContracts.Add(new DepartmentContractViewModel
            {
                DepartmentId = SelectedDepartment.Id,
                DepartmentViewModel = SelectedDepartment,
                OrderNumber = 10000 // workaround for correct ordering, will be set to correct OrderNumber by behavior
            });
        }

        private async void RemoveContract()
        {
            if (_dialogService.ShowQuestionDialog($"Do you want remove Contract Id '{SelectedContract.Id}'?"))
            {
                CancelEditAction?.Invoke();

                var entity = SelectedContract.OriginalObject;

                DepartmentContracts.Remove(SelectedContract);

                if (!entity.IsNew)
                    await _dataService.DeleteAsync(entity);
            }
        }

        private bool CanSave()
        {
            return IsValid;
        }

        private async void Save()
        {
            var changedDepartments = GetChangedDepartments();
            var changedContracts = GetChangedContracts();

            if (changedDepartments.Any())
            {
                changedDepartments.ForEach(x =>
                {
                    x.AcceptChanges();
                    if (x.KeyParentId.HasValue)
                    {
                        var originalObjectParent = Departments.FirstOrDefault(d => d.KeyId == x.KeyParentId)?.OriginalObject;
                        
                        if (originalObjectParent?.IsNew == false)
                            x.OriginalObject.ParentId = originalObjectParent.Id;
                        else if (originalObjectParent?.IsNew == true)
                            x.OriginalObject.Parent = originalObjectParent;
                    }
                });

                await _dataService.AddOrUpdateAsync(changedDepartments.Select(x => x.OriginalObject));
            }
            if (changedContracts.Any())
            {
                changedContracts.ForEach(x => x.AcceptChanges());

                await _dataService.AddOrUpdateAsync(changedContracts.Select(x => x.OriginalObject));
            }

            if (_dialogService.ShowQuestionDialog("Do you want to close window?"))
            {
                CloseAction?.Invoke(true);
            }
        }

        protected void Close()
        {
            var changedContracts = GetChangedContracts();

            CloseAction?.Invoke(changedContracts.Any(x => x.HasChanges()));
        }

        private DepartmentViewModel[] GetChangedDepartments()
        {
            return Departments.Where(x => x.HasChanges()).ToArray();
        }
        private DepartmentContractViewModel[] GetChangedContracts()
        {
            return DepartmentContracts.Where(x => x.HasChanges()).ToArray();
        }
    }
}
