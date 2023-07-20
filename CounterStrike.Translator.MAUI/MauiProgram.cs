using System.Reflection;
using Microsoft.Extensions.Logging;
using CounterStrike.Translator.MAUI.Data;
using Microsoft.Extensions.Configuration;

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

		builder.Services.AddSingleton<WeatherForecastService>();
        //builder.Services.AddTransient<MainPage>();

        return builder.Build();
	}
}
