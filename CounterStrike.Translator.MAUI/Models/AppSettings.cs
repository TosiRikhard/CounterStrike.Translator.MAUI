using Newtonsoft.Json;

namespace CounterStrike.Translator.MAUI.Models
{
    public class AppSettings
    {
        public bool IsFirstRun { get; set; }
        public bool ShowUserConfiguratorAtStartup { get; set; }
        public TranslatingOptions TranslatingOptions { get; set; }
    }


}