using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Windows;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Content
{
    public class MasterLinenViewModel :ViewModelBase
    {
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;
        private readonly IResolver _resolverService;

        private ObservableCollection<MasterLinenEntity> _masterLinens;
        private MasterLinenEntity _selectedMasterLinen;

        public MasterLinenEntity SelectedMasterLinen
        {
            get => _selectedMasterLinen;
            set => Set(() => SelectedMasterLinen, ref _selectedMasterLinen, value);
        }
        public ObservableCollection<MasterLinenEntity> MasterLinens
        {
            get => _masterLinens;
            set => Set(() => MasterLinens, ref _masterLinens, value);
        }

        public RelayCommand EditCommand { get; }
        public RelayCommand NewCommand { get; }
        public RelayCommand DeleteMasterLinenCommand { get; }

        public MasterLinenViewModel(ILaundryService dataService, IDialogService dialogService, IResolver resolver)
        {
            _laundryService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));

            EditCommand = new RelayCommand(Edit,(()=> SelectedMasterLinen != null));
            NewCommand = new RelayCommand(AddMasterLinen);
            DeleteMasterLinenCommand = new RelayCommand(DeleteMasterLinen, (() => SelectedMasterLinen != null));

            MasterLinens = new ObservableCollection<MasterLinenEntity>();
            Task.Factory.StartNew(() => GetData());
        }

        private async Task GetData()
        {
            _dialogService.ShowBusy();

            try
            {
                var master = await _laundryService.GetAllAsync<MasterLinenEntity>();
                MasterLinens = master.ToObservableCollection();

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
                EditCommand.RaiseCanExecuteChanged();
            }
        }

        private void DeleteMasterLinen()
        {
            var masterLinen = SelectedMasterLinen;
            if(masterLinen == null) return;

            if (!_dialogService.ShowQuestionDialog($"Do you want to DELETE {masterLinen.Name} ?"))
                return;
            
            _laundryService.DeleteAsync(masterLinen);

            MasterLinens.Remove(masterLinen);
            SelectedMasterLinen = MasterLinens?.FirstOrDefault();
        }

        private void Edit()
        {
            MasterLinenWindow(new MasterLinenEntityViewModel(SelectedMasterLinen));
        }

        private void AddMasterLinen()
        {
            MasterLinenWindow(null);
        }

        private async void MasterLinenWindow(MasterLinenEntityViewModel masterLinen)
        {
            var masterLinenWindow = _resolverService.Resolve<MasterLinenWindowModel>();

            masterLinenWindow.SetSelectedLinen(masterLinen);

            if (_dialogService.ShowDialog(masterLinenWindow))
            {
                MasterLinens.Clear();

                await GetData();
            }
        }
    }
}
