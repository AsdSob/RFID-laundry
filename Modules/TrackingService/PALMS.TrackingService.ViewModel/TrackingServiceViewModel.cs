using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;

namespace PALMS.TrackingService.ViewModel
{
    public class TrackingServiceViewModel : ViewModelBase, IInitializationAsync
    {
        private readonly ICanExecuteMediator _canExecuteMediator;

        private readonly IDialogService _dialogService;
        private readonly IResolver _resolverService;
        private object _content1;
        private object _content2;
        private object _content3;
        private ObservableCollection<object> _contents;
        public RelayCommand RefreshCommand { get; }

        public object Content3
        {
            get => _content3;
            set => Set(ref _content3, value);
        }
        public object Content2
        {
            get => _content2;
            set => Set(ref _content2, value);
        }
        public object Content1
        {
            get => _content1;
            set => Set(ref _content1, value);
        }

        public ObservableCollection<object> Contents
        {
            get => _contents;
            set => Set(ref _contents, value);
        }

        public TrackingServiceViewModel(ICanExecuteMediator canExecuteMediator, IResolver resolver, IDialogService dialogService)
        {
            _canExecuteMediator = canExecuteMediator ?? throw new ArgumentNullException(nameof(canExecuteMediator));
            RefreshCommand = new RelayCommand(Refresh);

            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _resolverService = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        private void Refresh()
        {
            InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            _canExecuteMediator.CanExecute = null;

            if (Content1 is IInitializationAsync content) await content.InitializeAsync();

            if (Content2 is IInitializationAsync content2) await content2.InitializeAsync();

            if (Content3 is IInitializationAsync content3) await content3.InitializeAsync();
        }
    }
}
