using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.Settings.ViewModel.Common;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.Common.Window;

namespace PALMS.Settings.ViewModel.Windows
{
    public class ReadTagWindowViewModel :ViewModelBase, IWindowDialogViewModel
    {
        private readonly IDialogService _dialogService;
        public Action<bool> CloseAction { get; set; }
        private bool IsSelected { get; set; }
        private ConcurrentDictionary<int, ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>>> _data =
            new ConcurrentDictionary<int, ConcurrentDictionary<string, Tuple<DateTime?, DateTime?>>>();

        private List<string> _tags;
        private string _selectedTag;
        private RfidCommon _impinj;
        private Task _runningTask;

        public Task RunningTask
        {
            get => _runningTask;
            set => Set(() => RunningTask, ref _runningTask, value);
        }
        public RfidCommon Impinj
        {
            get => _impinj;
            set => Set(() => Impinj, ref _impinj, value);
        }
        public string SelectedTag
        {
            get => _selectedTag;
            set => Set(() => SelectedTag, ref _selectedTag, value);
        }
        public List<string> Tags
        {
            get => _tags;
            set => Set(() => Tags, ref _tags, value);
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand StartReadCommand { get; }
        public RelayCommand StopReadCommand { get; }
        public RelayCommand<object> ShowAntennaTagCommand { get; }


        public async Task InitializeAsync()
        {
            IsSelected = false;
        }

        public ReadTagWindowViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            AddCommand = new RelayCommand(Add);
            CloseCommand = new RelayCommand(Close);

            StartReadCommand = new RelayCommand(StartRead);
            StopReadCommand = new RelayCommand(StopRead);

            ShowAntennaTagCommand = new RelayCommand<object>(SHowAntennaTags);

            Impinj = new RfidCommon();
            IsSelected = false;
        }

        public void SHowAntennaTags(object antennaNumb)
        {
            var antenna = int.Parse(antennaNumb.ToString());
            Tags = _data[antenna].Keys.ToList();
        }

        private void StartRead()
        {
           RunningTask = Task.Factory.StartNew(Impinj.StartRead);
        }

        private void StopRead()
        {
            _data = Impinj.StopRead();
        }

        public void Add()
        {
            if (SelectedTag == null) return;

            if (!_dialogService.ShowQuestionDialog($"Use \"{SelectedTag}\" Department ? "))
                return;
            CloseAction?.Invoke(true);
        }

        public void Close()
        {
            if (_dialogService.ShowQuestionDialog("Do you want to Save changes and close window ? "))
            {
                CloseAction?.Invoke(IsSelected);
            }
        }

        public string GetSelectedTag()
        {
            return SelectedTag;
        }

    }
}
