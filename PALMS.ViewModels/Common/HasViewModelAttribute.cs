using System;

namespace PALMS.ViewModels.Common
{
    public class HasViewModelAttribute : Attribute
    {
        public Type ViewModelType { get; }

        public HasViewModelAttribute(Type viewModelType)
        {
            ViewModelType = viewModelType;
        }
    }
}
