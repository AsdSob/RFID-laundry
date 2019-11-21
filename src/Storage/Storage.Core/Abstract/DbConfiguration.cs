namespace Storage.Core.Abstract
{
    public class DbConfiguration : IDbConfiguration
    {
        public string ConnectionString { get; set; }
    }
}