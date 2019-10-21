using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Services;

namespace PALMS.LinenList.ViewModel
{
    public class LinenListViewModel : ViewModelBase, IInitializationAsync
    {
        private readonly ICanExecuteMediator _canExecuteMediator;

        private object _content;

        public object Content
        {
            get => _content;
            set => Set(ref _content, value);
        }

        public LinenListViewModel (LinensListViewModel content, ICanExecuteMediator canExecuteMediator)
        {
            _canExecuteMediator = canExecuteMediator ?? throw new ArgumentNullException(nameof(canExecuteMediator));

            Content = content;
        }

        public async Task InitializeAsync()
        {
            _canExecuteMediator.CanExecute = null;

            if (Content is IInitializationAsync content) await content.InitializeAsync();
        }
    }
}
