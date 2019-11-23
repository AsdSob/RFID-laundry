using PALMS.Data.Objects.LinenModel;
using PALMS.Settings.ViewModel.Dictionaries.Base;
using PALMS.ViewModels.Common.Dictionaries;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Settings.ViewModel.Dictionaries
{
    public class FamilyLinenDictionaryViewModel : DictionaryViewModel<FamilyLinen, FamilyLinenViewModel>
    {
        public override string Name => "Family Linen";

        public FamilyLinenDictionaryViewModel(IDataService dataService, IDispatcher dispatcher, IDialogService dialogService)
            : base(dataService, dispatcher, dialogService)
        {
        }

        protected override FamilyLinenViewModel GetItem(FamilyLinen entity)
        {
            return new FamilyLinenViewModel(entity);
        }
    }
}
