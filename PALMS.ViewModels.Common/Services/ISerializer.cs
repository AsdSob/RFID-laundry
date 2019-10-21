namespace PALMS.ViewModels.Common.Services
{
    public interface ISerializer
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string data);
        T Deserialize<T>();
    }
}
