using System.Web;
using CounterStrike.Translator.MAUI.Models;

namespace CounterStrike.Translator.MAUI.Services.Translators;

public class LibreTranslator : ITranslator
{
    private static readonly string url = "https://libretranslate.de/translate?client=gtx&sl=auto&tl={1}&dt=t&q={0}";
    public async Task<Translation> TranslateAsync(string sourceText, string lang)
    {
        string fullUrl = EncodeUrl(url, sourceText, lang);
        // Do the translation using LibreTranslate and return result
        return new Translation(lang,"TEST", "Translated text");
    }

    protected string EncodeUrl(string url, string sourceText, string lang)
    {
        return string.Format(url, HttpUtility.UrlEncode(sourceText), lang);
    }
}