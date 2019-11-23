using PALMS.Data.Objects.Tracking;
using PALMS.Settings.ViewModel.Dictionaries.Base;
using PALMS.ViewModels.Common.Dictionaries;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Settings.ViewModel.Dictionaries
{
    public class TrackingTypeDictionaryViewModel : DictionaryViewModel<TrackingType, TrackingTypeViewModel>
    {
        public override string Name => "Tracking Type";

        public TrackingTypeDictionaryViewModel(IDataService dataService, IDispatcher dispatcher, IDialogService dialogService)
            : base(dataService, dispatcher, dialogService)
        {
        }

        protected override TrackingTypeViewModel GetItem(TrackingType entity)
        {
            return new TrackingTypeViewModel(entity);
        }
    }
}

