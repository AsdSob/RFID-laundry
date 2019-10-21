using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.InvoiceModel;
using PALMS.Invoices.ViewModel.EntityViewModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;

namespace PALMS.Invoices.ViewModel.Windows
{
    public class ChargeViewModel : ViewModelBase, IWindowDialogViewModel
    {
        public Action<bool> CloseAction { get; set; }
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private readonly IDispatcher _dispatcher;

        private Client _selectedClient;
        private ObservableCollection<TaxAndFeeViewModel> _taxAndFees;
        private ObservableCollection<ExtraChargeViewModel> _extraCharges;
        private TaxAndFeeViewModel _selectedTaxAndFee;
        private ExtraChargeViewModel _selectedExtraCharge;
        private List<UnitViewModel> _units;

        public Client SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }
        public ObservableCollection<ExtraChargeViewModel> ExtraCharges
        {
            get => _extraCharges;
            set => Set(ref _extraCharges, value);
        }
        public ObservableCollection<TaxAndFeeViewModel> TaxAndFees
        {
            get => _taxAndFees;
            set => Set(ref _taxAndFees, value);
        }
        public TaxAndFeeViewModel SelectedTaxAndFee
        {
            get => _selectedTaxAndFee;
            set => Set(ref _selectedTaxAndFee, value);
        }
        public ExtraChargeViewModel SelectedExtraCharge
        {
            get => _selectedExtraCharge;
            set => Set(ref _selectedExtraCharge, value);
        }
        public List<UnitViewModel> Units
        {
            get => _units;
            set => Set(ref _units, value);
        }
        public bool IsChanged { get; set; }
        public int? SelectedInvoiceId { get; set; }

        public RelayCommand AddTaxAndFeeCommand { get; }
        public RelayCommand RemoveTaxAndFeeCommand { get; }
        public RelayCommand AddExtraChargeCommand { get; }
        public RelayCommand RemoveExtraChargeCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand SaveCommand { get; }

        public ChargeViewModel(IDialogService dialogService, IDataService dataService, IDispatcher dispatcher)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            AddExtraChargeCommand = new RelayCommand(AddExtraCharge);
            RemoveExtraChargeCommand = new RelayCommand(RemoveExtraCharge, ()=> SelectedExtraCharge != null);
            AddTaxAndFeeCommand = new RelayCommand(AddTaxAndFee);
            RemoveTaxAndFeeCommand = new RelayCommand(RemoveTaxAndFee, () => SelectedTaxAndFee != null);

            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(Close);

            PropertyChanged += OnPropertyChanged;
        }

        public async Task InitializeAsync(Client selectedClient, int? invoiceId)
        {
            SelectedClient = selectedClient;
            SelectedInvoiceId = invoiceId;

            _dialogService.ShowBusy();

            try
            {
                var extraCharge = await _dataService.GetAsync<ExtraCharge>(x => x.InvoiceId == SelectedInvoiceId);
                var extraCharges = extraCharge.Select(x => new ExtraChargeViewModel(x));
                _dispatcher.RunInMainThread(() => ExtraCharges = extraCharges.ToObservableCollection());

                var taxAndFee = await _dataService.GetAsync<TaxAndFees>(x=> x.InvoiceId == SelectedInvoiceId && x.ClientId == SelectedClient.Id);
                var taxAndFees = taxAndFee.Select(x => new TaxAndFeeViewModel(x));
                _dispatcher.RunInMainThread(() => TaxAndFees = taxAndFees.ToObservableCollection());

                Units = EnumExtentions.GetValues<FeeUnitEnum>().ToList();
            }

            finally
            {
                _dialogService.HideBusy();
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedExtraCharge))
            {
                RemoveExtraChargeCommand.RaiseCanExecuteChanged();
            }
            if (e.PropertyName == nameof(SelectedTaxAndFee))
            {
                AddTaxAndFeeCommand.RaiseCanExecuteChanged();
            }
        }

        public void AddExtraCharge()
        {
            ExtraCharges.Add(new ExtraChargeViewModel()
            {
                InvoiceId = SelectedInvoiceId,
            });
            IsChanged = true;
        }

        public void RemoveExtraCharge()
        {
            if (SelectedExtraCharge == null || !_dialogService.ShowQuestionDialog($"Do you want to remove Extra Charge {SelectedExtraCharge.Name} ? "))
                return;

            ExtraCharges.Remove(SelectedExtraCharge);
            IsChanged = true;
        }

        public async void AddTaxAndFee()
        {
            var newTaxAndFee = new TaxAndFeeViewModel()
            {
                InvoiceId = SelectedInvoiceId,
                ClientId = SelectedClient.Id,
                UnitId = 1,
            };
            TaxAndFees.Add(newTaxAndFee);

            IsChanged = true;
        }

        public async void RemoveTaxAndFee()
        {
            if (SelectedTaxAndFee == null || !_dialogService.ShowQuestionDialog($"Do you want to remove Tax/Fee {SelectedTaxAndFee.Name} ? "))
                return;

            TaxAndFees.Remove(SelectedTaxAndFee);

            await _dataService.DeleteAsync(SelectedTaxAndFee.OriginalObject);
            IsChanged = true;
        }

        private async void Save()
        {
            var taxAndFees = TaxAndFees.Where(x => x.HasChanges()).ToList();
            if (!_dialogService.ShowQuestionDialog($"Do you want to Save TaxAndFees ? "))
                return;

            if(!taxAndFees.Any()) return;

            IsChanged = true;
            taxAndFees.ForEach(x => x.AcceptChanges());
            await _dataService.AddOrUpdateAsync(taxAndFees.Select(x=> x.OriginalObject));
        }

        public void Close()
        {
            if (!_dialogService.ShowQuestionDialog($"Do you want to close window ? "))
                return;
            CloseAction?.Invoke(IsChanged);
        }

        public ObservableCollection<ExtraChargeViewModel> GetExtraCharges()
        {
            var extraCharges = ExtraCharges;
            return extraCharges;
        }

        public ObservableCollection<TaxAndFeeViewModel> GetTaxAndFees()
        {
            var taxAndFees = TaxAndFees;
            return taxAndFees;
        }

    }
}
