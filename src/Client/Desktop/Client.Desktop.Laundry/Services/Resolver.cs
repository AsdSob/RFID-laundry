using System;
using Autofac;
using Client.Desktop.Laundry.ViewModels;
using Client.Desktop.ViewModels.Common.Services;

namespace Client.Desktop.Laundry.Services
{
    public class Resolver : IResolver
    {
        public T Resolve<T>()
        {
            return ViewModelLocator.Container.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return ViewModelLocator.Container.Resolve(type);
        }
    }
}