using System;

namespace Client.Desktop.ViewModels.Common.Services
{
    public interface IResolver
    {
        T Resolve<T>();
        object Resolve(Type type);
    }
}
