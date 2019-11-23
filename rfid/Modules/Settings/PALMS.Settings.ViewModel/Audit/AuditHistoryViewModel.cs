using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PALMS.Data.Audit;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Settings.ViewModel.Audit
{
    public class AuditHistoryViewModel : ViewModelBase, ISettingsContent, IInitializationAsync
    {
        private readonly IDataService _dataService;
        private readonly IDispatcher _dispatcher;
        private ObservableCollection<AuditModel> _auditModels;
        private AuditModel _selectedAuditModel;
        public string Name => "Audit History";


        public AuditModel SelectedAuditModel
        {
            get => _selectedAuditModel;
            set => Set(ref _selectedAuditModel, value);
        }

        public ObservableCollection<AuditModel> AuditModels
        {
            get => _auditModels;
            set => Set(ref _auditModels, value);
        }

        public ObservableCollection<AuditModelProperty> AuditModelProperties =>
            SelectedAuditModel?.AuditModelProperties.ToObservableCollection();

        public AuditHistoryViewModel(IDataService dataService, IDispatcher dispatcher)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));

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
            if (AuditModels?.Any() == true) return;

            var auditModel = await _dataService.GetAsync<AuditModel>();
            var auditModels = auditModel;
            _dispatcher.RunInMainThread(() => AuditModels = auditModels.ToObservableCollection());
        }
    }
}
