using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Client.Desktop.Laundry.Configuration
{
    public class SettingsManager<T> where T : class
    {
        private readonly string _filePath;

        /// <summary>
        ///     Used application directory.
        /// </summary>
        /// <param name="fileName"></param>
        public SettingsManager(string fileName)
        {
            _filePath = GetApplicationFilePath(fileName);
        }

        /// <summary>
        ///     Used local directory.
        /// </summary>
        public SettingsManager()
        {
            _filePath = GetLocalFilePath("settings.json");
        }

        private string GetApplicationFilePath(string fileName)
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, fileName);
        }

        private string GetLocalFilePath(string fileName)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), fileName);
        }

        public T LoadSettings()
        {
            return File.Exists(_filePath) ? JsonConvert.DeserializeObject<T>(File.ReadAllText(_filePath)) : null;
        }

        public async Task<T> LoadSettingsAsync()
        {
            if (!File.Exists(_filePath))
                return default;

            var data = await File.ReadAllTextAsync(_filePath);
            if (string.IsNullOrEmpty(data)) return default;

            //data = Encoding.UTF8.GetString(Convert.FromBase64String(data));

            return JsonConvert.DeserializeObject<T>(data);
        }

        public void SaveSettings(T settings)
        {
            string json = JsonConvert.SerializeObject(settings);
            File.WriteAllText(_filePath, json);
        }

        public async Task SaveSettingsAsync(T settings)
        {
            string json = JsonConvert.SerializeObject(settings);
            await File.WriteAllTextAsync(_filePath, json);
        }
    }
}
