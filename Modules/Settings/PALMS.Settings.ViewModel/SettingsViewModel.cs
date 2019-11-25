using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Settings.ViewModel
{
    public class SettingsViewModel : ViewModelBase, IInitializationAsync
    {
        private readonly ICanExecuteMediator _canExecuteMediator;

        private ISettingsViewModel _content;

        public ISettingsViewModel Content
        {
            get => _content;
            set => Set(ref _content, value);
        }

        public SettingsViewModel(ISettingsViewModel content, ICanExecuteMediator canExecuteMediator)
        {
            _canExecuteMediator = canExecuteMediator ?? throw new ArgumentNullException(nameof(canExecuteMediator));

            Content = content;
        }

        public async Task InitializeAsync()
        {
            _canExecuteMediator.CanExecute = () => !Content.HasChanges();

            if (Content is IInitializationAsync content) await content.InitializeAsync();
        }
    }

    public interface ISettingsViewModel
    {
        bool HasChanges();
    }
}