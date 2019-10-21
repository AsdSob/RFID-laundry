using PALMS.Data.Objects.LinenModel;
using PALMS.Settings.ViewModel.Dictionaries.Base;
using PALMS.ViewModels.Common.Dictionaries;
using PALMS.ViewModels.Common.Services;

namespace PALMS.Settings.ViewModel.Dictionaries
{
    public class TypeLinenDictionaryViewModel :DictionaryViewModel<LinenType, TypeLinenViewModel>
    {
        public override string Name => "Type Linen";

        public TypeLinenDictionaryViewModel(IDataService dataService, IDispatcher dispatcher, IDialogService dialogService)
            : base(dataService, dispatcher, dialogService)
        {
        }

        protected override TypeLinenViewModel GetItem(LinenType entity)
        {
            return new TypeLinenViewModel(entity);
        }
    }
}
