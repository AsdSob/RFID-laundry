using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PALMS.ViewModels.Common.Services;

namespace PALMS.WPFClient.Services
{
    public class JsonSerializer : ISerializer
    {
        public JsonSerializer()
        {
        }

        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public T Deserialize<T>()
        {
            throw new System.NotImplementedException();
        }
    }
}
