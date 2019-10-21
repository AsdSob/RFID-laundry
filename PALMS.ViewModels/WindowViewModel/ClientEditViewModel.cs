using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PALMS.ViewModels.Common;
using PALMS.ViewModels.Common.Enumerations;
using PALMS.ViewModels.Common.Extensions;
using PALMS.ViewModels.Common.Services;
using PALMS.ViewModels.EntityViewModel;
using UnitViewModel = PALMS.ViewModels.Common.Enumerations.UnitViewModel;

namespace PALMS.ViewModels
{
    public class ClientEditViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private ClientViewModel _client;
        private ObservableCollection<UnitViewModel> _ticketTemplates;
        private ObservableCollection<UnitViewModel> _noteTemplates;
        private ObservableCollection<UnitViewModel> _invoiceTemplates;

        public ClientViewModel Client
        {
            get => _client;
            set => Set(ref _client, value);
        }

        public ObservableCollection<UnitViewModel> TicketTemplates
        {
            get => _ticketTemplates;
            set => Set(ref _ticketTemplates, value);
        }
        public ObservableCollection<UnitViewModel> NoteTemplates
        {
            get => _noteTemplates;
            set => Set(ref _noteTemplates, value);
        }
        public ObservableCollection<UnitViewModel> InvoiceTemplates
        {
            get => _invoiceTemplates;
            set => Set(ref _invoiceTemplates, value);
        }

        public ClientEditViewModel(IDataService dataService)
        {
            _dataService = dataService;

            Client = new ClientViewModel();
        }

        public async Task InitializeAsync()
        {
            TicketTemplates = EnumExtentions.GetValues<LabelTypeEnum>().ToObservableCollection();
            NoteTemplates = EnumExtentions.GetValues<NoteTypeEnum>().ToObservableCollection();
            InvoiceTemplates = EnumExtentions.GetValues<InvoiceTypeEnum>().ToObservableCollection();

#if DEBUG
            if (Client.IsNew)
            {
                Client.ClientInfo.TicketId = TicketTemplates.FirstOrDefault()?.Id;
                Client.ClientInfo.NoteId = NoteTemplates.FirstOrDefault()?.Id;
                Client.ClientInfo.InvoiceId = InvoiceTemplates.FirstOrDefault()?.Id;
                Client.Active = true;
                Client.ClientInfo.ByCollectionDate = false;
                Client.ClientInfo.Start = 1;
                Client.ClientInfo.End = 31;
                Client.ClientInfo.Express = 1;
            }
#endif
        }

        public async Task SaveAsync()
        {
            await UpdateClientAsync();
        }

        private async Task UpdateClientAsync()
        {
            // client
            var client = Client.OriginalObject;
            if (Client.HasChanges())
            {
                Client.AcceptChanges();
                await _dataService.AddOrUpdateAsync(client);
            }

            // client info
            var clientInfo = Client.ClientInfo.OriginalObject;
            if (Client.ClientInfo.HasChanges())
            {
                Client.ClientInfo.AcceptChanges(client);
                await _dataService.AddOrUpdateAsync(clientInfo);
            }
            client.ClientInfo = clientInfo;

            // details
        }

        public void SetClient(ClientViewModel client)
        {
            Client = client;
        }
    }
}