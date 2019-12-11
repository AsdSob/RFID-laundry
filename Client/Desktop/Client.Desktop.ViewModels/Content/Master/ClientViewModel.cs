using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;

namespace Client.Desktop.ViewModels.Content.Master
{
    public class ClientViewModel : ViewModelBase
    {
        private readonly ILaundryService _dataService;
        private readonly IDialogService _dialogService;
        private readonly IResolver _resolverService;

        public ClientViewModel()
        {

        }
    }
}
