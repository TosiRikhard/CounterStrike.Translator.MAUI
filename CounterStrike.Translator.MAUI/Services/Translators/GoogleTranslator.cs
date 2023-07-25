using System.Web;
using CounterStrike.Translator.MAUI.Models;
using Newtonsoft.Json.Linq;
using Windows.Media.Protection.PlayReady;

namespace CounterStrike.Translator.MAUI.Services.Translators;

public class GoogleTranslator : ITranslator
{
    private static readonly string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl={1}&dt=t&q={0}";
    private static readonly HttpClient client = new HttpClient();
    private readonly AppSettingsAndStateService _appSettingsAndStateService;

    public GoogleTranslator(AppSettingsAndStateService appSettingsAndStateService)
    {
        _appSettingsAndStateService = appSettingsAndStateService;
        _appSettingsAndStateService.CurrentApiMaxCalls = 100;

    }

    public async Task<Translation> TranslateAsync(string sourceText, string lang)
    {

        var fullUrl = EncodeUrl(url, sourceText, lang);

        try
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
            var response = await client.GetStringAsync(fullUrl);
            var data = JArray.Parse(response);
            var translatedText = data[0][0][0].Value<string>();
            var detectedSourceLang = data[2].Value<string>();
            return new Translation(lang, translatedText, detectedSourceLang);
        }
        catch (HttpRequestException e)
        {
            return new Translation(lang, e.Message, null);
        }
    }

    protected string EncodeUrl(string url, string sourceText, string lang)
    {
        return string.Format(url, HttpUtility.UrlEncode(sourceText), lang);
    }
}