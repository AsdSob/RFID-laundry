using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Content
{
    public class DataViewModel : ViewModelBase
    {
        private string _text;

        public string Text
        {
            get => _text;
            set => Set(() => Text, ref _text, value);
        }

        //public DataViewModel()
        //{
        //    Text = "NEW Content of DataViewModel";
        //}

        public readonly ILaundryService _laundryService;

        public DataViewModel(ILaundryService dataService)
        {
            _laundryService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }


        public async Task<ObservableCollection<ClientEntityViewModel>> GetClients()
        {
            var client = await _laundryService.GetAllAsync<ClientEntity>();
            var clients = client.Select(x => new ClientEntityViewModel(x));
            return clients.ToObservableCollection();
        }

        public async Task<ObservableCollection<DepartmentEntityViewModel>> GetDepartments()
        {
            var department = await _laundryService.GetAllAsync<DepartmentEntity>();
            var departments = department.Select(x => new DepartmentEntityViewModel(x));
            return departments.ToObservableCollection();
        }

        public async Task<ObservableCollection<MasterLinenEntityViewModel>> GetMasterLinens()
        {
            var masterLinen = await _laundryService.GetAllAsync<MasterLinenEntity>();
            var masterLinens = masterLinen.Select(x => new MasterLinenEntityViewModel(x));
            return masterLinens.ToObservableCollection();
        }

        public async Task<ObservableCollection<ClientStaffEntityViewModel>> GetClientStaffs()
        {
            var staff = await _laundryService.GetAllAsync<ClientStaffEntity>();
            var staffs = staff.Select(x => new ClientStaffEntityViewModel(x));
            return staffs.ToObservableCollection();
        }

        public async Task<ObservableCollection<ClientLinenEntityViewModel>> GetLinens()
        {
            var linen = await _laundryService.GetAllAsync<ClientLinenEntity>();
            var linens = linen.Select(x => new ClientLinenEntityViewModel(x));
            return linens.ToObservableCollection();
        }
    }
}
