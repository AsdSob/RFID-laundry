using Client.Desktop.ViewModels.Common.Services;
using Newtonsoft.Json;

namespace Client.Desktop.ViewModels.Services
{
    public class JsonSerializer : ISerializer
    {
        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public T Deserialize<T>(string data)
        {
            if (string.IsNullOrEmpty(data)) return default;

            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
