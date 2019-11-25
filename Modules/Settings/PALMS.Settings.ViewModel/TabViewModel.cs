using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PALMS.ViewModels.Common;

namespace PALMS.Settings.ViewModel
{
    public class TabViewModel : ViewModelBase, ISettingsViewModel, IInitializationAsync
    {
        private ISettingsContent _content;
        public ISettingsContent Content
        {
            get => _content;
            set => Set(ref _content, value);
        }

        public string Name => Content?.Name;

        public TabViewModel(ISettingsContent content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public async Task InitializeAsync()
        {
            if (Content is IInitializationAsync content) await content.InitializeAsync();
        }

        public bool HasChanges()
        {
            return Content.HasChanges();
        }
    }
}