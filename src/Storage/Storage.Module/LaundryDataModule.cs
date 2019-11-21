using Autofac;
using Storage.Core.Abstract;
using Storage.Laundry;

namespace Storage.Module
{
    public class LaundryDataModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x =>
                {
                    //var options = x.Resolve<IAppConfiguration>();

                    return new DbConfiguration
                    {
                        ConnectionString = "Host=127.0.0.1;Port=5432;Database=laundry;User Id=postgres;Password=postgres;Timeout=100;Command Timeout=300;"
                    };
                })
                .As<IDbConfiguration>();

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