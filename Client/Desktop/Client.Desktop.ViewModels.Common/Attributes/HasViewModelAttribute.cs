using System;

namespace Client.Desktop.ViewModels.Common.Attributes
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
