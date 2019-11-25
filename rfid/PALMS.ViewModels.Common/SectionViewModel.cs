using GalaSoft.MvvmLight;

namespace PALMS.ViewModels.Common
{
    public abstract class SectionViewModel<T> : ViewModelBase, ISection where T : ViewModelBase
    {
        public abstract int Index { get; }

        public abstract string Name { get; }

        public bool IsVisible { get; set; } = true;

        public bool IsEnable { get; set; } = true;

        public abstract string Image { get; }
    }
}
