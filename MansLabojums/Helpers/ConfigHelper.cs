using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace MansLabojums.Helpers
{
    public static class ConfigHelper
    {
        private static readonly string configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "config.json");

        public static async Task<AppConfig> LoadConfigAsync()
        {
            if (!File.Exists(configFilePath))
            {
                var defaultConfig = new AppConfig();
                await SaveConfigAsync(defaultConfig);
                return defaultConfig;
            }

            var json = await File.ReadAllTextAsync(configFilePath);
            var config = JsonSerializer.Deserialize<AppConfig>(json);
            return config ?? new AppConfig(); // Ensure a non-null value is returned
        }

        public static async Task SaveConfigAsync(AppConfig config)
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(configFilePath, json);
        }
    }

    public class AppConfig
    {
        public string ApiUrl { get; set; } = "https://api.example.com";
        public string ApiKey { get; set; } = "default_api_key";
        public bool EnableLogging { get; set; } = true;
    }
}


