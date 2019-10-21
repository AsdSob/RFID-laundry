using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.LinenModel;
using PALMS.Data.Objects.NoteModel;
using PALMS.Invoices.ViewModel.EntityViewModel;
using PALMS.Reports.Common;
using PALMS.Reports.Model.NoteReports;
using PALMS.Reports.Model.Notes;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;

namespace PALMS.Invoices.ViewModel.Windows
{
    public class NoteEditViewModel : ViewModelBase, IWindowDialogViewModel
    {
        public Action<bool> CloseAction { get; set; }
        private readonly IDialogService _dialogService;
        private readonly IDataService _dataService;
        private readonly IDispatcher _dispatcher;
        private readonly IReportService _reportService;

        private NoteHeaderViewModel _selectedNoteHeader;
        private ObservableCollection<NoteRowViewModel> _noteRows;

        private List<UnitViewModel> _deliveryTypes;
        private List<UnitViewModel> _serviceTypes;
        private bool _isChanged;
        private List<LinenList> _linenLists;
        private Client _selectedClient;
        private Department _selectedDepartment;
        private PrimeInfo _primeInfo;

        public PrimeInfo PrimeInfo
        {
            get => _primeInfo;
            set => Set(ref _primeInfo, value);
        }
        public Department SelectedDepartment 
        {
            get => _selectedDepartment;
            set => Set(ref _selectedDepartment, value);
        }
        public Client SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }
        public List<LinenList> LinenLists
        {
            get => _linenLists;
            set => Set(ref _linenLists, value);
        }
        public bool IsChanged
        {
            get => _isChanged;
            set => Set(ref _isChanged, value);
        }
        public List<UnitViewModel> ServiceTypes
        {
            get => _serviceTypes;
            set => Set(ref _serviceTypes, value);
        }
        public List<UnitViewModel> DeliveryTypes
        {
            get => _deliveryTypes;
            set => Set(ref _deliveryTypes, value);
        }
        public ObservableCollection<NoteRowViewModel> NoteRows
        {
            get => _noteRows;
            set => Set(ref _noteRows, value);
        }
        public NoteHeaderViewModel SelectedNoteHeader
        {
            get => _selectedNoteHeader;
            set => Set(ref _selectedNoteHeader, value);
        }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand PrintCommand { get; }

        public NoteEditViewModel(IDataService dataService, IDispatcher dispatcher, IDialogService dialogService, IReportService reportService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _dataService = dataService ?? throw new ArgumentException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentException(nameof(dispatcher));
            _reportService = reportService ?? throw new ArgumentException(nameof(reportService));

            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(Close);
            CancelCommand = new RelayCommand(Cancel);
            PrintCommand = new RelayCommand(Print);
        }

        private void UnSubscribeItem(NoteRowViewModel item)
        {
            item.PropertyChanged -= ItemOnPropertyChanged;
        }

        private void SubscribeItem(NoteRowViewModel item)
        {
            item.PropertyChanged += ItemOnPropertyChanged;
        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is NoteRowViewModel item)) return;

            if (e.PropertyName == nameof(item.ServiceTypeId))
            {
                switch (item.ServiceTypeId)
                {
                    case (int)ServiceTypeEnum.Laundry:
                        item.Price = item.OriginalObject.LinenList.Laundry;
                        break;

                    case (int)ServiceTypeEnum.DryCleaning:
                        item.Price = item.OriginalObject.LinenList.DryCleaning;
                        break;

                    case (int)ServiceTypeEnum.Pressing:
                        item.Price = item.OriginalObject.LinenList.Pressing;
                        break;
                }
            }
        }

        private void NoteRowsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<NoteRowViewModel>())
                {
                    SubscribeItem(item);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (var item in e.OldItems.OfType<NoteRowViewModel>())
                {
                    UnSubscribeItem(item);
                }
            }
        }

        public async Task InitializeAsync(NoteHeaderViewModel noteHeader)
        {
            SelectedNoteHeader = noteHeader;

            var noteRow = await _dataService.GetAsync<NoteRow>(x => x.NoteHeaderId == SelectedNoteHeader.Id);
            var noteRows = noteRow.Select(x => new NoteRowViewModel(x));
            _dispatcher.RunInMainThread(() => NoteRows = noteRows.ToObservableCollection());

            var linen = await _dataService.GetAsync<LinenList>(x => x.MasterLinen);
            _dispatcher.RunInMainThread(() => LinenLists = linen);

            var client = await _dataService.GetAsync<Client>(x => x.ClientInfo);
            _dispatcher.RunInMainThread(() => SelectedClient = client.FirstOrDefault(x=> x.Id == SelectedNoteHeader.ClientId));

            var department = await _dataService.GetAsync<Department>(x => x.Id == SelectedNoteHeader.DepartmentId);
            _dispatcher.RunInMainThread(() => SelectedDepartment = department.FirstOrDefault());

            var info = await _dataService.GetAsync<PrimeInfo>();
            _dispatcher.RunInMainThread(() => PrimeInfo = info.FirstOrDefault());

            foreach (var row in NoteRows)
            {
                row.LinenName = LinenLists.FirstOrDefault(x => x.Id == row.LinenListId)?.MasterLinen.Name;
            }

            NoteRows.ForEach(SubscribeItem);
            NoteRows.CollectionChanged += NoteRowsCollectionChanged;

            IsChanged = false;

            DeliveryTypes = EnumExtentions.GetValues<DeliveryTypeEnum>();
            ServiceTypes = EnumExtentions.GetValues<ServiceTypeEnum>();
        }

        public async void Save()
        {
            if (!HasChanges()) return;

            if (!_dialogService.ShowQuestionDialog($"Do you want to save all changes ? "))
                return;

            SelectedNoteHeader.AcceptChanges();
            await _dataService.AddOrUpdateAsync(SelectedNoteHeader.OriginalObject);

            NoteRows.ForEach(x => x.AcceptChanges());
            await _dataService.AddOrUpdateAsync(NoteRows.Select(x => x.OriginalObject));

            IsChanged = true;
            CloseAction?.Invoke(IsChanged);
        }

        public void Close()
        {
            if (_dialogService.ShowQuestionDialog($"Do you want to close window ? "))
                CloseAction?.Invoke(IsChanged);
        }

        public void Cancel()
        {
            if (!HasChanges()) return;

            if (!_dialogService.ShowQuestionDialog($"Do you want to cancel all changes? ")) return;

            SelectedNoteHeader.Reset();
            NoteRows.ForEach(x => x.Reset());
        }

        public void Print()
        {
            bool showPrice;

            _dialogService.ShowQuestionDialog("Do you want to show price?");
            {
                showPrice = true;
            }

            var report = GetNoteReport(showPrice);

            if (report == null) return;
            _reportService.ShowReportPreview(report);
        }

        private IReport GetNoteReport(bool priceVisibility)
        {
            var note = SelectedNoteHeader;
            var deliveryType = DeliveryTypes.FirstOrDefault(x => x.Id == note?.DeliveryTypeId);
            var report = new NoteReport
            {
                ClientAddress = SelectedClient.ClientInfo.Address,
                ClientName = SelectedClient.Name,
                Comment = note.Comment,
                DeliveryDate = note.DeliveryDate,
                DeliveryKg = note.DeliveryWeight,
                DeliveryType = deliveryType?.Name,
                DepartmentName = SelectedDepartment.Name,
                NoteNumber = note.Name,
                CollectionDate = note.CollectionDate,
                CollectionWeight = note.CollectionWeight,
                ClientLogo = Extension.GetBitmap(Extension.GetBitmapImage(SelectedClient.ClientInfo.Logo)),
                PrimeLogo = Extension.GetBitmap(Extension.GetBitmapImage(PrimeInfo.Logo)),
                ShowItemPrice = priceVisibility,
            };

            var reportItems = new List<NotesReportRowItem>();
            foreach (var notRow in NoteRows.Where(x => x.NoteHeaderId == note.Id))
            {
                if (notRow.PrimeCollectionQty == 0 && notRow.PrimeDeliveredQty == 0 && notRow.ClientReceivedQty == 0) continue;

                var serviceType = ServiceTypes.FirstOrDefault(x => x.Id == notRow.ServiceTypeId)?.Name;
                var item = new NotesReportRowItem()
                {
                    Id = notRow.LinenListId,
                    Name = notRow.LinenName,
                    Price = notRow.Price,
                    DeliveryQuantity = notRow.PrimeDeliveredQty,
                    CollectionQuantity = notRow.PrimeCollectionQty,
                    Remark = notRow.Comment,
                    ServiceType = serviceType,
                };

                item.ClientReceivedQuantity = notRow.ClientReceivedQty;

                if (report.ShowItemPrice)
                {
                    switch (note.DeliveryTypeId)
                    {
                        case (int)DeliveryTypeEnum.Express:
                            item.Price += item.Price * note.ExpressCharge;
                            break;
                        case (int)DeliveryTypeEnum.ReWash:
                            item.Price = 0;
                            break;
                    }
                }
                reportItems.Add(item);
            }

            report.ItemRows = reportItems.OrderBy(x => x.Name).ToList();
            return report;
        }

        public bool HasChanges()
        {
            return (SelectedNoteHeader.HasChanges() || NoteRows.Any(x => x.HasChanges()));
        }
    }
}
