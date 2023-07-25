using CounterStrike.Translator.MAUI.Models;
using CounterStrike.Translator.MAUI.Services.Translators;

namespace CounterStrike.Translator.MAUI.Services;

public class TranslationManager
{
    private readonly List<ITranslator> _translators;
    private readonly AppSettingsAndStateService _appSettingsAndStateService;

    public TranslationManager(IEnumerable<ITranslator> translators, AppSettingsAndStateService appSettingsAndStateService)
    {
        _appSettingsAndStateService = appSettingsAndStateService;
        _translators = new List<ITranslator>(translators);
        
        // Get current translator name
        var currentTranslatorName = _translators[_currentTranslator].GetType().Name;
        _appSettingsAndStateService.CurrentTranslationEngine = currentTranslatorName;
    }

    private int _currentTranslator = 0;

    public async Task<Translation> Translate(string sourceText, string lang)
    {
        foreach (var translator in _translators)
        {
            // Get the name of the current translator
            var currentTranslatorName = _translators[_currentTranslator].GetType().Name;
            _appSettingsAndStateService.CurrentTranslationEngine = currentTranslatorName;
            try
            {
                var result = await _translators[_currentTranslator].TranslateAsync(sourceText, lang);
                _appSettingsAndStateService.CurrentApiCalls++;
                return result;
            }
            catch (Exception e)
            {
                // switch to the next translator
                _currentTranslator = (_currentTranslator + 1) % _translators.Count;
                _appSettingsAndStateService.CurrentApiCalls = 1;
                // continue to the next iteration of the loop to try the next translator
            }
        }

        // if all translators fail, throw an exception or handle it accordingly
        throw new Exception("All translators failed.");
    }
}