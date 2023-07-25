using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrike.Translator.MAUI.Models;
public class Translation
{
    public string TranslatedTo { get; set; }
    public string TranslateFrom { get; set; }
    public string TranslatedText { get; set; }

    public string UserName { get; set; }

    public Translation(string targetLanguage, string translatedText, string detectedSourceLanguage)
    {
        TranslatedTo = targetLanguage;
        TranslatedText = translatedText;
        TranslateFrom = detectedSourceLanguage;
    }
}
