using Autofac;
using Storage.Core.Abstract;
using Storage.Laundry;

namespace Storage.Module
{
    public class LaundryDataModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LaundryDbContextFactory>()
                .As<IDbContextFactory>()
                .OnActivated(x =>
                {
                    using (x.Instance.Create()){} // create db
                })
                .AutoActivate()
                .SingleInstance();
        }
    }
}