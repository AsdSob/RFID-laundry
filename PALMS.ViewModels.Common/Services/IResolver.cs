using System;

namespace PALMS.ViewModels.Common.Services
{
    public interface IResolver
    {
        T Resolve<T>();
        object Resolve(Type type);
    }
}
