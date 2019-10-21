using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Data.Audit;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Settings.ViewModel.Audit
{
    public class AuditHistoryViewModel : ViewModelBase, ISettingsContent, IInitializationAsync
    {
        private readonly IDataService _dataService;
        private readonly IDispatcher _dispatcher;
        private readonly IDialogService _dialogService;
        private List<AuditModel> _auditModels;
        private AuditModel _selectedAuditModel;
        private int _noteId;
        private int _noteRowId;
        public string Name => "Audit";


        public int NoteRowId
        {
            get => _noteRowId;
            set => Set(ref _noteRowId, value);
        }
        public int NoteId
        {
            get => _noteId;
            set => Set(ref _noteId, value);
        }
        public AuditModel SelectedAuditModel
        {
            get => _selectedAuditModel;
            set => Set(ref _selectedAuditModel, value);
        }

        public List<AuditModel> AuditModels
        {
            get => _auditModels;
            set => Set(ref _auditModels, value);
        }

        public ObservableCollection<AuditModelProperty> AuditModelProperties =>
            SelectedAuditModel?.AuditModelProperties.ToObservableCollection();

        public RelayCommand GetAuditCommand { get; }

        public AuditHistoryViewModel(IDataService dataService, IDispatcher dispatcher, IDialogService dialogService)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            GetAuditCommand = new RelayCommand(GetAudit);


            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedAuditModel))
            {
                RaisePropertyChanged(() => AuditModelProperties);
            }
        }

        public bool HasChanges()
        {
            return false;
        }

        public async Task InitializeAsync()
        {

        }




        public async void GetAudit()
        {
            _dialogService.ShowBusy();

            try
            {
                var auditModel = await _dataService.GetAsync<AuditModel>();
                var auditModels = auditModel;
                _dispatcher.RunInMainThread(() => AuditModels = auditModels.ToList());
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
    }
}
