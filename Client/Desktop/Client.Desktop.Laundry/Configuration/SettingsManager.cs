using System;
using System.IO;
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
            _filePath = GetLocalFilePath(fileName);
        }

        /// <summary>
        ///     Used application directory.
        /// </summary>
        public SettingsManager()
        {
            _filePath = GetApplicationFilePath("settings.json");
        }

        private string GetLocalFilePath(string fileName)
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, fileName);
        }

        private string GetApplicationFilePath(string fileName)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), fileName);
        }

        public T LoadSettings() =>
            File.Exists(_filePath) ?
                JsonConvert.DeserializeObject<T>(File.ReadAllText(_filePath)) :
                null;

        public void SaveSettings(T settings)
        {
            string json = JsonConvert.SerializeObject(settings);
            File.WriteAllText(_filePath, json);
        }
    }
}
