using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;
using PALMS.ViewModels.EntityViewModel;

namespace PALMS.ViewModels
{
    public class ChargeDetailsViewModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly IDataService _dataService;
        private ClientViewModel _selectedClient;
        private ObservableCollection<InvoiceDetailsViewModel> _invoiceDetails;
        private InvoiceDetailsViewModel _selectedCharge;
        private ObservableCollection<UnitViewModel> _units;

        public ClientViewModel SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }
        public ObservableCollection<InvoiceDetailsViewModel> InvoiceDetails
        {
            get => _invoiceDetails;
            set => Set(ref _invoiceDetails, value);
        }
        public InvoiceDetailsViewModel SelectedCharge
        {
            get => _selectedCharge;
            set => Set(ref _selectedCharge, value);
        }
        public bool IsValid { get; set; }
        public ObservableCollection<UnitViewModel> Units
        {
            get => _units;
            set => Set(ref _units, value);
        }

        public ObservableCollection<InvoiceDetailsViewModel> SortedInvoices =>
            InvoiceDetails.Where(x => x.ClientId == SelectedClient.Id).ToObservableCollection();

        public Action<bool> CloseAction { get; set; }
        public Action CancelEditAction { get; set; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand ExitCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand RemoveCommand { get; }

        public ChargeDetailsViewModel(IDialogService dialogService, IDataService dataService)
        {
            _dialogService = dialogService;
            _dataService = dataService;

            SaveCommand = new RelayCommand(() => Save(), CanSave);
            ExitCommand = new RelayCommand(() => Close());
            AddCommand = new RelayCommand(() => AddCharge());
            RemoveCommand = new RelayCommand(RemoveCharge, () => SelectedCharge != null);

            Units = EnumExtentions.GetValues<FeeUnitEnum>().ToObservableCollection();

            PropertyChanged += OnPropertyChanged;
        }

        public async Task InitializeAsync(ClientViewModel client)
        {
            SelectedClient = client;

            var invocieDetail = await _dataService.GetAsync<TaxAndFees>();
            var invocieDetails = invocieDetail.Select(x=> new InvoiceDetailsViewModel(x));
            InvoiceDetails = invocieDetails.Where(x => x.ClientId == SelectedClient.Id && x.InvoiceId == null).ToObservableCollection();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsValid))
            {
                SaveCommand.RaiseCanExecuteChanged();
                //RaisePropertyChanged(() => IsVisible);
            }
        }

        private void AddCharge ()
        {
            InvoiceDetails.Add(new InvoiceDetailsViewModel()
            {
                ClientId = SelectedClient.Id,
                UnitId = 1,
                
            });
        }

        private async void Save()
        {
            if (!_dialogService.ShowQuestionDialog("Do you want to save changes? "))
                return;

            var changedFees = GetChangedFees();
            changedFees.ForEach(x => x.AcceptChanges());
            await _dataService.AddOrUpdateAsync(changedFees.Select(x => x.OriginalObject));

            CloseAction?.Invoke(true);
        }

        protected void Close()
        {
            CloseAction?.Invoke(true);
        }

        private async void RemoveCharge()
        {
            if (_dialogService.ShowQuestionDialog($"Do you want remove Contract Id '{SelectedCharge.Name}'?"))
            {
                CancelEditAction?.Invoke();

                var entity = SelectedCharge.OriginalObject;

                InvoiceDetails.Remove(SelectedCharge);

                if (!entity.IsNew)
                    await _dataService.DeleteAsync(entity);
            }
        }
        private bool CanSave()
        {
            return IsValid;
        }

        private InvoiceDetailsViewModel[] GetChangedFees()
        {
            return InvoiceDetails.Where(x => x.HasChanges()).ToArray();
        }

    }
}
