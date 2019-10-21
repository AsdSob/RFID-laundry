using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PALMS.ViewModels.Common;

namespace PALMS.Invoices.ViewModel
{
    public class TabViewModel : ViewModelBase, ISettingsViewModel, IInitializationAsync, IClear
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
            if (Content is IInitializationAsync content)
                await content.InitializeAsync();
        }

        public void Clear()
        {
            (Content as IClear)?.Clear();
        }
    }
}
