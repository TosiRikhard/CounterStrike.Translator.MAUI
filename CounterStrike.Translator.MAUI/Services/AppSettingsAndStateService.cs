using CounterStrike.Translator.MAUI.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CounterStrike.Translator.MAUI.Services;

public class AppSettingsAndStateService
{
    private AppSettings _appSettings;
    public bool UserConfiguratorShown;

        public AppSettingsAndStateService(IConfiguration configuration)
        {
            _appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
        }

        public AppSettings GetSettings()
        {
            return _appSettings;
        }

        public async Task SaveSettings(AppSettings newSettings)
        {
            _appSettings = newSettings;

            var userSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "userSettings.json");
            var appSettingsObject = new { AppSettings = _appSettings };
            var userSettingsJson = JsonConvert.SerializeObject(appSettingsObject, Formatting.Indented);

            await File.WriteAllTextAsync(userSettingsPath, userSettingsJson);
        }
}
