using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.LinenModel;
using PALMS.Data.Objects.NoteModel;
using PALMS.Reports.Common;
using PALMS.Reports.Epplus.Model;
using PALMS.Reports.ViewModel.EntityViewModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;
using System.IO;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Objects.Received_data;


namespace PALMS.Reports.ViewModel.Windows
{
    public class CoordinateReportViewModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly IDataService _dataService;
        private readonly IResolver _resolverService;
        private readonly IDispatcher _dispatcher;
        private readonly IDialogService _dialogService;
        private readonly IExcelReportService _excelReportService;
        public Action<bool> CloseAction { get; set; }

        private DateTime _dateEnd;
        private DateTime _dateStart;
        private List<ClientViewModel> _clients;
        private List<Department> _departments;
        private List<LinenList> _linens;
        private List<MasterLinen> _masterLinens;
        private List<UnitReportViewModel> _departmentTypes;

        private List<UnitViewModel> _coordinateValueTypes;
        private List<UnitViewModel> _serviceTypes;
        private List<UnitViewModel> _deliveryTypes;

        private List<UnitReportViewModel> _selectedDepartmentTypes;
        private UnitViewModel _selectedX;
        private UnitViewModel _selectedY;
        private UnitViewModel _selectedZ;
        private UnitViewModel _selectedValueType;
        private UnitViewModel _selectedDeliveryType;
        private UnitViewModel _selectedServiceType;
        private List<NoteHeader> _noteHeaders;
        private List<NoteRow> _noteRows;
        private List<UnitViewModel> _laundryKgTypes;
        private UnitViewModel _selectedLaundryKgType;
        private List<LaundryKg> _laundryKg;


        public List<LaundryKg> LaundryKg
        {
            get => _laundryKg;
            set => Set(ref _laundryKg, value);
        }
        public UnitViewModel SelectedLaundryKgType
        {
            get => _selectedLaundryKgType;
            set => Set(ref _selectedLaundryKgType, value);
        }
        public List<UnitViewModel> LaundryKgTypes
        {
            get => _laundryKgTypes;
            set => Set(ref _laundryKgTypes, value);
        }
        public List<NoteRow> NoteRows
        {
            get => _noteRows;
            set => Set(ref _noteRows, value);
        }
        public List<NoteHeader> NoteHeaders
        {
            get => _noteHeaders;
            set => Set(ref _noteHeaders, value);
        }
        public UnitViewModel SelectedServiceType
        {
            get => _selectedServiceType;
            set => Set(ref _selectedServiceType, value);
        }
        public List<UnitReportViewModel> SelectedDepartmentTypes
        {
            get => _selectedDepartmentTypes;
            set => Set(ref _selectedDepartmentTypes, value);
        }
        public List<UnitReportViewModel> DepartmentTypes
        {
            get => _departmentTypes;
            set => Set(ref _departmentTypes, value);
        }
        public List<UnitViewModel> DeliveryTypes
        {
            get => _deliveryTypes;
            set => Set(ref _deliveryTypes, value);
        }
        public List<UnitViewModel> ServiceTypes
        {
            get => _serviceTypes;
            set => Set(ref _serviceTypes, value);
        }
        public List<UnitViewModel> CoordinateValueTypes
        {
            get => _coordinateValueTypes;
            set => Set(ref _coordinateValueTypes, value);
        }
        public List<MasterLinen> MasterLinens
        {
            get => _masterLinens;
            set => Set(ref _masterLinens, value);
        }
        public List<LinenList> Linens
        {
            get => _linens;
            set => Set(ref _linens, value);
        }
        public List<Department> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }
        public List<ClientViewModel> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }
        public DateTime DateStart
        {
            get => _dateStart;
            set => Set(ref _dateStart, value);
        }
        public DateTime DateEnd
        {
            get => _dateEnd;
            set => Set(ref _dateEnd, value);
        }
        public UnitViewModel SelectedValueType
        {
            get => _selectedValueType;
            set => Set(ref _selectedValueType, value);
        }
        public UnitViewModel SelectedX
        {
            get => _selectedX;
            set => Set(ref _selectedX, value);
        }
        public UnitViewModel SelectedY
        {
            get => _selectedY;
            set => Set(ref _selectedY, value);
        }
        public UnitViewModel SelectedZ
        {
            get => _selectedZ;
            set => Set(ref _selectedZ, value);
        }
        public UnitViewModel SelectedDeliveryType
        {
            get => _selectedDeliveryType;
            set => Set(ref _selectedDeliveryType, value);
        }

        public RelayCommand GetExcelCommand { get; }
        public RelayCommand GetLinenPriceCommand { get; }


        public List<UnitViewModel> SortedXCoordinate => SortXCoordinate();
        public List<UnitViewModel> SortedYCoordinate => SortYCoordinate();
        public List<UnitViewModel> SortedZCoordinate => SortZCoordinate();

        public CoordinateReportViewModel(IResolver resolver, IDataService dataService, IDispatcher dispatcher, IDialogService dialog, IExcelReportService excelService)
        {
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dialogService = dialog ?? throw new ArgumentNullException(nameof(dialog));
            _excelReportService = excelService ?? throw new ArgumentNullException(nameof(excelService));

            GetExcelCommand = new RelayCommand(SetExcel);
            GetLinenPriceCommand = new RelayCommand(GetLinensPrice);

            PropertyChanged += OnPropertyChanged;
        }

        public async Task InitializeAsync()
        {
            _dialogService.ShowBusy();
            try
            {
                var client = await _dataService.GetAsync<Client>(x => x.ClientInfo);
                var clients = client.Select(x => new ClientViewModel(x));
                _dispatcher.RunInMainThread(() => Clients = clients.OrderBy(x => x.ShortName).ToList());

                var department = await _dataService.GetAsync<Department>();
                _dispatcher.RunInMainThread(() => Departments = department.ToList());

                var linens = await _dataService.GetAsync<LinenList>(x => x.MasterLinen);
                var linenList = linens;
                _dispatcher.RunInMainThread(() => Linens = linenList);

                var kg = await _dataService.GetAsync<LaundryKg>();
                var kgs = kg;
                _dispatcher.RunInMainThread(() => LaundryKg = kgs);

                var masterLinen = await _dataService.GetAsync<MasterLinen>();
                _dispatcher.RunInMainThread(() => MasterLinens = masterLinen.ToList());

                var departmentType = EnumExtentions.GetValues<DepartmentTypeEnum>();
                DepartmentTypes = new List<UnitReportViewModel>();
                foreach (var depa in departmentType)
                {
                    var depType = new UnitReportViewModel(depa.Id, depa.Name);
                    SubscribeItem(depType);
                    DepartmentTypes.Add(depType);
                }

                var noteHeader = await _dataService.GetAsync<NoteHeader>(x => x.NoteRows);
                _dispatcher.RunInMainThread(() => NoteHeaders = noteHeader.ToList());

                ServiceTypes = EnumExtentions.GetValues<ServiceTypeEnum>();
                DeliveryTypes = EnumExtentions.GetValues<DeliveryTypeEnum>();
                LaundryKgTypes = EnumExtentions.GetValues<KgTypeEnum>();
                CoordinateValueTypes = new List<UnitViewModel>()
                {
                    new UnitViewModel((int)CoorTypeEnum.Pc,
                        CoorTypeEnum.Pc.ToString()),
                    new UnitViewModel((int)CoorTypeEnum.Price,
                        CoorTypeEnum.Price.ToString()),
                    new UnitViewModel((int)CoorTypeEnum.Weight,
                        CoorTypeEnum.Weight.ToString()),
                };

                DateEnd = DateTime.Now;
                DateStart = DateTime.Now;
                SelectedDeliveryType = null;
                SelectedServiceType = null;
                SelectedDepartmentTypes = DepartmentTypes.Where(x => x.IsSelected).ToList();
                SelectedValueType = CoordinateValueTypes.FirstOrDefault();
                SelectedLaundryKgType = LaundryKgTypes.FirstOrDefault();
            }

            catch (Exception ex)
            {
                _dialogService.HideBusy();
                Helper.RunInMainThread(() => _dialogService.ShowErrorDialog($"Initialization error: {ex.Message}"));
            }

            finally
            {
                _dialogService.HideBusy();
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedX))
            {
                RaisePropertyChanged(() => SortedYCoordinate);
            }
            if (e.PropertyName == nameof(SelectedY))
            {
                RaisePropertyChanged(() => SortedZCoordinate);
            }
            if (e.PropertyName == nameof(SelectedValueType))
            {
                RaisePropertyChanged(() => SortedZCoordinate);
                RaisePropertyChanged(() => SortedYCoordinate);
                RaisePropertyChanged(() => SortedXCoordinate);
            }

        }

        private void SubscribeItem(UnitReportViewModel item)
        {
            item.PropertyChanged += ItemOnPropertyChanged;
        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is UnitReportViewModel item)) return;
            SelectedDepartmentTypes = DepartmentTypes.Where(x => x.IsSelected).ToList();
        }

        #region Coordinates

        public List<UnitViewModel> SortXCoordinate()
        {
            var sortedXColumn = new List<UnitViewModel>()
            {
                new UnitViewModel((int) CoorTypeEnum.Day, CoorTypeEnum.Day.ToString()),
                new UnitViewModel((int) CoorTypeEnum.Month, CoorTypeEnum.Month.ToString()),
                new UnitViewModel((int) CoorTypeEnum.Year, CoorTypeEnum.Year.ToString()),
                new UnitViewModel((int) CoorTypeEnum.Client, CoorTypeEnum.Client.ToString()),
            };

            if (SelectedValueType.Id != (int) CoorTypeEnum.Weight)
            {
                sortedXColumn.Add(new UnitViewModel((int)CoorTypeEnum.Department, CoorTypeEnum.Department.ToString()));
            }

            return sortedXColumn;
        }
        public List<UnitViewModel> SortYCoordinate()
        {
            if (SelectedX == null) return null;

            return SortCoordinate(SelectedX.Id);
        }
        public List<UnitViewModel> SortZCoordinate()
        {
            if (SelectedY == null) return null;

            return SortCoordinate(SelectedY.Id);
        }
        public List<UnitViewModel> SortCoordinate(int col)
        {
            if (SelectedX == null)
                return null;
            var column = col;
            var sortedColumn = new List<UnitViewModel>();

            //Check if Selected Value Type is Weight
            if (SelectedValueType.Id == (int) CoorTypeEnum.Weight)
            {
                switch (column)
                {
                    case (int)CoorTypeEnum.Day:
                    case (int)CoorTypeEnum.Month:
                    case (int)CoorTypeEnum.Year:
                    {
                        sortedColumn.Add(new UnitViewModel((int)CoorTypeEnum.Client,
                            CoorTypeEnum.Client.ToString()));
                        sortedColumn.Add(new UnitViewModel((int)CoorTypeEnum.Department,
                            CoorTypeEnum.Department.ToString()));
                    }
                        break;

                    case (int)CoorTypeEnum.Client:
                    {
                        sortedColumn.Add(new UnitViewModel((int)CoorTypeEnum.Department,
                            CoorTypeEnum.Department.ToString()));
                    }
                        break;
                }
            }
            else
            {
                switch (column)
                {
                    case (int)CoorTypeEnum.Day:
                    case (int)CoorTypeEnum.Month:
                    case (int)CoorTypeEnum.Year:
                    {
                        sortedColumn.Add(new UnitViewModel((int)CoorTypeEnum.Client,
                            CoorTypeEnum.Client.ToString()));
                        sortedColumn.Add(new UnitViewModel((int)CoorTypeEnum.Department,
                            CoorTypeEnum.Department.ToString()));
                        sortedColumn.Add(new UnitViewModel((int)CoorTypeEnum.Linen,
                            CoorTypeEnum.Linen.ToString()));
                    }
                        break;

                    case (int)CoorTypeEnum.Client:
                    {
                        sortedColumn.Add(new UnitViewModel((int)CoorTypeEnum.Department,
                            CoorTypeEnum.Department.ToString()));
                        sortedColumn.Add(new UnitViewModel((int)CoorTypeEnum.Linen,
                            CoorTypeEnum.Linen.ToString()));
                    }
                        break;

                    case (int)CoorTypeEnum.Department:
                    {
                        sortedColumn.Add(new UnitViewModel((int)CoorTypeEnum.Linen,
                            CoorTypeEnum.Linen.ToString()));
                    }
                        break;
                }
            }


            return sortedColumn;
        }

        public string[] SetXData()
        {
            var horValues = new string[1];
            switch (SelectedX.Id)
            {
                case (int) CoorTypeEnum.Day:
                {
                    horValues = new string[DateEnd.DayOfYear - DateStart.DayOfYear + 1];

                    for (int i = 0; i < horValues.Length; i++)
                    {
                        horValues[i] = $"{DateStart.AddDays(i):dd/MMM/yyyy}";
                    }
                }
                    break;

                case (int) CoorTypeEnum.Month:
                {
                    horValues = new string[(DateEnd.Year - DateStart.Year) * 12 + DateEnd.Month - DateStart.Month + 1];

                    for (int i = 0; i < horValues.Length; i++)
                    {
                        horValues[i] = $"{DateStart.AddMonths(i):MMMM/yyyy}";
                    }
                }
                    break;

                case (int) CoorTypeEnum.Year:
                {
                    horValues = new string[DateEnd.Year - DateStart.Year + 1];

                    for (int i = 0; i < horValues.Length; i++)
                    {
                        horValues[i] = $"{DateStart.AddYears(i):yyyy}";
                    }
                }
                    break;

                case (int) CoorTypeEnum.Client:
                {
                    var clients = GetSelectedClients();
                    horValues = new string[clients.Length];

                    for (int i = 0; i < clients.Length; i++)
                    {
                        horValues[i] = clients[i].Name;
                    }
                }
                    break;

                case (int) CoorTypeEnum.Department:
                {
                    var count = SelectedDepartmentTypes.Count;
                    horValues = new string[count];

                    var i = 0;
                    foreach (var departmentType in SelectedDepartmentTypes.OrderBy(x => x.Name))
                    {
                        horValues[i] = departmentType.Name;
                        i++;
                    }
                }
                    break;
            }

            return horValues;
        }

        public List<CoordinateGroupData> SetExcelGroups(int count)
        {
            var period = count;
            var excelGroups = new List<CoordinateGroupData>();

            if (SelectedZ == null)
            {
                var notes = GetNoteHeaders();

                excelGroups.Add(new CoordinateGroupData()
                {
                    Name = SelectedY.Name,
                    VerticalData = SetExcelItems(period, notes)
                });
            }
            else
            {
                if (SelectedX.Id == (int)CoorTypeEnum.Day || SelectedX.Id == (int)CoorTypeEnum.Month ||
                    SelectedX.Id == (int)CoorTypeEnum.Year)
                {
                    if (SelectedY.Id == (int)CoorTypeEnum.Client)
                    {
                        var notes = GetNoteHeaders();
                        var clients = GetSelectedClients();

                        foreach (var client in clients)
                        {
                            excelGroups.Add(SetExcelGroupBase(period, notes.Where(x => x.ClientId == client.Id).ToList(), client.Name));
                        }
                    }

                    if (SelectedY.Id == (int)CoorTypeEnum.Department)
                    {
                        var notes = GetNoteHeaders();
                        var departmentTypes = SelectedDepartmentTypes;

                        foreach (var departmentType in departmentTypes)
                        {
                            var departmentNotes = new List<NoteHeader>();

                            foreach (var department in GetDepartments(0, departmentType.Id))
                            {
                                departmentNotes = notes.Where(x => x.DepartmentId == department.Id).ToList();
                            }
                            excelGroups.Add(SetExcelGroupBase(period, departmentNotes, departmentType.Name));
                        }
                    }
                }
                else
                {
                    if (SelectedY.Id == (int)CoorTypeEnum.Client)
                    {
                        var notes = GetNoteHeaders();
                        var clients = GetSelectedClients();

                        foreach (var client in clients)
                        {
                            excelGroups.Add(SetExcelGroupBase(period, notes.Where(x => x.ClientId == client.Id).ToList(), client.Name));
                        }
                    }

                    if (SelectedY.Id == (int)CoorTypeEnum.Department)
                    {
                        var notes = GetNoteHeaders();
                        var departmentTypes = SelectedDepartmentTypes;

                        foreach (var departmentType in departmentTypes)
                        {
                            var departmentNotes = new List<NoteHeader>();

                            foreach (var department in GetDepartments(0, departmentType.Id))
                            {
                                departmentNotes.AddRange(notes.Where(x => x.DepartmentId == department.Id).ToList());
                            }
                            excelGroups.Add(SetExcelGroupBase(period, departmentNotes, departmentType.Name));
                        }
                    }
                }
            }
            return excelGroups;
        }

        public CoordinateGroupData SetExcelGroupBase(int period, List<NoteHeader> notes, string name)
        {
            var excelGroup = new CoordinateGroupData()
            {
                Name = name,
                VerticalData = SetExcelItems(period, notes),
            };

            return excelGroup;
        }

        public List<CoordinateGroupItem> SetExcelItems(int count, List<NoteHeader> noteHeaders)
        {
            var excelItems = new List<CoordinateGroupItem>();
            var period = count;
            var notes = noteHeaders;

            if (SelectedZ == null)
            {
                // Date
                if (SelectedX.Id == (int)CoorTypeEnum.Day || SelectedX.Id == (int)CoorTypeEnum.Month || SelectedX.Id == (int)CoorTypeEnum.Year)
                {
                    if (SelectedY.Id == (int)CoorTypeEnum.Client)
                    {
                        var start = GetStartDate();
                        var clients = GetSelectedClients();

                        foreach (var client in clients.OrderBy(x => x.Name))
                        {
                            var item = new CoordinateGroupItem()
                            {
                                Id = client.Id,
                                Name = client.Name,
                                Values = new double[period]
                            };

                            for (int i = 0; i < period; i++)
                            {
                                var note = SortNoteHeadersByDate(notes, start, i)
                                    .Where(x => x.ClientId == client.Id);
                                item.Values[i] = SetExcelItemValues(note);
                            }
                            excelItems.Add(item);
                        }
                    }

                    if (SelectedY.Id == (int)CoorTypeEnum.Department)
                    {
                        var start = GetStartDate();

                        foreach (var departmentType in SelectedDepartmentTypes.OrderBy(x => x.Name))
                        {
                            var item = new CoordinateGroupItem()
                            {
                                Id = departmentType.Id,
                                Name = departmentType.Name,
                                Values = new double[period]
                            };
                            var sortedNotes = new List<NoteHeader>();

                            foreach (var department in GetDepartments(0, departmentType.Id))
                            {
                                sortedNotes.AddRange(notes.Where(x => x.DepartmentId == department.Id));
                            }

                            for (int i = 0; i < period; i++)
                            {
                                item.Values[i] = SetExcelItemValues(SortNoteHeadersByDate(sortedNotes, start, i));
                            }

                            excelItems.Add(item);
                        }
                    }

                    if (SelectedY.Id == (int)CoorTypeEnum.Linen)
                    {
                        var start = GetStartDate();

                        foreach (var masterLinen in MasterLinens.OrderBy(x => x.Name))
                        {
                            var checkValid = new double();
                            var item = new CoordinateGroupItem()
                            {
                                Id = masterLinen.Id,
                                Name = masterLinen.Name,
                                Values = new double[period]
                            };

                            for (int i = 0; i < period; i++)
                            {

                                item.Values[i] = SetExcelItemValues(SortNoteHeadersByDate(notes, start, i), masterLinen);
                                checkValid += item.Values[i];
                            }

                            if (checkValid > 0)
                            {
                                excelItems.Add(item);
                            }
                        }
                    }
                }

                // Client
                if (SelectedX.Id == (int)CoorTypeEnum.Client && SelectedY.Id == (int)CoorTypeEnum.Department)
                {
                    var clients = GetSelectedClients();
                    var departmentTypes = SelectedDepartmentTypes;

                    foreach (var depType in departmentTypes.OrderBy(x => x.Name))
                    {
                        var item = new CoordinateGroupItem()
                        {
                            Id = depType.Id,
                            Name = depType.Name,
                            Values = new double[period]
                        };

                        for (int i = 0; i < clients.Length; i++)
                        {
                            var note = notes.Where(x => x.ClientId == clients[i].Id).ToList();
                            item.Values[i] = SetExcelItemValues(note, depType.Id);
                        }

                        excelItems.Add(item);
                    }
                }
                if (SelectedX.Id == (int)CoorTypeEnum.Client && SelectedY.Id == (int)CoorTypeEnum.Linen)
                {
                    var clients = GetSelectedClients();

                    foreach (var masterLinen in MasterLinens.OrderBy(x => x.Name))
                    {
                        var checkValid = new double();
                        var item = new CoordinateGroupItem()
                        {
                            Id = masterLinen.Id,
                            Name = masterLinen.Name,
                            Values = new double[period]
                        };

                        for (int i = 0; i < period; i++)
                        {
                            item.Values[i] = SetExcelItemValues(notes.Where(x => x.ClientId == clients[i].Id).ToList(), masterLinen);
                            checkValid += item.Values[i];
                        }

                        if (checkValid > 0)
                        {
                            excelItems.Add(item);
                        }
                    }
                }

                // Department
                if (SelectedX.Id == (int)CoorTypeEnum.Department && SelectedY.Id == (int)CoorTypeEnum.Linen)
                {
                    var departmentTypes = SelectedDepartmentTypes;

                    foreach (var masterLinen in MasterLinens.OrderBy(x => x.Name))
                    {
                        var checkValid = new double();
                        var item = new CoordinateGroupItem()
                        {
                            Id = masterLinen.Id,
                            Name = masterLinen.Name,
                            Values = new double[period]
                        };

                        for (int i = 0; i < period; i++)
                        {
                            var departmentNotes = new List<NoteHeader>();

                            foreach (var dep in GetDepartments(0, departmentTypes[i].Id))
                            {
                                departmentNotes.AddRange(notes.Where(x => x.DepartmentId == dep.Id));
                            }

                            item.Values[i] = SetExcelItemValues(departmentNotes, masterLinen);
                            checkValid += item.Values[i];
                        }

                        if (checkValid > 0)
                        {
                            excelItems.Add(item);
                        }
                    }
                }

                return excelItems;
            }

            if (SelectedX.Id == (int)CoorTypeEnum.Day || SelectedX.Id == (int)CoorTypeEnum.Month || SelectedX.Id == (int)CoorTypeEnum.Year)
            {
                if (SelectedZ.Id == (int)CoorTypeEnum.Department)
                {
                    var start = GetStartDate();

                    foreach (var depType in SelectedDepartmentTypes)
                    {
                        var item = new CoordinateGroupItem()
                        {
                            Id = depType.Id,
                            Name = depType.Name,
                            Values = new double[period]
                        };

                        for (int i = 0; i < period; i++)
                        {
                            var note = SortNoteHeadersByDate(notes, start, i);
                            item.Values[i] = SetExcelItemValues(note, depType.Id);
                        }
                        excelItems.Add(item);
                    }
                }

                if (SelectedZ.Id == (int)CoorTypeEnum.Linen)
                {
                    var start = GetStartDate();

                    foreach (var masterLinen in MasterLinens.OrderBy(x => x.Name))
                    {
                        var checkValid = new double();
                        var item = new CoordinateGroupItem()
                        {
                            Id = masterLinen.Id,
                            Name = masterLinen.Name,
                            Values = new double[period]
                        };

                        for (int i = 0; i < period; i++)
                        {
                            item.Values[i] = SetExcelItemValues(SortNoteHeadersByDate(notes, start, i), masterLinen);
                            checkValid += item.Values[i];
                        }

                        if (checkValid > 0)
                        {
                            excelItems.Add(item);
                        }
                    }
                }
            }

            // Client
            if (SelectedX.Id == (int)CoorTypeEnum.Client && SelectedZ.Id == (int)CoorTypeEnum.Linen)
            {
                var clients = GetSelectedClients();

                foreach (var masterLinen in MasterLinens.OrderBy(x => x.Name))
                {
                    var checkValid = new double();
                    var item = new CoordinateGroupItem()
                    {
                        Id = masterLinen.Id,
                        Name = masterLinen.Name,
                        Values = new double[period]
                    };

                    for (int i = 0; i < period; i++)
                    {
                        item.Values[i] = SetExcelItemValues(notes.Where(x => x.ClientId == clients[i].Id).ToList(),
                            masterLinen);
                        checkValid += item.Values[i];
                    }

                    if (checkValid > 0)
                    {
                        excelItems.Add(item);
                    }
                }
            }

            // Department
            if (SelectedX.Id == (int)CoorTypeEnum.Department && SelectedY.Id == (int)CoorTypeEnum.Linen)
            {
                var departmentTypes = SelectedDepartmentTypes;

                foreach (var masterLinen in MasterLinens.OrderBy(x => x.Name))
                {
                    var checkValid = new double();
                    var item = new CoordinateGroupItem()
                    {
                        Id = masterLinen.Id,
                        Name = masterLinen.Name,
                        Values = new double[period]
                    };

                    for (int i = 0; i < period; i++)
                    {
                        var departmentNotes = new List<NoteHeader>();

                        foreach (var dep in GetDepartments(0, departmentTypes[i].Id))
                        {
                            departmentNotes.AddRange(notes.Where(x => x.DepartmentId == dep.Id));
                        }

                        item.Values[i] = SetExcelItemValues(departmentNotes, masterLinen);
                        checkValid += item.Values[i];
                    }

                    if (checkValid > 0)
                    {
                        excelItems.Add(item);
                    }
                }
            }

            return excelItems;
        }


        public double SetExcelItemValues(List<NoteHeader> noteHeaders, MasterLinen masterLinen)
        {
            var value = new double();
            var notes = noteHeaders;

            foreach (var linenList in Linens.Where(x => x.MasterLinenId == masterLinen.Id))
            {
                foreach (var note in notes)
                {
                    foreach (var noteRow in note.NoteRows.Where(x => x.LinenListId == linenList.Id))
                    {
                        value += GetValue(noteRow);
                    }
                }
            }

            return value;
        }

        public double SetExcelItemValues(List<NoteHeader> noteHeaders, int departmentType)
        {
            var sortedNotes = new List<NoteHeader>();

            foreach (var department in GetDepartments(0, departmentType))
            {
                sortedNotes.AddRange(noteHeaders.Where(x => x.DepartmentId == department.Id));
            }

            return SetExcelItemValues(sortedNotes);
        }

        public double SetExcelItemValues(IEnumerable<NoteHeader> notes)
        {
            var noteCoor = GetNoteHeadersCoorData(notes);

            var value = new double();
            noteCoor.ForEach(x => value += x.Value);
            return value;
        }

        #endregion

        #region Get base DATA

        public Client[] GetSelectedClients()
        {
            var clients = new Client[Clients.Count(x => x.IsSelected)];
            var i = 0;
            foreach (var client in Clients.Where(x => x.IsSelected).OrderBy(x => x.Name))
            {
                clients[i] = client.OriginalObject;
                i++;
            }
            return clients;
        }

        public DateTime GetStartDate()
        {
            DateTime startDate;

            startDate = new DateTime(DateStart.Year, DateStart.Month, DateStart.Day,
                DateTime.MinValue.Hour, DateTime.MinValue.Minute, DateTime.MinValue.Second);

            //switch (SelectedX.Id)
            //{
            //    //case (int)CoorTypeEnum.Day:
            //    //    startDate = new DateTime(DateStart.Year, DateStart.Month, DateStart.Day,
            //    //        DateTime.MinValue.Hour, DateTime.MinValue.Minute, DateTime.MinValue.Second);
            //    //    break;

            //    case (int)CoorTypeEnum.Month:
            //        startDate = new DateTime(DateStart.Year, DateStart.Month, DateTime.MinValue.Day,
            //            DateTime.MinValue.Hour, DateTime.MinValue.Minute, DateTime.MinValue.Second);
            //        break;

            //    case (int)CoorTypeEnum.Year:
            //        startDate = new DateTime(DateStart.Year, DateTime.MinValue.Month, DateTime.MinValue.Day,
            //            DateTime.MinValue.Hour, DateTime.MinValue.Minute, DateTime.MinValue.Second);
            //        break;

            //    default:
            //        startDate = new DateTime(DateStart.Year, DateStart.Month, DateStart.Day,
            //            DateTime.MinValue.Hour, DateTime.MinValue.Minute, DateTime.MinValue.Second);
            //        break;
            //}
            return startDate;
        }

        public DateTime GetEndDate()
        {
            DateTime endDate;

            endDate = new DateTime(DateEnd.Year, DateEnd.Month, DateEnd.Day,
                DateTime.MaxValue.Hour, DateTime.MaxValue.Minute, DateTime.MaxValue.Second);
            //switch (SelectedX.Id)
            //{
            //    //case (int)CoorTypeEnum.Day:
            //    //    endDate = new DateTime(DateEnd.Year, DateEnd.Month, DateEnd.Day,
            //    //        DateTime.MaxValue.Hour, DateTime.MaxValue.Minute, DateTime.MaxValue.Second);
            //    //    break;

            //    case (int)CoorTypeEnum.Month:
            //        endDate = new DateTime(DateEnd.Year, DateEnd.Month, DateTime.DaysInMonth(DateEnd.Year, DateEnd.Month),
            //            DateTime.MaxValue.Hour, DateTime.MaxValue.Minute, DateTime.MaxValue.Second);
            //        break;

            //    case (int)CoorTypeEnum.Year:
            //        endDate = new DateTime(DateEnd.Year, DateTime.MaxValue.Month, DateTime.DaysInMonth(DateEnd.Year, DateTime.MaxValue.Month),
            //            DateTime.MaxValue.Hour, DateTime.MaxValue.Minute, DateTime.MaxValue.Second);
            //        break;

            //    default:
            //        endDate = new DateTime(DateEnd.Year, DateEnd.Month, DateEnd.Day,
            //            DateTime.MaxValue.Hour, DateTime.MaxValue.Minute, DateTime.MaxValue.Second);
            //        break;
            //}
            return endDate;
        }

        public List<Department> GetDepartments(int clientId, int departmentTypeId)
        {
            var clients = GetSelectedClients();
            var departments = new List<Department>();

            foreach (var client in clients)
            {
                foreach (var selectedDepartmentType in SelectedDepartmentTypes)
                {
                    departments.AddRange(Departments.Where(x => x.DepartmentType == selectedDepartmentType.Id && x.ClientId == client.Id));
                }
            }

            if (departmentTypeId > 0 && clientId > 0)
            {
                return departments.Where(x => x.ClientId == clientId && x.DepartmentType == departmentTypeId).ToList();
            }

            if (departmentTypeId == 0 && clientId > 0)
            {
                return departments.Where(x => x.ClientId == clientId).ToList();
            }

            if (departmentTypeId > 0 && clientId == 0)
            {
                return departments.Where(x => x.DepartmentType == departmentTypeId).ToList();
            }

            return departments;
        }

        public List<NoteHeader> GetNoteHeaders()
        {
            var departments = GetDepartments(0, 0);
            var noteHeaders = new List<NoteHeader>();

            foreach (var department in departments)
            {
                if (SelectedDeliveryType == null)
                {
                    noteHeaders.AddRange(NoteHeaders.Where(x =>
                        x.DepartmentId == department.Id &&
                        x.DeliveredDate >= GetStartDate() &&
                        x.DeliveredDate <= GetEndDate()));
                }
                else
                {
                    noteHeaders.AddRange(NoteHeaders.Where(x =>
                        x.DepartmentId == department.Id &&
                        x.DeliveryTypeId == SelectedDeliveryType.Id &&
                        x.DeliveredDate >= GetStartDate() &&
                        x.DeliveredDate <= GetEndDate()));
                }
            }

            NoteRows = new List<NoteRow>();
            foreach (var noteHeader in noteHeaders)
            {
                NoteRows.AddRange(noteHeader.NoteRows);
            }

            return noteHeaders;
        }

        public List<NoteHeader> SortNoteHeadersByDate(List<NoteHeader> noteHeaders, DateTime start, int i)
        {
            var notes = new List<NoteHeader>();
            DateTime date;

            switch (SelectedX.Id)
            {
                case (int)CoorTypeEnum.Day:
                    date = start.AddDays(i);

                    notes = noteHeaders.Where(x => x.DeliveredDate >= date &&
                                                x.DeliveredDate <= new DateTime(date.Year, date.Month,
                                                    date.Day, DateTime.MaxValue.Hour, DateTime.MaxValue.Minute,
                                                    DateTime.MaxValue.Second)).ToList();
                    break;

                case (int)CoorTypeEnum.Month:
                    date = start.AddMonths(i);

                    notes = noteHeaders.Where(x => x.DeliveredDate >= date &&
                                                   x.DeliveredDate <= new DateTime(date.Year, date.Month,
                                                       DateTime.DaysInMonth(date.Year, date.Month),
                                                       DateTime.MaxValue.Hour, DateTime.MaxValue.Minute,
                                                       DateTime.MaxValue.Second)).ToList();
                    break;

                case (int)CoorTypeEnum.Year:
                    date = start.AddYears(i);
                    notes = noteHeaders.Where(x => x.DeliveredDate >= date &&
                                                   x.DeliveredDate <= new DateTime(date.Year, DateTime.MaxValue.Month,
                                                       DateTime.DaysInMonth(date.Year, DateTime.MaxValue.Month),
                                                       DateTime.MaxValue.Hour, DateTime.MaxValue.Minute,
                                                       DateTime.MaxValue.Second)).ToList();
                    break;
            }

            return notes;
        }

        public double GetValue(NoteRow noteRow)
        {
            if (!CheckRowServiceType(noteRow))
            {
                return 0;
            }

            var value = new double();

            switch (SelectedValueType.Id)
            {
                //case (int)CoorTypeEnum.Weight:
                //    value = noteRow.Weight * noteRow.ClientReceivedQty;
                //    break;

                case (int)CoorTypeEnum.Pc:
                    value = noteRow.ClientReceivedQty;
                    break;

                case (int)CoorTypeEnum.Price:
                    value = noteRow.ClientReceivedQty * noteRow.Price;
                    break;
            }

            return value;
        }

        public bool CheckRowServiceType(NoteRow noteRow)
        {
            if (SelectedServiceType == null)
            {
                return true;
            }

            return noteRow.ServiceTypeId == SelectedServiceType.Id;
        }

        public NoteHeaderCoordinate GetNoteHeaderCoorData(NoteHeader note)
        {
            if (note == null) return null;

            var newItem = new NoteHeaderCoordinate(note);

            foreach (var noteRow in note.NoteRows)
            {
                newItem.Value += GetValue(noteRow);
            }

            if (SelectedValueType.Id == (int)CoorTypeEnum.Price)
            {
                var client = Clients
                    .FirstOrDefault(x => x.Id == newItem.OriginalObject.ClientId);

                newItem.Value += note.DeliveryWeight * client.OriginalObject.ClientInfo.WeighPrice;

                switch (note.DeliveryTypeId)
                {
                    case (int)DeliveryTypeEnum.Express:
                        newItem.Value += newItem.Value * note.ExpressCharge;
                        break;

                    case (int)DeliveryTypeEnum.ReWash:
                        newItem.Value = 0;
                        break;
                }
            }

            return newItem;
        }

        public List<NoteHeaderCoordinate> GetNoteHeadersCoorData(IEnumerable<NoteHeader> notes)
        {
            var newItems = new List<NoteHeaderCoordinate>();

            foreach (var note in notes)
            {
                newItems.Add(GetNoteHeaderCoorData(note));
            }
            return newItems;
        }


        #endregion


        public string[] SetHorizontalData()
        {
            var horizontal = SetXData();

            return horizontal;
        }

        public List<CoordinateGroupData> SetVerticalData()
        {
            if (SelectedValueType.Id == (int) CoorTypeEnum.Weight)
                return (GetVerticalKgData(SetXData().Length));
            return SetExcelGroups(SetXData().Length);
        }

        public void SetExcel()
        {
            if (SelectedX == null || SelectedY == null)
                return;
            if (DateEnd < DateStart)
            {
                _dialogService.ShowInfoDialog("Please enter proper dates");
                return;
            }

            var horizontal = SetHorizontalData();
            var vertical = SetVerticalData();

            var data = new ExcelData()
            {
                Name = $"{SelectedX.Name} by {SelectedY.Name} {SelectedValueType.Name} report",
                PrintDate = DateTime.Now,
                ReportType = ReportType.SimpleExcel,
                HorizontalData = horizontal,
                VerticalData = vertical,
            };
            SaveExcel(data);
        }

        public void GetLinensPrice()
        {
            if (!GetSelectedClients().Any() && !SelectedDepartmentTypes.Any())
                return;
            var clients = GetSelectedClients();
            var horizontal = new string[clients.Length];
            var vertical = new List<CoordinateGroupData>();

            for (int i = 0; i < clients.Length; i++)
            {
                horizontal[i] = clients[i].Name;
            }

            foreach (var depType in SelectedDepartmentTypes.OrderBy(x => x.Name))
            {
                var excelGroup = new CoordinateGroupData()
                {
                    Name = depType.Name,
                    VerticalData = new List<CoordinateGroupItem>(),
                };

                var verticalData = new List<CoordinateGroupItem>();
                foreach (var masterLinen in MasterLinens)
                {
                    var item = GetMasterLinenExcelValues(depType.Id, masterLinen);

                    if (item == null) continue;
                    verticalData.Add(item);
                }

                excelGroup.VerticalData = verticalData.OrderBy(x => x.Name).ToList();
                vertical.Add(excelGroup);
            }

            var data = new ExcelData()
            {
                Name = $"Linen price report",
                PrintDate = DateTime.Now.ToLocalTime(),
                ReportType = ReportType.SimpleExcel,
                HorizontalData = horizontal,
                VerticalData = vertical,
            };
            SaveExcel(data);
        }

        public CoordinateGroupItem GetMasterLinenExcelValues(int depTypeId, MasterLinen masterLinen)
        {
            var clients = GetSelectedClients();
            var linens = new List<LinenList>();
            var departments = GetDepartments(0, depTypeId).Where(x => x.ParentId == null).ToList();
            var checkValid = new double();

            if (!departments.Any() || departments == null)
            {
                return null;
            }

            var item = new CoordinateGroupItem()
            {
                Id = masterLinen.Id,
                Name = masterLinen.Name,
                Values = new double[clients.Length],
            };

            foreach (var department in departments)
            {
                linens.AddRange(Linens.Where(x => x.DepartmentId == department.Id && x.MasterLinenId == masterLinen.Id));
            }


            if (!linens.Any() || linens == null)
            {
                return null;
            }

            for (int i = 0; i < clients.Length; i++)
            {
                var lin = linens.FirstOrDefault(x => x.ClientId == clients[i].Id);
                if (lin == null) continue;

                item.Values[i] = lin.Laundry;
            }

            foreach (var itemValue in item.Values)
            {
                checkValid += itemValue;
            }

            return checkValid > 0 ? item : null;
        }

#region KG Report

        private List<CoordinateGroupData> GetVerticalKgData(int cor)
        {
            var period = cor;
            var excelGroups = new List<CoordinateGroupData>();
            var laundryKg = SortLaundryKG();

            if (SelectedZ == null)
            {
                if (SelectedX.Id == (int) CoorTypeEnum.Day || SelectedX.Id == (int) CoorTypeEnum.Month ||
                    SelectedX.Id == (int) CoorTypeEnum.Year)
                {
                    if (SelectedY.Id == (int) CoorTypeEnum.Client || SelectedY.Id == (int)CoorTypeEnum.Department)
                    {
                        excelGroups.Add(new CoordinateGroupData()
                        {
                            Name = SelectedY.Name,
                            VerticalData = SetVerticalWeightData(laundryKg, period),
                        });
                       
                    }
                }

                if (SelectedX.Id == (int) CoorTypeEnum.Client)
                {
                    excelGroups.Add(new CoordinateGroupData()
                    {
                        Name = SelectedY.Name,
                        VerticalData = SetVerticalWeightData(laundryKg, period),
                    });
                }

            }
            else
            {
                if (SelectedX.Id == (int)CoorTypeEnum.Day || SelectedX.Id == (int)CoorTypeEnum.Month ||
                    SelectedX.Id == (int)CoorTypeEnum.Year)
                {
                    if (SelectedY.Id == (int)CoorTypeEnum.Client)
                    {
                        var clients = GetSelectedClients();

                        foreach (var client in clients.OrderBy(x=> x.ShortName))
                        {
                            excelGroups.Add(new CoordinateGroupData()
                            {
                                Name = client.ShortName,
                                VerticalData = SetVerticalWeightDataZ(laundryKg.Where(x=> x.ClientId == client.Id).ToList(), period),
                            });
                        }
                    }
                }
                
            }
            return excelGroups;
        }

        private List<CoordinateGroupItem> SetVerticalWeightDataZ(List<LaundryKg> laundryKg, int period)
        {
            var items = new List<CoordinateGroupItem>();
            var start = GetStartDate();
            var departments = SelectedDepartmentTypes;
            var id = 1;

            foreach (var department in departments)
            {
                var item = new CoordinateGroupItem()
                {
                    Id = id++,
                    Name = department.Name,
                    Values = new double[period]
                };

                for (int i = 0; i < period; i++)
                {
                    item.Values[i] = GetExcelWeightValue(SortLaundryKgByDate(laundryKg, start, i), department.Id);
                }

                items.Add(item);
            }


            return items;
        }
        private List<CoordinateGroupItem> SetVerticalWeightData(List<LaundryKg> laundryKg, int period)
        {
            var items =new List<CoordinateGroupItem>();
            var start = GetStartDate();
            var clients = GetSelectedClients();
            var departments = SelectedDepartmentTypes;
            var id = 1;

            if (SelectedX.Id == (int) CoorTypeEnum.Client)
            {
                foreach (var department in departments)
                {
                    var item = new CoordinateGroupItem()
                    {
                        Id = id++,
                        Name = department.Name,
                        Values = new double[period]
                    };

                    for (int i = 0; i < period; i++)
                    {
                        item.Values[i] = GetExcelWeightValue(laundryKg.Where(x=> x.ClientId == clients[i].Id).ToList(), department.Id);
                    }

                    items.Add(item);
                }
            }

            if (SelectedX.Id == (int) CoorTypeEnum.Day || SelectedX.Id == (int) CoorTypeEnum.Month ||
                SelectedX.Id == (int) CoorTypeEnum.Year)
            {
                if (SelectedY.Id == (int)CoorTypeEnum.Client)
                {
                    foreach (var client in clients)
                    {
                        var item = new CoordinateGroupItem()
                        {
                            Id = id++,
                            Name = client.ShortName,
                            Values = new double[period]
                        };

                        var clientKg = laundryKg.Where(x => x.ClientId == client.Id).ToList();
                        for (int i = 0; i < period; i++)
                        {
                            item.Values[i] = GetExcelWeightValue(SortLaundryKgByDate(clientKg, start, i), null);
                        }
                        items.Add(item);
                    }
                }

                if (SelectedY.Id == (int)CoorTypeEnum.Department)
                {
                    foreach (var department in departments)
                    {
                        var item = new CoordinateGroupItem()
                        {
                            Id = id++,
                            Name = department.Name,
                            Values = new double[period]
                        };

                        for (int i = 0; i < period; i++)
                        {
                            item.Values[i] = GetExcelWeightValue(SortLaundryKgByDate(laundryKg, start, i), department.Id);
                        }

                        items.Add(item);
                    }
                }
            }

            return items;
        }

      private double GetExcelWeightValue(List<LaundryKg> laundryKgs, int? depId)
        {
            var value = new double();
            var departments = SelectedDepartmentTypes;

            if (!depId.HasValue)
            {
                foreach (var laundryKg in laundryKgs)
                {
                    if (departments.Any(x => x.Id == (int)DepartmentTypeEnum.Linen))
                    {
                        value += laundryKg.ExtLinen;
                        value += laundryKg.Tunnel1;
                        value += laundryKg.Tunnel2;
                    }

                    if (departments.Any(x => x.Id == (int)DepartmentTypeEnum.FnB))
                    {
                        value += laundryKg.ExtFnB;
                    }

                    if (departments.Any(x => x.Id == (int)DepartmentTypeEnum.GuestLaundry))
                    {
                        value += laundryKg.ExtGuest;
                    }

                    if (departments.Any(x => x.Id == (int)DepartmentTypeEnum.Management))
                    {
                        value += laundryKg.ExtManager;
                    }

                    if (departments.Any(x => x.Id == (int)DepartmentTypeEnum.Uniform))
                    {
                        value += laundryKg.ExtUniform;
                    }
                }

            }
            else
            {
                foreach (var laundryKg in laundryKgs)
                {
                    switch (depId)
                    {
                        case (int) DepartmentTypeEnum.Linen:
                            value += laundryKg.ExtLinen;
                            value += laundryKg.Tunnel1;
                            value += laundryKg.Tunnel2;
                            break;
                        case (int) DepartmentTypeEnum.FnB:
                            value += laundryKg.ExtFnB;
                            break;
                        case (int) DepartmentTypeEnum.GuestLaundry:
                            value += laundryKg.ExtGuest;
                            break;
                        case (int) DepartmentTypeEnum.Management:
                            value += laundryKg.ExtManager;
                            break;
                        case (int) DepartmentTypeEnum.Uniform:
                            value += laundryKg.ExtUniform;
                            break;
                    }
                }
            }

            return value;
        }

        public List<LaundryKg> SortLaundryKgByDate(List<LaundryKg> laundryKgs, DateTime start, int i)
        {
            var kgs = new List<LaundryKg>();
            DateTime date;

            switch (SelectedX.Id)
            {
                case (int)CoorTypeEnum.Day:
                    date = start.AddDays(i);

                    kgs = laundryKgs.Where(x => x.WashingDate >= date &&
                                                x.WashingDate <= new DateTime(date.Year, date.Month,
                                                    date.Day, DateTime.MaxValue.Hour, DateTime.MaxValue.Minute,
                                                    DateTime.MaxValue.Second)).ToList();
                    break;

                case (int)CoorTypeEnum.Month:
                    date = start.AddMonths(i);

                    kgs = laundryKgs.Where(x => x.WashingDate >= date &&
                                                   x.WashingDate <= new DateTime(date.Year, date.Month,
                                                       DateTime.DaysInMonth(date.Year, date.Month),
                                                       DateTime.MaxValue.Hour, DateTime.MaxValue.Minute,
                                                       DateTime.MaxValue.Second)).ToList();
                    break;

                case (int)CoorTypeEnum.Year:
                    date = start.AddYears(i);
                    kgs = laundryKgs.Where(x => x.WashingDate >= date &&
                                                   x.WashingDate <= new DateTime(date.Year, DateTime.MaxValue.Month,
                                                       DateTime.DaysInMonth(date.Year, DateTime.MaxValue.Month),
                                                       DateTime.MaxValue.Hour, DateTime.MaxValue.Minute,
                                                       DateTime.MaxValue.Second)).ToList();
                    break;
            }

            return kgs;
        }


        private List<LaundryKg> SortLaundryKG()
        {
            if (SelectedLaundryKgType.Id == (int) KgTypeEnum.Clean)
            {
                return LaundryKg.Where(x =>
                        x.WashingDate <= DateEnd && x.WashingDate >= DateStart && x.KgTypeId == (int) KgTypeEnum.Clean)
                    .ToList();
            }
            return LaundryKg.Where(x =>
                    x.WashingDate <= DateEnd && x.WashingDate >= DateStart && x.KgTypeId == (int)KgTypeEnum.Soiled)
                .ToList();
        }



#endregion


        private void SaveExcel(ExcelData data)
        {
            if (data == null) return;

            var savePath = _dialogService.ShowSaveFileDialog("Excel file (*.xlsx)|*.xlsx",
                $"{data.Name}_{DateTime.Now:yyyyddMMHHmm}");
            if (string.IsNullOrEmpty(savePath)) return;

            _excelReportService.SaveAsAsync(data, savePath);

            var fileInfo = new FileInfo(savePath);
            _dialogService.ShowInfoDialog($"Report saved{Environment.NewLine}{Environment.NewLine}{fileInfo.Name}");
        }
    }
}
