namespace CounterStrike.Translator.MAUI.Models;

public class SteamUser
{
    public long SteamId { get; set; }
    public  string SteamId3 { get; set; }
    public  long SteamId64 { get; set; }
    public  string ProfileName { get; set; }
    public  string ProfileUrl { get; set; }
    public  string AvatarFrameUrl { get; set; }
    public string AvatarUrl { get; set; }
    public bool TranslatingEnabled { get; set; }
    public string CurrentLaunchOptions { get; set; } = string.Empty;
}
