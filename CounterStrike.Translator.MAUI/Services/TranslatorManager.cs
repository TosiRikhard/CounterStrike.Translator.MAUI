using CounterStrike.Translator.MAUI.Models;
using CounterStrike.Translator.MAUI.Services.Translators;

namespace CounterStrike.Translator.MAUI.Services;

public class TranslationManager
{
    private readonly List<ITranslator> _translators;

    public TranslationManager(IEnumerable<ITranslator> translators)
    {
        _translators = new List<ITranslator>(translators);
    }

    private int _currentTranslator = 0;

    public async Task<Translation> Translate(string sourceText, string lang)
    {
        try
        {
            return await _translators[_currentTranslator].TranslateAsync(sourceText, lang);
        }
        catch (Exception e)
        {
            // switch to the next translator
            _currentTranslator = (_currentTranslator + 1) % _translators.Count;
            return await _translators[_currentTranslator].TranslateAsync(sourceText, lang);
        }
        // Handle other exceptions
    }
}