using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CounterStrike.Translator.MAUI.Models;

namespace CounterStrike.Translator.MAUI.Services;
public class TranslatingService
{
    private readonly TranslationManager _translationManager;
    private readonly AppSettingsAndStateService _appSettingsAndStateService;
    public TranslatingService(TranslationManager translationManager, AppSettingsAndStateService appSettingsAndStateService)
    {
        _translationManager = translationManager;
        _appSettingsAndStateService = appSettingsAndStateService;
    }

    public async Task<Translation> TranslateMessage(string message)
    {
        var settings = _appSettingsAndStateService.GetSettings();
        var translatedMessage = await _translationManager.Translate(message, settings.TranslatingOptions.LanguageToTranslateTo);
        return translatedMessage;
    }
}
