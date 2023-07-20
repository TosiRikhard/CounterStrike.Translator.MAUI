using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using CounterStrike.Translator.MAUI.Models;
using HtmlAgilityPack;
using Microsoft.Win32;
using Path = System.IO.Path;

namespace CounterStrike.Translator.MAUI.Services;

public class SteamUserService
{

    private static readonly HttpClient HttpClient = new();

    public SteamUserService()
    {
    }

    public async Task<List<SteamUser>> GetSteamUsers()
    {
        var steamUserDataPaths = GetSteamLibraryPaths();
        var steamUsers = await GetSteamUsersWithCounterStrike(steamUserDataPaths);
        return steamUsers;
    }

    // This method fetches the Steam library paths that has app id 730 (CS:GO) installed
    public List<string> GetSteamLibraryPaths()
    {
        string libraryFoldersPath;
        try
        {
            libraryFoldersPath = GetLibraryFoldersPath();
        }
        catch (Exception ex)
        {
            // Handle exceptions such as registry key not found
            Console.WriteLine($"Error: {ex.Message}");
            return new List<string>();
        }

        var paths = new List<string>();
        string? currentPath = null;

        using var lineIterator = File.ReadLines(libraryFoldersPath).GetEnumerator();

        while (lineIterator.MoveNext())
        {
            var line = lineIterator.Current;
            if (currentPath == null && line.Contains("\"path\""))
            {
                var match = Regex.Match(line, "\"path\"\\s+\"([^\"]+)\"");
                if (match.Success)
                {
                    currentPath = match.Groups[1].Value;
                }
            }
            else if (currentPath != null && line.Contains("\"apps\""))
            {
                while (lineIterator.MoveNext() && !lineIterator.Current.Contains('}'))
                {
                    line = lineIterator.Current;
                    if (!line.Contains("\"730\"")) continue;
                    var userDataPath = Path.Combine(currentPath, "userdata");
                    paths.Add(userDataPath);
                    break;
                }
                currentPath = null;
            }
        }

        return paths;
    }

    // This method fetches the libraryfolders.vdf path from the registry
    private string GetLibraryFoldersPath()
    {
        var steamPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null)?.ToString();
        if (string.IsNullOrEmpty(steamPath))
        {
            throw new Exception("SteamPath registry key not found.");
        }

        var steamAppsPath = Path.Combine(steamPath, "steamapps");
        return Path.Combine(steamAppsPath, "libraryfolders.vdf");
    }

    public async Task<List<SteamUser>> GetSteamUsersWithCounterStrike(List<string> steamUserDataPaths)
    {
        // Get all the directories in all of the userdata paths
        var steamUserDirectories = steamUserDataPaths.SelectMany(Directory.GetDirectories).ToList();

        // Create a list to hold all the tasks
        var tasks = new List<Task>();

        // Check each steamUserDirectory for the existence of the folder 730 (CS:GO)
        var steamUsersWithCounterStrike = new ConcurrentBag<SteamUser>();
        foreach (var steamUserDirectory in steamUserDirectories)
        {
            var counterStrikePath = Path.Combine(steamUserDirectory, "730");
            if (!Directory.Exists(counterStrikePath)) continue;

            var steamId = long.Parse(Path.GetFileName(steamUserDirectory));
            var steamId64 = 76561197960265728L + steamId;

            // Get the current launch options for CS:GO
            var localConfigPath = Path.Combine(steamUserDirectory, "config", "localconfig.vdf");
            var currentLaunchOptions = GetLaunchOptions(localConfigPath, "730");

            var steamUser = new SteamUser
            {
                SteamId = steamId,
                SteamId3 = $"[U:1:{steamId}]",
                SteamId64 = steamId64,
                ProfileUrl = $"https://steamcommunity.com/profiles/{steamId64}",
                CurrentLaunchOptions = currentLaunchOptions,
                TranslatingEnabled = currentLaunchOptions.Contains("-netconport 2121")
            };

            // Create a new task to get the avatar and name for this steam user
            tasks.Add(Task.Run(async () =>
            {
                var (avatarUrl, name) = await GetAvatarAndNameAsync($"https://steamcommunity.com/profiles/{steamId64}");

                // Add a delay before making the next request
               // await Task.Delay(TimeSpan.FromSeconds(1)); // 1 second delay

                // Get avatar as memory stream
                try
                {
                    var avatarData = await HttpClient.GetByteArrayAsync(avatarUrl);
                    var avatarMemoryStream = new MemoryStream(avatarData);
                    steamUser.ProfileName = name;
                    steamUsersWithCounterStrike.Add(steamUser);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }));
        }

        // Wait for all the tasks to complete
        await Task.WhenAll(tasks);

        return steamUsersWithCounterStrike.ToList();
    }

    public async Task<(string, string)> GetAvatarAndNameAsync(string url)
    {
        try
        {
            var response = await HttpClient.GetAsync(url);
            var html = await response.Content.ReadAsStringAsync();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Find the avatar image, avatar frame image, and user's name in one pass
            var avatarNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'playerAvatarAutoSizeInner')]/img[last()]");
            var userNameNode = doc.DocumentNode.SelectSingleNode("//span[contains(@class, 'actual_persona_name')]");

            // Using null coalescing to simplify the code
            var avatarUrl = avatarNode?.GetAttributeValue("src", string.Empty) ?? string.Empty;
            var userName = userNameNode?.InnerText ?? string.Empty;

            return (avatarUrl, userName);
        }
        catch (Exception ex)
        {
            // Handle or log the exception as needed
            // ...
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public string GetLaunchOptions(string vdfPath, string appId)
    {
        var launchOptionsPattern = new Regex("\"LaunchOptions\"\\s+\"([^\"]+)\"", RegexOptions.Compiled);

        using var lineIterator = File.ReadLines(vdfPath).GetEnumerator();

        if (GetNextLineContaining(lineIterator, "\"apps\"") == null) return "";

        // Move iterator until we hit the line with the appid
        if (GetNextLineContaining(lineIterator, $"\"{appId}\"") == null) return "";

        // Now we are inside the app's block, we need to keep track of our level
        var level = 1;
        string? line;
        while (level > 0 && (line = GetNextLine(lineIterator)) != null)
        {
            level += line.Count(c => c == '{');
            level -= line.Count(c => c == '}');

            if (!line.Contains("\"LaunchOptions\"", StringComparison.Ordinal)) continue;

            var match = launchOptionsPattern.Match(line);
            string? currentLaunchOptions = null;
            currentLaunchOptions = match.Success ? match.Groups[1].Value : string.Empty;
            return currentLaunchOptions;
        }

        return string.Empty;
    }

    private static string? GetNextLine(IEnumerator<string> lineIterator)
    {
        return lineIterator.MoveNext() ? lineIterator.Current : null;
    }

    private static string? GetNextLineContaining(IEnumerator<string> lineIterator, string searchString)
    {
        while (lineIterator.MoveNext())
        {
            var line = lineIterator.Current;
            if (line.Contains(searchString, StringComparison.Ordinal))
            {
                return line;
            }
        }
        return null;
    }

    public void SetLaunchOptions(string vdfPath, string appId, string newOptions)
    {
        string tempFile = Path.GetTempFileName();
        using var lineIterator = File.ReadLines(vdfPath).GetEnumerator();
        using var writer = new StreamWriter(tempFile);

        string? line;
        while ((line = GetNextLineContaining(lineIterator, "\"apps\"")) != null)
        {
            writer.WriteLine(line);

            if ((line = GetNextLineContaining(lineIterator, appId)) != null)
            {
                writer.WriteLine(line);

                line = GetNextLineContaining(lineIterator, "\"LaunchOptions\"");
                if (line != null)
                {
                    // Replace the line with the launch options
                    writer.WriteLine($"\"LaunchOptions\" \"{newOptions}\"");
                    // Skip the original launch options line
                    continue;
                }
                else
                {
                    // App block exists, but no LaunchOptions. Add one.
                    writer.WriteLine($"\"LaunchOptions\" \"{newOptions}\"");
                    line = lineIterator.Current; // make sure to write the line that was read but not matched
                }
            }

            writer.WriteLine(line);
        }

        // Write the rest of the file
        while (lineIterator.MoveNext())
        {
            writer.WriteLine(lineIterator.Current);
        }

        writer.Close();
        File.Delete(vdfPath);
        File.Move(tempFile, vdfPath);
    }


    public async Task UpdateSteamUserLaunchOptions(SteamUser steamUser)
    {
        throw new NotImplementedException();
    }
}
