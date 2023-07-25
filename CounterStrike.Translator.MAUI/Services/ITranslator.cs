using CounterStrike.Translator.MAUI.Models;
namespace CounterStrike.Translator.MAUI.Services;

public interface ITranslator
{
    Task<Translation> TranslateAsync(string sourceText, string lang);
}