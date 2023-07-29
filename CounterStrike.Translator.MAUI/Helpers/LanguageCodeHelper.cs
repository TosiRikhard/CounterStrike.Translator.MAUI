﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrike.Translator.MAUI.Helpers;
public static class LanguageCodeHelper
{
    private static readonly HashSet<string> ValidLanguageCodes = new()
    {
        "af", "ak", "am", "ar", "as", "ay", "az", "be", "bg", "bho", "bm", "bn", "bs", "ca", "ceb", "ckb", "co", "cs", "cy",
        "da", "de", "doi", "dv", "ee", "el", "en", "eo", "es", "et", "eu", "fa", "fi", "fil", "fr", "fy", "ga", "gd", "gl",
        "gn", "gom", "gu", "ha", "haw", "he", "hi", "hmn", "hr", "ht", "hu", "hy", "id", "ig", "ilo", "is", "it", "ja", "jv",
        "ka", "kk", "km", "kn", "ko", "kri", "ku", "ky", "la", "lb", "lg", "ln", "lo", "lt", "lus", "lv", "mai", "mg", "mi",
        "mk", "ml", "mn", "mni-Mtei", "mr", "ms", "mt", "my", "ne", "nl", "no", "nso", "ny", "om", "or", "pa", "pl", "ps", "pt",
        "qu", "ro", "ru", "rw", "sa", "sd", "si", "sk", "sl", "sm", "sn", "so", "sq", "sr", "st", "su", "sv", "sw", "ta",
        "te", "tg", "th", "ti", "tk", "tl", "tr", "ts", "tt", "ug", "uk", "ur", "uz", "vi", "xh", "yi", "yo", "zh-CN",
        "zh-TW", "zu"
    };

    public static bool IsLanguageCodeValid(string languageCode)
    {
        return ValidLanguageCodes.Contains(languageCode.ToLower());
    }
}
