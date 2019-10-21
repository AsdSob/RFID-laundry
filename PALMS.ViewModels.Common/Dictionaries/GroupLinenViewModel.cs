using PALMS.Data.Objects.LinenModel;

namespace PALMS.ViewModels.Common.Dictionaries
{
    public class GroupLinenViewModel : DictionaryItemViewModel<GroupLinen>
    {
        private string _description;
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        protected override void Update(GroupLinen originalObject)
        {
            base.Update(originalObject);
            Description = OriginalObject.Description;
        }

        public override void AcceptChanges()
        {
            base.AcceptChanges();
            OriginalObject.Description = Description;
        }

        public GroupLinenViewModel(GroupLinen groupLinen) : base(groupLinen)
        {
        }

        public GroupLinenViewModel()
        {
        }

        public override bool HasChanges => base.HasChanges || Name != OriginalObject.Name ||
                                           Description != OriginalObject.Description;

        protected override string Validate(string columnName)
        {
            return base.Validate(columnName);
        }
    }
}
