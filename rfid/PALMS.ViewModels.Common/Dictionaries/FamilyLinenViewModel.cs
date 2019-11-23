using PALMS.Data.Objects.LinenModel;

namespace PALMS.ViewModels.Common.Dictionaries
{
    public class FamilyLinenViewModel : DictionaryItemViewModel<FamilyLinen>
    {
        public FamilyLinenViewModel(FamilyLinen familyLinen) : base(familyLinen)
        {
        }

        public FamilyLinenViewModel()
        {

        }

        public override bool HasChanges => base.HasChanges || Name != OriginalObject.Name;

        protected override string Validate(string columnName)
        {
            return base.Validate(columnName);
        }
    }
}
