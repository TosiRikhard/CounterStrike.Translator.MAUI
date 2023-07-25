using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrike.Translator.MAUI.Models;
public class TranslatingOptions
{
    public bool TranslateOnlyEnemyTeam { get; set; }
    public bool IgnoreOwnMessages { get; set; }
    public List<string> IgnoreLanguages { get; set; }
    public string LanguageToTranslateTo { get; set; }
    public string OwnProfileName { get; set; }
    public bool SendTranslationToTeamChat { get; set; }
}