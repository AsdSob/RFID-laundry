using Newtonsoft.Json;

namespace Client.Desktop.ViewModels.Common.Configuration
{
    public class AppConfiguration
    {
        [JsonProperty("dbConnectionString")]
        public string DbConnectionString { get; set; }
    }
}
