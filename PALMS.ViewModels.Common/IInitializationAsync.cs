using System.Collections.Generic;
using System.Threading.Tasks;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.LinenModel;
using PALMS.ViewModels.Common.Enumerations;

namespace PALMS.ViewModels.Common
{
    public interface IInitializationAsync
    {
        Task InitializeAsync();
    }
}