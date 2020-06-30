using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.EntityViewModels;

namespace Client.Desktop.ViewModels.Common.Services
{
    public interface ILaundryService : IBaseService
    {
        Task<ObservableCollection<MasterLinenEntityViewModel>> MasterLinens();
        Task<ObservableCollection<ClientEntityViewModel>> Clients();
        Task<ObservableCollection<DepartmentEntityViewModel>> Departments();
        Task<ObservableCollection<ClientLinenEntityViewModel>> ClientLinens();
        Task<ObservableCollection<RfidReaderEntityViewModel>> RfidReaders();
        Task<ObservableCollection<RfidAntennaEntityViewModel>> RfidAntennas();
    }
}

