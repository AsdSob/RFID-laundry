using PALMS.Data.Objects.Tracking;

namespace PALMS.ViewModels.Common.Dictionaries
{
    public class TrackingTypeViewModel : DictionaryItemViewModel<TrackingType>
    {
        public TrackingTypeViewModel(TrackingType trackingType) : base(trackingType)
        {
        }

        public TrackingTypeViewModel()
        {

        }

        public override bool HasChanges => base.HasChanges || Name != OriginalObject.Name;

        protected override string Validate(string columnName)
        {
            return base.Validate(columnName);
        }
    }
}
