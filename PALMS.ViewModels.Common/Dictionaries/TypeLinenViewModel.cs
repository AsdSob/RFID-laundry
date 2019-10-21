using PALMS.Data.Objects.LinenModel;

namespace PALMS.ViewModels.Common.Dictionaries
{
    public class TypeLinenViewModel : DictionaryItemViewModel<LinenType>
    {
        public TypeLinenViewModel(LinenType linenType) : base(linenType)
        {
        }

        public TypeLinenViewModel()
        {

        }

        public override bool HasChanges => base.HasChanges || Name != OriginalObject.Name;

        protected override string Validate(string columnName)
        {
            return base.Validate(columnName);
        }
    }
}
