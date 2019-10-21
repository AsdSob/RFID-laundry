using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.LinenModel;
using PALMS.Data.Objects.NoteModel;
using PALMS.Notes.ViewModel.EntityViewModel;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Notes.ViewModel
{
    public class NoteCommonMethods : ViewModelBase, IClear
    {
        private ObservableCollection<NoteRowViewModel> _noteRows;
        private readonly IDispatcher _dispatcher;
        private readonly IDataService _dataService;
        private ObservableCollection<NoteHeaderViewModel> _noteHeaders;
        private List<Client> _clients;
        private List<LinenList> _linenLists;
        private List<UnitViewModel> _washTypes;
        private ObservableCollection<NoteTreeItemViewModel> _noteTreeItems;
        private List<UnitViewModel> _deliveryTypes;
        private List<DepartmentViewModel> _departments;
        private ObservableCollection<NoteTreeItemViewModel> _departmentTreeItems;
        private PrimeInfo _primeInfo;
        private List<UnitViewModel> _noteStatus;
        private NoteHeaderViewModel _newNoteHeader;
        private int _selectedNoteStatus;


        public int SelectedNoteStatus
        {
            get => _selectedNoteStatus;
            set => Set(ref _selectedNoteStatus, value);
        }
        public NoteHeaderViewModel NewNoteHeader
        {
            get => _newNoteHeader;
            set => Set(ref _newNoteHeader, value);
        }
        public List<UnitViewModel> NoteStatus
        {
            get => _noteStatus;
            set => Set(ref _noteStatus, value);
        }
        public PrimeInfo PrimeInfo
        {
            get => _primeInfo;
            set => Set(ref _primeInfo, value);
        }
        public List<DepartmentViewModel> Departments
        {
            get => _departments;
            set => Set(ref _departments, value);
        }
        public List<UnitViewModel> DeliveryTypes
        {
            get => _deliveryTypes;
            set => Set(ref _deliveryTypes, value);
        }
        public List<UnitViewModel> WashTypes
        {
            get => _washTypes;
            set => Set(ref _washTypes, value);
        }
        public List<LinenList> LinenLists
        {
            get => _linenLists;
            set => Set(ref _linenLists, value);
        }
        public List<Client> Clients
        {
            get => _clients;
            set => Set(ref _clients, value);
        }
        public ObservableCollection<NoteHeaderViewModel> NoteHeaders
        {
            get => _noteHeaders;
            set => Set(ref _noteHeaders, value);
        }
        public ObservableCollection<NoteRowViewModel> NoteRows
        {
            get => _noteRows;
            set => Set(ref _noteRows, value);
        }
        public ObservableCollection<NoteTreeItemViewModel> NoteTreeItems
        {
            get => _noteTreeItems;
            set => Set(ref _noteTreeItems, value);
        }

        public ObservableCollection<NoteTreeItemViewModel> DepartmentTreeItems
        {
            get => _departmentTreeItems;
            set => Set(ref _departmentTreeItems, value);
        }

        public NoteCommonMethods(IDataService dataService, IDispatcher dispatcher)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public async Task InitializeAsync()
        {
            if (Clients != null) return;

            await GetNoteHeaders(1);

            DeliveryTypes = EnumExtentions.GetValues<DeliveryTypeEnum>();
            WashTypes = EnumExtentions.GetValues<ServiceTypeEnum>();
            NoteStatus = EnumExtentions.GetValues<NoteStatusEnum>();
        }

        public async Task GetNoteHeaders(int? status)
        {
            if (status == null) return;

            NoteRows = new ObservableCollection<NoteRowViewModel>();
            NoteHeaders = new ObservableCollection<NoteHeaderViewModel>();
            IEnumerable<NoteHeaderViewModel> noteHeaders;

            if (status == 0)
            {
                var noteHeader = await _dataService.GetAsync<NoteHeader>(x =>
                    x.InvoiceId == null);
                noteHeaders = noteHeader.Select(x => new NoteHeaderViewModel(x));
            }
            else
            {
                var noteHeader = await _dataService.GetAsync<NoteHeader>(x =>
                    x.NoteStatus == status && x.InvoiceId == null);
                noteHeaders = noteHeader.Select(x => new NoteHeaderViewModel(x));
            }

            _dispatcher.RunInMainThread(() => NoteHeaders = noteHeaders.ToObservableCollection());
            NoteHeaders.ForEach(x=> x.SetName());

            var noteRow = await _dataService.GetAsync<NoteRow>(x => x.LinenList.MasterLinen);
            var noteRows = noteRow.Select(x => new NoteRowViewModel(x)
            {
                LinenName = x.LinenList.MasterLinen.Name,
            });
            _dispatcher.RunInMainThread(() => NoteRows = noteRows.ToObservableCollection());
        }

        public ObservableCollection<NoteTreeItemViewModel> GetTreeItems()
        {
            var treeItems = new ObservableCollection<NoteTreeItemViewModel>();
            NoteTreeInitialize();

            if (NoteTreeItems != null)
            {
                treeItems = NoteTreeItems;
            }

            return treeItems;
        }

        public void NoteTreeInitialize()
        {
            NoteTreeItems = new ObservableCollection<NoteTreeItemViewModel>();
            DepartmentTreeItems = new ObservableCollection<NoteTreeItemViewModel>();

            InitializeTreeNotes();
        }

        public void InitializeTreeDepartments()
        {
            Debug.WriteLine("text Tree");

            if (Departments == null) return;

            var items = new ObservableCollection<NoteTreeItemViewModel>();
            foreach (var departmet in Departments)
            {
                var originalDepartment = new NoteTreeItemViewModel(departmet.OriginalObject)
                {
                    Id = departmet.Id,
                    ParentId = departmet.ParentId,
                    ClientId = departmet.ClientId,
                };
                items.Add(originalDepartment);
            }

            DepartmentTreeItems = items;
        }

        public void InitializeTreeNotes()
        {
            InitializeTreeDepartments();
            NoteTreeItems.AddRange(DepartmentTreeItems);

            var treeNoteItems = new ObservableCollection<NoteTreeItemViewModel>();

            var id = 0;

            if (NoteTreeItems.Count !=0)
            {
                id =NoteTreeItems.Max(x=> x.Id);
            }

            foreach (var note in NoteHeaders)
            {
                var originalNotes = new NoteTreeItemViewModel(note.OriginalObject)
                {
                    Id = ++id,
                    ParentId = note.DepartmentId,
                    CollectionDate = note.CollectionDate.ToString(DateExtension.DateFormat),
                    DeliveryDate = note.DeliveryDate.ToString(DateExtension.DateFormat),
                    CollectionWeight = note.CollectionWeight + " Kg",
                    DeliveryWeight = note.DeliveryWeight + " Kg",
                    ClientId = note.ClientId,
                    Name = note.Name,
                    DepartmentId = note.DepartmentId,
                    DeliveryTypeId = note.DeliveryTypeId,
                    ExpressCharge = note.ExpressCharge,
                };

                treeNoteItems.Add(originalNotes);
            }

            NoteTreeItems.AddRange(treeNoteItems);
        }


        public void Save(NoteHeaderViewModel noteHeader, ObservableCollection<NoteRowViewModel> noteRows)
        {
            SaveNoteHeader(noteHeader);
            SaveNoteRows(noteRows);
        }

        public async void SaveNoteRows(ObservableCollection<NoteRowViewModel> noteRows)
        {
            if(noteRows == null || noteRows.Count < 1) return;

            noteRows.ForEach(x => x.AcceptChanges());
            foreach (var noteRow in noteRows)
            {
                await _dataService.AddOrUpdateAsync(noteRow.OriginalObject);
            }
        }

        public async void SaveNoteHeader(NoteHeaderViewModel noteHeader)
        {
            if(noteHeader == null) return;

            noteHeader.AcceptChanges();
            await _dataService.AddOrUpdateAsync(noteHeader.OriginalObject);
        }

        public async Task<NoteHeaderViewModel> GetNewNote(NoteHeaderViewModel noteHeader)
        {
            if (noteHeader != null)
            {
                noteHeader.AcceptChanges();
                await _dataService.AddOrUpdateAsync(noteHeader.OriginalObject);

                NewNoteHeader = new NoteHeaderViewModel(noteHeader.OriginalObject);

            }
            return NewNoteHeader;
        }

        public async void Delete(NoteHeaderViewModel noteHeader, ObservableCollection<NoteRowViewModel> noteRows)
        {
            if (noteHeader == null) return;
            await _dataService.DeleteAsync(noteHeader.OriginalObject);

            if (noteRows.Count == 0) return;
            foreach (var noteRow in noteRows)
            {
                DeleteNoteRow(noteRow);
            }
        }

        public async void DeleteNoteRow(NoteRowViewModel noteRow)
        {
            await _dataService.DeleteAsync(noteRow.OriginalObject);
        }

        public void Clear()
        {
            Clients = null;
        }

        public int GetMaxNoteHeaderId()
        {
            if (NoteHeaders == null || NoteHeaders.Count == 0)
            {
                return 0;
            }

            return NoteHeaders.Max(x => x.Id);
        }

        public NoteRowViewModel AddNoteRowPriceWeight(NoteRowViewModel row)
        {
            var noteRow = row;

            var linen = LinenLists?.FirstOrDefault(x => x.Id == noteRow.LinenListId);
            if (linen != null)
            {
                noteRow.Weight = linen.Weight;
                noteRow.PriceUnit = linen.UnitId;

                switch (noteRow.ServiceTypeId)
                {
                    case (int) ServiceTypeEnum.Laundry:
                        noteRow.Price = linen.Laundry;
                        break;

                    case (int) ServiceTypeEnum.DryCleaning:
                        noteRow.Price = linen.DryCleaning;
                        break;

                    case (int) ServiceTypeEnum.Pressing:
                        noteRow.Price = linen.Pressing;
                        break;
                }
            }

            return noteRow;
        }

    }
}
