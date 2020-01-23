using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Content.Master
{
    public class MasterLinenViewModel :ViewModelBase
    {
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;

        private ObservableCollection<MasterLinenEntityViewModel> _masterLinens;
        private MasterLinenEntityViewModel _selectedMasterLinen;

        public MasterLinenEntityViewModel SelectedMasterLinen
        {
            get => _selectedMasterLinen;
            set => Set(() => SelectedMasterLinen, ref _selectedMasterLinen, value);
        }
        public ObservableCollection<MasterLinenEntityViewModel> MasterLinens
        {
            get => _masterLinens;
            set => Set(() => MasterLinens, ref _masterLinens, value);
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand AddMasterLinenCommand { get; }
        public RelayCommand DeleteMasterLinenCommand { get; }

        public MasterLinenViewModel(ILaundryService dataService, IDialogService dialogService)
        {
            _laundryService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            SaveCommand = new RelayCommand(Save);
            AddMasterLinenCommand = new RelayCommand(AddMasterLinen);
            DeleteMasterLinenCommand = new RelayCommand(DeleteMasterLinen, (() => SelectedMasterLinen != null));

            MasterLinens = new ObservableCollection<MasterLinenEntityViewModel>();
            Task.Factory.StartNew(() => GetData());
        }

        private async Task GetData()
        {
            _dialogService.ShowBusy();

            try
            {
                var master = await _laundryService.GetAllAsync<MasterLinenEntity>();
                var masters = master.Select(x => new MasterLinenEntityViewModel(x));
                MasterLinens = masters.ToObservableCollection();

            }
            catch (Exception e)
            {
                _dialogService.HideBusy();
            }

            finally
            {
                _dialogService.HideBusy();
            }

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedMasterLinen))
            {
                DeleteMasterLinenCommand.RaiseCanExecuteChanged();
            }
        }

        private void Save()
        {
            var masterLinens = MasterLinens.Where(x => x.HasChanges());

            foreach (var masterLinen in masterLinens)
            {
                masterLinen.AcceptChanges();

                _laundryService.AddOrUpdate(masterLinen.OriginalObject);
            }

            _dialogService.ShowInfoDialog("All changes saved");
        }

        private void DeleteMasterLinen()
        {
            var masterLinen = SelectedMasterLinen;
            if(masterLinen == null) return;

            if (!_dialogService.ShowQuestionDialog($"Do you want to DELETE {masterLinen.Name} ?"))
                return;
            
            _laundryService.Delete(masterLinen.OriginalObject);

            MasterLinens.Remove(masterLinen);
            SelectedMasterLinen = MasterLinens?.FirstOrDefault();
        }

        private void AddMasterLinen()
        {
            var masterLinen = new MasterLinenEntityViewModel()
            {
                PackingValue = 1,
            };

            MasterLinens.Add(masterLinen);
        }
    }
}
