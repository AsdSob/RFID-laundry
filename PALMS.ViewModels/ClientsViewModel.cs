using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.Reports.Common;
using PALMS.Reports.Model;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.EntityViewModel;
using DepartmentViewModel = PALMS.ViewModels.EntityViewModel.DepartmentViewModel;

namespace PALMS.ViewModels
{
    public class ClientsViewModel : ViewModelBase, IInitializationAsync
    {
        private readonly ICanExecuteMediator _canExecuteMediator;
        private readonly ILogger _logger;
        private readonly IResolver _resolver;
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private readonly IReportService _reportService;
        private ObservableCollection<ClientViewModel> _clients;
        private ClientViewModel _selectedClient;
        private ObservableCollection<DepartmentViewModel> _departments;

        public ObservableCollection<ClientViewModel> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }
        public ClientViewModel SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }
        public ObservableCollection<DepartmentViewModel> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }
        public  ObservableCollection<DepartmentViewModel> SortedDepartments =>
            Departments?.Where(x => x.ClientId == SelectedClient?.Id).ToObservableCollection();

        public RelayCommand AddCommand { get; }
        public RelayCommand AddDepartmentCommand { get; }
        public RelayCommand AddFeeCommand { get; }
        public RelayCommand EditCommand { get; set; }
        public RelayCommand RemoveCommand { get; set; }
        public RelayCommand PrintCommand { get; set; }

        public ClientsViewModel(IResolver resolverService, IDataService dataService, IDialogService dialogService,
            ICanExecuteMediator canExecuteMediator, IReportService reportService, ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _canExecuteMediator = canExecuteMediator ?? throw new ArgumentNullException(nameof(canExecuteMediator));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _resolver = resolverService ?? throw new ArgumentNullException(nameof(resolverService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));

            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand(Edit, () => SelectedClient != null);
            RemoveCommand = new RelayCommand(Remove, () => SelectedClient != null);
            PrintCommand = new RelayCommand(Print);
            AddDepartmentCommand = new RelayCommand(EditDepartment, () => SelectedClient != null);
            AddFeeCommand = new RelayCommand(EditFee, () => SelectedClient != null);

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient))
            {
                EditCommand.RaiseCanExecuteChanged();
                RemoveCommand.RaiseCanExecuteChanged();
                AddDepartmentCommand.RaiseCanExecuteChanged();
                AddFeeCommand.RaiseCanExecuteChanged();

                RaisePropertyChanged(() => SortedDepartments);
            }
            if (e.PropertyName == nameof(Departments))
            {
                RaisePropertyChanged(() => SortedDepartments);
            }
        }

        public async Task InitializeAsync()
        {
            _canExecuteMediator.CanExecute = null;

            if (Clients?.Any() == true) return;

            _dialogService.ShowBusy();

            try
            {
                var departments = await _dataService.GetAsync<Department>();
                Departments = departments.Select(x => new DepartmentViewModel(x)).ToObservableCollection();

                var clients = await _dataService.GetAsync<Client>(x => x.ClientInfo, x => x.TaxAndFees);

                Helper.RunInMainThread(() => Clients = clients.OrderBy(x => x.Name).Select(x => new ClientViewModel(x)).ToObservableCollection());
            }
            catch (Exception ex)
            {
                _dialogService.HideBusy();
                _logger.Error("Initialization error", ex);
                Helper.RunInMainThread(() => _dialogService.ShowErrorDialog($"Initialization error: {ex.Message}"));
            }
            finally
            {
                _dialogService.HideBusy();
            }
        }


        private bool NameUniqueValidation(ClientViewModel clientViewModel, string propertyName)
        {
            if (clientViewModel == null) throw new ArgumentNullException(nameof(clientViewModel));

            var properyInfo = typeof(ClientViewModel).GetProperty(propertyName);

            if (properyInfo == null) throw new ArgumentNullException($"{propertyName} not found");

            Func<object, object> additionalPrepareFunc = null;

            if (properyInfo.PropertyType == typeof(string))
            {
                additionalPrepareFunc = x => x?.ToString().Trim();
            }

            var clientPropertyValue = additionalPrepareFunc == null
                ? properyInfo.GetValue(clientViewModel)
                : additionalPrepareFunc(properyInfo.GetValue(clientViewModel));

            return !Clients.Any(x => Equals(properyInfo.GetValue(x), clientPropertyValue) &&
                                     x.Id != clientViewModel.Id);
        }

        private void Add()
        {
            var viewModel = _resolver.Resolve<ClientDetailsViewModel>();
            viewModel.EditClient.Client.NameUniqueValidationFunc = NameUniqueValidation;

            if (_dialogService.ShowDialog(viewModel))
            {
                if (viewModel.EditClient.Client?.IsNew == false)
                    Clients.Add(viewModel.EditClient.Client.Clone());
            }
        }

        private void Edit()
        {
            if (SelectedClient == null) return;

            var viewModel = _resolver.Resolve<ClientDetailsViewModel>();
            viewModel.SetClient(SelectedClient.Clone());
            viewModel.EditClient.Client.NameUniqueValidationFunc = NameUniqueValidation;

            if (_dialogService.ShowDialog(viewModel))
            {
                SelectedClient.Update(viewModel.GetClient());
            }
        }

        private async void EditDepartment()
        {
            if (SelectedClient == null)
                return;

            var departmentViewModel = _resolver.Resolve<DepartmentDetailsViewModel>();

            var departments = Departments.Where(x => x.ClientId == SelectedClient.Id).ToObservableCollection();

            await departmentViewModel.InitializeAsync(departments, SelectedClient);

            var showDialog = _dialogService.ShowDialog(departmentViewModel);

            if (showDialog)
            {
                UpdateDepartments();
            }
        }

        private async void EditFee()
        {
            if (SelectedClient == null)
                return;

            var chargeViewModel = _resolver.Resolve<ChargeDetailsViewModel>();
            await chargeViewModel.InitializeAsync(SelectedClient);

            _dialogService.ShowDialog(chargeViewModel);
        }

        private async void Remove()
        {
            if (SelectedClient == null)
                return;

            if (!_dialogService.ShowQuestionDialog($"Remove the client '{SelectedClient.Name}'?"))
                return;

            _dialogService.ShowBusy();

            var departments = Departments.Where(x => x.ClientId == SelectedClient.Id).Select(x => x.OriginalObject)
                .ToList();

            try
            {
                await _dataService.DeleteAsync(SelectedClient.OriginalObject);

                foreach (var department in departments)
                {
                    await _dataService.DeleteAsync(department);
                }

                Clients.Remove(SelectedClient);
            }
            finally
            {
                _dialogService.HideBusy();
            }
        }

        private void Print()
        {
            foreach (var client in Clients)
            {
                foreach (var department in Departments.Where(x=> x.ClientId == client.Id))
                {
                    department.Express = client.ClientInfo.Express;
                }
            }

            _dialogService.ShowInfoDialog("Done");
        }

        public async void UpdateDepartments()
        {
            Departments = new ObservableCollection<DepartmentViewModel>();

            var departments = await _dataService.GetAsync<Department>();
            Departments = departments.Select(x => new DepartmentViewModel(x)).ToObservableCollection();
        }

        private IReport GetLinesInvoiceReport()
        {
            var report = new LinensInvoiceReport
            {
                CreateDate = DateTime.Now,
                CustomerAddress = "UAE, Ruwais, Mosque",
                CustomerName = SelectedClient.Name,
                InvoiceDate = DateTime.Now.AddDays(-1),
                InvoiceNumber = $"{12:0000000}",
                VendorName = "PALMS",
                VendorAddress = "UAE",
                VendorEmail = "palms@palms.com",
                VendorWebSite = "http://palms.com",
                VendorPhone = "+971 100 00 00",
                DeliveryType = "Express"
            };

            report.Items.Add(new LinensInvoiceItem { Name = "Sheet", Description = "Double size sheet", Quantity = 40, UnitPrice = 2.2});
            report.Items.Add(new LinensInvoiceItem { Name = "Towel XL", Description = "Big size towel", Quantity = 100, UnitPrice = 0.8});
            report.Items.Add(new LinensInvoiceItem { Name = "Towel M", Description = "Medium size towel", Quantity = 100, UnitPrice = 0.6});
            report.Items.Add(new LinensInvoiceItem { Name = "Towel S", Description = "Small size towel", Quantity = 100, UnitPrice = 0.4});
            report.Items.Add(new LinensInvoiceItem { Name = "Pillowcase", Description = "Pillowcase 50x70", Quantity = 80, UnitPrice = 1.9});
            report.Items.Add(new LinensInvoiceItem { Name = "Quilt", Description = "Double size quilt", Quantity = 40, UnitPrice = 1.1});
            report.Items.Add(new LinensInvoiceItem { Name = "Bathrobe", Description = "", Quantity = 57, UnitPrice = 4.0});
            report.Items.Add(new LinensInvoiceItem { Name = "Cover XL", Description = "Big size cover", Quantity = 20, UnitPrice = 10.0});
            report.Items.Add(new LinensInvoiceItem { Name = "Cover M", Description = "Medium size cover", Quantity = 34, UnitPrice = 8.0});
            report.Items.Add(new LinensInvoiceItem { Name = "Cover S", Description = "Small size cover", Quantity = 24, UnitPrice = 6.0});

            int index =0;
            foreach (var item in report.Items)
            {
                item.Total = item.Quantity * item.UnitPrice;

                report.Total += item.Total;
                
                report.TotalQty = ++index;
            }
            

            return report;
        }

        //private IReport GetClientsReport()
        //{
        //    if (Clients == null) return null;

        //    var report = new ClientsReport();

        //    foreach (var client in Clients)
        //    {
        //        var clientItem = new ClientItem();
        //        clientItem.Id = client.Id;
        //        clientItem.Name = client.Name;
        //        clientItem.ShortName = client.ShortName;
        //        clientItem.OrderCount = 345;
        //        report.ItemRows.Add(clientItem);
        //    }

        //    return report;
        //}
    }
}
