using PALMS.Data.Objects.LinenModel;
using PALMS.Settings.ViewModel.Dictionaries.Base;
using PALMS.ViewModels.Common.Dictionaries;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Settings.ViewModel.Dictionaries
{
    public class GroupLinenDictionaryViewModel : DictionaryViewModel<GroupLinen, GroupLinenViewModel>
    {
        public override string Name => "Group Linen";

        public GroupLinenDictionaryViewModel(IDataService dataService, IDispatcher dispatcher, IDialogService dialogService)
            : base(dataService, dispatcher, dialogService)
        {
        }

        protected override GroupLinenViewModel GetItem(GroupLinen entity)
        {
            return new GroupLinenViewModel(entity);
        }
    }
}
