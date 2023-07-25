using System.Reflection;
using Microsoft.Extensions.Logging;
using CounterStrike.Translator.MAUI.Services;
using Microsoft.Extensions.Configuration;
using CounterStrike.Translator.MAUI.Services.Translators;

namespace CounterStrike.Translator.MAUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

        var a = Assembly.GetExecutingAssembly();
        using var stream = a.GetManifestResourceStream("CounterStrike.Translator.MAUI.appsettings.json");
        var config = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();
        builder.Configuration.AddConfiguration(config);

        // Load userSettings.json if it exists
        var userSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "userSettings.json");
        if (File.Exists(userSettingsPath))
        {
            var userConfig = new ConfigurationBuilder()
                .AddJsonFile(userSettingsPath)
                .Build();
            builder.Configuration.AddConfiguration(userConfig);
        }

        builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		builder.Services.AddSingleton<AppSettingsAndStateService>();
        builder.Services.AddSingleton<SteamUserService>();
        builder.Services.AddSingleton<TelnetConnection>();
        builder.Services.AddSingleton<TelnetService>();
        builder.Services.AddSingleton<TranslatingService>();
        builder.Services.AddSingleton<ITranslator, GoogleTranslator>();
        builder.Services.AddSingleton<ITranslator, LibreTranslator>();
        builder.Services.AddSingleton<TranslationManager>();
        
        return builder.Build();
    }
}
