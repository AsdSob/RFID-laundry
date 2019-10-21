using System;
using GalaSoft.MvvmLight;
using PALMS.ViewModels.Common;
using System.Threading.Tasks;
using PALMS.ViewModels.Common.Services;

namespace PALMS.MasterLinens.ViewModel
{
    public class MasterLinensViewModel : ViewModelBase, IInitializationAsync
    {
        private readonly ICanExecuteMediator _canExecuteMediator;

        private object _content;

        public object Content
        {
            get => _content;
            set => Set(ref _content, value);
        }

        public MasterLinensViewModel(TypeLinenTabViewModel content, ICanExecuteMediator canExecuteMediator)
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
